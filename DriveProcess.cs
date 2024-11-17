using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class DriveProcess
    {
        public static string DriveUpload(Options options, string processedDir, string file, byte[] bytes, string soapUrl, string uploadedBy)
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

        public static string DriveTrash(Options options, string file, string soapUrl, string uploadedBy)
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
