using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.CommonFuncations.Amplitude;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for STS
    /// </summary>
    public class STS : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "STS");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "STS");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "STS");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "STS");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "STS");
            }
            //ClientEndpoint?type={}&date={}action={}&msisdn={}network={}guid={}&subscriberid={}&ref={}

            string type = context.Request.QueryString["type"];
            string date = context.Request.QueryString["date"];
            string action = context.Request.QueryString["action"];
            string msisdn = context.Request.QueryString["msisdn"];
            string network = context.Request.QueryString["network"];
            string guid = context.Request.QueryString["guid"];
            string subscriberid = context.Request.QueryString["subscriberid"];
            string ref_param = context.Request.QueryString["ref"];
            string charge = context.Request.QueryString["charge"];

            bool user_was_added = false;
            string real_subid = "0";
            string tracking_id = string.Empty;

            if (type == "Navigation")
            {
                ServiceClass service = GetServiceList(ref lines).Find(x => x.spid_password == guid);
                ServiceURLS service_urls = GetServiceURLS(service.service_id, ref lines);
                if (service_urls != null && action == "Redirect")
                {
                    string myurl = service_urls.service_url + "?cli=" + Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1);
                    lines = Add2Log(lines, " redirecting to " + myurl, 100, "STS");
                    lines = Write2Log(lines);
                    context.Response.Redirect(myurl);
                }
            }
            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(ref_param) && !String.IsNullOrEmpty(action) && !String.IsNullOrEmpty(subscriberid) && !String.IsNullOrEmpty(guid))
            {
                string mydatetime = date.Replace(" AM", "").Replace(" PM", "");
                ServiceClass service = GetServiceList(ref lines).Find(x => x.spid_password == guid);
                if (service != null)
                {
                    real_subid = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from sts_subscribers where sts_subid = " + subscriberid, ref lines);
                    if (String.IsNullOrEmpty(real_subid))
                    {
                        real_subid = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values("+msisdn+","+service.service_id+",'"+ mydatetime + "',1)", ref lines).ToString();
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into sts_subscribers (sts_subid, subscriber_id, network_id) values("+subscriberid+","+real_subid+","+network+")", ref lines);
                        user_was_added = true;
                    }
                }


                if (user_was_added)
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("update requests set msisdn = " + msisdn + ", round_trip_datetime = now() where req_id = " + ref_param, ref lines);
                    tracking_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select transaction_id from requests where req_id = " + ref_param, ref lines);
                    if (!String.IsNullOrEmpty(tracking_id))
                    {
                        if (tracking_id.Contains("Track_"))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set msisdn = " + msisdn + ", subscriber_id = "+ real_subid + " where id = " + tracking_id.Replace("Track_", ""), ref lines);
                        }
                    }
                }
                ServiceURLS service_urls = GetServiceURLS(service.service_id, ref lines);
                switch (type)
                {
                    case "Navigation":
                        if (service_urls != null && action == "Redirect")
                        {
                            string myurl = service_urls.service_url + "?cli=" + Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1);
                            lines = Add2Log(lines, " redirecting to " + myurl, 100, "STS");
                            lines = Write2Log(lines);
                            context.Response.Redirect(myurl);
                        }
                        break;
                    case "Sync":
                        if (service_urls != null)
                        {
                            if (action == "Deletion")
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = '"+ mydatetime + "' where subscriber_id = " + real_subid, ref lines);
                                lines = Add2Log(lines, " redirecting to " + service_urls.error_url, 100, "STS");
                                lines = Write2Log(lines);
                                context.Response.Redirect(service_urls.error_url);
                            }
                            if (action == "AlreadySubscribed" || action == "Addition")
                            {
                                string myurl = service_urls.service_url + "?cli=" + Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1);
                                lines = Add2Log(lines, " redirecting to " + myurl, 100, "STS");
                                lines = Write2Log(lines);
                                context.Response.Redirect(myurl);
                            }
                        }
                        break;
                    case "Billing":
                        if (action == "BillingSuccess")
                        {
                            PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge), ref lines);
                            if (service_price == null)
                            {
                                Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge + ",'ZAR','R'," + charge + ")", ref lines);
                                if (price_id > 0)
                                {
                                    List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                    if (result_list != null)
                                    {
                                        HttpContext.Current.Application["PriceList"] = result_list;
                                        HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                    }
                                    service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge), ref lines);
                                }
                            }
                            if (service_price != null)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + real_subid + ", '" + mydatetime + "'," + service_price.price_id + ")", ref lines);
                            }
                        }
                        
                        break;
                }

                // Add Amplitude hook
                List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

                Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
                {
                    msisdn = Convert.ToInt64(msisdn),
                    task = "STS-billing",
                    service_id = service.service_id,
                    retcode = 1,
                    amount = Convert.ToDouble(charge),
                    result_msg = "type = " + type + ", action = " + action + ", network = " + network,
                    http = context.Request,
                    billing_date = DateTime.TryParse(date, out DateTime dt ) ? dt : DateTime.Now,
                    campaign_name = campaigns.Find(x => x.subscribe_service_id == service.service_id)?.view_name ?? "",     // lookup if there is a campaign active for this service id
                    service_name = service.service_name,
                    tracking_id = tracking_id
                });


            }



            lines = Write2Log(lines);

            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
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