using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for _9mobile
    /// </summary>
    public class _9mobile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "9mobile");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "9mobile");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "9mobile");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "9mobile");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string json = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming JSON = " + json, 100, "9mobile");
            string description = "", tranxId = "", code = "", extTxnId = "";
            string json_result = "{\"result\": \"ok\"}";
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(json);
                    //{"tranxId":"782376345743", "code":200, "description":" The payment was successful", "contextMsg":""} 
                    //{"description":"The balance of the subscriber is insufficient","extTxnId":"123453","code":"202","tranxId":"20180717121736000039290598246"}

                    code = auth_json_response.code;
                    description = auth_json_response.description;
                    tranxId = auth_json_response.tranxId;
                    extTxnId = auth_json_response.extTxnId;
                    if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(description) && !String.IsNullOrEmpty(tranxId) && !String.IsNullOrEmpty(extTxnId))
                    {
                        string result = "";
                        switch (code)
                        {
                            case "200":
                                result = "OK";
                                break;
                            default:
                                result = "NOK";
                                break;
                        }
                        NineMobileBillingDetails billing_details = null;

                        billing_details = GetUpdateBillingAttempt(extTxnId, result, description, code, ref lines);
                        if (billing_details != null)
                        {
                            if (code == "200")
                            {
                                Int64 billing_id = ExecuteQueryReturnInt64("insert into .billing (subscriber_id, billing_date_time, price_id) values(" + billing_details.subscriber_id + ",now()," + billing_details.price_id + ");", ref lines);
                                string datetime_now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                Int64 sub_id_175 = InsertSub(billing_details.msisdn.ToString(), billing_details.service_id.ToString(), billing_details.price_id, datetime_now, datetime_now, "1", "", "DBConnectionString_175", ref lines);
                                ExecuteQuery("update rebilling.onetime_billing set is_complete = 1 where subscriber_id = " + billing_details.subscriber_id, ref lines);
                            }
                            if (billing_details.is_onetime == 1)
                            {
                                ExecuteQuery("update rebilling.onetime_billing set next_billing_datetime = DATE_ADD(NOW(), INTERVAL 4 HOUR) where subscriber_id = " + billing_details.subscriber_id, ref lines);
                            }
                            //add billing
                            string date_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            CommonFuncations.Notifications.SendBillingNotification(billing_details.msisdn.ToString(), billing_details.service_id.ToString(), date_time, billing_details.price.ToString(), result, description, billing_details.subscription_date,  ref lines);
                        }
                    }
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, "InnerException = " + e.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + e.Message, 100, lines[0].ControlerName);
                }
            }
            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";
            context.Response.Write(json_result);

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