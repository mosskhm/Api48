using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;
using static Api.CommonFuncations.Amplitude;
using Api.Cache;
using Org.BouncyCastle.Asn1.Ocsp;
using static Api.DataLayer.DBQueries;

namespace Api.DEP
{
    /// <summary>
    /// Summary description for Subscription
    /// </summary>
    public class Subscription : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            bool send_subscription_notification = false;
            bool send_billing_notification = false;
            bool send_unsubscribe_notification = false;
            string user_msisdn = "";
            string channel_name = "";
            List<ServiceClass> services = null;
            string amount = "";
            PriceClass service_price = null;
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DEP_subscription");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "DEP_subscription");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "DEP_subscription");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "DEP_subscription");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "DEP_subscription");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "DEP_subscription");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
            string myip = context.Request.ServerVariables["REMOTE_ADDR"];
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "orange_guinea_billing");
            }


            try
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                string real_subid = "0";
                bool user_was_added = false;
                if (!xml.Contains("amount"))
                {
                    string svc_id = json_response.svc_id;
                    string user_id = json_response.user_id;
                    string status_id = json_response.status_id;

                    string status_name = json_response.status_name;

                    string created_at = json_response.created_at;
                    string mytime = Convert.ToDateTime(created_at).ToString("yyyy-MM-dd HH:mm:ss");
                    user_msisdn = json_response.user_msisdn;
                    string ext_ref = json_response.ext_ref;
                    channel_name = json_response.channel_name;
                    string subscription_id = json_response.subscription_id;

                    services = GetServiceList(ref lines);
                    ServiceClass service = services.Find(x => x.spid == svc_id);
                    
                    if (service != null)
                    {
                        //2022-05-26 13:54:32.538: IP = 

                        if (status_name != "PENDING")
                       {
                            lines = Add2Log(lines, "Service was found " + service.service_name, 100, "orange_guinea_billing");
                            real_subid = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from dep_subscribers where dep_subscription_id = " + subscription_id, ref lines);
                            if (String.IsNullOrEmpty(real_subid))
                            {
                                //2022-01-31T09:41:22+02:00
                                
                                real_subid = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + user_msisdn + "," + service.service_id + ",'" + mytime + "',1)", ref lines).ToString();

                                Api.DataLayer.DBQueries.ExecuteQuery("insert into dep_subscribers (dep_subscription_id, subscriber_id, channel_name) values(" + subscription_id + "," + real_subid + ",'" + channel_name + "')", ref lines);

                                //2025-09-16 Because Subscription.ashx.cs is being used for billing events so Subscribe event to amplitude should only happen if user doesn't already exist in dep_subscribers
                                // Add Amplitude hook
                                List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                                Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                                {
                                    msisdn = Convert.ToInt64(user_msisdn),
                                    task = "DEP-subscribe",
                                    service_id = service.service_id,
                                    retcode = Convert.ToInt32(status_id),
                                    amount = Convert.ToDouble(amount),
                                    result_msg = "result = " + json_response.result_name + ", status = " + status_name,
                                    http = context.Request,
                                    billing_date = DateTime.TryParse(created_at, out DateTime dta) ? dta : DateTime.Now,
                                    campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                                    service_name = service.service_name,
                                    channel = channel_name,
                                    tracking_id = json_response.ext_ref
                                });

                                if (!String.IsNullOrEmpty(ext_ref))
                                {
                                    Int64 number;

                                    bool success = Int64.TryParse(ext_ref, out number);
                                    if (success)
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests tr set tr.msisdn = " + user_msisdn + ", subscriber_id = " + real_subid + " where id = " + ext_ref, ref lines);
                                    }
                                }
                                send_subscription_notification = true;
                                user_was_added = true;
                            }
                        }
                        if ((status_name.Contains("EXPIRED") || status_name.Contains("CANCEL")) && real_subid != "0")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + real_subid, ref lines);
                            send_unsubscribe_notification = true;
                            // Add Amplitude hook
                            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                            Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                            {
                                msisdn = Convert.ToInt64(user_msisdn),
                                task = "DEP-unsubscribe",
                                service_id = service.service_id,
                                retcode = Convert.ToInt32(status_id),
                                result_msg = "result = none, status = " + status_name,
                                http = context.Request,
                                billing_date = DateTime.TryParse(created_at, out DateTime dt) ? dt : DateTime.Now,
                                campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                                service_name = service.service_name,
                                channel = channel_name,
                                tracking_id = json_response.ext_ref

                            });
                        }
                        /*
                        // Add Amplitude hook
                        /*List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                        Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                        {
                            msisdn = Convert.ToInt64(user_msisdn),
                            task = "DEP-unsubscribe",
                            service_id = service.service_id,
                            retcode = Convert.ToInt32(status_id),
                            result_msg = "result = none, status = " + status_name,
                            http = context.Request,
                            billing_date = DateTime.TryParse(created_at, out DateTime dt) ? dt : DateTime.Now,
                            campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                            service_name = service.service_name,
                            channel = channel_name,
                            tracking_id = json_response.ext_ref

                        });*/

                    }
                    else
                    {
                        lines = Add2Log(lines, "Service was not found", 100, "orange_guinea_billing");
                    }

                }
                else
                {
                    amount = json_response.amount;
                    string result_name = json_response.result_name;

                    string svc_id = json_response.subscription.svc_id;
                    
                    string user_id = json_response.subscription.user_id;
                    string status_id = json_response.subscription.status_id;

                    string status_name = json_response.subscription.status_name;

                    string created_at = json_response.subscription.created_at;
                    string mytime = Convert.ToDateTime(created_at).ToString("yyyy-MM-dd HH:mm:ss");
                    user_msisdn = json_response.subscription.user_msisdn;
                    channel_name = json_response.subscription.channel_name;
                    string subscription_id = json_response.subscription.subscription_id;

                    services = GetServiceList(ref lines);
                    ServiceClass service = services.Find(x => x.spid == svc_id);

                    if (service != null)
                    {
                        lines = Add2Log(lines, "Service was found " + service.service_name, 100, "orange_guinea_billing");
                        real_subid = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from dep_subscribers where dep_subscription_id = " + subscription_id, ref lines);

                        //2025-09-11: if user already exists in dep_subscribers then this is a billing event that should go to Amplitude
                        if (!String.IsNullOrEmpty(real_subid))
                        {
                            // Add Amplitude hook
                            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                            Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                            {
                                msisdn = Convert.ToInt64(user_msisdn),
                                task = "DEP-billing",
                                service_id = service.service_id,
                                retcode = Convert.ToInt32(status_id),
                                amount = Convert.ToDouble(amount),
                                result_msg = "result = " + result_name + ", status = " + status_name,
                                http = context.Request,
                                billing_date = DateTime.TryParse(created_at, out DateTime dt) ? dt : DateTime.Now,
                                campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                                service_name = service.service_name,
                                channel = channel_name,
                                tracking_id = json_response.ext_ref
                            });
                        }

                        if (String.IsNullOrEmpty(real_subid))
                        {
                            //2022-01-31T09:41:22+02:00
                            real_subid = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + user_msisdn + "," + service.service_id + ",'" + mytime + "',1)", ref lines).ToString();
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dep_subscribers (dep_subscription_id, subscriber_id, channel_name) values(" + subscription_id + "," + real_subid + ",'" + channel_name + "')", ref lines);
                            user_was_added = true;
                            send_subscription_notification = true;

                            //2025-09-11 Because Subscription.ashx.cs is being used for billing events so Subscribe event to amplitude should only happen if user doesn't already exist in dep_subscribers
                            // Add Amplitude hook
                            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                            Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                            {
                                msisdn = Convert.ToInt64(user_msisdn),
                                task = "DEP-subscribe",
                                service_id = service.service_id,
                                retcode = Convert.ToInt32(status_id),
                                amount = Convert.ToDouble(amount),
                                result_msg = "result = " + result_name + ", status = " + status_name,
                                http = context.Request,
                                billing_date = DateTime.TryParse(created_at, out DateTime dt) ? dt : DateTime.Now,
                                campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                                service_name = service.service_name,
                                channel = channel_name,
                                tracking_id = json_response.ext_ref
                            });
                        }
                        if (!String.IsNullOrEmpty(amount) && result_name == "SUCCESS")
                        {
                            service_price = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                            if (service_price == null)
                            {
                                Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + amount + ",'ZAR','R'," + (Convert.ToDouble(amount) / 100) + ")", ref lines);
                                if (price_id > 0)
                                {
                                    List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                    if (result_list != null)
                                    {
                                        HttpContext.Current.Application["PriceList"] = result_list;
                                        HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                    }
                                    service_price = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                }
                            }
                            if (service_price != null)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + real_subid + ", now()," + service_price.price_id + ")", ref lines);
                                send_billing_notification = true;
                            }
                        }
                        /*
                        // Add Amplitude hook
                        List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                        Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                        {
                            msisdn = Convert.ToInt64(user_msisdn),
                            task = "DEP-subscribe",
                            service_id = service.service_id,
                            retcode = Convert.ToInt32(status_id),
                            amount = Convert.ToDouble(amount),
                            result_msg = "result = " + result_name + ", status = " + status_name,
                            http = context.Request,
                            billing_date = DateTime.TryParse(created_at, out DateTime dt) ? dt : DateTime.Now,
                            campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                            service_name = service.service_name,
                            channel = channel_name,
                            tracking_id = json_response.ext_ref
                        });*/

                    }
                    else
                    {
                        lines = Add2Log(lines, "Service was not found", 100, "orange_guinea_billing");
                    }
                }
                string my_date_time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                if (channel_name == "AVM" && send_subscription_notification)
                {
                    Api.CommonFuncations.Notifications.SendSubscriptionNotification(user_msisdn, services[0].service_id.ToString(), my_date_time, ref lines);
                }


 

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Exception: " + ex.ToString(), 100, "orange_guinea_billing");
            }



            lines = Write2Log(lines);


            context.Response.ContentType = "text/plain";
            context.Response.Write("ok");
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