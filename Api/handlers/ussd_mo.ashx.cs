using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Services;
using static Api.Cache.SMS;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ussd_mo
    /// </summary>
    public class ussd_mo : IHttpHandler
    {
        public static string CheckStatusMenu(int service_id, string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            List<ServiceClass> subscriber_services = GetSimilarServices(Convert.ToInt32(service_id), ref lines);
            bool found = false;
            foreach (ServiceClass sc in subscriber_services)
            {
                LoginRequest LoginRequestBody = new LoginRequest()
                {
                    ServiceID = sc.service_id,
                    Password = sc.service_password
                };
                LoginResponse res = Login.DoLogin(LoginRequestBody);
                CheckUserStateRequest CheckUserStateBody = new CheckUserStateRequest()
                {
                    MSISDN = Convert.ToInt64(MSISDN),
                    ServiceID = sc.service_id,
                    TokenID = res.TokenID
                };
                CheckUserStateResponse res1 = CheckUserState.DoCheckUserState(CheckUserStateBody);

                if (res1 != null)
                {
                    if (res1.ResultCode == 1000)
                    {
                        if (res1.State == "Active")
                        {
                            menu = "Votre service MTN Yellow Games ";
                            int day_2_add = 0;
                            string validate_date = "", exp_time = "";
                            switch (sc.service_id)
                            {
                                case 669:
                                    menu = menu + "Jour à 50F/J est actif. Validité: ";
                                    day_2_add = 0;
                                    validate_date = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("dd/MM/yyyy") + " 23:59";
                                    exp_time = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("yyyy-MM-dd") + " 23:59:59";
                                    if (DateTime.Now < Convert.ToDateTime(exp_time))
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("UPDATE yellowdot.subscribers s SET s.state_id = 2, deactivation_date = now() WHERE s.subscriber_id = " + res1.SubscriberID, ref lines);
                                    }
                                    menu = menu + validate_date + Environment.NewLine + "0) Retour ";
                                    break;
                                case 697:
                                    menu = menu + "à 100F/03J est actif. Validité: ";
                                    day_2_add = 2;

                                    validate_date = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("dd/MM/yyyy") + " 23:59";
                                    exp_time = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("yyyy-MM-dd") + " 23:59:59";
                                    if (DateTime.Now < Convert.ToDateTime(exp_time))
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("UPDATE yellowdot.subscribers s SET s.state_id = 2, deactivation_date = now() WHERE s.subscriber_id = " + res1.SubscriberID, ref lines);
                                    }
                                    menu = menu + validate_date + Environment.NewLine + "0) Retour ";
                                    break;
                                case 698:
                                    menu = menu + "à 150/07J est actif. Validité: ";
                                    day_2_add = 6;
                                    validate_date = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("dd/MM/yyyy") + " 23:59";
                                    exp_time = Convert.ToDateTime(res1.SubscriptionDate).AddDays(day_2_add).ToString("yyyy-MM-dd") + " 23:59:59";
                                    if (DateTime.Now < Convert.ToDateTime(exp_time))
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("UPDATE yellowdot.subscribers s SET s.state_id = 2, deactivation_date = now() WHERE s.subscriber_id = " + res1.SubscriberID, ref lines);
                                    }
                                    menu = menu + validate_date + Environment.NewLine + "0) Retour ";
                                    break;
                            }


                        }
                    }

                }
            }
            if (found == false)
            {
                if (service_id == 669 || service_id == 697 || service_id == 698)
                {
                    menu = "Cher abonné, vous n'avez pas souscrit au service Yellow Games" + Environment.NewLine + "0) Retour ";
                }

            }
            return menu;
        }

        public static string UnsubscribeMultiServiceMenu(int service_id, string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            List<ServiceClass> subscriber_services = GetSimilarServices(Convert.ToInt32(service_id), ref lines);
            bool found = false;
            foreach (ServiceClass sc in subscriber_services)
            {
                LoginRequest LoginRequestBody = new LoginRequest()
                {
                    ServiceID = sc.service_id,
                    Password = sc.service_password
                };
                LoginResponse res = Login.DoLogin(LoginRequestBody);
                CheckUserStateRequest CheckUserStateBody = new CheckUserStateRequest()
                {
                    MSISDN = Convert.ToInt64(MSISDN),
                    ServiceID = sc.service_id,
                    TokenID = res.TokenID
                };
                CheckUserStateResponse res1 = CheckUserState.DoCheckUserState(CheckUserStateBody);

                if (res1 != null)
                {
                    if (res1.ResultCode == 1000)
                    {
                        if (res1.State == "Active")
                        {
                            found = true;
                            menu = "Cher client, votre service MTN Yellow Games à ";
                            SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                            {
                                ServiceID = sc.service_id,
                                TokenID = res.TokenID,
                                MSISDN = Convert.ToInt64(MSISDN),
                                TransactionID = "ussd_mo",
                                ActivationID = "3"
                            };
                            SubscribeResponse subscribe_response = CommonFuncations.UnSubscribe.DoUnSubscribe(subscribe_RequestBody);
                            if (subscribe_response != null)
                            {
                                lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");
                            }

                            switch (sc.service_id)
                            {
                                case 669:
                                    menu = menu + "50F/J a été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    break;
                                case 697:
                                    menu = menu + "100F/03J a été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    break;
                                case 698:
                                    menu = menu + "150/07Ja été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    break;
                            }


                        }
                    }

                }
            }
            if (found == false)
            {
                if (service_id == 669 || service_id == 697 || service_id == 698)
                {
                    menu = "Cher abonné, vous n'avez pas souscrit au service Yellow Games" + Environment.NewLine + "0) Retour ";
                }

            }
            return menu;
        }

        public static string UnsubscribeFulfilmentMultiServiceMenu(int service_id, string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            List<ServiceClass> subscriber_services = GetSimilarServices(Convert.ToInt32(service_id), ref lines);
            bool found = false;
            foreach (ServiceClass sc in subscriber_services)
            {
                LoginRequest LoginRequestBody = new LoginRequest()
                {
                    ServiceID = sc.service_id,
                    Password = sc.service_password
                };
                LoginResponse res = Login.DoLogin(LoginRequestBody);
                CheckUserStateRequest CheckUserStateBody = new CheckUserStateRequest()
                {
                    MSISDN = Convert.ToInt64(MSISDN),
                    ServiceID = sc.service_id,
                    TokenID = res.TokenID
                };
                CheckUserStateResponse res1 = CheckUserState.DoCheckUserState(CheckUserStateBody);

                if (res1 != null)
                {
                    if (res1.ResultCode == 1000)
                    {
                        if (res1.State == "Active")
                        {
                            found = true;
                            menu = "Cher client, votre service MTN Yellow Games à ";
                            //SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                            //{
                            //    ServiceID = sc.service_id,
                            //    TokenID = res.TokenID,
                            //    MSISDN = Convert.ToInt64(MSISDN),
                            //    TransactionID = "ussd_mo",
                            //    ActivationID = "3"
                            //};
                            //SubscribeResponse subscribe_response = CommonFuncations.UnSubscribe.DoUnSubscribe(subscribe_RequestBody);

                            string errmsg = "";
                            bool full_res = Fulfillment.CallFulfillment(sc, MSISDN, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false, ref lines, out errmsg);

                            ServiceClass service = GetServiceByServiceID(Convert.ToInt32(res1.ServiceID), ref lines);

                            LoginRequest RequestBody = new LoginRequest()
                            {
                                ServiceID = service.service_id,
                                Password = service.service_password
                            };
                            string token_id = "";
                            LoginResponse response = Login.DoLogin(RequestBody);
                            if (response != null)
                            {
                                if (response.ResultCode == 1000)
                                {
                                    token_id = response.TokenID;
                                }
                            }
                            string sms_text = "Cher abonné, votre souscription au service MTN Yello Games [TYPE] a bien été désactivé. Activer un nouveau en Tapant *709*1#.";

                            Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + res1.SubscriberID, ref lines);

                            //if (subscribe_response != null)
                            //{
                            //    lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");
                            //}

                            switch (sc.service_id)
                            {
                                case 669:
                                    menu = menu + "50F/J a été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    sms_text = sms_text.Replace("[TYPE]", "Jour");
                                    break;
                                case 697:
                                    menu = menu + "100F/03J a été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    sms_text = sms_text.Replace("[TYPE]", "pour 3 jours");
                                    break;
                                case 698:
                                    menu = menu + "150/07Ja été désactivé avec succès";
                                    menu = menu + Environment.NewLine + "0) Retour ";
                                    sms_text = sms_text.Replace("[TYPE]", "pour 7 jours");
                                    break;
                            }

                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = Convert.ToInt64(MSISDN),
                                Text = sms_text,
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
                    }

                }
            }
            if (found == false)
            {
                if (service_id == 669 || service_id == 697 || service_id == 698)
                {
                    menu = "Cher abonné, vous n'avez pas souscrit au service Yellow Games" + Environment.NewLine + "0) Retour ";
                }

            }
            return menu;
        }

        public static string SubscribeMultiServiceMenu(int service_id, int result_code, ref List<LogLines> lines)
        {
            string menu = "Thank you for choosing this service. You will receive a confirmation request within 5 minutes. Please accept to subscribe to the service.";

            switch (service_id)
            {
                case 669:
                case 697:
                case 698:
                    menu = "Cher client, vous avez activé avec succès le service MTN Yellow Game. Vous recevrez un message de confirmation sous peu.";
                    if (result_code == 1020)
                    {
                        menu = "Désolé, vous n'avez pas suffisamment de crédit. Veuillez recharger et réessayer ou composer *185# pour emprunter du crédit.";
                    }
                    if (result_code == 3010)
                    {
                        menu = "Vous avez déjà le service MTN Yellow Game activé.";
                    }
                    if (result_code == 1050)
                    {
                        menu = "Nous sommes désolés, l'abonnement a échoué. Veuillez réessayer plus tard";
                    }
                    break;
                case 973:
                case 974:
                case 975:
                case 976:
                case 977:
                case 978:
                case 979:
                case 980:
                case 999:
                case 1000:
                case 1001:
                case 1002:
                case 1003:
                case 1004:
                case 1005:
                case 1006:
                case 937:
                case 938:
                case 939:
                    menu = "Demande dabonnement prise en compte, vous recevrez un SMS de confirmation sous peu.";
                    if (result_code == 1050) menu = "Echec, veuillez reessayer plus tard";       // an error has occured
                    else if (result_code == 3010) menu = "Déjà abonné à ce service.";                 // already subscribed
                    break;

                case 1184:
                case 1185:
                case 1186:
                case 1187:
                case 1188:
                case 1189:
                    // orange ivory coast - lucky number
                    // 1184 to 1186 = airtime subscription
                    // 1187 to 1189 = airtime once off

                    // -- jackpot is 250k, 500k or 1000k
                    double jackpot = 250 * Math.Pow(2, (service_id - 1184) % 3);

                    // string jackpot_amount = jackpot.ToString("N0", new System.Globalization.CultureInfo("fr-FR"));
                    // something wrong with the Globalisation -- includes strange characters which up set the ussd
                    string jackpot_amount = jackpot.ToString("N0").Replace(",", ".");

                    if (result_code == 1000) menu = $"Bienvenu sur Numero d'OR. Vous etes sur le point de gagner {jackpot_amount}.000FCFA au prochain tirage de numero d'Or.Suivez le lien http://icorgtwn.ydot.co Bonne chance!";
                    else if (result_code == 3010) menu = "Déjà abonné à ce service.";                 // already subscribed
                    else menu = "Echec, veuillez reessayer plus tard.";
                    break;
            }

            lines = Add2Log(lines, $"Subscribe response for serviceID={service_id}, retcode={result_code}, message={menu}", 100, "");
            return menu;
        }

        public static string USSDBehaviuer(ServiceClass service, string ussdString, string ServiceID, string MSISDN, string linkid, string receiveCB, string senderCB, string serviceCode, USSDMenu ussd_menu, USSDSession ussd_session, out DYAReceiveMoneyRequest momo_request, ref List<LogLines> lines, out string menu_2_display, out bool is_close, string force_session_id)
        {
            momo_request = null;
            menu_2_display = "";
            string msgType = "1", opType = "1";
            int status = 0;
            is_close = false;
            string result = "";
            force_session_id = (String.IsNullOrEmpty(force_session_id) ? Guid.NewGuid().ToString() : force_session_id);
            string user_session_id = (ussd_session == null ? force_session_id : ussd_session.user_seesion_id);
            int topic_id = (ussd_session != null ? ussd_session.topic_id : ussd_menu.topic_id);
            if (!String.IsNullOrEmpty(ussd_menu.menu_2_display))
            {
                menu_2_display = ussd_menu.menu_2_display;
                result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu.menu_2_display, "1", "1");
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);
            }
            if (ussd_menu.action_id == 3) //close
            {
                menu_2_display = ussd_menu.menu_2_display;
                is_close = true;
                result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, "2", "2");
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);
            }
            if (ussd_menu.action_id == 59) //CheckSubscriberStatus
            {
                menu_2_display = CheckStatusMenu(service.service_id, MSISDN, ref lines);
                result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, "1", "1");
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);

            }
            if (ussd_menu.action_id == 60) //DeactivateMultiService
            {
                menu_2_display = UnsubscribeMultiServiceMenu(service.service_id, MSISDN, ref lines);
                result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, "1", "1");
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);

            }
            if (ussd_menu.action_id == 62) //DeactivateMultiServiceFull
            {
                menu_2_display = UnsubscribeFulfilmentMultiServiceMenu(service.service_id, MSISDN, ref lines);
                result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, "1", "1");
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);

            }

            if (ussd_menu.action_id == 63) //ActivateMultiService
            {
                menu_2_display = CheckStatusMenu(service.service_id, MSISDN, ref lines);
                if (menu_2_display.Contains("Cher abonné, vous n'avez pas souscrit au service Yellow Games"))
                {
                    menu_2_display = "Veuillez choisir une option :" + Environment.NewLine + Environment.NewLine;
                    menu_2_display = menu_2_display + "1) Activation/50F/1J" + Environment.NewLine;
                    menu_2_display = menu_2_display + "2) Activation/100F/03J" + Environment.NewLine;
                    menu_2_display = menu_2_display + "3) Activation/150F/07J" + Environment.NewLine;
                    menu_2_display = menu_2_display + "0) Retour" + Environment.NewLine;
                }
                else
                {
                    result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, "1", "1");
                }
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);
            }


            if (ussd_menu.action_id == 61) //fulfilment
            {
                ServiceClass subscriber_service = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                string errmsg = "";
                bool full_res = Fulfillment.CallFulfillment(subscriber_service, MSISDN, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref lines, out errmsg);
                if (full_res)
                {
                    int price_id = 0;
                    switch (ussd_menu.service_id)
                    {
                        case 669:
                            price_id = 1853;
                            break;
                        case 697:
                            price_id = 1852;
                            break;
                        case 698:
                            price_id = 1855;
                            break;
                    }
                    Int64 sub_id = Api.DataLayer.DBQueries.InsertSub(MSISDN, subscriber_service.service_id.ToString(), price_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "3", "", ref lines);
                    if (sub_id > 0)
                    {
                        menu_2_display = SubscribeMultiServiceMenu(subscriber_service.service_id, 1010, ref lines);
                        ServiceBehavior.DecideBehavior(subscriber_service, "1", MSISDN, sub_id, ref lines);
                    }
                    else
                    {
                        menu_2_display = SubscribeMultiServiceMenu(subscriber_service.service_id, 1050, ref lines);
                        full_res = Fulfillment.CallFulfillment(subscriber_service, MSISDN, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false, ref lines, out errmsg);
                    }


                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);

                }
                else
                {
                    is_close = true;
                    menu_2_display = errmsg;
                }
            }

            if (ussd_menu.action_id == 91) //MOMOAndSubscribe
            {

                int amount = 0;
                switch (ussd_menu.service_id)
                {
                    case 669:
                        amount = 50;
                        break;
                    case 697:
                        amount = 100;
                        break;
                    case 698:
                        amount = 150;
                        break;
                }
                ServiceClass momo_service = GetServiceByServiceID(669, ref lines);
                LoginRequest LoginRequestBody = new LoginRequest()
                {
                    ServiceID = momo_service.service_id,
                    Password = momo_service.service_password
                };
                LoginResponse res = Login.DoLogin(LoginRequestBody);
                if (res != null)
                {
                    if (res.ResultCode == 1000)
                    {
                        string token_id = res.TokenID;
                        momo_request = new DYAReceiveMoneyRequest()
                        {
                            MSISDN = Convert.ToInt64(MSISDN),
                            Amount = Convert.ToInt32(amount),
                            ServiceID = momo_service.service_id,
                            TokenID = token_id,
                            TransactionID = user_session_id
                        };
                    }
                }
                menu_2_display = "Veuillez approuver le paiement avec votre code PIN ";
                msgType = "2";
                opType = "2";
                status = 1;
                is_close = true;
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);
            }

            if (ussd_menu.action_id == 4 || ussd_menu.action_id == 5) //subscribe or unsubscribe
            {
                ServiceClass subscriber_service = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                LoginRequest LoginRequestBody = new LoginRequest()
                {
                    ServiceID = subscriber_service.service_id,
                    Password = subscriber_service.service_password
                };
                LoginResponse res = Login.DoLogin(LoginRequestBody);

                menu_2_display = "Thank You";

                is_close = true;
                //result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, "Thank You", "2", "2");
                //lines = Add2Log(lines, "Soap = " + result, 100, "ussd_mo");
                //string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                //lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo");
                //string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, result, ref lines);
                //lines = Add2Log(lines, "SendUSSD Response = " + response, 100, "ussd_mo");
                result = "";
                if (res != null)
                {
                    if (res.ResultCode == 1000)
                    {
                        string token_id = res.TokenID;
                        SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                        {
                            ServiceID = subscriber_service.service_id,
                            TokenID = token_id,
                            MSISDN = Convert.ToInt64(MSISDN),
                            TransactionID = "ussd_mo",
                            ActivationID = "3"
                        };
                        SubscribeResponse subscribe_response = (ussd_menu.action_id == 4 ? CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody) : CommonFuncations.UnSubscribe.DoUnSubscribe(subscribe_RequestBody));
                        if (subscribe_response != null)
                        {
                            lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");
                            if (ussd_menu.action_id == 4)
                            {
                                menu_2_display = SubscribeMultiServiceMenu(subscriber_service.service_id, subscribe_response.ResultCode, ref lines);
                            }
                        }
                    }
                }
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", "DBConnectionString_104", ref lines);
            }


            if (String.IsNullOrEmpty(result))
            {
                switch (topic_id)
                {
                    case 5:
                    case 2:
                    case 7:
                    case 14:
                        int page_number = (ussd_session != null ? ussd_session.page_number : 1);
                        int odd_page = (ussd_session != null ? ussd_session.odd_page : 0);
                        Int64 game_id = (ussd_session != null ? ussd_session.game_id : 0);
                        double selected_odd = (ussd_session != null ? ussd_session.selected_odd : 0);
                        int selected_bet_type_id = (ussd_session != null ? ussd_session.selected_bet_type_id : 0);
                        string selected_odd_name = (ussd_session != null ? ussd_session.selected_odd_name : "0");
                        string selected_odd_line = (ussd_session != null ? ussd_session.selected_odd_line : "null");
                        string amount = (ussd_session != null ? ussd_session.amount : "0");
                        int selected_league_id = (ussd_session != null ? ussd_session.selected_league_id : 0);


                        string amount_2_pay = (ussd_session != null ? ussd_session.amount_2_pay.ToString() : "0");
                        string bar_code = (ussd_session != null ? ussd_session.bar_code : "0");
                        string selected_subagent_name = (ussd_session != null ? ussd_session.selected_subagent_name : "0");
                        IdoBetUser user = new IdoBetUser();
                        switch (ussd_menu.action_id)
                        {

                            case 55://EnterAmountForSubAgent
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                status = 1;
                                Int64 pos_trans_id = 0;
                                menu_2_display = GetAmountForSubAgentMenu(MSISDN, ussdString, selected_subagent_name, out pos_trans_id, ref lines);
                                if (pos_trans_id > 0)
                                {
                                    ServiceClass subscriber_service1 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                                    LoginRequest LoginRequestBody = new LoginRequest()
                                    {
                                        ServiceID = subscriber_service1.service_id,
                                        Password = subscriber_service1.service_password
                                    };
                                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                                    if (res != null)
                                    {
                                        if (res.ResultCode == 1000)
                                        {
                                            string token_id = res.TokenID;
                                            momo_request = new DYAReceiveMoneyRequest()
                                            {
                                                MSISDN = Convert.ToInt64(MSISDN),
                                                Amount = Convert.ToInt32(ussdString),
                                                ServiceID = ussd_menu.service_id,
                                                TokenID = token_id,
                                                TransactionID = "POSTrans_" + pos_trans_id

                                            };
                                        }
                                    }
                                }



                                break;
                            case 54:
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#"))
                                {
                                    page_number = page_number + 1;
                                }
                                if (ussdString.Contains("*") && page_number > 1)
                                {
                                    page_number = page_number - 1;
                                }
                                menu_2_display = GetSlectedSubAgentMenu(MSISDN, page_number, ussdString, out selected_subagent_name, ref lines);
                                break;
                            case 53:
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#"))
                                {
                                    page_number = page_number + 1;
                                }
                                if (ussdString.Contains("*") && page_number > 1)
                                {
                                    page_number = page_number - 1;
                                }
                                menu_2_display = GetAgentMainMenu(MSISDN, page_number, ref lines);
                                break;
                            case 34: //DepositMoney
                                user = SearchUserNew(MSISDN, ref lines);
                                menu_2_display = GetDepositMoneyMenu(user, ussd_menu, MSISDN, ref lines);
                                break;
                            case 36: //StartDepositMoney
                                user = SearchUserNew(MSISDN, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                menu_2_display = GetStartDepositMoneyMenu(service, user, ussd_menu, MSISDN, ussdString, out momo_request, ref lines);
                                break;
                            case 33: //WithdrawMoney
                                user = SearchUserNew(MSISDN, ref lines);
                                menu_2_display = GetWithdrawMoneyMenu(user, ref lines);
                                break;
                            case 35: //StartWithdrawMoney
                                user = SearchUserNew(MSISDN, ref lines);
                                menu_2_display = GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                if (user.isValid == true)
                                {
                                    if (Convert.ToInt32(ussdString) <= user.AvailableForWithdraw && Convert.ToInt32(ussdString) > 0)
                                    {
                                        menu_2_display = CheckBeforeWithdrawMenu(service, MSISDN, ref lines);
                                        if (String.IsNullOrEmpty(menu_2_display))
                                        {
                                            //Refund user
                                            ServiceClass subscriber_service1 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
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
                                                    DYATransferMoneyRequest momotransfer_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = Convert.ToInt64(MSISDN),
                                                        Amount = Convert.ToInt32(ussdString),
                                                        ServiceID = service.service_id,
                                                        TokenID = token_id,
                                                        TransactionID = "USSDWithdraw_" + Guid.NewGuid().ToString()
                                                    };
                                                    string postBody = "", response_body = "";
                                                    //bool wd_result = StartWithdraw(user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                    bool do_withdraw = false;
                                                    string code = "";
                                                    if (user.AvailableForWithdraw > 0)
                                                    {
                                                        code = StartWithdrawNew(user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                    }



                                                    menu_2_display = GetStartWithdrawMoneyMenu(user, true, true, false, ref lines);

                                                    if (code == "")
                                                    {
                                                        if (do_withdraw)
                                                        {
                                                            string mail_body = "<p><h2>Start Withdraw has failed</h2><b>UserID:</b> " + user.id + "<br><b>Name:</b> " + user.firstName + " " + user.lastName + "<br><b>Amount: </b>" + ussdString + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>Json: </b>" + postBody + "<br><b>Response: </b>" + response_body + "<br></p>";
                                                            string mail_subject = "Start Withdraw has failed for user - " + user.id;
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
                                                        DYATransferMoneyResponse momotransfer_response = CommonFuncations.DYATransferMoney.DoTransfer(momotransfer_request);
                                                        if (momotransfer_response.ResultCode == 1000)
                                                        {
                                                            postBody = "";
                                                            response_body = "";
                                                            bool end_withdraw = EndWithdrawNew(momotransfer_response.Timestamp, momotransfer_response.TransactionID, true, code, user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                            if (!end_withdraw)
                                                            {
                                                                string mail_body = "", mail_subject = "";
                                                                mail_body = "<p><h2>End Withdraw has failed but DYATransfer was ok</h2><b>UserID:</b> " + user.id + "<br><b>Name:</b> " + user.firstName + " " + user.lastName + "<br><b>Amount: </b>" + ussdString + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>Request: </b>" + postBody + "<br>Response: " + response_body + "<br></p>";
                                                                mail_subject = "End Withdraw has failed but DYATransfer was ok - " + user.id;
                                                                string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                                            }
                                                            GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "Withdraw", "MOMO", "-" + ussdString, "/", ref lines);
                                                        }
                                                        else
                                                        {

                                                            postBody = "";
                                                            response_body = "";
                                                            bool end_withdraw = EndWithdrawNew(momotransfer_response.Timestamp, momotransfer_response.TransactionID, false, code, user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                            string mail_body = "", mail_subject = "";
                                                            mail_body = "<p><h2>Withdraw has failed DYATransfer</h2><b>UserID:</b> " + user.id + "<br><b>Name:</b> " + user.firstName + " " + user.lastName + "<br><b>Amount: </b>" + ussdString + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>MOMO Response: </b>" + momotransfer_response.ResultCode + " " + momotransfer_response.Description + "<br>User was refunded: " + end_withdraw + "<br><b>Request: </b>" + postBody + "<br><b>Response: </b>" + response_body + "</p>";
                                                            mail_subject = "Withdraw has failed DYATransfer for user - " + user.id;
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
                                            }
                                        }


                                    }
                                    else
                                    {
                                        //user has requested more than allowed
                                        menu_2_display = GetStartWithdrawMoneyMenu(user, false, false, true, ref lines);
                                    }
                                }
                                break;
                            case 25://CloseXXXBet
                            case 26:
                            case 27:
                                List<Int64> saved_ids = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(user_session_id, ref lines);
                                if (saved_ids != null)
                                {
                                    foreach (Int64 s in saved_ids)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set amount = " + amount + " where id = " + s, ref lines);
                                    }
                                }
                                //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set amount = " + amount + " where user_session_id = '" + user_session_id + "'", ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                status = 1;
                                //RequestForOrder
                                //bool req_for_order = GetRequestForOrder(ussd_session, ref lines);
                                bool req_for_order = PlaceBet(ussd_session, ref lines);

                                if (req_for_order == true)
                                {
                                    menu_2_display = GetCloseBet(ussd_session, amount, ref lines);
                                    ServiceClass subscriber_service2 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                                    LoginRequest LoginRequestBody = new LoginRequest()
                                    {
                                        ServiceID = subscriber_service2.service_id,
                                        Password = subscriber_service2.service_password
                                    };
                                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                                    if (res != null)
                                    {
                                        if (res.ResultCode == 1000)
                                        {
                                            string token_id = res.TokenID;
                                            momo_request = new DYAReceiveMoneyRequest()
                                            {
                                                MSISDN = Convert.ToInt64(MSISDN),
                                                Amount = Convert.ToInt32(amount),
                                                ServiceID = ussd_menu.service_id,
                                                TokenID = token_id,
                                                TransactionID = user_session_id

                                            };
                                        }
                                    }
                                }
                                else
                                {
                                    menu_2_display = GetCloseBetFailed(ref lines);
                                }
                                break;
                            case 22: //ConfirmBetXXX
                            case 23:
                            case 24:
                                amount = ussdString;
                                double num1;
                                bool res_parse = double.TryParse(amount, out num1);
                                if (res_parse == true)
                                {
                                    if (num1 >= 200 && num1 <= 1000000)
                                    {
                                        menu_2_display = GetConfirmBet(ussd_session, amount, ref lines);
                                    }
                                    else
                                    {

                                        menu_2_display = GetWrongPriceBetMeny(ref lines);
                                        DataLayer.DBQueries.ExecuteQuery("delete from ussd_saved_games where user_session_id = '" + user_session_id + "' order by id desc limit 1", ref lines);
                                    }
                                }
                                odd_page = 0;

                                break;
                            case 19: //PayAndConfirmXXX
                            case 20:
                            case 21:
                                //need to check if user clicks back and then returns to this section.
                                DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0)", ref lines);
                                menu_2_display = GetPayAndConfirm(ussd_session, ref lines);
                                odd_page = 0;
                                break;
                            case 32: //ticket status
                                page_number = (page_number == 0 ? 1 : page_number);
                                menu_2_display = iDoBet.GetCheckTicketMenu(MSISDN, page_number, ussdString, ref lines);
                                break;
                            case 31: //tickets status by phone
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#"))
                                {
                                    page_number = page_number + 1;
                                }
                                if (ussdString.Contains("*") && page_number > 1)
                                {
                                    page_number = page_number - 1;
                                }
                                menu_2_display = iDoBet.GetCheckTicketsMenu(MSISDN, page_number, ref lines);
                                break;
                            case 52:
                                menu_2_display = GetPayoutBarcodeTicket(ussd_menu, MSISDN, ref lines);
                                if (String.IsNullOrEmpty(menu_2_display))
                                {
                                    msgType = "2";
                                    opType = "2";
                                    is_close = true;
                                    //Refund user
                                    ServiceClass subscriber_service2 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                                    LoginRequest LoginRequestBody2 = new LoginRequest()
                                    {
                                        ServiceID = service.service_id,
                                        Password = service.service_password
                                    };
                                    LoginResponse res2 = Login.DoLogin(LoginRequestBody2);
                                    if (res2 != null)
                                    {
                                        if (res2.ResultCode == 1000 && Convert.ToInt32(amount_2_pay) > 0)
                                        {
                                            string token_id = res2.TokenID;
                                            DYATransferMoneyRequest momotransfer_request = new DYATransferMoneyRequest()
                                            {
                                                MSISDN = Convert.ToInt64(MSISDN),
                                                Amount = Convert.ToInt32(amount_2_pay),
                                                ServiceID = service.service_id,
                                                TokenID = token_id,
                                                TransactionID = "POSTicketPayout_" + bar_code
                                            };
                                            DYATransferMoneyResponse momotransfer_response = CommonFuncations.DYATransferMoney.DoTransfer(momotransfer_request);

                                            if (momotransfer_response.ResultCode == 1000)
                                            {
                                                int cashier_id = GetCashierID(MSISDN, ref lines);
                                                if (cashier_id > 0)
                                                {
                                                    DataLayer.DBQueries.ExecuteQuery("insert into casheir_transactions (cashier_id, bar_code, total_payout, date_time, dya_id) values(" + cashier_id + ", '" + bar_code + "', " + amount_2_pay + ", now()," + momotransfer_response.TransactionID + ");", ref lines);
                                                }
                                                GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "PyoutPOSTicket", "MOMO", "-" + ussdString, "/", ref lines);
                                                //dopayout
                                                bool dopayout_res = iDoBet.DoPayOutNew(momotransfer_response.Timestamp, momotransfer_response.TransactionID, bar_code, ref lines);
                                                //bool dopayout_res = iDoBet.DoPayout(bar_code, ref lines);
                                                menu_2_display = "Félicitations, le billet a été remboursé sur votre compte MOMO.";

                                            }
                                            else
                                            {
                                                menu_2_display = "Le billet n'a pas été remboursé sur votre compte MOMO." + Environment.NewLine;
                                                menu_2_display = menu_2_display + "Veuillez réessayer";
                                                string mail_body = "<p><h2>Payout has failed DYATransfer</h2><b>barCode:</b> " + bar_code + "<br><b>Amount: </b>" + amount_2_pay + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>MOMO Response: </b>" + momotransfer_response.ResultCode + " " + momotransfer_response.Description + "<br></p>";
                                                string mail_subject = "POS Ticlet Payout has failed DYATransfer for user - " + bar_code;
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
                                            menu_2_display = "option invalide" + Environment.NewLine;
                                            menu_2_display = menu_2_display + "Veuillez réessayer";
                                        }
                                    }
                                }

                                break;
                            case 44: //search ticket
                                menu_2_display = iDoBet.GetCheckTicketByBarcodeMenu(MSISDN, ussdString, out amount_2_pay, ref lines);
                                bar_code = ussdString;
                                break;
                            case 39: //add another game
                                if (game_id > 0)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0)", ref lines);
                                    game_id = 0;
                                    selected_odd = 0;
                                    page_number = 1;
                                    odd_page = 0;
                                }
                                menu_2_display = GetSportsTypeMenu(ref lines);
                                break;
                            case 41: //Delete and Start Over
                                List<Int64> saved_ids1 = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(user_session_id, ref lines);
                                if (saved_ids1 != null)
                                {
                                    foreach (Int64 s in saved_ids1)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set status = 3 where id = " + s, ref lines);
                                    }
                                }

                                //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set status = 3 where user_seesion_id = '" + user_session_id + "'", ref lines);
                                game_id = 0;
                                odd_page = 0;
                                selected_odd = 0;
                                page_number = 1;
                                menu_2_display = GetSportsTypeMenu(ref lines);
                                ussd_menu.action_id = 2;
                                break;
                            case 48:
                                menu_2_display = GetSoccerLeagueMenu(35, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 47:
                                menu_2_display = GetSoccerLeagueMenu(32, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 37: //display soccer leagu
                                menu_2_display = GetSoccerLeagueMenu(31, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 49:
                            case 50:
                            case 38://selected league
                            case 28://Start Over
                            case 29:
                            case 30:
                            case 16://AddXXXGame or 
                            case 17:
                            case 18:
                            case 7: //DisplayXXXTopEvents   
                            case 8:
                            case 9:
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#"))
                                {
                                    page_number = page_number + 1;
                                }
                                if (ussdString.Contains("*") && page_number > 0)
                                {
                                    page_number = page_number - 1;
                                }
                                if (ussd_menu.action_id == 28 || ussd_menu.action_id == 29 || ussd_menu.action_id == 30)
                                {
                                    List<Int64> saved_ids2 = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(user_session_id, ref lines);
                                    if (saved_ids2 != null)
                                    {
                                        foreach (Int64 s in saved_ids2)
                                        {
                                            DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set status = 3 where id = " + s, ref lines);
                                        }
                                    }
                                    //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set status = 3 where user_seesion_id = '"+user_session_id+"'", ref lines);
                                    game_id = 0;
                                    selected_odd = 0;
                                    odd_page = 0;
                                    page_number = 1;

                                }
                                if (ussd_menu.action_id == 16 || ussd_menu.action_id == 17 || ussd_menu.action_id == 18)
                                {
                                    if (game_id > 0)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0)", ref lines);
                                        game_id = 0;
                                        selected_odd = 0;
                                        page_number = 1;
                                        odd_page = 0;
                                    }

                                }
                                switch (ussd_menu.action_id)
                                {
                                    case 38:
                                        menu_2_display = iDoBet.GetEventsMenu(ussdString, 31, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu);
                                        break;
                                    case 49:
                                        menu_2_display = iDoBet.GetEventsMenu(ussdString, 32, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu);
                                        break;
                                    case 50:
                                        menu_2_display = iDoBet.GetEventsMenu(ussdString, 35, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu);
                                        break;
                                    case 28:
                                    case 16:
                                    case 7:
                                        menu_2_display = iDoBet.GetEventsMenu(31, page_number, ref lines);
                                        break;
                                    case 29:
                                    case 17:
                                    case 8:
                                        menu_2_display = iDoBet.GetEventsMenu(32, page_number, ref lines);
                                        break;
                                    case 30:
                                    case 18:
                                    case 9:
                                        menu_2_display = iDoBet.GetEventsMenu(35, page_number, ref lines);
                                        break;
                                }


                                break;
                            case 10: //DisplayXXXGame
                            case 11:
                            case 12:
                                page_number = (page_number == 0 ? 1 : page_number);
                                selected_odd = 0;
                                if (ussdString.Contains("#"))
                                {
                                    odd_page = odd_page + 1;
                                }
                                if (ussdString.Contains("*") && odd_page > 0)
                                {
                                    odd_page = odd_page - 1;
                                }
                                switch (ussd_menu.action_id)
                                {
                                    case 10:
                                        menu_2_display = iDoBet.GetGameOddsMenu(31, page_number, ussdString, game_id, odd_page, out game_id, selected_league_id, ref lines);
                                        break;
                                    case 11:
                                        menu_2_display = iDoBet.GetGameOddsMenu(32, page_number, ussdString, game_id, odd_page, out game_id, selected_league_id, ref lines);
                                        break;
                                    case 12:
                                        menu_2_display = iDoBet.GetGameOddsMenu(35, page_number, ussdString, game_id, odd_page, out game_id, selected_league_id, ref lines);
                                        break;
                                }
                                break;
                            case 13: //ConfirmXXXGame (select odd)
                            case 14:
                            case 15:
                                switch (ussd_menu.action_id)
                                {
                                    case 13:
                                        menu_2_display = iDoBet.GetConfirmMenu(31, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 14:
                                        menu_2_display = iDoBet.GetConfirmMenu(32, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 15:
                                        menu_2_display = iDoBet.GetConfirmMenu(35, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                }
                                break;
                        }
                        lines = Add2Log(lines, " page_number = " + page_number, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " game_id = " + game_id, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " odd_page = " + odd_page, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_odd = " + selected_odd, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " amount = " + amount, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_league_id = " + selected_league_id, 100, "ivr_subscribe");
                        result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, msgType, opType);
                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id, selected_odd, selected_bet_type_id,selected_odd_name, selected_odd_line, amount, selected_league_id, amount_2_pay, bar_code, selected_subagent_name) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + "," + status + ", '" + ussdString + "', " + ussd_menu.action_id + ", " + page_number + "," + odd_page + "," + game_id + "," + topic_id + ",'" + user_session_id + "'," + selected_odd + "," + selected_bet_type_id + ",'" + selected_odd_name + "','" + selected_odd_line + "', " + amount + "," + selected_league_id + "," + amount_2_pay + ",'" + bar_code + "','" + selected_subagent_name + "');", "DBConnectionString_104", ref lines);
                        break;
                }
            }

            return result;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ussd_mo");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_mo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_mo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_mo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_mo");
            string MSISDN = "", spID = "", ServiceID = "", linkid = "", traceUniqueID = "", msgType = "", senderCB = "", ussdString = "", cu_id = "", receiveCB = "", abort_soap = "";
            string ussd_soap = "";
            cu_id = context.Request.QueryString["cu_id"];
            ServiceClass service = new ServiceClass();
            DYAReceiveMoneyRequest momo_request = null;
            if (!String.IsNullOrEmpty(xml))
            {
                linkid = ProcessXML.GetXMLNode(xml, "ns1:linkid", ref lines);
                lines = Add2Log(lines, " linkid = " + linkid, 100, "ussd_mo");

                receiveCB = ProcessXML.GetXMLNode(xml, "ns2:receiveCB", ref lines);
                lines = Add2Log(lines, " receiveCB = " + receiveCB, 100, "ussd_mo");



                traceUniqueID = ProcessXML.GetXMLNode(xml, "ns1:traceUniqueID", ref lines);
                lines = Add2Log(lines, " traceUniqueID = " + traceUniqueID, 100, "ussd_mo");

                msgType = ProcessXML.GetXMLNode(xml, "ns2:msgType", ref lines);
                lines = Add2Log(lines, " msgType = " + msgType, 100, "ussd_mo");

                senderCB = ProcessXML.GetXMLNode(xml, "ns2:senderCB", ref lines);
                lines = Add2Log(lines, " senderCB = " + senderCB, 100, "ussd_mo");

                ussdString = ProcessXML.GetXMLNode(xml, "ns2:ussdString", ref lines);
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo");

                string serviceCode = ProcessXML.GetXMLNode(xml, "ns2:serviceCode", ref lines);
                lines = Add2Log(lines, " serviceCode = " + serviceCode, 100, "ussd_mo");

                MSISDN = ProcessXML.GetXMLNode(xml, "ns2:msIsdn", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo");

                spID = (ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:spId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines));
                lines = Add2Log(lines, " spID = " + spID, 100, "sms_dlr");

                ServiceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines));
                lines = Add2Log(lines, " ServiceID = " + ServiceID, 100, "sms_dlr");

                string time_stamp = (ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:timeStamp", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines));
                lines = Add2Log(lines, " time_stamp = " + time_stamp, 100, "sms_dlr");
                if (!String.IsNullOrEmpty(time_stamp))
                {
                    var date = DateTime.ParseExact(time_stamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    time_stamp = date.ToString("yyyy-MM-dd HH:mm:ss");
                }

                string abortReason = ProcessXML.GetXMLNode(xml, "ns2:abortReason", ref lines);
                lines = Add2Log(lines, " abortReason = " + abortReason, 100, "ussd_mo");

                service = GetServiceInfo(spID, ServiceID, "", ref lines);


                if (!String.IsNullOrEmpty(ussdString) && !String.IsNullOrEmpty(cu_id) && !String.IsNullOrEmpty(MSISDN))
                {
                    Int64 service_id = DataLayer.DBQueries.SelectQueryReturnInt64("select c.service_id from ussd_push.campaign_users cu, ussd_push.campaigns c where c.campaign_id = cu.campaign_id and cu.cu_id = " + cu_id, ref lines);
                    ServiceClass subscriber_service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
                    if (subscriber_service != null)
                    {
                        LoginRequest LoginRequestBody = new LoginRequest()
                        {
                            ServiceID = subscriber_service.service_id,
                            Password = subscriber_service.service_password
                        };
                        LoginResponse res = Login.DoLogin(LoginRequestBody);
                        if (res != null)
                        {
                            if (res.ResultCode == 1000)
                            {
                                string token_id = res.TokenID;
                                SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
                                {
                                    ServiceID = subscriber_service.service_id,
                                    TokenID = token_id,
                                    MSISDN = Convert.ToInt64(MSISDN),
                                    TransactionID = "ussd_mo",
                                    ActivationID = "3"
                                };
                                SubscribeResponse subscribe_response = CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
                                if (subscribe_response != null)
                                {
                                    lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ussd_mo");
                                    DataLayer.DBQueries.ExecuteQuery("update ussd_push.campaign_users set subscriber_date_time = now(), subscriber_ret_code = " + subscribe_response.ResultCode + " where cu_id = " + cu_id, ref lines);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (service != null && abortReason == "" && ussdString != "")
                    {
                        USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                        if (umc != null)
                        {
                            lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                            USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                            int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                            USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                            if (ussd_menu != null)
                            {
                                lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                                string menu_2_display = "";
                                bool is_close = false;
                                ussd_soap = USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            }



                            //int page_number = 0, odd_page = 0;
                            //Int64 game_id = 0;
                            //int last_menu_id = DataLayer.DBQueries.GetLastUSSDMenu(MSISDN, umc.ussd_id, ref lines, out page_number, out odd_page, out game_id);
                            //lines = Add2Log(lines, " Last menu id = " + last_menu_id, 100, "ivr_subscribe");

                        }




                    }
                }


            }


            string response_soap = "";
            response_soap = response_soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/ussd/notification/v1_0/local\">";
            response_soap = response_soap + "<soapenv:Header/>";
            response_soap = response_soap + "<soapenv:Body>";
            response_soap = response_soap + "<loc:notifyUssdReceptionResponse>";
            response_soap = response_soap + "<loc:result>0</loc:result>";
            response_soap = response_soap + "</loc:notifyUssdReceptionResponse>";
            response_soap = response_soap + "</soapenv:Body>";
            response_soap = response_soap + "</soapenv:Envelope>";
            lines = Add2Log(lines, " Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);

            if (ussd_soap != "")
            {
                lines = Add2Log(lines, "Soap = " + ussd_soap, 100, "ussd_mo");
                string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo");
                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, ussd_soap, ref lines);
                lines = Add2Log(lines, "SendUSSD Response = " + response, 100, "ussd_mo");
                if (momo_request != null)
                {
                    DYAReceiveMoneyResponse momo_response = CommonFuncations.DYAReceiveMoney.DoReceive(momo_request);
                    if (momo_response != null)
                    {
                        lines = Add2Log(lines, "MOMO Response = " + momo_response.ResultCode + ", " + momo_response.Description, 100, "ussd_mo");
                    }
                }
            }
            if (abort_soap != "")
            {
                lines = Add2Log(lines, "Abort Soap = " + abort_soap, 100, "ussd_mo");
                string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo");
                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, ussd_soap, ref lines);
                lines = Add2Log(lines, "SendAbortUSSD Response = " + response, 100, "ussd_mo");
            }
            lines = Write2Log(lines);
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