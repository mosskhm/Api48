using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    public class SubscribeController : ApiController
    {
        // OPTIONS: api/Subscribe - Handle CORS preflight request
        [HttpOptions]
        public void Options()
        {
            // CORS origin check for preflight request
            string origin = HttpContext.Current.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin) 
                && (
                    origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydot.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydgames.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydplatform.com", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydaplatform.com", StringComparison.OrdinalIgnoreCase)
                   )
               )
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                HttpContext.Current.Response.Headers.Add("Vary", "Origin");
            }
            
            HttpContext.Current.Response.StatusCode = 200;
        }
        
        // POST: api/Subscribe
        public SubscribeResponse Post([FromBody] SubscribeRequest RequestBody)
        {
            // CORS origin check for actual request
            string origin = HttpContext.Current.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin) 
                && (
                    origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydot.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydgames.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydplatform.com", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydaplatform.com", StringComparison.OrdinalIgnoreCase)
                   )
               )
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                HttpContext.Current.Response.Headers.Add("Vary", "Origin");
            }

            SubscribeResponse ret = CommonFuncations.Subscribe.DoSubscribe(RequestBody);
            return ret;
        }
        
        
    }
}
