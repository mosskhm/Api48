using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for sms_dlr
    /// </summary>
    public class sms_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "*****************************", 100, "MADApiSMSDLR");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MADApiSMSDLR");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MADApiSMSDLR");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MADApiSMSDLR");
            lines = Add2Log(lines, "Incomming Json = " + xml, 100, "MADApiSMSDLR");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "MADApiSMSDLR");
            }

            if (!String.IsNullOrEmpty(xml))
            {
                //{"messageId":"1165124343","senderAddress":"YellowBet","receiverAddress":"242067123773","deliveryStatus":"DELIVRD","submittedDate":1699484400000,"completedDate":1699484400000,"requestId":"87649df3-5f86-4810-9fab-8e6fe280fe8a","clientCorrelator":"string","transactionId":"rrt-3155777865844010061-d-geu1-31751-433223740-1"}
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string clientCorrelator = json_response.clientCorrelator;
                    string dlr_status = json_response.deliveryStatus;

                    Int64 number;

                    bool success = Int64.TryParse(clientCorrelator, out number);
                    if (success)
                    {
                        string mysql = "update send_sms set dlr_date_time = now(), dlr_result = '"+dlr_status+"' where id = " + number;
                        Api.DataLayer.DBQueries.ExecuteQuery(mysql, "DBConnectionString_161", ref lines);
                    }


                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
                }
            }
            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"OK\"}");
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