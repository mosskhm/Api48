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
    /// Summary description for notify_2018
    /// </summary>
    public class notify_2018 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Notify_2018");
            lines = Add2Log(lines, "Incomming Json = " + xml, 100, "MO");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "Notify_2018");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Notify_2018");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "Notify_2018");
            string json_response = "{\"response\":400,\"message\":\"Failed\"}";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
   //                 {
   //                     "msisdn":"2348026970486",
   //"amount":"200",
   //"referenceid":"5beec3812e745",
   //"eventTimestamp":"2018-02-28 13-54-02",
   //"responsecode":1,
   //"responsedescription" : "Successful"
   //                 }


                    dynamic payout_incomming = JsonConvert.DeserializeObject(xml);
                    string msisdn = payout_incomming.msisdn;
                    string reference = payout_incomming.reference;
                    string eventTimestamp = payout_incomming.eventTimestamp;
                    string status = payout_incomming.status;

                    if (!String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(reference) && !String.IsNullOrEmpty(eventTimestamp) && !String.IsNullOrEmpty(status))
                    {
                        if (status == "success")
                        {
                            Int64 dya_id = DataLayer.DBQueries.SelectQueryReturnInt64("SELECT t.dya_id FROM (SELECT * FROM dya_transactions d WHERE d.service_id = 9 ORDER BY d.dya_id DESC LIMIT 100000) t WHERE t.transaction_id = '"+ reference + "' order by t.dya_id desc limit 1", ref lines);
                            if (dya_id > 0)
                            {
                                DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '01', receive_datetime = '" + eventTimestamp + "' where dya_id = " + dya_id, ref lines);
                                json_response = "{\"response\":200,\"message\":\"Successful\"}";
                            }
                            else
                            {
                                json_response = "{\"response\":403,\"message\":\"reference not found\"}";
                            }
                        }
                        else
                        {
                            json_response = "{\"response\":200,\"message\":\"Successful\"}";
                        }
                    }
                    else
                    {
                        json_response = "{\"response\":401,\"message\":\"Missing params\"}";
                    }



                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, "Notify_2018");
                    json_response = "{\"response\":402,\"message\":\"Exception\"}";
                }
            }

            lines = Add2Log(lines, "Response = " + json_response, 100, "Notify_2018");
            lines = Write2Log(lines);
            
            context.Response.ContentType = "application/json";
            context.Response.Write(json_response);

            
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