using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.BtoBet;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class USSDBtoBet
    {
        public static string MoovUSSDBehaviuer(ServiceClass service, string ussdString, string ServiceID, string MSISDN, string linkid, string receiveCB, string senderCB, string serviceCode, USSDMenu ussd_menu, USSDSession ussd_session, out DYAReceiveMoneyRequest momo_request, ref List<LogLines> lines, out string menu_2_display, out bool is_close, string force_session_id)
        {
            momo_request = null;
            menu_2_display = "";
            is_close = false;
            string result = "";
            force_session_id = (String.IsNullOrEmpty(force_session_id) ? Guid.NewGuid().ToString() : force_session_id);
            string user_session_id = (ussd_session == null ? force_session_id : ussd_session.user_seesion_id);
            int topic_id = (ussd_session != null ? ussd_session.topic_id : ussd_menu.topic_id);
            if (!String.IsNullOrEmpty(ussd_menu.menu_2_display))
            {
                menu_2_display = ussd_menu.menu_2_display;
                result = ussd_menu.menu_2_display;
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", ref lines);
            }
            if (ussd_menu.action_id == 3) //close
            {
                menu_2_display = ussd_menu.menu_2_display;
                is_close = true;
                result = ussd_menu.menu_2_display;
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", ref lines);
            }

            if (String.IsNullOrEmpty(result))
            {
                switch (topic_id)
                {
                    case 6:
                        int page_number = (ussd_session != null ? ussd_session.page_number : 1);
                        int odd_page = (ussd_session != null ? ussd_session.odd_page : 0);
                        Int64 game_id = (ussd_session != null ? ussd_session.game_id : 0);
                        Int64 event_id = (ussd_session != null ? ussd_session.event_id : 0);
                        double selected_odd = (ussd_session != null ? ussd_session.selected_odd : 0);
                        int selected_bet_type_id = (ussd_session != null ? ussd_session.selected_bet_type_id : 0);
                        string selected_odd_name = (ussd_session != null ? ussd_session.selected_odd_name : "0");
                        string selected_odd_line = (ussd_session != null ? ussd_session.selected_odd_line : "null");
                        string amount = (ussd_session != null ? ussd_session.amount : "0");
                        int selected_league_id = (ussd_session != null ? ussd_session.selected_league_id : 0);
                        string msgType = "1", opType = "1";
                        int status = 0;
                        string amount_2_pay = (ussd_session != null ? ussd_session.amount_2_pay.ToString() : "0");
                        string bar_code = (ussd_session != null ? ussd_session.bar_code : "0");
                        string selected_subagent_name = (ussd_session != null ? ussd_session.selected_subagent_name : "0");
                        BtoBetUser user = new BtoBetUser();
                        switch (ussd_menu.action_id)
                        {

                            case 34: //DepositMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetDepositMoneyMenu(service, user, ussd_menu, MSISDN, ref lines);
                                break;
                            case 36: //StartDepositMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartDepositMoneyMenu(service, user, ussd_menu, MSISDN, ussdString, out momo_request, ref lines);
                                break;
                            case 33: //WithdrawMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetWithdrawMoneyMenu(user, ref lines);
                                break;
                            case 35: //StartWithdrawMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                if (user.IsSuccessful == true)
                                {
                                    if (Convert.ToInt32(ussdString) <= user.Real && Convert.ToInt32(ussdString) > 0)
                                    {
                                        menu_2_display = Api.CommonFuncations.BtoBet.CheckBeforeWithdrawMenu(service, MSISDN, ref lines);
                                        if (String.IsNullOrEmpty(menu_2_display))
                                        {
                                            //Refund user
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
                                                    string order_id = Guid.NewGuid().ToString();
                                                    DYATransferMoneyRequest momotransfer_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = Convert.ToInt64(MSISDN),
                                                        Amount = Convert.ToInt32(ussdString),
                                                        ServiceID = service.service_id,
                                                        TokenID = token_id,
                                                        TransactionID = "USSDWithdraw_" + order_id
                                                    };
                                                    string postBody = "", response_body = "";
                                                    //bool wd_result = StartWithdraw(user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                    bool do_withdraw = false;
                                                    string code = "";
                                                    if (user.Real > 0)
                                                    {
                                                        int fee = 0;
                                                        string transaction_id = WithdrawMoney(service, MSISDN, Convert.ToInt32(ussdString), order_id, out fee, ref lines);
                                                        momotransfer_request.Amount = (momotransfer_request.Amount - fee);
                                                        if (!String.IsNullOrEmpty(transaction_id))
                                                        {
                                                            DYATransferMoneyResponse momotransfer_response = CommonFuncations.DYATransferMoney.DoTransfer(momotransfer_request);
                                                            if (momotransfer_response.ResultCode == 1000)
                                                            {
                                                                postBody = "";
                                                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, true, false, false, ref lines);
                                                                GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(MSISDN), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "Withdraw", "MOMO", "-" + ussdString, "/", ref lines);
                                                                Api.DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id, service_id) values ("+momotransfer_response.TransactionID+","+transaction_id+","+service.service_id+")", ref lines);
                                                            }
                                                            else
                                                            {
                                                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                                                //Todo refund
                                                                string myorder_id = (momotransfer_response.TransactionID != "-1" ? "RefundUSSDWithdraw_" + momotransfer_response.TransactionID : "RefundUSSDWithdraw_" + order_id);
                                                                string deposit_trans = DepositMoney(service, MSISDN, Convert.ToInt32(ussdString), myorder_id, ref lines);
                                                                bool user_wasrefund = (!String.IsNullOrEmpty(deposit_trans) ? true : false);

                                                                postBody = "";
                                                                response_body = "";
                                                                string mail_body = "", mail_subject = "";
                                                                mail_body = "<p><h2>Withdraw Moov has failed DYATransfer</h2><b>UserID:</b> " + user.ExternalID + "<br><b>Name:</b> " + user.FirstName + " " + user.LastName + "<br><b>Amount: </b>" + ussdString + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>MOMO Response: </b>" + momotransfer_response.ResultCode + " " + momotransfer_response.Description + "<br>User was refunded: "+ user_wasrefund + " ("+deposit_trans+")<br><b>Request: </b>" + postBody + "<br><b>Response: </b>" + response_body + "</p>";
                                                                mail_subject = "Withdraw Moov has failed DYATransfer for user - " + user.ExternalID;
                                                                string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, true, ref lines);
                                                    }
                                                }
                                                
                                            }
                                        }


                                    }
                                    else
                                    {
                                        //user has requested more than allowed
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, true, ref lines);
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
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                status = 1;
                                menu_2_display = CloseBet(service, MSISDN, ussd_session, out momo_request, ref lines);
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
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmBet(ussd_session, amount, ref lines);
                                    }
                                    else
                                    {

                                        menu_2_display = Api.CommonFuncations.BtoBet.GetWrongPriceBetMeny(ref lines);
                                        DataLayer.DBQueries.ExecuteQuery("delete from ussd_saved_games where user_session_id = '" + user_session_id + "' order by id desc limit 1", ref lines);
                                    }
                                }
                                odd_page = 0;
                                break;
                            case 19: //PayAndConfirmXXX
                            case 20:
                            case 21:
                                //need to check if user clicks back and then returns to this section.
                                DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", "+event_id+")", ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetPayAndConfirm(ussd_session, ref lines);
                                odd_page = 0;
                                break;
                            case 32: //ticket status
                                page_number = (page_number == 0 ? 1 : page_number);
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketMenu(service, user, page_number, ussdString, ref lines);
                                break;
                            case 31: //tickets status by phone
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    page_number = page_number + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && page_number > 1)
                                {
                                    page_number = page_number - 1;
                                }
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketMenu(service, user, page_number, ref lines);
                                break;
                            case 44: //search ticket
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketByBarcodeMenu(service, user, ussdString, out amount_2_pay, ref lines);
                                bar_code = ussdString;
                                break;
                            case 39: //add another game
                                if (game_id > 0)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", "+event_id+")", ref lines);
                                    game_id = 0;
                                    selected_odd = 0;
                                    page_number = 1;
                                    odd_page = 0;
                                }
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSportsTypeMenu(ref lines);
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
                                event_id = 0;
                                odd_page = 0;
                                selected_odd = 0;
                                page_number = 1;
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSportsTypeMenu(ref lines);
                                ussd_menu.action_id = 2;
                                break;
                            case 48: //Tennis
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 5, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 47: //basketball 
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 2, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 37: //display soccer leagu
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 1, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 65://Highlights Games
                            case 66:
                            case 67:
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
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    page_number = page_number + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && page_number > 0)
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
                                    event_id = 0;
                                    selected_odd = 0;
                                    odd_page = 0;
                                    page_number = 1;

                                }
                                if (ussd_menu.action_id == 16 || ussd_menu.action_id == 17 || ussd_menu.action_id == 18)
                                {
                                    if (game_id > 0)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", "+event_id+")", ref lines);
                                        game_id = 0;
                                        event_id = 0;
                                        selected_odd = 0;
                                        page_number = 1;
                                        odd_page = 0;
                                    }

                                }
                                game_id = 0;
                                selected_odd = 0;
                                odd_page = 0;
                                switch (ussd_menu.action_id)
                                {
                                    case 38:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 49:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 50:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 28:
                                    case 16:
                                    case 7:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 29:
                                    case 17:
                                    case 8:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 30:
                                    case 18:
                                    case 9:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,2);
                                        break;
                                    case 65:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,1);
                                        break;
                                    case 66:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,1);
                                        break;
                                    case 67:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu,1);
                                        break;

                                }


                                break;
                            case 68: //DisplayXXXHighlightsGame
                            case 69:
                            case 70:
                            case 10: //DisplayXXXGame
                            case 11:
                            case 12:
                                page_number = (page_number == 0 ? 1 : page_number);
                                selected_odd = 0;
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    odd_page = odd_page + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && odd_page > 0)
                                {
                                    odd_page = odd_page - 1;
                                }
                                switch (ussd_menu.action_id)
                                {
                                    case 10:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 1, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 11:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 2, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 12:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 5, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 68:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 1, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 69:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 2, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 70:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 5, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                }
                                break;
                            case 13: //ConfirmXXXGame (select odd)
                            case 14:
                            case 15:
                                switch (ussd_menu.action_id)
                                {
                                    case 13:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 1, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 14:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 2, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 15:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 5, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                }
                                break;
                        }
                        lines = Add2Log(lines, " page_number = " + page_number, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " game_id = " + game_id, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " event_id = " + event_id, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " odd_page = " + odd_page, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_odd = " + selected_odd, 100, "ivr_subscribe");
                        int number;
                        bool c = Int32.TryParse(amount, out number);
                        amount = (c ? (number < 200 ? "0" : amount) : "0");

                        lines = Add2Log(lines, " amount = " + amount, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_league_id = " + selected_league_id, 100, "ivr_subscribe");
                        result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, msgType, opType);
                        
                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id, selected_odd, selected_bet_type_id,selected_odd_name, selected_odd_line, amount, selected_league_id, amount_2_pay, bar_code, selected_subagent_name, event_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + "," + status + ", '" + ussdString + "', " + ussd_menu.action_id + ", " + page_number + "," + odd_page + "," + game_id + "," + topic_id + ",'" + user_session_id + "'," + selected_odd + "," + selected_bet_type_id + ",'" + selected_odd_name + "','" + selected_odd_line + "', " + amount + "," + selected_league_id + "," + amount_2_pay + ",'" + bar_code + "','" + selected_subagent_name + "', "+event_id+");", ref lines);
                        break;
                }
            }

            return result;
        }

        public static string MTNCameroonUSSDBehaviuer(ServiceClass service, string ussdString, string ServiceID, string MSISDN, string linkid, string receiveCB, string senderCB, string serviceCode, USSDMenu ussd_menu, USSDSession ussd_session, out DYAReceiveMoneyRequest momo_request, ref List<LogLines> lines, out string menu_2_display, out bool is_close, string force_session_id)
        {
            momo_request = null;
            menu_2_display = "";
            is_close = false;
            string result = "";
            force_session_id = (String.IsNullOrEmpty(force_session_id) ? Guid.NewGuid().ToString() : force_session_id);
            string user_session_id = (ussd_session == null ? force_session_id : ussd_session.user_seesion_id);
            int topic_id = (ussd_session != null ? ussd_session.topic_id : ussd_menu.topic_id);
            if (!String.IsNullOrEmpty(ussd_menu.menu_2_display))
            {
                menu_2_display = ussd_menu.menu_2_display;
                result = ussd_menu.menu_2_display;
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",0, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", ref lines);
            }
            if (ussd_menu.action_id == 3) //close
            {
                menu_2_display = ussd_menu.menu_2_display;
                is_close = true;
                result = ussd_menu.menu_2_display;
                DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + ",1, '" + ussdString + "', " + ussd_menu.action_id + ", " + (ussd_session == null ? 0 : ussd_session.page_number) + "," + (ussd_session == null ? 0 : ussd_session.odd_page) + "," + (ussd_session == null ? 0 : ussd_session.game_id) + "," + topic_id + ",'" + user_session_id + "');", ref lines);
            }

            if (String.IsNullOrEmpty(result))
            {
                switch (topic_id)
                {
                    case 11:
                        int page_number = (ussd_session != null ? ussd_session.page_number : 1);
                        int odd_page = (ussd_session != null ? ussd_session.odd_page : 0);
                        Int64 game_id = (ussd_session != null ? ussd_session.game_id : 0);
                        Int64 event_id = (ussd_session != null ? ussd_session.event_id : 0);
                        double selected_odd = (ussd_session != null ? ussd_session.selected_odd : 0);
                        int selected_bet_type_id = (ussd_session != null ? ussd_session.selected_bet_type_id : 0);
                        string selected_odd_name = (ussd_session != null ? ussd_session.selected_odd_name : "0");
                        string selected_odd_line = (ussd_session != null ? ussd_session.selected_odd_line : "null");
                        string amount = (ussd_session != null ? ussd_session.amount : "0");
                        int selected_league_id = (ussd_session != null ? ussd_session.selected_league_id : 0);
                        string msgType = "1", opType = "1";
                        int status = 0;
                        string amount_2_pay = (ussd_session != null ? ussd_session.amount_2_pay.ToString() : "0");
                        string bar_code = (ussd_session != null ? ussd_session.bar_code : "0");
                        string selected_subagent_name = (ussd_session != null ? ussd_session.selected_subagent_name : "0");
                        BtoBetUser user = new BtoBetUser();
                        switch (ussd_menu.action_id)
                        {

                            case 34: //DepositMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetDepositMoneyMenu(service, user, ussd_menu, MSISDN, ref lines);
                                break;
                            case 36: //StartDepositMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartDepositMoneyMenu(service, user, ussd_menu, MSISDN, ussdString, out momo_request, ref lines);
                                break;
                            case 33: //WithdrawMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetWithdrawMoneyMenu(user, ref lines);
                                break;
                            case 35: //StartWithdrawMoney
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                if (user.IsSuccessful == true)
                                {
                                    if (Convert.ToInt32(ussdString) <= user.Real && Convert.ToInt32(ussdString) > 0)
                                    {
                                        menu_2_display = Api.CommonFuncations.BtoBet.CheckBeforeWithdrawMenu(service, MSISDN, ref lines);
                                        if (String.IsNullOrEmpty(menu_2_display))
                                        {
                                            //Refund user
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
                                                    string order_id = Guid.NewGuid().ToString();
                                                    DYATransferMoneyRequest momotransfer_request = new DYATransferMoneyRequest()
                                                    {
                                                        MSISDN = Convert.ToInt64(MSISDN),
                                                        Amount = Convert.ToInt32(ussdString),
                                                        ServiceID = service.service_id,
                                                        TokenID = token_id,
                                                        TransactionID = "USSDWithdraw_" + order_id
                                                    };
                                                    string postBody = "", response_body = "";
                                                    //bool wd_result = StartWithdraw(user, MSISDN, Convert.ToDouble(ussdString), out postBody, out response_body, ref lines);
                                                    bool do_withdraw = false;
                                                    string code = "";
                                                    if (user.Real > 0)
                                                    {
                                                        int fee = 0;
                                                        string transaction_id = WithdrawMoney(service, MSISDN, Convert.ToInt32(ussdString), order_id, out fee, ref lines);
                                                        momotransfer_request.Amount = (momotransfer_request.Amount - fee);
                                                        if (!String.IsNullOrEmpty(transaction_id))
                                                        {
                                                            DYATransferMoneyResponse momotransfer_response = CommonFuncations.DYATransferMoney.DoTransfer(momotransfer_request);
                                                            if (momotransfer_response.ResultCode == 1000)
                                                            {
                                                                postBody = "";
                                                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, true, false, false, ref lines);
                                                                GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(MSISDN), System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], "BJ", "event", "Withdraw", "MOMO", "-" + ussdString, "/", ref lines);
                                                                Api.DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id, service_id) values (" + momotransfer_response.TransactionID + "," + transaction_id + "," + service.service_id + ")", ref lines);
                                                            }
                                                            else
                                                            {
                                                                menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                                                //Todo refund
                                                                string myorder_id = (momotransfer_response.TransactionID != "-1" ? "RefundUSSDWithdraw_" + momotransfer_response.TransactionID : "RefundUSSDWithdraw_" + order_id);
                                                                string deposit_trans = DepositMoney(service, MSISDN, Convert.ToInt32(ussdString), myorder_id, ref lines);
                                                                bool user_wasrefund = (!String.IsNullOrEmpty(deposit_trans) ? true : false);

                                                                postBody = "";
                                                                response_body = "";
                                                                string mail_body = "", mail_subject = "";
                                                                mail_body = "<p><h2>Withdraw Moov has failed DYATransfer</h2><b>UserID:</b> " + user.ExternalID + "<br><b>Name:</b> " + user.FirstName + " " + user.LastName + "<br><b>Amount: </b>" + ussdString + "<br><b>MSISDN: </b>" + MSISDN + "<br><b>MOMO Response: </b>" + momotransfer_response.ResultCode + " " + momotransfer_response.Description + "<br>User was refunded: " + user_wasrefund + " (" + deposit_trans + ")<br><b>Request: </b>" + postBody + "<br><b>Response: </b>" + response_body + "</p>";
                                                                mail_subject = "Withdraw Moov has failed DYATransfer for user - " + user.ExternalID;
                                                                string emails = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailRecipients", ref lines);
                                                                string sender_email = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderEmail", ref lines);
                                                                string sender_name = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderName", ref lines);
                                                                string sender_assword = Api.Cache.ServerSettings.GetServerSettings("BtoBetSenderPassword", ref lines);
                                                                int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailPort", ref lines));
                                                                string email_host = Api.Cache.ServerSettings.GetServerSettings("BtoBetEmailHost", ref lines);
                                                                CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                                                            }

                                                        }
                                                        else
                                                        {
                                                            menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, false, ref lines);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, true, ref lines);
                                                    }
                                                }

                                            }
                                        }


                                    }
                                    else
                                    {
                                        //user has requested more than allowed
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetStartWithdrawMoneyMenu(user, false, false, true, ref lines);
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
                                msgType = "2";
                                opType = "2";
                                is_close = true;
                                status = 1;
                                menu_2_display = CloseBet(service, MSISDN, ussd_session, out momo_request, ref lines);
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
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmBet(ussd_session, amount, ref lines);
                                    }
                                    else
                                    {

                                        menu_2_display = Api.CommonFuncations.BtoBet.GetWrongPriceBetMeny(ref lines);
                                        DataLayer.DBQueries.ExecuteQuery("delete from ussd_saved_games where user_session_id = '" + user_session_id + "' order by id desc limit 1", ref lines);
                                    }
                                }
                                odd_page = 0;
                                break;
                            case 19: //PayAndConfirmXXX
                            case 20:
                            case 21:
                                //need to check if user clicks back and then returns to this section.
                                DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", " + event_id + ")", ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetPayAndConfirm(ussd_session, ref lines);
                                odd_page = 0;
                                break;
                            case 32: //ticket status
                                page_number = (page_number == 0 ? 1 : page_number);
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketMenu(service, user, page_number, ussdString, ref lines);
                                break;
                            case 31: //tickets status by phone
                                page_number = (page_number == 0 ? 1 : page_number);
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    page_number = page_number + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && page_number > 1)
                                {
                                    page_number = page_number - 1;
                                }
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketMenu(service, user, page_number, ref lines);
                                break;
                            case 44: //search ticket
                                user = Api.CommonFuncations.BtoBet.SearchUser(service, MSISDN, ref lines);
                                menu_2_display = Api.CommonFuncations.BtoBet.GetCheckTicketByBarcodeMenu(service, user, ussdString, out amount_2_pay, ref lines);
                                bar_code = ussdString;
                                break;
                            case 39: //add another game
                                if (game_id > 0)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", " + event_id + ")", ref lines);
                                    game_id = 0;
                                    selected_odd = 0;
                                    page_number = 1;
                                    odd_page = 0;
                                }
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSportsTypeMenu(ref lines);
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
                                event_id = 0;
                                odd_page = 0;
                                selected_odd = 0;
                                page_number = 1;
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSportsTypeMenu(ref lines);
                                ussd_menu.action_id = 2;
                                break;
                            case 48: //Tennis
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 5, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 47: //basketball 
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 2, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 37: //display soccer leagu
                                menu_2_display = Api.CommonFuncations.BtoBet.GetSoccerLeagueMenu(service, 1, "0", ussd_menu.ussd_id, ref lines, out selected_league_id);
                                break;
                            case 65://Highlights Games
                            case 66:
                            case 67:
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
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    page_number = page_number + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && page_number > 0)
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
                                    event_id = 0;
                                    selected_odd = 0;
                                    odd_page = 0;
                                    page_number = 1;

                                }
                                if (ussd_menu.action_id == 16 || ussd_menu.action_id == 17 || ussd_menu.action_id == 18)
                                {
                                    if (game_id > 0)
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn, game_id, odd_page, selected_ussd_string, date_time, status, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, service_id, event_id) values(" + MSISDN + ", " + game_id + ", " + odd_page + ", '" + ussdString + "', now(), 0,'" + user_session_id + "'," + selected_odd + ", " + selected_bet_type_id + ", '" + selected_odd_name + "', '" + selected_odd_line + "',0, " + service.service_id + ", " + event_id + ")", ref lines);
                                        game_id = 0;
                                        event_id = 0;
                                        selected_odd = 0;
                                        page_number = 1;
                                        odd_page = 0;
                                    }

                                }
                                game_id = 0;
                                selected_odd = 0;
                                odd_page = 0;
                                switch (ussd_menu.action_id)
                                {
                                    case 38:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 49:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 50:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetLeaguEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 28:
                                    case 16:
                                    case 7:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 29:
                                    case 17:
                                    case 8:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 30:
                                    case 18:
                                    case 9:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 2);
                                        break;
                                    case 65:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 1, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 1);
                                        break;
                                    case 66:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 2, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 1);
                                        break;
                                    case 67:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetEventsMenu(service, ussdString, 5, page_number, ref lines, out selected_league_id, selected_league_id, ussd_menu, 1);
                                        break;

                                }


                                break;
                            case 68: //DisplayXXXHighlightsGame
                            case 69:
                            case 70:
                            case 10: //DisplayXXXGame
                            case 11:
                            case 12:
                                page_number = (page_number == 0 ? 1 : page_number);
                                selected_odd = 0;
                                if (ussdString.Contains("#") || ussdString.Contains("9"))
                                {
                                    odd_page = odd_page + 1;
                                }
                                if ((ussdString.Contains("*") || ussdString.Contains("8")) && odd_page > 0)
                                {
                                    odd_page = odd_page - 1;
                                }
                                switch (ussd_menu.action_id)
                                {
                                    case 10:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 1, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 11:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 2, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 12:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetGameOddsMenu(service, 5, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 68:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 1, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 69:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 2, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                    case 70:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetHighlightGameOddsMenu(service, 5, page_number, ussdString, game_id, odd_page, out game_id, out event_id, selected_league_id, ref lines);
                                        break;
                                }
                                break;
                            case 13: //ConfirmXXXGame (select odd)
                            case 14:
                            case 15:
                                switch (ussd_menu.action_id)
                                {
                                    case 13:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 1, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 14:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 2, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                    case 15:
                                        menu_2_display = Api.CommonFuncations.BtoBet.GetConfirmMenu(service, 5, game_id, ussdString, odd_page, out selected_odd, out selected_bet_type_id, out selected_odd_name, out selected_odd_line, ref lines);
                                        break;
                                }
                                break;
                        }
                        lines = Add2Log(lines, " page_number = " + page_number, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " game_id = " + game_id, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " event_id = " + event_id, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " odd_page = " + odd_page, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_odd = " + selected_odd, 100, "ivr_subscribe");
                        int number;
                        bool c = Int32.TryParse(amount, out number);
                        amount = (c ? (number < 200 ? "0" : amount) : "0");

                        lines = Add2Log(lines, " amount = " + amount, 100, "ivr_subscribe");
                        lines = Add2Log(lines, " selected_league_id = " + selected_league_id, 100, "ivr_subscribe");
                        result = USSD.BuildSendUSSDSoap(service, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, menu_2_display, msgType, opType);

                        DataLayer.DBQueries.ExecuteQuery("insert into ussd_sessions (msisdn, ussd_id, date_time, menu_id, status, selected_ussdstring, action_id, page_number, odd_page, game_id, topic_id, user_session_id, selected_odd, selected_bet_type_id,selected_odd_name, selected_odd_line, amount, selected_league_id, amount_2_pay, bar_code, selected_subagent_name, event_id) value(" + MSISDN + ", " + ussd_menu.ussd_id + ",now(), " + ussd_menu.menu_id + "," + status + ", '" + ussdString + "', " + ussd_menu.action_id + ", " + page_number + "," + odd_page + "," + game_id + "," + topic_id + ",'" + user_session_id + "'," + selected_odd + "," + selected_bet_type_id + ",'" + selected_odd_name + "','" + selected_odd_line + "', " + amount + "," + selected_league_id + "," + amount_2_pay + ",'" + bar_code + "','" + selected_subagent_name + "', " + event_id + ");", ref lines);
                        break;
                }
            }

            return result;
        }
    }
}