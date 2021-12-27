using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CLARiNET
{
    internal class DriveApi
    {
        public static string BuildSoapRequest(string file, string contents, string uploadedBy, bool trashed)
        {
            string xmlData = "";
            string delimiter = "~";
            string[] data = null;
            string username = "";
            string filename = "";
            string wid = "";

            if (file.Contains(","))
            {
                delimiter = ",";
            }

            data = file.Split(delimiter);

            if (data.Length > 1)
            {
                username = Path.GetFileName(data[0]);
                filename = data[1];

                if (data.Length > 2)
                {
                    wid = data[2];
                }

                if (trashed)
                {
                    xmlData = Resources.Put_Drive_Document_Content_Trash_Request;
                }
                else
                {
                    xmlData = Resources.Put_Drive_Document_Content_Request;
                }

                xmlData = xmlData.Replace("{contents}", contents)
                    .Replace("{document_wid}", wid)
                    .Replace("{filename}", filename)
                    .Replace("{owned_by_username}", username)
                    .Replace("{uploaded_by_username}", username);
            }
            else
            {
                throw new Exception("Could not parse file name or read input line.");
            }

            return xmlData;
        }
    }
}
