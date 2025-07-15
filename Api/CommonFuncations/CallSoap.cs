using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Headers
    {
        public string key { get; set; }
        public string value { get; set; }
    };

    public class CallSoap
    {
        public static void CallSoapRequestAsync(string url, string body, List<Headers> headers, int json_xml, ref List<LogLines> lines)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    WebRequest request = WebRequest.Create(url);

                    request.Method = "POST";
                    if (json_xml == 1)
                    {
                        request.ContentType = "text/xml; charset=utf-8";
                    }
                    else
                    {
                        request.ContentType = "application/json; charset=utf-8";
                    }
                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    request.GetResponseAsync();

                    //WebResponse response = http.GetResponse();

                    //Stream receiveStream = response.GetResponseStream();
                    //StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    //MyResponse = readStream.ReadToEnd().ToString();
                }
                catch (WebException ex)
                {
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                        string logResponse = resp.Length > 500 ? resp.Substring(0, 500) + "...truncated" : resp;
                        lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

                }
            }
        }

        public static string GetURL(string url, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);

                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch(WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string GetURLWithHeader(string url, List<Headers> headers, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {

                WebRequest request = WebRequest.Create(url);
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                }

                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string GetURLIgnoreCertificate(string url, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = WebRequest.Create(url);

                ((System.Net.HttpWebRequest)request).ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }



        public static string GetURL(string url, ref List<LogLines> lines, string username, string password)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = WebRequest.Create(url);
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                lines = Add2Log(lines, "Authorization:  Basic " + encoded, 100, lines[0].ControlerName);
                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string GetURL(string url, ref List<LogLines> lines, out int status_code)
        {
            string responseFromServer = "";
            status_code = 200;
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  
                status_code = Convert.ToInt32(((HttpWebResponse)response).StatusCode);

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                responseFromServer = reader.ReadToEnd();
                string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);


                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                    status_code = Convert.ToInt32(((HttpWebResponse)ex.Response).StatusCode);
                }
                catch (Exception ex1)
                {
                    responseFromServer = ex1.ToString();
                    status_code = 500;
                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string GetURL(string url, ref List<LogLines> lines, out int status_code, List<Headers> headers)
        {
            string responseFromServer = "";
            status_code = 200;
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;

                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                WebResponse response = request.GetResponse();
                // Display the status.  
                status_code = Convert.ToInt32(((HttpWebResponse)response).StatusCode);

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                responseFromServer = reader.ReadToEnd();
                string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);


                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                status_code = 500;
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                    status_code = Convert.ToInt32(((HttpWebResponse)ex.Response).StatusCode);
                }
                catch (Exception ex1)
                {
                    responseFromServer = ex1.ToString();
                    
                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }


        public static string GetURL(string url, string user_agent, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                ((System.Net.HttpWebRequest)request).UserAgent = user_agent;

                request.Credentials = CredentialCache.DefaultCredentials;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                responseFromServer = reader.ReadToEnd();
                string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);

                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = responseFromServer.Length > 500 ? responseFromServer.Substring(0, 500) + "...truncated" : responseFromServer;
                    lines = Add2Log(lines, "Body = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string CallSoapRequest(string url, string soap, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + soap, 100, lines[0].ControlerName);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";



                byte[] byteArray = Encoding.UTF8.GetBytes(soap);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();

                string logResponse = MyResponse.Length > 500 ? MyResponse.Substring(0, 500) + "...truncated" : MyResponse;
                lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    string logResponse = resp.Length > 500 ? MyResponse.Substring(0, 500) + "...truncated" : resp;
                    lines = Add2Log(lines, "Response = " + logResponse, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string CallSoapRequestBerelo(string url, string soap, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + soap, 100, lines[0].ControlerName);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";



                byte[] byteArray = Encoding.UTF8.GetBytes(soap);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string GetURL(string url, List<Headers> headers, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    lines = Add2Log(lines, "Body = " + responseFromServer, 100, lines[0].ControlerName);
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + responseFromServer, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }


        public static string GetURL(string url, List<Headers> headers, string content_type, ref List<LogLines> lines)
        {
            string responseFromServer = "";
            try
            {
                WebRequest request = WebRequest.Create(url);

                request.Credentials = CredentialCache.DefaultCredentials;
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }
                request.ContentType = content_type;

                WebResponse response = request.GetResponse();
                // Display the status.  

                // Get the stream containing content returned by the server.  
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                try
                {
                    responseFromServer = reader.ReadToEnd();
                    lines = Add2Log(lines, "Body = " + responseFromServer, 100, lines[0].ControlerName);
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                }
                reader.Close();
                response.Close();
            }
            catch (WebException ex)
            {
                try
                {
                    responseFromServer = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + responseFromServer, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return responseFromServer;
        }

        public static string CallSoapRequest(string url, string soap, List<Headers> headers, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + soap, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                
                byte[] byteArray = Encoding.UTF8.GetBytes(soap);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string CallSoapRequestIgnoreCertificate(string url, string soap, List<Headers> headers, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + soap, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);

                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                request.Method = "POST";
                request.ContentType = "text/xml; charset=utf-8";
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(soap);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }

                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string CallSoapRequest1(string url, string soap, List<Headers> headers, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + soap, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/xml; charset=utf-8";
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }


                byte[] byteArray = Encoding.UTF8.GetBytes(soap);

                request.ContentLength = byteArray.Length;
                request.Timeout = 7000;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }

                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }


        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, ref List<LogLines> lines)
        {
            String MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {

                    lines = Add2Log(lines, "URL = " + url, 100, "");
                    lines = Add2Log(lines, "soap = " + body, 100, "");

                    WebRequest request = WebRequest.Create(url);

                    request.Method = "POST";
                    switch (json_xml)
                    {
                        case 1:
                            request.ContentType = "text/xml; charset=utf-8";
                            break;
                        case 2:
                            request.ContentType = "application/json; charset=utf-8";
                            break;
                        case 3:
                            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                            break;
                        case 4:
                            request.ContentType = "application/json";
                            break;
                    }

                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, "");
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, "");
                }
                catch (WebException ex)
                {
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        lines = Add2Log(lines, "Body = " + resp, 100, "");
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, "");
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, "");

                }
            }
           

            return MyResponse;
        }

        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            String MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {

                    lines = Add2Log(lines, "URL = " + url, 100, "");
                    lines = Add2Log(lines, "soap = " + body, 100, "");
                    logMessages.Add(new { message = "url = ", url, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });
                    logMessages.Add(new { message = "body = ", body, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });

                    WebRequest request = WebRequest.Create(url);

                    request.Method = "POST";
                    switch (json_xml)
                    {
                        case 1:
                            request.ContentType = "text/xml; charset=utf-8";
                            break;
                        case 2:
                            request.ContentType = "application/json; charset=utf-8";
                            break;
                        case 3:
                            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                            break;
                        case 4:
                            request.ContentType = "application/json";
                            break;
                    }

                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, "");
                        logMessages.Add(new { message = h.key + ":" + h.value, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, "");
                    logMessages.Add(new { message = "Response = " + MyResponse, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });
                }
                catch (WebException ex)
                {
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        lines = Add2Log(lines, "Body = " + resp, 100, "");
                        logMessages.Add(new { message = "Response = " + resp, application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, "");
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, "");
                    logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "CallSoap.CallSoapRequest", logz_id = logz_id });

                }
            }


            return MyResponse;
        }

        public static string CallSoapRequestUA(string url, string body, List<Headers> headers, int json_xml,string user_agent, ref List<LogLines> lines)
        {
            string MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {

                    lines = Add2Log(lines, "URL = " + url, 100, "");
                    lines = Add2Log(lines, "soap = " + body, 100, "");

                    WebRequest request = WebRequest.Create(url);
                    ((System.Net.HttpWebRequest)request).UserAgent = user_agent;
                    
                    request.Method = "POST";
                    switch (json_xml)
                    {
                        case 1:
                            request.ContentType = "text/xml; charset=utf-8";
                            break;
                        case 2:
                            request.ContentType = "application/json; charset=utf-8";
                            break;
                        case 3:
                            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                            break;
                        case 4:
                            request.ContentType = "application/json";
                            ((System.Net.HttpWebRequest)request).Accept = "application/json";
                            break;

                    }

                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, "");
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, "");
                }
                catch (WebException ex)
                {
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        lines = Add2Log(lines, "Body = " + resp, 100, "");
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, "");
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, "");

                }
            }


            return MyResponse;
        }

        public static string CallSoapRequestWithMethod(string url, string body, List<Headers> headers, int json_xml, string method, ref List<LogLines> lines)
        {
            String MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {

                    lines = Add2Log(lines, "URL = " + url, 100, "");
                    lines = Add2Log(lines, "soap = " + body, 100, "");

                    WebRequest request = WebRequest.Create(url);

                    request.Method = method;
                    switch (json_xml)
                    {
                        case 1:
                            request.ContentType = "text/xml; charset=utf-8";
                            break;
                        case 2:
                            request.ContentType = "application/json; charset=utf-8";
                            break;
                        case 3:
                            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                            break;
                        case 4:
                            request.ContentType = "application/json";
                            break;
                    }

                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, "");
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, "");
                }
                catch (WebException ex)
                {
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        lines = Add2Log(lines, "Body = " + resp, 100, "");
                        MyResponse = resp.ToString();
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, "");
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, "");

                }
            }


            return MyResponse;
        }

        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, out string error, ref List<LogLines> lines)
        {
            error = "";
            String MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {

                    lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);

                    WebRequest request = WebRequest.Create(url);

                    request.Method = "POST";
                    switch (json_xml)
                    {
                        case 1:
                            request.ContentType = "text/xml; charset=utf-8";
                            break;
                        case 2:
                            request.ContentType = "application/json; charset=utf-8";
                            break;
                        case 3:
                            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                            break;
                        case 4:
                            request.ContentType = "application/json";
                            break;
                        case 5:
                            request.ContentType = "application/json";
                            ((System.Net.HttpWebRequest)request).Accept = "*/*";
                            break;
                    }

                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
                }
                catch (WebException ex)
                {
                    error = "error";
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                        error = resp;
                    }
                    catch (Exception ex1)
                    {
                        
                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

                }
            }


            return MyResponse;
        }

        public static string CallSoapForm(string url, List<Headers> headers, int json_xml, Dictionary<string, string> formData, out string error, ref List<LogLines> lines)
        {
            error = "";
            String MyResponse = "";
            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                    string body = string.Join("&", formData.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={kv.Value}"));

                    lines = Add2Log(lines, "postData = " + body, 100, lines[0].ControlerName);
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    using (HttpClient httpClient = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            foreach (var keyValuePair in formData)
                            {
                                content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
                            }
                            try
                            {
                                response = httpClient.PostAsync(url, content).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    MyResponse = response.Content.ReadAsStringAsync().Result;

                                }
                                else
                                {
                                    error = "error";
                                    MyResponse = response.Content.ReadAsStringAsync().Result;
                                }
                            }
                            catch(Exception ex)
                            {
                                lines = Add2Log(lines, "exception = " + ex.ToString(), 100, lines[0].ControlerName);
                            }
                            

                            
                        }
                    }
                    lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
                }
                catch (WebException ex)
                {
                    error = "error";
                    try
                    {
                        var resp = response.Content.ReadAsStringAsync().Result;
                        lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                        error = resp;
                    }
                    catch (Exception ex1)
                    {

                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

                }
            }


            return MyResponse;
        }


        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, out bool is_failed, ref List<LogLines> lines)
        {
            is_failed = false;
            String MyResponse = "";
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);
                    WebRequest request = WebRequest.Create(url);

                    request.Method = "POST";
                    if (json_xml == 1)
                    {
                        request.ContentType = "text/xml; charset=utf-8";
                    }
                    else
                    {
                        request.ContentType = "application/json; charset=utf-8";
                    }
                    foreach (Headers h in headers)
                    {
                        request.Headers.Add(h.key, h.value);
                        lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    request.ContentLength = byteArray.Length;

                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the http stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();

                    WebResponse response = request.GetResponse();
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    MyResponse = readStream.ReadToEnd().ToString();
                }
                catch (WebException ex)
                {
                    is_failed = true;
                    try
                    {
                        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                        MyResponse = resp;
                        lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                    }
                    catch (Exception ex1)
                    {
                        is_failed = true;
                    }
                    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

                }
            }


            return MyResponse;
        }


        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, bool return_error, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                if (json_xml == 1)
                {
                    request.ContentType = "text/xml; charset=utf-8";
                }
                else if (json_xml == 2)
                {
                    request.ContentType = "application/json; charset=utf-8";
                }
                else
                {
                    request.ContentType = "application/json";
                    ((System.Net.HttpWebRequest)request).Accept = "*/*";
                    
                }
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(body);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                    if (return_error)
                    {
                        MyResponse = resp;
                    }
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, bool return_error, bool with_error, ref List<LogLines> lines)
        {

            String MyResponse = "Failuer";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);

                request.Method = "POST";
                if (json_xml == 1)
                {
                    request.ContentType = "text/xml; charset=utf-8";
                }
                else
                {
                    request.ContentType = "application/json; charset=utf-8";
                }
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(body);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                    if (return_error)
                    {
                        MyResponse = resp;
                    }
                }
                catch (Exception ex1)
                {
                    MyResponse = "Failuer";
                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                

            }

            return MyResponse;
        }

        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, string username, string password, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                lines = Add2Log(lines, "Authorization:  Basic " + encoded, 100, lines[0].ControlerName);

                request.Method = "POST";
                if (json_xml == 1)
                {
                    request.ContentType = "text/xml; charset=utf-8";
                }
                else
                {
                    request.ContentType = "application/json; charset=utf-8";
                }
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                
                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                
                

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }

        public static string CallSoapRequest(string url, string body, List<Headers> headers, int json_xml, string username, string password, string method, ref List<LogLines> lines)
        {
            String MyResponse = "";
            try
            {
                lines = Add2Log(lines, "URL = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "soap = " + body, 100, lines[0].ControlerName);
                WebRequest request = WebRequest.Create(url);
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
                lines = Add2Log(lines, "Authorization:  Basic " + encoded, 100, lines[0].ControlerName);

                request.Method = method;
                switch (json_xml)
                {
                    case 1:
                        request.ContentType = "text/xml; charset=utf-8";
                        break;
                    case 2:
                        request.ContentType = "application/json; charset=utf-8";
                        break;
                    case 3:
                        request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                        break;
                }
                foreach (Headers h in headers)
                {
                    request.Headers.Add(h.key, h.value);
                    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(body);

                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                // Write the data to the http stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                WebResponse response = request.GetResponse();

                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                MyResponse = readStream.ReadToEnd().ToString();
                lines = Add2Log(lines, "Response = " + MyResponse, 100, lines[0].ControlerName);
            }
            catch (WebException ex)
            {
                try
                {
                    var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {

                }
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);

            }

            return MyResponse;
        }
    }
}