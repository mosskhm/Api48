using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for who
    /// </summary>
    public class who : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", log_level, "who");


            context.Response.ContentType = "text/plain";

            string headers = string.Empty;
            foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                headers = key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                lines = Add2Log(lines, headers, log_level, "who");
                context.Response.Write(headers);
            }
            
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