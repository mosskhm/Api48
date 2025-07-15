using Api.CommonFuncations;
using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Cache.SMS;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for wapnotify
    /// </summary>
    public class wapnotify : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "wapnotification");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "wapnotification");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "wapnotification");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "wapnotification");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "wapnotification");
            }

            string result = context.Request.QueryString["result"];
            string authToken = context.Request.QueryString["authToken"];
            string tokenExpiryTime = context.Request.QueryString["tokenExpiryTime"];
            string description = context.Request.QueryString["description"];
            string transactionId = context.Request.QueryString["transactionId"];
            string y_id = context.Request.QueryString["y_id"];
            string url_2_redirect = "", error_url = "";
            bool state = false;

            if (!String.IsNullOrEmpty(y_id) && !String.IsNullOrEmpty(result))
            {
                DBQueries.ExecuteQuery("update yellowdot.requests set round_trip_datetime = now(), round_trip_result = \""+ result + ";" + (String.IsNullOrEmpty(description) ? "" : description) + "\", auth_token = \""+ (String.IsNullOrEmpty(authToken) ? "" : authToken) + "\" where req_id = " + y_id, ref lines);
                Int64[] m_result = DBQueries.GetMSISDNByReqID(y_id, ref lines);
                Int64 msisdn = m_result[0];
                Int64 service_id = m_result[1];
                ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);

                ServiceURLS service_urls = GetServiceURLS(service.service_id, ref lines);
                url_2_redirect = service_urls.service_url + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&sid=" + service.service_id;
                error_url = service_urls.error_url + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&result="+ result +"&desc="+description;

                if (msisdn > 0 && service != null && !String.IsNullOrEmpty(authToken))
                {
                    

                    string token_id = "";
                    LoginRequest RequestBody = new LoginRequest()
                    {
                        ServiceID = service.service_id,
                        Password = service.service_password
                    };
                    LoginResponse response = Login.DoLogin(RequestBody);
                    if (response != null)
                    {
                        if (response.ResultCode == 1000)
                        {
                            token_id = response.TokenID;
                            lines = Add2Log(lines, " token_id = " + token_id, 100, "wapnotification");
                            if(service.is_ondemand == false)
                            {
                                SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                {
                                    ServiceID = service.service_id,
                                    TokenID = token_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    TransactionID = "WapNotify123456",
                                    ActivationID = "0",
                                    AuthrizationID = authToken

                                };
                                lines = Add2Log(lines, " Subscribing user " + msisdn, 100, "wapnotification");
                                SubscribeResponse subscribe_response = CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
                                lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "wapnotification");

                                if (subscribe_response.ResultCode == 3010)
                                {
                                    if (service_urls != null)
                                    {
                                        state = true;
                                        lines = Add2Log(lines, " Redirecting to " + url_2_redirect, 100, "wapnotification");
                                    }
                                }
                                else
                                {
                                    if (service_urls != null)
                                    {
                                        error_url = service_urls.error_url + "?cli=" + Base64.EncodeDecodeBase64(msisdn.ToString(), 1) + "&result=" + result + "&desc=" + description;
                                        lines = Add2Log(lines, " Redirecting to " + error_url, 100, "wapnotification");
                                    }
                                }
                            }
                            else
                            {
                                SMSTexts sms_texts = GetSMSText(service, ref lines);
                                if (sms_texts != null)
                                {
                                    SendSMSRequest sms_RequestBody = new SendSMSRequest()
                                    {
                                        ServiceID = service.service_id,
                                        TokenID = token_id,
                                        MSISDN = Convert.ToInt64(msisdn),
                                        TransactionID = "WapNotify123456",
                                        AuthrizationID = authToken,
                                        Text = sms_texts.welcome_sms
                                    };
                                    lines = Add2Log(lines, " Sending SMS to " + msisdn + " - " + sms_texts.welcome_sms, 100, "wapnotification");
                                    SendSMSResponse sms_ResponseBody = SendSMS.DoSMS(sms_RequestBody);
                                    lines = Add2Log(lines, " ResultCode = " + sms_ResponseBody.ResultCode + ", Description = " + sms_ResponseBody.Description, 100, "wapnotification");
                                    //add user to subscriber db
                                    //add user to billing db
                                    Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values(" + msisdn + ", " + service.service_id + ", now(), 3, 0, '' )", ref lines);
                                    if (sub_id > 0)
                                    {
                                        PriceClass price_info = GetPricesInfo(service.service_id, 0, ref lines);
                                        if (price_info != null)
                                        {
                                            Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ", now(), " + price_info.price_id + ");", ref lines);
                                        }
                                        //msisdn=2349038125089&amp;dateTime=2019-03-20 13:13:00&amp;amount=20&amp;serviceId=685
                                        if (service.service_id == 9 || service.service_id == 685)
                                        {
                                            string add_draw_url = Cache.ServerSettings.GetServerSettings("LuckyNumberODAdd2DrawURL", ref lines) + "msisdn=" + msisdn + "&dateTime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "&amount=20&servicId=" + service.service_id;
                                            lines = Add2Log(lines, " Calling " + add_draw_url, 100, lines[0].ControlerName);
                                            string response1 = CallSoap.GetURL(add_draw_url, ref lines);
                                            lines = Add2Log(lines, " response = " + response1, 100, lines[0].ControlerName);
                                        }
                                        

                                    }
                                }
                                
                            }
                  
                            
                        }
                    }
                }

                string final_url = (state == true ? url_2_redirect : error_url);
                lines = Add2Log(lines, " Redirecting to " + error_url, 100, "wapnotification");
                lines = Write2Log(lines);
                context.Response.Redirect(final_url);
            }

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