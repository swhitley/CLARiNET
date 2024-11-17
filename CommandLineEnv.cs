using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CLARiNET
{
    public static class CommandLineEnv
    {
        public static (string, string) EnvironmentOption(string envNumStr, List<XElement> envs)
        {
            string host = null;
            if (String.IsNullOrEmpty(envNumStr))
            {
                do
                {
                    PrintEnvironments(envs);
                    Console.WriteLine("Enter the number that identifies the Workday environment (1 - " + envs.Count + "):\n");
                    envNumStr = Console.ReadLine().Trim();
                    Console.WriteLine("");
                    int envNum = 0;
                    if (int.TryParse(envNumStr, out envNum))
                    {
                        if (envNum <= envs.Count)
                        {
                            host = envs[envNum - 1].Element(XName.Get("e2-endpoint")).Value;
                            if (host == "{custom}")
                            {
                                Console.WriteLine("Enter a custom Host (ex: wcpdev-services1.wd101.myworkday.com):\n");
                                host = Console.ReadLine().Trim();

                            }

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
                if (int.TryParse(envNumStr, out _))
                {
                    host = envs[(int.Parse(envNumStr)) - 1].Element(XName.Get("e2-endpoint")).Value;
                }
                else
                {
                    host = envNumStr;
                }
            }
            return (envNumStr, host);
        }

        public static void PrintEnvironments(List<XElement> envs)
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
