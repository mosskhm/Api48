using Antlr.Runtime.Misc;
using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class BtoBet
    {

        public class SportEvents
        {
            public Int64 game_id { get; set; }
            public Int64 event_id { get; set; }
            public string home_team { get; set; }
            public string away_team { get; set; }
            public string game_time { get; set; }
            public int page_number { get; set; }
            public List<EventOdds> event_odds { get; set; }
            public string league_name { get; set; }
            public string leagueId { get; set; }
        }

        public class EventOdds
        {
            public Int64 bts_id { get; set; }
            public string ck_name { get; set; }
            public string ck_price { get; set; }
            public string line { get; set; }
        }



        public static List<EventOdds> GetGameOdds(ServiceClass service, string game_id, ref List<LogLines> lines)
        {
            List<EventOdds> result = new List<EventOdds>();
            string get_odds_url = Cache.ServerSettings.GetServerSettings("BToBetOddURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "MatchID=" + game_id;

            lines = Add2Log(lines, " get_odds_url = " + get_odds_url, 100, "ivr_subscribe");
            string response = CallSoap.GetURL(get_odds_url, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    Int64 bts_id = 0;

                    foreach (var item in json_response.t)
                    {
                        foreach (var item1 in item.o)
                        {
                            try
                            {
                                bts_id = item1.id;
                                if (bts_id == 3 || bts_id == 17 || bts_id == 4)
                                {
                                    foreach (var o in item1.m)
                                    {
                                        string odd_name = o.otoi;
                                        string odd_price = o.o;
                                        string line = o.id;
                                        result.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                    }
                                }
                                if (bts_id == 29)
                                {
                                    foreach (var o in item1.m)
                                    {
                                        string odd_name = o.otoi;
                                        string odd_price = o.o;
                                        string line = o.id;
                                        if (line == "43818" || line == "43819")
                                        {
                                            result.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " GetGameOdds exception " + ex.ToString(), 100, "ivr_subscribe");
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
                
            }
            return result;
        }

        public static List<SportEvents> GetEvents(ServiceClass service, int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string get_events_url = Cache.ServerSettings.GetServerSettings("BToBetEventURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "sportid=" + sport_type_id;
            lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
            string response = CallSoap.GetURL(get_events_url, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.d.m)
                    {
                        try
                        {
                            string game_id = item.mid;
                            string event_id = item.ec;
                            string home_team = item.ht;
                            string away_team = item.at;
                            string game_time = item.d;
                            game_time = Convert.ToDateTime(game_time).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                            home_team = home_team.Replace("&", "&amp;");
                            away_team = away_team.Replace("&", "&amp;");
                            string league_name = item.tn;
                            string league_id = item.tid;
                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            event_odds = new List<EventOdds>();
                            foreach (var odd in item.o)
                            {
                                bts_id = odd.id;
                                if (bts_id == 3 || bts_id == 29 || bts_id == 7 || bts_id == 4)
                                {
                                    foreach (var o in odd.m)
                                    {
                                        string odd_name = o.n;
                                        string odd_price = o.o;
                                        line = o.id;
                                        event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                    }
                                }
                            }

                            sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_id = Convert.ToInt64(event_id), event_odds = event_odds });




                            counter = counter + 1;
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " GetEvents exception " + ex.ToString(), 100, "ivr_subscribe");
                        }
                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " exception " + ex.ToString(), 100, "ivr_subscribe");
                }
                
            }

            
            return sports_events;
        }

        public static List<SportEvents> GetEvents(ServiceClass service, int sport_type_id, int selected_league_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string get_events_url = Cache.ServerSettings.GetServerSettings("BToBetEventURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "sportid=" + sport_type_id;
            lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
            string response = CallSoap.GetURL(get_events_url, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.d.m)
                    {
                        try
                        {
                            string game_id = item.mid;
                            string event_id = item.ec;
                            string home_team = item.ht;
                            string away_team = item.at;
                            string game_time = item.d;
                            game_time = Convert.ToDateTime(game_time).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                            home_team = home_team.Replace("&", "&amp;");
                            away_team = away_team.Replace("&", "&amp;");
                            string league_name = item.tn;
                            string league_id = item.tid;
                            if (selected_league_id.ToString() == league_id)
                            {
                                if (counter % 3 == 0)
                                {
                                    page_number = page_number + 1;
                                }

                                event_odds = new List<EventOdds>();
                                foreach (var odd in item.o)
                                {
                                    bts_id = odd.id;
                                    if (bts_id == 3 || bts_id == 29 || bts_id == 19 || bts_id == 4)
                                    {

                                        foreach (var o in odd.m)
                                        {
                                            string odd_name = o.n;
                                            string odd_price = o.o;
                                            line = o.id;
                                            event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                        }
                                    }
                                }




                                sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_id = Convert.ToInt64(event_id), event_odds = event_odds });
                                counter = counter + 1;
                            }

                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                        }
                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }
                
            }


            return sports_events;
        }

        public static List<SportEvents> GetHighlightsEvents(ServiceClass service, int sport_type_id, int selected_league_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string get_events_url = Cache.ServerSettings.GetServerSettings("BToBetHighlightsEventURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "sportid=" + sport_type_id;
            lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
            string response = CallSoap.GetURL(get_events_url, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.m)
                    {
                        try
                        {
                            string game_id = item.mid;
                            string event_id = item.ec;
                            string home_team = item.ht;
                            string away_team = item.at;
                            string game_time = item.d;
                            game_time = Convert.ToDateTime(game_time).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                            home_team = home_team.Replace("&", "&amp;");
                            away_team = away_team.Replace("&", "&amp;");
                            string league_name = item.tn;
                            string league_id = item.tid;
                            if (selected_league_id.ToString() == league_id)
                            {
                                if (counter % 3 == 0)
                                {
                                    page_number = page_number + 1;
                                }

                                event_odds = new List<EventOdds>();
                                foreach (var odd in item.o)
                                {
                                    bts_id = odd.id;
                                    if (bts_id == 3 || bts_id == 29 || bts_id == 19 || bts_id == 4)
                                    {

                                        foreach (var o in odd.m)
                                        {
                                            string odd_name = o.n;
                                            string odd_price = o.o;
                                            line = o.id;
                                            event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                        }
                                    }
                                }




                                sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_id = Convert.ToInt64(event_id), event_odds = event_odds });
                                counter = counter + 1;
                            }

                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                        }
                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }

            }


            return sports_events;
        }

        public static List<SportEvents> GetHighlightsEvents(ServiceClass service, int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string get_events_url = Cache.ServerSettings.GetServerSettings("BToBetHighlightsEventURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "sportid=" + sport_type_id;
            lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
            string response = CallSoap.GetURL(get_events_url, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.m)
                    {
                        try
                        {
                            string game_id = item.mid;
                            string event_id = item.ec;
                            string home_team = item.ht;
                            string away_team = item.at;
                            string game_time = item.d;
                            game_time = Convert.ToDateTime(game_time).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                            home_team = home_team.Replace("&", "&amp;");
                            away_team = away_team.Replace("&", "&amp;");
                            string league_name = item.tn;
                            string league_id = item.tid;
                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            event_odds = new List<EventOdds>();
                            foreach (var odd in item.o)
                            {
                                bts_id = odd.id;
                                if (bts_id == 3 || bts_id == 29 || bts_id == 19 || bts_id == 4)
                                {

                                    foreach (var o in odd.m)
                                    {
                                        string odd_name = o.n;
                                        string odd_price = o.o;
                                        line = o.id;
                                        event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                    }
                                }
                            }
                            sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_id = Convert.ToInt64(event_id), event_odds = event_odds });
                            counter = counter + 1;
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception  " + ex.ToString(), 100, "ivr_subscribe");
                        }
                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception  " + ex.ToString(), 100, "ivr_subscribe");
                }
                
            }


            return sports_events;
        }

        public class BtoBetUser
        {
            public string ExternalID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MobileNumber { get; set; }
            public double Real{ get; set; }
            public bool IsSuccessful { get; set; }

        }

        public static BtoBetUser SearchUser(ServiceClass service, string msisdn, ref List<LogLines> lines)
        {
            BtoBetUser result = null;
            string search_url = Cache.ServerSettings.GetServerSettings("BToBetSearchURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string soap = "{\"apiKey\":\""+ Cache.ServerSettings.GetServerSettings("APIBOKey_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + "\",\"username\":\""+msisdn+"\" }";
            List<Headers> headers = new List<Headers>();
            string response = CallSoap.CallSoapRequest(search_url, soap, headers, 2, true, ref lines);
            if (!String.IsNullOrEmpty(response))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    bool IsSuccessful = json_response.IsSuccessful;
                    if (IsSuccessful == true)
                    {
                        string ExternalID = json_response.Customer.Account.ExternalID;
                        string FirstName = json_response.Customer.Account.FirstName;
                        string LastName = json_response.Customer.Account.LastName;
                        string MobileNumber = json_response.Customer.Account.MobileNumber;
                        double Real = json_response.Customer.Balance.Real;
                        result = new BtoBetUser()
                        {
                            ExternalID = ExternalID,
                            FirstName = FirstName,
                            LastName = LastName,
                            MobileNumber = MobileNumber,
                            Real = (Real / 100),
                            IsSuccessful = true
                        };


                    }
                    else
                    {
                        result = new BtoBetUser()
                        {
                            IsSuccessful = false,

                        };
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception  " + ex.ToString(), 100, lines[0].ControlerName);
                }
                
            }
            else
            {
                result = new BtoBetUser()
                {
                    IsSuccessful = false,
                };
            }
            return result;
        }

        public static string GetDepositMoneyMenu(ServiceClass service, BtoBetUser user, USSDMenu ussd_menu, string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            bool user_has_momo = false;
            if (user.IsSuccessful == false)
            {
                if (String.IsNullOrEmpty(user.FirstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site https://www.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = "Quelque chose est arrivé s'il vous plaît appelez à l'équipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }

            }
            else
            {
                if (user.FirstName.ToLower().Contains("ussd-"))
                {
                    menu = "Bonjour " + MSISDN + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    menu = "Bonjour " + user.FirstName + " " + user.LastName + Environment.NewLine + Environment.NewLine;
                }
                
                //check if the user has a MOMO account
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
                        DYACheckAccountRequest request = new DYACheckAccountRequest()
                        {
                            MSISDN = Convert.ToInt64(MSISDN),
                            ServiceID = service.service_id,
                            TokenID = token_id
                        };
                        DYACheckAccountResponse response = CommonFuncations.DYACheckAccount.DODYACheckAccount(request);
                        if (response.ResultCode == 1000)
                        {
                            user_has_momo = true;
                        }
                    }
                }

                if (user_has_momo)
                {
                    menu = menu + "Entrer votre montant" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "Minimum: 200 FCFA" + Environment.NewLine;
                    menu = menu + "Maximum: 1,000,000 FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                    menu = menu + "taper *155# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
            }
            return menu;
        }


        public static string GetStartDepositMoneyMenu(ServiceClass service, BtoBetUser user, USSDMenu ussd_menu, string MSISDN, string amount, out DYAReceiveMoneyRequest momo_request, ref List<LogLines> lines)
        {
            momo_request = null;
            string menu = "";
            string token_id = "";
            bool user_has_momo = false;
            if (user.IsSuccessful == false)
            {
                if (String.IsNullOrEmpty(user.FirstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site https://www.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = "Quelque chose est arrivé s'il vous plaît appelez à l'équipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }

            }
            else
            {
                menu = "Bonjour " + user.FirstName + " " + user.LastName + Environment.NewLine + Environment.NewLine;
                //check if the user has a MOMO account
                //ServiceClass subscriber_service1 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
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
                        token_id = res1.TokenID;
                        DYACheckAccountRequest request = new DYACheckAccountRequest()
                        {
                            MSISDN = Convert.ToInt64(MSISDN),
                            ServiceID = service.service_id,
                            TokenID = token_id
                        };
                        DYACheckAccountResponse response = CommonFuncations.DYACheckAccount.DODYACheckAccount(request);
                        if (response.ResultCode == 1000)
                        {
                            user_has_momo = true;
                        }
                    }
                }

                if (user_has_momo)
                {
                    if (Convert.ToInt32(amount) >= 50 && Convert.ToInt32(amount) <= 1000000)
                    {
                        momo_request = new DYAReceiveMoneyRequest()
                        {
                            MSISDN = Convert.ToInt64(MSISDN),
                            Amount = Convert.ToInt32(amount),
                            ServiceID = service.service_id,
                            TokenID = token_id,
                            TransactionID = "BtoBetDeposit_" + Guid.NewGuid().ToString().Substring(0, 10).Replace("-", "")

                        };
                        menu = menu + "Vous avez presque fini" + Environment.NewLine;
                        menu = menu + "Pour compléter votre pari, confirmez le paiement" + Environment.NewLine;
                    }
                    else
                    {
                        menu = menu + "Entrer votre montant" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Minimum: 50 FCFA" + Environment.NewLine;
                        menu = menu + "Maximum: 1,000,000 FCFA" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Menu Principal ";
                    }

                }
                else
                {
                    menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                    menu = menu + "taper *155# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }


            }
            return menu;
        }


        public static string GetWithdrawMoneyMenu(BtoBetUser user, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.IsSuccessful == false)
            {
                if (String.IsNullOrEmpty(user.FirstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site https://www.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = "Quelque chose est arrivé s'il vous plaît appelez à l'équipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
            }
            else
            {
                menu = "Bonjour " + user.FirstName + " " + user.LastName + Environment.NewLine + Environment.NewLine;
                menu = menu + "Votre solde est " + user.Real + " FCFA" + Environment.NewLine;
                if (user.Real > 0)
                {
                    menu = menu + "Entrer le montant à retirer" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = menu + Environment.NewLine + "M) Menu Principal ";
                }
            }
            return menu;
        }

        public static string GetStartWithdrawMoneyMenu(BtoBetUser user, bool refund_money, bool w_result, bool amount_2_big, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.IsSuccessful == false)
            {
                menu = "Option invalide";
            }
            else
            {
                if (refund_money == true)
                {
                    menu = "La demande de retrait a abouti" + Environment.NewLine;
                }
                else
                {

                    menu = "La demande de retrait a échoué" + Environment.NewLine;
                    if (amount_2_big)
                    {
                        menu = menu + "Montant d'argent est trop grand" + Environment.NewLine;
                    }
                    menu = menu + "Veuillez réessayer";
                }
            }

            return menu;
        }

        public static string GetCloseBet(ServiceClass service, ref List<LogLines> lines)
        {
            string menu = "";
            menu = menu + "Pour compléter votre pari, confirmez le paiement." + Environment.NewLine;
            menu = menu + "Confirmez T&C et +18 " + Cache.ServerSettings.GetServerSettings("BToBetSiteTermsURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines) + Environment.NewLine;
            return menu;
        }

        public static string GetCloseBetFailed(ref List<LogLines> lines)
        {
            string menu = "";
            menu = "Votre pari a échoué." + Environment.NewLine;
            menu = menu + "Veuillez réessayer"; //Your bet has failed.
            return menu;
        }

        public static string GetConfirmBet(USSDSession ussd_session, string amount, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                menu = "Confirmer votre Pari:" + Environment.NewLine;
                menu = menu + "Numéro des Jeux: " + saved_games.Count() + Environment.NewLine;
                double total_ratio = 1;
                foreach (SavedGames r in saved_games)
                {
                    total_ratio = total_ratio * r.selected_odd;
                }
                total_ratio = Math.Round(total_ratio, 2);
                menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine;
                menu = menu + "Montant:" + amount + Environment.NewLine;
                double max_payout = Math.Round(total_ratio * Convert.ToDouble(amount), 2);
                menu = menu + "Gain Maximum: " + max_payout + Environment.NewLine + Environment.NewLine;
                menu = menu + "1) Confirmer" + Environment.NewLine;
                menu = menu + "2) Annuler & Recommencer" + Environment.NewLine + Environment.NewLine;

                menu = menu + "*) Retour";
            }
            return menu;
        }

        public static string GetWrongPriceBetMeny(ref List<LogLines> lines)
        {
            string menu = "Veuillez saisir un montant compris entre 200 FCFA et 1000000 FCFA" + Environment.NewLine + Environment.NewLine;
            menu = menu + "*) Retour";
            return menu;
        }

        public class BetSlipItems
        {
            public Int64 EventCode { get; set; }
            public string OutcomeShortCode { get; set; }
        }

        public class PlaceBetClass
        {
            public string Mobile { get; set; }
            public int Stake { get; set; }
            public List<BetSlipItems> BetSlipItems { get; set; }
        }



        public static string PlaceBet(ServiceClass service, string MSISDN, List<SavedGames> saved_games, out double max_p, ref List<LogLines> lines)
        {
            string BetSlipID = "";
            BetSlipItems bsi = new BetSlipItems();
            List<BetSlipItems> bsis = new List<BetSlipItems>();
            double total_ratio = 1;
            max_p = 0;
            foreach (SavedGames r in saved_games)
            {
                bsi = new BetSlipItems()
                {
                    EventCode = Convert.ToInt64(r.event_id),
                    OutcomeShortCode = r.selected_odd_name
                };
                bsis.Add(bsi);
                total_ratio = total_ratio * r.selected_odd;
            }
            total_ratio = Math.Round(total_ratio, 2);
            max_p = Math.Round(total_ratio * Convert.ToDouble(saved_games[0].amount), 2);

            if (bsis.Count > 0)
            {
                PlaceBetClass pbc = new PlaceBetClass()
                {
                    Mobile = MSISDN,
                    Stake = saved_games[0].amount,
                    BetSlipItems = bsis
                };

                string placebet_url = Cache.ServerSettings.GetServerSettings("BToBetPlaceBetURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                string x_api_key = Cache.ServerSettings.GetServerSettings("BToBetXApiKey_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                string postBody = JsonConvert.SerializeObject(pbc);
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "X-API-Key", value = x_api_key });
                string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(placebet_url, postBody, headers, 2, ref lines);

                if (!String.IsNullOrEmpty(response_body))
                {
                    try
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        if (json_response.ResponseCode == "0")
                        {
                            BetSlipID = json_response.BetSlipID;
                        }
                        else
                        {
                            string mail_body = "<p><b>BtoBet Place bet has failed for</b><br> <b>user:</b> " + MSISDN + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "BtoBet Place bet has failed for" + MSISDN;
                            string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                            string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }

            }

            return BetSlipID;
        }


        public static string CloseBet(ServiceClass service, string MSISDN, USSDSession ussd_session, out DYAReceiveMoneyRequest momo_request,  ref List<LogLines> lines)
        {
            string menu = "Une erreur est survenue. Veuillez réessayer plus tard";
            momo_request = null;
            bool login_continue_m = true;
            string token_id_m = "";
            BtoBetUser user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
            bool take_money = false;
            bool create_user = false;
            int amount = 0;
            int password = 0;
            if (user.IsSuccessful == true)
            {
                List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);

                if (saved_games != null)
                {
                    if (saved_games[0].amount <= user.Real)
                    {
                        amount = saved_games[0].amount;
                        double max_p = 0;
                        string BetSlipID = PlaceBet(service, MSISDN, saved_games, out max_p, ref lines);
                        if (!String.IsNullOrEmpty(BetSlipID) && BetSlipID != "0")
                        {
                            List<Int64> saved_ids = GetUSSDSavedGamesID(ussd_session.user_seesion_id, ref lines);
                            if (saved_ids != null)
                            {
                                foreach (Int64 s in saved_ids)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + BetSlipID + "', `status` = 2 where id = " + s, ref lines);
                                }
                            }
                            string text_msg_m = "Bonjour, votre pari a été placé avec succès.\n";// + Environment.NewLine;
                            text_msg_m = text_msg_m + "Code à barre: " + BetSlipID + "\n";//Environment.NewLine;
                            text_msg_m = text_msg_m + "Montant: " + saved_games[0].amount + "\n";//Environment.NewLine;
                            text_msg_m = text_msg_m + "Gain Maximum: " + max_p;//Environment.NewLine;

                            menu = "votre pari a été placé avec succès" + Environment.NewLine;
                            menu = menu + "Code à barre: " + BetSlipID + Environment.NewLine;
                            menu = menu + "Montant: " + saved_games[0].amount + Environment.NewLine;
                            menu = menu + "Gain Maximum: " + max_p;

                            switch (service.service_id)
                            {
                                case 715:
                                case 720:
                                    GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(MSISDN.ToString()), "127.0.0.1", "BJ", "event", "Bet", "MOMO", saved_games[0].amount.ToString(), "/", ref lines);
                                    break;
                            }

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

                            if (login_continue_m)
                            {
                                SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                                {
                                    ServiceID = service.service_id,
                                    MSISDN = Convert.ToInt64(MSISDN),
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
                        }
                    }
                    else
                    {
                        take_money = true;
                    }
                }
            }
            else
            {
                create_user = true;
                take_money = true;
            }

            
            if (create_user)
            {
                var random = new Random();
                password = random.Next(212345,999999);

                create_user = CreateUser(service, MSISDN, password, ref lines);
                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                
                List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
                if (saved_games != null)
                {
                    amount = saved_games[0].amount;
                }
            }

            

            if (take_money && user.IsSuccessful == true)
            {
                List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
                if (saved_games != null)
                {
                    amount = saved_games[0].amount;
                }

                menu = Api.CommonFuncations.BtoBet.GetCloseBet(service, ref lines);
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
                            MSISDN = Convert.ToInt64(MSISDN),
                            Amount = amount,
                            ServiceID = service.service_id,
                            TokenID = token_id,
                            TransactionID = ussd_session.user_seesion_id
                        };

                        if (password > 0)
                        {
                            string text_msg_m = "Bienvenue chez YellowBet. Pour placer un pari, visitez www.yellowbet.cm .Votre mot de passe est " + password;
                            switch (service.service_id)
                            {
                                case 720:
                                    text_msg_m = "Bienvenue chez YellowBet. Pour placer un pari, visitez www.greenwin.cm .Votre mot de passe est " + password;
                                    break;
                            }
                            
                            SendSMSRequest RequestSendSMSBody = new SendSMSRequest()
                            {
                                ServiceID = service.service_id,
                                MSISDN = Convert.ToInt64(MSISDN),
                                Text = text_msg_m,
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
                    }
                    else
                    {
                        menu = Api.CommonFuncations.BtoBet.GetCloseBetFailed(ref lines);
                    }
                }
                else
                {
                    menu = Api.CommonFuncations.BtoBet.GetCloseBetFailed(ref lines);
                }
            }


            return menu;
        }

        public static string GetPayAndConfirm(USSDSession ussd_session, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {

                menu = menu + "Entrer le montant:" + Environment.NewLine + Environment.NewLine;
                menu = menu + "Numéro des Jeux: " + saved_games.Count() + Environment.NewLine;
                double total_ratio = 1;
                foreach (SavedGames r in saved_games)
                {
                    total_ratio = total_ratio * r.selected_odd;
                }
                total_ratio = Math.Round(total_ratio, 2);
                menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine + Environment.NewLine;
                menu = menu + "*) Retour";
            }
            return menu;
        }


        public static string CheckBeforeWithdrawMenu(ServiceClass service, string msisdn, ref List<LogLines> lines)
        {
            string menu = "";
            Int64 current_momo_daily_amount = DBQueries.SelectQueryReturnInt64("SELECT s.current_momo FROM service_momo_limitation s WHERE s.service_id = " + service.service_id, ref lines);
            if (current_momo_daily_amount >= service.daily_momo_limit)
            {
                lines = Add2Log(lines, "Limit has reached", 100, "DYATransferMoney");
                menu = "Votre opération a échoué. Merci d'essayer plus tard.";
            }
            if (service.user_w_limit > 0)
            {
                Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_w FROM daily_withdrawls d WHERE d.msisdn = " + msisdn, ref lines);
                if (user_withdraw_limit >= service.user_w_limit)
                {
                    lines = Add2Log(lines, "User Limit has reached", 100, "DYATransferMoney");
                    menu = "Votre opération a échoué. Merci d'essayer plus tard.";
                }
            }
            if (service.user_limit_amount > 0)
            {
                Int64 user_withdraw_limit_amount = DBQueries.SelectQueryReturnInt64("SELECT d.amount FROM daily_withdrawls d WHERE d.msisdn = " + msisdn, ref lines);
                if (user_withdraw_limit_amount >= service.user_limit_amount)
                {
                    lines = Add2Log(lines, "User Limit amount has reached", 100, "DYATransferMoney");
                    menu = "Votre opération a échoué. Merci d'essayer plus tard.";
                }
            }
            return menu;
        }

        public class AgentWithdrawalProcessV3
        {
            public string PspId { get; set; }
            public string OrderId { get; set; }
            public string Currency { get; set; }
            public int Amount { get; set; }
            public string WithdrawalId { get; set; }
            public string Username { get; set; }
            public string PosId { get; set; }
            public string CashierId { get; set; }

        }

        public class AgentDepositProcess
        {
            public string PspId { get; set; }
            public string OrderId { get; set; }
            public string Currency { get; set; }
            public int Amount { get; set; }
            public string Username { get; set; }
            public string PosId { get; set; }
            public string CashierId { get; set; }

        }



        public static string WithdrawMoney(ServiceClass service, string msisdn, int amount, string order_id, out int fee,  ref List<LogLines> lines)
        {
            fee = 0;
            string transaction_id = "";
            string withdraw_url = Cache.ServerSettings.GetServerSettings("BToBetWithdrawURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string PspId = Cache.ServerSettings.GetServerSettings("BToBetPspId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string Currency = Cache.ServerSettings.GetServerSettings("BToBetCurrency_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string PosId = Cache.ServerSettings.GetServerSettings("BToBetPosId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string CashierId = Cache.ServerSettings.GetServerSettings("BToBetCashierId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string username = Cache.ServerSettings.GetServerSettings("BToBetUserName_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("BToBetPassword_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);

            AgentWithdrawalProcessV3 request = new AgentWithdrawalProcessV3()
            {
                PspId = PspId,
                OrderId = order_id,
                Currency = Currency,
                Amount = amount,
                WithdrawalId = null,
                Username = msisdn,
                PosId = PosId,
                CashierId = CashierId
            };

            string postBody = JsonConvert.SerializeObject(request);
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(withdraw_url, postBody, headers, 2, username, password, ref lines);

            if (!String.IsNullOrEmpty(response_body))
            {
                
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.Status == "0")
                    {
                        transaction_id = json_response.TransactionId;
                        fee = Convert.ToInt32(json_response.FeeAmount);
                    }
                    else
                    {
                        string mail_body = "<p><b>BtoBet Withdraw has failed for</b><br> <b>user:</b> " + msisdn + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                        string mail_subject = "BtoBet Withdraw has failed for " + msisdn;
                        string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                        string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }
            }

            return transaction_id;


        }

        public static string DepositMoney(ServiceClass service, string msisdn, int amount, string order_id, ref List<LogLines> lines)
        {
            string transaction_id = "";
            string deposit_url = Cache.ServerSettings.GetServerSettings("BToBetDepositURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string PspId = Cache.ServerSettings.GetServerSettings("BToBetPspId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string Currency = Cache.ServerSettings.GetServerSettings("BToBetCurrency_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string PosId = Cache.ServerSettings.GetServerSettings("BToBetPosId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string CashierId = Cache.ServerSettings.GetServerSettings("BToBetCashierId_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string username = Cache.ServerSettings.GetServerSettings("BToBetUserName_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("BToBetPassword_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);

            AgentDepositProcess request = new AgentDepositProcess()
            {
                PspId = PspId,
                OrderId = order_id,
                Currency = Currency,
                Amount = amount,
                Username = msisdn,
                PosId = PosId,
                CashierId = CashierId
            };

            string postBody = JsonConvert.SerializeObject(request);
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(deposit_url, postBody, headers, 2, username, password, ref lines);

            if (!String.IsNullOrEmpty(response_body))
            {
                
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.Status == "0")
                    {   
                        transaction_id = json_response.TransactionId;
                    }
                    else
                    {
                        string mail_body = "<p><b>BtoBet Deposit has failed for</b><br> <b>user:</b> " + msisdn + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                        string mail_subject = "BtoBet Deposit has failed for " + msisdn;
                        string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                        string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }
            }

            return transaction_id;


        }


        public static bool CreateUser(ServiceClass service, string msisdn, int password, ref List<LogLines> lines)
        {

            string firstname = "";
            string lastname = "";

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
                    DYACheckAccountRequest request = new DYACheckAccountRequest()
                    {
                        MSISDN = Convert.ToInt64(msisdn),
                        ServiceID = service.service_id,
                        TokenID = token_id
                    };
                    DYACheckAccountResponse response = CommonFuncations.DYACheckAccount.DODYACheckAccount(request);
                    if (response.ResultCode == 1000)
                    {
                        firstname = response.FirstName;
                        lastname = response.LastName;
                    }
                }
            }

            firstname = (!String.IsNullOrEmpty(firstname) ? firstname : msisdn);
            lastname = (!String.IsNullOrEmpty(lastname) ? lastname : "USSD");

            bool user_created = false;
            string createuser_url = Cache.ServerSettings.GetServerSettings("BToBetCreateUserURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string Currency = Cache.ServerSettings.GetServerSettings("BToBetCurrency_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string CountryISO = Cache.ServerSettings.GetServerSettings("BToBetCountryISO_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string LanguageISO = Cache.ServerSettings.GetServerSettings("BToBetLanguageISO_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string TimeZoneName = Cache.ServerSettings.GetServerSettings("BToBetTimeZoneName_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string CityAdress = Cache.ServerSettings.GetServerSettings("BToBetCityAdress_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string ApiKey = Cache.ServerSettings.GetServerSettings("APIBOKey_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            string MailRef = Cache.ServerSettings.GetServerSettings("BToBetMailRef_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);

            string soap = "{ ";
            soap = soap + "   \"customer\":{ ";
            soap = soap + "      \"PreferredNotificationType\":3,";
            soap = soap + "      \"CustomerV3\":{ ";
            soap = soap + "         \"CustomerDetails\":{ ";
            soap = soap + "            \"FirstName\":\""+ firstname + "\",";
            soap = soap + "            \"LastName\":\""+ lastname + "\",";
            soap = soap + "            \"Email\":\""+ msisdn + "@"+ MailRef + "\",";
            soap = soap + "            \"Username\":\""+ msisdn + "\",";
            soap = soap + "            \"PhoneNumber\":\""+ msisdn + "\",";
            soap = soap + "            \"MobileNumber\":\""+ msisdn + "\",";
            soap = soap + "            \"City\":\""+ CityAdress + "\",";
            soap = soap + "            \"Postcode\":\"17001\",";
            soap = soap + "            \"Address\":\""+ CityAdress + "\",";
            soap = soap + "            \"Gender\":\"Male\",";
            soap = soap + "            \"LanguageISO\":\""+ LanguageISO + "\",";
            soap = soap + "            \"CountryISO\":\""+ CountryISO + "\",";
            soap = soap + "            \"CurrencyISO\":\""+ Currency + "\",";
            soap = soap + "            \"Password\":\""+ password + "\",";
            soap = soap + "            \"DateOfBirth\":\"1970-01-01\",";
            soap = soap + "            \"IPAddress\":\"\",";
            soap = soap + "            \"Browser\":\"Chrome\",";
            soap = soap + "            \"CivilIdentificationCode\":\"123132132113112\",";
            soap = soap + "            \"Note\":\"\",";
            soap = soap + "            \"EmploymentStatus\":0,";
            soap = soap + "            \"Longitude\":null,";
            soap = soap + "            \"Latitude\":null,";
            soap = soap + "            \"TimeZoneName\":\""+ TimeZoneName + "\",";
            soap = soap + "            \"IsTestCustomer\":\"false\",";
            soap = soap + "            \"PassportNumber\":\"\",";
            soap = soap + "            \"Profession\":\"\"";
            soap = soap + "         }";
            soap = soap + "	 }";
            soap = soap + "   },";
            soap = soap + "   \"deviceType\":\"Default\",";
            soap = soap + "   \"apiKey\":\""+ ApiKey + "\",";
            soap = soap + "   \"activateAccount\":\"true\",";
            soap = soap + "   \"loginAccount\":\"false\"";
            soap = soap + "}";

            
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(createuser_url, soap, headers, 2, ref lines);

            if (!String.IsNullOrEmpty(response_body))
            {
                
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.IsSuccessful == true)
                    {
                        user_created = json_response.IsSuccessful;
                    }
                    else
                    {
                        string mail_body = "<p><b>BtoBet CreateUser has failed for</b><br> <b>user:</b> " + msisdn + "<br><br> <b>Request:</b> " + soap + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                        string mail_subject = "BtoBet CreateUser has failed for " + msisdn;
                        string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                        string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }
            }

            return user_created;


        }

        public class Tickets
        {
            public string barcode { get; set; }
            public string msisdn { get; set; }
            public double payout { get; set; }
            public double amount { get; set; }
            public string creation_time { get; set; }
            public string win_status { get; set; }
            public int page_number { get; set; }
        }

        public static List<Tickets> CheckTickets(ServiceClass service, BtoBetUser user, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            string check_ticket_url = Cache.ServerSettings.GetServerSettings("BToBetChecckTicketURL_" + service.service_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "ExternalUserID", value = user.ExternalID });
            headers.Add(new Headers { key = "Provider", value = "0" });
            string response_body = Api.CommonFuncations.CallSoap.GetURL(check_ticket_url, headers, ref lines);

            if (!String.IsNullOrEmpty(response_body))
            {
                if (response_body != "[]")
                {
                    
                    try
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        tickets = new List<Tickets>();
                        int page_number = 0;
                        int counter = 0;
                        foreach (var item in json_response)
                        {
                            string msisdn = user.MobileNumber;
                            string barcode = item.id;
                            double maxpayout = item.w;
                            string creation_time = Convert.ToDateTime(item.c).AddHours(1).ToString("dd/MM HH:mm"); //.ToString("dd/MM/yy HH:mm");
                            //creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.s;
                            string win_status = item.stid;
                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = maxpayout, creation_time = creation_time, amount = amount, win_status = win_status, page_number = page_number });

                            counter = counter + 1;

                        }
                        if (counter == 0)
                        {
                            tickets = null;
                            lines = Add2Log(lines, " tickets class is empty", 100, "ivr_subscribe");
                        }
                        else
                        {
                            lines = Add2Log(lines, " Finished building tickets class ", 100, "ivr_subscribe");
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
                
            }




            return tickets;
        }


        public static string GetCheckTicketMenu(ServiceClass service, BtoBetUser user, int page_number, string ussd_string, ref List<LogLines> lines)
        {
            string menu = "";

            List<Tickets> tickets = CheckTickets(service, user, ref lines);

            if (tickets != null)
            {
                List<Tickets> filtered_tickets = tickets.Where(x => x.page_number == page_number).ToList();
                Tickets filtered_ticket = filtered_tickets[Convert.ToInt32(ussd_string) - 1];
                if (filtered_ticket != null)
                {
                    menu = "Statut du Ticket:" + Environment.NewLine;
                    menu = menu + "Code Barre: " + filtered_ticket.barcode + Environment.NewLine;
                    menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                    string status = "";
                    switch (filtered_ticket.win_status)
                    {
                        case "0":
                            status = "en attente";
                            break;
                        case "2": //looser
                            status = "Perdant";
                            break;
                        case "1":
                            status = "Gagnant";
                            break;
                        case "3":
                            status = "Remboursement";
                            break;

                    }

                    menu = menu + "Statut: " + status + Environment.NewLine;

                    menu = menu + "Enjeu: " + filtered_ticket.amount + Environment.NewLine;
                    if (status == "en attente" || status == "Gagnant")
                    {
                        menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                    }

                    menu = menu + Environment.NewLine + "*) Retour";

                }
                else
                {
                    menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }

            }
            else
            {
                menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }
            return menu;
        }

        public static string GetCheckTicketMenu(ServiceClass service, BtoBetUser user, int page_number, ref List<LogLines> lines)
        {
            string menu = "";

            List<Tickets> tickets = CheckTickets(service, user, ref lines);

            if (tickets != null)
            {
                List<Tickets> filtered_tickets = tickets.Where(x => x.page_number == page_number).ToList();
                menu = "Statut du Ticket" + Environment.NewLine;
                int counter = 1;
                foreach (Tickets t in filtered_tickets)
                {
                    menu = menu + counter + ") " + t.barcode + " (" + t.creation_time + ")" + Environment.NewLine;
                    counter = counter + 1;
                }
                int total_pages = (tickets.Count() / 3) + (tickets.Count() % 3 == 0 ? 0 : 1);
                if (page_number == 1 && total_pages != page_number)
                {
                    menu = menu + Environment.NewLine + "#) Suivant";
                }
                if (page_number > 1 && page_number < total_pages)
                {
                    menu = menu + Environment.NewLine + "#) Suivant" + Environment.NewLine;
                    menu = menu + "*) Retour";
                }
                if (page_number == total_pages && page_number > 1)
                {
                    menu = menu + Environment.NewLine + "*) Retour";
                }

                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";

            }
            else
            {
                menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }

            
            return menu;
        }


        public static string GetCheckTicketByBarcodeMenu(ServiceClass service, BtoBetUser user, string bar_codeid, out string paid_amount, ref List<LogLines> lines)
        {
            string menu = "";
            paid_amount = "0";
            List<Tickets> tickets = CheckTickets(service, user, ref lines);
            if (tickets != null)
            {
                Tickets filtered_ticket = tickets.Find(x => x.barcode == bar_codeid);

                if (filtered_ticket != null)
                {
                    menu = "Statut du Ticket:" + Environment.NewLine;
                    menu = menu + "Code Barre: " + filtered_ticket.barcode + Environment.NewLine;
                    menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                    string status = "";
                    switch (filtered_ticket.win_status)
                    {
                        case "0":
                            status = "en attente";
                            break;
                        case "2": //looser
                            status = "Perdant";
                            break;
                        case "1":
                            status = "Gagnant";
                            break;
                        case "3":
                            status = "Remboursement";
                            break;

                    }
                    menu = menu + "Statut: " + status + Environment.NewLine;

                    menu = menu + "Enjeu: " + filtered_ticket.amount + Environment.NewLine;
                    if (status == "en attente" || status == "Gagnant")
                    {
                        menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                    }
                    if (status == "Gagnant - Payé")
                    {
                        menu = menu + "Montant payé: " + filtered_ticket.payout + Environment.NewLine;
                    }
                    menu = menu + Environment.NewLine + "*) Retour";
                }
                else
                {
                    menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }

            }
            else
            {
                menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }
            return menu;
        }

        public static string GetSportsTypeMenu(ref List<LogLines> lines)
        {
            string menu = "";
            menu = menu + "Choisir le Sport:" + Environment.NewLine + Environment.NewLine;
            menu = menu + "1) Football" + Environment.NewLine;
            menu = menu + "2) Basketball" + Environment.NewLine;
            menu = menu + "3) Tennis" + Environment.NewLine + Environment.NewLine;
            menu = menu + "M) Menu Principal ";
            return menu;
        }

        public static string GetSoccerLeagueMenu(ServiceClass service, int sport_type, string ussd_string, int ussd_id, ref List<LogLines> lines, out int out_league_id)
        {
            out_league_id = 0;
            string menu = "";
            List<SportEvents> sports_events = Cache.BtoBet.GetEventsFromCache(service, sport_type, ref lines);
            List<SportEvents> highlights_events = Cache.BtoBet.GetHighLightsEventsFromCache(service, sport_type, ref lines);

            menu = "Parions sur le foot:" + Environment.NewLine;
            int counter = 2;
            List<BtoBetLeague> idobet_leagues = Cache.BtoBet.GetBtoBetLeagues(ref lines);


            if (sports_events != null && highlights_events != null)
            {
                menu = menu + "1) Highlights (" + highlights_events.Count() + ")" + Environment.NewLine;
                menu = menu + "2) Tous les evenements (" + sports_events.Count() + ")" + Environment.NewLine;
                var query = highlights_events.GroupBy(x => x.leagueId, x => x.league_name, (league_id, league_name) => new { league_id = league_id, Count = league_name.Count(), league_name = league_name }).OrderByDescending(x => x.Count).ToList();

                if (idobet_leagues != null)
                {
                    idobet_leagues = idobet_leagues.Where(x => x.ussd_id == ussd_id).ToList();
                    foreach (BtoBetLeague l in idobet_leagues)
                    {
                        counter = counter + 1;
                        var filtered_query = query.Where(x => x.league_id == l.league_id.ToString()).ToList();
                        int games_count = sports_events.Where(x => x.leagueId == l.league_id.ToString()).Count();
                        if (filtered_query != null && filtered_query.Count() > 0)
                        {
                            menu = menu + counter + ") " + l.league_name + " (" + games_count + ")" + Environment.NewLine;
                            if (ussd_string == counter.ToString())
                            {
                                out_league_id = l.league_id;
                            }
                            
                            query = query.Where(x => x.league_id != l.league_id.ToString()).ToList();
                        }
                        if (counter == 3)
                        {
                            break;
                        }
                    }
                }

                if (counter < 3)
                {
                    foreach (var q in query)
                    {
                        counter = counter + 1;
                        string leage_name = highlights_events.Find(x => x.leagueId == q.league_id.ToString()).league_name;
                        int games_count = highlights_events.Where(x => x.leagueId == q.league_id.ToString()).Count();
                        menu = menu + counter + ") " + leage_name + " (" + games_count + ")" + Environment.NewLine;
                        if (ussd_string == counter.ToString())
                        {
                            out_league_id = Convert.ToInt32(q.league_id);
                        }
                       
                        if (counter == 3)
                        {
                            break;
                        }
                    }
                }
                menu = menu + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }



            return menu;
        }


        public static string GetEventsMenu(ServiceClass service, string ussd_string, int sport_type_id, int page_number, ref List<LogLines> lines, out int selected_league_id, int real_selected_league_id, USSDMenu ussd_menu, int choice)
        {
            selected_league_id = 0;

            string menu = "";
            List<SportEvents> sports_events = null;
            switch (choice)
            {
                case 1:
                    sports_events = Cache.BtoBet.GetHighLightsEventsFromCache(service, sport_type_id, ref lines);
                    break;
                case 2:
                    sports_events = Cache.BtoBet.GetEventsFromCache(service, sport_type_id, ref lines);
                    break;

            }

            if (ussd_string == "3")
            {
                string a = GetSoccerLeagueMenu(service, sport_type_id, ussd_string, ussd_menu.ussd_id, ref lines, out selected_league_id);
                sports_events = Cache.BtoBet.GetEventsWithLeagueIDFromCache(service, sport_type_id, (selected_league_id == 0 ? real_selected_league_id : selected_league_id), ref lines);
                selected_league_id = (selected_league_id == 0 ? real_selected_league_id : selected_league_id);
            }
            

            if (sports_events != null)
            {
                List<SportEvents> filtered_sports_events = sports_events.Where(x => x.page_number == page_number).ToList();
                if (filtered_sports_events != null)
                {
                    //menu = "Choisir un jeu:" + Environment.NewLine;
                    int counter = 1;
                    foreach (SportEvents se in filtered_sports_events)
                    {
                        menu = menu + counter + ") " + se.home_team + " Vs " + se.away_team + Environment.NewLine;
                        counter = counter + 1;
                    }
                    int total_pages = (sports_events.Count() / 3) + (sports_events.Count() % 3 == 0 ? 0 : 1);
                    if (page_number == 1 && total_pages != page_number)
                    {
                        menu = menu + Environment.NewLine + "#) Suivant";
                    }
                    if (page_number > 1 && page_number < total_pages)
                    {
                        menu = menu + Environment.NewLine + "#) Suivant" + Environment.NewLine;
                        menu = menu + "*) Retour ";
                    }
                    if (page_number == total_pages && page_number > 1)
                    {
                        menu = menu + Environment.NewLine + "*) Retour";
                    }
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";

                }
            }


            return menu;
        }

        public static string GetLeaguEventsMenu(ServiceClass service, string ussd_string, int sport_type_id, int page_number, ref List<LogLines> lines, out int selected_league_id, int real_selected_league_id, USSDMenu ussd_menu, int choice)
        {
            selected_league_id = 0;

            string menu = "";
            List<SportEvents> sports_events = null;

            string a = GetSoccerLeagueMenu(service, sport_type_id, ussd_string, ussd_menu.ussd_id, ref lines, out selected_league_id);
            sports_events = Cache.BtoBet.GetEventsWithLeagueIDFromCache(service, sport_type_id, (selected_league_id == 0 ? real_selected_league_id : selected_league_id), ref lines);
            selected_league_id = (selected_league_id == 0 ? real_selected_league_id : selected_league_id);

            if (selected_league_id == 0)
            {
                sports_events = Cache.BtoBet.GetEventsFromCache(service, sport_type_id, ref lines);
            }
            else
            {
                if (sports_events.Count == 0)
                {
                    sports_events = Cache.BtoBet.GetHighLightsEventsWithLeagueIDFromCache(service, sport_type_id, selected_league_id, ref lines);
                }
            }


            if (sports_events != null)
            {
                List<SportEvents> filtered_sports_events = sports_events.Where(x => x.page_number == page_number).ToList();
                if (filtered_sports_events != null)
                {
                    //menu = "Choisir un jeu:" + Environment.NewLine;
                    int counter = 1;
                    foreach (SportEvents se in filtered_sports_events)
                    {
                        menu = menu + counter + ") " + se.home_team + " Vs " + se.away_team + Environment.NewLine;
                        counter = counter + 1;
                    }
                    int total_pages = (sports_events.Count() / 3) + (sports_events.Count() % 3 == 0 ? 0 : 1);
                    if (page_number == 1 && total_pages != page_number)
                    {
                        menu = menu + Environment.NewLine + "#) Suivant";
                    }
                    if (page_number > 1 && page_number < total_pages)
                    {
                        menu = menu + Environment.NewLine + "#) Suivant" + Environment.NewLine;
                        menu = menu + "*) Retour ";
                    }
                    if (page_number == total_pages && page_number > 1)
                    {
                        menu = menu + Environment.NewLine + "*) Retour";
                    }
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";

                }
            }


            return menu;
        }


        public static string GetGameOddsMenu(ServiceClass service, int sport_type_id, int page_number, string ussd_string, Int64 game_id, int odd_page, out Int64 out_game_id, out Int64 out_event_id, int selected_league_id, ref List<LogLines> lines)
        {
            string menu = "";
            out_game_id = 0;
            out_event_id = 0;
            SportEvents game = null;
            List<SportEvents> sports_events = null;
            if (selected_league_id == 0)
            {
                sports_events = Cache.BtoBet.GetEventsFromCache(service, sport_type_id, ref lines);
            }
            else
            {
                sports_events = Cache.BtoBet.GetEventsWithLeagueIDFromCache(service, sport_type_id, selected_league_id, ref lines);
                if (sports_events.Count == 0)
                {
                    sports_events = Cache.BtoBet.GetHighLightsEventsWithLeagueIDFromCache(service, sport_type_id, selected_league_id, ref lines);
                }
            }



            if (sports_events != null && game_id == 0)
            {
                List<SportEvents> filtered_sports_events = sports_events.Where(x => x.page_number == page_number).ToList();
                if (filtered_sports_events != null)
                {
                    game = new SportEvents();
                    game = filtered_sports_events[Convert.ToInt32(ussd_string) - 1];
                }
            }
            if (game_id > 0 && sports_events != null)
            {
                game = new SportEvents();
                game = sports_events.Find(x => x.game_id == game_id);
            }

            if (game != null)
            {
                game = CheckGameOdds(service, game, ref lines);
                out_game_id = game.game_id;
                out_event_id = game.event_id;
                menu = game.home_team + " Vs " + game.away_team + Environment.NewLine;
                string game_time = Convert.ToDateTime(game.game_time).ToString("MMMM", CultureInfo.CreateSpecificCulture("fr")) + " " + Convert.ToDateTime(game.game_time).ToString("dd") + ", " + Convert.ToDateTime(game.game_time).ToString("HH:mm");
                menu = menu + game_time + Environment.NewLine;
                int odd_count = game.event_odds.Count();
                List<EventOdds> filtred_odd = new List<EventOdds>();
                int counter = 1;
                string odd_name = "";
                int bts_id = GetBTSID(odd_page, sport_type_id, out odd_name);
                lines = Add2Log(lines, " odd_count = " + odd_count, 100, "ivr_subscribe");
                switch (odd_page)
                {
                    case 0://FT
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + odd_name + (sport_type_id == 1 ? "-" + eo.ck_name : "") + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count > 4)
                        {
                            menu = menu + Environment.NewLine + "#) Suivant";
                        }
                        break;
                    case 1://UO
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + (eo.ck_name.ToLower() == "ov25" ? "O " : "U ") + "2.5 (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count >= 8)
                        {
                            menu = menu + Environment.NewLine + "#) Suivant" + Environment.NewLine;
                        }
                        menu = menu + "*) Retour";
                        break;
                    case 2://DC
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + eo.ck_name + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        menu = menu + Environment.NewLine + "*) Retour";
                        break;
                }
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }

            return menu;
        }

        public static int GetBTSID(int odd_page, int sport_type_id, out string odd_game)
        {
            int bts_id = 0;
            odd_game = "";
            switch (sport_type_id)
            {
                case 1:
                    bts_id = (odd_page == 0 ? 3 : (odd_page == 1 ? 29 : 17));
                    odd_game = (odd_page == 0 ? "FT" : "");
                    break;
                case 2:
                    bts_id = 4;
                    odd_game = (odd_page == 0 ? "2 Way" : "");
                    break;
                case 5:
                    bts_id = 4;
                    odd_game = (odd_page == 0 ? "2 Way" : "");
                    break;
            }
            return bts_id;
        }

        public static SportEvents CheckGameOdds(ServiceClass service, SportEvents game, ref List<LogLines> lines)
        {
            List<EventOdds> events = Cache.BtoBet.GetEventOddFromCache(service, game.game_id.ToString(), ref lines);
            if (events != null)
            {
                game.event_odds = events;
            }
            return game;
        }

        public static string GetHighlightGameOddsMenu(ServiceClass service, int sport_type_id, int page_number, string ussd_string, Int64 game_id, int odd_page, out Int64 out_game_id, out Int64 out_event_id, int selected_league_id, ref List<LogLines> lines)
        {
            string menu = "";
            out_game_id = 0;
            out_event_id = 0;
            SportEvents game = null;
            List<SportEvents> sports_events = null;
            if (selected_league_id == 0)
            {
                sports_events = Cache.BtoBet.GetHighLightsEventsFromCache(service, sport_type_id, ref lines);
            }
            else
            {
                sports_events = Cache.BtoBet.GetEventsWithLeagueIDFromCache(service, selected_league_id, sport_type_id, ref lines);
            }




            if (sports_events != null && game_id == 0)
            {
                List<SportEvents> filtered_sports_events = sports_events.Where(x => x.page_number == page_number).ToList();
                if (filtered_sports_events != null)
                {
                    game = new SportEvents();
                    game = filtered_sports_events[Convert.ToInt32(ussd_string) - 1];
                }
            }
            if (game_id > 0 && sports_events != null)
            {
                game = new SportEvents();
                game = sports_events.Find(x => x.game_id == game_id);
            }

            if (game != null)
            {
                game = CheckGameOdds(service, game, ref lines);
                out_game_id = game.game_id;
                out_event_id = game.event_id;
                menu = game.home_team + " Vs " + game.away_team + Environment.NewLine;
                string game_time = Convert.ToDateTime(game.game_time).ToString("MMMM", CultureInfo.CreateSpecificCulture("fr")) + " " + Convert.ToDateTime(game.game_time).ToString("dd") + ", " + Convert.ToDateTime(game.game_time).ToString("HH:mm");
                menu = menu + game_time + Environment.NewLine;
                int odd_count = game.event_odds.Count();
                List<EventOdds> filtred_odd = new List<EventOdds>();
                int counter = 1;
                string odd_name = "";
                int bts_id = GetBTSID(odd_page, sport_type_id, out odd_name);
                lines = Add2Log(lines, " odd_count = " + odd_count, 100, "ivr_subscribe");
                switch (odd_page)
                {
                    case 0://FT
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + odd_name + (sport_type_id == 1 ? "-" + eo.ck_name : "") + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count > 4)
                        {
                            menu = menu + Environment.NewLine + "#) Suivant";
                        }
                        break;
                    case 1://UO
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + (eo.ck_name.ToLower() == "ov25" ? "O " : "U ") + "2.5 (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count >= 8)
                        {
                            menu = menu + Environment.NewLine + "#) Suivant" + Environment.NewLine;
                        }
                        menu = menu + "*) Retour";
                        break;
                    case 2://DC
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + eo.ck_name + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        menu = menu + Environment.NewLine + "*) Retour";
                        break;
                }
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }

            return menu;
        }

        public static string GetConfirmMenu(ServiceClass service, int sport_type_id, Int64 game_id, string ussd_string, int odd_page, out double selected_odd, out int selected_bet_type_id, out string selected_odd_name, out string selected_odd_line, ref List<LogLines> lines)
        {
            string menu = "";
            selected_odd = 0;
            selected_bet_type_id = 0;
            selected_odd_name = "";
            selected_odd_line = "";
            List<SportEvents> sports_events = Cache.BtoBet.GetEventsFromCache(service, sport_type_id, ref lines);
            SportEvents game = new SportEvents();
            game = sports_events.Find(x => x.game_id == game_id);
            if (game == null)
            {
                sports_events = Cache.BtoBet.GetHighLightsEventsFromCache(service, sport_type_id, ref lines);
                game = sports_events.Find(x => x.game_id == game_id);
            }
            if (game != null)
            {
                game = CheckGameOdds(service, game, ref lines);
                try
                {
                    menu = "Confirmer:" + Environment.NewLine;
                    menu = menu + game.home_team + " Vs " + game.away_team + Environment.NewLine;
                    List<EventOdds> filtred_odds = new List<EventOdds>();
                    EventOdds filtred_odd = new EventOdds();
                    string odd_name = "";
                    int bts_id = GetBTSID(odd_page, sport_type_id, out odd_name);
                    switch (odd_page)
                    {
                        case 0://FT
                            filtred_odds = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                            filtred_odd = filtred_odds[Convert.ToInt32(ussd_string) - 1];
                            menu = menu + " " + odd_name + "-" + filtred_odd.ck_name + " (" + filtred_odd.ck_price + ")" + Environment.NewLine;
                            break;
                        case 1://UO
                            filtred_odds = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                            filtred_odd = filtred_odds[Convert.ToInt32(ussd_string) - 1];
                            menu = menu + (filtred_odd.ck_name.ToLower() == "ov25" ? "O " : "U ") + "2.5 (" + filtred_odd.ck_price + ")" + Environment.NewLine;
                            break;
                        case 2://DC
                            filtred_odds = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                            filtred_odd = filtred_odds[Convert.ToInt32(ussd_string) - 1];
                            menu = menu + filtred_odd.ck_name + " (" + filtred_odd.ck_price + ")" + Environment.NewLine;
                            break;
                    }
                    selected_odd = Convert.ToDouble(filtred_odd.ck_price);
                    selected_bet_type_id = Convert.ToInt32(filtred_odd.bts_id);
                    selected_odd_name = filtred_odd.ck_name;
                    selected_odd_line = filtred_odd.line;

                    menu = menu + Environment.NewLine + "1) Ajouter un Jeu" + Environment.NewLine;
                    menu = menu + "2) Confirmer & Payer" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "*) Retour";
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, " exception = " + e.ToString(), 100, "ivr_subscribe");
                }

            }
            return menu;
        }


    }
}