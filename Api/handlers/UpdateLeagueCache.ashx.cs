using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for UpdateLeagueCache
    /// </summary>
    public class UpdateLeagueCache : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "updateleaguecache");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "updateleaguecache");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "updateleaguecache");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "updateleaguecache");

            if (context.Request.ServerVariables["REMOTE_ADDR"] == "80.241.216.32" || context.Request.ServerVariables["REMOTE_ADDR"] == "::1")
            {
                lines = Add2Log(lines, " updating cache ", 100, "updateleaguecache");
                HttpContext.Current.Application["GetiDoBetLeagues"] = null;
                List<iDoBetLeague> leagues = Api.Cache.iDoBet.GetiDoBetLeagues(ref lines);
                context.Response.Write("ok");
            }
            else
            {
                lines = Add2Log(lines, " forbiden ip ", 100, "updateleaguecache");
                context.Response.Write("ko");
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