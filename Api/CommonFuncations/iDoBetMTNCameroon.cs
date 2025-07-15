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
    public class iDoBetMTNCameroon
    {
        public static string CheckFreeBet(USSDSession ussd_session, string msisdn, out bool placebet, out USSDBonus ub, ref List<LogLines> lines)
        {
            string menu = "Mauvais choix";
            placebet = false;
            ub = null;
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                List<USSDBonus> ussd_b = GetUSSDBonusPerService("720", ref lines);
                if (ussd_b != null)
                {
                    ub = ussd_b.Find(x => x.msisdn == msisdn);
                    if (ub != null)
                    {
                        if (saved_games.Count() >= ub.min_selections)
                        {
                            double total_ratio = 1;
                            bool min_selection = true;
                            foreach (SavedGames r in saved_games)
                            {
                                if (r.selected_odd < ub.min_odd_per_selections)
                                {
                                    min_selection = false;
                                    break;
                                }
                                total_ratio = total_ratio * r.selected_odd;
                            }
                            if (total_ratio >= ub.min_total_odd && min_selection)
                            {
                                placebet = true;
                                menu = "";
                            }
                        }
                    }
                }
            }
            return menu;
        }

        public static string GetUSSDBonusCloseBet(ExecuteOrderDetails barcode, ref List<LogLines> lines)
        {
            string menu = "";
            if (barcode != null)
            {
                if (!String.IsNullOrEmpty(barcode.barcode))
                {
                    menu = menu + "Votre pari gratuit a ete approuve" + Environment.NewLine;
                }

            }
            return menu;
        }

        public static List<Tickets> CheckTicketsByUserIDNew(string idobetuser_id, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    UserID = idobetuser_id
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

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
                            string order_number = item.orderNumber;
                            string barcode = item.barcode;
                            double maxpayout = item.maxPayout;
                            double payout = item.payout;
                            string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM/yy HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;
                            string total_selections = item.totalSelections;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, order_number = order_number, total_selections = total_selections });

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

        public static List<IdoBetUser> GetAllUsersByBranch(string branch_id, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            List<IdoBetUser> users = null;
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"BranchIds\": \"+" + branch_id + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        //{"ID":992889,"RegistrationDate":"2021-07-15T00:00:00","IsCustomer":true,"UserName":"669687293","Email":"",
                        //"FirstName":"Mag","LastName":"USSD 669","Phone":"224669687293","CreditLimit":null,"AvailableCredit":0.0,"IdentityCard":null,
                        //"IPin":3854,"IsValid":true,"IsLocked":false,"IsFullRegistrationDetails":true,"CurrencyCode":"GNF","AvailableForWithdraw":0.0,
                        //"PendingWithdraws":null,"Bonus":0.0,"Commission":0.0,"AcceptNewsLetter":null,
                        //"RegistrationDetails":{"UtmSource":"footsoldiers","UtmMedium":"ussd_footsoldiers","UtmCampaign":"safiatou ketoure 224666948111","PromoCode":null,"ReferenceURL":null}}
                        if (json_response.data.ToString() != "[]")
                        {
                            users = new List<IdoBetUser>();
                            user = new IdoBetUser();
                            SubAgentsResponse res = new SubAgentsResponse();
                            foreach (var item in json_response.data)
                            {
                                double bonus = 0;
                                try
                                {
                                    bonus = item.Bonus;
                                }
                                catch (Exception ex)
                                {
                                    bonus = 0;
                                }
                                double available_for_withdra = (item.AvailableCredit - bonus - item.AvailableForWithdraw) * 0.97 + item.AvailableForWithdraw;

                                string email = Convert.ToString(item.Email) ?? "";
                                string reg_date = Convert.ToString(item.RegistrationDate) ?? "";
                                reg_date = reg_date.Replace("T", "");
                                string utm_source = (String.IsNullOrEmpty(item.RegistrationDetails.UtmSource.ToString()) ? "" : item.RegistrationDetails.UtmSource);
                                string utm_meduim = (String.IsNullOrEmpty(item.RegistrationDetails.UtmMedium.ToString()) ? "" : item.RegistrationDetails.UtmMedium);
                                string utm_campaign = (String.IsNullOrEmpty(item.RegistrationDetails.UtmCampaign.ToString()) ? "" : item.RegistrationDetails.UtmCampaign);
                                string promo_code = (String.IsNullOrEmpty(item.RegistrationDetails.PromoCode.ToString()) ? "" : item.RegistrationDetails.PromoCode);
                                user = new IdoBetUser()
                                {
                                    isValid = item.IsFullRegistrationDetails,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit,
                                    AvailableForWithdraw = available_for_withdra,
                                    isLocked = item.IsLocked,
                                    Email = email,
                                    msisdn = item.Phone,
                                    RegistrationDate = reg_date,
                                    UtmCampaign = utm_campaign,
                                    UtmMedium = utm_meduim,
                                    UtmSource = utm_source,
                                    PromoCode = promo_code

                                };
                                users.Add(user);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
            }
            return users;
        }

        public static List<Tickets> CheckTicketsNewWYear(string MSISDN, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    externalIds = MSISDN
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

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
                            string order_number = item.orderNumber;
                            string barcode = item.barcode;
                            double maxpayout = item.maxPayout;
                            double payout = item.payout;
                            string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM/yy HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;
                            string total_selections = item.totalSelections;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, order_number = order_number, total_selections = total_selections });

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

        public static string GetInvoice(string barcode, ref List<LogLines> lines)
        {

            string url = "";
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetGetOrderKeyUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                string postBody = "{\"barcode\":\"" + barcode + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    try
                    {
                        url = json_response.data;
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
            }



            return url;
        }


        public static IdoBetUser SearchUserByUserID(string idobet_id, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"UserID\": \"+" + idobet_id + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (json_response.data.ToString() != "[]")
                        {
                            user = new IdoBetUser();
                            SubAgentsResponse res = new SubAgentsResponse();
                            foreach (var item in json_response.data)
                            {
                                double bonus = 0;
                                try
                                {
                                    bonus = item.Bonus;
                                }
                                catch (Exception ex)
                                {
                                    bonus = 0;
                                }
                                double available_for_withdra = (item.AvailableCredit - bonus - item.AvailableForWithdraw) * 0.97 + item.AvailableForWithdraw;

                                string email = Convert.ToString(item.Email) ?? "";
                                string reg_date = Convert.ToString(item.RegistrationDate) ?? "";
                                reg_date = reg_date.Replace("T", "");
                                user = new IdoBetUser()
                                {
                                    isValid = item.IsFullRegistrationDetails,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit,
                                    AvailableForWithdraw = available_for_withdra,
                                    isLocked = item.IsLocked,
                                    Email = email,
                                    msisdn = item.Phone,
                                    RegistrationDate = reg_date
                                };
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
            }





            return user;
        }

        public static bool ValidateRapidosInput(string ussd_string, int action_id, out String[] strlist, ref List<LogLines> lines)
        {
            bool result = false;
            strlist = new string[] { };
            if (ussd_string.Contains(" "))
            {
                String[] spearator = { " " };
                int count = 2;
                switch (action_id)
                {
                    case 72:
                        count = 2;
                        break;
                    case 73:
                        count = 3;
                        break;
                    case 74:
                        count = 5;
                        break;
                }

                // using the method 
                strlist = ussd_string.Split(spearator, count,
                       StringSplitOptions.RemoveEmptyEntries);
                int counter = 0;
                foreach (string s in strlist)
                {
                    int num1;
                    result = Int32.TryParse(s, out num1);
                    counter = counter + 1;
                    if (!result)
                    {
                        break;
                    }

                }
                if (count != counter)
                {
                    result = false;
                }

            }
            return result;

        }

        public static bool ValidateLottoInput(string ussd_string, int action_id, out String[] strlist, ref List<LogLines> lines)
        {
            bool result = false;
            strlist = new string[] { };
            if (ussd_string.Contains(" "))
            {
                String[] spearator = { " " };
                int count = 5;

                // using the method 
                strlist = ussd_string.Split(spearator, count,
                       StringSplitOptions.RemoveEmptyEntries);
                int counter = 0;
                foreach (string s in strlist)
                {
                    int num1;
                    result = Int32.TryParse(s, out num1);
                    counter = counter + 1;
                    if (!result)
                    {
                        break;
                    }

                }
                if (count != counter)
                {
                    result = false;
                }

            }
            return result;

        }

        public static string WrongRapidosNumber(string lang)
        {
            string menu = "";
            switch (lang)
            {
                case "fr":
                    menu = "Ensemble de nombres invalide. cliquez sur * pour revenir au menu precedent";
                    break;
                case "en":
                    menu = "You have entered invalid numbers. click * to go back.";
                    break;
            }
            
            return menu;
        }

        public static string GetConfirmRapidosMenu(int action_id, string ussd_string, out Int64 game_id, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            game_id = 0;
            menu = (lang == "fr" ? "Confirmer:" : "Confirm:") + Environment.NewLine;
            string min_bet = "100";
            string max_bet = "";
            switch (action_id)
            {
                case 72:
                    menu = menu + "Rapidos 2" + Environment.NewLine;
                    game_id = 46;
                    max_bet = "20000";
                    break;
                case 73:
                    menu = menu + "Rapidos 3" + Environment.NewLine;
                    game_id = 47;
                    max_bet = "4000";
                    break;
                case 74:
                    menu = menu + "Rapidos 5" + Environment.NewLine;
                    game_id = 48;
                    max_bet = "200";
                    break;
            }
            menu = menu + ussd_string;
            switch(lang)
            {
                case "fr":
                    menu = menu + Environment.NewLine + "Veuillez saisir un montant compris entre " + min_bet + " FCFA et " + max_bet + " FCFA:" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "*) Retour ";
                    break;
                case "en":
                    menu = menu + Environment.NewLine + "Enter an amoung between " + min_bet + " FCFA to " + max_bet + " FCFA:" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "*) Back ";
                    break;
            }
            
            return menu;
        }

        public static string GetConfirmLottoMenu(int action_id, string ussd_string, out Int64 game_id, ref List<LogLines> lines)
        {
            string menu = "";
            game_id = 0;
            menu = "Confirmer:" + Environment.NewLine;
            menu = menu + ussd_string;
            menu = menu + Environment.NewLine + "Le cout du billet de loto est de 200 FCFA" + Environment.NewLine;
            menu = menu + Environment.NewLine + "1) Confirmer";
            menu = menu + Environment.NewLine + "*) Retour ";
            return menu;
        }


        public static bool StartDepositNew(IdoBetUser user, string dya_id, string time_stamp, double amount, string msisdn, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            response_body = "";
            postBody = "";
            if (!String.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = "" });

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

        public static string StartWithdrawNew(IdoBetUser user, string msisdn, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            string result = "";
            postBody = "";
            response_body = "";
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
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

        public static bool EndWithdrawNew(string time_stamp, string dya_id, bool isApprove, string transferCode, IdoBetUser user, string msisdn, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            postBody = "";
            response_body = "";
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
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

        public static Int64 CollectSendMoney(string token, string subagent_id, string action, string comments, string amount, ref List<LogLines> lines)
        {
            Int64 trans_id = 0;
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

            CollectSendMoneyRequest request = new CollectSendMoneyRequest()
            {
                MethodGroup = "Sport",
                MethodName = "DirectTransferMoney",
                UserId = Convert.ToInt32(subagent_id),
                Amount = Convert.ToInt32(amount),
                ExternalID = comments,
                TransactionType = action,
                Comments = comments
            };

            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "1" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = token });
            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.data.ToString() != "[]")
                    {
                        string trans_str = json_response.data[0].TransactionId.ToString();
                        Int64 number;

                        bool success = Int64.TryParse(trans_str, out number);
                        trans_id = (success ? number : 0);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }

            }

            return trans_id;
        }

        public static string GetiDoBetTransaction(Int64 dya_id, string token_id, ref List<LogLines> lines)
        {
            string result = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
            iDoBetTransactionRequest request = new iDoBetTransactionRequest()
            {
                methodGroup = "Sport",
                methodName = "GetTrasacions",
                externalTransactionId = dya_id.ToString()
            };
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "9" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = token_id });

            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.isSuccessfull == true)
                    {
                        result = json_response.data.transactionId;
                    }
                    else
                    {
                        string mail_body = "<p><b>GetiDoBetTransaction has failed</b><br> <b>DYAID:</b> " + dya_id + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                        string mail_subject = "GetiDoBetTransaction has Failed - " + dya_id;
                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                        //CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                }
            }

            return result;
        }

        public static string PayCommison(int user_id, int amount, string token_id, string time_stamp, string comments, string external_id, ref List<LogLines> lines)
        {
            string result = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
            PayCommisionRequest request = new PayCommisionRequest()
            {
                methodGroup = "Sport",
                methodName = "PayCommission",
                toUserId = user_id,
                amount = Convert.ToDouble(amount),
                comments = comments,
                externalTransactionId = external_id,
                transactionExtraData = "{\"Timestamp\":\"" + time_stamp + "\"}",
                autoApprove = true
            };
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "9" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = token_id });

            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.isSuccessfull == true)
                    {
                        result = json_response.data[0].transactionId.ToString();
                    }
                    else
                    {
                        string mail_body = "<p><b>Pay Commision has failed</b><br> <b>UserID:</b> " + user_id + "<br><br> <b>Amount:</b> " + amount + "<br><br><b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                        string mail_subject = "Pay Commision has Failed - " + user_id;
                        string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                        string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                        string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                        string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                        int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                        string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                        CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                }
            }

            return result;
        }

        public static bool DoPayOutNew(string time_stamp, string dya_id, string bar_code, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = Api.Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);

            if (!String.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                DoPayOutRequest request = new DoPayOutRequest()
                {
                    methodGroup = "Sport",
                    methodName = "DoPayout",
                    barcode = bar_code,
                    externalTransactionId = dya_id,
                    transactionExtraData = "{\"Timestamp\":\"" + time_stamp + "\"}"
                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = Api.Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCodeMTNCameroon", ref lines) });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
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
                        else
                        {
                            string mail_body = "<p><b>DoPayout Moov has failed for</b><br> <b>Barcode:</b> " + bar_code + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "Warning from DoPayout Moov- " + bar_code;
                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
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

        public static bool TakeMoneyFromSubAgentAndDeposit2MOMO(Int64 dya_id, Int64 pos_trans, ref List<LogLines> lines)
        {
            bool result = false;
            Api.DataLayer.DBQueries.ExecuteQuery("update pos_requests set dya_id = " + dya_id + " where id = " + pos_trans, ref lines);
            GetPosTrans pos_details = GetPosTransaction(pos_trans, ref lines);
            if (pos_details != null)
            {
                List<iDoBetAgents> agents = Cache.iDoBet.GetiDoBetAgents(ref lines);
                if (agents != null)
                {
                    iDoBetAgents agent = agents.Find(x => x.agent_username == pos_details.agent_username);
                    if (agent != null)
                    {
                        string token = GetTokenNew(agent.agent_username, agent.agent_password, ref lines);
                        if (token != null)
                        {
                            List<SubAgentsResponse> sub_agents = GetSubAgents(agent, token, ref lines);
                            if (sub_agents != null)
                            {
                                SubAgentsResponse filtered_sub_agent = sub_agents.Find(x => x.subagent_username == pos_details.subagent_username);
                                if (filtered_sub_agent != null)
                                {
                                    //debit from Agent to SubAgent
                                    Int64 trans_id = CollectSendMoney(token, filtered_sub_agent.subagent_id, "DEBIT", pos_trans.ToString(), pos_details.amount, ref lines);
                                    if (trans_id > 0)
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("update pos_requests set debit_trans_id = " + trans_id + " where id = " + pos_trans, ref lines);
                                        //credit from Agent to MOMO 2030
                                        trans_id = CollectSendMoney(token, "2030", "CREDIT", pos_trans.ToString(), pos_details.amount, ref lines);
                                        if (trans_id > 0)
                                        {
                                            Api.DataLayer.DBQueries.ExecuteQuery("update pos_requests set credit_trans_id = " + trans_id + " where id = " + pos_trans, ref lines);
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


            }
            return result;
        }

        public static IdoBetUser SearchUserNew(string MSISDN, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"Phone\": \"+" + MSISDN + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (json_response.data.ToString() != "[]")
                        {
                            user = new IdoBetUser();
                            SubAgentsResponse res = new SubAgentsResponse();

                            foreach (var item in json_response.data)
                            {
                                double bonus = 0;
                                try
                                {
                                    bonus = item.Bonus;
                                }
                                catch (Exception ex)
                                {
                                    bonus = 0;
                                }
                                double available_for_withdra = (item.AvailableCredit - bonus - item.AvailableForWithdraw) * 0.97 + item.AvailableForWithdraw;

                                string reg_date = Convert.ToString(item.RegistrationDate) ?? "";
                                reg_date = reg_date.Replace("T", "");
                                string email = Convert.ToString(item.Email) ?? "";
                                bool isvalid = item.IsLocked == false ? true : false;
                                user = new IdoBetUser()
                                {
                                    isValid = isvalid,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit,
                                    AvailableForWithdraw = available_for_withdra,
                                    RegistrationDate = reg_date,
                                    Email = email
                                };
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
            }

            return user;
        }

        public static List<Tickets> CheckTicketsNew(string MSISDN, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    externalIds = MSISDN
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

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
                            string barcode = item.barcode;
                            double maxpayout = item.maxPayout;
                            double payout = item.payout;
                            string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(0).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;
                            string bonus = item.bonus;
                            string total_selections = item.totalSelections;
                            string product_name = item.productName;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, bonus = bonus, total_selections = total_selections, product_name = product_name });

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

        public static List<Tickets> GetUnPaidTickets(ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            string token_id = GetTokenNew(ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetUnpaidOrders"
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

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
                            double maxpayout = item.payout;
                            double payout = item.payout;
                            string barcode = item.barcode;
                            //string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            //creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            //double amount = item.amount;
                            string win_status = "2";
                            string order_status = "3";
                            string total_selections = item.totalSelections;
                            string product_name = item.productName;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, total_selections = total_selections, product_name = product_name });

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

        public static List<Tickets> CheckTicketByBarCodeNew(string barcode, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    barcodes = barcode
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

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
                            double maxpayout = item.maxPayout;
                            double payout = item.payout;
                            string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(0).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;
                            string bonus = item.bonus;
                            string total_selections = item.totalSelections;
                            string product_name = item.productName;

                            string externalTransactionId = item.externalTransactionId;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, bonus = bonus, total_selections = total_selections, product_name = product_name, externalTransactionId = externalTransactionId });

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

        public static Tickets CheckTicketByBarCodeNew(string token_id, string barcode, ref List<LogLines> lines)
        {

            Tickets ticket = null;

            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                GetOrderRequest request = new GetOrderRequest()
                {
                    methodGroup = "Sport",
                    methodName = "GetOrders",
                    barcodes = barcode
                };
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                string postBody = JsonConvert.SerializeObject(request, settings);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    try
                    {
                        List<Tickets> tickets = new List<Tickets>();
                        int page_number = 0;
                        int counter = 0;
                        foreach (var item in json_response.data.orders)
                        {
                            string msisdn = item.externalId;
                            double maxpayout = item.maxPayout;
                            double payout = item.payout;
                            string creation_time = item.createTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(0).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;
                            string total_selections = item.totalSelections;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }
                            ticket = new Tickets()
                            {
                                msisdn = msisdn,
                                barcode = barcode,
                                payout = payout,
                                creation_time = creation_time,
                                amount = amount,
                                win_status = win_status,
                                order_status = order_status,
                                page_number = page_number,
                                max_payout = maxpayout,
                                total_selections = total_selections
                            };

                            tickets.Add(ticket);

                            counter = counter + 1;

                        }
                        if (counter == 0)
                        {
                            ticket = null;
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



            return ticket;
        }

        public class PlaceBetRapidosRequest
        {
            public int payment { get; set; }
            public int lotteryTypeId { get; set; }
            public int numberOfDraws { get; set; }
            public int isSpecialJackpot { get; set; }
            public int betTypeId { get; set; }
            public string betNumbers { get; set; }
            public int groupId { get; set; }
            public string externalId { get; set; }
            public bool isExecute { get; set; }
        }

        public class ExecuteBetRapidosRequest
        {
            public string userBetId { get; set; }
            public string requestGuid { get; set; }
            public int employeeId { get; set; }
            public string externalId { get; set; }
            public Int64 externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }

        }

        public static ExecuteOrderDetails PlaceBetRapidos(DYATransactions dya_trans, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            ExecuteOrderDetails result = null;
            string barcode = "", order_number = "", amount = "", idobet_transid = "";
            double max_p = 0;
            postBody = "";
            response_body = "";

            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid.Replace("Rapidos_", ""), ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (saved_games != null)
            {
                amount = saved_games[0].amount.ToString();
                int bet_type_id = 6;
                switch (saved_games[0].game_id)
                {
                    case "46":
                        max_p = dya_trans.amount * 4;
                        break;
                    case "47":
                        max_p = dya_trans.amount * 300;
                        break;
                    case "48":
                        max_p = dya_trans.amount * 25000;
                        break;
                    case "41":
                        max_p = 200000000;
                        bet_type_id = 1;
                        break;
                    case "42":
                        max_p = 400000000;
                        bet_type_id = 1;
                        break;
                }



                PlaceBetRapidosRequest request = new PlaceBetRapidosRequest()
                {
                    payment = dya_trans.amount,
                    lotteryTypeId = Convert.ToInt32(saved_games[0].game_id),
                    numberOfDraws = 1,
                    isSpecialJackpot = 0,
                    betTypeId = bet_type_id,
                    betNumbers = "[{\"n\":\"" + saved_games[0].selected_ussd_string.Replace(" ", ",") + "\"}]",
                    groupId = 0,
                    externalId = dya_trans.msisdn.ToString(),
                    isExecute = false
                };
                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string request_execute_order_url = Cache.ServerSettings.GetServerSettings("iDoBetPlaceBetLottoMTNCameroon", ref lines);
                lines = Add2Log(lines, " Sending to url " + request_execute_order_url, 100, "ivr_subscribe");
                response_body = CommonFuncations.CallSoap.CallSoapRequest(request_execute_order_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.isSuccessfull = true)
                    {
                        string requestGuid = json_response.requestGuid;
                        string lottoUserBetId = json_response.lottoUserBetId;
                        string employeeId = json_response.employeeId;

                        ExecuteBetRapidosRequest request1 = new ExecuteBetRapidosRequest()
                        {
                            employeeId = Convert.ToInt32(employeeId),
                            requestGuid = requestGuid,
                            userBetId = lottoUserBetId,
                            externalId = dya_trans.msisdn.ToString(),
                            externalTransactionId = dya_trans.dya_trans,
                            transactionExtraData = "{\"Timestamp\":\"" + dya_trans.datetime + "\"}"

                        };
                        postBody = JsonConvert.SerializeObject(request1);
                        lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                        request_execute_order_url = Cache.ServerSettings.GetServerSettings("iDoBetExecuteOrderLottoUrlMTNCameroon", ref lines);
                        lines = Add2Log(lines, " Sending to url " + request_execute_order_url, 100, "ivr_subscribe");
                        response_body = CommonFuncations.CallSoap.CallSoapRequest(request_execute_order_url, postBody, headers, 2, true, ref lines);
                        lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                        try
                        {
                            json_response = JsonConvert.DeserializeObject(response_body);
                            if (json_response.isSuccessfull = true)
                            {
                                barcode = json_response.barcode;
                                order_number = json_response.number;
                                idobet_transid = GetiDoBetTransaction(dya_trans.dya_trans, token_id, ref lines);
                                result = new ExecuteOrderDetails()
                                {
                                    order_number = order_number,
                                    barcode = barcode,
                                    max_payout = max_p.ToString(),
                                    amount = amount,
                                    idobet_transid = idobet_transid
                                };
                                List<Int64> saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid.Replace("Rapidos_", ""), ref lines);
                                if (saved_ids != null)
                                {
                                    foreach (Int64 s in saved_ids)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                                    }
                                }
                                if (!String.IsNullOrEmpty(idobet_transid))
                                {
                                    DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values(" + dya_trans.dya_trans + "," + idobet_transid + ")", ref lines);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
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

        public static ExecuteOrderDetails GetExecuteOrderNew(DYATransactions dya_trans, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            ExecuteOrderDetails result = null;
            string barcode = "", order_number = "", amount = "", idobet_transid = "", total_bonus = "0";
            double max_p = 0;
            postBody = "";
            response_body = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid, ref lines);
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string request_execute_order_url = Cache.ServerSettings.GetServerSettings("iDoBetExecuteOrderUrlMTNCameroon", ref lines);
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
                        List<Int64> saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid, ref lines);
                        if (saved_ids != null)
                        {
                            foreach (Int64 s in saved_ids)
                            {
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                            }
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

        public static bool PlaceBet(USSDSession ussd_session, ref List<LogLines> lines)
        {
            bool result = false;
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                List<rows> rows = new List<iDoBet.rows>();
                List<selections> selections = new List<iDoBet.selections>();
                string[] selection_keys = new string[saved_games.Count()];
                string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
                string terminal_security_token = Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCodeMTNCameroon", ref lines);

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
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                lines = Add2Log(lines, " Token " + token_id, 100, "ivr_subscribe");
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetPlaceBetUrlMTNCameroon", ref lines);
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.timeoutGuid != null)
                    {
                        result = true;
                        List<Int64> saved_ids = GetUSSDSavedGamesID(ussd_session.user_seesion_id, ref lines);
                        if (saved_ids != null)
                        {
                            foreach (Int64 s in saved_ids)
                            {
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set time_out_guid = '" + json_response.timeoutGuid + "', `status` = 1 where id = " + s, ref lines);
                            }
                        }

                        //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set time_out_guid = '" + json_response.timeoutGuid + "', `status` = 1 where user_session_id = '" + ussd_session.user_seesion_id + "'", ref lines);
                    }
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
                }
            }
            return result;
        }

        public static List<SportEvents> GetEvents(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string get_events_url = Cache.ServerSettings.GetServerSettings("iDoBetGetEventsUrlMTNCameroon", ref lines) + "sportIds=" + sport_type_id + "&userToken=" + token_id + "&LeagueIds=" + selected_league_id;
                lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(get_events_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.events)
                    {
                        string game_id = item.data.id;
                        string home_team = item.data.home;
                        string away_team = item.data.away;
                        string game_time = item.data.time;
                        game_time = Convert.ToDateTime(game_time.Replace("T", " ").Replace("Z", "")).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                        home_team = home_team.Replace("&", "&amp;");
                        away_team = away_team.Replace("&", "&amp;");
                        string league_name = item.data.leagueName;
                        string league_id = item.data.leagueId;
                        if (counter % 3 == 0)
                        {
                            page_number = page_number + 1;
                        }
                        foreach (var bts in item.bts)
                        {
                            bts_id = bts.data.id;
                            line = bts.data.line;
                            foreach (var odds in bts.odds)
                            {
                                string odd_name = odds.ck;
                                string odd_price = odds.price;
                                event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                            }
                        }
                        sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_odds = event_odds });
                        event_odds = new List<EventOdds>();
                        counter = counter + 1;

                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }

            }
            return sports_events;
        }

        public static List<SportEvents> GetEvents(int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string get_events_url = Cache.ServerSettings.GetServerSettings("iDoBetGetEventsUrlMTNCameroon", ref lines) + "sportIds=" + sport_type_id + "&userToken=" + token_id;
                lines = Add2Log(lines, " get_events_url = " + get_events_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(get_events_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    int page_number = 0;
                    int counter = 0;
                    string line = "";
                    Int64 bts_id = 0;
                    event_odds = new List<EventOdds>();
                    sports_events = new List<SportEvents>();
                    foreach (var item in json_response.events)
                    {
                        try
                        {
                            string game_id = item.data.id;
                            string home_team = item.data.home;
                            string away_team = item.data.away;
                            string game_time = item.data.time;
                            game_time = Convert.ToDateTime(game_time.Replace("T", " ").Replace("Z", "")).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");
                            home_team = home_team.Replace("&", "&amp;");
                            away_team = away_team.Replace("&", "&amp;");
                            string league_name = item.data.leagueName;
                            string league_id = item.data.leagueId;
                            string country = item.data.countryName;
                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }
                            foreach (var bts in item.bts)
                            {
                                bts_id = bts.data.id;
                                line = bts.data.line;
                                foreach (var odds in bts.odds)
                                {
                                    string odd_name = odds.ck;
                                    string odd_price = odds.price;
                                    event_odds.Add(new EventOdds { bts_id = bts_id, ck_name = odd_name, ck_price = odd_price, line = line });
                                }
                            }
                            sports_events.Add(new SportEvents { leagueId = league_id, league_name = league_name, game_id = Convert.ToInt64(game_id), home_team = home_team, away_team = away_team, game_time = game_time, page_number = page_number, event_odds = event_odds, country = country });
                            event_odds = new List<EventOdds>();
                            counter = counter + 1;
                        }
                        catch (Exception ex)
                        {

                        }



                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }

            }
            return sports_events;
        }


        //menu

        public static List<SubAgentsResponse> GetSubAgents(iDoBetAgents agent, string token, ref List<LogLines> lines)
        {

            List<SubAgentsResponse> result = null;
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

            SubAgentsRequest request = new SubAgentsRequest()
            {
                MethodGroup = "Sport",
                MethodName = "GetUsers",
                BranchIds = agent.branch_id
            };
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "1" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = token });

            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);

                try
                {
                    int counter = 0;
                    int page_number = 0;
                    if (json_response.data.ToString() != "[]")
                    {
                        result = new List<SubAgentsResponse>();
                        SubAgentsResponse res = new SubAgentsResponse();
                        foreach (var item in json_response.data)
                        {
                            if (item.UserName != agent.agent_username)
                            {
                                if (counter % 3 == 0)
                                {
                                    page_number = page_number + 1;
                                }
                                string available_credit = Math.Floor(Convert.ToDouble(item.AvailableCredit)).ToString();
                                string commision = "0";
                                try
                                {
                                    commision = item.Commission;
                                }
                                catch (Exception ex)
                                {
                                    commision = "0";
                                }

                                res = new SubAgentsResponse()
                                {
                                    subagent_id = item.ID,
                                    subagent_username = item.UserName,
                                    available_credit = available_credit,
                                    page_number = page_number,
                                    commision = commision
                                };
                                result.Add(res);
                                counter = counter + 1;

                            }
                        }
                        if (result != null)
                        {
                            result = result.OrderBy(x => x.subagent_id).ToList();
                            counter = 0;
                            page_number = 0;
                            foreach (SubAgentsResponse r in result)
                            {
                                if (counter % 3 == 0)
                                {
                                    page_number = page_number + 1;
                                }
                                r.page_number = page_number;
                                counter = counter + 1;
                            }
                        }


                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                }
            }
            return result;
        }

        public static string CheckBeforeWithdrawMenu(ServiceClass service, string msisdn, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            Int64 current_momo_daily_amount = DBQueries.SelectQueryReturnInt64("SELECT s.current_momo FROM service_momo_limitation s WHERE s.service_id = " + service.service_id, ref lines);
            if (current_momo_daily_amount >= service.daily_momo_limit)
            {
                lines = Add2Log(lines, "Limit has reached", 100, "DYATransferMoney");
                menu = (lang == "fr" ? "Votre operation a echoue. Merci d'essayer plus tard." : "Operation has failed. Please try again later");
            }
            if (service.user_w_limit > 0)
            {
                Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_w FROM daily_withdrawls d WHERE d.msisdn = " + msisdn, ref lines);
                if (user_withdraw_limit >= service.user_w_limit)
                {
                    lines = Add2Log(lines, "User Limit has reached", 100, "DYATransferMoney");
                    menu = (lang == "fr" ? "Votre operation a echoue. Merci d'essayer plus tard." : "Operation has failed. Please try again later");
                }
            }
            if (service.user_limit_amount > 0)
            {
                Int64 user_withdraw_limit_amount = DBQueries.SelectQueryReturnInt64("SELECT d.amount FROM daily_withdrawls d WHERE d.msisdn = " + msisdn, ref lines);
                if (user_withdraw_limit_amount >= service.user_limit_amount)
                {
                    lines = Add2Log(lines, "User Limit amount has reached", 100, "DYATransferMoney");
                    menu = (lang == "fr" ? "Votre operation a echoue. Merci d'essayer plus tard." : "Operation has failed. Please try again later");
                }
            }
            return menu;
        }

        public static string GetStartWithdrawMoneyMenu(IdoBetUser user, bool refund_money, bool w_result, bool amount_2_big, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.isValid == false)
            {
                menu = (lang == "fr" ?"Option invalide" : "Invalid option" );
            }
            else
            {
                if (refund_money == true)
                {

                    menu = (lang == "fr" ? "La demande de retrait a abouti" : "Withdraw was successful") + Environment.NewLine;
                }
                else
                {

                    menu = (lang == "fr" ? "La demande de retrait a echoue" : "Withdraw has failed") + Environment.NewLine;
                    if (amount_2_big)
                    {
                        menu = menu + (lang == "fr" ? "Montant d'argent est trop grand" : "Amount is too big") + Environment.NewLine;
                    }
                    menu = menu + (lang == "fr" ? "Veuillez reessayer" : "Please try again");
                }
            }

            return menu;
        }

        public static bool UpdateTransaction(string time_stamp, string dya_id, string transactionId, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            postBody = "";
            response_body = "";
            string token_id = Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                UpdateTransactionRequest request = new UpdateTransactionRequest()
                {
                    methodGroup = "Sport",
                    methodName = "UpdateTransaction",
                    transactionId = transactionId,
                    externalTransactionId = dya_id,
                    transactionExtraData = "{\"Timestamp\":\"" + time_stamp + "\"}",
                };
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
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

        public static string GetPayoutBarcodeTicket(USSDMenu ussd_menu, string MSISDN, ref List<LogLines> lines)
        {
            bool user_has_momo = false;
            string menu = "";
            ServiceClass subscriber_service1 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
            LoginRequest LoginRequestBody1 = new LoginRequest()
            {
                ServiceID = subscriber_service1.service_id,
                Password = subscriber_service1.service_password
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
                        ServiceID = ussd_menu.service_id,
                        TokenID = token_id
                    };
                    DYACheckAccountResponse response = CommonFuncations.DYACheckAccount.DODYACheckAccount(request);
                    if (response.ResultCode == 1000)
                    {
                        user_has_momo = true;
                    }
                }
            }

            if (!user_has_momo)
            {
                menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                menu = menu + "taper *126# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }
            return menu;
        }

        public static string GetAmountForSubAgentMenu(string MSISDN, string ussd_string, string selected_subagent_name, out Int64 pos_trans_id, ref List<LogLines> lines)
        {
            string menu = "";
            pos_trans_id = 0;
            List<iDoBetAgents> agents = Cache.iDoBet.GetiDoBetAgents(ref lines);
            if (agents != null)
            {
                iDoBetAgents agent = agents.Find(x => x.msisdn == MSISDN);
                if (agent != null)
                {
                    string token = GetTokenNew(agent.agent_username, agent.agent_password, ref lines);
                    if (token != null)
                    {
                        List<SubAgentsResponse> sub_agents = GetSubAgents(agent, token, ref lines);
                        if (sub_agents != null)
                        {
                            SubAgentsResponse filtered_sub_agent = sub_agents.Find(x => x.subagent_username == selected_subagent_name);
                            if (filtered_sub_agent != null)
                            {
                                pos_trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into pos_requests (agent_name, sub_agent_name, date_time, amount) values('" + agent.agent_username + "','" + selected_subagent_name + "',now(), " + ussd_string + ")", ref lines);
                                if (pos_trans_id > 0)
                                {
                                    menu = "Veuillez approuver la transaction MOMO";
                                }
                                else
                                {
                                    menu = "Error 55";
                                }

                            }
                        }

                    }
                }
            }

            return menu;
        }

        public static string GetSlectedSubAgentMenu(string MSISDN, int page_number, string ussd_string, out string selected_subagent_name, ref List<LogLines> lines)
        {
            selected_subagent_name = "0";
            string menu = "";
            List<iDoBetAgents> agents = Cache.iDoBet.GetiDoBetAgents(ref lines);
            if (agents != null)
            {
                iDoBetAgents agent = agents.Find(x => x.msisdn == MSISDN);
                if (agent != null)
                {
                    string token = GetTokenNew(agent.agent_username, agent.agent_password, ref lines);
                    if (token != null)
                    {
                        List<SubAgentsResponse> sub_agents = GetSubAgents(agent, token, ref lines);
                        if (sub_agents != null)
                        {
                            List<SubAgentsResponse> filtered_sub_agents = sub_agents.Where(x => x.page_number == page_number).ToList();
                            SubAgentsResponse filtered_sub_agent = filtered_sub_agents[Convert.ToInt32(ussd_string) - 1];
                            if (filtered_sub_agent != null)
                            {
                                int available_credit = 0;
                                try
                                {
                                    available_credit = Convert.ToInt32(filtered_sub_agent.available_credit);

                                }
                                catch (Exception ex)
                                {

                                }

                                if (available_credit > 0)
                                {
                                    selected_subagent_name = filtered_sub_agent.subagent_username;
                                    menu = "Vous etes sur le point de collecter de l'argent aupres d'" + filtered_sub_agent.subagent_username + Environment.NewLine;
                                    menu = menu + "entrer le montant de l'argent" + Environment.NewLine;
                                    menu = menu + "maximum " + available_credit + " FCFA" + Environment.NewLine;
                                    menu = menu + Environment.NewLine + "*) Retour";
                                }
                                else
                                {
                                    menu = "Vous ne pouvez pas collecter 0 FCFA aupres d'" + filtered_sub_agent.subagent_username + Environment.NewLine;
                                    menu = menu + Environment.NewLine + "*) Retour";
                                }
                            }
                        }
                    }
                }
            }
            if (menu == "")
            {
                menu = "Error 54" + Environment.NewLine;
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }
            return menu;
        }

        public static string GetFootSoldierMainMenu(string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            List<FootSoldiers> footsoldiers = Cache.FS.GetFootSoldiers(ref lines);
            if (footsoldiers != null)
            {
                FootSoldiers fs = footsoldiers.Find(x => x.footsoldier_msisdn == Convert.ToInt64(MSISDN));
                if (fs != null)
                {
                    menu = "Bonjour " + fs.footsoldier_name + Environment.NewLine + Environment.NewLine;
                    menu = menu + "Entrez le numero de telephone de l'utilisateur" + Environment.NewLine;
                }
                else
                {
                    menu = "Error 78" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                }
            }

            return menu;
        }

        public class CreatreUserRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public int userType { get; set; }
            public string userName { get; set; }
            public string email { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string phone { get; set; }
            public string registrationDetails { get; set; }
            public double depositAmount { get; set; }

        }

        public class CreateUserResponse
        {
            public string userId { get; set; }
            public string password { get; set; }
        }

        public static CreateUserResponse CreateUser(string user_msisdn, string user_fullname, FootSoldiers fs, ref List<LogLines> lines)
        {
            CreateUserResponse response = new CreateUserResponse()
            {
                userId = "-1",
                password = "-1",
            };

            string token_id = Api.Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);

            if (!String.IsNullOrEmpty(token_id))
            {
                string fname = "USSD " + user_msisdn.Substring(3, 3), lname = "USSD " + user_msisdn.Substring(3, 3);
                try
                {
                    string[] words = user_fullname.Split(' ');
                    fname = words[0];
                    lname = words[1];

                }
                catch (Exception ex)
                {

                }



                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                CreatreUserRequest request = new CreatreUserRequest()
                {
                    methodGroup = "Sport",
                    methodName = "CreateUser",
                    userType = 2,
                    userName = user_msisdn.Substring(3),
                    email = "",
                    firstName = fname,
                    depositAmount = 0,
                    lastName = lname,
                    phone = user_msisdn,
                    registrationDetails = "{\"utm_source\": \"FootSoldiers\",\"utm_medium\": \"USSD_FootSoldiers\",\"utm_campaign\": \"" + fs.footsoldier_name + " " + fs.footsoldier_msisdn + "\"}"

                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = "yellowbet.cm" });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            string user_id = json_response.data.userId;
                            string password = json_response.data.password;

                            response = new CreateUserResponse()
                            {
                                userId = user_id,
                                password = password
                            };
                        }
                        else
                        {
                            string mail_body = "<p><b>Create User has failed for</b><br> <b>user_msisdn:</b> " + user_msisdn + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "Warning from Create User - " + user_msisdn;
                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
            }
            return response;
        }

        public static CreateUserResponse CreateUserWOFS(string user_msisdn, string first_name, string last_name, string email, string deposit_amount, ref List<LogLines> lines)
        {
            CreateUserResponse response = new CreateUserResponse()
            {
                userId = "-1",
                password = "-1",
            };

            string token_id = Api.Cache.iDoBet.GetMTNCamerooniDoBetUserTokenNew(0, ref lines);

            if (!String.IsNullOrEmpty(token_id))
            {

                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                CreatreUserRequest request = new CreatreUserRequest()
                {
                    methodGroup = "Sport",
                    methodName = "CreateUser",
                    userType = 2,
                    userName = user_msisdn.Substring(3),
                    email = email,
                    depositAmount = Convert.ToDouble(deposit_amount),
                    firstName = first_name,
                    lastName = last_name,
                    phone = user_msisdn,
                    registrationDetails = "{}"

                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "24" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = "yellowbet.cm" });

                string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (json_response.isSuccessfull == true)
                        {
                            string user_id = json_response.data.userId;
                            string password = json_response.data.password;

                            response = new CreateUserResponse()
                            {
                                userId = user_id,
                                password = password
                            };
                        }
                        else
                        {
                            string mail_body = "<p><b>Create User has failed for</b><br> <b>user_msisdn:</b> " + user_msisdn + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "Warning from Create User - " + user_msisdn;
                            string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                            string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                            string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                            string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                            int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                            string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                            CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
            }
            return response;
        }

        public static string GetFootSoldiersFullNameMenu(string MSISDN, string ussd_string, out bool is_close, USSDMenu ussd_menu, string user_msisdn, out CreateUserResponse create_user, ref List<LogLines> lines)
        {
            is_close = true;
            string menu = "";
            create_user = new CreateUserResponse();
            List<FootSoldiers> footsoldiers = Cache.FS.GetFootSoldiers(ref lines);
            if (footsoldiers != null)
            {
                FootSoldiers fs = footsoldiers.Find(x => x.footsoldier_msisdn == Convert.ToInt64(MSISDN));
                if (fs != null)
                {
                    create_user = CreateUser(user_msisdn.ToString(), ussd_string, fs, ref lines);
                    if (create_user.userId != "-1")
                    {
                        menu = "L'utilisateur a ete cree avec succes" + Environment.NewLine;
                    }
                    else
                    {
                        menu = "Error 78_2" + Environment.NewLine;
                        menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                        is_close = false;
                    }
                }
                else
                {
                    menu = "Error 78" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                }
            }
            return menu;
        }

        public static string GetFootSoldiersMSISDNMenu(string MSISDN, string ussd_string, out bool is_close, ServiceClass service, ref List<LogLines> lines)
        {
            is_close = true;
            string menu = "";
            List<FootSoldiers> footsoldiers = Cache.FS.GetFootSoldiers(ref lines);
            if (footsoldiers != null)
            {
                FootSoldiers fs = footsoldiers.Find(x => x.footsoldier_msisdn == Convert.ToInt64(MSISDN));
                if (fs != null)
                {
                    bool user_has_momo = false;
                    Int64 number;
                    bool success = Int64.TryParse(ussd_string, out number);
                    if (success)
                    {
                        IdoBetUser user = Api.CommonFuncations.iDoBetMTNCameroon.SearchUserNew("237" + ussd_string, ref lines);
                        int user_id = 0;
                        try
                        {
                            user_id = user.id;
                        }
                        catch (Exception ex)
                        {

                        }
                        if (user_id <= 0)
                        {
                            //TODO getaccountholder info
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
                                //ToDo Create user 
                            }

                            menu = "Entrez le nom complet de l'utilisateur" + Environment.NewLine + Environment.NewLine;
                            is_close = false;
                        }
                        else
                        {
                            menu = "L'utilisateur est deja abonne";
                        }

                    }
                    else
                    {
                        menu = "Numero de telephone incorrect" + Environment.NewLine + Environment.NewLine;
                    }
                }
                else
                {
                    menu = "Error 78" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                }
            }
            return menu;
        }

        public static string GetAgentMainMenu(string MSISDN, int page_number, ref List<LogLines> lines)
        {
            string menu = "";
            List<iDoBetAgents> agents = Cache.iDoBet.GetiDoBetAgents(ref lines);
            if (agents != null)
            {
                iDoBetAgents agent = agents.Find(x => x.msisdn == MSISDN);
                if (agent != null)
                {
                    string token = GetTokenNew(agent.agent_username, agent.agent_password, ref lines);
                    if (token != null)
                    {
                        List<SubAgentsResponse> sub_agents = GetSubAgents(agent, token, ref lines);
                        if (sub_agents != null)
                        {
                            List<SubAgentsResponse> filtered_sub_agents = sub_agents.Where(x => x.page_number == page_number).ToList();
                            if (filtered_sub_agents != null)
                            {
                                menu = "Bonjour " + agent.agent_name + Environment.NewLine;
                                menu = menu + "selectionner un SubAgent:" + Environment.NewLine;

                                int counter = 1;
                                foreach (SubAgentsResponse sa in filtered_sub_agents)
                                {
                                    menu = menu + counter + ") " + sa.subagent_username + " (" + sa.available_credit + ")" + Environment.NewLine;
                                    counter = counter + 1;
                                }
                                int total_pages = (sub_agents.Count() / 3) + (sub_agents.Count() % 3 == 0 ? 0 : 1);

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
                        }
                    }
                }
            }
            if (menu == "")
            {
                menu = "Error 53" + Environment.NewLine;
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }
            return menu;
        }

        public static string GetDepositMoneyMenu(IdoBetUser user, USSDMenu ussd_menu, string MSISDN, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            bool user_has_momo = false;
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    switch(lang)
                    {
                        case "fr":
                            menu = "Rejoigner YellowBet pour gagner d'interessants bonus!" + Environment.NewLine;
                            menu = menu + @"Visiter notre site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Join YellowBet to win amazing bounuses" + Environment.NewLine;
                            menu = menu + @"Visit our site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }
                    
                }
                else
                {
                    
                    switch (lang)
                    {
                        case "fr":
                            menu = "Quelque chose est arrive s'il vous plait appelez a l'equipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Please contact our support team at serviceclient@yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                    }
                }

            }
            else
            {
                menu = (lang == "fr" ? "Bonjour " : "Hello ") + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
                //check if the user has a MOMO account
                ServiceClass subscriber_service1 = GetServiceByServiceID(Convert.ToInt32(ussd_menu.service_id), ref lines);
                LoginRequest LoginRequestBody1 = new LoginRequest()
                {
                    ServiceID = subscriber_service1.service_id,
                    Password = subscriber_service1.service_password
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
                            ServiceID = ussd_menu.service_id,
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
                    menu = menu + (lang == "fr" ? "Entrer votre montant" : "Enter amount") + Environment.NewLine + Environment.NewLine;
                    menu = menu + "Minimum: 200 FCFA" + Environment.NewLine;
                    menu = menu + "Maximum: 1,000,000 FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) " + (lang == "fr" ? "Menu Principal " : "Main Menu");
                }
                else
                {
                    
                    switch (lang)
                    {
                        case "fr":
                            menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                            menu = menu + "taper *126# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = menu + "If you don't have a MOMO account," + Environment.NewLine;
                            menu = menu + "dial *126# to open" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }

                }


            }
            return menu;
        }

        public static string GetStartDepositMoneyMenu(ServiceClass service, IdoBetUser user, USSDMenu ussd_menu, string MSISDN, string amount, out DYAReceiveMoneyRequest momo_request, string lang, ref List<LogLines> lines)
        {
            momo_request = null;
            string menu = "";
            string token_id = "";
            bool user_has_momo = false;
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    switch (lang)
                    {
                        case "fr":
                            menu = "Rejoigner YellowBet pour gagner d'interessants bonus!" + Environment.NewLine;
                            menu = menu + @"Visiter notre site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Join YellowBet to win amazing bounuses" + Environment.NewLine;
                            menu = menu + @"Visit our site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }

                }
                else
                {

                    switch (lang)
                    {
                        case "fr":
                            menu = "Quelque chose est arrive s'il vous plait appelez a l'equipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Please contact our support team at serviceclient@yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                    }
                }
            }
            else
            {
                menu = (lang == "fr" ? "Bonjour " : "Hello ") + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
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
                    if (Convert.ToInt32(amount) >= 200 && Convert.ToInt32(amount) <= 1000000)
                    {
                        momo_request = new DYAReceiveMoneyRequest()
                        {
                            MSISDN = Convert.ToInt64(MSISDN),
                            Amount = Convert.ToInt32(amount),
                            ServiceID = service.service_id,
                            TokenID = token_id,
                            TransactionID = "iDoBetDeposit_" + Guid.NewGuid().ToString().Substring(0, 10).Replace("-", "")

                        };
                        switch(lang)
                        {
                            case "fr":
                                menu = menu + "Vous avez presque fini" + Environment.NewLine;
                                menu = menu + "Pour completer votre pari, confirmez le paiement" + Environment.NewLine;
                                break;
                            case "en":
                                menu = menu + "You are almost done" + Environment.NewLine;
                                menu = menu + "Confirm your payment or dial *126#" + Environment.NewLine;
                                break;
                        }
                        
                    }
                    else
                    {
                        menu = menu + (lang == "fr" ? "Entrer votre montant" : "Enter amount") + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Minimum: 200 FCFA" + Environment.NewLine;
                        menu = menu + "Maximum: 1,000,000 FCFA" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) " + (lang == "fr" ? "Menu Principal " : "Main Menu");
                    }

                }
                else
                {
                    switch (lang)
                    {
                        case "fr":
                            menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                            menu = menu + "taper *126# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = menu + "If you don't have a MOMO account," + Environment.NewLine;
                            menu = menu + "dial *126# to open" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }
                }


            }
            return menu;
        }

        public static string GetWithdrawMoneyMenu(IdoBetUser user, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    switch (lang)
                    {
                        case "fr":
                            menu = "Rejoigner YellowBet pour gagner d'interessants bonus!" + Environment.NewLine;
                            menu = menu + @"Visiter notre site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Join YellowBet to win amazing bounuses" + Environment.NewLine;
                            menu = menu + @"Visit our site http://m.yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }
                }
                else
                {
                    switch (lang)
                    {
                        case "fr":
                            menu = "Quelque chose est arrive s'il vous plait appelez a l'equipe de soutien 91111919" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Please contact our support team at serviceclient@yellowbet.cm" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                    }
                }
            }
            else
            {
                switch(lang)
                {
                    case "fr":
                        menu = "Bonjour " + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Votre solde est " + user.AvailableForWithdraw + " FCFA" + Environment.NewLine;
                        if (user.AvailableForWithdraw > 0)
                        {
                            menu = menu + "Entrer le montant a retirer" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                        }
                        else
                        {
                            menu = menu + Environment.NewLine + "M) Menu Principal ";
                        }
                        break;
                    case "en":
                        menu = "Hello " + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Your balance is " + user.AvailableForWithdraw + " FCFA" + Environment.NewLine;
                        if (user.AvailableForWithdraw > 0)
                        {

                            menu = menu + "Enter the amount to withdraw" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                        }
                        else
                        {
                            menu = menu + Environment.NewLine + "M) Main Menu ";
                        }
                        break;
                }

                
            }
            return menu;
        }

        public static string GetOrderStatus(string order_status, string lang)
        {
            string result = "";
            switch(lang)
            {
                case "fr":
                    switch (order_status)
                    {
                        case "4":
                            result = " - Paye";
                            break;
                        case "5":
                            result = " - Bloque";
                            break;
                        case "6":
                            result = " - Annule";
                            break;
                        case "7":
                            result = " - Expire";
                            break;
                    }
                    break;
                case "en":
                    switch (order_status)
                    {
                        case "4":
                            result = " - Paid";
                            break;
                        case "5":
                            result = " - Blocked";
                            break;
                        case "6":
                            result = " - Canceled";
                            break;
                        case "7":
                            result = " - Expired";
                            break;
                    }
                    break;
            }
            
            return result;
        }

        public static string GetCheckTicketMenu(string MSISDN, int page_number, string ussd_string, string lang, ref List<LogLines> lines)
        {
            string menu = "";

            List<Tickets> tickets = CheckTicketsNew(MSISDN, ref lines);

            if (tickets != null)
            {
                List<Tickets> filtered_tickets = tickets.Where(x => x.page_number == page_number).ToList();
                Tickets filtered_ticket = filtered_tickets[Convert.ToInt32(ussd_string) - 1];
                if (filtered_ticket != null)
                {
                    string status = "";
                    switch (lang)
                    {
                        case "fr":
                            menu = "Statut du Ticket:" + Environment.NewLine;
                            menu = menu + "Code Barre: " + filtered_ticket.barcode + Environment.NewLine;
                            menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                            
                            switch (filtered_ticket.win_status)
                            {
                                case "0":
                                    status = "en attente";
                                    break;
                                case "1": //looser
                                    status = "Perdant";
                                    break;
                                case "2":
                                    status = "Gagnant";
                                    break;
                                case "3":
                                    status = "Remboursement";
                                    break;

                            }
                            status = status + GetOrderStatus(filtered_ticket.order_status, lang);

                            menu = menu + "Statut: " + status + Environment.NewLine;

                            menu = menu + "Enjeu: " + filtered_ticket.amount + Environment.NewLine;
                            if (status == "en attente" || status == "Gagnant")
                            {
                                menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (status == "Gagnant - Paye")
                            {
                                menu = menu + "Montant paye: " + filtered_ticket.payout + Environment.NewLine;
                            }

                            menu = menu + Environment.NewLine + "*) Retour ";
                            break;
                        case "en":
                            menu = "Ticket Status:" + Environment.NewLine;
                            menu = menu + "Barcode: " + filtered_ticket.barcode + Environment.NewLine;
                            menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                            switch (filtered_ticket.win_status)
                            {
                                case "0":
                                    status = "Pending";
                                    break;
                                case "1": //looser
                                    status = "Lost";
                                    break;
                                case "2":
                                    status = "Winner";
                                    break;
                                case "3":
                                    status = "Refund";
                                    break;

                            }
                            status = status + GetOrderStatus(filtered_ticket.order_status, lang);

                            menu = menu + "Status: " + status + Environment.NewLine;

                            menu = menu + "Stake: " + filtered_ticket.amount + Environment.NewLine;
                            if (status == "Pending" || status == "Winner")
                            {
                                menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (status == "Winner - Paid")
                            {
                                menu = menu + "Paid Amount: " + filtered_ticket.payout + Environment.NewLine;
                            }

                            menu = menu + Environment.NewLine + "*) Back ";
                            break;
                    }
                    

                }
                else
                {
                    switch(lang)
                    {
                        case "fr":
                            menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Menu Principal ";
                            break;
                        case "en":
                            menu = "Ticket was not found" + Environment.NewLine + Environment.NewLine;
                            menu = menu + "M) Main Menu ";
                            break;
                    }
                    
                }

            }
            else
            {
                switch (lang)
                {
                    case "fr":
                        menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Menu Principal ";
                        break;
                    case "en":
                        menu = "Ticket was not found" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Main Menu ";
                        break;
                }
            }
            return menu;
        }

        public static int GetCashierID(string msisdn, ref List<LogLines> lines)
        {
            int cashier_id = 0;
            List<CashierInfo> commisions = Api.Cache.iDoBet.GetCashierInfo(ref lines);
            if (commisions != null)
            {
                CashierInfo filtered_commision = commisions.Find(x => x.msisdn == Convert.ToInt64(msisdn));
                if (filtered_commision != null)
                {
                    cashier_id = filtered_commision.cashier_id;
                }
            }
            return cashier_id;
        }
        public static int GetCashierCommision(string msisdn, string amount, ref List<LogLines> lines)
        {
            int commision = 0;
            List<CashierInfo> commisions = Api.Cache.iDoBet.GetCashierInfo(ref lines);
            if (commisions != null)
            {
                CashierInfo filtered_commision = commisions.Find(x => x.msisdn == Convert.ToInt64(msisdn) && Convert.ToInt32(amount) >= x.start_range && Convert.ToInt32(amount) <= x.end_range);
                if (filtered_commision != null)
                {
                    commision = filtered_commision.commision;
                }
            }
            return commision;
        }

        public static string GetCheckTicketByBarcodeMenu(string msisdn, string bar_codeid, out string paid_amount, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            paid_amount = "0";
            bool display_withdraw_option = false;
            List<Tickets> tickets = CheckTicketByBarCodeNew(bar_codeid, ref lines);
            if (tickets != null)
            {
                Tickets filtered_ticket = tickets.Find(x => x.barcode == bar_codeid);

                if (filtered_ticket != null)
                {
                    int commision = GetCashierCommision(msisdn, filtered_ticket.payout.ToString(), ref lines);
                    string status = "";
                    switch (lang)
                    {
                        case "fr":
                            menu = "Statut du Ticket:" + Environment.NewLine;
                            menu = menu + "Code Barre: " + filtered_ticket.barcode + Environment.NewLine;
                            menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                            switch (filtered_ticket.win_status)
                            {
                                case "0":
                                    status = "en attente";
                                    break;
                                case "1":
                                    status = "Perdant";
                                    break;
                                case "2":
                                    status = "Gagnant"; //winner
                                    if (String.IsNullOrEmpty(filtered_ticket.msisdn))
                                    {
                                        display_withdraw_option = true;
                                    }
                                    if (filtered_ticket.order_status == "4")
                                    {
                                        display_withdraw_option = false;
                                        status = "Gagnant - Paye";
                                    }
                                    break;
                                case "3": // refund
                                    status = "Remboursement";
                                    if (String.IsNullOrEmpty(filtered_ticket.msisdn))
                                    {
                                        display_withdraw_option = true;
                                    }
                                    if (filtered_ticket.order_status == "4")
                                    {
                                        status = "l'utilisateur a ete rembourse";
                                        display_withdraw_option = false;
                                    }
                                    break;

                            }
                            menu = menu + "Statut: " + status + Environment.NewLine;

                            menu = menu + "Enjeu: " + filtered_ticket.amount + Environment.NewLine;
                            if (status == "en attente" || status == "Gagnant")
                            {
                                menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (status == "Gagnant - Paye")
                            {
                                menu = menu + "Montant paye: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (display_withdraw_option == true)
                            {
                                if (commision > 0)
                                {
                                    menu = menu + "Commission: " + commision;
                                }

                                paid_amount = (filtered_ticket.payout + commision).ToString();
                                menu = menu + Environment.NewLine + "1) Retirer de l'argent";
                            }
                            menu = menu + Environment.NewLine + "*) Retour ";
                            break;
                        case "en":
                            menu = "Ticket Status:" + Environment.NewLine;
                            menu = menu + "Barcode: " + filtered_ticket.barcode + Environment.NewLine;
                            menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                            
                            switch (filtered_ticket.win_status)
                            {
                                case "0":
                                    status = "Pending";
                                    break;
                                case "1":
                                    status = "Lost";
                                    break;
                                case "2":
                                    status = "Winner"; //winner
                                    if (String.IsNullOrEmpty(filtered_ticket.msisdn))
                                    {
                                        display_withdraw_option = true;
                                    }
                                    if (filtered_ticket.order_status == "4")
                                    {
                                        display_withdraw_option = false;
                                        status = "Winner - Paid";
                                    }
                                    break;
                                case "3": // refund
                                    status = "Refund";
                                    if (String.IsNullOrEmpty(filtered_ticket.msisdn))
                                    {
                                        display_withdraw_option = true;
                                    }
                                    if (filtered_ticket.order_status == "4")
                                    {
                                        status = "User was refunded";
                                        display_withdraw_option = false;
                                    }
                                    break;

                            }
                            menu = menu + "Status: " + status + Environment.NewLine;

                            menu = menu + "Stake: " + filtered_ticket.amount + Environment.NewLine;
                            if (status == "Pending" || status == "Winner")
                            {
                                menu = menu + "Max payout: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (status == "Winner - Paid")
                            {
                                menu = menu + "Amount paid: " + filtered_ticket.payout + Environment.NewLine;
                            }
                            if (display_withdraw_option == true)
                            {
                                if (commision > 0)
                                {
                                    menu = menu + "Commission: " + commision;
                                }

                                paid_amount = (filtered_ticket.payout + commision).ToString();
                                menu = menu + Environment.NewLine + "1) Withdraw";
                            }
                            menu = menu + Environment.NewLine + "*) Back ";
                            break;
                    }
                    


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

        public static string GetCheckTicketsMenu(string MSISDN, int page_number, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            List<Tickets> tickets = CheckTicketsNew(MSISDN, ref lines);
            if (tickets != null)
            {
                List<Tickets> filtered_tickets = tickets.Where(x => x.page_number == page_number).ToList();
                int counter = 1;
                int total_pages = (tickets.Count() / 3) + (tickets.Count() % 3 == 0 ? 0 : 1);
                switch (lang)
                {
                    case "fr":
                        menu = "Statut du Ticket" + Environment.NewLine;

                        foreach (Tickets t in filtered_tickets)
                        {
                            menu = menu + counter + ") " + t.barcode + " (" + t.creation_time + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
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
                            menu = menu + Environment.NewLine + "*) Retour ";
                        }
                        menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                        break;
                    case "en":
                        menu = "Ticket Status" + Environment.NewLine;

                        foreach (Tickets t in filtered_tickets)
                        {
                            menu = menu + counter + ") " + t.barcode + " (" + t.creation_time + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        
                        if (page_number == 1 && total_pages != page_number)
                        {
                            menu = menu + Environment.NewLine + "#) Next";
                        }
                        if (page_number > 1 && page_number < total_pages)
                        {
                            menu = menu + Environment.NewLine + "#) Next" + Environment.NewLine;
                            menu = menu + "*) Back";
                        }
                        if (page_number == total_pages && page_number > 1)
                        {
                            menu = menu + Environment.NewLine + "*) Back ";
                        }

                        menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
                        break;
                }
                
                

            }
            else
            {
                switch(lang)
                {
                    case "en":
                        menu = "Ticket was not found" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Main Menu ";
                        break;
                    case "fr":
                        menu = "Ticket introuvable" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Menu Principal ";
                        break;
                }
            }
            return menu;
        }

        public static string GetSoccerLeagueMenu(int sport_type, string ussd_string, int ussd_id, ref List<LogLines> lines, out int out_league_id, string lang)
        {
            out_league_id = 0;
            string menu = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetMTNCameroonEventsFromCache(sport_type, ref lines);
            switch (sport_type)
            {
                case 31:
                    menu = (lang =="fr" ? "Parions sur le foot:" : "Let's bet on football") + Environment.NewLine;
                    break;
                case 32:
                    menu = (lang == "fr" ? "Parions sur le basket:" : "Let's bet on basketball") + Environment.NewLine;
                    break;
                case 35:
                    menu = (lang == "fr" ? "Parions sur le tennis:" : "Let's bet on tennis") + Environment.NewLine;
                    break;
            }
            int counter = 2;
            List<iDoBetLeague> idobet_leagues = Cache.iDoBet.GetiDoBetLeagues(ref lines);


            if (sports_events != null)
            {
                menu = menu + (lang == "fr" ? "1) Tous les evenements (" : "1) All events") + sports_events.Count() + ")" + Environment.NewLine;
                var query = sports_events.GroupBy(x => x.leagueId, x => x.league_name, (league_id, league_name) => new { league_id = league_id, Count = league_id.Count(), league_name = league_name }).ToList();

                if (idobet_leagues != null)
                {
                    idobet_leagues = idobet_leagues.Where(x => x.ussd_id == ussd_id).ToList();
                    foreach (iDoBetLeague l in idobet_leagues)
                    {
                        var filtered_query = query.Where(x => x.league_id == l.league_id.ToString()).ToList();
                        int games_count = sports_events.Where(x => x.leagueId == l.league_id.ToString()).Count();
                        if (filtered_query != null && filtered_query.Count() > 0)
                        {
                            menu = menu + counter + ") " + l.league_name + " (" + games_count + ")" + Environment.NewLine;
                            if (ussd_string == counter.ToString())
                            {
                                out_league_id = l.league_id;
                            }
                            counter = counter + 1;
                            query = query.Where(x => x.league_id != l.league_id.ToString()).ToList();
                        }
                        if (counter == 4)
                        {
                            break;
                        }
                    }
                }

                if (counter < 4)
                {
                    foreach (var q in query)
                    {
                        string leage_name = sports_events.Find(x => x.leagueId == q.league_id.ToString()).league_name;
                        int games_count = sports_events.Where(x => x.leagueId == q.league_id.ToString()).Count();
                        menu = menu + counter + ") " + leage_name + " (" + games_count + ")" + Environment.NewLine;
                        if (ussd_string == counter.ToString())
                        {
                            out_league_id = Convert.ToInt32(q.league_id);
                        }
                        counter = counter + 1;
                        if (counter == 4)
                        {
                            break;
                        }
                    }
                }
                menu = menu + Environment.NewLine;
                menu = menu + (lang == "fr" ? "M) Menu Principal " : "M) Main Menu ");
            }
            return menu;
        }






        public static string GetConfirmMenu(int sport_type_id, Int64 game_id, string ussd_string, int odd_page, out double selected_odd, out int selected_bet_type_id, out string selected_odd_name, out string selected_odd_line, string lang, ref List<LogLines> lines, USSDSession ussd_session, string msisdn, ServiceClass service)
        {
            string menu = "";
            selected_odd = 0;
            selected_bet_type_id = 0;
            selected_odd_name = "";
            selected_odd_line = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetMTNCameroonEventsFromCache(sport_type_id, ref lines);
            SportEvents game = new SportEvents();
            game = sports_events.Find(x => x.game_id == game_id);
            if (game != null)
            {
                try
                {
                    menu = (lang == "fr" ? "Confirmer:" : "Confirm:") + Environment.NewLine;
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
                            menu = menu + (filtred_odd.ck_name.ToLower() == "u" ? "U " : "O ") + filtred_odd.line + " (" + filtred_odd.ck_price + ")" + Environment.NewLine;
                            break;
                        case 2://DC
                            filtred_odds = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                            filtred_odd = filtred_odds[Convert.ToInt32(ussd_string) - 1];
                            menu = menu + "DC-" + filtred_odd.ck_name + " (" + filtred_odd.ck_price + ")" + Environment.NewLine;
                            break;
                    }
                    selected_odd = Convert.ToDouble(filtred_odd.ck_price);
                    selected_bet_type_id = Convert.ToInt32(filtred_odd.bts_id);
                    selected_odd_name = filtred_odd.ck_name;
                    selected_odd_line = filtred_odd.line;

                    switch(lang)
                    {
                        case "fr":
                            menu = menu + Environment.NewLine + "1) Ajouter un Jeu" + Environment.NewLine;
                            menu = menu + "2) Confirmer &amp; Payer" + Environment.NewLine;
                            menu = menu + Environment.NewLine + "*) Retour ";
                            break;
                        case "en":
                            menu = menu + Environment.NewLine + "1) Add a game" + Environment.NewLine;
                            menu = menu + "2) Confirm &amp; Pay" + Environment.NewLine;
                            menu = menu + Environment.NewLine + "*) Back ";
                            break;
                    }

                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id) values(" + msisdn + ", " + game_id + ", " + odd_page + ", '" + ussd_string + "', now(), 0,'" + ussd_session.user_seesion_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ")", ref lines);

                    List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);

                    if (saved_games != null)
                    {
                        List<USSDBonus> ussd_b = GetUSSDBonus(service.service_id.ToString(), ref lines);
                        if (ussd_b != null)
                        {
                            USSDBonus ub = ussd_b.Find(x => x.msisdn == msisdn);
                            if (ub != null)
                            {
                                if (saved_games.Count() >= ub.min_selections)
                                {
                                    double total_ratio = 1;
                                    bool min_selection = true;
                                    foreach (SavedGames r in saved_games)
                                    {
                                        if (r.selected_odd < ub.min_odd_per_selections)
                                        {
                                            min_selection = false;
                                            break;
                                        }
                                        total_ratio = total_ratio * r.selected_odd;
                                    }
                                    if (total_ratio >= ub.min_total_odd && min_selection)
                                    {
                                        menu = menu + "3) Free Bet" + Environment.NewLine;
                                    }
                                }
                            }
                        }
                    }

                    switch (lang)
                    {
                        case "fr":
                            menu = menu + Environment.NewLine + "*) Retour ";
                            break;
                        case "en":
                            menu = menu + Environment.NewLine + "*) Back ";
                            break;
                    }
                }
                catch (Exception e)
                {
                    lines = Add2Log(lines, " exception = " + e.ToString(), 100, "ivr_subscribe");
                }

            }
            return menu;
        }
        public static int GetBTSID(int odd_page, int sport_type_id, out string odd_game)
        {
            int bts_id = 0;
            odd_game = "";
            switch (sport_type_id)
            {
                case 31:
                    bts_id = (odd_page == 0 ? 310310 : (odd_page == 1 ? 356310 : 346310));
                    odd_game = (odd_page == 0 ? "FT" : "");
                    break;
                case 32:
                    bts_id = 320320;
                    odd_game = (odd_page == 0 ? "2 Way" : "");
                    break;
                case 35:
                    bts_id = 320350;
                    odd_game = (odd_page == 0 ? "2 Way" : "");
                    break;
            }
            return bts_id;
        }

        public static string GetGameOddsMenu(int sport_type_id, int page_number, string ussd_string, Int64 game_id, int odd_page, out Int64 out_game_id, int selected_league_id, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            out_game_id = 0;
            SportEvents game = null;
            List<SportEvents> sports_events = null;
            if (selected_league_id == 0)
            {
                sports_events = Cache.iDoBet.GetMTNCameroonEventsFromCache(sport_type_id, ref lines);
            }
            else
            {
                sports_events = Cache.iDoBet.GetMTNCameroonEventsWithLeagueIDFromCache(selected_league_id, sport_type_id, ref lines);
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
                out_game_id = game.game_id;
                menu = game.home_team + " Vs " + game.away_team + Environment.NewLine;
                string game_time = Convert.ToDateTime(game.game_time).ToString("MMMM", CultureInfo.CreateSpecificCulture(lang)).Replace("û", "u").Replace("é", "e") + " " + Convert.ToDateTime(game.game_time).ToString("dd") + ", " + Convert.ToDateTime(game.game_time).ToString("HH:mm");
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
                            menu = menu + counter + ") " + odd_name + "-" + eo.ck_name + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count > 4)
                        {
                            menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next");
                        }
                        break;
                    case 1://UO
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") " + (eo.ck_name.ToLower() == "under" ? "U " : "O ") + eo.line + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        if (odd_count >= 8)
                        {
                            menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next") + Environment.NewLine;
                        }
                        menu = menu + (lang == "fr" ? "*) Retour" : "*) Back");
                        break;
                    case 2://DC
                        filtred_odd = game.event_odds.Where(x => x.bts_id == bts_id).ToList();
                        foreach (EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ". DC-" + eo.ck_name + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        menu = menu + Environment.NewLine + (lang == "fr" ? "*) Retour" : "*) Back");
                        break;
                }
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }

            return menu;
        }

        public static string GetConfirmBet(USSDSession ussd_session, string amount, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                double total_ratio = 1;
                double max_payout = 0;
                switch (lang)
                {
                    case "fr":
                        menu = "Confirmer votre Pari:" + Environment.NewLine;
                        menu = menu + "Numero des Jeux: " + saved_games.Count() + Environment.NewLine;
                        
                        foreach (SavedGames r in saved_games)
                        {
                            total_ratio = total_ratio * r.selected_odd;
                        }
                        total_ratio = Math.Round(total_ratio, 2);
                        menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine;
                        menu = menu + "Montant: " + amount + Environment.NewLine;
                        max_payout = Math.Round(total_ratio * Convert.ToDouble(amount), 2);
                        max_payout = (max_payout >= 20000000 ? 20000000 : max_payout);
                        menu = menu + "Gain Maximum: " + max_payout + Environment.NewLine + Environment.NewLine;
                        menu = menu + "1) Confirmer" + Environment.NewLine;
                        menu = menu + "2) Annuler &amp; Recommencer" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "*) Retour ";
                        break;
                    case "en":
                        menu = "Confirm your bet:" + Environment.NewLine;
                        menu = menu + "Number of games: " + saved_games.Count() + Environment.NewLine;
                        foreach (SavedGames r in saved_games)
                        {
                            total_ratio = total_ratio * r.selected_odd;
                        }
                        total_ratio = Math.Round(total_ratio, 2);
                        menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine;
                        menu = menu + "Amount: " + amount + Environment.NewLine;
                        max_payout = Math.Round(total_ratio * Convert.ToDouble(amount), 2);
                        max_payout = (max_payout >= 20000000 ? 20000000 : max_payout);
                        menu = menu + "Maximum gain: " + max_payout + Environment.NewLine + Environment.NewLine;
                        menu = menu + "1) Confirm" + Environment.NewLine;
                        menu = menu + "2) Cancel" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "*) Back ";
                        break;
                }
                
            }
            return menu;
        }

        public static string GetCloseBetFailed(string lang, ref List<LogLines> lines)
        {
            string menu = "";
            switch (lang)
            {
                case "en":
                    menu = "Your Bet has failed" + Environment.NewLine;
                    menu = menu + "Please try again"; //Your bet has failed.
                    break;
                case "fr":
                    menu = "Votre pari a echoue." + Environment.NewLine;
                    menu = menu + "Veuillez reessayer"; //Your bet has failed.
                    break;
            }
            
            return menu;
        }
        public static string GetCloseBet(USSDSession ussd_session, string amount, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                //menu = "Vous avez presque fini." + Environment.NewLine;
                //menu = menu + "Numero des Jeux: " + saved_games.Count() + Environment.NewLine;
                double total_ratio = 1;
                foreach (SavedGames r in saved_games)
                {
                    total_ratio = total_ratio * r.selected_odd;
                }
                total_ratio = Math.Round(total_ratio, 2);
                //menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine;
                //menu = menu + "Somme d'argent:" + amount + Environment.NewLine;
                //double max_payout = Math.Round(total_ratio * Convert.ToDouble(amount), 2);
                //menu = menu + "Gain Maximum: " + max_payout + Environment.NewLine + Environment.NewLine;

                switch (lang)
                {
                    case "fr":
                        menu = menu + "Pour completer votre pari, confirmez le paiement ou composez *126#" + Environment.NewLine;
                        menu = menu + "Confirmez T&amp;C et +18 http://m.yellowbet.cm" + Environment.NewLine;
                        break;
                    case "en":
                        menu = menu + "To complete your bet, confirm the payment or dial *126#" + Environment.NewLine;
                        menu = menu + "Confirm T&amp;C +18 http://m.yellowbet.cm" + Environment.NewLine;
                        break;
                }
                
            }
            return menu;
        }

        public static string GetWrongPriceBetMeny(string lang, ref List<LogLines> lines)
        {
            string menu = "";
            switch (lang)
            {
                case "fr":
                    menu = menu + "Veuillez saisir un montant compris entre 200 FCFA et 1000000 FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "*) Retour ";
                    break;
                case "en":
                    menu = menu + "Please try an amount between 200 FCFA and 1000000 FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "*) Back ";
                    break;
            }

            return menu;
        }

        public static string GetWrongPriceBetRapidos(int max_bet, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            switch(lang)
            {
                case "en":
                    menu = "Please enter an amount between 200 FCFA to " + max_bet + " FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "*) Back";
                    break;
                case "fr":
                    menu = "Veuillez saisir un montant compris entre 200 FCFA et " + max_bet + " FCFA" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "*) Retour";
                    break;
            }
            
            return menu;
        }



        public static string GetPayAndConfirm(USSDSession ussd_session, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                double total_ratio = 1;
                switch (lang)
                {
                    case "fr":
                        menu = menu + "Entrer le montant:" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Numero des Jeux: " + saved_games.Count() + Environment.NewLine;
                        foreach (SavedGames r in saved_games)
                        {
                            total_ratio = total_ratio * r.selected_odd;
                        }
                        total_ratio = Math.Round(total_ratio, 2);
                        menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine + Environment.NewLine;
                        menu = menu + "*) Retour";
                        break;
                    case "en":
                        menu = menu + "Enter Amount:" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Number of games: " + saved_games.Count() + Environment.NewLine;
                        foreach (SavedGames r in saved_games)
                        {
                            total_ratio = total_ratio * r.selected_odd;
                        }
                        total_ratio = Math.Round(total_ratio, 2);
                        menu = menu + "Total Ratio: " + total_ratio + Environment.NewLine + Environment.NewLine;
                        menu = menu + "*) Back";
                        break;
                }
                
            }
            return menu;
        }

        public static string GetSportsTypeMenu(string lang, ref List<LogLines> lines)
        {
            string menu = "";
            switch(lang)
            {
                case "fr":
                    menu = menu + "Choisir le Sport:" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "1) Football" + Environment.NewLine;
                    menu = menu + "2) Basketball" + Environment.NewLine;
                    menu = menu + "3) Tennis" + Environment.NewLine;
                    menu = menu + "4) Rapidos" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                    break;
                case "en":
                    menu = menu + "Choose a game:" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "1) Football" + Environment.NewLine;
                    menu = menu + "2) Basketball" + Environment.NewLine;
                    menu = menu + "3) Tennis" + Environment.NewLine;
                    menu = menu + "4) Rapidos" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Main Menu ";
                    break;

            }
            
            return menu;
        }

        public static string GetEventsMenu(string ussd_string, int sport_type_id, int page_number, ref List<LogLines> lines, out int selected_league_id, int real_selected_league_id, USSDMenu ussd_menu, string lang)
        {
            selected_league_id = 0;

            string menu = "";
            List<SportEvents> sports_events = null;
            if (ussd_string == "1")
            {
                sports_events = Cache.iDoBet.GetMTNCameroonEventsFromCache(sport_type_id, ref lines);
            }
            else
            {
                string a = GetSoccerLeagueMenu(sport_type_id, ussd_string, ussd_menu.ussd_id, ref lines, out selected_league_id, lang);
                sports_events = Cache.iDoBet.GetMTNCameroonEventsWithLeagueIDFromCache((selected_league_id == 0 ? real_selected_league_id : selected_league_id), sport_type_id, ref lines);
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
                        menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next");
                    }
                    if (page_number > 1 && page_number < total_pages)
                    {
                        menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next") + Environment.NewLine;
                        menu = menu + (lang == "fr" ? "*) Retour " : "*) Back ");
                    }
                    if (page_number == total_pages && page_number > 1)
                    {
                        menu = menu + Environment.NewLine + (lang == "fr" ? "*) Retour " : "*) Back ");
                    }
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";

                }
            }


            return menu;
        }

        public static string GetEventsMenu(int sport_type_id, int page_number, string lang, ref List<LogLines> lines)
        {
            string menu = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetMTNCameroonEventsFromCache(sport_type_id, ref lines);
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
                        menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next");
                    }
                    if (page_number > 1 && page_number < total_pages)
                    {
                        menu = menu + Environment.NewLine + (lang == "fr" ? "#) Suivant" : "#) Next")+ Environment.NewLine;
                        menu = menu + (lang == "fr" ? "*) Retour" : "*) Back");
                    }
                    if (page_number == total_pages && page_number > 1)
                    {
                        menu = menu + Environment.NewLine + (lang == "fr" ? "*) Retour" : "*) Back");
                    }
                    menu = menu + Environment.NewLine + "M) ";// Menu Principal ";

                }
            }


            return menu;
        }

        public static string GetTokenNew(ref List<LogLines> lines)
        {
            string token_id = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            string iDoBetUserName = Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCameroon", ref lines);
            string iDoBetPassword = Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");


            iDoBetLoginUser request = new iDoBetLoginUser()
            {
                methodGroup = "Sport",
                methodName = "LoginUser",
                userName = iDoBetUserName,
                password = iDoBetPassword
            };

            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "9" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = null });
            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.isSuccessfull == true)
                    {
                        token_id = json_response.data.token;
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }

            }
            return token_id;
        }

        public static string GetTokenCheckTicketNew(ref List<LogLines> lines)
        {
            string token_id = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            string iDoBetUserName = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCameroon", ref lines);
            string iDoBetPassword = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");


            iDoBetLoginUser request = new iDoBetLoginUser()
            {
                methodGroup = "Sport",
                methodName = "LoginUser",
                userName = iDoBetUserName,
                password = iDoBetPassword
            };

            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "9" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = null });
            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.isSuccessfull == true)
                    {
                        token_id = json_response.data.token;
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }

            }
            return token_id;
        }

        public static string GetTokenNew(string iDoBetUserName, string iDoBetPassword, ref List<LogLines> lines)
        {
            string token_id = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrlMTNCameroon", ref lines);
            lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");


            iDoBetLoginUser request = new iDoBetLoginUser()
            {
                methodGroup = "Sport",
                methodName = "LoginUser",
                userName = iDoBetUserName,
                password = iDoBetPassword
            };

            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "BrandId", value = "24" });
            headers.Add(new Headers { key = "ChannelId", value = "9" });
            headers.Add(new Headers { key = "Language", value = "en" });
            headers.Add(new Headers { key = "Terminal", value = "" });
            headers.Add(new Headers { key = "ExtraData", value = "" });
            headers.Add(new Headers { key = "Token", value = null });
            string response_body = CommonFuncations.CallSoap.CallSoapRequest(idobet_endpoint, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    if (json_response.isSuccessfull == true)
                    {
                        token_id = json_response.data.token;
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                }

            }
            return token_id;
        }
    }
}