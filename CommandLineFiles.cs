using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CLARiNET
{
    public static class CommandLineFiles
    {
        public static string[] FilesCheck(string command, string path, string searchPattern)
        {
            string[] files = Directory.GetFiles(path, searchPattern, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
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
                if (command.ToEnum<Command>() == Command.CLAR_DOWNLOAD)
                {
                    if (!Directory.Exists(path))
                    {
                        throw new Exception("Directory does not exist.");
                    }
                }
                else
                {
                    throw new Exception("No files found.");
                }
            }
            return files;
        }

      
    }
}
