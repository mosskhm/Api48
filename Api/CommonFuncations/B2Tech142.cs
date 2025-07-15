using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class B2Tech142
    {
        public class APISettings
        {
            public string username { get; set; }
            public string password { get; set; }
            public string security_terminal_token { get; set; }
            public string token { get; set; }
            public string token_exp { get; set; }
            public string general_terminal_token { get; set; }
            public string event_base_url { get; set; }
            public string placebet_base_url { get; set; }
            public string executeorder_base_url { get; set; }
            public string execute_base_url { get; set; }
            public string channel_id { get; set; }
            public string brand_id { get; set; }
            public string tbl_prefix { get; set; }

        }
        public class iDoBetLoginUser
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string userName { get; set; }
            public string password { get; set; }
        }

        public static string GetTokenNew(string username, string password, string ChannelId, string BrandId, string url, bool isForce, ref List<LogLines> lines)
        {
            string token_id = "";

            if (isForce == false)
            {
                if (HttpContext.Current.Application["GetToken"+username+"_"+BrandId] != null)
                {
                    DateTime expdate = (DateTime)HttpContext.Current.Application["GetToken"+username+"_"+BrandId+"_expdate"];
                    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    if (DateTime.Now < expdate)
                    {
                        token_id = (string)HttpContext.Current.Application["GetToken" + username + "_" + BrandId];
                    }
                }
            }

            if (String.IsNullOrEmpty(token_id))
            {
                iDoBetLoginUser request = new iDoBetLoginUser()
                {
                    methodGroup = "Sport",
                    methodName = "LoginUser",
                    userName = username,
                    password = password
                };

                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = BrandId });
                headers.Add(new Headers { key = "ChannelId", value = ChannelId });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            token_id = json_response.data.token;
                            HttpContext.Current.Application["GetToken" + username + "_" + BrandId] = token_id;
                            HttpContext.Current.Application["GetToken" + username + "_" + BrandId + "_expdate"] = DateTime.Now.AddHours(10);
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }

                }
            }
            
            return token_id;
        }

        public class SportEvents
        {
            public Int64 game_id { get; set; }
            public string home_team { get; set; }
            public string away_team { get; set; }
            public string game_time { get; set; }
            public int page_number { get; set; }
            public List<EventOdds> event_odds { get; set; }
            public string league_name { get; set; }
            public string leagueId { get; set; }
            public string country { get; set; }
            public string sport_id { get; set; }
            public string sport_name { get; set; }
        }

        public class EventOdds
        {
            public Int64 bts_id { get; set; }
            public string ck_name { get; set; }
            public string ck_price { get; set; }
            public string line { get; set; }
        }
        public static List<SportEvents> GetEvents(APISettings api_settings, bool isForce, string sportTypeIds, string leagueIds, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;
            
            if (isForce == false)
            {
                if (HttpContext.Current.Application["GetEvents" + api_settings.channel_id + "_" + api_settings.brand_id + "_" + sports_events + "_" + leagueIds] != null)
                {
                    DateTime expdate = (DateTime)HttpContext.Current.Application["GetEvents" + api_settings.channel_id + "_" + api_settings.brand_id + "_" + sports_events + "_" + leagueIds +"_expdate"];
                    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    if (DateTime.Now < expdate)
                    {
                        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetEvents" + api_settings.channel_id + "_" + api_settings.brand_id + "_" + sports_events + "_" + leagueIds];
                    }
                }
            }

            if (sports_events == null)
            {
                string mydate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
                string final_url = api_settings.event_base_url + "?betTypeIds=310310&statusId=0&eventTypeId=0&sportTypeIds=" + sportTypeIds + "&toDate=" + mydate + "T00:00:00.000Z&betTypeIds=346310&betTypeIds=356310" + (leagueIds == "0" ? "" : "&leagueIds=" + leagueIds);

                lines = Add2Log(lines, " get_events_url = " + final_url, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "Terminal", value = api_settings.general_terminal_token });

                string response = CallSoap.GetURLWithHeader(final_url, headers, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.data)
                    {
                        string game_id = item.id;
                        string home_team = item.h;
                        string away_team = item.a;
                        string game_time = item.gt;
                        game_time = Convert.ToDateTime(game_time.Replace("T", " ").Replace("Z", "")).ToString("yyyy-MM-dd HH:mm:ss");
                        home_team = home_team.Replace("&", "&amp;");
                        away_team = away_team.Replace("&", "&amp;");
                        string league_name = item.ln;
                        string league_id = item.lid;
                        string sport_name = item.sn;
                        string sport_id = item.sid;
                        if (counter % 3 == 0)
                        {
                            page_number = page_number + 1;
                        }
                        foreach (var bts in item.bts)
                        {
                            bts_id = bts.id;
                            line = "";
                            foreach (var odds in bts.odds)
                            {
                                string odd_name = odds.n;
                                string odd_price = odds.p;
                                event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                            }
                        }
                        sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_odds = event_odds, sport_id = sport_id, sport_name = sport_name });
                        event_odds = new List<EventOdds>();
                        counter = counter + 1;

                    }
                    HttpContext.Current.Application["GetEvents" + api_settings.channel_id + "_" + api_settings.brand_id + "_" + sports_events + "_" + leagueIds] = sports_events;
                    HttpContext.Current.Application["GetEvents" + api_settings.channel_id + "_" + api_settings.brand_id + "_" + sports_events + "_" + leagueIds + "_expdate"] = DateTime.Now.AddMinutes(2);
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }
            }
            

            
            return sports_events;
        }

        public class Tickets
        {
            public string barcode { get; set; }
            public string msisdn { get; set; }
            public double payout { get; set; }
            public double amount { get; set; }
            public string creation_time { get; set; }
            public string win_status { get; set; }
            public string order_status { get; set; }
            public int page_number { get; set; }
            public double max_payout { get; set; }
            public string order_number { get; set; }
            public string bonus { get; set; }
            public string total_selections { get; set; }
            public string product_name { get; set; }
            public string user_id { get; set; }
            public string branch_id { get; set; }
            public string creation_time1 { get; set; }
            public string externalTransactionId { get; set; }

        }

        public class GetOrderRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string fromDate { get; set; }
            public string toDate { get; set; }
            public string barcodes { get; set; }
            public string winStatusIds { get; set; }
            public string externalIds { get; set; }
            public int orderStatusId { get; set; }
            public int skip { get; set; }
            public int take { get; set; }
            public string sortBy { get; set; }
            public bool sortDesc { get; set; }
            public string UserID { get; set; }
        }

        public static List<Tickets> GetTicket(APISettings api_settings, string MSISDN, string barcode, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;

            string idobet_endpoint = api_settings.execute_base_url;
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
            GetOrderRequest request = new GetOrderRequest();

            if (MSISDN != "0")
            {
                request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    externalIds = MSISDN
                };
            }
            if (barcode != "0")
            {
                request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    barcodes = barcode
                };
            }

                
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            string postBody = JsonConvert.SerializeObject(request, settings);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
            headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
            headers.Add(new Headers { key = "language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = api_settings.security_terminal_token });
            headers.Add(new Headers { key = "Token", value = api_settings.token });

            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    tickets = new List<Tickets>();
                    int page_number = 0;
                    int counter = 0;
                    foreach (var item in json_response.data.orders)
                    {
                        string msisdn = item.externalId;
                        string o_barcode = item.barcode;
                        double maxpayout = item.maxPayout;
                        double payout = item.payout;
                        string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                        creation_time = Convert.ToDateTime(creation_time).AddHours(0).ToString("dd/MM HH:mm");


                        string creation_time1 = Convert.ToDateTime(item.createTime).ToString("dd/MM/yy HH:mm");

                        double amount = item.amount;
                        string win_status = item.winStatusId;
                        string order_status = item.status;
                        string total_selections = item.totalSelections;
                        string product_name = item.productName;
                        string user_id = item.userId;
                        string branch_id = item.branchId;
                        string order_number = item.orderNumber;

                        double bonus = 0;
                        try
                        {
                            bonus = item.Bonus;
                        }
                        catch (Exception ex)
                        {
                            bonus = 0;
                        }

                        if (counter % 3 == 0)
                        {
                            page_number = page_number + 1;
                        }

                        tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, bonus = bonus.ToString(), total_selections = total_selections, product_name = product_name, branch_id = branch_id, user_id = user_id, order_number = order_number, creation_time1 = creation_time1 });

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
            



            return tickets;
        }

        public class IdoBetUser
        {
            public int id { get; set; }
            public int iPin { get; set; }
            public double availableCredit { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public bool isValid { get; set; }
            public double AvailableForWithdraw { get; set; }
            public string msgTransactionFee {  get; set; }      // optional content to send to user for their transaction fee
            public string Email { get; set; }
            public bool isLocked { get; set; }
            public string msisdn { get; set; }
            public string RegistrationDate { get; set; }
            public string UtmSource { get; set; }
            public string UtmMedium { get; set; }
            public string UtmCampaign { get; set; }
            public string PromoCode { get; set; }
            public string Bonus { get; set; }
            public string PendingWithdraws { get; set; }
        }

        public class DirectDepositRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public int userId { get; set; }
            public double amount { get; set; }
            public string phone { get; set; }
            public string externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
        }

        public static bool Deposit(APISettings api_settings, IdoBetUser user, string dya_id, string time_stamp, double amount, string msisdn, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = api_settings.token;
            response_body = "";
            postBody = "";
            if (!String.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = api_settings.execute_base_url;
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                DirectDepositRequest request = new DirectDepositRequest()
                {
                    methodGroup = "Sport",
                    methodName = "DirectDeposit",
                    userId = user.id,
                    amount = amount,
                    phone = "+" + msisdn,
                    externalTransactionId = dya_id,
                    transactionExtraData = "{\"Timestamp\":\"" + time_stamp + "\"}"
                };
                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = api_settings.security_terminal_token });

                response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            string trans_id = json_response.data.transactionId;
                            if (!String.IsNullOrEmpty(trans_id))
                            {
                                result = true;
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values(" + dya_id + "," + trans_id + ")", ref lines);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
            }
            return result;
        }

        public class WithdrawRequestNew
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public double amount { get; set; }
            public int userId { get; set; }
            public string ipin { get; set; }
        }

        public class EndWithdrawRequestNew
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string transferCode { get; set; }
            public double amount { get; set; }
            public int userId { get; set; }
            public bool isApprove { get; set; }
            public string comments { get; set; }
            public string externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
        }

        public static string StartWithdraw(APISettings api_settings, IdoBetUser user, string msisdn, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            string result = "";
            postBody = "";
            response_body = "";
            string token_id = api_settings.token;
            if (!String.IsNullOrEmpty(token_id))
            {
                WithdrawRequestNew request = new WithdrawRequestNew()
                {
                    methodGroup = "Sport",
                    methodName = "BeginWithdraw",
                    amount = amount,
                    userId = user.id,
                    ipin = user.iPin.ToString()
                };
                string idobet_endpoint = api_settings.execute_base_url;
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = api_settings.general_terminal_token });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            result = json_response.data.transferCode;
                        }

                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
            }
            return result;
        }

        public static bool EndWithdraw(APISettings api_settings, string time_stamp, string dya_id, bool isApprove, string transferCode, IdoBetUser user, string msisdn, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            postBody = "";
            response_body = "";
            string token_id = api_settings.token;
            if (!String.IsNullOrEmpty(token_id))
            {
                EndWithdrawRequestNew request = new EndWithdrawRequestNew()
                {
                    methodGroup = "Sport",
                    methodName = "EndWithdraw",
                    amount = amount,
                    userId = user.id,
                    transferCode = transferCode,
                    isApprove = isApprove,
                    comments = "",
                    externalTransactionId = dya_id,
                    transactionExtraData = "{\"Timestamp\":\"" + time_stamp + "\"}",
                };
                string idobet_endpoint = api_settings.execute_base_url;
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = api_settings.general_terminal_token });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            result = true;
                            string trans_id = json_response.data.transactionId;
                            if (!String.IsNullOrEmpty(trans_id))
                            {
                                result = true;
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values(" + dya_id + "," + trans_id + ")", ref lines);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
            }
            return result;
        }

        public class SavedGames
        {
            public double selected_odd { get; set; }
            public string msisdn { get; set; }
            public int amount { get; set; }
            public string game_id { get; set; }
            public int selected_bet_type_id { get; set; }
            public string selected_odd_name { get; set; }
            public string selected_odd_line { get; set; }
            public string time_out_guid { get; set; }
            public string event_id { get; set; }
            public string selected_ussd_string { get; set; }
        }

        public class USSDSession
        {
            public Int64 session_id { get; set; }
            public int odd_page { get; set; }
            public Int64 game_id { get; set; }
            public string ussd_string { get; set; }
            public int action_id { get; set; }
            public int page_number { get; set; }
            public int topic_id { get; set; }
            public string user_seesion_id { get; set; }
            public double selected_odd { get; set; }
            public int selected_bet_type_id { get; set; }
            public string selected_odd_name { get; set; }
            public string selected_odd_line { get; set; }
            public string amount { get; set; }
            public int selected_league_id { get; set; }
            public int amount_2_pay { get; set; }
            public string bar_code { get; set; }
            public string selected_subagent_name { get; set; }
            public Int64 event_id { get; set; }
            public string rapidos_string { get; set; }

        }

        public class PlaceBetRequest
        {
            public List<iDoBet.rows> rows { get; set; }
            public List<iDoBet.selections> selections { get; set; }
            public string ExternalId { get; set; }
        }
        public static bool PlaceBet(APISettings api_settings,USSDSession ussd_session, ref List<LogLines> lines)
        {
            bool result = false;
            List<SavedGames> saved_games = Api.DataLayer.DBQueries.GetiDoBetSavedGames142(ussd_session.user_seesion_id, api_settings.tbl_prefix, ref lines);
            if (saved_games != null)
            {
                List<iDoBet.rows> rows = new List<iDoBet.rows>();
                List<iDoBet.selections> selections = new List<iDoBet.selections>();
                string[] selection_keys = new string[saved_games.Count()];
                string token_id = api_settings.token;
                string terminal_security_token = api_settings.security_terminal_token;

                int game_count = 0;
                foreach (SavedGames r in saved_games)
                {
                    selection_keys[game_count] = r.game_id;
                    game_count = game_count + 1;
                }

                rows.Add(new iDoBet.rows { amount = saved_games[0].amount.ToString(), selectionKeys = selection_keys });

                foreach (SavedGames r in saved_games)
                {

                    selections.Add(new iDoBet.selections { eventId = Convert.ToInt64(r.game_id), betTypeId = r.selected_bet_type_id, oddName = r.selected_odd_name, oddLine = r.selected_odd_line, oddPrice = r.selected_odd.ToString(), key = r.game_id });
                }


                PlaceBetRequest request = new PlaceBetRequest()
                {

                    ExternalId = saved_games[0].msisdn,
                    rows = rows,
                    selections = selections,

                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");

                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = api_settings.security_terminal_token });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = api_settings.placebet_base_url;
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.timeoutGuid != null)
                    {
                        result = true;
                        List<Int64> saved_ids = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(ussd_session.user_seesion_id, api_settings.tbl_prefix, ref lines);
                        if (saved_ids != null)
                        {
                            string mysaved_id = "";
                            foreach (Int64 s in saved_ids)
                            {
                                mysaved_id = mysaved_id + s + ",";

                            }
                            if (!String.IsNullOrEmpty(mysaved_id))
                            {
                                mysaved_id = mysaved_id.Substring(0, mysaved_id.Length - 1);
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games"+api_settings.tbl_prefix+" set time_out_guid = '" + json_response.timeoutGuid + "', `status` = 1 where id in (" + mysaved_id + ")", "DBConnectionString_104", ref lines);
                            }

                            //foreach (Int64 s in saved_ids)
                            //{
                            //    DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set time_out_guid = '" + json_response.timeoutGuid + "', `status` = 1 where id = " + s, ref lines);
                            //}
                        }

                        //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set time_out_guid = '" + json_response.timeoutGuid + "', `status` = 1 where user_session_id = '" + ussd_session.user_seesion_id + "'", ref lines);
                    }
                    else
                    {
                        try
                        {
                            bool cont = true;
                            foreach (var a in json_response.validationResult[0].orderBetValidationResults)
                            {
                                if (a.validationResult == 241)
                                {
                                    lines = Add2Log(lines, " **** 241 ****", 100, "ivr_subscribe");
                                    Int64 event_id = a.@event.eventId;
                                    string bet_type = a.@event.betTypeId;
                                    string odd_name = a.@event.oddName;
                                    string odd_price = a.@event.oddPrice;
                                    List<Int64> saved_ids = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(ussd_session.user_seesion_id, api_settings.tbl_prefix, ref lines);
                                    if (saved_ids != null)
                                    {
                                        foreach (Int64 s in saved_ids)
                                        {
                                            Int64 myevent_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select game_id from ussd_saved_games"+api_settings.tbl_prefix+" u where u.id = " + s, "DBConnectionString_104", ref lines);
                                            if (event_id == myevent_id)
                                            {
                                                Api.DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games"+api_settings.tbl_prefix+" set selected_odd = " + odd_price + " where id = " + s, "DBConnectionString_104", ref lines);
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    result = false;
                                    cont = false;
                                }
                            }
                            if (cont)
                            {
                                result = PlaceBet(api_settings, ussd_session, ref lines);
                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception handeling 241 " + ex.ToString(), 100, "ivr_subscribe");
                        }
                        //check if error is 241
                    }
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
                }
            }
            return result;
        }

        public class ExecuteOrderDetails
        {
            public string order_number { get; set; }
            public string barcode { get; set; }
            public string amount { get; set; }
            public string max_payout { get; set; }
            public string idobet_transid { get; set; }
            public string total_bonus { get; set; }
        }

        public class DYATransactions
        {
            public Int64 dya_trans { get; set; }
            public Int64 msisdn { get; set; }
            public int service_id { get; set; }
            public int amount { get; set; }
            public int dya_method { get; set; }
            public string partner_transid { get; set; }
            public string airtime_tokenid { get; set; }
            public string datetime { get; set; }
            public int already_updated { get; set; }
        }

        public class ExecuteOrderRequest
        {
            public string timeoutGuid { get; set; }
            public int externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
        }

        public static ExecuteOrderDetails GetExecuteOrderNew(APISettings api_settings,DYATransactions dya_trans, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            ExecuteOrderDetails result = null;
            string barcode = "", order_number = "", amount = "", idobet_transid = "", total_bonus = "0";
            double max_p = 0;
            postBody = "";
            response_body = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames142(dya_trans.partner_transid, api_settings.tbl_prefix, ref lines);
            string token_id = api_settings.token;
            if (saved_games != null)
            {
                amount = saved_games[0].amount.ToString();


                double total_ratio = 1;
                foreach (SavedGames r in saved_games)
                {
                    total_ratio = total_ratio * r.selected_odd;
                }
                total_ratio = Math.Round(total_ratio, 2);
                max_p = Math.Round(total_ratio * Convert.ToDouble(amount), 2);

                ExecuteOrderRequest request = new ExecuteOrderRequest()
                {
                    timeoutGuid = saved_games[0].time_out_guid,
                    externalTransactionId = Convert.ToInt32(dya_trans.dya_trans),
                    transactionExtraData = "{\"Timestamp\":\"" + dya_trans.datetime + "\"}"

                };
                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = api_settings.brand_id });
                headers.Add(new Headers { key = "ChannelId", value = api_settings.channel_id });
                headers.Add(new Headers { key = "language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = api_settings.security_terminal_token });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string request_execute_order_url = api_settings.executeorder_base_url;
                lines = Add2Log(lines, " Sending to url " + request_execute_order_url, 100, "ivr_subscribe");
                response_body = CommonFuncations.CallSoap.CallSoapRequest(request_execute_order_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.barcode != null && json_response.barcode != "null" && json_response.isSuccessfull == true)
                    {
                        barcode = json_response.barcode;
                        order_number = json_response.orderNumber;
                        idobet_transid = json_response.transactionId;
                        total_bonus = json_response.totalBonus;

                        result = new ExecuteOrderDetails()
                        {
                            order_number = order_number,
                            barcode = barcode,
                            max_payout = max_p.ToString(),
                            amount = amount,
                            idobet_transid = idobet_transid,
                            total_bonus = total_bonus

                        };
                        List<Int64> saved_ids = Api.DataLayer.DBQueries.GetUSSDSavedGamesID(dya_trans.partner_transid, api_settings.tbl_prefix, ref lines);
                        if (saved_ids != null)
                        {
                            string mysaved_id = "";
                            foreach (Int64 s in saved_ids)
                            {
                                mysaved_id = mysaved_id + s + ",";

                            }
                            if (!String.IsNullOrEmpty(mysaved_id))
                            {
                                mysaved_id = mysaved_id.Substring(0, mysaved_id.Length - 1);
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games"+api_settings.tbl_prefix+" set barcode = '" + barcode + "', `status` = 2 where id in (" + mysaved_id + ")", "DBConnectionString_104", ref lines);
                            }

                            //foreach (Int64 s in saved_ids)
                            //{
                            //    DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                            //}
                        }
                        DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values(" + dya_trans.dya_trans + "," + idobet_transid + ")", ref lines);
                    }
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
                }
            }


            return result;
        }

        public static string GetCloseBet(APISettings api_settings,USSDSession ussd_session, string amount, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames142(ussd_session.user_seesion_id, api_settings.tbl_prefix, ref lines);
            if (saved_games != null)
            {
                double total_ratio = 1;
                foreach (SavedGames r in saved_games)
                {
                    total_ratio = total_ratio * r.selected_odd;
                }
                total_ratio = Math.Round(total_ratio, 2);
            }
            menu = menu + "Pour completer votre pari, confirmez le paiement." + Environment.NewLine;
            menu = menu + "Confirmez T&amp;C et +18 http://yellowbet.com.gn" + Environment.NewLine;
            return menu;
        }

        public static string GetWrongPriceBetRapidos(int max_bet, ref List<LogLines> lines)
        {
            string menu = "Veuillez saisir un montant compris entre 1200 GNF et " + max_bet + " GNF" + Environment.NewLine + Environment.NewLine;
            menu = menu + "*) Retour";
            return menu;
        }

    }
}