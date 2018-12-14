using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityClient.Utils
{
    public static class IpHelper
    {
        public static string GetCountry(string ip)
        {
            if (ip == null) throw new ArgumentNullException("ip");
            if (ip.StartsWith("127.0.0.")) return "XX";
            try
            {
                var request = (HttpWebRequest)(WebRequest.Create("http://api.hostip.info/country.php?ip=" + Uri.EscapeDataString(ip)));
                request.Timeout = 500;
                var response = (HttpWebResponse)(request.GetResponse());

                var sb = new StringBuilder();
                var buf = new byte[8192];
                Stream resStream = response.GetResponseStream();

                int count;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        sb.Append(Encoding.ASCII.GetString(buf, 0, count));
                }
                while (count > 0);

                return sb.ToString();
            }
            catch { return "XX"; }
        }

        public static string GetExternalIp()
        {
            string externalIP = "";
            externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
            return externalIP;
        }
    }
}
