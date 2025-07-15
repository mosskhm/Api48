using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Cache.SMS;
using static Api.CommonFuncations.BtoBet;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ServiceBehavior
    {
        public static void DecideBillingBehavior(ServiceClass service, string updateType, string msisdn, Int64 sub_id, ref List<LogLines> lines)
        {
            string platform_id = Cache.ServerSettings.GetServerSettings("PlatformID", ref lines);
            LoginRequest RequestBodyLNIC = new LoginRequest()
            {
                ServiceID = service.service_id,
                Password = service.service_password
            };
            LoginResponse responseLNIC = Login.DoLogin(RequestBodyLNIC);
            string token_id = "";
            if (responseLNIC != null)
            {
                if (responseLNIC.ResultCode == 1000)
                {
                    token_id = responseLNIC.TokenID;
                }
            }
            string general_txt = "";
            SendSMSRequest RequestSendSMSBodyGeneral = null;
            switch (service.service_id)
            {
                case 1077:
                    general_txt = "Welcome to Naija Soccer Star. Increase your chances of being among the finalists to win 10m by sending NS to 205 multiple times. Visit naijasoccerstars.com for more info. Thanks";
                    RequestSendSMSBodyGeneral = new SendSMSRequest()
                    {
                        ServiceID = service.service_id,
                        MSISDN = Convert.ToInt64(msisdn),
                        Text = general_txt,
                        TokenID = token_id,
                        TransactionID = "12345"
                    };
                    break;
                case 742:
                case 743:
                case 744:
                    string benin_price = "";
                    string text_msg = "";
                    string benin_validdate = "";
                    switch (service.service_id)
                    {
                        case 742:
                            benin_price = "100F/J";
                            benin_validdate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                            text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games Jour. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>";
                            break;
                        case 743:
                            benin_price = "500F/07J";
                            benin_validdate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
                            text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games pour 7 jours. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>.";
                            break;
                        case 744:
                            benin_price = "1500F/30J";
                            benin_validdate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
                            text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games pour 30 jours. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>.";
                            break;
                    }
                    string long_url = "http://cam.ydgames.co/camydgames/index?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                    string new_id = Base64.Reverse(Base64.EncodeDecodeBase64(sub_id.ToString(), 1).Replace("=", ""));
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now(), " + msisdn + ")", "DBConnectionString_104", ref lines);

                    string short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + new_id;// CreateShortURL32(sub_id, long_url, ref lines);
                    text_msg = text_msg.Replace("<BENINDATE>", benin_validdate).Replace("<SHORTURL>", short_url).Replace("<BENINPRICE>", benin_price);
                    lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                    RequestSendSMSBodyGeneral = new SendSMSRequest()
                    {
                        ServiceID = service.service_id,
                        MSISDN = Convert.ToInt64(msisdn),
                        Text = text_msg,
                        TokenID = token_id,
                        TransactionID = "12345"
                    };
                    break;
                case 1052:
                    if (!string.IsNullOrEmpty(token_id))
                    {
                        general_txt = "Congratulations,you have been selected for the audition. Expect a call from us soon. Thank you";
                        RequestSendSMSBodyGeneral = new SendSMSRequest()
                        {
                            ServiceID = service.service_id,
                            MSISDN = Convert.ToInt64(msisdn),
                            Text = general_txt,
                            TokenID = token_id,
                            TransactionID = "12345"
                        };
                    }
                    break;
                case 893: //Family Fued
                    string f_sms = HML.CreateFamilyFuedLink(msisdn, ref lines);
                    if (!String.IsNullOrEmpty(f_sms))
                    {
                        bool send_sms_f = HML.SendSMS(msisdn, f_sms, service.sms_mt_code, ref lines);
                    }
                    break;
                //case 9:
                //case 727:
                //case 728:
                //case 729:
                //    if (responseLNIC != null)
                //    {
                //        if (responseLNIC.ResultCode == 1000)
                //        {
                //            string text_msg_lnic = "";
                //            text_msg_lnic = "Welcome to Lucky Number Game. Your number has entered for the next draw to win Cash & Airtime. Follow @luckynumberofficial on IG for the draw by 6pm. Good luck!";
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg_lnic, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBodyLNIC = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg_lnic,
                //                TokenID = responseLNIC.TokenID,
                //                TransactionID = "12345"
                //            };
                //            SendSMSResponse ResponseSendSMSBodyLNIC = SendSMS.DoSMS(RequestSendSMSBodyLNIC);
                //            if (ResponseSendSMSBodyLNIC != null)
                //            {
                //                try
                //                {
                //                    if (ResponseSendSMSBodyLNIC.ResultCode == 1010 || ResponseSendSMSBodyLNIC.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                //    break;
                //case 993:
                //case 994:
                //case 995:
                //case 996:
                //case 997:
                //case 998:
                    
                //    if (responseLNIC != null)
                //    {
                //        if (responseLNIC.ResultCode == 1000)
                //        {
                //            string text_msg_lnic = "";
                //            text_msg_lnic = "Welcome to Lucky Number draw game. Your Glo number has entered for today's draw by 6pm. N100m jackpot up for grabs. Follow us on IG @luckynumberglo. Good luck!";
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg_lnic, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBodyLNIC = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg_lnic,
                //                TokenID = responseLNIC.TokenID,
                //                TransactionID = "12345"
                //            };
                //            SendSMSResponse ResponseSendSMSBodyLNIC = SendSMS.DoSMS(RequestSendSMSBodyLNIC);
                //            if (ResponseSendSMSBodyLNIC != null)
                //            {
                //                try
                //                {
                //                    if (ResponseSendSMSBodyLNIC.ResultCode == 1010 || ResponseSendSMSBodyLNIC.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                //    break;
            }
            if (RequestSendSMSBodyGeneral != null)
            {
                SendSMSResponse ResponseSendSMSBodyGeneral = SendSMS.DoSMS(RequestSendSMSBodyGeneral);
                if (ResponseSendSMSBodyGeneral != null)
                {
                    try
                    {
                        if (ResponseSendSMSBodyGeneral.ResultCode == 1010 || ResponseSendSMSBodyGeneral.ResultCode == 1000)
                        {
                            lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                        }
                        else
                        {
                            lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                        }
                    }
                    catch (Exception ex)
                    {

                        lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
                else
                {
                    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                }
            }


        }

        public static void DecideBehaviorNew(ServiceClass service, string updateType, string msisdn, Int64 sub_id, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            List<SMSServiceInfo> sms_info = Api.Cache.Services.GetSMSServiceInfo(ref lines);
            SMSServiceInfo si = sms_info.Find(x => x.service_id == service.service_id);
            if (si != null)
            {
                string token_id = "";
                LoginRequest RequestBodySMS = new LoginRequest()
                {
                    ServiceID = service.service_id,
                    Password = service.service_password
                };
                LoginResponse responseLoginSMS = Login.DoLogin(RequestBodySMS);
                if (responseLoginSMS != null)
                {
                    if (responseLoginSMS.ResultCode == 1000)
                    {
                        token_id = responseLoginSMS.TokenID;
                    }
                }
                if (!String.IsNullOrEmpty(token_id))
                {
                    string welcome_sms = si.welcome_sms;
                    welcome_sms = (updateType == "3" ? si.rebilling_sms : welcome_sms);
                    string long_url = si.long_url;

					//Replace place holders from the email
					welcome_sms = welcome_sms.Replace("[MSISDN]", msisdn);

					if (!String.IsNullOrEmpty(long_url))
                    {
                        long_url = long_url + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                        string short_url = CreateShortURL(sub_id, long_url, ref lines);
                        welcome_sms = welcome_sms.Replace("[SHORTURL]", short_url);
                        if (welcome_sms.Contains("[BENINDATE]"))
                        {
                            string benin_validdate = DateTime.Now.AddDays(1).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                            welcome_sms = welcome_sms.Replace("[BENINDATE]", benin_validdate);
                        }
                        
                    }
                    if (!String.IsNullOrEmpty(welcome_sms))
                    {
                        lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + welcome_sms, 100, lines[0].ControlerName);
                        logMessages.Add(new { message = "Sending SMS to " + msisdn + " " + welcome_sms, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = Convert.ToInt64(msisdn), logz_id = logz_id });
                        SendSMSRequest RequestSendSMSBodyLag = new SendSMSRequest()
                        {
                            ServiceID = service.service_id,
                            MSISDN = Convert.ToInt64(msisdn),
                            Text = welcome_sms,
                            TokenID = token_id,
                            TransactionID = "12345"
                        };
                        SendSMSResponse ResponseSendSMSBodyLag = SendSMS.DoSMS(RequestSendSMSBodyLag);
                        if (ResponseSendSMSBodyLag != null)
                        {
                            try
                            {
                                if (ResponseSendSMSBodyLag.ResultCode == 1010 || ResponseSendSMSBodyLag.ResultCode == 1000)
                                {
                                    lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                    logMessages.Add(new { message = "Send SMS Was OK", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = Convert.ToInt64(msisdn), logz_id = logz_id });
                                }
                                else
                                {
                                    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                    logMessages.Add(new { message = "Send SMS Failed", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = Convert.ToInt64(msisdn), logz_id = logz_id });

                                }
                            }
                            catch (Exception ex)
                            {

                                lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                                logMessages.Add(new { message = "Send SMS Failed " + ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, msisdn = Convert.ToInt64(msisdn), logz_id = logz_id });
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                            logMessages.Add(new { message = "Send SMS Failed", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = Convert.ToInt64(msisdn), logz_id = logz_id });
                        }
                    }
                    

                }

            }
            switch (service.service_id)
            {
                case 1052:
                    LoginRequest RequestBodyLag = new LoginRequest()
                    {
                        ServiceID = service.service_id,
                        Password = service.service_password
                    };
                    LoginResponse responseLag = Login.DoLogin(RequestBodyLag);
                    if (responseLag != null)
                    {
                        if (responseLag.ResultCode == 1000)
                        {
                            string lag_txt = "Congratulations,you have been selected for the audition. Expect a call from us soon. Thank you";
                            //Bravo! Welcome to Fantasy Football League. Visit http://www.eplfantasyleague.com?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&amp;sid=" + service.service_id + " to get started.";

                            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + lag_txt, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBodyLag = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = Convert.ToInt64(msisdn),
                                Text = lag_txt,
                                TokenID = responseLag.TokenID,
                                TransactionID = "12345"
                            };
                            SendSMSResponse ResponseSendSMSBodyLag = SendSMS.DoSMS(RequestSendSMSBodyLag);
                            if (ResponseSendSMSBodyLag != null)
                            {
                                try
                                {
                                    if (ResponseSendSMSBodyLag.ResultCode == 1010 || ResponseSendSMSBodyLag.ResultCode == 1000)
                                    {
                                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                                }
                            }
                            else
                            {
                                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                            }
                        }
                    }
                    break;
                
                case 893: //Family Fued
                    string f_sms = HML.CreateFamilyFuedLink(msisdn, ref lines);
                    if (!String.IsNullOrEmpty(f_sms))
                    {
                        bool send_sms_f = HML.SendSMS(msisdn, f_sms, service.sms_mt_code, ref lines);
                    }
                    break;
                
            }
        }

        public static void DecideBehavior(ServiceClass service, string updateType, string msisdn, Int64 sub_id, ref List<LogLines> lines)
        {
            string platform_id = Cache.ServerSettings.GetServerSettings("PlatformID", ref lines);
            switch (service.service_id)
            {
                case 1052:
                    LoginRequest RequestBodyLag = new LoginRequest()
                    {
                        ServiceID = service.service_id,
                        Password = service.service_password
                    };
                    LoginResponse responseLag = Login.DoLogin(RequestBodyLag);
                    if (responseLag != null)
                    {
                        if (responseLag.ResultCode == 1000)
                        {
                            string lag_txt = "Congratulations,you have been selected for the audition. Expect a call from us soon. Thank you";
                            //Bravo! Welcome to Fantasy Football League. Visit http://www.eplfantasyleague.com?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&amp;sid=" + service.service_id + " to get started.";

                            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + lag_txt, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBodyLag = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = Convert.ToInt64(msisdn),
                                Text = lag_txt,
                                TokenID = responseLag.TokenID,
                                TransactionID = "12345"
                            };
                            SendSMSResponse ResponseSendSMSBodyLag = SendSMS.DoSMS(RequestSendSMSBodyLag);
                            if (ResponseSendSMSBodyLag != null)
                            {
                                try
                                {
                                    if (ResponseSendSMSBodyLag.ResultCode == 1010 || ResponseSendSMSBodyLag.ResultCode == 1000)
                                    {
                                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                                }
                            }
                            else
                            {
                                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                            }
                        }
                    }
                    break;
                //case 9:
                //case 727:
                //case 728:
                //case 729:
                //    LoginRequest RequestBodyLNNG = new LoginRequest()
                //    {
                //        ServiceID = service.service_id,
                //        Password = service.service_password
                //    };
                //    LoginResponse responseLNNG = Login.DoLogin(RequestBodyLNNG);
                //    if (responseLNNG != null)
                //    {
                //        if (responseLNNG.ResultCode == 1000)
                //        {
                //            string text_msg_lnmtnng = "Welcome to Lucky Number Game. Your number has entered for the next draw to win Cash & Airtime. Follow @luckynumberofficial on IG for the draw by 6pm. Good luck!";
                //            //Bravo! Welcome to Fantasy Football League. Visit http://www.eplfantasyleague.com?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&amp;sid=" + service.service_id + " to get started.";
                            
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg_lnmtnng, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBodyLNIC = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg_lnmtnng,
                //                TokenID = responseLNNG.TokenID,
                //                TransactionID = "12345"
                //            };
                //            SendSMSResponse ResponseSendSMSBodyLNIC = SendSMS.DoSMS(RequestSendSMSBodyLNIC);
                //            if (ResponseSendSMSBodyLNIC != null)
                //            {
                //                try
                //                {
                //                    if (ResponseSendSMSBodyLNIC.ResultCode == 1010 || ResponseSendSMSBodyLNIC.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                //    break;
                case 893: //Family Fued
                    string f_sms = HML.CreateFamilyFuedLink(msisdn, ref lines);
                    if (!String.IsNullOrEmpty(f_sms))
                    {
                        bool send_sms_f = HML.SendSMS(msisdn, f_sms, service.sms_mt_code, ref lines);
                    }
                    break;
                //case 1017:
                //case 993:
                //case 994:
                //case 995:
                //case 996:
                //case 997:
                //case 998:
                //case 937:
                //case 938:
                //case 939:
                //    LoginRequest RequestBodyLNIC = new LoginRequest()
                //    {
                //        ServiceID = service.service_id,
                //        Password = service.service_password
                //    };
                //    LoginResponse responseLNIC = Login.DoLogin(RequestBodyLNIC);
                //    if (responseLNIC != null)
                //    {
                //        if (responseLNIC.ResultCode == 1000)
                //        {
                //            string text_msg_lnic = "";
                //            //Bravo! Welcome to Fantasy Football League. Visit http://www.eplfantasyleague.com?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&amp;sid=" + service.service_id + " to get started.";
                //            switch(service.service_id)
                //            {
                //                case 937:
                //                    text_msg_lnic = "Vous etes sur le point de gagner 1.000.000FCFA CASH au prochain tirage de Numero Bonheur.Suivez le lien https://numerobonheur.com/ Bonne chance!";
                //                    break;
                //                case 938:
                //                    text_msg_lnic = "Vous etes sur le point de gagner 500.000FCFA CASH au prochain tirage de Numero Bonheur.Suivez le lien https://numerobonheur.com/ Bonne chance!";
                //                    break;
                //                case 939:
                //                    text_msg_lnic = "Vous etes sur le point de gagner 250.000FCFA CASH au prochain tirage de Numero Bonheur.Suivez le lien https://numerobonheur.com/ Bonne chance!";
                //                    break;
                //                case 1017:
                //                    text_msg_lnic = "Vous etes sur le point de gagner 500.000FCFA CASH au prochain tirage de Numero Bonheur.Suivez le lien https://numerobonheur.com/ Bonne chance!";
                //                    break;
                //                default:
                //                    //text_msg_lnic = "You're one step closer to winning cash and airtime! You've qualified for the next draw. Follow on IG @luckynumberofficial to watch today’s live draw by 6PM ! Good luck!";
                //                    text_msg_lnic = "Welcome to Lucky Number draw game. Your Glo number has entered for today's draw by 6pm. N100m jackpot up for grabs. Follow us on IG @luckynumberglo. Good luck!";
                //                    break;
                //            }
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg_lnic, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBodyLNIC = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg_lnic,
                //                TokenID = responseLNIC.TokenID,
                //                TransactionID = "12345"
                //            };
                //            SendSMSResponse ResponseSendSMSBodyLNIC = SendSMS.DoSMS(RequestSendSMSBodyLNIC);
                //            if (ResponseSendSMSBodyLNIC != null)
                //            {
                //                try
                //                {
                //                    if (ResponseSendSMSBodyLNIC.ResultCode == 1010 || ResponseSendSMSBodyLNIC.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                    //break;
                case 1010:
                case 1030:
                case 1031:
                case 1032:
                    if (platform_id == "1")
                    {
                        //send welcome SMS
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
                                string real_price = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT p.real_price FROM yellowdot.prices p WHERE p.service_id = " + service.service_id, ref lines);
                                token_id = response.TokenID;
                               // string long_url = "http://redirectcellc.bigcash.co.za?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                                string long_url = "https://bcgamescc.ydaplatform.com/redirect?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                                string short_url = CreateShortURL(sub_id, long_url, ref lines);
                                string text_msg = @"Thank you for subscribing to BigCash at R" + real_price + ".00/day . Play now on "+short_url+" .To manage your subscriptions dial *133*1# ";
                                lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    Text = text_msg,
                                    TokenID = token_id,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
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
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                        }
                    }
                    break;
                //case 717:
                //case 718:
                //case 719:
                //    LoginRequest RequestBodyFF = new LoginRequest()
                //    {
                //        ServiceID = service.service_id,
                //        Password = service.service_password
                //    };
                //    LoginResponse responseFF = Login.DoLogin(RequestBodyFF);
                //    if (responseFF != null)
                //    {
                //        if (responseFF.ResultCode == 1000)
                //        {
                //            string text_msg = "Bravo! Welcome to Fantasy Football League. Visit http://www.eplfantasyleague.com?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&amp;sid="+service.service_id+" to get started.";
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBodyFF = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg,
                //                TokenID = responseFF.TokenID,
                //                TransactionID = "12345"
                //            };

                //            string postBody = JsonConvert.SerializeObject(RequestSendSMSBodyFF);
                //            string url = "https://api.ydplatform.com/api/SendSMS";
                //            List<Headers> headers = new List<Headers>();
                //            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(url, postBody, headers, 2, ref lines);
                //            if (!String.IsNullOrEmpty(response_body))
                //            {
                //                try
                //                {
                //                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                //                    if (json_response.ResultCode == 1010 || json_response.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                catch (Exception ex)
                //                {

                //                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                //    break;
                //case 685:
                    //string token_id1 = "";
                    //LoginRequest RequestBody1 = new LoginRequest()
                    //{
                    //    ServiceID = service.service_id,
                    //    Password = service.service_password
                    //};
                    //LoginResponse response1 = Login.DoLogin(RequestBody1);
                    //if (response1 != null)
                    //{
                    //    if (response1.ResultCode == 1000)
                    //    {
                    //        string text_msg1 = "Welcome to LuckyNumber! Your number has been succcesfully entered in to today's draw at 20:00. You can win up to N100,000,000";
                    //        lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg1, 100, lines[0].ControlerName);
                    //        SendSMSRequest RequestSendSMSBody1 = new SendSMSRequest()
                    //        {
                    //            ServiceID = service.service_id,
                    //            MSISDN = Convert.ToInt64(msisdn),
                    //            Text = text_msg1,
                    //            TokenID = token_id1,
                    //            TransactionID = "12345"
                    //        };
                    //        SendSMSResponse response_sendsms1 = SendSMS.DoSMS(RequestSendSMSBody1, 1);
                    //        if (response_sendsms1 != null)
                    //        {
                    //            if (response_sendsms1.ResultCode == 1010)
                    //            {
                    //                lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                    //            }
                    //            else
                    //            {
                    //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                    //        }

                    //    }
                    //}
                    //break;
                //CR7 Vodacom
                //case 435:
                //    if (updateType == "1" && platform_id == "2")
                //    {
                //        string token_id = "";
                //        LoginRequest RequestBody = new LoginRequest()
                //        {
                //            ServiceID = service.service_id,
                //            Password = service.service_password
                //        };
                //        LoginResponse response = Login.DoLogin(RequestBody);
                //        if (response != null)
                //        {
                //            if (response.ResultCode == 1000)
                //            {
                //                token_id = response.TokenID;
                //                string long_url = "http://helm.tekmob.com/pim/cr7zamtn?cli=" + Base64.EncodeDecodeBase64(msisdn, 1);
                //                string short_url = CreateShortURL(sub_id, long_url, ref lines);
                //                string text_msg = "Welcome to CR7 Portal! Click here " + short_url;
                //                lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                //                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                //                {
                //                    ServiceID = service.service_id,
                //                    MSISDN = Convert.ToInt64(msisdn),
                //                    Text = text_msg,
                //                    TokenID = token_id,
                //                    TransactionID = "12345"
                //                };
                //                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                //                if (response_sendsms != null)
                //                {
                //                    if (response_sendsms.ResultCode == 1010)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                else
                //                {
                //                    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                }

                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //        else
                //        {
                //            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //        }
                //    }
                //    break;
                
                //YDTV
                case 704:
                    if (updateType == "1" && platform_id == "2")
                    {
                        //send welcome SMS
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
                                string long_url = "http://ydtv.ydot.co/ydtv/index?cli=" + Base64.EncodeDecodeBase64(msisdn, 1);
                                string short_url = CreateShortURL(sub_id, long_url, ref lines);
                                string text_msg = @"Welcome to YellowDot TV Portal! @R3.00 per day. 1st day free. Click to play " + short_url + " .To opt out click http://ydtv.ydot.co/ydtv/unsub";
                                //lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                                //SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                //{
                                //    ServiceID = service.service_id,
                                //    MSISDN = Convert.ToInt64(msisdn),
                                //    Text = text_msg,
                                //    TokenID = token_id,
                                //    TransactionID = "12345"
                                //};
                                //SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                //if (response_sendsms != null)
                                //{
                                //    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
                                //    {
                                //        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                //    }
                                //    else
                                //    {
                                //        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                //    }
                                //}
                                //else
                                //{
                                //    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                //}

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                        }
                    }
                    break;

                //ydg VOD
                case 435:
                    if (updateType == "1" && platform_id == "2")
                    {
                        //send welcome SMS
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
                                string long_url = "http://games.ydot.co/games/index?cli=" + Base64.EncodeDecodeBase64(msisdn, 1);
                                string short_url = CreateShortURL(sub_id, long_url, ref lines);
                                string text_msg = @"Welcome to YellowDot Games Portal! @R3.00 per day. 1st day free. Click to play " + short_url + " .To opt out click http://games.ydot.co/games/unsub" ;
                                //lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                                //SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                //{
                                //    ServiceID = service.service_id,
                                //    MSISDN = Convert.ToInt64(msisdn),
                                //    Text = text_msg,
                                //    TokenID = token_id,
                                //    TransactionID = "12345"
                                //};
                                //SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                //if (response_sendsms != null)
                                //{
                                //    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
                                //    {
                                //        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                //    }
                                //    else
                                //    {
                                //        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                //    }
                                //}
                                //else
                                //{
                                //    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                //}

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                        }
                    }
                    break;
                //case 946:
                //case 947:
                //case 948:
                //case 949:
                //    string token_id_pdn = "";
                //    LoginRequest RequestBody_pdn = new LoginRequest()
                //    {
                //        ServiceID = service.service_id,
                //        Password = service.service_password
                //    };
                //    LoginResponse response_pdn = Login.DoLogin(RequestBody_pdn);
                //    if (response_pdn != null)
                //    {
                //        if (response_pdn.ResultCode == 1000)
                //        {
                //            string base64msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT TO_BASE64(s.msisdn) FROM subscribers s WHERE s.service_id = 914 AND s.state_id = 1 ORDER BY s.subscriber_id DESC LIMIT 1", ref lines);
                //            string long_url = "https://ydpodcast.com/login?cli=" + base64msisdn;
                            
                //            token_id_pdn = response_pdn.TokenID;
                //            string new_id = Base64.Reverse(Base64.EncodeDecodeBase64(sub_id.ToString(), 1).Replace("=", ""));
                //            Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now(), " + msisdn + ")", "DBConnectionString_104", ref lines);

                //            string short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + new_id;// CreateShortURL32(sub_id, long_url, ref lines);
                //            string text_msg = "Welcome to PodCast Service. Click the URL to enjoy our podcasts " + short_url;
                //            lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                //            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                //            {
                //                ServiceID = service.service_id,
                //                MSISDN = Convert.ToInt64(msisdn),
                //                Text = text_msg,
                //                TokenID = token_id_pdn,
                //                TransactionID = "12345"
                //            };
                //            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                //            if (response_sendsms != null)
                //            {
                //                if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
                //                {
                //                    lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                }
                //                else
                //                {
                //                    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //            }

                //        }
                //        else
                //        {
                //            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //        }
                //    }
                //    else
                //    {
                //        lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //    }
                //    break;
                //Ydgames
                //case 80:
                //case 852:
                //case 853:
                //    if (updateType == "1")
                //    {
                //        //send welcome SMS
                //        string token_id = "";
                //        LoginRequest RequestBody = new LoginRequest()
                //        {
                //            ServiceID = service.service_id,
                //            Password = service.service_password
                //        };
                //        LoginResponse response = Login.DoLogin(RequestBody);
                //        if (response != null)
                //        {
                //            if (response.ResultCode == 1000)
                //            {
                //                token_id = response.TokenID;
                //                string text_msg = "";
                //                text_msg = "Bravo! Welcome to Be your best daily video subscription service on MTNN. Elevate your exellence towards becoming the best version of yourself. Visit <SHORTURL>";
                //                string long_url = "http://beyourbest.ydot.co/beyourbest/index?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                //                string new_id = Base64.Reverse(Base64.EncodeDecodeBase64(sub_id.ToString(), 1).Replace("=", ""));
                //                Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now(), " + msisdn + ")", "DBConnectionString_104", ref lines);

                //                string short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + new_id;// CreateShortURL32(sub_id, long_url, ref lines);
                //                text_msg = text_msg.Replace("<SHORTURL>", short_url);
                //                lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                //                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                //                {
                //                    ServiceID = service.service_id,
                //                    MSISDN = Convert.ToInt64(msisdn),
                //                    Text = text_msg,
                //                    TokenID = token_id,
                //                    TransactionID = "12345"
                //                };
                //                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                //                if (response_sendsms != null)
                //                {
                //                    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                //                    }
                //                    else
                //                    {
                //                        lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                    }
                //                }
                //                else
                //                {
                //                    lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                //                }

                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //        else
                //        {
                //            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                //        }
                //    }
                //    break;
                case 1016:
                case 889:
                case 742:
                case 743:
                case 744:
                    if (updateType == "1" && platform_id == "1")
                    {
                        //send welcome SMS
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
                                string benin_price = "";
                                string text_msg = "";
                                string benin_validdate = "";
                                switch (service.service_id)
                                {
                                    case 1016:
                                    case 889:
                                        benin_price = "100F/07J";
                                        benin_validdate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                                        text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games. Cliquez et jouez maintenant: <SHORTURL>";
                                        break;
                                    case 742:
                                        string price_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT b.price_id FROM billing b WHERE b.subscriber_id = " + sub_id, ref lines);
                                        benin_price = "100F/J";
                                        switch (price_id)
                                        {
                                            case "1890":
                                                benin_price = "100F/J";
                                                break;
                                            case "1894":
                                                benin_price = "50F/20H";
                                                break;
                                            case "1895":
                                                benin_price = "75F/22H";
                                                break;
                                            case "1896":
                                                benin_price = "25F/18H";
                                                break;
                                        }
                                        
                                        benin_validdate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
                                        text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games Jour. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>";
                                        break;
                                    case 743:
                                        benin_price = "500F/07J";
                                        benin_validdate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
                                        text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games pour 7 jours. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>.";
                                        break;
                                    case 744:
                                        benin_price = "1500F/30J";
                                        benin_validdate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
                                        text_msg = "Cher client, vous avez active avec succes votre service YellowDot Games pour 30 jours. Valide jusqu'au <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>.";
                                        break;
                                }
                                string long_url = "http://cam.ydgames.co/camydgames/index?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                                string new_id = Base64.Reverse(Base64.EncodeDecodeBase64(sub_id.ToString(), 1).Replace("=", ""));
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',"+sub_id+",'" + long_url + "',now(), " + msisdn + ")", "DBConnectionString_104", ref lines);

                                string short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + new_id;// CreateShortURL32(sub_id, long_url, ref lines);
                                text_msg = text_msg.Replace("<BENINDATE>", benin_validdate).Replace("<SHORTURL>", short_url).Replace("<BENINPRICE>", benin_price);
                                lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    Text = text_msg,
                                    TokenID = token_id,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
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
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                        }
                    }
                    break;
                case 669:
                case 697:
                case 698:
                case 699:
                case 700:
                case 701:
                case 79:
                    if (updateType == "1" && platform_id == "1")
                    {
                        //send welcome SMS
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
                                string benin_price = "";
                                string text_msg = "";
                                string benin_validdate = "";
                                switch (service.service_id)
                                {
                                    case 669:
                                    case 699:
                                        benin_price = "50F/J";
                                        benin_validdate = DateTime.Now.AddDays(1).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                        text_msg = "Cher client, vous avez active avec succes votre service MTN Yellow Games Jour. Validite le <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                        break;
                                    case 697:
                                    case 700:
                                        benin_price = "100F/03J";
                                        benin_validdate = DateTime.Now.AddDays(3).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                        text_msg = "Cher client, vous avez active avec succes votre service MTN Yellow Games pour 3 jours. Validite le <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                        break;
                                    case 698:
                                    case 701:
                                        benin_price = "150F/07J";
                                        benin_validdate = DateTime.Now.AddDays(7).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                        text_msg = "Cher client, vous avez active avec succes votre service MTN Yellow Games pour 7 jours. Validite le <BENINDATE>. Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                        break;
                                    case 79:
                                        text_msg = "Welcome to YDGames Portal! Click to play : http://www.ydgames.co/nigeria/index/" + Base64.EncodeDecodeBase64(msisdn, 1);
                                        break;
                                }
                                string long_url = "http://track32.ydot.co/Track/B32/YDGB?cli=" + Base64.EncodeDecodeBase64(msisdn, 1) + "&sid=" + service.service_id;
                                string short_url = CreateShortURL32(sub_id, long_url, ref lines);
                                text_msg = text_msg.Replace("<BENINDATE>", benin_validdate).Replace("<SHORTURL>", short_url).Replace("<BENINPRICE>", benin_price);
                                lines = Add2Log(lines, " Sending SMS to " + msisdn + " " + text_msg, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    Text = text_msg,
                                    TokenID = token_id,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1010 || response_sendsms.ResultCode == 1000)
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
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        else
                        {
                            lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                        }
                    }
                    break;
                //Zone Alarm
                case 6:
                case 7:
                case 8:
                case 360:
                    if (platform_id == "1")
                    {
                        switch (updateType)
                        {
                            case "1":
                                //get token 
                                //generate key
                                string url = CommonFuncations.ZoneAlarm.CallZAGenerateKey(sub_id, msisdn, ref lines);
                                if (!String.IsNullOrEmpty(url))
                                {
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
                                            string token_id = response.TokenID;
                                            lines = Add2Log(lines, " token_id = " + token_id, 100, lines[0].ControlerName);
                                            string text = "Thank you for subscribing to Zone Alarm. Click to download the application " + url;
                                            lines = Add2Log(lines, "Going to send SMS to " + msisdn + " : " + text, 100, lines[0].ControlerName);
                                            Api.HttpItems.SendSMSRequest RequestSendSMSBody = new Api.HttpItems.SendSMSRequest()
                                            {
                                                ServiceID = service.service_id,
                                                MSISDN = Convert.ToInt64(msisdn),
                                                Text = text,
                                                TokenID = token_id,
                                                TransactionID = "12345"
                                            };
                                            Api.HttpItems.SendSMSResponse response_sendsms = Api.CommonFuncations.SendSMS.DoSMS(RequestSendSMSBody);
                                            if (response_sendsms != null)
                                            {
                                                lines = Add2Log(lines, response_sendsms.ResultCode + " - " + response_sendsms.Description, 100, lines[0].ControlerName);
                                            }
                                        }
                                    }
                                }
                                //send sms
                                break;
                            case "2":
                                string za_res = CommonFuncations.ZoneAlarm.CallZADeleteUser(msisdn, ref lines);
                                break;
                        }
                    }
                    break;
                //case 81:
                //case 82:
                //case 854:
                //case 855:
                //case 83:
                //case 84:
                //case 85:
                //case 86:
                //case 87:
                //case 88:
                //case 89:
                //case 353:
                //case 358:
                //case 359:
                //case 629:
                //case 630:
                //case 628:
                //case 665:
                //case 666:
                //case 667:
                //case 668:
                //case 190:
                //case 886:
                //case 887:
                //    if (updateType == "1" && platform_id == "1")
                //    {
                //        ServiceConfiguration service_conf = GetVideoServiceConf(service.service_id, ref lines);
                //        if (service_conf != null)
                //        {
                //            string long_url = service_conf.lp_url + "?sub_id=" + sub_id+"&cli="+ Base64.EncodeDecodeBase64(msisdn, 1)+"&sid="+service.service_id;
                //            string short_url = CreateShortURL(sub_id, long_url, ref lines);
                //            string text = "Bravo! Welcome to "+ service.service_name + " service on MTN. Visit url ";
                //            if (!String.IsNullOrEmpty(short_url))
                //            {
                //                text = text + short_url;
                //                lines = Add2Log(lines, "Going to send SMS to " + msisdn + " : " + text, 100, lines[0].ControlerName);
                //                Api.HttpItems.SendSMSRequest RequestSendSMSBody = new Api.HttpItems.SendSMSRequest()
                //                {
                //                    ServiceID = service.service_id,
                //                    MSISDN = Convert.ToInt64(msisdn),
                //                    Text = text,
                //                    TokenID = service_conf.token,
                //                    TransactionID = "12345"
                //                };
                //                Api.HttpItems.SendSMSResponse response_sendsms = Api.CommonFuncations.SendSMS.DoSMS(RequestSendSMSBody);
                //                if (response_sendsms != null)
                //                {
                //                    lines = Add2Log(lines, response_sendsms.ResultCode + " - " + response_sendsms.Description, 100, lines[0].ControlerName);
                //                }
                //            }
                //            else
                //            {
                //                lines = Add2Log(lines, "Short URL creation has failed", 100, lines[0].ControlerName);
                //            }
                //        }
                //    }
                //    break;
                case 443:
                case 444:
                case 445:
                case 446:
                case 447:
                case 448:
                case 449:
                case 441:
                    if (updateType == "1" && platform_id == "1")
                    {
                        ServiceConfiguration service_conf = GetDevotionalServiceConf(service.service_id, ref lines);
                        if (service_conf != null)
                        {
                            string long_url = service_conf.xml_url + "?sub_id=" + sub_id;
                            string short_url = CreateShortURL(sub_id, long_url, ref lines);
                            string text = "Thank you for subscribing to " + service.service_name + ". Click this Link to view your Video Content ";
                            if (!String.IsNullOrEmpty(short_url))
                            {
                                text = text + short_url;
                                lines = Add2Log(lines, "Going to send SMS to " + msisdn + " : " + text, 100, lines[0].ControlerName);
                                Api.HttpItems.SendSMSRequest RequestSendSMSBody = new Api.HttpItems.SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = Convert.ToInt64(msisdn),
                                    Text = text,
                                    TokenID = service_conf.token,
                                    TransactionID = "Devotional"+sub_id
                                };
                                Api.HttpItems.SendSMSResponse response_sendsms = Api.CommonFuncations.SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    lines = Add2Log(lines, response_sendsms.ResultCode + " - " + response_sendsms.Description, 100, lines[0].ControlerName);
                                }
                            }
                            else
                            {
                                lines = Add2Log(lines, "Short URL creation has failed", 100, lines[0].ControlerName);
                            }
                        }
                    }
                    break;
                case 91:
                    if (platform_id == "1")
                    {
                        string MySqlQ = "select m.mo_id from mo_requests m where m.msisdn = " + msisdn + " and m.service_id = 91 and m.subscription_result = 1010 and m.subscription_id = 0 and datediff(m.date_time, now()) <=1  order by m.mo_id desc limit 1";
                        Int64 mo_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64(MySqlQ, ref lines);
                        if (mo_id > 0)
                        {
                            MySqlQ = "update mo_requests set subscription_id = " + sub_id + " where mo_id = " + mo_id;
                            Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                        }
                    }
                    break;
            }
        }

        public static void PBStar(string msisdn, ServiceClass service, string text, ref List<LogLines> lines)
        {
            List<iDoBet.SportEvents> football_sports_events = Cache.iDoBet.GetMTNCongoEventsFromCache(31, ref lines);
            List<iDoBet.SportEvents> basketball_sports_events = Cache.iDoBet.GetMTNCongoEventsFromCache(32, ref lines);
            List<iDoBet.SportEvents> tennis_sports_events = Cache.iDoBet.GetMTNCongoEventsFromCache(35, ref lines);

            string prefix = "";// service.service_id.ToString();// (service.service_id == 716 ? "_mgn" : "_ogn");


            string[] words2 = text.Split('*');
            string type = words2[0];
            string amount = words2[1];
            string fgame = "";
            string fbet_type = "";
            iDoBet.EventOdds my_odd = null;
            string user_session_id = Guid.NewGuid().ToString();
            int odd_page = 0;
            string ussdString = "";
            DYAReceiveMoneyRequest momo_request = null;
            switch (type)
            {
                case "s1":
                    for (int i = 2; i <= words2.Length - 1; i++)
                    {
                        if (i % 2 == 0)
                        {
                            fgame = words2[i];
                        }
                        else
                        {
                            fbet_type = words2[i];
                        }
                        if (!String.IsNullOrEmpty(fgame) && !String.IsNullOrEmpty(fbet_type))
                        {
                            iDoBet.SportEvents selected_event = null;
                            if (fbet_type.StartsWith("ft"))
                            {
                                selected_event = football_sports_events.Find(x => x.game_id == Convert.ToInt64(fgame));
                                if (selected_event != null)
                                {
                                    string odd_name = "";
                                    int bts_id = iDoBet.GetBTSID(odd_page, 31, out odd_name);
                                    List<iDoBet.EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
                                    if (event_odds != null)
                                    {
                                        switch (fbet_type)
                                        {
                                            case "ft1":
                                                my_odd = event_odds[0];
                                                ussdString = "1";
                                                break;
                                            case "ftx":
                                                my_odd = event_odds[1];
                                                ussdString = "2";
                                                break;
                                            case "ft2":
                                                my_odd = event_odds[2];
                                                ussdString = "3";
                                                break;
                                        }
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", ref lines);
                                    }

                                }
                            }
                            if (fbet_type.Contains("2.5"))
                            {
                                odd_page = 1;
                                string odd_name = "";
                                int bts_id = iDoBet.GetBTSID(odd_page, 31, out odd_name);
                                List<iDoBet.EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
                                if (event_odds != null)
                                {
                                    if (fbet_type.StartsWith("u"))
                                    {
                                        my_odd = event_odds[0];
                                        ussdString = "1";
                                    }
                                    if (fbet_type.StartsWith("o"))
                                    {
                                        my_odd = event_odds[1];
                                        ussdString = "2";
                                    }
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", ref lines);
                                }
                            }
                            if (fbet_type.StartsWith("dc"))
                            {
                                odd_page = 2;
                                string odd_name = "";
                                int bts_id = iDoBet.GetBTSID(odd_page, 31, out odd_name);
                                List<iDoBet.EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
                                if (event_odds != null)
                                {
                                    if (fbet_type.Contains("1x"))
                                    {
                                        my_odd = event_odds[0];
                                        ussdString = "1";
                                    }
                                    if (fbet_type.Contains("x2"))
                                    {
                                        my_odd = event_odds[1];
                                        ussdString = "2";
                                    }
                                    if (fbet_type.Contains("12"))
                                    {
                                        my_odd = event_odds[2];
                                        ussdString = "3";
                                    }
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", ref lines);
                                }
                            }
                            if (fbet_type.StartsWith("w"))
                            {
                                int sport_type = 32;
                                selected_event = basketball_sports_events.Find(x => x.game_id == Convert.ToInt64(fgame));
                                if (selected_event == null)
                                {
                                    sport_type = 35;
                                    selected_event = tennis_sports_events.Find(x => x.game_id == Convert.ToInt64(fgame));
                                }
                                if (selected_event != null)
                                {
                                    string odd_name = "";
                                    int bts_id = iDoBet.GetBTSID(odd_page, sport_type, out odd_name);
                                    List<iDoBet.EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
                                    if (event_odds != null)
                                    {
                                        switch (fbet_type)
                                        {
                                            case "w1":
                                                my_odd = event_odds[0];
                                                ussdString = "1";
                                                break;
                                            case "w2":
                                                my_odd = event_odds[1];
                                                ussdString = "2";
                                                break;
                                        }
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", ref lines);
                                    }

                                }
                            }
                            fgame = "";
                            fbet_type = "";
                        }
                    }

                    USSDSession ussd_session = new USSDSession();
                    ussd_session.user_seesion_id = user_session_id;
                    bool req_for_order = false;
                    req_for_order = Api.CommonFuncations.iDoBetMTNNG.PlaceBet(ussd_session, ref lines);
                    if (req_for_order == true)
                    {
                        LoginRequest LoginRequestBody = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                        if (res != null)
                        {
                            if (res.ResultCode == 1000)
                            {
                                string token_id = res.TokenID;
                                momo_request = new DYAReceiveMoneyRequest()
                                {
                                    MSISDN = Convert.ToInt64(msisdn),
                                    Amount = Convert.ToInt32(amount),
                                    ServiceID = service.service_id,
                                    TokenID = token_id,
                                    TransactionID = user_session_id
                                };
                                string postBody = JsonConvert.SerializeObject(momo_request);
                                string url = "https://api239.ydplatform.com/api/DYAReceiveMoney";
                                List<Headers> headers = new List<Headers>();
                                lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                                CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                                lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                            }
                        }
                    }
                    break;
                case "r2":
                case "r3":
                case "r5":
                    fgame = (type == "r2" ? "35" : (type == "r3" ? "36" : "37"));
                    ussdString = words2[2].Replace(",", " ");
                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", 0, '" + ussdString + "', now(), 0,'" + user_session_id + "',0, 0, '0', '0'," + amount + ", " + service.service_id + ")", ref lines);
                    LoginRequest LoginRequestBody1 = new LoginRequest()
                    {
                        ServiceID = service.service_id,
                        Password = service.service_password
                    };
                    LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
                    if (res1 != null)
                    {
                        if (res1.ResultCode == 1000)
                        {
                            string token_id = res1.TokenID;
                            momo_request = new DYAReceiveMoneyRequest()
                            {
                                MSISDN = Convert.ToInt64(msisdn),
                                Amount = Convert.ToInt32(amount),
                                ServiceID = service.service_id,
                                TokenID = token_id,
                                TransactionID = "Rapidos_" + user_session_id

                            };
                            string postBody = JsonConvert.SerializeObject(momo_request);
                            string url = "https://api239.ydplatform.com/api/DYAReceiveMoney";
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                            CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                            lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                        }
                    }
                    break;
                case "l1":
                    string game_id = words2[2];
                    ussdString = words2[3].Replace(",", " ");
                    switch (game_id)
                    {
                        case "n2":
                            fgame = "4";
                            break;
                        case "n3":
                            fgame = "3";
                            break;
                        case "n4":
                            fgame = "2";
                            break;
                        case "n5":
                            fgame = "1";
                            break;
                        case "c3":
                            fgame = "11";
                            break;
                        case "c4":
                            fgame = "10";
                            break;
                        case "c5":
                            fgame = "9";
                            break;
                        case "t2":
                            fgame = "5";
                            break;
                        case "t3":
                            fgame = "6";
                            break;
                        case "t4":
                            fgame = "7";
                            break;
                        case "t5":
                            fgame = "8";
                            break;
                    }
                    int lotto_service = (msisdn.StartsWith("24266") ? 777 : 888);
                    ServiceClass l_service = GetServiceByServiceID(lotto_service, ref lines);
                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", 0, '" + ussdString + "', now(), 0,'" + user_session_id + "',0, 0, '0', '0'," + amount + ", " + l_service.service_id + ")", ref lines);
                    LoginRequest LoginRequestBody_lotto = new LoginRequest()
                    {
                        ServiceID = l_service.service_id,
                        Password = l_service.service_password
                    };
                    LoginResponse res_lotto = Login.DoLogin(LoginRequestBody_lotto);
                    if (res_lotto != null)
                    {
                        if (res_lotto.ResultCode == 1000)
                        {
                            string token_id = res_lotto.TokenID;
                            momo_request = new DYAReceiveMoneyRequest()
                            {
                                MSISDN = Convert.ToInt64(msisdn),
                                Amount = Convert.ToInt32(amount),
                                ServiceID = l_service.service_id,
                                TokenID = token_id,
                                TransactionID = "DusanLotto_" + user_session_id
                            };
                            string postBody = JsonConvert.SerializeObject(momo_request);
                            string url = "https://api239.ydplatform.com/api/DYAReceiveMoney";
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                            CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                            lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                        }
                    }
                    break;
            }
        }

        public static void PlaceBet(string msisdn, ServiceClass service, string text, ref List<LogLines> lines)
        {


            if (text.Contains("|"))
            {
                string[] words1 = text.Split('|');
                foreach (string s1 in words1)
                {
                    PBStar(msisdn, service, s1, ref lines);
                }
            }
            else
            {
                PBStar(msisdn, service, text, ref lines);
            }


        }

        public static void DecideBehaviorMO(ServiceClass service, string msisdn, string keyword, string time_stamp, string short_code, string linkID, ref List<LogLines> lines)
        {
            string platform_id = Cache.ServerSettings.GetServerSettings("PlatformID", ref lines);
            string MySqlQ = "";
            switch (service.service_id)
            {
                case 956:
                    if (keyword.ToLower().StartsWith("winsapa|"))
                    {
                        string my_text = keyword.Substring(8).ToLower();
                        PlaceBet(msisdn, service, my_text, ref lines);
                    }
                    break;
                case 993:
                case 994:
                case 995:
                case 996:
                case 997:
                case 998:
                    if (keyword == "LNR")
                    {
                        LoginRequest LoginRequestBodyLNG = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse res1 = Login.DoLogin(LoginRequestBodyLNG);
                        if (res1 != null)
                        {
                            if (res1.ResultCode == 1000)
                            {
                                WinningUsersFromDB winners = GetWinnersFromDB("993,994,995,996,997,998", "993", ref lines);
                                if (winners != null)
                                {
                                    string general_msg = winners.general_msg.Replace("<WINCOUNT>", winners.number_of_winners).Replace("<DIGITS>", winners.last_5_digit).Replace("Yesterday's ", "");
                                    SendSMSRequest SendSMSRequestBody = new SendSMSRequest()
                                    {
                                        MSISDN = Convert.ToInt64(msisdn),
                                        TokenID = res1.TokenID,
                                        ServiceID = service.service_id,
                                        Text = general_msg,
                                        TransactionID = "sms_123456",
                                        LinkID = linkID

                                    };
                                    lines = Add2Log(lines, "Sending general_msg SMS to " + msisdn + ", " + general_msg, 100, "MO");
                                    SendSMSResponse response_sendsms1 = SendSMS.DoSMS(SendSMSRequestBody);
                                    lines = Add2Log(lines, " ResultCode = " + response_sendsms1.ResultCode + ", Description = " + response_sendsms1.Description, 100, "wapnotification");
                                    if (response_sendsms1 != null)
                                    {
                                        if (response_sendsms1.ResultCode == 1010)
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
                        }
                    }
                    break;

                case 702:
                case 685:
                    if (platform_id == "1")
                    {
                        LoginRequest LoginRequestBody1 = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
                        if (res1 != null)
                        {
                            if (res1.ResultCode == 1000)
                            {
                                SMSTexts sms_texts = GetSMSText(service, ref lines);
                                SendSMSRequest SendSMSRequestBody = new SendSMSRequest()
                                {
                                    MSISDN = Convert.ToInt64(msisdn),
                                    TokenID = res1.TokenID,
                                    ServiceID = service.service_id,
                                    Text = sms_texts.welcome_sms,
                                    TransactionID = "sms_123456",
                                    LinkID = linkID

                                };
                                lines = Add2Log(lines, "Sending welcome SMS to " + msisdn + ", " + sms_texts.welcome_sms, 100, "MO");
                                SendSMSResponse response_sendsms1 = SendSMS.DoSMS(SendSMSRequestBody, 1);
                                lines = Add2Log(lines, " ResultCode = " + response_sendsms1.ResultCode + ", Description = " + response_sendsms1.Description, 100, "wapnotification");
                                if (response_sendsms1 != null)
                                {
                                    if (response_sendsms1.ResultCode == 1010)
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
                                //add user to subscriber db
                                //add user to billing db
                                Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values("+msisdn+", "+ service.service_id + ", now(), 3, 2, '"+keyword+"' )", ref lines);
                                if (sub_id > 0)
                                {
                                    PriceClass price_info = GetPricesInfo(service.service_id, 0, ref lines);
                                    if (price_info != null)
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values("+sub_id+", now(), "+price_info.price_id+");", ref lines);
                                    }
                                    //msisdn=2349038125089&amp;dateTime=2019-03-20 13:13:00&amp;amount=20&amp;serviceId=685
                                    string add_draw_url = Cache.ServerSettings.GetServerSettings("LuckyNumberODAdd2DrawURL", ref lines) + "msisdn=" + msisdn + "&dateTime="+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "&amount=20&servicId="+service.service_id;
                                    lines = Add2Log(lines, " Calling " + add_draw_url, 100, lines[0].ControlerName);
                                    string response = CallSoap.GetURL(add_draw_url, ref lines);
                                    lines = Add2Log(lines, " response = " + response, 100, lines[0].ControlerName);

                                }

                                
                            }
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "something is wrong", 100, "MO");
                    }
                    
                    MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text) values (" + msisdn + ",'" + short_code + "'," + service.service_id + ",'" + time_stamp + "','" + keyword + "')";
                    Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);

                    break;
                case 3:
                    if (keyword == "1" || keyword == "2" || keyword == "3")
                    {
                        UserQuestion user_q = CheckUpdateUserPTAnswer(msisdn, service.spid, short_code, Convert.ToInt32(keyword), ref lines);
                        if (user_q != null)
                        {
                            //login
                            LoginRequest LoginRequestBody = new LoginRequest()
                            {
                                ServiceID = 3,
                                Password = service.service_password
                            };
                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                            if (res != null)
                            {
                                if (res.ResultCode == 1000)
                                {
                                    //sendsms
                                    lines = Add2Log(lines, " Sending SMS to " + user_q.MSISDN + " - " + user_q.question, 100, "MO");
                                    lines = Add2Log(lines, " Test ID " + user_q.test_id, 100, "MO");
                                    SendSMSRequest SendSMSRequestBody = new SendSMSRequest()
                                    {
                                        MSISDN = user_q.MSISDN,
                                        TokenID = res.TokenID,
                                        ServiceID = 3,
                                        Text = user_q.question,
                                        TransactionID = Guid.NewGuid().ToString()
                                    };
                                    SendSMSResponse sms_response = SendSMS.DoSMS(SendSMSRequestBody);
                                    if (sms_response != null)
                                    {
                                        if (sms_response.ResultCode == 1010)
                                        {
                                            lines = Add2Log(lines, " SMS was sent", 100, "MO");
                                            if (user_q.is_complete == false)
                                            {
                                                //insert p_user_answeres in case of is_complete false
                                                InsertUserPT(user_q.subscriber_id, user_q.test_id, user_q.question_id, ref lines);
                                            }
                                        }
                                        else
                                        {
                                            lines = Add2Log(lines, " SMS was not sent", 100, "MO");
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        //send error message
                        //login
                        LoginRequest LoginRequestBody = new LoginRequest()
                        {
                            ServiceID = 3,
                            Password = service.service_password
                        };
                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                        if (res != null)
                        {
                            if (res.ResultCode == 1000)
                            {
                                //sendsms
                                lines = Add2Log(lines, " Sending SMS to " + msisdn + " - Choose 1,2 or 3", 100, "MO");
                                SendSMSRequest SendSMSRequestBody = new SendSMSRequest()
                                {
                                    MSISDN = Convert.ToInt64(msisdn),
                                    TokenID = res.TokenID,
                                    ServiceID = 3,
                                    Text = "Choose 1,2 or 3!",
                                    TransactionID = Guid.NewGuid().ToString()
                                };
                                SendSMSResponse sms_response = SendSMS.DoSMS(SendSMSRequestBody);
                            }
                        }
                    }
                    MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text) values (" + msisdn + ",'" + short_code + "'," + service.service_id + ",'" + time_stamp + "','" + keyword + "')";
                    Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                    break;
                case 91:
                    if (platform_id == "1")
                    {
                        string token_id = "";
                        LoginRequest RequestBody = new LoginRequest()
                        {
                            ServiceID = 91,
                            Password = service.service_password
                        };
                        LoginResponse response = Login.DoLogin(RequestBody);
                        if (response != null)
                        {
                            if (response.ResultCode == 1000)
                            {
                                token_id = response.TokenID;
                                lines = Add2Log(lines, " token_id = " + token_id, 100, "MO");
                                if (keyword == "win")
                                {
                                    lines = Add2Log(lines, " SubscribingUser" , 100, "MO");
                                    SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                    {
                                        ServiceID = 91,
                                        TokenID = token_id,
                                        MSISDN = Convert.ToInt64(msisdn),
                                        TransactionID = "123456"
                                    };
                                    SubscribeResponse subscribe_response = CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
                                    lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "MO");
                                    if (subscribe_response.ResultCode == 3010)
                                    {
                                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), time_stamp, "ok", ref lines);
                                    }
                                    MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text, subscription_result) values ("+msisdn+",'6060',91,'"+time_stamp+"','"+keyword+"',"+ subscribe_response.ResultCode + ")";
                                    Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                                }
                                if (keyword.Contains("stop"))
                                {
                                    lines = Add2Log(lines, " UnSubscribing User", 100, "MO");
                                    SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                    {
                                        ServiceID = 91,
                                        TokenID = token_id,
                                        MSISDN = Convert.ToInt64(msisdn),
                                        TransactionID = "123456"
                                    };
                                    SubscribeResponse unsubscribe_response = CommonFuncations.UnSubscribe.DoUnSubscribe(subscribe_RequestBody);
                                    lines = Add2Log(lines, " ResultCode = " + unsubscribe_response.ResultCode + ", Description = " + unsubscribe_response.Description, 100, "MO");
                                    MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text) values (" + msisdn + ",'6060',91,'" + time_stamp + "','" + keyword + "')";
                                    Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                                }
                                if (keyword != "win" && !keyword.Contains("stop"))
                                {
                                    if (keyword == "1" || keyword == "2")
                                    {
                                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), time_stamp, keyword, ref lines);
                                        MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text) values (" + msisdn + ",'6060',91,'" + time_stamp + "','" + keyword + "')";
                                        Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, " SubscribingUser", 100, "MO");
                                        SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                        {
                                            ServiceID = 91,
                                            TokenID = token_id,
                                            MSISDN = Convert.ToInt64(msisdn),
                                            TransactionID = "123456"
                                        };
                                        SubscribeResponse subscribe_response = CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
                                        lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "MO");
                                        if (subscribe_response.ResultCode == 3010)
                                        {
                                            CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), time_stamp, "ok", ref lines);
                                        }
                                        MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text, subscription_result) values (" + msisdn + ",'6060',91,'" + time_stamp + "','" + keyword + "'," + subscribe_response.ResultCode + ")";
                                        Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                                        //CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), time_stamp, "ok", ref lines);
                                    }
                                }
                                
                                
                            }
                        }

                    }
                    break;
                default:
                    MySqlQ = "insert into mo_requests (msisdn, short_code, service_id, date_time, sms_text) values (" + msisdn + ",'"+ short_code + "',"+service.service_id+",'" + time_stamp + "','" + keyword + "')";
                    Api.DataLayer.DBQueries.ExecuteQuery(MySqlQ, ref lines);
                    break;
            }
        }


        public static void DecideBehaviorMOMO(ServiceClass service, DYATransactions dya_trans, string StatusCode, ref List<LogLines> lines)
        {
            List<Int64> ussd_bl = Cache.USSD.GetUSSDBlackListUsersNoRefund(ref lines);
            bool refund_allowed = true;
            if (ussd_bl != null)
            {
                if (ussd_bl.Contains(dya_trans.msisdn))
                {
                    refund_allowed = false;
                }
            }
            if (refund_allowed)
            {
                string platform_id = Cache.ServerSettings.GetServerSettings("PlatformID", ref lines);
                switch (service.service_id)
                {
                    case 669:
                        string token_id_bjydg = "", text_msg_bjydg = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_bjydg = false;
                        LoginRequest RequestBody_bjydg = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_bjydg = Login.DoLogin(RequestBody_bjydg);
                        if (response_bjydg != null)
                        {
                            if (response_bjydg.ResultCode == 1000)
                            {
                                token_id_bjydg = response_bjydg.TokenID;
                                login_continue_bjydg = true;
                            }
                        }
                        int real_service_id = 669;
                        int price_id = 0;
                        if (StatusCode == "01")
                        {
                            string errmsg = "";
                            string benin_price = "";
                            string benin_validdate = "";
                            switch (dya_trans.amount)
                            {
                                case 50:
                                    real_service_id = 669;
                                    price_id = 1853;
                                    benin_price = "50F/J";
                                    benin_validdate = DateTime.Now.AddDays(1).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                    text_msg_bjydg = "Cher client, vous avez active avec succes votre service MTN Yellow Games Jour. Validite le <BENINDATE> . Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                    break;
                                case 100:
                                    real_service_id = 697;
                                    price_id = 1852;
                                    benin_price = "100F/03J";
                                    benin_validdate = DateTime.Now.AddDays(3).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                    text_msg_bjydg = "Cher client, vous avez active avec succes votre service MTN Yellow Games pour 3 jours. Validite le <BENINDATE> . Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                    break;
                                case 150:
                                    real_service_id = 698;
                                    price_id = 1855;
                                    benin_price = "150F/07J";
                                    benin_validdate = DateTime.Now.AddDays(7).AddHours(-1).ToString("dd/MM/yyyy HH:00");
                                    text_msg_bjydg = "Cher client, vous avez active avec succes votre service MTN Yellow Games pour 7 jours. Validite le <BENINDATE> . Cliquez et jouez maintenant: <SHORTURL> Cout <BENINPRICE>. Desactiver: *709*4#";
                                    break;
                            }
                            ServiceClass subscriber_service = GetServiceByServiceID(real_service_id, ref lines);
                            bool full_res = Fulfillment.CallFulfillmentNoCharhing(subscriber_service, dya_trans.msisdn.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref lines, out errmsg);

                            Int64 sub_id = Api.DataLayer.DBQueries.InsertSub(dya_trans.msisdn.ToString(), real_service_id.ToString(), price_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "3", "", ref lines);
                            string long_url = "http://track32.ydot.co/Track/B32/YDGB?cli=" + Base64.EncodeDecodeBase64(dya_trans.msisdn.ToString(), 1) + "&sid=" + real_service_id;
                            string short_url = CreateShortURL32(sub_id, long_url, ref lines);
                            text_msg_bjydg = text_msg_bjydg.Replace("<BENINDATE>", benin_validdate).Replace("<SHORTURL>", short_url).Replace("<BENINPRICE>", benin_price);
                        }
                        if (login_continue_bjydg)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_bjydg, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody_bjydg = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_bjydg,
                                TokenID = token_id_bjydg,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms_bjydg = SendSMS.DoSMS(RequestSendSMSBody_bjydg);
                            if (response_sendsms_bjydg != null)
                            {
                                if (response_sendsms_bjydg.ResultCode == 1000 || response_sendsms_bjydg.ResultCode == 1010)
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
                        break;
                    //MTN Millionaire
                    case 709:
                        string token1 = "", text_msg1 = "";
                        ServiceClass service1 = GetServiceByServiceID(708, ref lines);
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

                        if (StatusCode == "01")
                        {
                            text_msg1 = Benin.DecideWelcomeMSG();
                            DataLayer.DBQueries.ExecuteQuery("insert into promo.joined_users (msisdn, service_id, date_time, keyword, dya_id) values(" + dya_trans.msisdn + "," + service.service_id + ",now(), 'momo'," + dya_trans.dya_trans + " );", ref lines);
                            DataLayer.DBQueries.ExecuteQuery("delete from promo.dnd where msisdn = " + dya_trans.msisdn, ref lines);
                        }
                        else
                        {
                            if (StatusCode == "529")
                            {
                                text_msg1 = "Yello! Vous n avez pas suffisamment de credit pour participer a la promo MTN Millionnaire. Veuillez recharger votre compte et reessayer ou composer *185# pour emprunter du credit et reesayer. Merci";
                            }
                            else
                            {
                                text_msg1 = "Cher abonne, votre inscription à la promo MTN Millionnaire a echoue. Veuillez reessayer plus tard.";
                            }
                        }

                        lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg1, 100, lines[0].ControlerName);
                        SendSMSRequest RequestSendSMSBody1 = new SendSMSRequest()
                        {
                            ServiceID = service.service_id,
                            MSISDN = dya_trans.msisdn,
                            Text = text_msg1,
                            TokenID = token1,
                            TransactionID = "12345"
                        };
                        SendSMSResponse response_sendsms1 = SendSMS.DoSMS(RequestSendSMSBody1);
                        if (response_sendsms1 != null)
                        {
                            if (response_sendsms1.ResultCode == 1000 || response_sendsms1.ResultCode == 1010)
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

                        break;
                    //BtoBet YellowBet
                    case 726:
                        string token_id_lnb = "", text_msg_lnb = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_lnb = false;
                        LoginRequest RequestBody_lnb = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_lnb = Login.DoLogin(RequestBody_lnb);
                        if (response_lnb != null)
                        {
                            if (response_lnb.ResultCode == 1000)
                            {
                                token_id_lnb = response_lnb.TokenID;
                                login_continue_lnb = true;
                            }
                        }

                        if (StatusCode == "01" || StatusCode == "1000")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_lnb = false;
                            }

                            if (dya_trans.partner_transid.Length >= 44)
                            {
                                login_continue_lnb = false;
                            }

                            if (login_continue_lnb)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {

                                    //deposit idobet
                                    IdoBetUser user_ms = null;
                                    bool result_ms = false;
                                    string postBody = "", response_body = "";


                                    user_ms = B2TechLNBMTN.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    result_ms = (user_ms.isValid == true ? B2TechLNBMTN.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);




                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        text_msg_lnb = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_lnb = text_msg_lnb + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_ms = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_lnb = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms


                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("KironTrans_"))
                                    {
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("KironTrans_", ""), out number);
                                        Api.DataLayer.DBQueries.ExecuteQuery("update pos_requests set dya_id = " + dya_trans.dya_trans + " where id = " + number, ref lines);
                                        text_msg_lnb = "L'argent a ete collecte avec succes." + Environment.NewLine + "Veuillez contacter le support LNBPari";
                                    }
                                    else
                                    {
                                        if (dya_trans.partner_transid.Contains("POSTrans_"))
                                        {
                                            //take money from subagent and deposit to momo
                                            Int64 number;
                                            bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                            bool collect_send_money = false;
                                            if (success)
                                            {
                                                collect_send_money = iDoBetLNBPariMTNBenin.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                                if (collect_send_money)
                                                {
                                                    text_msg_lnb = "L'argent a ete collecte avec succes";
                                                }
                                                else
                                                {
                                                    text_msg_lnb = "L'argent a ete collecte a echoue";
                                                    string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                    string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                    string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                    string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                    string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                    string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                    int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                    string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                    CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                                }
                                            }



                                        }
                                        else
                                        {

                                            string postBody = "", response_body = "";

                                            CommonFuncations.iDoBet.ExecuteOrderDetails barcode = null;

                                            if (dya_trans.partner_transid.Length == 36)
                                            {
                                                barcode = CommonFuncations.B2TechLNBMTN.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                                if (barcode != null)
                                                {
                                                    text_msg_lnb = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                                    text_msg_lnb = text_msg_lnb + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                                    text_msg_lnb = text_msg_lnb + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                                    text_msg_lnb = text_msg_lnb + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                                    text_msg_lnb = text_msg_lnb + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                                    lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                                    if (!String.IsNullOrEmpty(barcode.total_bonus))
                                                    {
                                                        text_msg_lnb = text_msg_lnb + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                                    }
                                                    //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                                }
                                                else
                                                {
                                                    bool user_was_refund = false;
                                                    LoginRequest LoginRequestBody = new LoginRequest()
                                                    {
                                                        ServiceID = service.service_id,
                                                        Password = service.service_password
                                                    };
                                                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                    if (res != null)
                                                    {
                                                        if (res.ResultCode == 1000)
                                                        {
                                                            DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                            {
                                                                MSISDN = dya_trans.msisdn,
                                                                Amount = dya_trans.amount,
                                                                ServiceID = service.service_id,
                                                                TokenID = res.TokenID,
                                                                TransactionID = dya_trans.dya_trans.ToString()
                                                            };
                                                            DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                            if (momo_response != null)
                                                            {
                                                                if (momo_response.ResultCode == 1000)
                                                                {
                                                                    user_was_refund = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    text_msg_lnb = text_msg_lnb = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                    string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                    string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                    string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                    string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                    string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                    string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                    int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                    string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                    CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                                    //TODO start refund

                                                }
                                            }




                                            //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);

                                        }
                                    }


                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_lnb)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_lnb, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_lnb,
                                TokenID = token_id_lnb,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 746:
                    case 732:
                        string token_id_ms1 = "", text_msg_ms1 = "";
                        bool login_continue_ms1 = false;
                        LoginRequest RequestBody_ms1 = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_ms1 = Login.DoLogin(RequestBody_ms1);
                        if (response_ms1 != null)
                        {
                            if (response_ms1.ResultCode == 1000)
                            {
                                token_id_ms1 = response_ms1.TokenID;
                                login_continue_ms1 = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_ms1 = false;
                            }

                            if (login_continue_ms1)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_ms = null;
                                    string postBody = "", response_body = "";
                                    bool result_ms = false;

                                    user_ms = B2TechGNOrange.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    result_ms = (user_ms.isValid == true ? B2TechGNOrange.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);

                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);

                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        //text_msg_ms1 = "Bonjour, votre depot a ete place avec succes. ";
                                        //text_msg_ms1 = text_msg_ms1 + "Pariez en ligne pour gagner des bonus incroyables.";
                                        text_msg_ms1 = "";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {
                                        if (refund_allowed)
                                        {
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                    };
                                                    //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    DYATransferMoneyResponse momo_response = null;
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund_ms = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }



                                        text_msg_ms1 = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetOrangeGuinea.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_ms1 = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_ms1 = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        if (dya_trans.partner_transid.Contains("Rapidos_") || dya_trans.partner_transid.Length == 36)
                                        {
                                            string postBody = "", response_body = "";
                                            //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                            ExecuteOrderDetails barcode = null;
                                            if (dya_trans.partner_transid.Contains("Rapidos_"))
                                            {
                                                barcode = CommonFuncations.iDoBetOrangeGuinea.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                            }
                                            else
                                            {
                                                try
                                                {

                                                    barcode = CommonFuncations.B2TechGNOrange.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);


                                                }
                                                catch (Exception ex)
                                                {
                                                    lines = Add2Log(lines, " Exception on GetExecuteOrderNew " + ex.ToString(), 100, "ivr_subscribe");
                                                }

                                            }

                                            if (barcode != null)
                                            {
                                                text_msg_ms1 = "Bonjour, votre pari a ete place avec succes. ";// + Environment.NewLine;
                                                text_msg_ms1 = text_msg_ms1 + "Numero de commande: " + barcode.order_number + " ";// Environment.NewLine;
                                                text_msg_ms1 = text_msg_ms1 + "Code a barre: " + barcode.barcode + " ";//Environment.NewLine;
                                                text_msg_ms1 = text_msg_ms1 + "Montant: " + barcode.amount + " ";//Environment.NewLine;
                                                text_msg_ms1 = text_msg_ms1 + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                                lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                                if (!String.IsNullOrEmpty(barcode.total_bonus))
                                                {
                                                    text_msg_ms1 = text_msg_ms1 + " Bonus: " + barcode.total_bonus;//Environment.NewLine;
                                                }

                                                //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                            }
                                            else
                                            {

                                                bool user_was_refund = false;
                                                if (refund_allowed)
                                                {
                                                    LoginRequest LoginRequestBody = new LoginRequest()
                                                    {
                                                        ServiceID = service.service_id,
                                                        Password = service.service_password
                                                    };
                                                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                    if (res != null)
                                                    {
                                                        if (res.ResultCode == 1000)
                                                        {
                                                            DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                            {
                                                                MSISDN = dya_trans.msisdn,
                                                                Amount = dya_trans.amount,
                                                                ServiceID = service.service_id,
                                                                TokenID = res.TokenID,
                                                                TransactionID = dya_trans.dya_trans.ToString()
                                                            };
                                                            DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                            if (momo_response != null)
                                                            {
                                                                if (momo_response.ResultCode == 1000)
                                                                {
                                                                    user_was_refund = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                text_msg_ms1 = text_msg_ms1 = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                //CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                                //TODO start refund

                                            }
                                        }

                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_ms1)
                        {
                            if (!String.IsNullOrEmpty(text_msg_ms1))
                            {
                                lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_ms1, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = dya_trans.msisdn,
                                    Text = text_msg_ms1,
                                    TokenID = token_id_ms1,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;

                    case 956:
                        string token_id_ng = "", text_msg_ng = "We are sorry, we encountered an error.";
                        bool login_continue_ng = false;
                        LoginRequest RequestBody_ng = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_ng = Login.DoLogin(RequestBody_ng);
                        if (response_ng != null)
                        {
                            if (response_ng.ResultCode == 1000)
                            {
                                token_id_ng = response_ng.TokenID;
                                login_continue_ng = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_ng = false;
                            }

                            if (login_continue_ng)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_ms = iDoBetMTNNG.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_ms = (user_ms.isValid == true ? iDoBetMTNNG.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        text_msg_ng = "Hello, your deposit has been successfully placed. ";
                                        text_msg_ng = text_msg_ng + "Bet online to win amazing bonuses. ";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_ms = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_ng = "Hello, your payment has failed. You will be reimbursed!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetMTNNG.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_ng = "The money was successfully collected";
                                            }
                                            else
                                            {
                                                text_msg_ng = "The money collection failed";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {

                                        if (dya_trans.partner_transid.Contains("DusanLotto_"))
                                        {
                                            string postBody = "", response_body = "";
                                            string ticket_number = "";
                                            string game_numbers = "";
                                            ticket_number = CommonFuncations.DusanLotto.PlaceBet(dya_trans, out postBody, out response_body, out game_numbers, ref lines);
                                            if (!String.IsNullOrEmpty(ticket_number))
                                            {
                                                string product_name = "YD Lotto";
                                                text_msg_ng = "Thank you for playing " + product_name + ".";
                                                text_msg_ng = text_msg_ng + " Your chosen numbers are " + game_numbers.Replace(",", " ");// + Environment.NewLine;
                                                text_msg_ng = text_msg_ng + ". Amount: " + dya_trans.amount;// Environment.NewLine;
                                                text_msg_ng = text_msg_ng + ". Bar Code: " + ticket_number;// Environment.NewLine;
                                                text_msg_ng = text_msg_ng + ". Change your life with " + product_name + "!";// Environment.NewLine;
                                            }
                                            else
                                            {
                                                bool user_was_refund = false;
                                                LoginRequest LoginRequestBody = new LoginRequest()
                                                {
                                                    ServiceID = service.service_id,
                                                    Password = service.service_password
                                                };
                                                LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                if (res != null)
                                                {
                                                    if (res.ResultCode == 1000)
                                                    {
                                                        DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                        {
                                                            MSISDN = dya_trans.msisdn,
                                                            Amount = dya_trans.amount,
                                                            ServiceID = service.service_id,
                                                            TokenID = res.TokenID,
                                                            TransactionID = dya_trans.dya_trans.ToString()
                                                        };
                                                        DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                        if (momo_response != null)
                                                        {
                                                            if (momo_response.ResultCode == 1000)
                                                            {
                                                                user_was_refund = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                text_msg_ng = text_msg_ng = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                string mail_body = "<p><h2>Lotto place bet has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                string mail_subject = "Lotto place bet has failed for user - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            }
                                        }
                                        else
                                        {
                                            string postBody = "", response_body = "";
                                            //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                            ExecuteOrderDetails barcode = null;
                                            int real_amount = Convert.ToInt32(Math.Round((dya_trans.amount * 0.8), 0));
                                            dya_trans.amount = real_amount;
                                            if (dya_trans.partner_transid.Contains("Rapidos_"))
                                            {
                                                barcode = CommonFuncations.iDoBetMTNNG.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                            }
                                            else
                                            {
                                                barcode = CommonFuncations.iDoBetMTNNG.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                            }

                                            if (barcode != null)
                                            {
                                                text_msg_ng = "Hello, your bet has been successfully placed. ";// + Environment.NewLine;
                                                text_msg_ng = text_msg_ng + "Order Number: " + barcode.order_number + " ";// Environment.NewLine;
                                                text_msg_ng = text_msg_ng + "Barcode: " + barcode.barcode + " ";//Environment.NewLine;
                                                text_msg_ng = text_msg_ng + "Amount: " + barcode.amount + " ";//Environment.NewLine;
                                                text_msg_ng = text_msg_ng + "Maximum Gain: " + barcode.max_payout;//Environment.NewLine;
                                                lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                                if (!String.IsNullOrEmpty(barcode.total_bonus))
                                                {
                                                    text_msg_ng = text_msg_ng + " Bonus: " + barcode.total_bonus;//Environment.NewLine;
                                                }
                                                //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                            }
                                            else
                                            {
                                                if (postBody != "")
                                                {
                                                    bool user_was_refund = false;
                                                    LoginRequest LoginRequestBody = new LoginRequest()
                                                    {
                                                        ServiceID = service.service_id,
                                                        Password = service.service_password
                                                    };
                                                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                    if (res != null)
                                                    {
                                                        if (res.ResultCode == 1000)
                                                        {
                                                            RefundAirTimeRequest momo_request = new RefundAirTimeRequest()
                                                            {
                                                                MSISDN = dya_trans.msisdn,
                                                                Amount = dya_trans.amount,
                                                                ServiceID = service.service_id,
                                                                TokenID = res.TokenID,
                                                                TransactionID = dya_trans.dya_trans.ToString()
                                                            };
                                                            //RefundAirTimeResponse momo_response = RefundAirTime.DoRefundAirTime(momo_request);
                                                            RefundAirTimeResponse momo_response = null;
                                                            //DYATransferMoneyResponse momo_response = null;
                                                            if (momo_response != null)
                                                            {
                                                                if (momo_response.ResultCode == 1000)
                                                                {
                                                                    user_was_refund = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    text_msg_ng = text_msg_ng = "We are sorry, the bet failed. You will be reimbursed!";
                                                    string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                    string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                    string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                    string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                    string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                    string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                    int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                    string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                    CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                                }
                                                else
                                                {
                                                    text_msg_ng = "";
                                                    login_continue_ng = false;
                                                }

                                                //TODO start refund

                                            }
                                        }

                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_ng)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_ng, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_ng,
                                TokenID = token_id_ng,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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



                        break;

                    case 733:
                        string token_id_cg = "", text_msg_cg = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_cg = false;
                        LoginRequest RequestBody_cg = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_cg = Login.DoLogin(RequestBody_cg);
                        if (response_cg != null)
                        {
                            if (response_cg.ResultCode == 1000)
                            {
                                token_id_cg = response_cg.TokenID;
                                login_continue_cg = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_cg = false;
                            }

                            if (login_continue_cg)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_ms = null;
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_ms = false;
                                    user_ms = B2TechCGMTN.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    result_ms = (user_ms.isValid == true ? B2TechCGMTN.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);

                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        text_msg_cg = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_cg = text_msg_cg + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        login_continue_cg = false;
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                //DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_ms = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_cg = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetMTNCongo.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_cg = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_cg = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        string postBody = "", response_body = "";
                                        //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                        ExecuteOrderDetails barcode = null;
                                        if (dya_trans.partner_transid.Contains("Rapidos_"))
                                        {
                                            barcode = CommonFuncations.iDoBetMTNCongo.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                        }
                                        else
                                        {
                                            barcode = CommonFuncations.B2TechCGMTN.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                        }

                                        if (barcode != null)
                                        {
                                            text_msg_cg = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                            text_msg_cg = text_msg_cg + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                            text_msg_cg = text_msg_cg + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                            text_msg_cg = text_msg_cg + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                            text_msg_cg = text_msg_cg + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                            lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                            if (!String.IsNullOrEmpty(barcode.total_bonus))
                                            {
                                                text_msg_cg = text_msg_cg + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                            }
                                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {
                                            if (postBody != "")
                                            {
                                                bool user_was_refund = false;
                                                LoginRequest LoginRequestBody = new LoginRequest()
                                                {
                                                    ServiceID = service.service_id,
                                                    Password = service.service_password
                                                };
                                                LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                if (res != null)
                                                {
                                                    if (res.ResultCode == 1000)
                                                    {
                                                        DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                        {
                                                            MSISDN = dya_trans.msisdn,
                                                            Amount = dya_trans.amount,
                                                            ServiceID = service.service_id,
                                                            TokenID = res.TokenID,
                                                            TransactionID = dya_trans.dya_trans.ToString()
                                                        };
                                                        DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                        //DYATransferMoneyResponse momo_response = null;
                                                        if (momo_response != null)
                                                        {
                                                            if (momo_response.ResultCode == 1000)
                                                            {
                                                                user_was_refund = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                text_msg_cg = text_msg_cg = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            }
                                            else
                                            {
                                                text_msg_cg = "";
                                                login_continue_cg = false;
                                            }

                                            //TODO start refund

                                        }
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_cg)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_cg, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_cg,
                                TokenID = token_id_cg,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;

                    case 1101:
                        string token_id_cg1 = "", text_msg_cg1 = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_cg1 = false;
                        LoginRequest RequestBody_cg1 = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_cg1 = Login.DoLogin(RequestBody_cg1);
                        if (response_cg1 != null)
                        {
                            if (response_cg1.ResultCode == 1000)
                            {
                                token_id_cg1 = response_cg1.TokenID;
                                login_continue_cg1 = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_cg1 = false;
                            }
                            if (dya_trans.partner_transid.Length >= 44)
                            {
                                login_continue_cg1 = false;
                            }

                            if (login_continue_cg1)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_ms = null;
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_ms = false;
                                    user_ms = B2TechCGAirtel.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    result_ms = (user_ms.isValid == true ? B2TechCGAirtel.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);

                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        text_msg_cg = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_cg = text_msg_cg + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        login_continue_cg = false;
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_ms = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_cg1 = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }


                                if (dya_trans.partner_transid.Contains("POSTrans_"))
                                {
                                    //take money from subagent and deposit to momo
                                    Int64 number;
                                    bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                    bool collect_send_money = false;
                                    if (success)
                                    {
                                        //collect_send_money = iDoBetAirtelCongo.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                        if (collect_send_money)
                                        {
                                            text_msg_cg1 = "L'argent a ete collecte avec succes";
                                        }
                                        else
                                        {
                                            text_msg_cg1 = "L'argent a ete collecte a echoue";
                                            string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                            string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        }
                                    }



                                }
                                else
                                {
                                    string postBody = "", response_body = "";
                                    //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                    ExecuteOrderDetails barcode = null;
                                    //if (dya_trans.partner_transid.Contains("Rapidos_"))
                                    //{
                                    //    barcode = CommonFuncations.iDoBetMTNCongo.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                    //}
                                    //else
                                    //{
                                    barcode = CommonFuncations.B2TechCGAirtel.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                    //}

                                    if (barcode != null)
                                    {
                                        text_msg_cg1 = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                        text_msg_cg1 = text_msg_cg1 + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                        text_msg_cg1 = text_msg_cg1 + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                        text_msg_cg1 = text_msg_cg1 + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                        text_msg_cg1 = text_msg_cg1 + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                        lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                        if (!String.IsNullOrEmpty(barcode.total_bonus))
                                        {
                                            text_msg_cg1 = text_msg_cg1 + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                        }
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {
                                        if (postBody != "")
                                        {
                                            bool user_was_refund = false;
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = dya_trans.dya_trans.ToString()
                                                    };
                                                    DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    //DYATransferMoneyResponse momo_response = null;
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund = true;
                                                        }
                                                    }
                                                }
                                            }
                                            text_msg_cg1 = text_msg_cg1 = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                        }
                                        else
                                        {
                                            text_msg_cg1 = "";
                                            login_continue_cg1 = false;
                                        }

                                        //TODO start refund

                                    }
                                }



                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                            if (login_continue_cg1)
                            {
                                lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_cg1, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = dya_trans.msisdn,
                                    Text = text_msg_cg1,
                                    TokenID = token_id_cg1,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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

                        break;
                    case 913:
                    case 888:
                    case 777:
                        string token_id_msd = "", text_msg_msd = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_msd = false;
                        LoginRequest RequestBody_msd = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_msd = Login.DoLogin(RequestBody_msd);
                        if (response_msd != null)
                        {
                            if (response_msd.ResultCode == 1000)
                            {
                                token_id_msd = response_msd.TokenID;
                                login_continue_msd = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_msd = false;
                            }

                            if (login_continue_msd)
                            {
                                if (dya_trans.partner_transid.Contains("DusanLotto_"))
                                {
                                    string postBody = "", response_body = "";
                                    string ticket_number = "";
                                    string game_numbers = "";
                                    ticket_number = CommonFuncations.DusanLotto.PlaceBet(dya_trans, out postBody, out response_body, out game_numbers, ref lines);
                                    if (!String.IsNullOrEmpty(ticket_number))
                                    {
                                        string product_name = (service.service_id == 913 ? "LOTO CASH+" : "Loto FORTUNE");
                                        text_msg_msd = "Bonjour! Merci d'avoir jouer au " + product_name + ".";
                                        text_msg_msd = text_msg_msd + " Vos numéros choisis sont " + game_numbers.Replace(",", " ");// + Environment.NewLine;
                                        text_msg_msd = text_msg_msd + ". MISE: " + dya_trans.amount;// Environment.NewLine;
                                        text_msg_msd = text_msg_msd + ". CODE BARRE: " + ticket_number;// Environment.NewLine;
                                        text_msg_msd = text_msg_msd + ". Change ta vie avec " + product_name + "!";// Environment.NewLine;
                                    }
                                    else
                                    {
                                        bool user_was_refund = false;
                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = dya_trans.dya_trans.ToString()
                                                };
                                                DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund = true;
                                                    }
                                                }
                                            }
                                        }
                                        text_msg_msd = text_msg_msd = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Lotto place bet has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Lotto place bet has failed for user - " + dya_trans.msisdn;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                    }
                                }
                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_msd)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_msd, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_msd,
                                TokenID = token_id_msd,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 723:
                    case 758:
                    case 716:
                    case 1132:
                    case 1133:
                        string token_id_ms = "", text_msg_ms = "";// ;
                        bool login_continue_ms = false;
                        LoginRequest RequestBody_ms = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_ms = Login.DoLogin(RequestBody_ms);
                        if (response_ms != null)
                        {
                            if (response_ms.ResultCode == 1000)
                            {
                                token_id_ms = response_ms.TokenID;
                                login_continue_ms = true;
                            }
                        }

                        if (StatusCode == "01" || StatusCode == "1000")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_ms = false;
                            }

                            if (login_continue_ms)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_ms = null;
                                    bool result_ms = false;
                                    string postBody = "", response_body = "";
                                    
                                    
                                        user_ms = B2TechGNMTN.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    if (service.service_id == 1133)
                                    {
                                        result_ms = (user_ms.isValid == true ? B2TechGNOrange.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    }
                                    else
                                    {
                                        result_ms = (user_ms.isValid == true ? B2TechGNMTN.StartDepositNew(user_ms, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    }
                                        
                                    
                                    //IdoBetUser user_ms = iDoBetMTNGuinea.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    

                                    
                                    bool user_was_refund_ms = false;
                                    if (result_ms)
                                    {
                                        text_msg_ms = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_ms = text_msg_ms + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_ms = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_ms = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_ms.id + "<br><b>Name:</b> " + user_ms.firstName + " " + user_ms.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_ms + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_ms.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetMTNGuinea.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_ms = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_ms = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        if (dya_trans.partner_transid.Contains("Rapidos_") || dya_trans.partner_transid.Length == 36)
                                        {
                                            string postBody = "", response_body = "";
                                            //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                            ExecuteOrderDetails barcode = null;
                                            if (dya_trans.partner_transid.Contains("Rapidos_"))
                                            {
                                                barcode = CommonFuncations.iDoBetMTNGuinea.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                            }
                                            else
                                            {
                                            if (service.service_id == 1133)
                                                {
                                                    barcode = CommonFuncations.B2TechGNOrange.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                                }
                                                else
                                                {
                                                    barcode = CommonFuncations.B2TechGNMTN.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                                }
                                                    
                                                

                                            }

                                            if (barcode != null)
                                            {
                                                text_msg_ms = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                                text_msg_ms = text_msg_ms + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                                text_msg_ms = text_msg_ms + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                                text_msg_ms = text_msg_ms + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                                text_msg_ms = text_msg_ms + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                                lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                                if (!String.IsNullOrEmpty(barcode.total_bonus))
                                                {
                                                    text_msg_ms = text_msg_ms + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                                }

                                                //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                            }
                                            else
                                            {
                                                if (response_body.Contains(""))
                                                {

                                                }
                                                bool user_was_refund = false;
                                                LoginRequest LoginRequestBody = new LoginRequest()
                                                {
                                                    ServiceID = service.service_id,
                                                    Password = service.service_password
                                                };
                                                LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                if (res != null)
                                                {
                                                    if (res.ResultCode == 1000)
                                                    {
                                                        DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                        {
                                                            MSISDN = dya_trans.msisdn,
                                                            Amount = dya_trans.amount,
                                                            ServiceID = service.service_id,
                                                            TokenID = res.TokenID,
                                                            TransactionID = dya_trans.dya_trans.ToString()
                                                        };
                                                        DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                        if (momo_response != null)
                                                        {
                                                            if (momo_response.ResultCode == 1000)
                                                            {
                                                                user_was_refund = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                text_msg_ms = text_msg_ms = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                                //TODO start refund

                                            }
                                        }
                                        
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_ms)
                        {
                            if (!String.IsNullOrEmpty(text_msg_ms))
                            {
                                lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_ms, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = dya_trans.msisdn,
                                    Text = text_msg_ms,
                                    TokenID = token_id_ms,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMSWU(RequestSendSMSBody, "https://api125.ydplatform.com/api/sendsms");
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 755:
                        string token_id_cmo = "", text_msg_cmo = "";
                        bool login_continue_cmo = false;
                        LoginRequest RequestBody_cmo = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_cmo = Login.DoLogin(RequestBody_cmo);
                        if (response_cmo != null)
                        {
                            if (response_cmo.ResultCode == 1000)
                            {
                                token_id_cmo = response_cmo.TokenID;
                                login_continue_cmo = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_cmo = false;
                            }

                            if (login_continue_cmo)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_cmo = iDoBetOrangeCameroon.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_cmo = (user_cmo.isValid == true ? iDoBetOrangeCameroon.StartDepositNew(user_cmo, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    bool user_was_refund_cmo = false;
                                    if (result_cmo)
                                    {
                                        text_msg_cmo = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_cmo = text_msg_cmo + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;

                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_cmo = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_ms = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_cmo.id + "<br><b>Name:</b> " + user_cmo.firstName + " " + user_cmo.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_cmo + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_cmo.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetOrangeCameroon.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_ms = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_ms = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        string postBody = "", response_body = "";
                                        //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                        ExecuteOrderDetails barcode = null;
                                        if (dya_trans.partner_transid.Contains("Rapidos_"))
                                        {
                                            barcode = CommonFuncations.iDoBetOrangeCameroon.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                        }
                                        else
                                        {
                                            barcode = CommonFuncations.iDoBetOrangeCameroon.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                        }

                                        if (barcode != null)
                                        {
                                            text_msg_cmo = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                            text_msg_cmo = text_msg_cmo + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                            text_msg_cmo = text_msg_cmo + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                            text_msg_cmo = text_msg_cmo + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                            text_msg_cmo = text_msg_cmo + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                            lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                            if (!String.IsNullOrEmpty(barcode.total_bonus))
                                            {
                                                text_msg_cmo = text_msg_cmo + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                            }
                                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {
                                            bool user_was_refund = false;
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = dya_trans.dya_trans.ToString()
                                                    };
                                                    DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund = true;
                                                        }
                                                    }
                                                }
                                            }
                                            text_msg_cmo = text_msg_cmo = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            //TODO start refund

                                        }
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_cmo)
                        {
                            if (!String.IsNullOrEmpty(text_msg_cmo))
                            {
                                lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_cmo, 100, lines[0].ControlerName);
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = dya_trans.msisdn,
                                    Text = text_msg_cmo,
                                    TokenID = token_id_cmo,
                                    TransactionID = "12345"
                                };
                                SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                                if (response_sendsms != null)
                                {
                                    if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 754:
                    case 720:
                        string token_id_cm = "", text_msg_cm = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_cm = false;
                        LoginRequest RequestBody_cm = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_cm = Login.DoLogin(RequestBody_cm);
                        if (response_cm != null)
                        {
                            if (response_cm.ResultCode == 1000)
                            {
                                token_id_cm = response_cm.TokenID;
                                login_continue_cm = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_cm = false;
                            }

                            if (login_continue_cm)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_cm = iDoBetMTNCameroon.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_cm = (user_cm.isValid == true ? iDoBetMTNCameroon.StartDepositNew(user_cm, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    bool user_was_refund_cm = false;
                                    if (result_cm)
                                    {
                                        text_msg_cm = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_cm = text_msg_cm + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_cm = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_ms = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_cm.id + "<br><b>Name:</b> " + user_cm.firstName + " " + user_cm.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_cm + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_cm.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetMTNCameroon.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_ms = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_ms = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        string postBody = "", response_body = "";
                                        //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                        ExecuteOrderDetails barcode = null;
                                        if (dya_trans.partner_transid.Contains("Rapidos_"))
                                        {
                                            barcode = CommonFuncations.iDoBetMTNCameroon.PlaceBetRapidos(dya_trans, out postBody, out response_body, ref lines);
                                        }
                                        else
                                        {
                                            barcode = CommonFuncations.iDoBetMTNCameroon.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                        }

                                        if (barcode != null)
                                        {
                                            text_msg_cm = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                            text_msg_cm = text_msg_cm + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                            text_msg_cm = text_msg_cm + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                            text_msg_cm = text_msg_cm + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                            text_msg_cm = text_msg_cm + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                            lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                            if (!String.IsNullOrEmpty(barcode.total_bonus))
                                            {
                                                text_msg_cm = text_msg_cm + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                            }
                                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {
                                            bool user_was_refund = false;
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = dya_trans.dya_trans.ToString()
                                                    };
                                                    DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund = true;
                                                        }
                                                    }
                                                }
                                            }
                                            text_msg_cm = text_msg_cm = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            //TODO start refund

                                        }
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_cm)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_cm, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_cm,
                                TokenID = token_id_cm,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 730:
                        string token_id_lnbmoov = "", text_msg_lnbmoov = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_lnbmoov = false;
                        LoginRequest RequestBody_lnbmoov = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_lnbmoov = Login.DoLogin(RequestBody_lnbmoov);
                        if (response_lnbmoov != null)
                        {
                            if (response_lnbmoov.ResultCode == 1000)
                            {
                                token_id_lnbmoov = response_lnbmoov.TokenID;
                                login_continue_lnbmoov = true;
                            }
                        }

                        if (StatusCode == "1000")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_lnbmoov = false;
                            }
                            if (dya_trans.partner_transid.Length >= 44)
                            {
                                login_continue_lnbmoov = false;
                            }

                            if (login_continue_lnbmoov)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    
                                        //deposit idobet
                                        IdoBetUser user_m = B2TechLNBMoov.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                        string postBody = "", response_body = "";
                                        //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                        bool result_lnbm = (user_m.isValid == true ? B2TechLNBMoov.StartDepositNew(user_m, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                        bool user_was_refund_lnbmoov = false;
                                        if (result_lnbm)
                                        {
                                            text_msg_lnbmoov = "Bonjour, votre depot a ete place avec succes.\n";
                                            text_msg_lnbmoov = text_msg_lnbmoov + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {

                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                    };
                                                    //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    DYATransferMoneyResponse momo_response = null;
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund_lnbmoov = true;
                                                        }
                                                    }
                                                }
                                            }


                                            text_msg_lnbmoov = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_m.id + "<br><b>Name:</b> " + user_m.firstName + " " + user_m.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_lnbmoov + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Start Deposit has failed for user - " + user_m.id;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            //TODO start refund
                                        }
                                        //send sms
                                    

                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetLNBPariMoovBenin.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_lnbmoov = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_lnbmoov = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        if (dya_trans.partner_transid.Length == 36)
                                        {
                                            string postBody = "", response_body = "";
                                            CommonFuncations.iDoBet.ExecuteOrderDetails barcode = null;
                                            barcode = CommonFuncations.B2TechLNBMoov.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                            if (barcode != null)
                                            {
                                                text_msg_lnbmoov = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                                text_msg_lnbmoov = text_msg_lnbmoov + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                                text_msg_lnbmoov = text_msg_lnbmoov + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                                text_msg_lnbmoov = text_msg_lnbmoov + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                                text_msg_lnbmoov = text_msg_lnbmoov + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;
                                                lines = Add2Log(lines, " total_bonus =  " + barcode.total_bonus, 100, lines[0].ControlerName);
                                                if (!String.IsNullOrEmpty(barcode.total_bonus))
                                                {
                                                    text_msg_lnbmoov = text_msg_lnbmoov + "\nBonus: " + barcode.total_bonus;//Environment.NewLine;
                                                }
                                                //GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                            }
                                            else
                                            {
                                                bool user_was_refund = false;
                                                LoginRequest LoginRequestBody = new LoginRequest()
                                                {
                                                    ServiceID = service.service_id,
                                                    Password = service.service_password
                                                };
                                                LoginResponse res = Login.DoLogin(LoginRequestBody);
                                                if (res != null)
                                                {
                                                    if (res.ResultCode == 1000)
                                                    {
                                                        DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                        {
                                                            MSISDN = dya_trans.msisdn,
                                                            Amount = dya_trans.amount,
                                                            ServiceID = service.service_id,
                                                            TokenID = res.TokenID,
                                                            TransactionID = dya_trans.dya_trans.ToString()
                                                        };
                                                        DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                        if (momo_response != null)
                                                        {
                                                            if (momo_response.ResultCode == 1000)
                                                            {
                                                                user_was_refund = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                text_msg_lnbmoov = text_msg_lnbmoov = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                                string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                                //TODO start refund

                                            }
                                        }
                                            
                                        
                                        
                                        
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_lnbmoov)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_lnbmoov, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_lnbmoov,
                                TokenID = token_id_lnbmoov,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;

                    case 715:
                        string token_id_m = "", text_msg_m = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue_m = false;
                        LoginRequest RequestBody_m = new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        };
                        LoginResponse response_m = Login.DoLogin(RequestBody_m);
                        if (response_m != null)
                        {
                            if (response_m.ResultCode == 1000)
                            {
                                token_id_m = response_m.TokenID;
                                login_continue_m = true;
                            }
                        }

                        if (StatusCode == "1000")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue_m = false;
                            }

                            if (login_continue_m)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user_m = iDoBetMoov.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result_m = (user_m.isValid == true ? iDoBetMoov.StartDepositNew(user_m, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    bool user_was_refund_m = false;
                                    if (result_m)
                                    {
                                        text_msg_m = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg_m = text_msg_m + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund_m = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg_m = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user_m.id + "<br><b>Name:</b> " + user_m.firstName + " " + user_m.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund_m + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user_m.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = iDoBetMoov.TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg_m = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg_m = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        string postBody = "", response_body = "";
                                        //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                        ExecuteOrderDetails barcode = CommonFuncations.iDoBetMoov.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                        if (barcode != null)
                                        {
                                            text_msg_m = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                            text_msg_m = text_msg_m + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                            text_msg_m = text_msg_m + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                            text_msg_m = text_msg_m + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                            text_msg_m = text_msg_m + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;

                                            GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {
                                            bool user_was_refund = false;
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = dya_trans.dya_trans.ToString()
                                                    };
                                                    DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund = true;
                                                        }
                                                    }
                                                }
                                            }
                                            text_msg_m = text_msg_m = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            //TODO start refund

                                        }
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue_m)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg_m, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg_m,
                                TokenID = token_id_m,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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
                        break;
                    case 706:
                    case 683:
                    case 684:
                    case 682:
                        string token_id = "", text_msg = "Nous sommes desoles, nous avons rencontre une erreur.";
                        bool login_continue = false;
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
                                login_continue = true;
                            }
                        }

                        if (StatusCode == "01")
                        {

                            if (dya_trans.partner_transid.ToLower().Contains("online_"))
                            {
                                login_continue = false;
                            }

                            if (login_continue)
                            {


                                if (dya_trans.partner_transid.Contains("iDoBetDeposit_"))
                                {
                                    //deposit idobet
                                    IdoBetUser user = SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    string postBody = "", response_body = "";
                                    //bool result = StartDeposit(user, dya_trans.amount, out postBody, out response_body, ref lines);
                                    bool result = (user.isValid == true ? StartDepositNew(user, dya_trans.dya_trans.ToString(), dya_trans.datetime, dya_trans.amount, dya_trans.msisdn.ToString(), out postBody, out response_body, ref lines) : false);
                                    bool user_was_refund = false;
                                    if (result)
                                    {
                                        text_msg = "Bonjour, votre depot a ete place avec succes.\n";
                                        text_msg = text_msg + "Pariez en ligne pour gagner des bonus incroyables.\n";
                                        GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "Deposit", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                    }
                                    else
                                    {

                                        LoginRequest LoginRequestBody = new LoginRequest()
                                        {
                                            ServiceID = service.service_id,
                                            Password = service.service_password
                                        };
                                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                                        if (res != null)
                                        {
                                            if (res.ResultCode == 1000)
                                            {
                                                DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                {
                                                    MSISDN = dya_trans.msisdn,
                                                    Amount = dya_trans.amount,
                                                    ServiceID = service.service_id,
                                                    TokenID = res.TokenID,
                                                    TransactionID = "RefundFailedDeposit_" + dya_trans.dya_trans.ToString()
                                                };
                                                //DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                DYATransferMoneyResponse momo_response = null;
                                                if (momo_response != null)
                                                {
                                                    if (momo_response.ResultCode == 1000)
                                                    {
                                                        user_was_refund = true;
                                                    }
                                                }
                                            }
                                        }


                                        text_msg = "Bonjour, votre paiement a echoue. Vous serez rembourse!";
                                        string mail_body = "<p><h2>Start Deposit has failed</h2><b>UserID:</b> " + user.id + "<br><b>Name:</b> " + user.firstName + " " + user.lastName + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>MSISDN: </b>" + dya_trans.msisdn + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>iDoBetResponse: </b>" + response_body + "<br></p>";
                                        string mail_subject = "Start Deposit has failed for user - " + user.id;
                                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                        //TODO start refund
                                    }
                                    //send sms
                                }
                                else
                                {
                                    if (dya_trans.partner_transid.Contains("POSTrans_"))
                                    {
                                        //take money from subagent and deposit to momo
                                        Int64 number;
                                        bool success = Int64.TryParse(dya_trans.partner_transid.Replace("POSTrans_", ""), out number);
                                        bool collect_send_money = false;
                                        if (success)
                                        {
                                            collect_send_money = TakeMoneyFromSubAgentAndDeposit2MOMO(dya_trans.dya_trans, number, ref lines);
                                            if (collect_send_money)
                                            {
                                                text_msg = "L'argent a ete collecte avec succes";
                                            }
                                            else
                                            {
                                                text_msg = "L'argent a ete collecte a echoue";
                                                string mail_body = "<p><h2>Money Collection has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>POSTrans: </b>" + dya_trans.partner_transid + "<br></p>";
                                                string mail_subject = "Money Collection has failed - " + dya_trans.msisdn;
                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                            }
                                        }



                                    }
                                    else
                                    {
                                        string postBody = "", response_body = "";
                                        //ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrder(dya_trans, out postBody, out response_body, ref lines);
                                        ExecuteOrderDetails barcode = CommonFuncations.iDoBet.GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                                        if (barcode != null)
                                        {
                                            text_msg = "Bonjour, votre pari a ete place avec succes.\n";// + Environment.NewLine;
                                            text_msg = text_msg + "Numero de commande: " + barcode.order_number + "\n";// Environment.NewLine;
                                            text_msg = text_msg + "Code a barre: " + barcode.barcode + "\n";//Environment.NewLine;
                                            text_msg = text_msg + "Montant: " + barcode.amount + "\n";//Environment.NewLine;
                                            text_msg = text_msg + "Gain Maximum: " + barcode.max_payout;//Environment.NewLine;

                                            GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(dya_trans.msisdn.ToString()), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "Bet", "MOMO", dya_trans.amount.ToString(), "/", ref lines);
                                        }
                                        else
                                        {
                                            bool user_was_refund = false;
                                            LoginRequest LoginRequestBody = new LoginRequest()
                                            {
                                                ServiceID = service.service_id,
                                                Password = service.service_password
                                            };
                                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                                            if (res != null)
                                            {
                                                if (res.ResultCode == 1000)
                                                {
                                                    DYATransferMoneyRequest momo_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = dya_trans.msisdn,
                                                        Amount = dya_trans.amount,
                                                        ServiceID = service.service_id,
                                                        TokenID = res.TokenID,
                                                        TransactionID = dya_trans.dya_trans.ToString()
                                                    };
                                                    DYATransferMoneyResponse momo_response = DYATransferMoney.DoTransfer(momo_request);
                                                    if (momo_response != null)
                                                    {
                                                        if (momo_response.ResultCode == 1000)
                                                        {
                                                            user_was_refund = true;
                                                        }
                                                    }
                                                }
                                            }
                                            text_msg = "Nous sommes desoles, le pari a echoue. Vous serez rembourse!";
                                            string mail_body = "<p><h2>Excecute Order has failed</h2><b>MSISDN:</b> " + dya_trans.msisdn + "<br><b>DYATransactionID:</b> " + dya_trans.dya_trans + "<br><b>Amount: </b>" + dya_trans.amount + "<br><b>UserSession: </b>" + dya_trans.partner_transid + "<br><b>User was refunded: </b>" + user_was_refund + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                            string mail_subject = "Excecute Order has failed for user - " + dya_trans.msisdn;
                                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);

                                            //TODO start refund

                                        }
                                    }

                                }

                            }
                            else
                            {
                                lines = Add2Log(lines, " Login Failed", 100, lines[0].ControlerName);
                            }
                        }
                        if (login_continue)
                        {
                            lines = Add2Log(lines, " Sending SMS to " + dya_trans.msisdn + " " + text_msg, 100, lines[0].ControlerName);
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = dya_trans.msisdn,
                                Text = text_msg,
                                TokenID = token_id,
                                TransactionID = "12345"
                            };
                            SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                            if (response_sendsms != null)
                            {
                                if (response_sendsms.ResultCode == 1000 || response_sendsms.ResultCode == 1010)
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

                        break;
                }
            }
            else
            {
                lines = Add2Log(lines, " User is in black list", 100, lines[0].ControlerName);
            }
            
        }

        public static void DecideDYAFailedNotification(Int64 dya_id, int service_id, string partner_transid, string url, string body, int json_xml, ref List<LogLines> lines)
        {
            lines = Add2Log(lines, " Checking DecideDYAFailedNotification() : ", 100, lines[0].ControlerName);
            bool took_action = false;
            switch (service_id)
            {
                case 716:
                case 732:
                case 759:

                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + url + "','" + body + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    break;
                case 715:
                case 682:
                    if (partner_transid.ToLower().Contains("online_13_"))
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_notify_retry(dya_id, url, body, json_xml, next_date_time, number_of_retry, is_successful) values("+ dya_id + ", '"+ url + "','"+body+"',"+json_xml+ ", now(), 0,0) ON DUPLICATE KEY UPDATE number_of_retry = number_of_retry + 1", ref lines);
                        took_action = true;
                    }
                    break;
            }
            if (took_action == false)
            {
                lines = Add2Log(lines, " DecideDYAFailedNotification() - Doing nothing : ", 100, lines[0].ControlerName);
            }
        }
    }
}