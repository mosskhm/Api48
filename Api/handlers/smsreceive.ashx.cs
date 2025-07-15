using Api.CommonFuncations;
using Api.HttpItems;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for smsreceive
    /// </summary>
    public class smsreceive : IHttpHandler
    {
        public class returned_service
        {
            public int service_id { get; set; }
            public string returned_sms_text { get; set; }
            public string token_id { get; set; }
        }
        public returned_service DecidePerTextAndShortCode(string text, string short_code, string msisdn, string mobile_operator)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "smsreceive_" + short_code);

            returned_service result = null;
            string returned_sms_text = "";

            lines = Add2Log(lines, "MSISDN = " + msisdn, 100, "");
            lines = Add2Log(lines, "operator = " + mobile_operator, 100, "");
            lines = Add2Log(lines, "Text = " + text, 100, "");
            lines = Add2Log(lines, "ShortCode = " + short_code, 100, "");

            try
            {


                if (String.IsNullOrEmpty(text))
                {
                    ServiceClass service = null;
                    switch (short_code)
                    {
                        case "55222":
                            service = GetServiceByServiceID(389, ref lines);
                            returned_sms_text = "You have sent an invalid keyword. Please text HELP to 55222.";
                            break;
                        case "55223":
                            service = GetServiceByServiceID(637, ref lines);
                            returned_sms_text = "You have sent an invalid keyword. To Subscribe to Health Care Alerts service for only N50 BiWeekly send MENU to 55223";
                            break;
                        case "55227":
                            service = GetServiceByServiceID(364, ref lines);
                            returned_sms_text = "You have sent an invalid keyword. Please text HELP to 55227.";
                            break;
                        case "7019":
                            service = GetServiceByServiceID(669, ref lines);
                            returned_sms_text = "Vous avez envoye' un mot cle' non valide. S'il vous plaît texte HELP a' 7019.";
                            break;
                        case "8408":
                            service = null;
                            break;
                        case "31167":
                        case "27840001666":
                            service = GetServiceByServiceID(1010, ref lines);
                            returned_sms_text = "You have sent an invalid keyword. Please text HELP to 27840001666";
                            break;
                        case "7717":
                            service = GetServiceByServiceID(1184, ref lines);
                            returned_sms_text = "Pour souscrire a numéro d'Or, envoyez NO1, NO2 ou NO3 au 7717 ou composez le *590*5#. Pour annuler, envoyez STOP NO(1; 2 ou 3) au 7717.";
                            break;
                        default:
                            throw new Exception($"receive a blank message to unknown shortcode {short_code}");
                    }

                    if (service != null)
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
                                result = new returned_service()
                                {
                                    service_id = service.service_id,
                                    token_id = token_id,
                                    returned_sms_text = returned_sms_text
                                };
                            }
                        }
                    }
                }


                if (text.ToLower().Contains("help") || text.ToLower().Contains("menu"))
                {
                    ServiceClass service = null;
                    switch (short_code)
                    {
                        case "55222":
                            service = GetServiceByServiceID(389, ref lines);
                            returned_sms_text = "To subscribe for Liverpool English, text LV TO 55222,For Worldcup Update text WU TO 55222,for Local football News text LFN to 55222,for Manchester City, text MC to 55222,for Liverpool, text LP to 55222,for Chelsea Hausa, text CHH to 55222,for Arsenal Hausa, text ASH to 55222, for Manchester United Hausa, text MUH to 55222, for Manchester city Hausa, text MCH to 55222.  Service cost N100/7days. Text STOP KEYWORD to 55222 to unsubscribe.";
                            break;
                        case "55223":
                            service = GetServiceByServiceID(637, ref lines);
                            returned_sms_text = "Dial 55223, Choose health concern - Subscribe Service, Press\n 1.Listen Health Tips\n 2.Talk to Doctor\n 3.Drop Health Question\n 4.Search Hospitals";
                            break;
                        case "55227":
                            service = GetServiceByServiceID(364, ref lines);
                            returned_sms_text = "To subscribe For Billy Graham, text BG TO 55227,For God TV, text GTV TO 55227,for  Evening Prayer, text EP to 55227,for Daily grace, text DG to 55227,for TD Jakes, text TD to 55227,for Joyce Meyer, text JM to 55227. Service cost N50/3days. Text STOP KEYWORD to 55227 to unsubscribe.";
                            break;
                        case "7019":
                            service = null;
                            break;
                        case "31167":
                        case "27840001666":
                            service = GetServiceByServiceID(1010, ref lines);
                            returned_sms_text = "To subscribe to BigCash, text BC TO 27840001666 Text STOP KEYWORD to 55227 to 27840001666.";
                            break;
                        case "7717":
                            service = GetServiceByServiceID(1184, ref lines);
                            returned_sms_text = "Pour souscrire a numéro d'Or, envoyez NO1, NO2 ou NO3 au 7717 ou composez le *590*5#. Pour annuler, envoyez STOP NO(1; 2 ou 3) au 7717.";
                            break;
                        default:
                            throw new Exception($"received {text} to unknown shortcode {short_code}");
                         
                    }

                    if (service != null)
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
                                result = new returned_service()
                                {
                                    service_id = service.service_id,
                                    token_id = token_id,
                                    returned_sms_text = returned_sms_text
                                };
                            }
                        }
                    }
                }



                if (result == null && (short_code == "+27840001666" || short_code == "31167")) //user has sent a keyword
                {
                    if (text.ToLower() == "bc" || text.ToLower() == "stop bc" || text.ToLower() == "bc stop")
                    {
                        ServiceClass service = GetServiceByServiceID(1010, ref lines);
                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                    }
                    if (text.ToLower() == "bc1" || text.ToLower() == "stop bc1" || text.ToLower() == "bc1 stop")
                    {
                        ServiceClass service = GetServiceByServiceID(1030, ref lines);
                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                    }
                    if (text.ToLower() == "bc2" || text.ToLower() == "stop bc2" || text.ToLower() == "bc2 stop")
                    {
                        ServiceClass service = GetServiceByServiceID(1031, ref lines);
                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                    }
                    if (text.ToLower() == "bc3" || text.ToLower() == "stop bc3" || text.ToLower() == "bc3 stop")
                    {
                        ServiceClass service = GetServiceByServiceID(1032, ref lines);
                        CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                    }

                }

                if (result == null && short_code == "8408") //user has sent a keyword
                {
                    ServiceClass service = GetServiceByServiceID(773, ref lines);
                    CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                }

                if (result == null && (short_code == "480" || short_code == "+480"))
                {
                    lines = Add2Log(lines, "Processing " + short_code, 100, "smsreceive");
                    ServiceClass service = GetServiceByServiceID(890, ref lines);
                    CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), text, ref lines);
                }

                if (result == null && short_code == "8343" && text.ToLower().StartsWith("pb|"))
                {
                    int service_id = (msisdn.StartsWith("24266") ? 716 : 732);
                    ServiceClass service = GetServiceByServiceID(service_id, ref lines);
                    string my_text = text.Substring(3).ToLower();
                    PlaceBet(msisdn, service, my_text, ref lines);
                }

                //orange IC
                if (result == null && short_code == "7717")
                {
                    lines = Add2Log(lines, $"Orange IC: processing message {text}", 100, "");

                    // check if text contains our keywords
                    Match match = Regex.Match(text, @"\b(NO([1-3])|tirage|statut)\b", RegexOptions.IgnoreCase);


                    if (!match.Success) // failed -- invalid message
                    {
                        returned_sms_text = "Numéro d'or! Pour souscrire envoyez N(O1, O2 ou O3), se désabonner: stop N(O1, O2, O3) pour des informations supplémentaires: statut; tirage; au 7717. Accedez au site web en cliquant ici: http://icorgtwn.ydot.co";
                    }
                    else if (match.Value.ToLower().Contains("tirage"))          // result
                    {
                        returned_sms_text = LN.LNservice.sms_last_results(ref lines, 32);       // Orange IC = 32
                    }
                    else if (match.Value.ToLower().Contains("stat"))            // status
                    {
                        returned_sms_text = LN.LNservice.sms_subscription_status(ref lines, 32, msisdn);
                    }
                    else // match NO[1-3]
                    {

                        // Get the last digit from the matched group
                        // - determine the SERVICE ID 
                        int digit = int.Parse(match.Groups[2].Value);
                        int service_id = 1183 + digit;
                        lines = Add2Log(lines, $"Orange IC: determined service_ID = {service_id} from NO{digit}", 100, "");

                        // get service Token
                        ServiceClass service = GetServiceByServiceID(service_id, ref lines) ?? throw new Exception($"failed to get service details for serviceID={service_id}");
                        LoginResponse response = Login.DoLogin(new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        });

                        if (response == null || response.ResultCode != 1000) throw new Exception($"OrangeIC: failed to get token for serviceId = {service_id}");


                        // attempt to subscribe or unsubscribe
                        SubscribeRequest subscriber = new SubscribeRequest()
                        {
                            MSISDN = Convert.ToInt64(msisdn),
                            ServiceID = service_id,
                            TokenID = response.TokenID,
                            TransactionID = "sms_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                        };

                        // determine the direction based on whether it has STOP
                        bool unSubscribe = text.ToUpper().Contains("STOP");

                        // attempt to subscribe or unsubscribe
                        SubscribeResponse subResult = null;
                        if (unSubscribe) subResult = UnSubscribe.DoUnSubscribe(subscriber);
                        else subResult = Subscribe.DoSubscribe(subscriber);

                        // set the message based on the (un)Subscript attempt
                        if (subResult == null) returned_sms_text = "erreur technique";
                        else if (unSubscribe) returned_sms_text = "Merci davoir participé. Vous avez été désabonné avec succès de ce service. Si vous souhaitez revenir un jour, envoyez NO1; NO2 ou NO3 au 7717";
                        else returned_sms_text = ussd_mo.SubscribeMultiServiceMenu(service_id, subResult.ResultCode, ref lines);

                        lines = Add2Log(lines, $"Orange IC: unSubscribe flag = {unSubscribe}, retcode = {subResult.ResultCode}, results = {subResult.Description}", 100, "");

                        // define return
                        result = new returned_service()
                        {
                            service_id = service.service_id,
                            token_id = response.TokenID,
                            returned_sms_text = returned_sms_text
                        };
                    }

                    // check if we have a message to return, but no result yet - then use serivceID=1184
                    if (result == null && returned_sms_text != "")
                    {
                        // get service Token -- anyone of the services will do
                        ServiceClass service = GetServiceByServiceID(1184, ref lines) ?? throw new Exception($"failed to get service details for serviceID=1184");
                        LoginResponse response = Login.DoLogin(new LoginRequest()
                        {
                            ServiceID = service.service_id,
                            Password = service.service_password
                        });

                        // check we logged in successfully
                        if (response == null || response.ResultCode != 1000) throw new Exception("OrangeIC: failed to get token for serviceId = 1184");

                        // set the return value
                        result = new returned_service()
                        {
                            service_id = service.service_id,
                            token_id = response.TokenID,
                            returned_sms_text = returned_sms_text
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"exception: {ex.Message}", 100, "");
                lines = Add2Log(lines, ex.StackTrace, 100, "");
            }

            if (result != null && !String.IsNullOrEmpty(result.returned_sms_text)) lines = Add2Log(lines, $"reply = {result.returned_sms_text}", 100, "");
            lines = Write2Log(lines);

            return result;
        }

        public static void PBStar(string msisdn, ServiceClass service, string text, ref List<LogLines> lines)
        {
            List<SportEvents> football_sports_events = Cache.iDoBet.GetMTNGuineaEventsFromCache(31, ref lines);
            List<SportEvents> basketball_sports_events = Cache.iDoBet.GetMTNGuineaEventsFromCache(32, ref lines);
            List<SportEvents> tennis_sports_events = Cache.iDoBet.GetMTNGuineaEventsFromCache(35, ref lines);

            string prefix = (service.service_id == 716 ? "_mgn" : "_ogn");


            string[] words2 = text.Split('*');
            string type = words2[0];
            string amount = words2[1];
            string fgame = "";
            string fbet_type = "";
            EventOdds my_odd = null;
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
                            SportEvents selected_event = null;
                            if (fbet_type.StartsWith("ft"))
                            {
                                selected_event = football_sports_events.Find(x => x.game_id == Convert.ToInt64(fgame));
                                if (selected_event != null)
                                {
                                    string odd_name = "";
                                    int bts_id = GetBTSID(odd_page, 31, out odd_name);
                                    List<EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
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
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", "DBConnectionString_104", ref lines);
                                    }

                                }
                            }
                            if (fbet_type.Contains("2.5"))
                            {
                                odd_page = 1;
                                string odd_name = "";
                                int bts_id = GetBTSID(odd_page, 31, out odd_name);
                                List<EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
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
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", "DBConnectionString_104", ref lines);
                                }
                            }
                            if (fbet_type.StartsWith("dc"))
                            {
                                odd_page = 2;
                                string odd_name = "";
                                int bts_id = GetBTSID(odd_page, 31, out odd_name);
                                List<EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
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
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", "DBConnectionString_104", ref lines);
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
                                    int bts_id = GetBTSID(odd_page, sport_type, out odd_name);
                                    List<EventOdds> event_odds = selected_event.event_odds.Where(x => x.bts_id == bts_id).ToList();
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
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + my_odd.ck_price + ", " + bts_id + ", '" + my_odd.ck_name + "', '" + my_odd.line + "'," + amount + ", " + service.service_id + ")", "DBConnectionString_104", ref lines);
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
                    if (service.service_id == 716)
                    {
                        req_for_order = Api.CommonFuncations.iDoBetMTNGuinea.PlaceBet(ussd_session, ref lines);
                    }
                    else
                    {
                        req_for_order = Api.CommonFuncations.iDoBetOrangeGuinea.PlaceBet(ussd_session, ref lines);
                    }
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
                                string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
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
                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", 0, '" + ussdString + "', now(), 0,'" + user_session_id + "',0, 0, '0', '0'," + amount + ", " + service.service_id + ")", "DBConnectionString_104", ref lines);
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
                            string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
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
                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games" + prefix + " (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + fgame + ", 0, '" + ussdString + "', now(), 0,'" + user_session_id + "',0, 0, '0', '0'," + amount + ", " + l_service.service_id + ")", "DBConnectionString_104", ref lines);
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
                            string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
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


        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "smsreceive");
            lines = Add2Log(lines, "Incomming Post = " + xml, 100, "smsreceive");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "smsreceive");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "smsreceive");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "smsreceive");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                String val = context.Request.QueryString[key];
                if (val != null) lines = Add2Log(lines, $" {key} = {val}, len={val.Length}", 100, "smsreceive");
            }

            string msisdn = context.Request.QueryString["from"]?.Replace("+", "").Replace("%2B", "") ?? "";
            string short_code = context.Request.QueryString["to"] ?? "";
            string text = context.Request.QueryString["text"] ?? "";
            string mobile_operator = context.Request.QueryString["op"] ?? "";


            if 
                (
                    !String.IsNullOrEmpty(msisdn) && 
                    !String.IsNullOrEmpty(short_code) &&
                    (
                        context.Request.ServerVariables["REMOTE_ADDR"] == "185.167.99.119" ||
                        context.Request.ServerVariables["REMOTE_ADDR"] == "185.167.99.120" || 
                        context.Request.ServerVariables["REMOTE_ADDR"] == "::1" || 
                        context.Request.ServerVariables["REMOTE_ADDR"] == "192.168.1.3"
                    )

                )
            {

                returned_service reply = DecidePerTextAndShortCode(text, short_code, msisdn, mobile_operator);
                if (reply == null) lines = Add2Log(lines, " - no post processing required", 100, "");
                else
                {
                    lines = Add2Log(lines, " - sending SMS: service_id = " + reply.service_id + ", msg = " + reply.returned_sms_text, 100, "");
                    try
                    {
                        SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                        {
                            ServiceID = reply.service_id,
                            MSISDN = Convert.ToInt64(msisdn),
                            Text = reply.returned_sms_text,
                            TokenID = reply.token_id,
                            TransactionID = "smsreceive_" + msisdn + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                        };
                        SendSMSResponse response_sendsms = SendSMS.DoSMS(RequestSendSMSBody);
                        if (response_sendsms == null || response_sendsms.ResultCode != 1000) lines = Add2Log(lines, " Send SMS Failed", 100, "");
                        else lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                    }
                    catch (Exception e)
                    {
                        lines = Add2Log(lines, $"!! exception: failed to send sms - {e.Message}", 100, "");
                        lines = Add2Log(lines, e.StackTrace, 100, "");
                    }
                }
                
            }
            else
            {
                lines = Add2Log(lines, $"ignoring message - unauthorized sender {context.Request.ServerVariables}", 100, "");
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