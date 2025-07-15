using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class TrackAController : Controller
    {
        // GET: Track
        public ActionResult Vod(string id)
        {
            string ViewName = "C";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "trackavod.ydot.co");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "trackavod.ydot.co");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "trackavod.ydot.co");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "trackavod.ydot.co");
            lines = Add2Log(lines, "id = " + id, 100, "trackavod.ydot.co");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "trackavod.ydot.co");
            }
            string referer = (String.IsNullOrEmpty(Request.ServerVariables["HTTP_REFERER"]) ? "-" : Request.ServerVariables["HTTP_REFERER"]);
            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);
            ViewBag.Logo = "";
            ViewBag.Text = "";
            ViewBag.Pixel = "";
            ViewBag.Title = (id == null ? "" : id);
            ViewBag.GtagEvent = "";
            string gaclient_id = "";
            ViewBag.MetaRefresh = "";
            string error_msg = Request.QueryString["desc"];
            ViewBag.ErrMSG = "";
            if (!String.IsNullOrEmpty(error_msg))
            {
                switch (error_msg)
                {
                    case "Timeout waiting for response":
                        ViewBag.ErrMSG = "There was a timeout. Please try again";
                        break;
                    case "Customer Declined":
                        ViewBag.ErrMSG = "You have declined the subscription. Please try again";
                        break;
                    case "Insufficient funds":
                        ViewBag.ErrMSG = "You have insufficient airtime, please recharge and try again";
                        break;

                }
                return View();
            }

            if (Request.Cookies["_ga"] != null)
            {
                gaclient_id = Request.Cookies["_ga"].Value;
                lines = Add2Log(lines, "_ga = " + Request.Cookies["_ga"].Value, 100, "trackavod.ydot.co");
            }
            else
            {
                lines = Add2Log(lines, "_ga was not found ", 100, "track.ydot.co");
            }

            if (campaigns != null)
            {
                CampaignTracking campaign = campaigns.Find(x => x.seo_name.ToLower() == id.ToLower());
                if (campaign != null)
                {
                    ViewName = (campaign.view_name == "" ? ViewName : campaign.view_name);
                    ViewBag.Logo = campaign.logo;
                    ViewBag.Pixel = campaign.pixel;
                    ViewBag.SubscriptionURL = "";
                    ServiceClass service = GetServiceByServiceID(campaign.subscribe_service_id, ref lines);
                    ViewBag.DisplayForm = false;
                    if (service != null)
                    {
                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"];
                        string msisdnACR = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"];
                        string msisdn1 = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VC_ACR"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                        if (msisdn != "-1")
                        {
                            string url1 = "http://80.241.216.32/vodenrich.php?hex_val=" + msisdn1;
                            lines = Add2Log(lines, "url = " + url1, 100, "Subscribe");
                            lines = Add2Log(lines, "Before msisdn", 100, "Subscribe");
                            msisdn = Api.CommonFuncations.CallSoap.GetURL(url1, ref lines);
                            lines = Add2Log(lines, "After msisdn", 100, "Subscribe");
                            lines = Add2Log(lines, "msisdn = " + msisdn, 100, "Subscribe");
                        }
                        string encoded_msisdn = (msisdn == "-1" ? "-1" : (msisdn == "NOMSISDN" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1)));
                        if (encoded_msisdn != "-1")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into vod_enrich (msisdn, encrypted_msisdn, date_time) values(" + msisdn + ", '" +
                            System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"] + "',now())", ref lines);
                            msisdn = encoded_msisdn;
                        }
                        else
                        {
                            string my_url = "http://tracksa.ydot.co/track/vod/" + id + "?cli=-1&" + Request.ServerVariables["QUERY_STRING"];
                            lines = Add2Log(lines, "Redirecting to " + my_url, 100, "Subscribe");
                            lines = Write2Log(lines);
                            return Redirect(my_url); //add query strings
                        }
                        LoginRequest LoginRequestBody = new LoginRequest()
                        {
                            ServiceID = campaign.subscribe_service_id,
                            Password = service.service_password
                        };
                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                        if (res != null)
                        {
                            if (res.ResultCode == 1000)
                            {
                                string token_id = res.TokenID;
                                lines = Add2Log(lines, " token_id = " + token_id, 100, "track.ydot.co");
                                lines = Add2Log(lines, " SubscribingUser", 100, "MO");
                                string trans_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select v.encrypted_msisdn from vod_enrich v where v.msisdn = " + msisdn + " order by id desc limit 1", ref lines); 
                                SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                {
                                    ServiceID = campaign.subscribe_service_id,
                                    TokenID = token_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    TransactionID = trans_id,
                                    GetURL = "1"
                                };
                                SubscribeResponse subscribe_response = Api.CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
                                lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "track.ydot.co");

                                switch (subscribe_response.ResultCode)
                                {
                                    case 3011:
                                    case 3010:
                                        ViewBag.GtagEvent = "gtag('event', 'Started', { 'event_category': 'Subscription', 'event_label': 'Subscription USSD', 'value': '1'});";
                                        ViewBag.Text = campaign.user_is_already_subscribed;
                                        if (!String.IsNullOrEmpty(campaign.url_behind))
                                        {
                                            if (campaign.url_behind.Contains("http:"))
                                            {
                                                if (subscribe_response.ResultCode == 3011)
                                                {
                                                    Int64 sub_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select subscriber_id from subscribers s where s.msisdn = " + msisdn + " and service_id = " + campaign.subscribe_service_id + " order by s.subscriber_id desc limit 1", ref lines);
                                                    if (sub_id > 0)
                                                    {
                                                        string mysqlQ2 = "insert into tracking.tracking_requests (date_time, campaign_id, msisdn, ip, user_agent, query_string,subscription_retcode,subscription_description, ga_client_id, referer, subscriber_id) values(now(), " + campaign.campaign_id + "," + msisdn + ",'" + Request.ServerVariables["REMOTE_ADDR"] + "', '" + Request.ServerVariables["HTTP_USER_AGENT"].Replace("'", "''") + "','" + Request.ServerVariables["QUERY_STRING"].Replace("'", "''") + "', 1302, '" + subscribe_response.Description + "', '" + gaclient_id + "', '" + referer + "', " + sub_id + ")";
                                                        Api.DataLayer.DBQueries.ExecuteQuery(mysqlQ2, ref lines);
                                                    }
                                                }
                                                string redirect_url = (campaign.url_behind.Contains("?") ? campaign.url_behind + "&cli=" + Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1) : campaign.url_behind + "?cli=" + Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                                                lines = Add2Log(lines, " user is already subscribed redirecting to " + redirect_url, 100, "track.ydot.co");
                                                lines = Write2Log(lines);
                                                return Redirect(redirect_url);
                                            }
                                        }
                                        break;
                                    case 1010:
                                        ViewBag.GtagEvent = "gtag('event', 'Started', { 'event_category': 'Subscription', 'event_label': 'Subscription USSD', 'value': '1'});";
                                        if (campaign.is_doi == true)
                                        {
                                            ViewBag.Text = campaign.ok_text_after_subscription_wdoi;
                                        }
                                        else
                                        {
                                            ViewBag.Text = campaign.ok_text_after_subscription_wo_doi;
                                        }
                                        break;
                                    case 1302:
                                        lines = Add2Log(lines, "Redirecting to " + subscribe_response.URL, 100, "track.ydot.co");
                                        ViewBag.SubscriptionURL = subscribe_response.URL;
                                        string mysqlQ1 = "insert into tracking.tracking_requests (date_time, campaign_id, msisdn, ip, user_agent, query_string,subscription_retcode,subscription_description, ga_client_id, referer) values(now(), " + campaign.campaign_id + "," + msisdn + ",'" + Request.ServerVariables["REMOTE_ADDR"] + "', '" + Request.ServerVariables["HTTP_USER_AGENT"].Replace("'", "''") + "','" + Request.ServerVariables["QUERY_STRING"].Replace("'", "''") + "', " + subscribe_response.ResultCode + ", '" + subscribe_response.Description + "', '" + gaclient_id + "', '" + referer + "')";
                                        Api.DataLayer.DBQueries.ExecuteQuery(mysqlQ1, ref lines);
                                        lines = Add2Log(lines, "Redirecting to " + subscribe_response.URL, 100, "track.ydot.co");
                                        lines = Write2Log(lines);
                                        return Redirect(subscribe_response.URL);
                                        //ViewBag.MetaRefresh = "<meta http-equiv=\"refresh\" content=\"2;URL='" + subscribe_response.URL + "'\">";
                                        break;
                                    default:
                                        ViewBag.Text = campaign.ko_text_after_subscription;
                                        break;
                                }

                                string mysqlQ = "insert into tracking.tracking_requests (date_time, campaign_id, msisdn, ip, user_agent, query_string,subscription_retcode,subscription_description, ga_client_id, referer) values(now(), " + campaign.campaign_id + "," + msisdn + ",'" + Request.ServerVariables["REMOTE_ADDR"] + "', '" + Request.ServerVariables["HTTP_USER_AGENT"].Replace("'", "''") + "','" + Request.ServerVariables["QUERY_STRING"].Replace("'", "''") + "', " + subscribe_response.ResultCode + ", '" + subscribe_response.Description + "', '" + gaclient_id + "', '" + referer + "')";
                                Api.DataLayer.DBQueries.ExecuteQuery(mysqlQ, ref lines);
                            }
                            else
                            {
                                lines = Add2Log(lines, "Login has failed " + res.ResultCode + " - " + res.Description, 100, "track.ydot.co");
                            }
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "Services are empty", 100, "track.ydot.co");
                    }
                }
                else
                {
                    lines = Add2Log(lines, "Campaign was not found", 100, "track.ydot.co");
                }
            }
            else
            {
                lines = Add2Log(lines, "Campaigns are empty", 100, "track.ydot.co");
            }

            lines = Write2Log(lines);
            return View(ViewName);
        }
    }
}