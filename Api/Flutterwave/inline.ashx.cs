using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Flutterwave
{
    /// <summary>
    /// Summary description for inline
    /// </summary>
    public class inline : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            context.Response.ContentType = "text/html";
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