﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using CommandLine;
using System.Reflection;
using System.Net;
using System.Text;
using System.Xml.XPath;
using System.Xml;

namespace CLARiNET
{
    class Program
    {
        static List<XElement> envs = null;
        static Options options = new Options();
        static string appDir = null;
        static string inboundDir = null;
        static string processedDir = null;
        static string[] files = null;
        static string searchPattern = "*.clar";
        static string host = "";
        static string cloudCollection = "";
        const string ccxUrl = "https://{host}/ccx";
        const string cloudRepoUrl = ccxUrl + "/cc-cloud-repo/collections/";

        static void Main(string[] args)
        {            
            string soapUrl = "";
            string restUrl = "";

            appDir = AppDomain.CurrentDomain.BaseDirectory;
            inboundDir = Path.Combine(appDir, "inbound");
            processedDir = Path.Combine(appDir, "processed");

            try
            {
                Console.WriteLine("\n** CLARiNET by Whitley Media **\n\n");

                InitDirectories(inboundDir, processedDir);

                if (!InitOptions(args))
                {
                    return;
                }

                if (options.Encrypt)
                {
                    OptionsEncrypt();
                    return;
                }

                if (options.PrintEnvironments)
                {
                    PrintEnvironments(envs);
                    return;
                }

                
                if (options != null)
                {
                    // Command
                    CommandOption();
                    CommandValidate();
                    // Path
                    PathOption();
                    PathValidate();
                    // Parameters
                    ParameterOption();
                    ParameterValidate();
                    // Files Check
                    FilesCheck();
                    // Environment
                    EnvironmentOption();
                    // Tenant
                    TenantOption();
                    // Username
                    UsernameOption();
                    // Password
                    PasswordOption();
                }

                if (options.PrintCommandline)
                {
                    Console.WriteLine("\n\n");
                    Console.WriteLine("clarinet {0} {1} {2} {3} {4} {5} {6}",
                        options.Command,
                        options.Path,
                        options.Parameters,
                        options.EnvNum,
                        options.Tenant,
                        options.Username,
                        Crypto.Protect(options.Password));
                    Console.WriteLine("\n\n");
                    return;
                }


                // Post Init and UI Option Handling
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                    case Command.CLAR_DOWNLOAD:                    
                        break;
                    case Command.DRIVE_UPLOAD:
                    case Command.DRIVE_TRASH:
                    case Command.PHOTO_DOWNLOAD:
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        soapUrl = SoapUrlBuild(options.Command);
                        break;
                }

                // API Call
                string result = "";         
                restUrl = cloudRepoUrl.Replace("{host}", host) + cloudCollection;
                soapUrl = soapUrl.Replace("{tenant}", options.Tenant).Replace("{version}", "v37.0");

                files = Directory.GetFiles(options.Path, searchPattern, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });

                // Special file handling and console update
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                        Console.WriteLine("\n\nDeploying the CLAR and awaiting the result...\n\n");
                        break;
                    case Command.CLAR_DOWNLOAD:
                        Console.WriteLine("\n\nDownloading the CLAR and awaiting the result...\n\n");
                        files = new string[] { Path.Combine(options.Path, options.Parameters + "." + DateTime.Now.ToString("s").Replace(":", ".") + ".clar") };                   
                        break;
                    case Command.DRIVE_UPLOAD:
                        Console.WriteLine("\n\nUploading files...\n\n");
                        break;
                    case Command.DRIVE_TRASH:
                        Console.WriteLine("\n\nTrashing files...\n\n");
                        if (files.Length > 0)
                        {
                            files = File.ReadAllLines(files[0]);
                        }
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        Console.WriteLine("\n\nDownloading photos...\n\n");
                        break;
                    case Command.PHOTO_UPLOAD:
                        Console.WriteLine("\n\nUploading photos...\n\n");
                        break;
                    case Command.DOCUMENT_UPLOAD:
                        Console.WriteLine("\n\nUploading documents...\n\n");
                        break;
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        Console.WriteLine("\n\nUploading attachments...\n\n");
                        break;
                    default:
                        break;
                }

                foreach (string file in files)
                {
                    Byte[] bytes;

                    switch (options.Command)
                    {
                        case Command.CLAR_UPLOAD:
                            bytes = File.ReadAllBytes(file);
                            result = Encoding.Default.GetString(WDWebService.CallRest(options.Tenant, options.Username + "@" + options.Tenant, options.Password, restUrl, WebRequestMethods.Http.Put, bytes));
                            break;
                        case Command.CLAR_DOWNLOAD:                            
                            bytes = WDWebService.CallRest(options.Tenant, options.Username + "@" + options.Tenant, options.Password, restUrl + "?fmt=clar", WebRequestMethods.Http.Get, null);
                            File.WriteAllBytes(file, bytes);
                            result = Encoding.Default.GetString(WDWebService.CallRest(options.Tenant, options.Username + "@" + options.Tenant, options.Password, restUrl, WebRequestMethods.Http.Get, null));
                            File.WriteAllText(file.Replace(".clar", ".xml"), result);
                            XDocument xDoc = XDocument.Parse(result);
                            XmlNamespaceManager xnm = new XmlNamespaceManager(new NameTable());
                            xnm.AddNamespace("default", "urn:com.workday/esb/cloud/10.0");
                            result = "Last Uploaded to Workday: " + DateTime.Parse(xDoc.XPathSelectElement("//default:deployed-since", xnm).Value).ToLocalTime().ToString("s");
                            break;
                        case Command.DRIVE_UPLOAD:                            
                            bytes = File.ReadAllBytes(file);
                            result = DriveUpload(file, bytes, soapUrl, options.Username);
                            break;
                        case Command.DRIVE_TRASH:                            
                            result = DriveTrash(file, soapUrl, options.Username);
                            break;
                        case Command.PHOTO_DOWNLOAD:
                            result = Photos.Download(options, file, soapUrl);
                            break;
                        case Command.PHOTO_UPLOAD:
                            bytes = File.ReadAllBytes(file);
                            result = Photos.Upload(file, bytes, soapUrl, processedDir, options);
                            break;
                        case Command.DOCUMENT_UPLOAD:                           
                            bytes = File.ReadAllBytes(file);
                            result = Documents.Upload(file, bytes, soapUrl, processedDir, options);                            
                            break;
                        case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                            bytes = File.ReadAllBytes(file);
                            result = CandidateAttachments.Upload(file, bytes, soapUrl, processedDir, options);
                            break;
                    }

                    if (result.ToLower().IndexOf("<html") >= 0)
                    {
                        Console.WriteLine("No XML response detected.  Workday may be unavailable or your parameters are incorrect.");
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(result))
                        {
                            Console.WriteLine("Result for " + file + "\n");
                            Console.WriteLine(result);
                            Console.WriteLine("\n\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\n" + ex.Message);
                Console.WriteLine("\n");
            }
        }

        private static string PasswordPrompt()
        {
            string pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            return pass;
        }

        private static void PrintEnvironments(List<XElement> envs)
        {
            string[] envName = new string[3];
            string colFormat = "{0,-35} {1,-35} {2,-35}\n";

            Console.WriteLine("-- Workday Environments --\n");
            for (int ndx = 0; ndx < envs.Count; ndx++)
            {
                if (ndx > 0 && ndx % 3 == 0)
                {
                    Console.Write(colFormat, envName[0], envName[1], envName[2]);
                    envName = new string[3];
                }
                if (envName[0] == null)
                {
                    envName[0] = ndx + 1 + ") " + envs[ndx].FirstAttribute.Value;
                }
                else
                {
                    if (envName[1] == null)
                    {
                        envName[1] = ndx + 1 + ") " + envs[ndx].FirstAttribute.Value;
                    }
                    else
                    {
                        envName[2] = ndx + 1 + ") " + envs[ndx].FirstAttribute.Value;
                    }
                }
            }
            if (envName[0] != null)
            {
                Console.Write(colFormat, envName[0], envName[1], envName[2]);
            }
            Console.WriteLine("\n");
        }

        static void InitDirectories(string inboundDir, string outboundDir)
        {
            // Create inbound/outbount directories
            if (!Directory.Exists(inboundDir))
            {
                Directory.CreateDirectory(inboundDir);
            }
            if (!Directory.Exists(outboundDir))
            {
                Directory.CreateDirectory(outboundDir);
            }
        }

        static bool InitOptions(string[] args)
        {
            // Parse Environments
            XDocument xDoc = XDocument.Parse(Resources.WDEnvironments);
            envs = new List<XElement>(xDoc.Descendants(XName.Get("env")));

            // Parse Arguments
            ParserResult<Options> pResult = Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    options = o;
                });

            if (pResult.Tag == ParserResultType.NotParsed)
            {
                return false;
            }

            return true;
        }

        static void OptionsEncrypt()
        {
            Console.WriteLine("Enter a password to encrypt:\n");
            string pass = PasswordPrompt();
            string encPass = "";
            try
            {
                encPass = Crypto.Protect(pass);
            }
            // Perform a retry
            catch
            {
                encPass = Crypto.Protect(pass);
            }
            Console.WriteLine("\n\n" + encPass);
            Console.WriteLine("\n");
        }

        static void CommandOption()
        {
            if (String.IsNullOrEmpty(options.Command))
            {
                Console.WriteLine("Enter the command:\n");
                options.Command = Console.ReadLine().Trim().ToUpper();
                Console.WriteLine(""); 
            }
        }

        static void CommandValidate()
        {
            // Ensure Command is uppercase.
            if (options.Command != null)
            {
                options.Command = options.Command.Trim().ToUpper();
            }
            // Check for valid commands
            switch (options.Command)
            {
                case Command.CLAR_UPLOAD:
                case Command.CLAR_DOWNLOAD:
                case Command.DRIVE_UPLOAD:
                case Command.DRIVE_TRASH:
                case Command.PHOTO_DOWNLOAD:
                case Command.PHOTO_UPLOAD:
                case Command.DOCUMENT_UPLOAD:
                case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                    break;
                default:
                    throw new Exception("Invalid command. Please use --help for a list of valid commands.");
            }
            Console.WriteLine("\n\nCommand: " + options.Command + "\n");
        }

        static void PathOption()
        {
            // Path parameter is a file
            if (options.Path != null && options.Command == Command.CLAR_UPLOAD)
            {
                if (File.Exists(options.Path))
                {
                    searchPattern = Path.GetFileName(options.Path);
                    options.Path = options.Path.Substring(0, options.Path.Length - searchPattern.Length);
                }
            }
            // Path or File
            if (String.IsNullOrEmpty(options.Path))
            {
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                    case Command.CLAR_DOWNLOAD:
                        options.Path = appDir;
                        break;
                    case Command.DRIVE_UPLOAD:
                        options.Path = inboundDir;
                        break;
                    case Command.DRIVE_TRASH:
                        options.Path = appDir;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        options.Path = appDir;
                        break;
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        options.Path = inboundDir;
                        break;
                    default:
                        break;
                }                
            }            
        }

        static void PathValidate()
        {
            Console.WriteLine("Processing: " + options.Path + "\n");
        }

        static void ParameterOption()
        {
            if (String.IsNullOrEmpty(options.Parameters))
            {
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                    case Command.CLAR_DOWNLOAD:
                        Console.WriteLine("Enter the Cloud Collection:\n");
                        cloudCollection = Console.ReadLine().Trim();
                        Console.WriteLine("");
                        options.Parameters = cloudCollection;
                        if (searchPattern.ToLower().IndexOf(".clar") < 0)
                        {
                            searchPattern = "*.clar";
                        }
                        if (options.Command == Command.CLAR_DOWNLOAD)
                        {
                            searchPattern = "";
                        }
                        break;
                    case Command.DRIVE_UPLOAD:
                        options.Parameters = "*.*";
                        searchPattern = options.Parameters;
                        break;
                    case Command.DRIVE_TRASH:
                        options.Parameters = "*trash*";
                        searchPattern = options.Parameters;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        Console.WriteLine("Enter the id file name:\n");
                        options.Parameters = Console.ReadLine().Trim();
                        Console.WriteLine("");
                        searchPattern = options.Parameters;     
                        break;
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        searchPattern = "*.*";
                        break;
                }
            }
        }

        static void ParameterValidate()
        {
            // Set search pattern and cloud collection if parameters are included.
            if (options.Parameters != null)
            {
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                        cloudCollection = options.Parameters;
                        break;
                    case Command.CLAR_DOWNLOAD:
                        searchPattern = "";
                        options.Parameters = Path.GetFileName(Path.TrimEndingDirectorySeparator(options.Parameters));
                        cloudCollection = options.Parameters;
                        break;
                    case Command.DRIVE_UPLOAD:
                    case Command.DRIVE_TRASH:
                        searchPattern = options.Parameters;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        searchPattern = options.Parameters;
                        break;
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        searchPattern = "*.*";
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("Using parameters: " + options.Parameters + "\n");            
        }

        static void FilesCheck()
        {
            files = Directory.GetFiles(options.Path, searchPattern, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            if (files.Length > 0)
            {
                if (files.Length == 1)
                {
                    Console.WriteLine("File name: " + files[0] + "\n");
                }
                else
                {
                    Console.WriteLine("Found {0:N0} files.\n", files.Length);
                    Console.WriteLine("First file name: " + files[0] + "\n");
                }
            }
            else
            {
                if (options.Command == Command.CLAR_DOWNLOAD)
                {
                    if (!Directory.Exists(options.Path))
                    {
                        throw new Exception("Directory does not exist.");
                    }
                }
                else
                {
                    throw new Exception("No files found.");
                }
            }
        }

        static void EnvironmentOption()
        {
            if (String.IsNullOrEmpty(options.EnvNum))
            {
                do
                {
                    PrintEnvironments(envs);
                    Console.WriteLine("Enter the number that identifies the Workday environment (1 - " + envs.Count + "):\n");
                    options.EnvNum = Console.ReadLine().Trim();
                    Console.WriteLine("");
                    int envNum = 0;
                    if (int.TryParse(options.EnvNum, out envNum))
                    {
                        if (envNum < envs.Count)
                        {
                            host = envs[envNum - 1].Element(XName.Get("e2-endpoint")).Value;
                            Console.WriteLine("\nHost: " + host + "\n");
                            Console.WriteLine("Is the Host correct? (Y/N)\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Entry was incorrect. Press {enter} to continue.\n");
                    }
                } while (Console.ReadLine().Trim().ToUpper() != "Y");
                Console.WriteLine("\n");
            }
            else
            {
                // Look up the host based on the number.
                host = envs[(int.Parse(options.EnvNum)) - 1].Element(XName.Get("e2-endpoint")).Value;
            }
        }

        static void TenantOption()
        {
            // Tenant
            if (String.IsNullOrEmpty(options.Tenant))
            {
                Console.WriteLine("Enter the tenant:\n");
                options.Tenant = Console.ReadLine().Trim();
                Console.WriteLine("");
            }
        }

        static void UsernameOption()
        {
            // Username
            if (String.IsNullOrEmpty(options.Username))
            {
                Console.WriteLine("Enter the username:\n");
                options.Username = Console.ReadLine().Trim();
                Console.WriteLine("");
            }
        }

        static void PasswordOption()
        {
            if (String.IsNullOrEmpty(options.Password))
            {
                Console.WriteLine("Enter the password (will not be displayed):\n");
                options.Password = PasswordPrompt();
            }
            else
            {
                options.Password = Crypto.Unprotect(options.Password);
            }
        }

        static string SoapUrlBuild(string command)
        {
            string soapUrl = ccxUrl.Replace("{host}", host);
            soapUrl = WDWebService.GetServiceURL(soapUrl, options.Tenant, options.Username, options.Password);
            switch (options.Command)
            {
                case Command.DRIVE_UPLOAD:
                case Command.DRIVE_TRASH:
                    soapUrl += "/{tenant}/Drive/{version}";
                    break;
                case Command.PHOTO_UPLOAD:
                case Command.PHOTO_DOWNLOAD:
                    soapUrl += "/{tenant}/Human_Resources/{version}";
                    break;
                case Command.DOCUMENT_UPLOAD:
                    soapUrl += "/{tenant}/Staffing/{version}";
                    break;
                case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                    soapUrl += "/{tenant}/Recruiting/{version}";
                    break;
            }
            
            return soapUrl;
        }

        static string DriveUpload(string file, byte[] bytes, string soapUrl, string uploadedBy)
        {
            string result = "";
            try
            {
                if (file.Trim().Length > 0)
                {
                    Console.WriteLine("\n\nUploading " + file + " to Drive and awaiting the result...\n\n");
                    string fileContents = Convert.ToBase64String(bytes);
                    string xmlData = DriveApi.BuildSoapRequest(file, fileContents, uploadedBy, false);
                    result = WDWebService.CallAPI(options.Username + "@" + options.Tenant, options.Password, soapUrl, xmlData);
                    if (result.IndexOf("<?xml") == 0)
                    {
                        string processedFile = Path.Combine(processedDir, Path.GetFileName(file));
                        int num = 2;
                        while (File.Exists(processedFile) && num < 100)
                        {
                            processedFile = Path.Combine(processedDir, Path.GetFileNameWithoutExtension(file) + "." + num.ToString("000") + Path.GetExtension(file));
                            num++;
                        }
                        File.Move(file, processedFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nError: " + ex.Message);
                Console.WriteLine("\n");
            }

            return result;
        }

        static string DriveTrash(string file, string soapUrl, string uploadedBy)
        {
            string result = "";
            try
            {
                if (file.Trim().Length > 0)
                {
                    Console.WriteLine("\n\nTrashing " + file + " in Drive and awaiting the result...\n\n");
                    string xmlData = DriveApi.BuildSoapRequest(file, "", uploadedBy, true);
                    result = WDWebService.CallAPI(options.Username + "@" + options.Tenant, options.Password, soapUrl, xmlData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nError: " + ex.Message);
                Console.WriteLine("\n");
            }

            return result;
        }
    }
}