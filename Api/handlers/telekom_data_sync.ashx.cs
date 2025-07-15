using Api.Cache;
using Api.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for telekom_data_sync
    /// </summary>
    public class telekom_data_sync : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "TelekomDataSync");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MOMO");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "TelekomDataSync");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "TelekomDataSync");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "TelekomDataSync");

            Int64 sub_id = 0;
            int channel_id = 7;
            string description = "";
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "TelekomDataSync");
            }

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                
                string result_name = "",subscription_started_at = "", subscription_id = "", user_msisdn = "", svc_id = "", svc_name = "", channel_name = "", status_id = "", status_name = "", billing_rate = "", campaign_id = "", ext_ref = "", created_at = "";
                string amount = json_response.amount;
                if (!String.IsNullOrEmpty(amount))
                {
                    subscription_started_at = json_response.subscription.subscription_started_at;
                    result_name = json_response.result_name;
                    if (result_name == "SUCCESS")
                    {
                        string[] allowedFormats = {"yyyy-MM-dd HH:mm:ss","yyyy-MM-dd HH:mm:ss.f","yyyy-MM-dd HH:mm:ss.ff","yyyy-MM-dd HH:mm:ss.fff"};
                        DateTime parsedDate;
                        if (DateTime.TryParseExact(subscription_started_at, allowedFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                        {
                            parsedDate = parsedDate.Date;

                            // Get today's date (without time)
                            DateTime today = DateTime.Today;

                            if (parsedDate != today)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into telkom_requests (json, type, date_time) values('" + xml + "',2,now())", "DBConnectionString_161", ref lines);
                            }
                            else
                            {
                                subscription_id = json_response.subscription.subscription_id;
                                user_msisdn = json_response.subscription.user_msisdn;
                                svc_id = json_response.subscription.svc_id;
                                svc_name = json_response.subscription.svc_name;

                                channel_name = json_response.subscription.channel_name;

                                status_id = json_response.subscription.status_id;
                                status_name = json_response.subscription.status_name;
                                billing_rate = json_response.subscription.billing_rate;
                                campaign_id = json_response.subscription.campaign_id;

                                ext_ref = json_response.subscription.ext_ref;
                                created_at = json_response.subscription.created_at;

                                subscription_id = json_response.subscription.subscription_id;
                                sub_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT s.subscriber_id FROM subscribers_telekom s WHERE s.telekom_sub_id = " + subscription_id, ref lines);
                                svc_id = json_response.subscription.svc_id;
                                ServiceClass service = GetServiceList(ref lines).Find(x => x.spid == svc_id);
                                if (sub_id > 0)
                                {

                                    if (service != null)
                                    {
                                        lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                                        PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                        if (price_c == null)
                                        {
                                            Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount + ",'',''," + amount + ")", ref lines);
                                            if (price_id > 0)
                                            {
                                                price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                            }
                                        }
                                        if (price_c != null)
                                        {
                                            Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ",now(), " + price_c.price_id + ")", ref lines);
                                        }
                                    }
                                }
                                else
                                {
                                    PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                    if (price_c == null)
                                    {
                                        Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount + ",'',''," + amount + ")", ref lines);
                                        if (price_id > 0)
                                        {
                                            price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                        }
                                    }
                                    sub_id = Api.DataLayer.DBQueries.InsertSub(user_msisdn, service.service_id.ToString(), price_c.price_id, created_at, created_at, channel_id.ToString(), "", "DBConnectionString_175", ref lines);
                                    if (price_c != null)
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ",now(), " + price_c.price_id + ")", ref lines);
                                    }
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_misc (subscriber_id, channel_name) values(" + sub_id + ",'" + channel_name + "')", ref lines);
                                    description = (sub_id > 0 ? "OK" : "NOK");
                                    lines = Add2Log(lines, "Adding Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                                    Api.CommonFuncations.Notifications.SendSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                                    if (billing_rate != "0")
                                    {
                                        Api.CommonFuncations.Notifications.SendBillingNotification(user_msisdn, service.service_id.ToString(), created_at, (Convert.ToDouble(billing_rate) / 100).ToString(), ref lines);
                                    }
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_telekom (subscriber_id, telekom_sub_id) values(" + sub_id + "," + subscription_id + ")", ref lines);
                                    if (!String.IsNullOrEmpty(ext_ref))
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set msisdn = " + user_msisdn + ", subscriber_id = " + sub_id + " where id = " + ext_ref, ref lines);
                                    }
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    subscription_id = json_response.subscription_id;
                    user_msisdn = json_response.user_msisdn;
                    svc_id = json_response.svc_id;
                    svc_name = json_response.svc_name;

                    channel_name = json_response.channel_name;

                    status_id = json_response.status_id;
                    status_name = json_response.status_name;
                    billing_rate = json_response.billing_rate;
                    campaign_id = json_response.campaign_id;

                    ext_ref = json_response.ext_ref;
                    created_at = json_response.created_at;

                    switch (channel_name)
                    {
                        case "WAP":
                            channel_id = 7;
                            break;
                        case "SMS":
                            channel_id = 2;
                            break;
                        case "USSD":
                            channel_id = 3;
                            break;

                    }

                    created_at = Convert.ToDateTime(created_at).ToString("yyyy-MM-dd HH:mm:ss");


                    lines = Add2Log(lines, " subscription_id = " + subscription_id, 100, "sub_callbacks");
                    lines = Add2Log(lines, " user_msisdn " + user_msisdn, 100, "sub_callbacks");
                    lines = Add2Log(lines, " svc_id " + svc_id, 100, "sub_callbacks");
                    lines = Add2Log(lines, " svc_name " + svc_name, 100, "sub_callbacks");
                    lines = Add2Log(lines, " channel_name " + channel_name, 100, "sub_callbacks");
                    lines = Add2Log(lines, " status_id " + status_id, 100, "sub_callbacks");

                    lines = Add2Log(lines, " status_name " + status_name, 100, "sub_callbacks");
                    lines = Add2Log(lines, " billing_rate " + billing_rate, 100, "sub_callbacks");
                    lines = Add2Log(lines, " ext_ref " + ext_ref, 100, "sub_callbacks");
                    lines = Add2Log(lines, " created_at " + created_at, 100, "sub_callbacks");
                    lines = Add2Log(lines, " campaign_id " + campaign_id, 100, "sub_callbacks");

                    if (user_msisdn != null)
                    {
                        try
                        {
                            ServiceClass service = GetServiceList(ref lines).Find(x => x.spid == svc_id);
                            if (service != null)
                            {
                                lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                                PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(billing_rate), ref lines);

                                if (price_c == null)
                                {
                                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + billing_rate + ",'',''," + billing_rate + ")", ref lines);
                                    if (price_id > 0)
                                    {
                                        price_c = GetPricesInfo(service.service_id, Convert.ToDouble(billing_rate), ref lines);
                                    }
                                }
                                switch (status_id)
                                {
                                    case "2": //Active

                                        sub_id = Api.DataLayer.DBQueries.InsertSub(user_msisdn, service.service_id.ToString(), price_c.price_id, created_at, created_at, channel_id.ToString(), "", "DBConnectionString_175", ref lines);
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_misc (subscriber_id, channel_name) values(" + sub_id + ",'" + channel_name + "')", ref lines);
                                        if (campaign_id != "0")
                                        {
                                            ///Api.DataLayer.DBQueries.ExecuteQuery("insert into tracking.tracking_requests (date_time, campaign_id, msisdn, ip, user_agent, subscriber_id) values(now(), "+campaign_id+ "," + user_msisdn + ",'','', " + sub_id+")", ref lines);
                                        }

                                        description = (sub_id > 0 ? "OK" : "NOK");
                                        lines = Add2Log(lines, "Adding Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                                        Api.CommonFuncations.Notifications.SendSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                                        if (billing_rate != "0")
                                        {
                                            Api.CommonFuncations.Notifications.SendBillingNotification(user_msisdn, service.service_id.ToString(), created_at, (Convert.ToDouble(billing_rate) / 100).ToString(), ref lines);
                                        }
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_telekom (subscriber_id, telekom_sub_id) values(" + sub_id + "," + subscription_id + ")", ref lines);
                                        if (!String.IsNullOrEmpty(ext_ref))
                                        {
                                            Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set msisdn = " + user_msisdn + ", subscriber_id = " + sub_id + " where id = " + ext_ref, ref lines);
                                        }
                                        break;
                                    case "5000":
                                    case "5001":
                                    case "5002":
                                    case "8": //Canceled
                                    case "4": //Churned 
                                        sub_id = Api.DataLayer.DBQueries.UnsubscribeSub(user_msisdn, service.service_id.ToString(), created_at, channel_id.ToString(), "", created_at, ref lines);
                                        description = (sub_id > 0 ? "OK" : "NOK");
                                        lines = Add2Log(lines, "Deleting Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                                        Api.CommonFuncations.Notifications.SendUnSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                                        break;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, "Exception " + ex.ToString(), 100, "sub_callbacks");
                        }
                    }
                }


                

                //try
                //{
                //    //{"amount":500,"subscription":{"subscription_id":2125641519,"user_id":4478320,"user_msisdn":"27693866315","svc_id":26,"svc_name":"YellowDot Games","channel_name":"WAP","status_id":2,"status_name":"ACTIVE","renewal_type":"AUTO","billing_rate":500,"billing_cycle":"DAILY","campaign_id":0,"ext_ref":"25177575","created_at":"2024-06-04 10:59:16.3","subscription_started_at":"2024-06-04 10:59:16.3","updated_at":"2024-06-04 10:59:16.3","expires_at":"2024-06-05 10:59:16.957","last_billed_at":"2024-06-04 10:59:16.962782","next_billing_at":"2024-06-05 10:59:16.957"},"billing_ref":"b864b41f-2250-11ef-91f2-ee9cde365a3a","result_id":1,"result_name":"SUCCESS","billing_type":"NORMAL"}

                //    //2020-05-29 10:45:44.567: Incomming XML = {"bc_id":188,"mno_id":4,"svc_id":30,"ext_ref":"","user_id":1925846,"svc_name":"Gamewin","status_id":2,"created_at":"2020-05-29T10:45:38+02:00","expires_at":"2020-05-30T10:45:38+02:00","updated_at":"2020-05-29T10:45:39+02:00","campaign_id":0,"status_name":"ACTIVE","user_msisdn":"27659909790","affiliate_id":0,"billing_rate":300,"channel_name":"USSD","renewal_type":"AUTO","billing_cycle":"DAILY","last_billed_at":"2020-05-29T10:45:38+02:00","next_billing_at":"2020-05-30T10:45:38+02:00","subscription_id":4600532,"subscription_started_at":"2020-05-29T10:45:38+02:00"}


                //    //{"amount":500,"subscription":{"subscription_id":2125689722,"user_id":7619615,"user_msisdn":"27692857569","svc_id":26,"svc_name":"YellowDot Games","channel_name":"WAP","status_id":2,"status_name":"ACTIVE","renewal_type":"AUTO","billing_rate":500,"billing_cycle":"DAILY","campaign_id":700235,"ext_ref":"25212922","created_at":"2024-06-06 11:01:26.177","subscription_started_at":"2024-06-06 11:01:26.177","updated_at":"2024-06-06 11:01:26.177","expires_at":"2024-06-07 11:01:26.483","last_billed_at":"2024-06-06 11:01:26.48678","next_billing_at":"2024-06-07 11:01:26.483"},"billing_ref":"44f96caa-23e3-11ef-837e-32ca193f4dc6","result_id":1,"result_name":"SUCCESS","billing_type":"NORMAL"}
                //    //{"subscription_id":2125689770,"user_id":7620755,"user_msisdn":"27692880233","svc_id":360,"svc_name":"Bigcash - USSD2","channel_name":"USSD","status_id":2,"status_name":"ACTIVE","renewal_type":"AUTO","billing_rate":500,"billing_cycle":"DAILY","campaign_id":0,"ext_ref":"","created_at":"2024-06-06 11:05:35.71","subscription_started_at":"2024-06-06 11:05:35.71","updated_at":"2024-06-06 11:05:35.71","expires_at":"2024-06-07 11:05:41.701","last_billed_at":"2024-06-06 11:05:35.71","next_billing_at":"2024-06-07 11:05:41.701"}

                //    dynamic json_response = JsonConvert.DeserializeObject(xml);
                //    string amount = json_response.amount;
                //    string subscription_id = "", user_msisdn = "", svc_id = "", svc_name = "", channel_name = "", status_id = "", status_name = "", billing_rate = "", campaign_id = "", ext_ref = "", created_at = "";
                //    if (!String.IsNullOrEmpty(amount))
                //    {
                        
                //        subscription_id = json_response.subscription.subscription_id;
                //        user_msisdn = json_response.subscription.user_msisdn;
                //        svc_id = json_response.subscription.svc_id;
                //        svc_name = json_response.subscription.svc_name;

                //        channel_name = json_response.subscription.channel_name;

                //        status_id = json_response.subscription.status_id;
                //        status_name = json_response.subscription.status_name;
                //        billing_rate = json_response.subscription.billing_rate;
                //        campaign_id = json_response.subscription.campaign_id;

                //        ext_ref = json_response.subscription.ext_ref;
                //        created_at = json_response.subscription.created_at;

                //    }
                //    else
                //    {
                //        Api.DataLayer.DBQueries.ExecuteQuery("insert into telkom_requests (json, type, date_time) values('" + xml + "',1,now()", "DBConnectionString_161", ref lines);
                //        subscription_id = json_response.subscription_id;
                //        user_msisdn = json_response.user_msisdn;
                //        svc_id = json_response.svc_id;
                //        svc_name = json_response.svc_name;

                //        channel_name = json_response.channel_name;

                //        status_id = json_response.status_id;
                //        status_name = json_response.status_name;
                //        billing_rate = json_response.billing_rate;
                //        campaign_id = json_response.campaign_id;

                //        ext_ref = json_response.ext_ref;
                //        created_at = json_response.created_at;
                //    }
                    
                    
                //    switch (channel_name)
                //    {
                //        case "WAP":
                //            channel_id = 7;
                //            break;
                //        case "SMS":
                //            channel_id = 2;
                //            break;
                //        case "USSD":
                //            channel_id = 3;
                //            break;

                //    }
                    
                //    created_at = Convert.ToDateTime(created_at).ToString("yyyy-MM-dd HH:mm:ss");
                    

                //    lines = Add2Log(lines, " subscription_id = " + subscription_id, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " user_msisdn " + user_msisdn, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " svc_id " + svc_id, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " svc_name " + svc_name, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " channel_name " + channel_name, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " status_id " + status_id, 100, "sub_callbacks");

                //    lines = Add2Log(lines, " status_name " + status_name, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " billing_rate " + billing_rate, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " ext_ref " + ext_ref, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " created_at " + created_at, 100, "sub_callbacks");
                //    lines = Add2Log(lines, " campaign_id " + campaign_id, 100, "sub_callbacks");
                //    //Incomming XML = {"bc_id":188,"mno_id":4,"svc_id":26,"ext_ref":"a361b920409811ec94b6bdbe173035","user_id":13,"svc_name":"YellowDot Games","status_id":2,"created_at":"2021-11-08T15:34:57+02:00","expires_at":"2021-11-08T15:34:57+02:00","updated_at":"2021-11-08T15:34:58+02:00","campaign_id":599997,"status_name":"ACTIVE","user_msisdn":"27614077981","affiliate_id":0,"billing_rate":300,"channel_name":"WAP_DOI","renewal_type":"AUTO","billing_cycle":"DAILY","last_billed_at":"2021-11-08T15:34:57+02:00","next_billing_at":"2021-11-08T15:34:57+02:00","subscription_id":13128398,"subscription_started_at":"2021-11-08T15:34:57+02:00"}


                //    if (user_msisdn != null)
                //    {
                //        try
                //        {
                //            ServiceClass service = GetServiceList(ref lines).Find(x => x.spid == svc_id);
                //            if (service != null)
                //            {
                //                lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                //                PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(billing_rate), ref lines);

                //                if (price_c == null)
                //                {
                //                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + billing_rate + ",'',''," + billing_rate + ")", ref lines);
                //                    if (price_id > 0)
                //                    {
                //                        List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                //                        if (result_list != null)
                //                        {
                //                            HttpContext.Current.Application["PriceList"] = result_list;
                //                            HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                //                        }
                //                        price_c = GetPricesInfo(service.service_id, Convert.ToDouble(billing_rate), ref lines);
                //                    }
                //                }
                //                switch (status_id)
                //                {
                //                    case "2": //Active

                //                        sub_id = DBQueries.InsertSub(user_msisdn, service.service_id.ToString(), price_c.price_id, created_at, created_at, channel_id.ToString(), "", "DBConnectionString_175", ref lines);
                //                        Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_misc (subscriber_id, channel_name) values("+sub_id+",'"+channel_name+"')", ref lines);
                //                        if (campaign_id != "0")
                //                        {
                //                            ///Api.DataLayer.DBQueries.ExecuteQuery("insert into tracking.tracking_requests (date_time, campaign_id, msisdn, ip, user_agent, subscriber_id) values(now(), "+campaign_id+ "," + user_msisdn + ",'','', " + sub_id+")", ref lines);
                //                        }
                                        
                //                        description = (sub_id > 0 ? "OK" : "NOK");
                //                        lines = Add2Log(lines, "Adding Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                //                        CommonFuncations.Notifications.SendSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                //                        if (billing_rate != "0")
                //                        {
                //                            CommonFuncations.Notifications.SendBillingNotification(user_msisdn, service.service_id.ToString(), created_at, (Convert.ToDouble(billing_rate) / 100).ToString(), ref lines);
                //                        }
                //                        DBQueries.ExecuteQuery("insert into subscribers_telekom (subscriber_id, telekom_sub_id) values("+ sub_id + ","+ subscription_id + ")", ref lines);
                //                        if (!String.IsNullOrEmpty(ext_ref))
                //                        {
                //                            Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set msisdn = " + user_msisdn + ", subscriber_id = " + sub_id + " where id = " + ext_ref, ref lines);
                //                        }
                //                        break;
                //                    case "5000":
                //                    case "5001":
                //                    case "5002":
                //                    case "8": //Canceled
                //                    case "4": //Churned 
                //                        sub_id = DBQueries.UnsubscribeSub(user_msisdn, service.service_id.ToString(), created_at, channel_id.ToString(), "", created_at, ref lines);
                //                        description = (sub_id > 0 ? "OK" : "NOK");
                //                        lines = Add2Log(lines, "Deleting Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                //                        CommonFuncations.Notifications.SendUnSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                //                        break;
                //                }

                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            lines = Add2Log(lines, "Exception " + ex.ToString(), 100, "sub_callbacks");
                //        }
                //    }

                //    //2020-05-29 10:45:11.215: Incomming XML = {"amount":300,"result_id":"1","billing_ref":154831057,"result_name":"SUCCESS","billing_type":"NORMAL","subscription":{"bc_id":188,"mno_id":4,"svc_id":26,"ext_ref":"27680905999_26_mcm2_tm","user_id":1914128,"svc_name":"YellowDot Games","status_id":2,"created_at":"2020-05-28T10:44:08+02:00","expires_at":"2020-05-30T10:45:05+02:00","updated_at":"2020-05-29T10:45:05+02:00","campaign_id":0,"status_name":"ACTIVE","user_msisdn":"27680905999","affiliate_id":0,"billing_rate":300,"channel_name":"WAP_DOI","renewal_type":"AUTO","billing_cycle":"DAILY","last_billed_at":"2020-05-29T10:45:05+02:00","next_billing_at":"2020-05-30T10:45:05+02:00","subscription_id":4563587,"subscription_started_at":"2020-05-28T10:44:08+02:00"}}
                    
                //    if (!String.IsNullOrEmpty(amount))
                //    {
                //        string result_name = json_response.result_name;
                //        if (result_name != "INSUFFICIENT_FUNDS")
                //        {
                //            subscription_id = json_response.subscription.subscription_id;
                //            sub_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT s.subscriber_id FROM subscribers_telekom s WHERE s.telekom_sub_id = " + subscription_id, ref lines);
                //            svc_id = json_response.subscription.svc_id;
                //            ServiceClass service = GetServiceList(ref lines).Find(x => x.spid == svc_id);
                //            if (sub_id > 0)
                //            {
                                
                //                if (service != null)
                //                {
                //                    lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                //                    PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                //                    if (price_c == null)
                //                    {
                //                        Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount + ",'',''," + amount + ")", ref lines);
                //                        if (price_id > 0)
                //                        {
                //                            List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                //                            if (result_list != null)
                //                            {
                //                                HttpContext.Current.Application["PriceList"] = result_list;
                //                                HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                //                            }
                //                            price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                //                        }
                //                    }
                //                    if (price_c != null)
                //                    {
                //                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ",now(), " + price_c.price_id + ")", ref lines);
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                //                if (price_c == null)
                //                {
                //                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount + ",'',''," + amount + ")", ref lines);
                //                    if (price_id > 0)
                //                    {
                //                        List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                //                        if (result_list != null)
                //                        {
                //                            HttpContext.Current.Application["PriceList"] = result_list;
                //                            HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                //                        }
                //                        price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                //                    }
                //                }
                //                sub_id = DBQueries.InsertSub(user_msisdn, service.service_id.ToString(), price_c.price_id, created_at, created_at, channel_id.ToString(), "", "DBConnectionString_175", ref lines);
                //                if (price_c != null)
                //                {
                //                    Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ",now(), " + price_c.price_id + ")", ref lines);
                //                }
                //                Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_misc (subscriber_id, channel_name) values(" + sub_id + ",'" + channel_name + "')", ref lines);
                //                description = (sub_id > 0 ? "OK" : "NOK");
                //                lines = Add2Log(lines, "Adding Subscriber " + user_msisdn + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                //                CommonFuncations.Notifications.SendSubscriptionNotification(user_msisdn, service.service_id.ToString(), created_at, ref lines);
                //                if (billing_rate != "0")
                //                {
                //                    CommonFuncations.Notifications.SendBillingNotification(user_msisdn, service.service_id.ToString(), created_at, (Convert.ToDouble(billing_rate) / 100).ToString(), ref lines);
                //                }
                //                DBQueries.ExecuteQuery("insert into subscribers_telekom (subscriber_id, telekom_sub_id) values(" + sub_id + "," + subscription_id + ")", ref lines);
                //                if (!String.IsNullOrEmpty(ext_ref))
                //                {
                //                    Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set msisdn = " + user_msisdn + ", subscriber_id = " + sub_id + " where id = " + ext_ref, ref lines);
                //                }
                //            }
                //        }
                //    }

                //}
                //catch (Exception e)
                //{
                //    lines = Add2Log(lines, "Exception " + e.ToString(), 100, "sub_callbacks");
                //}
            }
            lines = Write2Log(lines);
            string response_soap = "ok";
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

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