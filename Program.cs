﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using CommandLine;
using System.Reflection;

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

                // Option UI
                OptionsUI();

                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                        cloudCollection = options.Parameters; 
                        break;
                    case Command.DRIVE_UPLOAD:
                        soapUrl = SoapUrlBuild();
                        break;
                    case Command.DRIVE_TRASH:
                        soapUrl = SoapUrlBuild();
                        break;
                }

                // API Call
                string result = "";
                options.Username = options.Username + "@" + options.Tenant;
                restUrl = cloudRepoUrl.Replace("{host}", host) + cloudCollection;
                soapUrl = soapUrl.Replace("{tenant}", options.Tenant).Replace("{version}", "v37.0");

                files = Directory.GetFiles(options.Path, searchPattern, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });

                if (options.Command == Command.DRIVE_TRASH)
                {
                    if (files.Length > 0)
                    {
                        files = File.ReadAllLines(files[0]);
                    }
                }

                foreach (string file in files)
                {
                    Byte[] bytes;

                    switch (options.Command)
                    {
                        case Command.CLAR_UPLOAD:
                            bytes = File.ReadAllBytes(file);
                            Console.WriteLine("\n\nDeploying the CLAR and awaiting the result...\n\n");
                            result = WDWebService.CallRest(options.Tenant, options.Username, options.Password, restUrl, "PUT", bytes);
                            break;
                        case Command.DRIVE_UPLOAD:
                            bytes = File.ReadAllBytes(file);
                            result = DriveUpload(file, bytes, soapUrl);
                            break;
                        case Command.DRIVE_TRASH:
                            result = DriveTrash(file, soapUrl);
                            break;
                    }

                    if (result.IndexOf("<?xml") < 0 && result.IndexOf("<SOAP") < 0)
                    {
                        Console.WriteLine("No XML response detected.  Workday may be unavailable or your parameters are incorrect.");
                    }
                    else
                    {
                        Console.WriteLine("Result for " + file + "\n");
                        Console.WriteLine(result);
                        Console.WriteLine("\n\n");
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

            // Ensure Command is uppercase.
            if (options != null && options.Command != null)
            {
                options.Command = options.Command.Trim().ToUpper();
            }

            // Set search pattern if parameters are included.
            if (options != null && options.Parameters != null)
            {
                if (options.Command != Command.CLAR_UPLOAD)
                {
                    searchPattern = options.Parameters;
                }
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

        static void OptionsUI()
        {
            // Command
            CommandOption();
            // Path
            PathOption();
            // Parameters
            ParameterOption();
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

        static void CommandOption()
        {
            if (String.IsNullOrEmpty(options.Command))
            {
                Console.WriteLine("Enter the command:\n");
                options.Command = Console.ReadLine().Trim().ToUpper();
                Console.WriteLine("");
                // Check for valid commands
                switch (options.Command)
                {
                    case Command.DRIVE_UPLOAD:
                    case Command.DRIVE_TRASH:
                    case Command.CLAR_UPLOAD:
                        break;
                    default:
                        throw new Exception("Invalid command. Please use --help for a list of valid commands.");
                }
            }
            Console.WriteLine("\n\nCommand: " + options.Command + "\n");
        }

        static void PathOption()
        {
            // Path or File
            if (String.IsNullOrEmpty(options.Path))
            {
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                        options.Path = appDir;
                        break;
                    case Command.DRIVE_UPLOAD:
                        options.Path = inboundDir;
                        break;
                    case Command.DRIVE_TRASH:
                        options.Path = appDir;
                        break;
                }
            }
            Console.WriteLine("Processing: " + options.Path + "\n");
        }

        static void ParameterOption()
        {
            if (String.IsNullOrEmpty(options.Parameters))
            {
                switch (options.Command)
                {
                    case Command.CLAR_UPLOAD:
                        Console.WriteLine("Enter the Cloud Collection:\n");
                        cloudCollection = Console.ReadLine().Trim();
                        Console.WriteLine("");                        
                        options.Parameters = cloudCollection;
                        searchPattern = "*.clar";
                        break;
                    case Command.DRIVE_UPLOAD:
                        options.Parameters = "*.*";
                        searchPattern = options.Parameters;
                        break;
                    case Command.DRIVE_TRASH:
                        options.Parameters = "*trash*";
                        searchPattern = options.Parameters;
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
                throw new Exception("No files found.");
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

        static string SoapUrlBuild()
        {
            string soapUrl = ccxUrl.Replace("{host}", host);
            soapUrl = WDWebService.GetServiceURL(soapUrl, options.Tenant, options.Username, options.Password);
            soapUrl += "/{tenant}/Drive/{version}";
            return soapUrl;
        }

        static string DriveUpload(string file, byte[] bytes, string soapUrl)
        {
            string result = "";
            try
            {
                if (file.Trim().Length > 0)
                {
                    Console.WriteLine("\n\nUploading " + file + " to Drive and awaiting the result...\n\n");
                    string fileContents = Convert.ToBase64String(bytes);
                    string xmlData = DriveApi.BuildSoapRequest(file, fileContents, false);
                    result = WDWebService.CallAPI(options.Username, options.Password, soapUrl, xmlData);
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

        static string DriveTrash(string file, string soapUrl)
        {
            string result = "";
            try
            {
                if (file.Trim().Length > 0)
                {
                    Console.WriteLine("\n\nTrashing " + file + " in Drive and awaiting the result...\n\n");
                    string xmlData = DriveApi.BuildSoapRequest(file, "", true);
                    result = WDWebService.CallAPI(options.Username, options.Password, soapUrl, xmlData);
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