using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.Text.Json;
using System.Text;
using System.IO;

namespace CLARiNET
{
    public class WDWebService
    {
        public static string CallRest(string tenant, string username, string password, string url, string method, byte[] data)
        {
            using (var webClient = new WebClient())
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (!String.IsNullOrEmpty(username))
                {
                    webClient.Credentials = new NetworkCredential(username, password);
                    webClient.Headers.Add("X-Originator", "CLARiNET");
                    webClient.Headers.Add("X-Tenant", tenant);
                    return Encoding.Default.GetString(webClient.UploadData(url, data));
                }
            }
            return "";
        }
    }
}
