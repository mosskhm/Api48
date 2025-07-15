using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class EnrichmentController : Controller
    {

        // GET: Enrichment

        public ActionResult TrackV(string id)
        {
            string view = "Enrich";
            if (id == "who")
            {
                List<LogLines> lines = new List<LogLines>();
                string url = "";
                lines = Add2Log(lines, "*****************************", 100, "tracke_who");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "tracke_who");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "tracke_who");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "tracke_who");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "devotional_Enrichment");
                string who_data = System.Web.HttpContext.Current.Request.Headers["Accept-Language"];
                ViewBag.WhoData = headers;
                view = "who";
                lines = Write2Log(lines);
            }

            if (!String.IsNullOrEmpty(id))
            {
                if (id.ToLower() != "who")
                {
                    string ViewName = "C";
                    List<LogLines> lines = new List<LogLines>();
                    lines = Add2Log(lines, "*****************************", 100, "trackvvod.ydot.co");
                    lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "trackvvod.ydot.co");
                    lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "trackvvod.ydot.co");
                    lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "trackvvod.ydot.co");
                    lines = Add2Log(lines, "id = " + id, 100, "trackavod.ydot.co");
                    foreach (String key in Request.QueryString.AllKeys)
                    {
                        lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "trackvvod.ydot.co");
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
                        lines = Add2Log(lines, "_ga = " + Request.Cookies["_ga"].Value, 100, "trackvvod.ydot.co");
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

                                lines = Add2Log(lines, "HTTP_X_VCZA_ACR = " + msisdn, 100, "Subscribe");
                                lines = Add2Log(lines, "HTTP_X_VC_ACR = " + msisdn1, 100, "Subscribe");

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
                                        msisdn = Api.CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 2);
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

            return View(view);

        }
        public ActionResult Enrich(string id)
        {
            string view = "Enrich";

            if (id == "trackimi")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", log_level, "HEIMI");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HEIMI");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HEIMI");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HEIMI");
                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_Y_MSISDN"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                        string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HEIMI");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HEIMI");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HEIMI");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HEIMI");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HEIMI");
                    lines = Write2Log(lines);
                }
            }

            if (id == "trackvod")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string return_url = "";

                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", log_level, "HEVod");
                        string headers = string.Empty;
                        foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                        {
                            headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                        }
                        lines = Add2Log(lines, headers, 100, "HEVod");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HEVod");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HEVod");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HEVod");
                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
						string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"];
						string msisdnACR = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"];
                        string msisdn1 = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VC_ACR"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);

                        if (msisdn != "-1")
                        {
                            string url1 = "http://80.241.216.32/vodenrich.php?hex_val=" + msisdn1;
                            lines = Add2Log(lines, "url = " + url1, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            lines = Add2Log(lines, "Before msisdn", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            msisdn = Api.CommonFuncations.CallSoap.GetURL(url1, ref lines);
                            lines = Add2Log(lines, "After msisdn", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            lines = Add2Log(lines, "msisdn = " + msisdn, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            //lines = Add2Log(lines, "Start Hex2Bin", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            //string output_file = Vodacom.Hex2Bin(msisdn, ref lines);
                            ////string output_file = Vodacom.ConvertHextoASCII(msisdn, ref lines);
                            //lines = Add2Log(lines, "Start OpenSSL", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                            //msisdn = Vodacom.OpenSSL(output_file, ref lines);
                            //lines = Add2Log(lines, "End OpenSSL", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Subscribe");
                        }
                        string encoded_msisdn = (msisdn == "-1" ? "-1" : (msisdn == "NOMSISDN" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1)));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        if (msisdn != "-1")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into vod_enrich (msisdn, encrypted_msisdn, date_time) values(" + msisdn + ", '" + 
							System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VCZA_ACR"] + "',now())", ref lines);
							//System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_VC_ACR"] + "')", ref lines);
                        }
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HEVod");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HEVod");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HEVod");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HEVod");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HEVod");
                    lines = Write2Log(lines);
                }
            }

            if (id == "tracksa")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string emsisdn = Request.QueryString["emsisdn"];
                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", log_level, "HESA");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HESA");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HESA");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HESA");
                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                        if (!String.IsNullOrEmpty(emsisdn))
                        {
                            Int64 j;
                            string msisdn = "", encoded_msisdn = "";
                            if (Int64.TryParse(emsisdn, out j))
                            {
                                msisdn = CommonFuncations.VodacomIMI.DecryptMSISDN(true, Convert.ToInt64(emsisdn), ref lines);
                            }
                            else
                            {
                                msisdn = "";
                            }

                            encoded_msisdn = (msisdn == "" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                            url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                            lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HESA");
                            lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HESA");
                            lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HESA");
                        }
                        else
                        {
                            string my_redurl = "http://ydplatform.com/enrichment/enrich/tracksa" + (String.IsNullOrEmpty(Request.ServerVariables["QUERY_STRING"]) == true ? "" : "?" + Request.ServerVariables["QUERY_STRING"]);
                            url = "http://apollo.365.co.za:9092/identify?redirect=" + my_redurl;
                            lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HESA");
                        }
                        
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HESA");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HESA");
                    lines = Write2Log(lines);
                }
            }

            if (id == "trackng")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string opera = (String.IsNullOrEmpty(Request.QueryString["pingback"]) ? "" : Request.QueryString["pingback"]);
                if (opera == "1")
                {
                    //string filePath = Server.MapPath("~/opera/PingMirror.aspx");
                    string[] paramNames = Request.QueryString.AllKeys;
                    ViewBag.paramNames = paramNames;
                    return View("opera_ng");
                }
                else
                {
                    string enc_param = Request.QueryString["p"];
                    string return_url = "";
                    if (!String.IsNullOrEmpty(enc_param))
                    {
                        return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                        if (return_url != "")
                        {
                            string url = "";

                            var msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"] ?? System.Web.HttpContext.Current.Request.QueryString["cli"];
                            msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : (msisdn.Any(c => char.IsLetter(c)) ? CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 2): msisdn));
                            string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));

                            lines = Add2Log(lines, "*****************************", log_level, "HENG");
                            lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HENG");
                            lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HENG");
                            lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HENG");
                            lines = Add2Log(lines, "MSISDN = " + msisdn, 100, "HENG");
                            string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                            url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                            lines = Add2Log(lines, "msisdn = " + msisdn, 100, "track1");
                            lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HENG");
                            lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HENG");
                            lines = Write2Log(lines);
                            return Redirect(url);
                        }
                        else
                        {
                            lines = Add2Log(lines, "p decode is empty = ", 100, "HENG");
                            lines = Write2Log(lines);
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "p is empty = ", 100, "HENG");
                        lines = Write2Log(lines);
                    }
                }
                
                
            }

            if (id == "trackic")
            {
                List<LogLines> lines = new List<LogLines>();
                lines = Add2Log(lines, "*****************************", 100, "HEIC_GH");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HEIC_GH");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HEIC_GH");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HEIC_GH");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "HEIC_GH");

                string enc_param = Request.QueryString["p"];
                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_YELLOWDOT_MSISDN"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                        string g_msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                        msisdn = (!String.IsNullOrEmpty(g_msisdn) ? g_msisdn : msisdn);
                        string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HEIC_GH");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HEIC_GH");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HEIC_GH");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HEIC_GH");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HEIC_GH");
                    lines = Write2Log(lines);
                }
                lines = Write2Log(lines);
            }

            if (id == "opera_ng")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string return_url = "";

                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "opera_ng");
                string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                ViewBag.WhoData = msisdn;
                view = "opera_ng";
                lines = Write2Log(lines);

            }

            if (id == "trackbn")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", log_level, "HEBN");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HEBN");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HEBN");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HEBN");

                        string headers = string.Empty;
                        foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                        {
                            headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                        }
                        lines = Add2Log(lines, headers, 100, "HEBN");


                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_MSISDN"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                        msisdn = (msisdn == "-1" ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"] : msisdn);
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);

                        string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HEBN");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HEBN");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HEBN");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HEBN");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HEBN");
                    lines = Write2Log(lines);
                }
            }

            if (id == "trackmmng")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string token = Request.QueryString["t"];

                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param) && token == ConfigurationManager.AppSettings["trackmmngPassword"])
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", log_level, "trackmmng");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "trackmmng");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "trackmmng");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "trackmmng");
                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                        string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "trackmmng");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "trackmmng");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "trackmmng");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "trackmmng");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "trackmmng");
                    lines = Write2Log(lines);
                }
            }


            if (id == "ngydgames")
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string url = "";
                lines = Add2Log(lines, "*****************************", log_level, "Nigeria_Enrichment");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "Nigeria_Enrichment");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                url = (String.IsNullOrEmpty(referer) == true ? "http://www.ydgames.co/nigeria/index" : referer);
                url = (url.Contains("?") == true ? url + "&cli=" + encoded_msisdn : url + "?cli=" + encoded_msisdn);
                lines = Add2Log(lines, "msisdn = " + msisdn, 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "Nigeria_Enrichment");
                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "Nigeria_Enrichment");
                lines = Write2Log(lines);
                return Redirect(url);
            }

            if (id == "devotional")
            {
                string ret_url = Request.QueryString["ret_url"];
                if (!String.IsNullOrEmpty(ret_url))
                {
                    ret_url = HttpUtility.UrlDecode(ret_url);
                }
                else
                {
                    ret_url = "http://dev_test.ydplatform.com/";
                }

                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                List<LogLines> lines = new List<LogLines>();
                string url = "";
                lines = Add2Log(lines, "*****************************", log_level, "devotional_Enrichment");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "devotional_Enrichment");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "devotional_Enrichment");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "devotional_Enrichment");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);

                //Random rnd = new Random();
                //int rndnumber = rnd.Next(1,100);
                //msisdn = (msisdn == "-1" ? (rndnumber % 2 == 0 ? "-1" : "27784164170") : msisdn);

                string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                url = ret_url; //(String.IsNullOrEmpty(referer) == true ? ret_url : referer);
                url = (url.Contains("?") == true ? url + "&cli=" + encoded_msisdn : url + "?cli=" + encoded_msisdn);
                lines = Add2Log(lines, "msisdn = " + msisdn, 100, "devotional_Enrichment");
                lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "devotional_Enrichment");
                lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "devotional_Enrichment");
                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "devotional_Enrichment");
                lines = Write2Log(lines);
                return Redirect(url);
            }

            if (id == "who" || String.IsNullOrEmpty(id))
            {
                List<LogLines> lines = new List<LogLines>();
                string url = "";
                lines = Add2Log(lines, "*****************************", 100, "who");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "who");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "who");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "who");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, 100, "devotional_Enrichment");
                string who_data = System.Web.HttpContext.Current.Request.Headers["Accept-Language"];
                ViewBag.WhoData = who_data;
                view = "who";
                lines = Write2Log(lines);
            }

            if (id == "trackcmmtn")
            {
                List<LogLines> lines = new List<LogLines>();
                string enc_param = Request.QueryString["p"];
                string return_url = "";
                if (!String.IsNullOrEmpty(enc_param))
                {
                    return_url = CommonFuncations.Base64.EncodeDecodeBase64(enc_param, 2);
                    if (return_url != "")
                    {
                        string url = "";
                        lines = Add2Log(lines, "*****************************", 100, "HECMMTN");
                        lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "HECMMTN");
                        lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "HECMMTN");
                        lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "HECMMTN");

                        string headers = string.Empty;
                        foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                        {
                            headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                        }
                        lines = Add2Log(lines, headers, 100, "HECMMTN");


                        string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_UP_CALLING_LINE_ID"];
                        msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);

                        string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        url = (return_url.Contains("?") == true ? return_url + "&cli=" + encoded_msisdn : return_url + "?cli=" + encoded_msisdn);
                        lines = Add2Log(lines, "msisdn = " + msisdn, 100, "HECMMTN");
                        lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "HECMMTN");
                        lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "HECMMTN");
                        lines = Write2Log(lines);
                        return Redirect(url);
                    }
                    else
                    {
                        lines = Add2Log(lines, "p decode is empty = ", 100, "HECMMTN");
                        lines = Write2Log(lines);
                    }
                }
                else
                {
                    lines = Add2Log(lines, "p is empty = ", 100, "HECMMTN");
                    lines = Write2Log(lines);
                }
            }


            return View(view);
        }

        
    }
}
