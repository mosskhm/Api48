using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Api.Opera
{
    public static class Opera
    {
        public static String GetRemoteAddress(HttpRequestBase request)
        {
            // Since we are only supporting opera-mini , only x-forwarded-for header
            // is needed
            string[] priorityList = { "x-forwarded-for" };
            char X_FORWARDED_FOR_SEPARATOR = ',';
            string remoteAddr = null;
            string[] remoteAddrSplit = null;
            foreach (string headerName in priorityList)
            {
                remoteAddr = GetFullHeader(request.Headers.GetValues(headerName));
                if (remoteAddr != null)
                {
                    break;
                }
            }
            if (remoteAddr == null)
                remoteAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            else
            {
                remoteAddrSplit = remoteAddr.Trim().Split(new char[] { X_FORWARDED_FOR_SEPARATOR });
                int ipLength = remoteAddrSplit.Length;

                IList<string> remoteAddrList = new List<string>();

                for (int i = 0; i < ipLength; i++)
                {
                    if (Regex.IsMatch(remoteAddrSplit[i].Trim(), "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+"))
                        remoteAddrList.Add(remoteAddrSplit[i].Trim());
                }
                if (remoteAddrSplit.Length == 1)
                    remoteAddr = remoteAddrSplit[0];
                else
                    remoteAddr = FilterPrivateIPs(remoteAddrList);
            }
            return remoteAddr;
        }

        public static string GetFullHeader(string[] headerValues)
        {
            StringBuilder builder = new StringBuilder();
            if (headerValues != null)
            {
                foreach (string value in headerValues)
                {

                    if (!String.IsNullOrEmpty(builder.ToString()))
                        builder.Append(",");
                    builder.Append(value);
                }
            }
            else
                return null;
            return builder.ToString();
        }

        public static string FilterPrivateIPs(IList<String> remoteAddrList)
        {
            string remoteAddr = null;
            string[] privateIPs = { "10.", "127.", "172.16.", "172.17.", "172.18.", "172.19.", "172.20.", "172.21.",
                "172.22.", "172.23.", "172.24.", "172.25.", "172.26.", "172.27.", "172.28.", "172.29.", "172.30.",
                "172.31.", "192.168." };
            foreach (string ipFromHeader in remoteAddrList)
            {
                bool matched = false;
                foreach (string privateIP in privateIPs)
                {
                    if (ipFromHeader.StartsWith(privateIP))
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                {
                    remoteAddr = ipFromHeader;
                    break;
                }
            }
            return remoteAddr;
        }

        public static void ForwardPings(IList<string> urls, IDictionary<string, string> incomingHttpHeaders, ReadOnlyCollection<string> allowedDomains)
        {
            int count = 0;
            const string HTTP_STRING = "http://";
            Regex regex = new Regex(HTTP_STRING, RegexOptions.IgnoreCase);
            foreach (string url in urls)
            {
                // Not more than three pings should be sent from the script
                if (count > 2)
                    break;

                string modURL = url;
                // If no protocol is specified, assume http.
                if (!regex.IsMatch(url))
                    modURL = string.Concat(HTTP_STRING, modURL);

                try
                {
                    string endPoint = HttpUtility.UrlDecode(modURL, Encoding.UTF8);
                    Uri serverUri = new Uri(endPoint);

                    //Ping to be sent only to allowed domains
                    if (!IsDomainAllowed(serverUri, allowedDomains))
                    {
                        continue;
                    }

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serverUri);
                    req.Method = "GET";

                    // Set headers in the http
                    foreach (KeyValuePair<string, string> item in incomingHttpHeaders)
                    {
                        if (String.Compare(item.Key, "User-Agent", true) == 0)
                            req.UserAgent = item.Value;
                        else if (String.Compare(item.Key, "Referer", true) == 0)
                            req.Referer = item.Value;
                        else
                            req.Headers.Add(item.Key, item.Value);
                    }
                    req.Headers.Add("Cache-Control", "no-cache");
                    req.Headers.Add("opera-remote-address", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                    count = count + 1;

                    // Get the response
                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                        {
                            string answer = reader.ReadToEnd();
                        }
                    }

                }// Try Block ends
                catch (UriFormatException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            } // For loop ends
        }

        public static bool IsDomainAllowed(Uri serverUrl, ReadOnlyCollection<string> allowedDomains)
        {
            bool urlMatched = false;
            foreach (string domain in allowedDomains)
            {
                if (serverUrl != null && serverUrl.Host.EndsWith(domain))
                {
                    urlMatched = true;
                    break;
                }
            }
            return urlMatched;
        }
    }
}