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
            string workerRef = Resources.Worker_Reference;
            string result = "";
            string path = Path.GetDirectoryName(file);

            Dictionary<string,string> ids = File.ReadLines(file).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line.Count() > 1 ? line[1] : "");

            List<string> workerRefList = new List<string>();
            try
            {
                foreach (var batch in ids.Batch(50))
                {
                    try
                    {
                        xmlData = Resources.Get_Worker_Photos_Request;
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
                                    }
                                }
                            }
                        }
                        
                    }
                    catch(Exception batchEx)
                    {
                        Console.WriteLine("\n\nError: " + batchEx.Message);
                        Console.WriteLine("\n");
                    }
                }
                result = String.Format("Processed {0:N0} id{1}.", ids.Count(), ids.Count() == 1 ? "" : "s");
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
