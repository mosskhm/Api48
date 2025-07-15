using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.timwe
{
    /// <summary>
    /// Summary description for sub_callback
    /// </summary>
    public class sub_callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_subcallbacks");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_subcallbacks");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_subcallbacks");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_subcallbacks");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_subcallbacks");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "Timwe_subcallbacks");
            }

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"Response\":\"OK\"}");
            lines = Write2Log(lines);
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