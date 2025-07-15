using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Benin
    {
        public class returned_service
        {
            public int service_id { get; set; }
            public string returned_sms_text { get; set; }
            public string token_id { get; set; }
        }

        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            //msisdn = msisdn.Substring(3);
            short_code = (short_code == "500" ? "LNBPari" : short_code);
            string url = Cache.ServerSettings.GetServerSettings("BeninKannelURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + System.Uri.EscapeDataString(text) + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "") + "&source-addr-ton=1&source-addr-npi=1&dest-addr-ton=1&dest-addr-npi=1";
            lines = Add2Log(lines, "SMS Url = " + url, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }

        public static bool SendMoovSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            //msisdn = msisdn.Substring(3);
            short_code = (short_code == "500" ? "LNBPari" : short_code);
            string url = Cache.ServerSettings.GetServerSettings("MoovKannelURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + System.Uri.EscapeDataString(text) + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "");
            lines = Add2Log(lines, "SMS Url = " + url, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }

        public static string ChargeAmount(string msisdn, ServiceClass service, string dya_id, ref List<LogLines> lines)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();

            string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/payment/amount_charging/v2_1/local\">";
            soap = soap + "<soapenv:Header>";
            soap = soap + "<RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "<spId>"+ service.spid + "</spId>";
            soap = soap + "<spPassword>"+ final_password + "</spPassword>";
            soap = soap + "<bundleID></bundleID>";
            soap = soap + "<timeStamp>"+ timeStamp + "</timeStamp>";
            soap = soap + "<oauth_token></oauth_token>";
            soap = soap + "</RequestSOAPHeader>";
            soap = soap + "</soapenv:Header>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<loc:chargeAmount>";
            soap = soap + "<loc:endUserIdentifier>"+ msisdn + "</loc:endUserIdentifier>";
            soap = soap + "<loc:charge>";
            soap = soap + "<description>charge</description>";
            soap = soap + "<currency>XOF</currency>";
            soap = soap + "<amount>"+ (service.airtimr_amount * 100) + "</amount>";
            soap = soap + "<code></code>";
            soap = soap + "</loc:charge>";
            soap = soap + "<loc:referenceCode>"+ dya_id + "</loc:referenceCode>";
            soap = soap + "</loc:chargeAmount>";
            soap = soap + "</soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";
            string url_name = "SDPChargeURL_" + service.operator_id + (service.is_staging == true ? "_STG" : "");
            string url = Cache.ServerSettings.GetServerSettings(url_name, ref lines);
            lines = Add2Log(lines, "Soap = " + soap, 100, "");
            lines = Add2Log(lines, "Sending to " + url, 100, "");
            List<Headers> headers = new List<Headers>();
            string response = CommonFuncations.CallSoap.CallSoapRequest(url, soap, headers, 1, true, true, ref lines);
            lines = Add2Log(lines, "Response = " + response, 100, "");
            return response;
        }

        public static string DecideWelcomeMSG()
        {
            string result = "Felicitation cher abonne votre souscription a la promo MTN Millionnaire a ete prise en compte pour 100FCFA. Vous etes eligible pour le tirage de cette journee et pour les gros lots. Vous pouvez toujours augmenter votre chance en envoyant a volonte OK au 7060 a 100F/envoie. Desactivation: STOP au 7060. Info:111 T&C: http://bit.ly/PromoMTN";
            Random rnd = new Random();
            int text_selection = rnd.Next(1, 6);
            switch (text_selection)
            {
                case 1:
                    result = "Felicitation! Votre souscription a la promo MTN Millionnaire a ete prise en compte pour 100FCFA. cher abonne vous etes eligible pour le tirage de cette journee et pour les gros lots. Vous pouvez toujours augmenter vos chances en composant a volonte *760# a 100F. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
                case 2:
                    result = "Bravo! Votre participation a la promo MTN Millionnaire a 100F ete bien enregistre pour competir pour les 1.000.000 FCFA de cette journee et pour les gros lots. Cher abonne composez a nouveau *760# pour augmenter encore vos chances de gagner. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
                case 3:
                    result = "Super! vous n etes plus loin de gagner les 1.000.000FCFA, votre souscription a la promo MTN Millionnaire a ete prise en compte pour 100FCFA. Cher abonne vous etes eligible pour le tirage de cette journee et pour les gros lots. Continuez a augmenter vos chances en composant *760# a 100F. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
                case 4:
                    result = "Encore quelques tentatives pour gagner les 1.000.000FCFA. Cher abonne votre souscription a la promo MTN Millionnaire a ete prise en compte a 100F. Vous etes eligible pour le tirage de cette journee et pour les gros lots. Augmentez encore vos chances en composant maintenant *760# a 100F. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
                case 5:
                    result = "Bingo! Vous etes a quelques participations pres pour remporter les 1.000.000FCFA. Votre souscription a la promo MTN Millionnaire a ete prise en compte a 100F. Cher abonne vous etes eligible pour le tirage de cette journee et pour les gros lots. Augmentez encore vos chances en composant *760# a 100F. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
                case 6:
                    result = "Plus que quelques participations encore pour gagner les 1.000.000FCFA de la journee. Votre souscription a la promo MTN Millionnaire a ete prise en compte a 100F. Vous etes eligible pour le tirage de cette journee et pour les gros lots. Cher abonne augmentez encore vos chances en composant *760# a 100F. Info:111 T&C: http://bit.ly/PromoMTN";
                    break;
            }

            return result;

        }

        public static bool SubscribePromoUserViaPOS(string msisdn, string mobile_operator, string date_time, string date_time1, string transaction_id, ref List<LogLines> lines)
        {
            bool result = false;
            if (mobile_operator == "12")
            {
                int service_id = 708;

                Int64 dya_id = DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) values("+msisdn+","+service_id+",'"+date_time+ "',100,0,'Success',5,'"+transaction_id+"');", ref lines);
                if (dya_id > 0)
                {
                    DataLayer.DBQueries.ExecuteQuery("insert into promo.joined_users (msisdn, service_id, date_time, keyword, dya_id) values(" + msisdn + "," + service_id + ",'"+ date_time1 + "', 'POS'," + dya_id + " );", ref lines);
                    DataLayer.DBQueries.ExecuteQuery("delete from promo.dnd where msisdn = " + msisdn, ref lines);
                    result = true;
                }
            }
            return result;
        }

        public static returned_service DecidePerTextAndShortCode(string text, string short_code, string msisdn, string mobile_operator, out bool is_close, ref List<LogLines> lines)
        {
            is_close = true;
            returned_service result = null;
            text = text.ToLower();
            string returned_sms_text = "", token = "", token1 = "";
            if (short_code == "7060" && mobile_operator == "12")
            {
                ServiceClass service = GetServiceByServiceID(708, ref lines);
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
                        token = res.TokenID;
                    }
                }

                result = new returned_service()
                {
                    returned_sms_text = "Cher abonné merci d'avoir participé à MTN PROMO Millionnaire. La Promo a pris fin le 01 Janvier 2020. La date des derniers tirages et de leur remise officielle vous sera communiquée ultérieurement. Merci.",
                    token_id = token,
                    service_id = service.service_id
                };
                return result;

                if (text == "") //wrong syntaxt
                {
                    result = new returned_service()
                    {
                        returned_sms_text = "Cher abonné, pour participer à la promo MTN Millionnaire, composez *760# ou envoyez OK par sms au 7060, ou appelez le 7060 ou encore cliquez sur le lien : promo.mtn.bj. cout 100f/participation. Deactivation: STOP at 7060. Info: 111 T & C: mtn.bj",
                        token_id = token,
                        service_id = service.service_id
                    };
                }
                //annuler, quitter, arreter
                if ((text.Contains("stop") || text.Contains("annuler") || text.Contains("quitter") || text.Contains("arreter")) && result == null) //add to dnd for 1 week
                {
                    result = new returned_service()
                    {
                        returned_sms_text = "Cher abonnne, vous avez desactive avec succès votre participation à la promo MTN millionnaire. Pour participer de nouveau à la promo, composez *760# ou envoyez OK par sms au 7060. Cout: 100f/j. Info: 111.",
                        token_id = token,
                        service_id = service.service_id
                    };
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into promo.dnd (msisdn, date_time, keyword) values("+msisdn+", now(), '"+text.Replace("'","")+"')", ref lines);
                }

                if ((text.Contains("statut") || text.Contains("total") || text.Contains("points")) && result == null)
                {
                    Int64 number_of_points = DataLayer.DBQueries.SelectQueryReturnInt64("SELECT COUNT(*) FROM promo.joined_users u WHERE u.service_id = 708 AND u.msisdn = " + msisdn, ref lines);
                    result = new returned_service()
                    {
                        returned_sms_text = "Cher abonne vous avez totalise " + number_of_points + " participations sur la promo millionnaire, vous etes donc represente " + number_of_points + " fois dans la base pour tirage. Ces participations vous permettent juste d avoir plus de chance d etre tire au sort. Desactivation: STOP au 7060. Info:111 T&C: http://bit.ly/PromoMTN",
                        token_id = token,
                        service_id = service.service_id
                    };
                }

                //STATUT, TOTAL, POINTS
                if (text.Contains("info") && result == null)
                {
                    result = new returned_service()
                    {
                        returned_sms_text = "Cher abonné, pour participer à la promo MTN Millionaire, composez *760# ou envoyez OK par sms au 7060. Cout 100f/participation. Desactivation: STOP au 7060. Pour avoir le total de ta participation envoie Statut par sms au 7060. Info:111 T&C: http://bit.ly/PromoMTN",
                        token_id = token,
                        service_id = service.service_id
                    };
                }

                if (Api.Cache.Promo.FindPromoBlockedUsers(Convert.ToInt64(msisdn), ref lines)) //User is in the black list
                {
                    result = new returned_service()
                    {
                        returned_sms_text = "Cher abonné, vous n'etes pas autorisé à vous abonner à ce service",
                        token_id = token,
                        service_id = service.service_id
                    };
                }

                if (!String.IsNullOrEmpty(text) && result == null) //need to bill user
                {
                    ChargeAirTimeRequest ChrageRequest = new ChargeAirTimeRequest()
                    {
                        ServiceID = service.service_id,
                        TokenID = token,
                        MSISDN = Convert.ToInt64(msisdn),
                        Amount = service.airtimr_amount,
                        TransactionID = Guid.NewGuid().ToString()
                    };
                    ChargeAirTimeResponse ChargeResponse = ChargeAirTime.DoChrageAirTime(ChrageRequest);
                    if (ChargeResponse!= null)
                    {
                        if (ChargeResponse.ResultCode == 1000)
                        {
                            result = new returned_service()
                            {
                                returned_sms_text = DecideWelcomeMSG(),//"Felicitation cher abonne votre souscription a la promo MTN Millionnaire a ete prise en compte pour 100FCFA. Vous etes eligible pour le tirage de cette journee et pour les gros lots. Vous pouvez toujours augmenter votre chance en envoyant a volonte OK au 7060 a 100F/envoie. Desactivation: STOP au 7060. Info:111 T&C: http://bit.ly/PromoMTN",
                                token_id = token,
                                service_id = service.service_id
                            };
                            DataLayer.DBQueries.ExecuteQuery("insert into promo.joined_users (msisdn, service_id, date_time, keyword, dya_id) values("+ msisdn + ","+ service.service_id + ",now(), '"+ text + "',"+ ChargeResponse.TransactionID + " );", ref lines);
                            DataLayer.DBQueries.ExecuteQuery("delete from promo.dnd where msisdn = " + msisdn, ref lines);
                        }
                        else
                        {
                            //Bill MOMO
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

                            DYACheckAccountRequest request = new DYACheckAccountRequest()
                            {
                                MSISDN = Convert.ToInt64(msisdn),
                                ServiceID = service1.service_id,
                                TokenID = token1
                            };
                            DYACheckAccountResponse response = CommonFuncations.DYACheckAccount.DODYACheckAccount(request);
                            if (response.ResultCode == 1000)
                            {
                                string text1 = "Cher abonné, Vous n'avez pas assez de crédit sur votre compte Voulez-vous jouer à MTN Millionnaire via votre compte Momo?" + Environment.NewLine;
                                text1 = text1 + "1. OUI" + Environment.NewLine;
                                text1 = text1 + "2. NON";

                                result = new returned_service()
                                {
                                    returned_sms_text = text1,
                                    token_id = token,
                                    service_id = service.service_id
                                };
                                is_close = false;
                            }
                            else
                            {
                                if (ChargeResponse.Description.Contains("Insufficient Balance"))
                                {
                                    result = new returned_service()
                                    {
                                        returned_sms_text = "Yello! Vous n avez pas suffisamment de credit pour participer a la promo MTN Millionnaire. Veuillez recharger votre compte et reessayer ou composer *185# pour emprunter du credit et reesayer. Merci",
                                        token_id = token,
                                        service_id = service.service_id
                                    };
                                }
                                else
                                {
                                    result = new returned_service()
                                    {
                                        returned_sms_text = "Cher abonné, votre inscription à la promo MTN Millionnaire a échoué. Veuillez réessayer plus tard.",
                                        token_id = token,
                                        service_id = service.service_id
                                    };
                                }
                            }

                        }
                    }
                    else
                    {
                        result = new returned_service()
                        {
                            returned_sms_text = "Cher abonné, votre inscription à la promo MTN Millionnaire a échoué. Veuillez réessayer plus tard.",
                            token_id = token,
                            service_id = service.service_id
                        };
                    }
                }
            }
            return result;
        }
    }
}