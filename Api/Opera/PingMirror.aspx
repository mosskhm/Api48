<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PingMirror.aspx.cs" Inherits="Api.Opera.PingMirror" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections.ObjectModel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="Server">
    <meta http-equiv="Content-Type" content="text/html;CHARSET=iso-8859-1">      
</head>
<body>        
    <%
        const string OPERA_QUERY_STRING = "opera-querystring";
        const string OPERA_REMOTE_ADDRESS = "opera-remote-address";       
                
        // Get All Request Headers
        IDictionary<string, string> incomingHttpHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        NameValueCollection headers = Request.Headers;
        foreach (string key in headers.AllKeys)
        {
            incomingHttpHeaders.Add(key, headers[key]);
        }
        
        
        // List of msisdn headers
        IList<string> msisdnHeadersList = new List<string> { "msisdn", "http_x_msisdn", "user-identity-forward-msisdn",
                "x-msp-msisdn", "http_x_nokia_msisdn", "http_x_msp_msisdn", "http_x_wte_msisdn",
                "http_user_identity_forward_msisdn", "http_msisdn", "http_x_fh_msisdn", "http_x_wap_msisdn",
                "http_x_wap_network_client_msisdn", "http_x_h3g_msisdn", "http_mscope_msisdn", "http_i_msisdn",
                "http_x_up_calling_line_id", "x-up-calling-line-id", "x-nokia-msisdn", "x-msisdn", "x-wap-msisdn",
                "imisdn", "x-bam-msisdn", "y-msisdn", "x-wap-network-client-msisdn" };
                
        // Get All Request Parameters 
        Response.Write("<BR>");
        Response.Write("<BR>");
        Response.Write("<span style=\"font-weight:bold; color:blue; \">'Setting msisdn from GET Parameters to Incoming Http Headers' :: </span>");    
        //string[] paramNames = Request.Params.AllKeys; 
        string[] paramNames = Request.QueryString.AllKeys;  
        foreach (var param in paramNames)
        {
            if (msisdnHeadersList.Contains(param.ToLower()) && !incomingHttpHeaders.ContainsKey(param.ToLower()))
            {
                Response.Write("<BR>");
                Response.Write(param);
                Response.Write("<BR>");
                incomingHttpHeaders.Add(param, Request.QueryString[param]);
            }
        }

        // Add Extra Header "opera-querystring" and "opera-remote-address"
        if (!incomingHttpHeaders.ContainsKey(OPERA_QUERY_STRING))
            incomingHttpHeaders.Add(OPERA_QUERY_STRING, Request.Url.Query);
        else
            incomingHttpHeaders[OPERA_QUERY_STRING] = Request.Url.Query;
            
        string operaRemoteAddress = GetRemoteAddress(Request);
        if (!incomingHttpHeaders.ContainsKey(OPERA_REMOTE_ADDRESS))
            incomingHttpHeaders.Add(OPERA_REMOTE_ADDRESS, operaRemoteAddress);
        else
            incomingHttpHeaders[OPERA_REMOTE_ADDRESS] = operaRemoteAddress;

        Response.Write("<BR>");
        Response.Write("<BR>");
        Response.Write("<span style=\"font-weight:bold; color:blue; \">'opera-querystring' Header :: </span>");
        Response.Write("<BR>");
        Response.Write(Request.Url.Query);
        Response.Write("<BR>");

        Response.Write("<span style=\"font-weight:bold; color:blue; \">'opera-remote-address' Header :: </span>");
        Response.Write("<BR>");
        Response.Write(operaRemoteAddress);
        Response.Write("<BR>");

        Response.Write("<span style=\"font-weight:bold; color:blue; \">Incoming Http Headers :: </span>");
        Response.Write("<BR>{");
        foreach (KeyValuePair<string, string> header in incomingHttpHeaders)
        {
            Response.Write(header.Key + "=" + header.Value + ",");
        }
        Response.Write("}<BR>");
        
        
        // List of standard http headers
        IList<string> standardHeadersList = new List<string>(); 
        standardHeadersList.Add("accept");
        standardHeadersList.Add("accept-charset");
        standardHeadersList.Add("accept-encoding");
        standardHeadersList.Add("accept-language");
        standardHeadersList.Add("accept-datetime");
        standardHeadersList.Add("authorization");
        standardHeadersList.Add("cache-control");
        standardHeadersList.Add("connection");
        standardHeadersList.Add("cookie");
        standardHeadersList.Add("content-length");
        standardHeadersList.Add("content-md5");
        standardHeadersList.Add("content-type");
        standardHeadersList.Add("date");
        standardHeadersList.Add("dnt");
        standardHeadersList.Add("expect");
        standardHeadersList.Add("from");
        standardHeadersList.Add("front-end-https");
        standardHeadersList.Add("host");
        standardHeadersList.Add("if-match");
        standardHeadersList.Add("if-modified-since");
        standardHeadersList.Add("if-none-match");
        standardHeadersList.Add("if-range");
        standardHeadersList.Add("if-unmodified-Since");
        standardHeadersList.Add("max-forwards");
        standardHeadersList.Add("origin");
        standardHeadersList.Add("pragma");
        standardHeadersList.Add("proxy-authorization");
        standardHeadersList.Add("proxy-connection");
        standardHeadersList.Add("range");
        //standardHeadersList.Add("referer");
        standardHeadersList.Add("te");
        standardHeadersList.Add("upgrade");        
        standardHeadersList.Add("via");
        standardHeadersList.Add("warning");
        standardHeadersList.Add("x-requested-with");        
        standardHeadersList.Add("x-do-not-track");
        //standardHeadersList.Add("X-Forwarded-For");
        standardHeadersList.Add("x-forwarded-proto");
                 
        ReadOnlyCollection<string> standardHeadersReadOnly = new ReadOnlyCollection<string>(standardHeadersList);
        
        // List of allowed domains
        IList<string> allowedDomainsList = new List<string>(); 
        allowedDomainsList.Add("opera.com");
        allowedDomainsList.Add("opera-mini.net");
        allowedDomainsList.Add("operamini.com");
        ReadOnlyCollection<string> allowedDomainsReadOnly = new ReadOnlyCollection<string>(allowedDomainsList);

        // Remove Standard Headers
        foreach (string headerToBeRemoved in standardHeadersReadOnly)
        {
            if (incomingHttpHeaders.ContainsKey(headerToBeRemoved))
                incomingHttpHeaders.Remove(headerToBeRemoved);
        }

        Response.Write("<span style=\"font-weight:bold; color:blue; \">Http Headers after removing standard headers ::</span>");
        Response.Write("<BR>{");
        foreach (KeyValuePair<string, string> header in incomingHttpHeaders)
        {
            Response.Write(header.Key + "=" + header.Value + ",");
        }
        Response.Write("}<BR>");
        
        // Execution starts
		string tParam = Request.Params["t"];
		string pId = Request.Params["pid"];
        Response.Write("<span style=\"font-weight:bold; color:blue; \">pid  ::</span>");
        Response.Write("<BR>");
        Response.Write(pId);
        Response.Write("<BR>");
        Response.Write("<span style=\"font-weight:bold; color:blue; \">tParam ::</span>");
        Response.Write("<BR>");
        Response.Write(tParam);
        Response.Write("<BR>");
		
		IList<string> urls = new List<string>();
		string[] arrayTypeUrls = null;
        char[] charSeparators = new char[] {','};
		if(!string.IsNullOrEmpty(tParam) && !string.IsNullOrEmpty(pId)) {
            tParam = tParam.Trim();
            pId = pId.Trim();
            
			if (tParam.StartsWith("["))
			{
                arrayTypeUrls = tParam.Split(charSeparators);
                foreach (string url in arrayTypeUrls)
                {
                    string modURL = url.Replace("\"", "");
                    if (modURL.StartsWith("["))
                        modURL = modURL.Substring(1);
                    if (modURL.EndsWith("]"))
                        modURL = modURL.Substring(0, modURL.Length - 1);
                    urls.Add(modURL);
                }
            
			}
			else
				urls.Add(tParam);
		}

        // Function to forward pings with request headers
        if (incomingHttpHeaders != null && urls != null && urls.Count > 0)
        {
            Response.Write("<span style=\"font-weight:bold; color:blue; \">Urls eligible for ping forwarding :: </span>");
            Response.Write("<BR>[");
            foreach(string url in urls)
                Response.Write(url + ",");
            Response.Write("]<BR>");
            Response.Write("<span style=\"font-weight:bold; color:blue; \">Allowed Domains :: </span>");
            Response.Write("<BR>");
            Response.Write("\".opera.com\", \".opera-mini.net\", \".operamini.com\"");
            Response.Write("<BR>");
            ForwardPings(urls, incomingHttpHeaders, allowedDomainsReadOnly);
        }        
    %>
    <script runat="server" type="text/C#">
        String GetRemoteAddress(HttpRequest request)
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
                remoteAddr = Request.ServerVariables["REMOTE_ADDR"];
            else
            {
                remoteAddrSplit = remoteAddr.Trim().Split(new char[] {X_FORWARDED_FOR_SEPARATOR} );
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
    
        string GetFullHeader(string[] headerValues)
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
        
        string FilterPrivateIPs(IList<String> remoteAddrList)
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

        void ForwardPings(IList<string> urls, IDictionary<string, string> incomingHttpHeaders, ReadOnlyCollection<string> allowedDomains)
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
                    
                    // Set headers in the request
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
                    req.Headers.Add("opera-remote-address", Request.ServerVariables["REMOTE_ADDR"]);
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

        bool IsDomainAllowed(Uri serverUrl, ReadOnlyCollection<string> allowedDomains)
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
    </script>

</body>
</html>
