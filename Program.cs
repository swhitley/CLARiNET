using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using CommandLine;
using System.Reflection;

namespace CLARiNET
{
    class Program
    {
 
        static void Main(string[] args)
        {
            Options options = new Options();
            string url = "https://{host}/ccx/cc-cloud-repo/collections/";
            string host = "";

            try
            {
                XDocument xDoc = XDocument.Parse(Resources.WDEnvironments);
                List<XElement> envs = new List<XElement>(xDoc.Descendants(XName.Get("env")));

                Console.WriteLine("\n** CLARiNET by Whitley Media **\n\n");

                ParserResult<Options> pResult = Parser.Default.ParseArguments<Options>(args)
                               .WithParsed<Options>(o =>
                               {
                                   options = o;
                               });
                if (pResult.Tag == ParserResultType.NotParsed)
                {
                    return;
                }

                if (options.PrintEnvironments)
                {
                    PrintEnvironments(envs);
                    return;
                }

                // CLAR file
                if (options.ClarFile == null)
                {
                    // Check for a single CLAR file in this directory.
                    string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.clar");
                    if (files.Length == 1)
                    {
                        options.ClarFile = files[0];
                        Console.WriteLine("Processing the CLAR file: " + options.ClarFile + "\n");
                    }
                    else
                    {
                        Console.WriteLine("Enter the full path and file name for the CLAR file:\n");
                        options.ClarFile = Console.ReadLine().Trim();
                        Console.WriteLine("");
                    }
                }

                // Collection Name
                if (options.CollectionName == null)
                {
                    Console.WriteLine("Enter the Workday Cloud Collection name:\n");
                    options.CollectionName = Console.ReadLine().Trim();
                    Console.WriteLine("");
                }

                // Environment Number
                if (options.EnvNum == null)
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

                // Tenant
                if (options.Tenant == null)
                {
                    Console.WriteLine("Enter the tenant:\n");
                    options.Tenant = Console.ReadLine().Trim();
                    Console.WriteLine("");
                }

                // Username
                if (options.Username == null)
                {
                    Console.WriteLine("Enter the username:\n");
                    options.Username = Console.ReadLine().Trim();
                    Console.WriteLine("");
                }

                // Password
                if (options.Password == null)
                {
                    Console.WriteLine("Enter the password (will not be displayed):\n");
                    options.Password = PasswordPrompt();
                }

                Console.WriteLine("\n\nDeploying the CLAR and awaiting the result...\n\n");

                // REST Call
                Byte[] bytes = File.ReadAllBytes(options.ClarFile);
                options.Username = options.Username + "@" + options.Tenant;
                url = url.Replace("{host}", host) + options.CollectionName;
                string result = WDWebService.CallRest(options.Tenant, options.Username, options.Password, url, "PUT", bytes);
                Console.WriteLine("Result:\n");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
    }
}