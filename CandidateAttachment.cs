using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CLARiNET
{
    internal static class CandidateAttachments
    {
        public static string Upload(string file, byte[] bytes, string soapUrl, string processedDir, Options options)
        {
            string xmlData = "";
            string result = "";

            try
            {
                xmlData = ResourceFile.Read("Put_Candidate_Attachment_Request.xml");
                string[] fileVars = Path.GetFileName(file).Split("~");
                if (fileVars.Length > 1)
                {
                    Console.WriteLine("\n\nProcessing {0} for {1} on {2}", fileVars[2], fileVars[0], fileVars[1]);
                    string candidateId = fileVars[0];
                    string applicationId = fileVars[1];
                    string filename = fileVars[2];
                    string contentType = WDContentType.Lookup(filename);
                    string comment = "";
                    // worker id ~ filename
                    // TODO: Manifest for comment and content type?
                    xmlData = xmlData.Replace("{candidateId}", candidateId)
                        .Replace("{applicationId}", applicationId)
                        .Replace("{filename}", filename.EscapeXml())
                        .Replace("{filedata}", Convert.ToBase64String(bytes))
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
                        result = String.Format("Processed {0} for {1}", filename, candidateId);
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
