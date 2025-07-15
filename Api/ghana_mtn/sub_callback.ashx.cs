using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.ghana_mtn
{
    /// <summary>
    /// Summary description for sub_callback
    /// </summary>
    public class sub_callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "mtn_ghana_subcallbacks");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "mtn_ghana_subcallbacks");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "mtn_ghana_subcallbacks");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "mtn_ghana_subcallbacks");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "mtn_ghana_subcallbacks");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "mtn_ghana_subcallbacks");
            }
            //2024-03-13 10:10:02.798: Incomming Json/ XML = {"externalServiceId":"233554306449","requestId":"s19880237461506893","requestTimeStamp":"20240313081004","channel":"3","featureId":"CallBack","requestParam":{"planId":"9921110001","command":"NotifyCharging","data":[{"name":"Msisdn","value":"554306449"},{"name":"OfferCode","value":"9921110001"},{"name":"TransactionId","value":"s19880237461506893"},{"name":"ClientTransactionId","value":"rrt-7297037684000191909-a-geu2-25291-69200036-1"},{"name":"Channel","value":"3"},{"name":"Type","value":"CONTENT_CHARGE"},{"name":"SubscriptionStatus","value":"A"},{"name":"ChargeAmount","value":"0"},{"name":"BillingId","value":"s19880237475356494"},{"name":"SubscriberLifeCycle","value":"SUB1"}]}}

            //2024-03-13 10:17:13.463: Incomming Json/ XML = {"externalServiceId":"233554306449","requestId":"deAct123080120696977","requestTimeStamp":"20240313081715","channel":"75","featureId":"CallBack","requestParam":{"planId":"9921110001","command":"NotifyDeActivation","data":[{"name":"Msisdn","value":"554306449"},{"name":"OfferCode","value":"9921110001"},{"name":"TransactionId","value":"deAct123080120696977"},{"name":"ClientTransactionId","value":"1000000000020843331"},{"name":"SubscriberLifeCycle","value":"UNSUB1"},{"name":"SubscriptionStatus","value":"D"},{"name":"Channel","value":"75"},{"name":"Reason","value":"USER_INIT_REQUEST"},{"name":"Type","value":"DEACTIVATION"}]}}


            context.Response.ContentType = "application/json";
            context.Response.Write("{\"Response\":\"OK\"}");
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