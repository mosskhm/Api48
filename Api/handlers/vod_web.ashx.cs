using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for vod_web
    /// </summary>
    public class vod_web : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "vod_web");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "vod_web");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "vod_web");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "vod_web");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string incomming_post = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming post = " + incomming_post, 100, "vod_web");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "vod_red");
            }
            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";
            string json_result = "{\"code\": \"0\", \"description\": \"ok\"}";
            context.Response.Write(json_result);

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