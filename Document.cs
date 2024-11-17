using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CLARiNET
{
    internal static class Documents
    {
        public static string Upload(string file, byte[] bytes, string soapUrl, string processedDir, Options options)
        {
            string xmlData = "";
            string result = "";

            try
            {
                xmlData = ResourceFile.Read("Put_Worker_Document_Request.xml");
                string[] fileVars = Path.GetFileName(file).Split("~");
                if (fileVars.Length > 1)
                {
                    Console.WriteLine("\n\nProcessing {0} for {1}", fileVars[1], fileVars[0]);
                    string workerId = fileVars[0];
                    string filename = fileVars[1];
                    string workerType = "Employee_ID";
                    string comment = "";
                    string contentType = WDContentType.Lookup(filename);
                    if (fileVars.Length > 2)
                    {
                        filename = fileVars[2];
                        if (fileVars[1].ToUpper() == "C")
                        {
                            workerType = "Contingent_Worker_ID";
                        }
                    }
                    // worker id ~ filename
                    // worker id ~ C ~ filename
                    // TODO: Manifest for comment and content type?
                    xmlData = xmlData.Replace("{workerId}", workerId)
                        .Replace("{filename}", filename.EscapeXml())
                        .Replace("{workerIdType}", workerType)
                        .Replace("{filedata}", Convert.ToBase64String(bytes))
                        .Replace("{documentCategory}", options.Parameters)
                        .Replace("{comment}", comment.EscapeXml())
                        .Replace("{contentType}", contentType);

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
                        result = String.Format("Processed {0} for {1}", filename, workerId);
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
    }    
}
