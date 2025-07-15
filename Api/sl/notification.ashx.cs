using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.sl
{
    /// <summary>
    /// Summary description for notification
    /// </summary>
    public class notification : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ln_sl");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ln_sl");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ln_sl");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ln_sl");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ln_sl");
            }

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            string result = "OK";
            string description = "";
            if (!String.IsNullOrEmpty(xml))
            {
                lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ln_sl");
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string service_id = json_response.service_id;
                    string amount_billed = json_response.amount_billed;
                    string action = json_response.action;
                    string msisdn = json_response.msisdn;
                    Int64 sub_id = 0;

                    if (!String.IsNullOrEmpty(service_id) && !String.IsNullOrEmpty(action) && !String.IsNullOrEmpty(msisdn))
                    {
                        msisdn = (msisdn.StartsWith("2327") ? msisdn : "232" + msisdn);
                        //msisdn = "232" + msisdn;
                        ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
                        if (service != null)
                        {
                            PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount_billed), ref lines);
                            if (price_c == null)
                            {
                                Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount_billed + ",'ZWD','ZWD'," + amount_billed + ")", ref lines);
                                if (price_id > 0)
                                {
                                    List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                    if (result_list != null)
                                    {
                                        HttpContext.Current.Application["PriceList"] = result_list;
                                        HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                    }
                                    price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount_billed), ref lines);
                                }
                            }

                            if (price_c != null)
                            {
                                string mydate_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                switch (action)
                                {
                                    case "subscribe":
                                        sub_id = Api.DataLayer.DBQueries.InsertSub(msisdn, service_id, price_c.price_id, mydate_time, mydate_time, "2", "", ref lines);
                                        lines = Add2Log(lines, "Subscribing " + msisdn + " to service " + service.service_name + " sub_id = " + sub_id, 100, "sub_callbacks");
                                        break;
                                    case "bill":
                                        sub_id = Api.DataLayer.DBQueries.InsertSub(msisdn, service_id, price_c.price_id, mydate_time, mydate_time, "2", "", ref lines);
                                        lines = Add2Log(lines, "Rebilling " + msisdn + " from service " + service.service_name + " sub_id = " + sub_id, 100, "sub_callbacks");
                                        break;
                                    case "deactivate":
                                        sub_id = Api.DataLayer.DBQueries.UnsubscribeSub(msisdn, service_id, mydate_time, "2", "", mydate_time, ref lines);
                                        lines = Add2Log(lines, "Unsubscribing " + msisdn + " from service " + service.service_name + " sub_id = " + sub_id, 100, "sub_callbacks");
                                        break;
                                }
                                if (sub_id <= 0)
                                {
                                    lines = Add2Log(lines, "Something went wrong" + sub_id, 100, "sub_callbacks");
                                    result = "NOK";
                                    description = "Something went wrong";
                                }
                            }
                            else
                            {
                                lines = Add2Log(lines, "Price was not found" + sub_id, 100, "sub_callbacks");
                                result = "NOK";
                                description = "Price was not found";
                            }

                        }
                        else
                        {
                            lines = Add2Log(lines, "Service was not found" + sub_id, 100, "sub_callbacks");
                            result = "NOK";
                            description = "Service was not found";
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "Missing data", 100, "sub_callbacks");
                        result = "NOK";
                        description = "Missing parameters";
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception: " + ex.ToString(), 100, "sub_callbacks");
                    result = "NOK";
                    description = "something went wrong";
                }


            }

            lines = Write2Log(lines);


            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\": \"" + result + "\", \"description\": \"" + description + "\"}");
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