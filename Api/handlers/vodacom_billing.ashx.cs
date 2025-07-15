using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for vodacom_billing
    /// </summary>
    public class vodacom_billing : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "vodacom_billing");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "vodacom_billing");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "vodacom_billing");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "vodacom_billing");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string json = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming JSON = " + json, 100, "vodacom_billing");
            string msisdn = "", service_key = "", action = "", date = "", amount = "", emsisdn = "";
            string json_result = "{\"code\": \"0\", \"description\": \"ok\"}";
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(json);
                    emsisdn = auth_json_response.emsisdn;
                    service_key = auth_json_response.service_key;
                    action = auth_json_response.action;
                    date = auth_json_response.date;
                    
                    if (action.ToLower() == "bill")
                    {
                        amount = auth_json_response.amount;
                        ServiceClass service = GetVodacomService(service_key, ref lines);
                        if (service != null)
                        {
                            amount = (amount.Contains(".") == true ? (Convert.ToDouble(amount) * 100).ToString() : amount + "00");
                            PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                            if (price_c != null)
                            {
                                Int64 sub_id = DataLayer.DBQueries.InsertSub(msisdn, service.service_id.ToString(), price_c.price_id, date, date, "1", "", ref lines);
                                string json_code = (sub_id > 0 ? "0" : "-1");
                                string json_desc = (sub_id > 0 ? "ok" : "User was not found or error inserting user");
                                if (sub_id > 0)
                                {
                                    CommonFuncations.Notifications.SendBillingNotification(msisdn, service.service_id.ToString(), date, (Convert.ToDouble(amount) / 100).ToString(), ref lines);
                                }
                            }
                            else
                            {
                                json_result = "{\"code\": \"-1\", \"description\": \"price was not found\"}";
                            }
                        }
                        else
                        {
                            json_result = "{\"code\": \"-1\", \"description\": \"Service was not found\"}";
                        }

                    }

                    if (action.ToLower() == "sub")
                    {
                        ServiceClass service = GetVodacomService(service_key, ref lines);
                        if (service != null)
                        {
                            string ext_ref = auth_json_response.ext_reference;
                            //convert to emsisdn to msisdn
                            msisdn = "";// VodacomIMI.DecryptMSISDN(service.is_staging, Convert.ToInt64(emsisdn), ref lines);
                            Int64 sub_id = DataLayer.DBQueries.InsertSub(msisdn, service.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "7", "", ref lines);
                            string json_code = (sub_id > 0 ? "0" : "-1");
                            string json_desc = (sub_id > 0 ? "ok" : "User was not found or error inserting user");
                            if (sub_id > 0)
                            {
                                CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), date, ref lines);
                                ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                            }
                            
                        }
                        
                    }
                    
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
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