using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for reload_services
    /// </summary>
    public class reload_services : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Reload_Services");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "Reload_Services");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Reload_Services");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "Reload_Services");
            if (context.Request.ServerVariables["REMOTE_ADDR"] == "80.241.216.32" || context.Request.ServerVariables["REMOTE_ADDR"] == "185.167.99.120")
            {
                bool reload_service = Api.Cache.Services.ReloadServices(ref lines);
                context.Response.ContentType = "text/plain";
                if (reload_service)
                {
                    context.Response.Write("Success");
                }
                else
                {
                    context.Response.Write("Failed");
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Not Allowed");
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