using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for momo_congob
    /// </summary>
    public class momo_congob : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "MOMO_CongoB");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MOMO_CongoB");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MOMO_CongoB");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MOMO_CongoB");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MOMO_CongoB");

            string dya_transid = "", StatusCode = "", StatusDesc = "", MOMTransactionID = "";
            if (!String.IsNullOrEmpty(xml))
            {
                




            }

            lines = Write2Log(lines);
            string response_soap = "";
            response_soap = response_soap + "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            response_soap = response_soap + "<response>OK</response>";

            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}