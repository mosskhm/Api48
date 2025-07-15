using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for smsreceive_benin
    /// </summary>
    public class smsreceive_benin : IHttpHandler
    {
        
        

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "SMSReceiveBenin");
            lines = Add2Log(lines, "Incomming Post = " + xml, 100, "SMSReceiveBenin");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "SMSReceiveBenin");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "SMSReceiveBenin");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "SMSReceiveBenin");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "SMSReceiveBenin");
            }
            string msisdn = (String.IsNullOrEmpty(context.Request.QueryString["from"]) ? "" : context.Request.QueryString["from"].Replace("+", "").Replace("%2B", ""));
            string short_code = context.Request.QueryString["to"];
            string text = context.Request.QueryString["text"];
            string mobile_operator = context.Request.QueryString["op"];

            if (!String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(short_code) && (context.Request.ServerVariables["REMOTE_ADDR"] == "185.167.99.119" || context.Request.ServerVariables["REMOTE_ADDR"] == "185.167.99.120" || context.Request.ServerVariables["REMOTE_ADDR"] == "::1"))
            {
                Benin.returned_service r_service = null;
                if (text == "1" || text == "2")
                {
                    string token1 = "";
                    ServiceClass service1 = GetServiceByServiceID(709, ref lines);
                    LoginRequest LoginRequestBody1 = new LoginRequest()
                    {
                        ServiceID = service1.service_id,
                        Password = service1.service_password
                    };
                    LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
                    if (res1 != null)
                    {
                        if (res1.ResultCode == 1000)
                        {
                            token1 = res1.TokenID;
                        }
                    }
                    switch (text)
                    {
                        case "1":
                            
                            DYAReceiveMoneyRequest momo_request1 = new DYAReceiveMoneyRequest()
                            {
                                MSISDN = Convert.ToInt64(msisdn),
                                Amount = service1.airtimr_amount,
                                ServiceID = service1.service_id,
                                TokenID = token1,
                                TransactionID = Guid.NewGuid().ToString(),
                                Delay = "1500"

                            };
                            DYAReceiveMoneyResponse momo_response1 = CommonFuncations.DYAReceiveMoney.DoReceive(momo_request1);
                            if (momo_response1 != null)
                            {
                                if (momo_response1.ResultCode == 1010)
                                {
                                    r_service = new Benin.returned_service()
                                    {
                                        returned_sms_text = "Yello! Pour terminer votre paiement, confirmez le paiement ou composez le *880#, puis sélectionnez (8).",
                                        service_id = service1.service_id,
                                        token_id = token1
                                    };
                                    
                                }
                                else
                                {
                                    r_service = new Benin.returned_service()
                                    {
                                        returned_sms_text = "Cher abonné, votre inscription à la promo MTN Millionnaire a échoué. Veuillez réessayer plus tard.",
                                        service_id = service1.service_id,
                                        token_id = token1
                                    };
                                }
                            }
                            else
                            {
                                r_service = new Benin.returned_service()
                                {
                                    returned_sms_text = "Cher abonné, votre inscription à la promo MTN Millionnaire a échoué. Veuillez réessayer plus tard.",
                                    service_id = service1.service_id,
                                    token_id = token1
                                };
                            }
                            break;
                        case "2":
                            r_service = new Benin.returned_service()
                            {
                                returned_sms_text = "Cher abonné, merci d'utiliser MTN Millionnaire",
                                service_id = service1.service_id,
                                token_id = token1
                            };
                            break;
                    }
                }
                else
                {
                    bool is_close = true;
                    r_service = Benin.DecidePerTextAndShortCode(text, short_code, msisdn, mobile_operator, out is_close, ref lines);
                }

                
                if (r_service != null)
                {
                    lines = Add2Log(lines, "service_id = " + r_service.service_id + " text = " + r_service.returned_sms_text, 100, lines[0].ControlerName);

                    SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                    {
                        ServiceID = r_service.service_id,
                        MSISDN = Convert.ToInt64(msisdn),
                        Text = r_service.returned_sms_text,
                        TokenID = r_service.token_id,
                        TransactionID = "smsreceive"
                    };
                    SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                    if (response_sendsms != null)
                    {
                        if (response_sendsms.ResultCode == 1000)
                        {
                            lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                        }
                        else
                        {
                            lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                    }
                }
            }

            lines = Write2Log(lines);
            context.Response.ContentType = "text/html";
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