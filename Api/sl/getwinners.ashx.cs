using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.sl
{
    /// <summary>
    /// Summary description for getwinners
    /// </summary>
    public class getwinners : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "getwinners_sl");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "getwinners_sl");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "getwinners_sl");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "getwinners_sl");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "getwinners_sl");
            }

            List<LNWinnersAPI> ln_winners = GetLNWinnersApi("892", ref lines);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(ln_winners);

            lines = Write2Log(lines);


            context.Response.ContentType = "application/json";
            context.Response.Write(jsonString);
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