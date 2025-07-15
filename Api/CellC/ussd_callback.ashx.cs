using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CellC
{
    /// <summary>
    /// Summary description for ussd_callback
    /// </summary>
    public class ussd_callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            string response_soap = "OK";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_cellc");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_cellc");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_cellc");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_cellc");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_cellc");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ussd_cellc");
            }

            foreach (String key in context.Request.ServerVariables.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.ServerVariables[key], 100, "ussd_cellc");
            }

            lines = Add2Log(lines, "Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

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