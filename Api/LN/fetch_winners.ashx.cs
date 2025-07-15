using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using static Api.Logger.Logger;
using System.Text.Json;

namespace Api.LN
{
    /// <summary>
    /// Summary description for fetch_winners handle
    /// </summary>
    public class fetch_winners : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string json;
            List<LogLines> lines = new List<LogLines>();

            try
            {
                lines = Add2Log(lines, "*****************************", 100, "LN_fetch_winners");
                lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "");
                lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "");
                lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "");

                
                // Get and sanitize input parameters
                string serviceIdStr = context.Request.QueryString["SERVICE_ID"] ?? "0";
                string drawDateStr = context.Request.QueryString["DRAW_DATE"] ?? DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                DateTime drawDate = DateTime.TryParse(drawDateStr, out var dd) ? dd : DateTime.Today.AddDays(-1);

                // fetch results for this
                Dictionary<string, List<TWN_Result>> resultsByDate = LNservice.fetch_winners(ref lines, serviceIdStr, drawDate);
        

                // validate that we go something back
                if (resultsByDate == null || resultsByDate.Count == 0)
                {
                    json = JsonSerializer.Serialize(new { error = $"No draw results found for {drawDate:yyyy-MM-dd}" });
                }
                else    // convert results to json
                {
                    var output = new
                    {
                        results = new List<object>()
                    };

                    foreach (var r in resultsByDate)
                    {
                        output.results.Add(new
                        {
                            drawDate = r.Key,
                            drawResults = r.Value
                        });
                    }

                    json = JsonSerializer.Serialize(output);
                }                
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! error occurred: {ex.Message}\n{ex.StackTrace}", 100, "");
                json = JsonSerializer.Serialize(new { error = "An error occurred fetching draw results." });
            }

            lines = Write2Log(lines);


            // CORS origin check
            string origin = context.Request.Headers["Origin"];
            if (    !string.IsNullOrEmpty(origin) 
                    && (
                        origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                        || origin.EndsWith(".ydot.co", StringComparison.OrdinalIgnoreCase)
                        || origin.EndsWith(".ydgames.co", StringComparison.OrdinalIgnoreCase)
                        || origin.EndsWith(".ydplatform.com", StringComparison.OrdinalIgnoreCase)
                        || origin.EndsWith(".ydaplatform.com", StringComparison.OrdinalIgnoreCase)
                       )
               )
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", origin);        // include this origin as valid
                context.Response.Headers.Add("Vary", "Origin");     // Important for caching proxies/CDNs
            }

            context.Response.ContentType = "application/json";
            context.Response.Write(json);
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