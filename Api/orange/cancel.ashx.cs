using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for cancel
    /// </summary>
    public class cancel : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Orange_WebPaymentCancel");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "Orange_WebPaymentCancel");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Orange_WebPaymentCancel");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "Orange_WebPaymentCancel");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "Orange_WebPaymentCancel");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "Orange_WebPaymentCancel");
            }

            string dya_id = context.Request.QueryString["dya_id"];
            string url = "https://m.yellowbet.com.gn/#/user/deposit";


            if (!String.IsNullOrEmpty(dya_id))
            {
                Int64 number;

                bool success = Int64.TryParse(dya_id, out number);
                if (success)
                {
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(number, "1050", "User Cancel", ref lines);
                }

                
            }

            

            lines = Write2Log(lines);
            context.Response.Redirect(url);
            
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