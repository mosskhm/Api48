using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for LP_Test
    /// </summary>
    public class LP_Test : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("<!DOCTYPE html>");
            context.Response.Write("<html>");
            context.Response.Write("<head>");
            context.Response.Write("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            context.Response.Write("</head>");
            context.Response.Write("<body>");
            context.Response.Write("<h4><a href = \"http://track32.ydot.co/track/gloln/glonjc\">Glo Landing Page</a></h4>");
            context.Response.Write("<h4><a href = \"http://track32.ydot.co/track/c/NPU\">MTN Podcast Daily</a></h4>");
            context.Response.Write("<h4><a href = \"http://track32.ydot.co/Track/G/CFFTU\">Class 54</a></h4>");
            context.Response.Write("<h4><a href = \"http://ydplatform.com/enrichment/enrich/trackng\">HE Test1</a></h4>");
            context.Response.Write("<h4><a href = \"http://api32.ydplatform.com/enrichment/enrich/trackng\">HE Test2</a></h4>");
            context.Response.Write("<h4><a href = \"http://crm.ydplatform.com/enrichment/enrich/trackng\">HE Test3</a></h4>");
            context.Response.Write("</body>");
            context.Response.Write("<html>");


            
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