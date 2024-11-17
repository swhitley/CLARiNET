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
    internal static class Photos
    {
        public static string Download(Options options, string file, string soapUrl)
        {
            string xmlData = "";
            string workerRef = ResourceFile.Read("Worker_Reference.txt");
            string result = "";
            string path = Path.GetDirectoryName(file);
            int batchErrors = 0;
            int errors = 0;
            int batchNum = 1;

            Dictionary<string,string> ids = File.ReadLines(file).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line.Count() > 1 ? line[1] : "");

            List<string> workerRefList = new List<string>();
            try
            {
                foreach (var batch in ids.Batch(50))
                {
                    try
                    {
                        xmlData = ResourceFile.Read("Get_Worker_Photos_Request.xml");
                        foreach (var item in batch)
                        {
                            if (!String.IsNullOrEmpty(item.Key))
                            {
                                string idType = item.Value.Trim();
                                if (String.IsNullOrEmpty(idType))
                                {
                                    idType = "Employee_ID";
                                }
                                workerRefList.Add(workerRef.Replace("{id}", item.Key.Trim())
                                    .Replace("{id_type}", idType));
                            }
                        }

                        xmlData = xmlData.Replace("{worker_references}", String.Join(" ", workerRefList.ToArray()));
                        result = WDWebService.CallAPI(options.Username + "@" + options.Tenant, options.Password, soapUrl, xmlData);
                        if (result.IndexOf("<?xml") == 0)
                        {
                            using (StringReader sr = new StringReader(result))
                            {
                                XPathDocument doc = new XPathDocument(sr);
                                XmlNamespaceManager ns = new XmlNamespaceManager(new NameTable());
                                ns.AddNamespace("wd", "urn:com.workday/bsvc");

                                XPathNodeIterator nodes = doc.CreateNavigator().Select("//wd:Worker_Photo", ns);
                                foreach(XPathNavigator item in nodes)
                                {
                                    try
                                    {
                                        string emplid = item.SelectSingleNode("wd:Worker_Photo_Data/wd:ID", ns).Value;
                                        string filename = item.SelectSingleNode("wd:Worker_Photo_Data/wd:Filename", ns).Value;
                                        string photo = item.SelectSingleNode("wd:Worker_Photo_Data/wd:File", ns).Value;                                    
                                        string fileOut = Path.Combine(path, emplid + "~" + filename);
                                        byte[] bytes = Convert.FromBase64String(photo);
                                        if (!File.Exists(fileOut) || (File.Exists(fileOut) && bytes.Length != new FileInfo(fileOut).Length))
                                        {
                                            File.WriteAllBytes(fileOut, bytes);
                                        }
                                    }
                                    catch (Exception ex3)
                                    {
                                        Console.WriteLine("\n\nError: " + ex3.Message);
                                        Console.WriteLine("\n");
                                        errors++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n\nBatch {0:N0} Error: No data returned.\n\n{1}", batchNum, result);
                            Console.WriteLine("\n");
                            errors++;
                        }
                        
                    }
                    catch(Exception batchEx)
                    {
                        Console.WriteLine("\n\nBatch {0:N0} Error: {1}", batchNum, batchEx.Message);
                        Console.WriteLine("\n");
                        batchErrors++;
                    }
                    batchNum++;
                }
                result = String.Format("Processed {0:N0} id{1}\nFile Errors: {2:N0}\nBatch Errors: {3:N0}\n", ids.Count(), ids.Count() == 1 ? "" : "s", errors, batchErrors);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nError: " + ex.Message);
                Console.WriteLine("\n");
            }

            return result;
        }

        public static string Upload(string file, byte[] bytes, string soapUrl, string processedDir, Options options)
        {
            string xmlData = "";
            string result = "";

            try
            {
                xmlData = ResourceFile.Read("Put_Worker_Photo_Request.xml");
                string[] fileVars = Path.GetFileName(file).Split("~");
                if (fileVars.Length > 1)
                {
                    Console.WriteLine("\n\nProcessing {0} for {1}", fileVars[1], fileVars[0]);
                    string workerId = fileVars[0];
                    string filename = fileVars[1];
                    string workerType = "Employee_ID";
            
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
                    xmlData = xmlData.Replace("{workerId}", workerId)
                        .Replace("{filename}", filename)
                        .Replace("{workerIdType}", workerType)
                        .Replace("{filedata}", Convert.ToBase64String(bytes));

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
