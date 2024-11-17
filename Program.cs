using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using CommandLine;
using System.Reflection;
using System.Net;
using System.Text;
using System.Xml.XPath;
using System.Xml;
using HtmlAgilityPack;

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
                    CommandLineAuth.OptionsEncrypt();
                    return;
                }

                if (options.PrintEnvironments)
                {
                    CommandLineEnv.PrintEnvironments(envs);
                    return;
                }

                
                if (options != null)
                {
                    // Command
                    options.Command = CommandLineCommand.CommandGet(options.Command);
                    options.Command = CommandLineCommand.CommandValidate(options.Command);
                    // Path
                    options.Path = CommandLinePath.PathOption(options.Path, options.Command);
                    CommandLinePath.PathValidate(options.Path);
                    // Parameters
                    (options.Parameters, cloudCollection, searchPattern) = CommandLineParameters.ParameterOption(options.Command, options.Parameters, cloudCollection, searchPattern);
                    (options.Parameters, cloudCollection, searchPattern) = CommandLineParameters.ParameterValidate(options.Command, options.Parameters, cloudCollection, searchPattern);
                    // Files Check
                    files = CommandLineFiles.FilesCheck(options.Command, options.Path, searchPattern);
                    // Environment
                    (options.EnvNum, host) = CommandLineEnv.EnvironmentOption(options.EnvNum, envs);
                    // Tenant
                    options.Tenant = CommandLineTenant.TenantOption(options.Tenant);
                    // Username
                    options.Username = CommandLineAuth.UsernameOption(options.Username);
                    // Password
                    options.Password = CommandLineAuth.PasswordOption(options.Password);
                }

                string quote = "'";
                if (OperatingSystem.IsWindows())
                {
                    quote = "\"";
                }

                Console.WriteLine("\n\n");
                Console.WriteLine("clarinet {0} {1} {2} {3} {4} {5} {6}",
                    options.Command,
                    quote + options.Path + quote,
                    quote + options.Parameters + quote,
                    host,
                    options.Tenant,
                    options.Username,
                    Crypto.Protect(options.Password));
                Console.WriteLine("\n\n");
                if (options.PrintCommandline)
                {
                    return;
                }


                // Post Init and UI Option Handling
                switch (options.Command.ToEnum<Command>())
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
                switch (options.Command.ToEnum<Command>())
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
                        WDContentType.Load();
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
                        WDContentType.Load();
                        break;
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        Console.WriteLine("\n\nUploading attachments...\n\n");
                        WDContentType.Load();
                        break;
                    default:
                        break;
                }

                foreach (string file in files)
                {
                    Byte[] bytes;

                    switch (options.Command.ToEnum<Command>())
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
                            result = DriveProcess.DriveUpload(options, processedDir, file, bytes, soapUrl, options.Username);
                            break;
                        case Command.DRIVE_TRASH:                            
                            result = DriveProcess.DriveTrash(options, file, soapUrl, options.Username);
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
            XDocument xDoc = XDocument.Parse(ResourceFile.Read("WDEnvironments.xml"));
            envs = new List<XElement>(xDoc.Descendants(XName.Get("env")));

            // Custom Host Node
            XElement custom = new XElement("env", new XElement("e2-endpoint") { Value = "{custom}"});
            custom.SetAttributeValue("name", "Enter a Custom Host Name");
            custom.SetAttributeValue("type", "impl");
            envs.Add(custom);

            // Parse Arguments
            ParserResult <Options> pResult = Parser.Default.ParseArguments<Options>(args)
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

        static string SoapUrlBuild(string command)
        {
            string soapUrl = ccxUrl.Replace("{host}", host);
            soapUrl = WDWebService.GetServiceURL(soapUrl, options.Tenant, options.Username, options.Password);
            switch (options.Command.ToEnum<Command>())
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
       
    }
}