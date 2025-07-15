using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    
    public class iDoBetDoPayout : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "iDoBetDoPayout");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "iDoBetDoPayout");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "iDoBetDoPayout");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "iDoBetDoPayout");
            string barcode = context.Request.QueryString["barcode"];
            bool result = false;
            if (!String.IsNullOrEmpty(barcode))
            {
                result = CommonFuncations.iDoBet.DoPayout(barcode, ref lines);
            }
            lines = Add2Log(lines, "result = " + result, 100, "iDoBetDoPayout");
            lines = Write2Log(lines);

            context.Response.Write(result);

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