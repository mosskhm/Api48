using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for getfeed
    /// </summary>
    public class getfeed : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "getfeed");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "getfeed");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "getfeed");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "getfeed");
            string service_id = context.Request.QueryString["service_id"];
            string event_type = context.Request.QueryString["event_type"];
            event_type = (!String.IsNullOrEmpty(event_type) ? event_type : "31");
            List<SportEvents> sports_events = null;
            string response = "";
            switch (service_id)
            {
                case "716":
                    //sports_events = Cache.iDoBet.GetMTNGuineaEventsFromCache(Convert.ToInt32(event_type), ref lines);
                    break;
                case "726":
                    //sports_events = Cache.iDoBet.GetMTNBeninLNBPEventsFromCache(Convert.ToInt32(event_type), ref lines);
                    break;
            }

            if (sports_events != null)
            {
                response = JsonConvert.SerializeObject(sports_events);
            }

            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";
            context.Response.Write(response);
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