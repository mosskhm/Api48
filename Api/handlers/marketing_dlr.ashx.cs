using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for marketing_dlr
    /// </summary>
    public class marketing_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "marketing_dlr");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "marketing_dlr");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "marketing_dlr");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "marketing_dlr");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "marketing_dlr");
            }
            string marketing_id = context.Request.QueryString["marketing_id"];
            string status = context.Request.QueryString["status"];


            if (!String.IsNullOrEmpty(marketing_id) && status == "8")
            {
                DataLayer.DBQueries.ExecuteQuery("update marketing_camp set dlr_date = now() where id = " + marketing_id, "DBConnectionString_104", ref lines);
            }
            lines = Write2Log(lines);
            context.Response.ContentType = "text/html";
            context.Response.Write("OK");

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