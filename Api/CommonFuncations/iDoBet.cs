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
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class iDoBet
    {
        public class PlaceBetRequest
        {
            public List<rows> rows { get; set; }
            public List<selections> selections { get; set; }
            public string ExternalId { get; set; }
        }

        public class iDoBetLoginUser
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string userName { get; set; }
            public string password { get; set; }
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
            public int bulkSize { get; set; }
            public bool checkBalnace { get; set; }
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

        public class PayCommisionRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public int toUserId { get; set; }
            public double amount { get; set; }
            public string comments { get; set; }
            public string externalTransactionId { get; set; }
            public bool autoApprove { get; set; }
            public string transactionExtraData { get; set; }
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

        public class RequestForOrder
        {
            public string TerminalSecurityCode { get; set; }
            public string ExternalID { get; set; }
            public string userToken { get; set; }
            public List<rows> rows { get; set; }
            public List<selections> selections { get; set; }
            public bool IsBooking { get; set; }

        }

        public class rows
        {
            public string amount { get; set; }
            public string[] selectionKeys { get; set; }
        }

        public class selections
        {
            public Int64 eventId { get; set; }
            public int betTypeId { get; set; }
            public string oddName { get; set; }
            public string oddLine { get; set; }
            public string oddPrice { get; set; }
            public string key { get; set; }
        }

        public class ExecuteOrder
        {
            public string userToken { get; set; }
            public string timeoutGuid { get; set; }
        }

        public class ExecuteOrderRequest
        {
            public string timeoutGuid { get; set; }
            public int externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
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

        public class SearchUserRequest
        {
            public string token { get; set; }
            public string searchBy { get; set; }
            public string searchFor { get; set; }
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
            public string msgTransactionFee { get; set; }      // optional content to send to user for their transaction fee

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

        public class BeginWithDrawRequest
        {
            public string userToken { get; set; }
            public string userId { get; set; }
            public string iPin { get; set; }
            public double amount { get; set; }
        }

        public class WithDrawRequest
        {
            public string userToken { get; set; }
            public string userId { get; set; }
            public string code { get; set; }
            public string comments { get; set; }
            public string password { get; set; }
            public string externalID { get; set; }
        }

        public class DepositRequest
        {
            public string userToken { get; set; }
            public double Amount { get; set; }
            public string ToUserId { get; set; }
            public string iPin { get; set; }
            public string Password { get; set; }


        }

        public class DoPayOutResponse
        {
            public string userToken { get; set; }
            public string barcode { get; set; }
            public string terminalSecurityCode { get; set; }
        }

        public class DoPayOutRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string barcode { get; set; }
            public string externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
        }

        public class SubAgentsRequest
        {
            public string MethodGroup { get; set; }
            public string MethodName { get; set; }
            public int BranchIds { get; set; }
        }

        public class SubAgentsResponse
        {
            public string subagent_id { get; set; }
            public string subagent_username { get; set; }
            public string available_credit { get; set; }
            public int page_number { get; set; }
            public string commision { get; set; }
            public string credit_limit { get; set; }
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

        public class UpdateTransactionRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string transactionId { get; set; }
            public string externalTransactionId { get; set; }
            public string transactionExtraData { get; set; }
        }

        public class CollectSendMoneyRequest
        {
            public string MethodGroup { get; set; }
            public string MethodName { get; set; }
            public int UserId { get; set; }
            public int Amount { get; set; }
            public string ExternalID { get; set; }
            public string TransactionType { get; set; }
            public string Comments { get; set; }
        }

        public class iDoBetTransactionRequest
        {
            public string methodGroup { get; set; }
            public string methodName { get; set; }
            public string externalTransactionId { get; set; }
        }

        public static bool DoPayout(string barcode, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = Api.Cache.iDoBet.GetiDoBetUserToken(0, ref lines);

            if (!String.IsNullOrEmpty(token_id))
            {
                DoPayOutResponse payout_response = new DoPayOutResponse()
                {
                    userToken = token_id,
                    barcode = barcode,
                    terminalSecurityCode = Api.Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCode", ref lines)
                };
                string payout_url = Api.Cache.ServerSettings.GetServerSettings("iDoBetdoPayoutUrl", ref lines);
                string postBody = JsonConvert.SerializeObject(payout_response);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(payout_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");

                if (!String.IsNullOrEmpty(response_body))
                {
                    

                    try
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        if (json_response.isSuccessfull == true)
                        {
                            result = true;
                        }
                        else
                        {
                            string mail_body = "<p><b>DoPayout has failed for</b><br> <b>Barcode:</b> " + barcode + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "Warning from DoPayout - " + barcode;
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

        public static bool StartDeposit(IdoBetUser user, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            response_body = "";
            postBody = "";
            if (!String.IsNullOrEmpty(token_id))
            {
                DepositRequest dep_request = new DepositRequest()
                {
                    userToken = token_id,
                    ToUserId = user.id.ToString(),
                    iPin = user.iPin.ToString(),
                    Amount = amount,
                    Password = Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword", ref lines)
                };
                string dep_url = Cache.ServerSettings.GetServerSettings("iDoBetDepositUrl", ref lines);
                postBody = JsonConvert.SerializeObject(dep_request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                response_body = CommonFuncations.CallSoap.CallSoapRequest(dep_url, postBody, headers, 2, true, ref lines);
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

        

        public static bool StartDepositNew(IdoBetUser user, string dya_id, string time_stamp, double amount, string msisdn, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            response_body = "";
            postBody = "";
            if (!String.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

        public static string GetStartWithdrawMoneyMenu(IdoBetUser user, bool refund_money, bool w_result, bool amount_2_big, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.isValid == false)
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

       
        public static bool StartWithdraw(IdoBetUser user, string msisdn, double amount, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            postBody = "";
            response_body = "";
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                BeginWithDrawRequest bwd_request = new BeginWithDrawRequest()
                {
                    userToken = token_id,
                    userId = user.id.ToString(),
                    iPin = user.iPin.ToString(),
                    amount = amount
                };
                string bwd_url = Cache.ServerSettings.GetServerSettings("iDoBetBWDUrl", ref lines);
                postBody = JsonConvert.SerializeObject(bwd_request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                response_body = CommonFuncations.CallSoap.CallSoapRequest(bwd_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");

                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        if (!String.IsNullOrEmpty(json_response.code.ToString()))
                        {
                            string code = json_response.code;
                            //:TODO run transferinfo in order to get the real amount (requestAmount)
                            WithDrawRequest wd_request = new WithDrawRequest()
                            {
                                userToken = token_id,
                                userId = user.id.ToString(),
                                code = code,
                                comments = "Remboursement",
                                password = Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword", ref lines),
                                externalID = "+" + msisdn
                            };
                            string wd_url = Cache.ServerSettings.GetServerSettings("iDoBetWDUrl", ref lines);
                            postBody = JsonConvert.SerializeObject(wd_request);
                            lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                            headers = new List<Headers>();
                            response_body = CommonFuncations.CallSoap.CallSoapRequest(wd_url, postBody, headers, 2, true, ref lines);
                            lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                            if (!String.IsNullOrEmpty(response_body))
                            {
                                json_response = JsonConvert.DeserializeObject(response_body);
                                if (json_response.amount > 0)
                                {
                                    result = true;
                                }
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
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

        

        public static bool UpdateTransaction(string time_stamp, string dya_id, string transactionId, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            bool result = false;
            postBody = "";
            response_body = "";
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");

                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
                menu = menu + "taper *400# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                menu = menu + "M) Menu Principal ";
            }
            return menu;
        }

        


        public static Int64 CollectSendMoney(string token, string subagent_id, string action, string comments, string amount, ref List<LogLines> lines)
        {
            Int64 trans_id = 0;
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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
                        string mail_body = "<p><b>Pay Commision has failed</b><br> <b>UserID:</b> " + user_id + "<br><br> <b>Amount:</b> " + amount + "<br><br><b>Request:</b> "+ postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
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
            string token_id = Api.Cache.iDoBet.GetiDoBetUserToken(0, ref lines);

            if (!String.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                headers.Add(new Headers { key = "Terminal", value = Api.Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCode", ref lines) });

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
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values("+dya_id+","+trans_id+")", ref lines);
                            }
                        }
                        else
                        {
                            string mail_body = "<p><b>DoPayout has failed for</b><br> <b>Barcode:</b> " + bar_code + "<br><br> <b>Request:</b> " + postBody + "<br><br> <b>Response:</b> " + response_body + "<br></p>";
                            string mail_subject = "Warning from DoPayout - " + bar_code;
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

        public static List<SubAgentsResponse> GetSubAgents(iDoBetAgents agent, string token, ref List<LogLines> lines)
        {

            List<SubAgentsResponse> result = null;
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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
                                pos_trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into pos_requests (agent_name, sub_agent_name, date_time, amount) values('"+agent.agent_username+"','"+selected_subagent_name+"',now(), "+ussd_string+")", ref lines);
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
                                    menu = "Vous êtes sur le point de collecter de l'argent auprès d'" + filtered_sub_agent.subagent_username + Environment.NewLine;
                                    menu = menu + "entrer le montant de l'argent" + Environment.NewLine;
                                    menu = menu + "maximum " + available_credit + " CFA" + Environment.NewLine;
                                    menu = menu + Environment.NewLine + "*) Retour";
                                }
                                else
                                {
                                    menu = "Vous ne pouvez pas collecter 0 CFA auprès d'" + filtered_sub_agent.subagent_username + Environment.NewLine;
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
                                menu = menu + "sélectionner un SubAgent:" + Environment.NewLine;

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

        public static string GetDepositMoneyMenu(IdoBetUser user, USSDMenu ussd_menu, string MSISDN, ref List<LogLines> lines)
        {
            string menu = "";
            bool user_has_momo = false;
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site http://m.yellowbet.com" + Environment.NewLine + Environment.NewLine;
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
                menu = "Bonjour " + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
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
                    menu = menu + "Entrer votre montant" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "Minimum: 200 GNF" + Environment.NewLine;
                    menu = menu + "Maximum: 1,000,000 GNF" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }
                else
                {
                    menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                    menu = menu + "taper *400# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }

                
            }
            return menu;
        }

        public static string GetStartDepositMoneyMenu(ServiceClass service, IdoBetUser user, USSDMenu ussd_menu, string MSISDN, string amount, out DYAReceiveMoneyRequest momo_request, ref List<LogLines> lines)
        {
            momo_request = null;
            string menu = "";
            string token_id = "";
            bool user_has_momo = false;
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site http://m.yellowbet.com" + Environment.NewLine + Environment.NewLine;
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
                menu = "Bonjour " + user.firstName + " " + user.lastName + Environment.NewLine + Environment.NewLine;
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
                            TransactionID = "iDoBetDeposit_" + Guid.NewGuid().ToString().Substring(0,10).Replace("-","")

                        };
                        menu = menu + "Vous avez presque fini" + Environment.NewLine;
                        menu = menu + "Pour compléter votre pari, confirmez le paiement ou composez *400#, puis sélectionnez (8)" + Environment.NewLine;
                    }
                    else
                    {
                        menu = menu + "Entrer votre montant" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "Minimum: 200 GNF" + Environment.NewLine;
                        menu = menu + "Maximum: 1,000,000 GNF" + Environment.NewLine + Environment.NewLine;
                        menu = menu + "M) Menu Principal ";
                    }
                    
                }
                else
                {
                    menu = menu + "Si vous n'avez pas de compte MOMO," + Environment.NewLine;
                    menu = menu + "taper *400# pour ouvrir un compte MOMO" + Environment.NewLine + Environment.NewLine;
                    menu = menu + "M) Menu Principal ";
                }


            }
            return menu;
        }

        public static string GetWithdrawMoneyMenu(IdoBetUser user, ref List<LogLines> lines)
        {
            string menu = "";
            if (user.isValid == false)
            {
                if (String.IsNullOrEmpty(user.firstName))
                {
                    menu = "Rejoigner YellowBet pour gagner d'intéressants bonus!" + Environment.NewLine;
                    menu = menu + @"Visiter notre site http://m.yellowbet.com" + Environment.NewLine + Environment.NewLine;
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
                menu = "Bonjour " + user.firstName + " " + user.lastName +Environment.NewLine + Environment.NewLine;
                menu = menu + "Votre solde est " + user.AvailableForWithdraw + " GNF" + Environment.NewLine;
                if (user.AvailableForWithdraw > 0)
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


        public static IdoBetUser SearchUser(string MSISDN, ref List<LogLines> lines)
        {

            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {

                string search_user_url = Cache.ServerSettings.GetServerSettings("iDoBetSearchUserUrl", ref lines);
                lines = Add2Log(lines, " search_user_url = " + search_user_url, 100, "ivr_subscribe");

                SearchUserRequest request = new SearchUserRequest()
                {
                    token = token_id,
                    searchBy = "phone",
                    searchFor = "+" + MSISDN
                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(search_user_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    
                    try
                    {
                        if (json_response.users.ToString() != "[]")
                        {
                            if (json_response.users[0].isValid == true)
                            {
                                user.isValid = true;
                                user.firstName = json_response.users[0].firstName;
                                user.lastName = json_response.users[0].lastName;
                                user.iPin = json_response.users[0].iPin;
                                user.id = json_response.users[0].id;
                                user.availableCredit = json_response.users[0].availableCredit;
                            }
                            else
                            {
                                user.isValid = false;
                                user.firstName = json_response.users[0].firstName;
                                user.lastName = json_response.users[0].lastName;
                                user.iPin = json_response.users[0].iPin;
                                user.id = json_response.users[0].id;
                                user.availableCredit = json_response.users[0].availableCredit;
                            }
                            
                        }
                        
                        
                    }
                    catch(Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                        
                    }
                    
                    
                }

            }
            return user;
        }

        public static IdoBetUser SearchUserNew(string MSISDN, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"Phone\": \"+"+MSISDN+"\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                                string email = Convert.ToString(item.Email) ?? "";
                                user = new IdoBetUser()
                                {
                                    isValid = item.IsFullRegistrationDetails,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit,
                                    AvailableForWithdraw = item.AvailableForWithdraw,
                                    isLocked = item.IsLocked,
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

        public static IdoBetUser SearchUserByUserID(string idobet_id, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"UserID\": \"+" + idobet_id + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                                string email = Convert.ToString(item.Email) ?? "";
                                user = new IdoBetUser()
                                {
                                    isValid = item.IsFullRegistrationDetails,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit,
                                    AvailableForWithdraw = item.AvailableForWithdraw,
                                    isLocked = item.IsLocked,
                                    Email = email,
                                    msisdn = item.Phone
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

        public static IdoBetUser SearchUserNew1(string MSISDN, ref List<LogLines> lines)
        {
            IdoBetUser user = new IdoBetUser();
            user.isValid = false;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName_STG", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword_STG", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string postBody = "{\"MethodGroup\":\"Sport\", \"MethodName\": \"GetUsers\", \"Phone\": \"+" + MSISDN + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "2" });
                headers.Add(new Headers { key = "ChannelId", value = "1" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl_STG", ref lines);
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
                                user = new IdoBetUser()
                                {
                                    isValid = item.IsFullRegistrationDetails,
                                    firstName = item.FirstName,
                                    lastName = item.LastName,
                                    iPin = item.IPin,
                                    id = item.ID,
                                    availableCredit = item.AvailableCredit
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


        public static List<Tickets> CheckTickets(ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {

                string check_tickets_url = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketsUrl", ref lines) + "userToken=" + token_id + "&OrderStatus=3";
                lines = Add2Log(lines, " check_tickets_url = " + check_tickets_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(check_tickets_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    tickets = new List<Tickets>();
                    int page_number = 0;
                    int counter = 0;
                    foreach (var item in json_response.orders)
                    {
                        string msisdn = item.externalId;
                        string barcode = item.barcode;
                        double payout = item.maxPayout;
                        string creation_time = item.creationTime; //.ToString("dd/MM/yy HH:mm");
                        creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                        double amount = item.amount;
                        string win_status = item.winStatus;
                        string order_status = item.status;

                        if (counter % 3 == 0)
                        {
                            page_number = page_number + 1;
                        }

                        tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number });

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

            }
            return tickets;
        }

        public static List<Tickets> CheckTickets(string MSISDN, bool use_orderstatus, ref List<LogLines> lines)
        {
            
            List<Tickets> tickets = null;
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {

                string check_tickets_url = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketsUrl", ref lines) + "userToken=" + token_id + "&externalid=" + MSISDN + (use_orderstatus == true ? "&OrderStatus=3" : "");
                lines = Add2Log(lines, " check_tickets_url = " + check_tickets_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(check_tickets_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    tickets = new List<Tickets>();
                    int page_number = 0;
                    int counter = 0;
                    foreach (var item in json_response.orders)
                    {
                        string msisdn = item.externalId;
                        string barcode = item.barcode;
                        double payout = item.payout;
                        string creation_time = item.creationTime; //.ToString("dd/MM/yy HH:mm");
                        creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                        double amount = item.amount;
                        string win_status = item.winStatus;
                        string order_status = item.status;

                        if (counter % 3 == 0)
                        {
                            page_number = page_number + 1;
                        }

                        tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number });
                        
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

            }
            return tickets;
        }

        public static List<Tickets> CheckTicketsNew(string MSISDN, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout });

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

        public static List<Tickets> CheckTicketsNewWYear(string MSISDN, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, order_number = order_number });

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
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetGetOrderKeyUrl", ref lines);
                lines = Add2Log(lines, " idobet_endpoint = " + idobet_endpoint, 100, "ivr_subscribe");
                string postBody = "{\"barcode\":\""+ barcode + "\"}";
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

        public static List<Tickets> CheckTicketsByUserIDNew(string idobetuser_id, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout, order_number = order_number });

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

        public static List<Tickets> CheckTicketsNew(string token_id, string MSISDN, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout });

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

       

        public static List<Tickets> CheckTickets(string token_id, string MSISDN, bool use_orderstatus, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            if (!String.IsNullOrEmpty(token_id))
            {

                string check_tickets_url = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketsUrl", ref lines) + "userToken=" + token_id + "&externalid=" + MSISDN + (use_orderstatus == true ? "&OrderStatus=3" : "");
                lines = Add2Log(lines, " check_tickets_url = " + check_tickets_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(check_tickets_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    try
                    {
                        tickets = new List<Tickets>();
                        int page_number = 0;
                        int counter = 0;
                        foreach (var item in json_response.orders)
                        {
                            string msisdn = item.externalId;
                            string barcode = item.barcode;
                            double payout = item.payout;
                            string creation_time = item.creationTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatus;
                            string order_status = item.status;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number });

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

        public static string GetOrderStatus(string order_status)
        {
            string result = "";
            switch (order_status)
            {
                case "4":
                    result = " - Payé";
                    break;
                case "5":
                    result = " - Bloqué";
                    break;
                case "6":
                    result = " - Annulé";
                    break;
                case "7":
                    result = " - Expiré";
                    break;
            }
            return result;
        }

        public static string GetCheckTicketMenu(string MSISDN, int page_number, string ussd_string, ref List<LogLines> lines)
        {
            string menu = "";
            
            List<Tickets> tickets = CheckTicketsNew(MSISDN, ref lines);

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
                    status = status + GetOrderStatus(filtered_ticket.order_status);

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


        public static List<Tickets> GetUnPaidTickets(ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            string token_id = GetTokenNew(ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout });

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
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!string.IsNullOrEmpty(token_id))
            {
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number, max_payout = maxpayout });

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
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
                headers.Add(new Headers { key = "BrandId", value = "13" });
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
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatusId;
                            string order_status = item.status;

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
                                max_payout = maxpayout
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

        public static List<Tickets> CheckTicketByBarCode(string bar_codeid, ref List<LogLines> lines)
        {

            List<Tickets> tickets = null;
            //string username = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            //string password = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
            //string token_id = GetToken(username, password, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenCheckTicket(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {

                string check_tickets_url = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketsUrl", ref lines) + "userToken=" + token_id + "&barcode=" + bar_codeid;
                lines = Add2Log(lines, " check_tickets_url = " + check_tickets_url, 100, "ivr_subscribe");
                string response = CallSoap.GetURL(check_tickets_url, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    try
                    {
                        tickets = new List<Tickets>();
                        int page_number = 0;
                        int counter = 0;
                        foreach (var item in json_response.orders)
                        {
                            string msisdn = item.externalId;
                            string barcode = item.barcode;
                            double payout = item.maxPayout;
                            string creation_time = item.creationTime; //.ToString("dd/MM/yy HH:mm");
                            creation_time = Convert.ToDateTime(creation_time).AddHours(1).ToString("dd/MM HH:mm");
                            double amount = item.amount;
                            string win_status = item.winStatus;
                            string order_status = item.status;

                            if (counter % 3 == 0)
                            {
                                page_number = page_number + 1;
                            }

                            tickets.Add(new Tickets { msisdn = msisdn, barcode = barcode, payout = payout, creation_time = creation_time, amount = amount, win_status = win_status, order_status = order_status, page_number = page_number });

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

        public static string GetCheckTicketByBarcodeMenu(string msisdn, string bar_codeid, out string paid_amount, ref List<LogLines> lines)
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
                    menu = "Statut du Ticket:" + Environment.NewLine;
                    menu = menu + "Code Barre: " + filtered_ticket.barcode + Environment.NewLine;
                    menu = menu + "Date: " + filtered_ticket.creation_time + Environment.NewLine;
                    string status = "";
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
                                status = "Gagnant - Payé";
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
                                status = "l'utilisateur a été remboursé";
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
                    if (status == "Gagnant - Payé")
                    {
                        menu = menu + "Montant payé: " + filtered_ticket.payout + Environment.NewLine;
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

        public static string GetCheckTicketsMenu(string MSISDN, int page_number, ref List<LogLines> lines)
        {
            string menu = "";
            List<Tickets> tickets = CheckTicketsNew(MSISDN, ref lines);
            if (tickets != null)
            {
                List<Tickets> filtered_tickets = tickets.Where(x => x.page_number == page_number).ToList();
                menu = "Statut du Ticket" + Environment.NewLine;
                int counter = 1;
                foreach (Tickets t in filtered_tickets)
                {
                    menu = menu + counter + ") " + t.barcode + " ("+t.creation_time+")" + Environment.NewLine;
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

        public static ExecuteOrderDetails GetExecuteOrder(DYATransactions dya_trans, out string postBody, out string response_body, ref List<LogLines> lines)
        {
            ExecuteOrderDetails result = null;
            string barcode = "", order_number = "", amount = "";
            double max_p = 0;
            postBody = "";
            response_body = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
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

                ExecuteOrder request = new ExecuteOrder()
                {
                    userToken = token_id,
                    timeoutGuid = saved_games[0].time_out_guid
                };
                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();

                string request_execute_order_url = Cache.ServerSettings.GetServerSettings("iDoBetExecuteOrderUrl", ref lines);
                lines = Add2Log(lines, " Sending to url " + request_execute_order_url, 100, "ivr_subscribe");
                response_body = CommonFuncations.CallSoap.CallSoapRequest(request_execute_order_url, postBody, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    if (json_response.order.barcode != null && json_response.order.barcode != "null")
                    {
                        barcode = json_response.order.barcode;
                        order_number = json_response.order.orderNumber;
                        result = new ExecuteOrderDetails()
                        {
                            order_number = order_number,
                            barcode = barcode,
                            max_payout = max_p.ToString(),
                            amount = amount

                        };
                        List<Int64> saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid, ref lines);
                        if (saved_ids != null)
                        {
                            foreach(Int64 s in saved_ids)
                            {
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                            }
                        }
                        
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
            string barcode = "", order_number = "", amount = "", idobet_transid = "";
            double max_p = 0;
            postBody = "";
            response_body = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(dya_trans.partner_transid, ref lines);
            string token_id = Cache.iDoBet.GetiDoBetUserTokenNew(0, ref lines);
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
                    transactionExtraData = "{\"Timestamp\":\""+dya_trans.datetime+"\"}"

                };
                postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "BrandId", value = "13" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });

                string request_execute_order_url = Cache.ServerSettings.GetServerSettings("iDoBetExecuteOrderUrl", ref lines);
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
                        result = new ExecuteOrderDetails()
                        {
                            order_number = order_number,
                            barcode = barcode,
                            max_payout = max_p.ToString(),
                            amount = amount,
                            idobet_transid = idobet_transid

                        };
                        List<Int64> saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid, ref lines);
                        if (saved_ids != null)
                        {
                            foreach (Int64 s in saved_ids)
                            {
                                DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                            }
                        }
                        DataLayer.DBQueries.ExecuteQuery("insert into idobet_ids (dya_id, idobet_id) values("+dya_trans.dya_trans+","+idobet_transid+")", ref lines);
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
                string token_id = Cache.iDoBet.GetiDoBetUserTokenNew(0, ref lines);
                string terminal_security_token = Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCode", ref lines);

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
                headers.Add(new Headers { key = "BrandId", value = "13" });
                headers.Add(new Headers { key = "ChannelId", value = "9" });
                headers.Add(new Headers { key = "Language", value = "en" });
                headers.Add(new Headers { key = "Terminal", value = "" });
                headers.Add(new Headers { key = "ExtraData", value = "" });
                headers.Add(new Headers { key = "Token", value = token_id });
                string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetPlaceBetUrl", ref lines);
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

        public static bool GetRequestForOrder(USSDSession ussd_session, ref List<LogLines> lines)
        {
            bool result = false;
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                List<rows> rows = new List<iDoBet.rows>();
                List<selections> selections = new List<iDoBet.selections>();
                string[] selection_keys = new string[saved_games.Count()];
                string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
                string terminal_security_token = Cache.ServerSettings.GetServerSettings("iDoBetTerminalSecurityCode", ref lines);

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
                RequestForOrder request = new RequestForOrder()
                {
                    TerminalSecurityCode = terminal_security_token,
                    ExternalID = saved_games[0].msisdn,
                    userToken = token_id,
                    rows = rows,
                    selections = selections,
                    IsBooking = false
                };
                string postBody = JsonConvert.SerializeObject(request);
                lines = Add2Log(lines, " Json = " + postBody, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();

                string request_for_orde_url = Cache.ServerSettings.GetServerSettings("iDoBetRequestForOrderUrl", ref lines);
                lines = Add2Log(lines, " Sending to url " + request_for_orde_url, 100, "ivr_subscribe");
                string response_body = CommonFuncations.CallSoap.CallSoapRequest(request_for_orde_url, postBody, headers, 2, true, ref lines);
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
                        //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set time_out_guid = '"+ json_response.timeoutGuid + "', `status` = 1 where user_session_id = '"+ ussd_session.user_seesion_id+ "'", ref lines);
                    }
                }
                catch(Exception e)
                {
                    lines = Add2Log(lines, " Exception " + e.ToString(), 100, "ivr_subscribe");
                }
            }
            return result;
        }

        public static string GetSoccerLeagueMenu(int sport_type, string ussd_string, int ussd_id, ref List<LogLines> lines, out int out_league_id)
        {
            out_league_id = 0;
            string menu = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetEventsFromCache(sport_type, ref lines);
            switch (sport_type)
            {
                case 31:
                    menu = "Parions sur le foot:" + Environment.NewLine;
                    break;
                case 32:
                    menu = "Parions sur le basket:" + Environment.NewLine;
                    break;
                case 35:
                    menu = "Parions sur le tennis:" + Environment.NewLine;
                    break;
            }
            int counter = 2;
            List<iDoBetLeague> idobet_leagues = Cache.iDoBet.GetiDoBetLeagues(ref lines);


            if (sports_events != null)
            {
                menu = menu + "1) Tous les evenements (" + sports_events.Count() + ")" + Environment.NewLine;
                var query = sports_events.GroupBy(x => x.leagueId, x => x.league_name, (league_id, league_name) => new { league_id = league_id, Count = league_id.Count(), league_name = league_name}).ToList();
                
                if (idobet_leagues != null)
                {
                    idobet_leagues = idobet_leagues.Where(x => x.ussd_id == ussd_id).ToList();
                    foreach(iDoBetLeague l in idobet_leagues)
                    {
                        var filtered_query = query.Where(x => x.league_id == l.league_id.ToString()).ToList();
                        int games_count = sports_events.Where(x => x.leagueId == l.league_id.ToString()).Count();
                        if (filtered_query != null && filtered_query.Count() > 0)
                        {
                            menu = menu + counter + ") " + l.league_name + " (" + games_count + ")" + Environment.NewLine;
                            if(ussd_string == counter.ToString())
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
                menu = menu + "M) Menu Principal ";





            }
            


            return menu;
        }

        public static List<SportEvents> GetEvents(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string get_events_url = Cache.ServerSettings.GetServerSettings("iDoBetGetEventsUrl", ref lines) + "sportIds=" + sport_type_id + "&userToken=" + token_id + "&LeagueIds=" + selected_league_id;
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

            string token_id = Cache.iDoBet.GetiDoBetUserToken(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string get_events_url = Cache.ServerSettings.GetServerSettings("iDoBetGetEventsUrl", ref lines) + "sportIds=" + sport_type_id + "&userToken=" + token_id;
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
                        catch(Exception ex)
                        {

                        }

                        

                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }

            }
            return sports_events;
        }

        public static List<SportEvents> GetEventsNew(int sport_type_id, ref List<LogLines> lines)
        {
            List<EventOdds> event_odds = null;
            List<SportEvents> sports_events = null;

            string token_id = Cache.iDoBet.GetiDoBetUserTokenNew(0, ref lines);
            if (!String.IsNullOrEmpty(token_id))
            {
                string get_events_url = Cache.ServerSettings.GetServerSettings("iDoBetGetEventsUrl_STG", ref lines) + "sportIds=" + sport_type_id + "&userToken=" + token_id;
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
                        catch (Exception ex)
                        {

                        }



                    }
                    lines = Add2Log(lines, " Finished building event class ", 100, "ivr_subscribe");
                }

            }
            return sports_events;
        }

        public static string GetConfirmMenu(int sport_type_id, Int64 game_id, string ussd_string, int odd_page, out double selected_odd, out int selected_bet_type_id, out string selected_odd_name, out string selected_odd_line,  ref List<LogLines> lines)
        {
            string menu = "";
            selected_odd = 0;
            selected_bet_type_id = 0;
            selected_odd_name = "";
            selected_odd_line = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetEventsFromCache(sport_type_id, ref lines);
            SportEvents game = new SportEvents();
            game = sports_events.Find(x => x.game_id == game_id);
            if (game != null)
            {
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
                            menu = menu + " "+ odd_name + "-" + filtred_odd.ck_name + " (" + filtred_odd.ck_price + ")" + Environment.NewLine;
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

                    menu = menu + Environment.NewLine + "1) Ajouter un Jeu" + Environment.NewLine;
                    menu = menu + "2) Confirmer &amp; Payer" + Environment.NewLine;
                    menu = menu + Environment.NewLine + "*) Retour";
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

        public static string GetGameOddsMenu(int sport_type_id, int page_number, string ussd_string, Int64 game_id, int odd_page, out Int64 out_game_id, int selected_league_id, ref List<LogLines> lines)
        {
            string menu = "";
            out_game_id = 0;
            SportEvents game = null;
            List<SportEvents> sports_events = null;
            if (selected_league_id == 0)
            {
                sports_events = Cache.iDoBet.GetEventsFromCache(sport_type_id, ref lines);
            }
            else
            {
                sports_events = Cache.iDoBet.GetEventsWithLeagueIDFromCache(selected_league_id, sport_type_id, ref lines);
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
                        foreach(EventOdds eo in filtred_odd)
                        {
                            menu = menu + counter + ") "+ odd_name + "-" + eo.ck_name + " ("+eo.ck_price+")" + Environment.NewLine;
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
                            menu = menu + counter + ") " +(eo.ck_name.ToLower() == "under" ? "U ": "O ") + eo.line + " (" + eo.ck_price + ")" + Environment.NewLine;
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
                            menu = menu + counter + ". DC-" + eo.ck_name + " (" + eo.ck_price + ")" + Environment.NewLine;
                            counter = counter + 1;
                        }
                        menu = menu + Environment.NewLine + "*) Retour";
                        break;
                }
                menu = menu + Environment.NewLine + "M) ";// Menu Principal ";
            }

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
                double max_payout = Math.Round(total_ratio * Convert.ToDouble(amount),2);
                menu = menu + "Gain Maximum: " + max_payout + Environment.NewLine + Environment.NewLine;
                menu = menu + "1) Confirmer" + Environment.NewLine;
                menu = menu + "2) Annuler &amp; Recommencer" + Environment.NewLine + Environment.NewLine;

                menu = menu + "*) Retour";
            }
            return menu;
        }

        public static string GetCloseBetFailed(ref List<LogLines> lines)
        {
            string menu = "";
            menu = "Votre pari a échoué." + Environment.NewLine;
            menu = menu + "Veuillez réessayer"; //Your bet has failed.
            return menu;
        }
        public static string GetCloseBet(USSDSession ussd_session, string amount, ref List<LogLines> lines)
        {
            string menu = "";
            List<SavedGames> saved_games = DataLayer.DBQueries.GetiDoBetSavedGames(ussd_session.user_seesion_id, ref lines);
            if (saved_games != null)
            {
                //menu = "Vous avez presque fini." + Environment.NewLine;
                //menu = menu + "Numéro des Jeux: " + saved_games.Count() + Environment.NewLine;
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

                menu = menu + "Pour compléter votre pari, confirmez le paiement ou composez *400#, puis sélectionnez (8)." + Environment.NewLine;
                menu = menu + "Confirmez T&amp;C et +18 http://yellowbet.com/terms" + Environment.NewLine;
            }
            return menu;
        }

        public static string GetWrongPriceBetMeny(ref List<LogLines> lines)
        {
            string menu = "Veuillez saisir un montant compris entre 200 GNF et 1000000 GNF" + Environment.NewLine + Environment.NewLine;
            menu = menu + "*) Retour";
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

        public static string GetEventsMenu(string ussd_string, int sport_type_id, int page_number, ref List<LogLines> lines ,out int selected_league_id, int real_selected_league_id, USSDMenu ussd_menu)
        {
            selected_league_id = 0;
            
            string menu = "";
            List<SportEvents> sports_events = null;
            if (ussd_string == "1")
            {
                sports_events = Cache.iDoBet.GetEventsFromCache(sport_type_id, ref lines);
            }
            else
            {
                string a = GetSoccerLeagueMenu(sport_type_id, ussd_string, ussd_menu.ussd_id, ref lines, out selected_league_id);
                sports_events = Cache.iDoBet.GetEventsWithLeagueIDFromCache((selected_league_id == 0 ? real_selected_league_id : selected_league_id), sport_type_id, ref lines);
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

        public static string GetEventsMenu(int sport_type_id, int page_number, ref List<LogLines> lines)
        {
            string menu = "";
            List<SportEvents> sports_events = Cache.iDoBet.GetEventsFromCache(sport_type_id, ref lines);
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
                        menu = menu + "*) Retour";
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




        public static string GetToken(ref List<LogLines> lines)
        {
            string token_id = null;
            string iDoBetUserName = Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName", ref lines);
            string iDoBetPassword = Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword", ref lines);
            string iDoBetLoginUrl = Cache.ServerSettings.GetServerSettings("iDoBetLoginUrl", ref lines);
            string json = "{\"username\": \""+ iDoBetUserName + "\",\"password\": \""+ iDoBetPassword + "\"}";
            lines = Add2Log(lines, "iDoBet Login Json = " + json, 100, lines[0].ControlerName);
            lines = Add2Log(lines, "Sending to " + iDoBetLoginUrl, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string response = CallSoap.CallSoapRequest(iDoBetLoginUrl, json, headers, 2, ref lines);
            lines = Add2Log(lines, "iDoBet Login response = " + response, 100, lines[0].ControlerName);
            if (response != null)
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    token_id = json_response.token;
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "iDoBet GetToken exception " + ex.ToString(), 100, lines[0].ControlerName);
                    token_id = null;
                }
            }

            return token_id;

        }

        public static string GetTokenNew(ref List<LogLines> lines)
        {
            string token_id = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
            string iDoBetUserName = Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName", ref lines);
            string iDoBetPassword = Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
            string iDoBetUserName = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName", ref lines);
            string iDoBetPassword = Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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

        public static string GetToken(string iDoBetUserName, string iDoBetPassword, ref List<LogLines> lines)
        {
            string token_id = null;
            string iDoBetLoginUrl = Cache.ServerSettings.GetServerSettings("iDoBetLoginUrl", ref lines);
            string json = "{\"username\": \"" + iDoBetUserName + "\",\"password\": \"" + iDoBetPassword + "\"}";
            lines = Add2Log(lines, "iDoBet Login Json = " + json, 100, lines[0].ControlerName);
            lines = Add2Log(lines, "Sending to " + iDoBetLoginUrl, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string response = CallSoap.CallSoapRequest(iDoBetLoginUrl, json, headers, 2, ref lines);
            lines = Add2Log(lines, "iDoBet Login response = " + response, 100, lines[0].ControlerName);
            if (response != null)
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    token_id = json_response.token;
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "iDoBet GetToken exception " + ex.ToString(), 100, lines[0].ControlerName);
                    token_id = null;
                }
            }

            return token_id;

        }

        

        public static string GetTokenNew(string iDoBetUserName, string iDoBetPassword, ref List<LogLines> lines)
        {
            string token_id = "";
            string idobet_endpoint = Cache.ServerSettings.GetServerSettings("iDoBetEndPointUrl", ref lines);
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
            headers.Add(new Headers { key = "BrandId", value = "13" });
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