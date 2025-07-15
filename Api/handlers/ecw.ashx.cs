using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ecw
    /// </summary>
    public class ecw : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ECW");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ECW");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ECW");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ECW");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ECW");

            lines = Write2Log(lines);
            string response_soap = "";
            context.Response.ContentType = "text/plain";
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