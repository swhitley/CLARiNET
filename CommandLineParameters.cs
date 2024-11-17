using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class CommandLineParameters
    {
        public static (string, string, string) ParameterOption(string command, string parameters, string cloudCollection, string searchPattern)
        {
            if (String.IsNullOrEmpty(parameters))
            {
                switch (command.ToEnum<Command>())
                {
                    case Command.CLAR_UPLOAD:
                    case Command.CLAR_DOWNLOAD:
                        Console.WriteLine("Enter the Cloud Collection:\n");
                        cloudCollection = Console.ReadLine().Trim();
                        Console.WriteLine("");
                        parameters = cloudCollection;
                        if (searchPattern.ToLower().IndexOf(".clar") < 0)
                        {
                            searchPattern = "*.clar";
                        }
                        if (command.ToEnum<Command>() == Command.CLAR_DOWNLOAD)
                        {
                            searchPattern = "";
                        }
                        break;
                    case Command.DRIVE_UPLOAD:
                        parameters = "*.*";
                        searchPattern = parameters;
                        break;
                    case Command.DRIVE_TRASH:
                        parameters = "*trash*";
                        searchPattern = parameters;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        Console.WriteLine("Enter the id file name:\n");
                        parameters = Console.ReadLine().Trim();
                        Console.WriteLine("");
                        searchPattern = parameters;
                        break;
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        searchPattern = "*.*";
                        break;
                }
            }
            return (parameters, cloudCollection, searchPattern);
        }

        public static (string, string, string) ParameterValidate(string command, string parameters, string cloudCollection, string searchPattern)
        {
            // Set search pattern and cloud collection if parameters are included.
            if (parameters != null)
            {
                switch (command.ToEnum<Command>())
                {
                    case Command.CLAR_UPLOAD:
                        cloudCollection = parameters;
                        break;
                    case Command.CLAR_DOWNLOAD:
                        searchPattern = "";
                        parameters = Path.GetFileName(Path.TrimEndingDirectorySeparator(parameters));
                        cloudCollection = parameters;
                        break;
                    case Command.DRIVE_UPLOAD:
                    case Command.DRIVE_TRASH:
                        searchPattern = parameters;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        searchPattern = parameters;
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
            Console.WriteLine("Using parameters: " + parameters + "\n");
            return (parameters, cloudCollection, searchPattern);
        }
    }
}
