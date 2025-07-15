using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ivr_sendsms
    /// </summary>
    public class ivr_sendsms : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ivr_sendsms");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ivr_sendsms");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ivr_sendsms");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ivr_sendsms");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ivr_sendsms");
            }

            string transaction_id = context.Request.QueryString["TransactionID"];
            string call_status = context.Request.QueryString["call_status"];
            string unique_id = context.Request.QueryString["UniqueID"];
            string dtmf = context.Request.QueryString["dtmf"];
            string call_status_id = "";
            string audio_file = context.Request.QueryString["audio_file"];
            string cid = context.Request.QueryString["cid"];
            string cli = context.Request.QueryString["cli"];


            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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