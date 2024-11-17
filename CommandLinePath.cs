using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class CommandLinePath
    {
        public static string PathOption(string path, string command)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string inboundDir = Path.Combine(appDir, "inbound");

            // Path parameter is a file
            if (path != null && command.ToEnum<Command>() == Command.CLAR_UPLOAD)
            {
                if (File.Exists(path))
                {
                    string searchPattern = Path.GetFileName(path);
                    path = path.Substring(0, path.Length - searchPattern.Length);
                }
            }
            // Path or File
            if (String.IsNullOrEmpty(path))
            {
                switch (command.ToEnum<Command>())
                {
                    case Command.CLAR_UPLOAD:
                    case Command.CLAR_DOWNLOAD:
                        path = appDir;
                        break;
                    case Command.DRIVE_TRASH:
                        path = appDir;
                        break;
                    case Command.PHOTO_DOWNLOAD:
                        path = appDir;
                        break;
                    case Command.DRIVE_UPLOAD:
                    case Command.PHOTO_UPLOAD:
                    case Command.DOCUMENT_UPLOAD:
                    case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                        path = inboundDir;
                        break;
                    default:
                        break;
                }
            }

            return path;
        }

        public static void PathValidate(string path)
        {
            Console.WriteLine("Processing: " + path + "\n");
        }

   
    }
}
