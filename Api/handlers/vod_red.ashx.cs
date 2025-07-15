using Api.CommonFuncations;
using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.Vodacom;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for vod_red
    /// </summary>
    public class vod_red : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "vod_red");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "vod_red");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "vod_red");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "vod_red");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "vod_red");
            }

            string req_id = context.Request.QueryString["req_id"];
            string status_code = context.Request.QueryString["status-code"];
            string result = context.Request.QueryString["result"];
            string result_description = context.Request.QueryString["result-description"];
            string url_2_redirect = "", error_url = "";
            bool state = false;
            Int64 sub_id = 0;
            int db_price_id = 0;

            if (!String.IsNullOrEmpty(req_id) && !String.IsNullOrEmpty(status_code) && !String.IsNullOrEmpty(result_description))
            {
                result = (String.IsNullOrEmpty(result) ? "" : result);
                DBQueries.ExecuteQuery("update yellowdot.requests set round_trip_datetime = now(), round_trip_result = \"" + status_code + ";" + result + ";" + result_description + "\" where req_id = " + req_id, ref lines);
                string[] m_result = DBQueries.GetInfoByReqID(req_id, ref lines);
                string msisdn = m_result[0];
                string service_id = m_result[1];
                string transaction_id = m_result[2];
                ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);

                ServiceURLS service_urls = GetServiceURLS(service.service_id, ref lines);
                url_2_redirect = service_urls.service_url + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&sid=" + service.service_id;
                error_url = service_urls.error_url + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&result=" + result + "&desc=" + result_description;
                string updateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (status_code == "0" || status_code == "7")
                {
                    
                    sub_id = DBQueries.InsertSub(msisdn.ToString(), service_id.ToString(), db_price_id, updateTime, updateTime, "0", "", ref lines);
                    lines = Add2Log(lines, "Adding Subscriber " + msisdn + ", sub_id = " + sub_id , 100, "vod_red");
                    string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT v.encrypted_msisdn from vod_enrich v WHERE v.msisdn = "+msisdn+" ORDER BY v.id DESC LIMIT 1", ref lines);
                    VodSub service_offer = GetServiceOffers(service, msisdn.ToString(), enc_msisdn, ref lines);
                    if (!String.IsNullOrEmpty(service_offer.subscription_id) && sub_id > 0)
                    {
                        lines = Add2Log(lines, "Vodacom SubscritionID: " + service_offer.subscription_id, 100, "vod_red");
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_vodacom (subscriber_id,vodacom_sub_id) values("+sub_id+","+ service_offer.subscription_id + ")", ref lines);
                    }
                    else
                    {
                        lines = Add2Log(lines, "Vodacom SubscritionID was not found", 100, "vod_red");
                    }

                    CommonFuncations.Notifications.SendSubscriptionNotification(msisdn.ToString(), service.service_id.ToString(), updateTime, transaction_id, result_description, ref lines);
                    //send sms
                    ServiceBehavior.DecideBehavior(service, "1", msisdn.ToString(), sub_id, ref lines);
                    Int64 tracking_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT tr.id FROM tracking.tracking_requests tr, tracking.tracking_campaign tc WHERE tr.msisdn = "+msisdn+" AND DATEDIFF(tr.date_time, NOW()) > -1 AND tc.campaign_id = tr.campaign_id AND tc.subscribe_service_id = "+service.service_id+" AND tr.subscriber_id = 0 ORDER BY tr.id DESC LIMIT 1", ref lines);
                    if (tracking_id > 0)
                    {
                        DBQueries.ExecuteQuery("update tracking.tracking_requests set subscriber_id = " +sub_id+ " where id = " + tracking_id, ref lines);
                    }


                }
                else
                {
                    string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT v.encrypted_msisdn from vod_enrich v WHERE v.msisdn = " + msisdn + " ORDER BY v.id DESC LIMIT 1", ref lines);
                    lines = Add2Log(lines, "Sleeping for 3 seconds", 100, "vod_red");
                    Thread.Sleep(3000);
                    VodSub service_offer = GetServiceOffers(service, msisdn.ToString(), enc_msisdn, ref lines);
                    if (!String.IsNullOrEmpty(service_offer.subscription_id))
                    {
                        sub_id = DBQueries.InsertSub(msisdn.ToString(), service_id.ToString(), db_price_id, updateTime, updateTime, "0", "", ref lines);
                        lines = Add2Log(lines, "Adding Subscriber " + msisdn + ", sub_id = " + sub_id, 100, "vod_red");
                        lines = Add2Log(lines, "Vodacom SubscritionID: " + service_offer.subscription_id, 100, "vod_red");
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_vodacom (subscriber_id,vodacom_sub_id) values(" + sub_id + "," + service_offer.subscription_id + ")", ref lines);
                        CommonFuncations.Notifications.SendSubscriptionNotification(msisdn.ToString(), service.service_id.ToString(), updateTime, transaction_id, result_description, ref lines);
                        //send sms
                        ServiceBehavior.DecideBehavior(service, "1", msisdn.ToString(), sub_id, ref lines);
                        Int64 tracking_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT tr.id FROM tracking.tracking_requests tr, tracking.tracking_campaign tc WHERE tr.msisdn = " + msisdn + " AND DATEDIFF(tr.date_time, NOW()) > -1 AND tc.campaign_id = tr.campaign_id AND tc.subscribe_service_id = " + service.service_id + " AND tr.subscriber_id = 0 ORDER BY tr.id DESC LIMIT 1", ref lines);
                        if (tracking_id > 0)
                        {
                            DBQueries.ExecuteQuery("update tracking.tracking_requests set subscriber_id = " + sub_id + " where id = " + tracking_id, ref lines);
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "Vodacom SubscritionID was not found", 100, "vod_red");
                        Int64 tracking_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT tr.id FROM tracking.tracking_requests tr, tracking.tracking_campaign tc WHERE tr.msisdn = " + msisdn + " AND DATEDIFF(tr.date_time, NOW()) > -1 AND tc.campaign_id = tr.campaign_id AND tc.subscribe_service_id = " + service.service_id + " AND tr.subscriber_id = 0 ORDER BY tr.id DESC LIMIT 1", ref lines);
                        CommonFuncations.Notifications.SendSubscriptionNotification(msisdn.ToString(), service.service_id.ToString(), updateTime, transaction_id, result_description, ref lines);
                        switch (service.service_id)
                        {
                            case 704:
                            case 435:
                                url_2_redirect = "http://tracksa.ydot.co/track/DeclienetVod/" + service.service_id + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&result=" + result + "&desc=" + result_description + "&tracking_id=" + tracking_id;
                                break;
                            default:
                                url_2_redirect = error_url;
                                break;
                        }
                    }


                    
                    
                }
                
                

            }
            
            lines = Add2Log(lines, "Redirecting to " + url_2_redirect, 100, "vod_red");
            lines = Write2Log(lines);
            context.Response.Redirect(url_2_redirect);

            //context.Response.ContentType = "text/plain";
            //context.Response.Write("OK");
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