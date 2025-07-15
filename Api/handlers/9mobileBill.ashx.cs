using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for _9mobileBill
    /// </summary>
    public class _9mobileBill : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "9MobileBill");
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "9MobileBill");
            }

            string msisdn = context.Request.QueryString["msisdn"];
            string env = context.Request.QueryString["env"];
            string service = context.Request.QueryString["service"];
            string amount = context.Request.QueryString["amount"];
            string sync_async = context.Request.QueryString["method"];
            if (!String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(env) && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(sync_async))
            {
                string response = CommonFuncations.NineMobile.BillUser(env, sync_async, msisdn, service, amount, "1234567", ref lines);
                context.Response.Write("Billing Response = " + response + "<br>");
                if (response != "")
                {
                    try
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response);
                        string code = json_response.code;
                        string description = json_response.description;
                        string contextMsg = json_response.contextMsg;
                        string tranxId = json_response.tranxId;
                        string extTxnId = json_response.extTxnId;

                        context.Response.Write("code = " + code + "<br>");
                        context.Response.Write("description = " + description + "<br>");
                        context.Response.Write("contextMsg = " + contextMsg + "<br>");
                        context.Response.Write("tranxId = " + tranxId + "<br>");
                        context.Response.Write("extTxnId = " + extTxnId + "<br>");
                    }
                    catch (Exception e)
                    {

                    }
                }
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