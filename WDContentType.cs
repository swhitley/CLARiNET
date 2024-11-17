using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;
using System.IO;

namespace CLARiNET
{
    // ContentTypes.json from https://raw.githubusercontent.com/patrickmccallum/mimetype-io/master/src/mimeData.json
    public class WDContentType
    {
        public static SortedDictionary<string, string> wdContentTypes = new SortedDictionary<string, string>();

        public static void Load()
        {
            //string[] keys = Resources.WorkdayContentTypes.Split("\n");
            string[] keys = ResourceFile.Read("WorkdayContentTypes.txt").Split("\n");
            var doc = JsonDocument.Parse(ResourceFile.Read("ContentTypes.json"));

            foreach (string key in keys)
            {
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    if (item.GetProperty("name").ToString() == key)
                    {
                        var fileTypes = item.GetProperty("fileTypes");
                        foreach (var fileType in fileTypes.EnumerateArray())
                        {
                            if (!wdContentTypes.ContainsKey(fileType.ToString()))
                            {
                                wdContentTypes.Add(fileType.ToString(), key);
                            }
                        }
                    }
                }
                
            }
        }

        public static string Lookup(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            string contentType = "application/octet-stream";

            if (wdContentTypes.ContainsKey(ext))
            {
                contentType = wdContentTypes[ext];
            }

            return contentType;

        }       

    }
}
