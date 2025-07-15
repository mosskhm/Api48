using Api.Cache;
using Api.CommonFuncations;
using Api.HttpItems;
using Api.Logger;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Cache.SMS;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.DataLayer
{
    public static class DBQueries
    {
        public class DLValidateBill
        {
            public Int64 sub_id { get; set; }
            public int ret_code { get; set; }
            public string description { get; set; }
            public bool is_staging { get; set; }
            public int operator_id { get; set; }
        }

        public class CLSendUSSD
        {
            public Int64 cu_id { get; set; }
            public int ret_code { get; set; }
            public string description { get; set; }
            public bool is_staging { get; set; }
            public Int64 msisdn { get; set; }
            public int campaign_id { get; set; }
            public int service_id { get; set; }
            public string msg { get; set; }
            public int operator_id { get; set; }

        }

        public class CPToolGrossRevenu
        {
            public string date { get; set; }
            public string service_name { get; set; }
            public string operator_name { get; set; }
            public string country { get; set; }
            public int new_sub { get; set; }
            public int gross_revenu { get; set; }

        }
        public class CPToolGrossRevForLastMonths
        {
            public string year { get; set; }
            public string month { get; set; }
            public string volume { get; set; }
        }

        public class CPToolUserDetails
        {
            public int service_id { get; set; }
            public string service_name { get; set; }
            public string user_name { get; set; }
            public string last_login { get; set; }
        }

        public class Quotes
        {
            public int quote_id { get; set; }
            public string quote { get; set; }
        }

        public class UserQuotes
        {
            public Int64 subscriber_id { get; set; }
            public Int64 MSISDN { get; set; }
            public int quote_id { get; set; }
            public string quote { get; set; }
        }

        public class Question
        {
            public int question_id { get; set; }
            public string question { get; set; }
            public int test_id { get; set; }
        }

        public class UserQuestion
        {
            public Int64 subscriber_id { get; set; }
            public Int64 MSISDN { get; set; }
            public int question_id { get; set; }
            public string question { get; set; }
            public int test_id { get; set; }
            public bool is_complete { get; set; }
        }

        public class CBPerDate
        {
            public int service_id { get; set; }
            public string date { get; set; }
            public int volume { get; set; }
        }

        public class LNWinners
        {
            public int winner_id { get; set; }
            public Int64 MSISDN { get; set; }
            public string winning_date { get; set; }
            public int amount { get; set; }
            public string payed_date { get; set; }
            public bool was_payed { get; set; }
            public string last_dya_error { get; set; }
            public string lasy_dya_attempt { get; set; }
            public bool sms_sent { get; set; }
            public string last_sms_sent { get; set; }
            public string last_sms_error { get; set; }
        }

        public class LNWinnersAPI
        {
            public string winning_date { get; set; }
            public string winning_msisdn { get; set; }
            public List<string> winners { get; set; }
            public string prize { get; set; }
        }



        public class ServiceConfiguration
        {
            public int service_id { get; set; }
            public string service_password { get; set; }
            public string xml_url { get; set; }
            public string lp_url { get; set; }
            public string token { get; set; }

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

        public class CheckLogin
        {
            public int ResultCode { get; set; }
            public string Description { get; set; }
        }

        public class UserHistory
        {
            public string subscriber_id { get; set; }
            public string service_name { get; set; }
            public string subscription_date { get; set; }
            public string deactivation_date { get; set; }
            public string state_id { get; set; }
        }

        public class BillingHistory
        {
            public string service_name { get; set; }
            public string billing_date { get; set; }
            public string price { get; set; }
            public string curency { get; set; }
        }

        public class SMSContentStatus
        {
            public int service_id { get; set; }
            public string service_name { get; set; }
            public int content { get; set; }
            public int active_subs { get; set; }
        }

        public class SMSContent
        {
            public int sms_content_id { get; set; }
            public int service_id { get; set; }
            public string service_name { get; set; }
            public string content_text { get; set; }
        }

        public class NineMobileBillingDetails
        {
            public Int64 subscriber_id { get; set; }
            public Int64 msisdn { get; set; }
            public int service_id { get; set; }
            public double price { get; set; }
            public int price_id { get; set; }
            public string subscription_date { get; set; }
            public int is_onetime { get; set; }
        }

        public class CheckNineMobileUser
        {
            public Int64 subscriber_id { get; set; }
            public int service_id { get; set; }
            public string keyword { get; set; }
            public string service_name { get; set; }
        }

        public class IVRTransactionDetails
        {
            public Int64 transaction_id { get; set; }
            public Int64 msisdn { get; set; }
            public int campaign_id { get; set; }
            public string campaign_name { get; set; }
            public int dtmf { get; set; }
            public int service_id { get; set; }
            public Int64 subscriber_id { get; set; }
            public Int64 content_id { get; set; }
            public int campaign_type { get; set; }
            public string sms_text { get; set; }
            public string long_url { get; set; }

        }

        public class iDoBetLeague
        {
            public int league_id { get; set; }
            public string league_name { get; set; }
            public int ussd_id { get; set; }
        }

        public class BtoBetLeague
        {
            public int league_id { get; set; }
            public string league_name { get; set; }
            public int ussd_id { get; set; }
        }

        public class iDoDraftTicket
        {
            public int saved_game_id { get; set; }
            public string msisdn { get; set; }
            public string date_time { get; set; }
            public string amount { get; set; }
            public string time_guid_id { get; set; }
            public Int64 dya_id { get; set; }
            public int service_id { get; set; }
            public string result { get; set; }
            public int number_of_games { get; set; }
        }

        public class iDoBetAgents
        {
            public int agent_id { get; set; }
            public string agent_username { get; set; }
            public string agent_password { get; set; }
            public string msisdn { get; set; }
            public int branch_id { get; set; }
            public string agent_name { get; set; }
            public int service_id { get; set; }
            public string msisdn_commision { get; set; }
        }

        public class GetPosTrans
        {
            public string agent_username { get; set; }
            public string subagent_username { get; set; }
            public string amount { get; set; }
        }

        public class CashierInfo
        {
            public int cashier_id { get; set; }
            public Int64 msisdn { get; set; }
            public string full_name { get; set; }
            public int commision_plan_id { get; set; }
            public int start_range { get; set; }
            public int end_range { get; set; }
            public int commision { get; set; }
        }

        public class OrangeBillingRequest
        {
            public Int64 msisdn { get; set; }
            public int amount { get; set; }
            public string session_id { get; set; }
            public int d_type { get; set; }
        }

        public class transactions
        {
            public Int64 id { get; set; }
            public Int64 msisdn { get; set; }
            public int amount { get; set; }
            public string session_id { get; set; }
            public int status { get; set; }
            public int d_type { get; set; }
            public string billing_id { get; set; }
            public int service_id { get; set; }

        }

        public class CampaignTracking
        {
            public int campaign_id { get; set; }
            public string seo_name { get; set; }
            public bool use_mtn_enrich { get; set; }
            public int subscribe_service_id { get; set; }
            public string logo { get; set; }
            public string ok_text_after_subscription_wdoi { get; set; }
            public string ok_text_after_subscription_wo_doi { get; set; }
            public string ko_text_after_subscription { get; set; }
            public string ko_text_after_enrichment { get; set; }
            public string user_is_already_subscribed { get; set; }
            public int status { get; set; }
            public bool is_doi { get; set; }
            public string view_name { get; set; }
            public string pixel { get; set; }
            public string url_behind { get; set; }
            public int partner_id { get; set; }
            public int immediate_subscribe { get; set; }
        }

        public class WinningUsersFromDB
        {
            public Int64 id { get; set; }
            public string last_5_digit { get; set; }
            public Int64 subscriber_id { get; set; }
            public Int64 winning_msisdn { get; set; }
            public int prize { get; set; }
            public int number_of_digits { get; set; }
            public string prize_name { get; set; }
            public string general_msg { get; set; }
            public string winner_msg { get; set; }
            public string not_paid_winner_msg { get; set; }
            public string curency_symbol { get; set; }
            public string number_of_winners { get; set; }
        }

        public static WinningUsersFromDB GetWinnersFromDB(string service_ids, string single_service_id, ref List<LogLines> lines)
        {
            string general_msg = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT t.general_msg from twn_settings t WHERE t.service_id = " + single_service_id, ref lines);
            WinningUsersFromDB result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "SELECT right(tw.selected_msisdn,5), tw.winning_date, COUNT(*) FROM twn_winners tw, subscribers s WHERE s.subscriber_id = tw.subscriber_id AND s.service_id IN ("+service_ids+") GROUP BY right(tw.selected_msisdn,5), tw.winning_date ORDER BY tw.winning_date DESC";

                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new WinningUsersFromDB();
                    while (reader.Read())
                    {
                        result = new WinningUsersFromDB()
                        { last_5_digit = Convert.ToString(reader.GetValue(0)), number_of_winners = Convert.ToString(reader.GetValue(2)), general_msg = general_msg };
                        break;
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static List<CampaignTracking> GetCampaigns(ref List<LogLines> lines)
        {
            List<CampaignTracking> result = null;
            CampaignTracking res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select * from tracking.tracking_campaign sc where sc.status = 1";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<CampaignTracking>();
                    while (reader.Read())
                    {
                        res = new CampaignTracking()
                        {
                            campaign_id = Convert.ToInt32(reader.GetValue(0)),
                            seo_name = Convert.ToString(reader.GetValue(1)),
                            use_mtn_enrich = Convert.ToBoolean(reader.GetValue(2)),
                            subscribe_service_id = Convert.ToInt32(reader.GetValue(3)),
                            is_doi = Convert.ToBoolean(reader.GetValue(4)),
                            logo = Convert.ToString(reader.GetValue(5)),
                            ok_text_after_subscription_wdoi = Convert.ToString(reader.GetValue(6)),
                            ok_text_after_subscription_wo_doi = Convert.ToString(reader.GetValue(7)),
                            ko_text_after_subscription = Convert.ToString(reader.GetValue(8)),
                            ko_text_after_enrichment = Convert.ToString(reader.GetValue(9)),
                            user_is_already_subscribed = Convert.ToString(reader.GetValue(10)),
                            view_name = Convert.ToString(reader.GetValue(13)),
                            partner_id = Convert.ToInt32(reader.GetValue(14)),
                            immediate_subscribe = Convert.ToInt32(reader.GetValue(15)),
                            pixel = Convert.ToString(reader.GetValue(16)),
                            url_behind = Convert.ToString(reader.GetValue(17))
                        };
                        result.Add(res);

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static transactions GetPendingTransactions(string transaction_id, ref List<LogLines> lines)
        {
            transactions result = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_161", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select * from orange_billing_requests s WHERE id = " + transaction_id;



                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new transactions();
                    while (reader.Read())
                    {
                        result = new transactions { id = Convert.ToInt64(reader.GetValue(0)), msisdn = Convert.ToInt64(reader.GetValue(1)), amount = Convert.ToInt32(reader.GetValue(3)), session_id = Convert.ToString(reader.GetValue(4)), status = Convert.ToInt32(reader.GetValue(5)), d_type = Convert.ToInt32(reader.GetValue(7)), billing_id = Convert.ToString(reader.GetValue(8)), service_id = Convert.ToInt32(reader.GetValue(9)) };
                    }
                    

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static OrangeBillingRequest UpdateGetOrangeBillingRequest(Int64 refrence_id, int status, string billing_id, ref List<LogLines> lines)
        {
            OrangeBillingRequest result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_161", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                MySql = "select msisdn, amount, session_id, d_type from orange_billing_requests where id = " + refrence_id;
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        result = new OrangeBillingRequest()
                        {
                            msisdn = Convert.ToInt64(reader.GetValue(0)),
                            amount = Convert.ToInt32(reader.GetValue(1)),
                            session_id = Convert.ToString(reader.GetValue(2)),
                            d_type = Convert.ToInt32(reader.GetValue(3))
                        };
                        MySql = "update orange_billing_requests set status = " + status + ", recieve_date_time = now(), is_complete = 0, billing_id = '"+ billing_id + "' where id = " + refrence_id;
                        lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                        DBQueries.ExecuteQuery(MySql, "DBConnectionString_161", ref lines);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<CashierInfo> GetCashierInfo(ref List<LogLines> lines)
        {
            List<CashierInfo> result = null;
            CashierInfo res1 = new CashierInfo();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT c.cashier_id, c.msisdn, c.full_name, c.commision_plan_id, cm.start_range, cm.end_range, cm.commision FROM cashiers c, cashier_commisions cm WHERE c.commision_plan_id = cm.commision_plan_id";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<CashierInfo>();
                    while (reader.Read())
                    {
                        res1 = new CashierInfo()
                        {
                            cashier_id = Convert.ToInt32(reader.GetValue(0)),
                            msisdn = Convert.ToInt64(reader.GetValue(1)),
                            full_name = Convert.ToString(reader.GetValue(2)),
                            commision_plan_id = Convert.ToInt32(reader.GetValue(3)),
                            start_range = Convert.ToInt32(reader.GetValue(4)),
                            end_range = Convert.ToInt32(reader.GetValue(5)),
                            commision = Convert.ToInt32(reader.GetValue(6))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class SMSServiceInfo
        {
            public int service_id { get; set; }
            public string welcome_sms { get; set; }
            public string rebilling_sms { get; set; }
            public string deactivation_sms { get; set; }
            public string long_url { get; set; }
            public string base_url { get; set; }
            public int valid_date_exp { get; set; }
        }

        public static List<SMSServiceInfo> GetSMSServiceInfo(ref List<LogLines> lines)
        {
            List<SMSServiceInfo> result = new List<SMSServiceInfo>();
            SMSServiceInfo res1 = new SMSServiceInfo();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT * from service_sms";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<SMSServiceInfo>();
                    while (reader.Read())
                    {
                        res1 = new SMSServiceInfo()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            welcome_sms = Convert.ToString(reader.GetValue(1)),
                            rebilling_sms = Convert.ToString(reader.GetValue(2)),
                            deactivation_sms = Convert.ToString(reader.GetValue(3)),
                            long_url = Convert.ToString(reader.GetValue(4)),
                            base_url = Convert.ToString(reader.GetValue(5)),
                            valid_date_exp = Convert.ToInt32(reader.GetValue(6))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<LNWinnersAPI> GetLNWinnersApi(string service_id, ref List<LogLines> lines)
        {
            List<LNWinnersAPI> result = new List<LNWinnersAPI>();
            LNWinnersAPI res1 = new LNWinnersAPI();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT da, selected_msisdn, GROUP_CONCAT(msisdn), prize, service_id FROM (SELECT DATE(t.winning_date) da, t.selected_msisdn, if (s.msisdn IS NULL,0,s.msisdn) msisdn, tp.prize, tp.service_id FROM twn_prizes tp, twn_winners t LEFT JOIN subscribers s ON(s.subscriber_id = t.subscriber_id) WHERE tp.prize_id = t.prize_id) t where service_id = "+service_id+" GROUP BY da, selected_msisdn, prize";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<LNWinnersAPI>();
                    while (reader.Read())
                    {
                        List<string> winners = new List<string>();
                        string my_winner = Convert.ToString(reader.GetValue(2));
                        if (my_winner.Contains(","))
                        {
                            string[] winner_arr = my_winner.Split(',');
                            foreach(string w in winner_arr)
                            {
                                winners.Add(w);
                            }
                        }
                        else
                        {
                            winners.Add(my_winner);
                        }

                        res1 = new LNWinnersAPI()
                        {
                            winning_date = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                            winning_msisdn = Convert.ToString(reader.GetValue(1)),
                            winners = winners,
                            prize = Convert.ToString(reader.GetValue(3))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static GetPosTrans GetPosTransaction(Int64 pos_trans, ref List<LogLines> lines)
        {
            GetPosTrans result = null;
            
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT p.agent_name, p.sub_agent_name, p.amount FROM pos_requests p where p.id = " + pos_trans;


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    
                    while (reader.Read())
                    {
                        result = new GetPosTrans()
                        {
                            agent_username = Convert.ToString(reader.GetValue(0)),
                            subagent_username = Convert.ToString(reader.GetValue(1)),
                            amount = Convert.ToString(reader.GetValue(2))
                        };
                        
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<iDoBetAgents> GetiDoBetAgents(ref List<LogLines> lines)
        {
            List<iDoBetAgents> result = null;
            iDoBetAgents res1 = new iDoBetAgents();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT * FROM pos_agents";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<iDoBetAgents>();
                    while (reader.Read())
                    {
                        res1 = new iDoBetAgents()
                        {
                            agent_id = Convert.ToInt32(reader.GetValue(0)),
                            agent_username = Convert.ToString(reader.GetValue(1)),
                            agent_password = Convert.ToString(reader.GetValue(2)),
                            msisdn = Convert.ToString(reader.GetValue(3)),
                            branch_id = Convert.ToInt32(reader.GetValue(4)),
                            agent_name = Convert.ToString(reader.GetValue(5)),
                            service_id = Convert.ToInt32(reader.GetValue(6)),
                            msisdn_commision = Convert.ToString(reader.GetValue(7))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class FootSoldiers
        {
            public int id { get; set; }
            public string footsoldier_name { get; set; }
            public Int64 footsoldier_msisdn { get; set; }
        }

        public static List<FootSoldiers> GetFootSoldiers(ref List<LogLines> lines)
        {
            List<FootSoldiers> result = null;
            FootSoldiers res1 = new FootSoldiers();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT * FROM foot_soldiers f WHERE f.`status` = 1";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<FootSoldiers>();
                    while (reader.Read())
                    {
                        res1 = new FootSoldiers()
                        {
                            id = Convert.ToInt32(reader.GetValue(0)),
                            footsoldier_name = Convert.ToString(reader.GetValue(1)),
                            footsoldier_msisdn = Convert.ToInt64(reader.GetValue(2)),
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public class Banks
        {
            public int bank_id { get; set; }
            public string bank_code { get; set; }
            public string bank_name { get; set; }
            public int country_id { get; set; }


        }
        public static List<Banks> GetBanks(ref List<LogLines> lines)
        {
            List<Banks> result = null;
            Banks res1 = new Banks();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT * FROM flutterwave_banks";


                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<Banks>();
                    while (reader.Read())
                    {
                        res1 = new Banks()
                        {
                            bank_id = Convert.ToInt32(reader.GetValue(0)),
                            bank_code = Convert.ToString(reader.GetValue(1)),
                            bank_name = Convert.ToString(reader.GetValue(2)),
                            country_id = Convert.ToInt32(reader.GetValue(3))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }



        public static List<iDoDraftTicket> GetDraftTickets(ref List<LogLines> lines)
        {
            List<iDoDraftTicket> result = null;
            iDoDraftTicket res1 = new iDoDraftTicket();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select u.id, u.msisdn, u.date_time, u.amount, u.time_out_guid, d.dya_id, d.service_id, d.result, d.result_desc, count(u.time_out_guid) from ussd_saved_games u, dya_transactions d where u.time_out_guid <> '0' and u.`status` = 1 and d.result <> '0' and u.id > 207 and d.transaction_id = u.user_session_id and d.dya_method = 2 and d.service_id in (716) group by u.time_out_guid order by u.id desc";

                
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<iDoDraftTicket>();
                    while (reader.Read())
                    {
                        res1 = new iDoDraftTicket()
                        {
                            saved_game_id = Convert.ToInt32(reader.GetValue(0)),
                            msisdn = Convert.ToString(reader.GetValue(1)),
                            date_time = Convert.ToString(reader.GetValue(2)),
                            amount = Convert.ToString(reader.GetValue(3)),
                            time_guid_id = Convert.ToString(reader.GetValue(4)),
                            dya_id = Convert.ToInt64(reader.GetValue(5)),
                            service_id = Convert.ToInt32(reader.GetValue(6)),
                            result = Convert.ToString(reader.GetValue(7)) + " - " + Convert.ToString(reader.GetValue(8)),
                            number_of_games = Convert.ToInt32(reader.GetValue(9))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<iDoBetLeague> GetiDobetMainLegues(ref List<LogLines> lines)
        {
            List<iDoBetLeague> result = null;
            iDoBetLeague res1 = new iDoBetLeague();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select ul.league_id, ul.league_name, ul.ussd_id from ussd_leagues ul order by ul.order_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<iDoBetLeague>();
                    while (reader.Read())
                    {
                        res1 = new iDoBetLeague()
                        {
                            league_id = Convert.ToInt32(reader.GetValue(0)),
                            league_name = Convert.ToString(reader.GetValue(1)),
                            ussd_id = Convert.ToInt32(reader.GetValue(2))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<BtoBetLeague> GetBtoBetMainLegues(ref List<LogLines> lines)
        {
            List<BtoBetLeague> result = null;
            BtoBetLeague res1 = new BtoBetLeague();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select ul.league_id, ul.league_name, ul.ussd_id from ussd_leagues ul order by ul.order_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<BtoBetLeague>();
                    while (reader.Read())
                    {
                        res1 = new BtoBetLeague()
                        {
                            league_id = Convert.ToInt32(reader.GetValue(0)),
                            league_name = Convert.ToString(reader.GetValue(1)),
                            ussd_id = Convert.ToInt32(reader.GetValue(2))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static Int64[] GetMSISDNByReqID(string req_id, ref List<LogLines> lines)
        {
            Int64 msisdn = 0;
            Int64 service_id = 0;
            Int64[] result = new long[] { msisdn, service_id};
            

            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "select r.msisdn, r.service_id from requests r where r.req_id = " + req_id;
                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    msisdn = Convert.ToInt64(reader.GetValue(0));
                    service_id = Convert.ToInt64(reader.GetValue(1));
                }
                result = new long[] { msisdn, service_id };

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static string[] GetInfoByReqID(string req_id, ref List<LogLines> lines)
        {
            string msisdn = "0";
            string service_id = "0";
            string transaction_id = "0";
            string[] result = new string[] { msisdn, service_id, transaction_id };


            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "select r.msisdn, r.service_id, transaction_id from requests r where r.req_id = " + req_id;
                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    msisdn = Convert.ToString(reader.GetValue(0));
                    service_id = Convert.ToString(reader.GetValue(1));
                    transaction_id = Convert.ToString(reader.GetValue(2));
                }
                result = new string[] { msisdn, service_id, transaction_id };

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static Int64 InsertWapRequest(string msisdn, int service_id, int subscription_method_id, int pin_code, ref List<LogLines> lines)
        {
            Int64 result = 0;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            try
            {
                lines = Add2Log(lines, " call sp_InsertRequest(" + msisdn + "," + service_id + "," + subscription_method_id + "," + pin_code + ");", 100, lines[0].ControlerName);
                connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "sp_InsertRequest";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@_msisdn", msisdn);
                command.Parameters.AddWithValue("@_service_id", service_id);
                command.Parameters.AddWithValue("@_subscription_method_id", subscription_method_id);
                command.Parameters.AddWithValue("@_pin_code", pin_code);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt64(reader.GetValue(0));
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static string CheckWithdrawTransaction(DYATransferMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            string result = "";

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select d.dya_id, d.msisdn, d.service_id from dya_transactions_success d where d.transaction_id = '"+RequestBody.TransactionID+"' and d.service_id = " + RequestBody.ServiceID;

                
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 60;            // in seconds
                reader = command.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    result = Convert.ToString(reader.GetValue(0));
                    lines = Add2Log(lines, "DUPLICATE CHECK: found existing transaction for " + RequestBody.TransactionID +", ServiceID=" + RequestBody.ServiceID+", dya_id="+result+", msisdn="+Convert.ToString(reader.GetValue(1)), 100, lines[0].ControlerName);
                } // else transaction doesn't exist in the dya_transactions_success table

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = "error";
            }
            finally
            {
                if (reader != null)     reader.Close();
                if (connection != null) connection.Close();
            }

            return result;
        }

        public static string CheckAirTimeRefundTransaction(RefundAirTimeRequest RequestBody, ref List<LogLines> lines)
        {
            string result = "";

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "SELECT d.dya_id, d.result, d.transaction_id FROM dya_transactions d WHERE d.service_id = " + RequestBody.ServiceID + " AND d.msisdn = " + RequestBody.MSISDN + " AND d.dya_method in (3,4,5)";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        if (Convert.ToString(reader.GetValue(1)) == "01" && Convert.ToString(reader.GetValue(2)) == RequestBody.TransactionID)
                        {
                            result = Convert.ToString(reader.GetValue(0));
                            break;
                        }

                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = "error";
            }
            finally
            {
                if (reader != null)     reader.Close();
                if (connection != null) connection.Close();
            }

            return result;
        }


        public static List<IVRTransactionDetails> GetIVRTransactionDetails(string transaction_id, ref List<LogLines> lines)
        {
            List<IVRTransactionDetails> result = null;

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select op.call_id, op.msisdn, op.obd_c_id, oc.campaign_name, ops.dtmf, ops.service_id, oc.campaign_type, oc.sms_text, oc.long_url from ivr.obd_promo_users op, ivr.obd_campaigns oc, ivr.obd_audio_promo oap, ivr.obd_promo_services ops where op.call_id = " + transaction_id + " and ops.obd_ap_id = oap.obd_ap_id and oap.obd_ap_id = oc.obd_ap_id and oc.obd_c_id = op.obd_c_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    IVRTransactionDetails res = new IVRTransactionDetails();
                    result = new List<IVRTransactionDetails>();
                    while (reader.Read())
                    {
                        res = new IVRTransactionDetails()
                        {
                            transaction_id = Convert.ToInt64(reader.GetValue(0)),
                            msisdn = Convert.ToInt64(reader.GetValue(1)),
                            campaign_id = Convert.ToInt32(reader.GetValue(2)),
                            campaign_name = Convert.ToString(reader.GetValue(3)),
                            dtmf = Convert.ToInt32(reader.GetValue(4)),
                            service_id = Convert.ToInt32(reader.GetValue(5)),
                            campaign_type = Convert.ToInt32(reader.GetValue(6)),
                            sms_text = Convert.ToString(reader.GetValue(7)),
                            long_url = Convert.ToString(reader.GetValue(8))
                        };
                        result.Add(res);
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static List<IVRTransactionDetails> GetIVRContentTransactionDetails(string transaction_id, ref List<LogLines> lines)
        {
            List<IVRTransactionDetails> result = null;

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select op.call_id, op.msisdn, 0, '', if(op.dtmf is null,0,op.dtmf), s.service_id, op.subscriber_id, op.content_id from ivr.obd_content_users op, yellowdot.subscribers s where op.call_id = "+transaction_id+" and s.subscriber_id = op.subscriber_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    IVRTransactionDetails res = new IVRTransactionDetails();
                    result = new List<IVRTransactionDetails>();
                    while (reader.Read())
                    {
                        res = new IVRTransactionDetails()
                        {
                            transaction_id = Convert.ToInt64(reader.GetValue(0)),
                            msisdn = Convert.ToInt64(reader.GetValue(1)),
                            campaign_id = Convert.ToInt32(reader.GetValue(2)),
                            campaign_name = Convert.ToString(reader.GetValue(3)),
                            dtmf = Convert.ToInt32(reader.GetValue(4)),
                            service_id = Convert.ToInt32(reader.GetValue(5)),
                            subscriber_id = Convert.ToInt64(reader.GetValue(6)),
                            content_id = Convert.ToInt64(reader.GetValue(7))
                        };
                        result.Add(res);
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static List<CheckNineMobileUser> CheckNineMobileUserInfo(string msisdn, string short_code, string service_id, string mobile_operator, ref List<LogLines> lines)
        {
            List<CheckNineMobileUser> result = null;

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s.subscriber_id, s.service_id, s1.service_name, if(if(ns.service_name = '',s1.service_name,ns.service_name) is null,s1.service_name,ns.service_name) from subscribers s, services s1, service_configuration sc left join nine_mobile_service_name ns on(ns.service_id = sc.service_id) where sc.service_id = s.service_id and s1.service_id = s.service_id and sc.operator_id = "+ mobile_operator + " and sc.sms_mt_code = " + short_code+" and s.state_id = 1 and s.msisdn = " + msisdn + (service_id != "0" ? " and s.service_id = " + service_id : "");
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    CheckNineMobileUser res = new CheckNineMobileUser();
                    result = new List<CheckNineMobileUser>();
                    while (reader.Read())
                    {
                        res = new CheckNineMobileUser()
                        {
                            subscriber_id = Convert.ToInt64(reader.GetValue(0)),
                            service_id = Convert.ToInt32(reader.GetValue(1)),
                            keyword = Convert.ToString(reader.GetValue(2)),
                            service_name = Convert.ToString(reader.GetValue(3))
                        };
                        result.Add(res);
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }


        public static NineMobileBillingDetails GetUpdateBillingAttempt(string billing_attempt_id, string result, string reason, string code, ref List<LogLines> lines)
        {
            NineMobileBillingDetails res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select n.id, n.subscriber_id, s.msisdn, s.service_id, p.real_price, p.price_id, s.subscription_date, if(o.id IS NULL, 0, 1) from nine_mobile_billingattempt n, subscribers s LEFT JOIN rebilling.onetime_billing o ON (o.subscriber_id = s.subscriber_id), prices p where p.price_id = n.price_id and s.subscriber_id = n.subscriber_id and n.id = " + billing_attempt_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    
                    while (reader.Read())
                    {
                        res = new NineMobileBillingDetails()
                        {
                            subscriber_id = Convert.ToInt64(reader.GetValue(1)),
                            msisdn = Convert.ToInt64(reader.GetValue(2)),
                            service_id = Convert.ToInt32(reader.GetValue(3)),
                            price = reader.GetDouble(4),
                            price_id = Convert.ToInt32(reader.GetValue(5)),
                            subscription_date = reader.GetDateTime(6).ToString("yyyy-MM-dd HH:mm:ss"),
                            is_onetime = Convert.ToInt32(reader.GetValue(7))
                        };
                    }
                    ExecuteQuery("update nine_mobile_billingattempt set code = " + code + ", description = '"+reason.Replace("'","\'")+ "', result_date_time = now() where id = " + billing_attempt_id, ref lines);
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static List<SMSContent> GetSMSContent(string service_id, ref List<LogLines> lines)
        {
            List<SMSContent> result = null;
            SMSContent res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try 
            {
                
                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select sc.sms_content_id, sc.service_id, s1.service_name, sc.content from sms_content sc, services s1 where sc.service_id = "+ service_id + " and s1.service_id = sc.service_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<SMSContent>();
                    while (reader.Read())
                    {
                        res = new SMSContent()
                        {
                            sms_content_id = Convert.ToInt32(reader.GetValue(0)),
                            service_id = Convert.ToInt32(reader.GetValue(1)),
                            service_name = Convert.ToString(reader.GetValue(2)),
                            content_text = Convert.ToString(reader.GetValue(3))
                        };
                        result.Add(res);
                    }
                }
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<SMSContentStatus> GetSMSContentStatus(ref List<LogLines> lines)
        {
            List<SMSContentStatus> result = null;
            SMSContentStatus res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select t.service_id, t.service_name, sum(content), sum(subs) from (select s.service_id, s.service_name, count(s.service_id) content, 0 subs from sms_content sc, services s where s.service_id = sc.service_id group by s.service_id union all select s1.service_id, s1.service_name, 0 content, count(s1.service_id) subs from subscribers s, services s1 where s1.service_id = s.service_id and s.state_id = 1 and (s1.service_id between 93 and 143 or s1.service_id = 145) group by s1.service_id, s1.service_name) t group by t.service_id, t.service_name";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<SMSContentStatus>();
                    while (reader.Read())
                    {
                        res = new SMSContentStatus()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            service_name = Convert.ToString(reader.GetValue(1)),
                            content = Convert.ToInt32(reader.GetValue(2)),
                            active_subs = Convert.ToInt32(reader.GetValue(3))
                        };
                        result.Add(res);

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<BillingHistory> GetBillingHistory(Int64 msisdn, int service_id, ref List<LogLines> lines)
        {
            List<BillingHistory> result = null;
            BillingHistory res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s1.service_name, b.billing_date_time, p.real_price, p.curency_code  from billing b, subscribers s, prices p, services s1 where b.subscriber_id = s.subscriber_id and s1.service_id = s.service_id and p.price_id = b.price_id and s.msisdn = "+msisdn+" and s1.service_id = "+service_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<BillingHistory>();
                    while (reader.Read())
                    {
                        res = new BillingHistory()
                        {
                            service_name = Convert.ToString(reader.GetValue(0)),
                            billing_date = Convert.ToString(reader.GetValue(1)),
                            price = Convert.ToString(reader.GetValue(2)),
                            curency = Convert.ToString(reader.GetValue(3))
                        };
                        result.Add(res);

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<UserHistory> GetUserHistory(Int64 msisdn, int service_id, ref List<LogLines> lines)
        {
            List<UserHistory> result = null;
            UserHistory res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s.subscriber_id, s1.service_name, s.subscription_date, if(s.deactivation_date is null, '', s.deactivation_date), s.state_id from subscribers s, services s1 where s.service_id = "+service_id+" and s1.service_id = s.service_id and s.msisdn = " + msisdn;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<UserHistory>();
                    while (reader.Read())
                    {
                        res = new UserHistory()
                        {
                            subscriber_id = Convert.ToString(reader.GetValue(0)),
                            service_name = Convert.ToString(reader.GetValue(1)),
                            subscription_date = Convert.ToString(reader.GetValue(2)),
                            deactivation_date = Convert.ToString(reader.GetValue(3)),
                            state_id = Convert.ToString(reader.GetValue(4))
                        };
                        result.Add(res);

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static CheckLogin CheckLoginToken(int service_id, string token_id, ref List<LogLines> lines)
        {
            CheckLogin res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select sc.token, sc.token_experation from service_configuration sc where sc.service_id = " + service_id + " limit 1";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        if ((Convert.ToString(reader.GetValue(0)) != token_id) || (Convert.ToString(reader.GetValue(1)) == "0000-00-00 00:00:00"))
                        {
                            res = new CheckLogin()
                            {
                                ResultCode = 2001,
                                Description = "Invalid Token",
                            };
                            
                        }
                        if (Convert.ToString(reader.GetValue(1)) != "0000-00-00 00:00:00")
                        {
                            DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(1)));
                            if (DateTime.Now > token_exp)
                            {
                                res = new CheckLogin()
                                {
                                    ResultCode = 2002,
                                    Description = "Token Expired",
                                };
                            }
                        }

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static bool CheckShortURL(string id_tocheck, ref List<LogLines> lines)
        {
            bool result = false;

            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "select s.id from shrot_url_access s where s.id = '" + id_tocheck + "'";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = true;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static ServiceConfiguration GetVideoServiceConf(int service_id, ref List<LogLines> lines)
        {

            ServiceConfiguration res1 = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select sc.service_id, sc.service_password, v.url from service_configuration sc, video52223_configuration v where v.service_id = sc.service_id and v.service_id = "+service_id+" group by sc.service_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    
                    while (reader.Read())
                    {
                        string token = "";
                        Api.HttpItems.LoginRequest RequestBody = new Api.HttpItems.LoginRequest()
                        {
                            ServiceID = Convert.ToInt32(reader.GetValue(0)),
                            Password = Convert.ToString(reader.GetValue(1))
                        };
                        Api.HttpItems.LoginResponse response = Api.CommonFuncations.Login.DoLogin(RequestBody);
                        if (response != null)
                        {
                            if (response.ResultCode == 1000)
                            {
                                token = response.TokenID;
                            }
                        }

                        string lp_url = Convert.ToString(reader.GetValue(2));
                        //string lp_url = "http://video.ydot.co/home/layout/" + Convert.ToInt32(reader.GetValue(0));
                        //switch (Convert.ToInt32(reader.GetValue(0)))
                        //{
                        //    //case 630:
                        //    //    lp_url = "http://sporttv.ydot.co";
                        //    //    break;
                        //    case 628:
                        //    case 82:
                        //        lp_url = "http://worldcup.ydot.co";
                        //        break;
                        //}
                        
                        res1 = new ServiceConfiguration()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            service_password = Convert.ToString(reader.GetValue(1)),
                            xml_url = Convert.ToString(reader.GetValue(2)),
                            lp_url = lp_url,//ProcessXML.GetXMLNode(Convert.ToString(reader.GetValue(2)), "loc", ref lines),
                            token = token
                        };
                        
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res1;
        }

        public static ServiceConfiguration GetDevotionalServiceConf(int service_id, ref List<LogLines> lines)
        {

            ServiceConfiguration res1 = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select sc.service_id, sc.service_password, v.url from service_configuration sc, devotional_services v where v.service_id = sc.service_id and v.service_id = " + service_id + " group by sc.service_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        string token = "";
                        Api.HttpItems.LoginRequest RequestBody = new Api.HttpItems.LoginRequest()
                        {
                            ServiceID = Convert.ToInt32(reader.GetValue(0)),
                            Password = Convert.ToString(reader.GetValue(1))
                        };
                        Api.HttpItems.LoginResponse response = Api.CommonFuncations.Login.DoLogin(RequestBody);
                        if (response != null)
                        {
                            if (response.ResultCode == 1000)
                            {
                                token = response.TokenID;
                            }
                        }

                        res1 = new ServiceConfiguration()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            service_password = Convert.ToString(reader.GetValue(1)),
                            xml_url = Convert.ToString(reader.GetValue(2)),
                            lp_url = "http://video.ydot.co/home/layout/" + Convert.ToInt32(reader.GetValue(0)),//ProcessXML.GetXMLNode(Convert.ToString(reader.GetValue(2)), "loc", ref lines),
                            token = token
                        };

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res1;
        }


        public static string CreateYBShortURL(string msisdn, string long_url, int msg_id, ref List<LogLines> lines)
        {
            string short_url = "";
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                string new_id = "";
                bool check = true;
                int start_len = 4;
                do
                {
                    new_id = CommonFuncations.Base64.CreateRandomStarting(start_len) + "_" + msg_id;
                    start_len = start_len + 1;
                    check = CheckShortURL(new_id, ref lines);
                    if (!check)
                    {
                        short_url = Cache.ServerSettings.GetServerSettings("ShortYBURLBase", ref lines) + new_id;
                        bool r = Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                        check = (r == true ? false : true);
                    }
                } while (check == true);


                //connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                //connection.Open();

                //MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                //string myid = CommonFuncations.Base64.CreateRandomStarting(4);
                //myid = myid + "_" + msg_id;
                //mySql = "select s.id from shrot_url_access s where s.id = '" + myid +"'";
                //lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                //command.CommandText = mySql;
                //command.CommandTimeout = 6000;
                //reader = command.ExecuteReader();

                //if (reader.HasRows == true)
                //{
                //    string new_id = "";
                //    bool check = true;
                //    int start_len = 4;
                //    do
                //    {
                //        new_id = CommonFuncations.Base64.CreateRandomStarting(start_len) + "_" + msg_id;
                //        start_len = start_len + 1;
                //        check = CheckShortURL(new_id, ref lines);
                //        if (check)
                //        {
                //            short_url = Cache.ServerSettings.GetServerSettings("ShortYBURLBase", ref lines) + new_id;
                //            bool r = Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                //            if (!r)
                //            {
                //                check = false;
                //            }
                //        }
                //    } while (check == true);
                    
                //}
                //else
                //{
                //    short_url = Cache.ServerSettings.GetServerSettings("ShortYBURLBase", ref lines) + myid;
                //    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + myid + "',1,'" + long_url + "',now(), "+msisdn+")", ref lines);
                //}
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }

        public static string CreateShortURL(string short_url, string msisdn, string long_url, int msg_id, ref List<LogLines> lines)
        {
            string t_short_url = short_url;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                string new_id = "";
                bool check = true;
                int start_len = 4;
                do
                {
                    new_id = CommonFuncations.Base64.CreateRandomStarting(start_len) + "_" + msg_id;
                    start_len = start_len + 1;
                    check = CheckShortURL(new_id, ref lines);
                    if (!check)
                    {
                        short_url = t_short_url + new_id;
                        long_url = (long_url.Contains("&") ? long_url + "&cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1) : long_url + "?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        bool r = Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                        check = (r == true ? false : true);
                    }

                } while (check == true);


                //connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                //connection.Open();

                //MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                //string myid = CommonFuncations.Base64.CreateRandomStarting(4);
                //myid = myid + "_" + msg_id;
                //mySql = "select s.id from shrot_url_access s where s.id = '" + myid + "'";
                //lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                //command.CommandText = mySql;
                //command.CommandTimeout = 6000;
                //reader = command.ExecuteReader();

                //if (reader.HasRows == true)
                //{
                    
                    
                //}
                //else
                //{
                    
                //    short_url = short_url + myid;
                //    long_url = (long_url.Contains("&") ? long_url + "&cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1) : long_url + "?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                //    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + myid + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                //}
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }

        public static string CreateShortMinusURL(string short_url, string msisdn, string long_url, int msg_id, ref List<LogLines> lines)
        {
            string t_short_url = short_url;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                string new_id = "";
                bool check = true;
                int start_len = 4;
                do
                {
                    new_id = CommonFuncations.Base64.CreateRandomStarting(start_len) + "-" + msg_id;
                    start_len = start_len + 1;
                    check = CheckShortURL(new_id, ref lines);
                    if (check == false)
                    {
                        short_url = t_short_url + new_id;
                        long_url = (long_url.Contains("&") ? long_url + "&cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1) : long_url + "?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                        bool r = Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                        check = (r == true ? false : true);
                    }
                } while (check == true);

                //connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                //connection.Open();

                //MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                //string myid = CommonFuncations.Base64.CreateRandomStarting(4);
                //myid = myid + "-" + msg_id;
                //mySql = "select s.id from shrot_url_access s where s.id = '" + myid + "'";
                //lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                //command.CommandText = mySql;
                //command.CommandTimeout = 6000;
                //reader = command.ExecuteReader();

                //if (reader.HasRows == true)
                //{
                   
                    
                //}
                //else
                //{

                //    short_url = short_url + myid;
                //    long_url = (long_url.Contains("&") ? long_url + "&cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1) : long_url + "?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                //    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + myid + "',1,'" + long_url + "',now(), " + msisdn + ")", ref lines);
                //}
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }

        public static string CreateShortURL(Int64 sub_id, string long_url, ref List<LogLines> lines)
        {
            string short_url = "";
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "select s.id from shrot_url_access s where s.subscriber_id = " + sub_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    string id = "";
                    while (reader.Read())
                    {

                        id = Convert.ToString(reader.GetValue(0));
                        short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase", ref lines) + id;
                        Api.DataLayer.DBQueries.ExecuteQuery("update shrot_url_access set real_url = '" + long_url + "', creation_date = now() where id = '" + id + "'", ref lines);
                    }
                }
                else
                {
                    string new_id = "";
                    bool check = true;
                    do
                    {
                        new_id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                        check = CheckShortURL(new_id, ref lines);
                    } while (check == true);
                    short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase", ref lines) + new_id;
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now())", ref lines);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }

        public static string CreateShortURL32(Int64 sub_id, string long_url, ref List<LogLines> lines)
        {
            string short_url = "";
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "select s.id from shrot_url_access s where s.subscriber_id = " + sub_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    string id = "";
                    while (reader.Read())
                    {

                        id = Convert.ToString(reader.GetValue(0));
                        short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + id;
                        Api.DataLayer.DBQueries.ExecuteQuery("update shrot_url_access set real_url = '" + long_url + "', creation_date = now() where id = '" + id + "'", ref lines);
                    }
                }
                else
                {
                    string new_id = "";
                    bool check = true;
                    do
                    {
                        new_id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                        check = CheckShortURL(new_id, ref lines);
                    } while (check == true);
                    short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase_32", ref lines) + new_id;
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now())", ref lines);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }



        public static string CreateShortURL(Int64 sub_id, string long_url, string msisdn, ref List<LogLines> lines)
        {
            string short_url = "";
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {

                connection = new MySql.Data.MySqlClient.MySqlConnection(Api.Cache.ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "select s.id from shrot_url_access s where s.msisdn = " + msisdn;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 6000;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    string id = "";
                    while (reader.Read())
                    {

                        id = Convert.ToString(reader.GetValue(0));
                        short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase", ref lines) + id;
                        Api.DataLayer.DBQueries.ExecuteQuery("update shrot_url_access set real_url = '" + long_url + "', creation_date = now() where id = '" + id + "'", ref lines);
                    }
                }
                else
                {
                    string new_id = "";
                    bool check = true;
                    do
                    {
                        new_id = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                        check = CheckShortURL(new_id, ref lines);
                    } while (check == true);
                    short_url = Cache.ServerSettings.GetServerSettings("ShortURLBase", ref lines) + new_id;
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into shrot_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "'," + sub_id + ",'" + long_url + "',now(),"+msisdn+")", ref lines);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return short_url;
        }


        public static bool UpdateWinner(string id, bool was_payed, string payed_date, string last_dya_msg, string last_dya_attempt, bool sms_sent, string sms_msg, string last_sms_attempt, ref List<LogLines> lines)
        {
            bool result = true;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "update lndyawinners set was_payed = "+was_payed+ ", payed_date = '"+ payed_date + "', last_dya_error = '"+ last_dya_msg + "', last_dya_attempt = '"+ last_dya_attempt + "', sms_sent = "+sms_sent+ ", last_sms_attempt = '"+last_sms_attempt+ "', last_sms_error = '"+sms_msg+"' where winner_id = " + id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }
        public static bool DeleteWinner(string id,ref List<LogLines> lines)
        {
            bool result = true;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "delete from lndyawinners where winner_id = " + id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static bool AddWinner(string msisdn, string winning_date, string amount_won, ref List<LogLines> lines)
        {
            bool result = true;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                mySql = "insert into lndyawinners (msisdn, winning_date, amount) values("+msisdn+",'"+winning_date+"',"+amount_won+");";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();

                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static List<LNWinners> GetWinners(ref List<LogLines> lines)
        {
            List<LNWinners> result = null;
            LNWinners res1 = new LNWinners();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                
                mySql = "select * from lndyawinners l order by l.winning_date desc";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<LNWinners>();
                    while (reader.Read())
                    {
                        res1 = new LNWinners()
                        {
                            winner_id = Convert.ToInt32(reader.GetValue(0)),
                            MSISDN = Convert.ToInt64(reader.GetValue(1)),
                            winning_date = reader.GetDateTime(2).ToString("yyyy-MM-dd"),
                            amount = Convert.ToInt32(reader.GetValue(3)),
                            payed_date = (reader.GetDateTime(4).ToString("yyyy-MM-dd") == "1970-01-01" ? "N/A" : reader.GetDateTime(4).ToString("yyyy-MM-dd")),
                            was_payed = Convert.ToBoolean(reader.GetValue(5)),
                            last_dya_error = Convert.ToString(reader.GetValue(6)),
                            lasy_dya_attempt = (reader.GetDateTime(7).ToString("yyyy-MM-dd") == "1970-01-01" ? "N/A" : reader.GetDateTime(7).ToString("yyyy-MM-dd")),
                            sms_sent = Convert.ToBoolean(reader.GetValue(8)),
                            last_sms_sent = (reader.GetDateTime(9).ToString("yyyy-MM-dd") == "1970-01-01" ? "N/A" : reader.GetDateTime(9).ToString("yyyy-MM-dd")),
                            last_sms_error = Convert.ToString(reader.GetValue(10))

                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static DYATransactions UpdateGetDYAReciveTransSV(Int64 dya_trans, string status_code, string status_desc, ref List<LogLines> lines)
        {
            DYATransactions result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                MySql = "select msisdn, service_id, amount, dya_method, transaction_id, token_id, date_time, result from dya_transactions where dya_id = " + dya_trans;
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        lines = Add2Log(lines, "result on DB = " + Convert.ToString(reader.GetValue(7)), 100, lines[0].ControlerName);
                        lines = Add2Log(lines, "status_code = " + status_code, 100, lines[0].ControlerName);

                        if (Convert.ToString(reader.GetValue(7)) == "01" && status_code == "01")
                        {
                            result = null;
                        }
                        else
                        {
                            result = new DYATransactions()
                            {
                                msisdn = Convert.ToInt64(reader.GetValue(0)),
                                service_id = Convert.ToInt32(reader.GetValue(1)),
                                amount = Convert.ToInt32(reader.GetValue(2)),
                                dya_method = Convert.ToInt32(reader.GetValue(3)),
                                partner_transid = Convert.ToString(reader.GetValue(4)),
                                dya_trans = dya_trans,
                                airtime_tokenid = Convert.ToString(reader.GetValue(5)),
                                datetime = reader.GetDateTime(6).ToString("yyyy-MM-dd HH:mm:ss"),
                            };
                            MySql = "update dya_transactions set result = '" + status_code.Replace("'", "") + "', result_desc = '" + status_desc.Replace("'", "") + "', receive_datetime = now() where dya_id = " + dya_trans;
                            lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                            DBQueries.ExecuteQuery(MySql, ref lines);
                        }

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static DYATransactions UpdateGetDYAReciveTrans(Int64 dya_trans, string status_code, string status_desc, ref List<LogLines> lines)
        {
            DYATransactions result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                MySql = "select msisdn, service_id, amount, dya_method, transaction_id, token_id, date_time, result from dya_transactions where dya_id = " + dya_trans;
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    
                    while (reader.Read())
                    {

                        lines = Add2Log(lines, "result on DB = " + Convert.ToString(reader.GetValue(7)), 100, lines[0].ControlerName);
                        lines = Add2Log(lines, "status_code = " + status_code, 100, lines[0].ControlerName);

                        if (Convert.ToString(reader.GetValue(7)) == "01" && status_code == "01" && (Convert.ToInt32(reader.GetValue(1)) == 726 || Convert.ToInt32(reader.GetValue(1)) == 730))
                        {
                            result = null;
                        }
                        else
                        {
                            result = new DYATransactions()
                            {
                                msisdn = Convert.ToInt64(reader.GetValue(0)),
                                service_id = Convert.ToInt32(reader.GetValue(1)),
                                amount = Convert.ToInt32(reader.GetValue(2)),
                                dya_method = Convert.ToInt32(reader.GetValue(3)),
                                partner_transid = Convert.ToString(reader.GetValue(4)),
                                dya_trans = dya_trans,
                                airtime_tokenid = Convert.ToString(reader.GetValue(5)),
                                datetime = reader.GetDateTime(6).ToString("yyyy-MM-dd HH:mm:ss"),
                            };
                            MySql = "update dya_transactions set result = '" + status_code.Replace("'", "") + "', result_desc = '" + status_desc.Replace("'", "") + "', receive_datetime = now() where dya_id = " + dya_trans;
                            lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                            DBQueries.ExecuteQuery(MySql, ref lines);
                        }
                        
                        
                        
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static Int64 InsertDYAReceiveTrans(DYAReceiveMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1','',2, '"+RequestBody.TransactionID+"');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertDYAReceiveTrans(DYAReceiveMoneyRequest RequestBody, string datetime, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",'"+ datetime + "'," + RequestBody.Amount + ",'-1','',2, '" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertRefundAmountTrans(RefundAirTimeRequest RequestBody, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1','',3, '" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertRefundAmountTrans(RefundAirTimeRequest RequestBody,string dya_method, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1',''," + dya_method + ", '" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertChargeAmountTrans(ChargeAirTimeRequest RequestBody, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1','',4, '" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertDYATrans(DYATransferMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),"+RequestBody.Amount+",'-1','','"+RequestBody.TransactionID+"');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertDYATrans(DYATransferMoneyRequest RequestBody, string datetime, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",'"+ datetime + "'," + RequestBody.Amount + ",'-1','','" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertBankTrans(BankTransferRequest RequestBody, string datetime, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",'" + datetime + "'," + RequestBody.Amount + ",'-1','','" + RequestBody.TransactionID + "');";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
                Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_banktransfer_transactions (dya_id, full_name, bank_id, account_number) values(" + dya_id+",'"+RequestBody.FullName.Replace("'","''")+"','"+RequestBody.BankID+"','"+RequestBody.AccountNumber+"')", ref lines);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static Int64 InsertBankCollectionInline(BankCollectionInLineRequest RequestBody, string datetime, ref List<LogLines> lines)
        {
            Int64 dya_id = 0;
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();

                command = connection.CreateCommand();
                MySql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, transaction_id, dya_method) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",'" + datetime + "'," + RequestBody.Amount + ",'-1','','" + RequestBody.TransactionID + "',2);";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                dya_id = command.LastInsertedId;
                Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_banktransfer_transactions values(" + dya_id + ",'" + RequestBody.FullName.Replace("'", "''") + "','','','"+RequestBody.Email+"',"+RequestBody.ServiceID+",'"+RequestBody.RedirectURL+"','"+RequestBody.LogoURL+"')", ref lines);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return dya_id;
        }

        public static void UpdateDYATrans(Int64 dya_id, string result, string result_desc, ref List<LogLines> lines)
        {
            string MySql = "";

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                
                command = connection.CreateCommand();
                MySql = "update dya_transactions set result = '"+ result + "', result_desc = '"+ result_desc.Replace("'","") + "' where dya_id = "+dya_id;
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

            }
        }

        public static void InsertUpdateCBPerDate(CBPerDate res, ref List<LogLines> lines)
        {
            string mySql = "";
            Int64 line_id = 0;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select c.id from cb_per_day c where c.date = '" + res.date + "' and c.service_id = " + res.service_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    //update
                    while (reader.Read())
                    {
                        line_id = Convert.ToInt64(reader.GetValue(0));
                    }

                }
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

                connection = null;
                reader = null;
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                command = connection.CreateCommand();

                if (line_id > 0)
                {
                    mySql = "update cb_per_day set cb_end_of_day = " + res.volume + " where id = " + line_id;
                }
                else
                {
                    mySql = "insert into cb_per_day (date, service_id, cb_end_of_day) values ('"+res.date+"',"+res.service_id+","+res.volume+")";
                }
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
        }

        


        public static List<CBPerDate> GetCBPerDate(string date, ref List<LogLines> lines)
        {
            List<CBPerDate> result = null;
            CBPerDate res1 = new CBPerDate();
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                string my_date = date + " 23:59:59";
                mySql = "select service_id, '"+date+"', count(service_id) from (select * from vm_getcb v where v.state_id = 1 and v.subscription_date <= '"+ my_date + "' union all select * from vm_getcb v where v.state_id = 2 and v.subscription_date <= '" + my_date + "' and v.deactivation_date > '" + my_date + "') t group by service_id;";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<CBPerDate>();
                    while (reader.Read())
                    {
                        res1 = new CBPerDate()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            date = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                            volume = Convert.ToInt32(reader.GetValue(2))
                        };
                        result.Add(res1);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static void UpdatePTAnswer(int answer_id, int result, ref List<LogLines> lines)
        {
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "update p_user_answeres set user_answer = " + result + " where answer_id = " + answer_id;
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {

                if (connection != null)
                {
                    connection.Close();
                }

            }

        }

        public static bool CheckPTIsComplete(Int64 subscriber_id, int test_id, ref List<LogLines> lines)
        {
            bool result = false;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select count(pua.user_answer) from p_user_answeres pua where pua.subscriber_id = " + subscriber_id + " and pua.test_id = " + test_id + " and pua.user_answer != -1 group by pua.subscriber_id, pua.test_id";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader.GetValue(0)) == 5)
                        {
                            result = true;
                        }
                    }
                }
                

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static Question GetPTNextQuestionOrResult(Int64 subscriber_id, int test_id, bool is_complete, ref List<LogLines> lines)
        {
            Question result = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                if (is_complete == true)
                {
                    mySql = "select pq.question_id, pq.question, pq.test_id from p_questions pq where pq.is_result = 1 and pq.test_id = "+test_id+" and (select if (sum(pua.user_answer) is null, 0, sum(pua.user_answer)*2) from p_user_answeres pua where pua.subscriber_id = "+subscriber_id+" and pua.test_id = "+test_id+" and pua.user_answer != -1) between pq.min_points and pq.max_points";
                }
                else
                {
                    mySql = "select pq.question_id, pq.question, pq.test_id from p_questions pq where pq.test_id = " + test_id + " and pq.is_result = 0 and pq.question_id not in (select pua.question_id from p_user_answeres pua where pua.subscriber_id = "+subscriber_id+" and pua.test_id = "+test_id+") limit 1";
                }
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        result = new Question()
                        {
                            question_id = Convert.ToInt32(reader.GetValue(0)),
                            question = Convert.ToString(reader.GetValue(1)),
                            test_id = Convert.ToInt32(reader.GetValue(2))
                        };
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static UserQuestion CheckUpdateUserPTAnswer(string MSISDN, string SPID, string short_code, int message, ref List<LogLines> lines)
        {
            UserQuestion res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s.subscriber_id, s.msisdn, pua.answer_id, pua.test_id, pua.question_id from subscribers s, service_configuration sc, p_user_answeres pua where s.state_id = 1 and pua.user_answer = -1 and pua.subscriber_id = s.subscriber_id and s.service_id = sc.service_id and s.service_id = 3 and sc.spid = " + SPID + " and s.msisdn = " + MSISDN + " and sc.sms_mt_code = " + short_code + " order by pua.answer_id desc limit 1";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        //update question
                        UpdatePTAnswer(Convert.ToInt32(reader.GetValue(2)), message, ref lines);
                        //check isfull
                        bool is_complete = CheckPTIsComplete(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt32(reader.GetValue(3)), ref lines);
                        //get next question or result
                        Question question = GetPTNextQuestionOrResult(Convert.ToInt64(reader.GetValue(0)), Convert.ToInt32(reader.GetValue(3)), is_complete, ref lines);

                        res = new UserQuestion()
                        {
                            is_complete = is_complete,
                            question = question.question,
                            question_id = question.question_id,
                            test_id = question.test_id,
                            subscriber_id = Convert.ToInt64(reader.GetValue(0)),
                            MSISDN = Convert.ToInt64(reader.GetValue(1))
                        };

                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static Question GetUserPT(Int64 subscriber_id, ref List<LogLines> lines)
        {
            Question res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select * from ((select min(p.question_id) qid, p.question, p.test_id from p_questions p where p.is_result = 0 and p.test_id not in (select pua.test_id from p_user_answeres pua where pua.subscriber_id = " + subscriber_id + ") order by rand() limit 1) union all (select min(p.question_id) qid, p.question, p.test_id from p_questions p where p.is_result = 0)) zz order by zz.qid desc limit 1";

                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        res = new Question()
                        {
                            question_id = Convert.ToInt32(reader.GetValue(0)),
                            question = Convert.ToString(reader.GetValue(1)),
                            test_id = Convert.ToInt32(reader.GetValue(2))
                        };
                    }
                }
                else
                {
                    res = new Question()
                    {
                        question_id = 1,
                        question = "Do you know your true colour? Find out in 5 steps! Step 1: Music should be... 1=Passionate 2=Mellow instrumental 3=Epic. Send 1,2 or 3",
                        test_id = 1
                    };
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static List<UserQuestion> GetPTUsers(int is_today, ref List<LogLines> lines)
        {
            
            string mySql = "";
            List<UserQuestion> res = new List<UserQuestion>();
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s.subscriber_id, s.msisdn from subscribers s where s.service_id = 3";
                mySql = mySql + (is_today == 1 ? " and date(s.subscription_date) = date(now())" : " and date(s.subscription_date) != date(now())");
                mySql = mySql + " and s.state_id = 1 and s.subscriber_id not in (select pua.subscriber_id from p_user_answeres pua where pua.question_date = date(now()))";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        Question q = GetUserPT(Convert.ToInt64(reader.GetValue(0)), ref lines);
                        if (q != null)
                        {
                            res.Add(new UserQuestion { subscriber_id = Convert.ToInt64(reader.GetValue(0)), MSISDN = Convert.ToInt64(reader.GetValue(1)), question_id = q.question_id, question = q.question, test_id = q.test_id });
                        }
                    }
                }
                if (res.Count() == 0)
                {
                    res = null;
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static void InsertUserPT(Int64 subscriber_id, int test_id, int question_id, ref List<LogLines> lines)
        {
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "insert into p_user_answeres (subscriber_id, test_id, question_id, question_date, user_answer) values(" + subscriber_id + ", " + test_id + ", "+ question_id+ ", date(now()), -1)";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();



            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {

                if (connection != null)
                {
                    connection.Close();
                }

            }
        }
        
        public static void InsertUserQuote(Int64 subscriber_id, int quote_id, ref List<LogLines> lines)
        {
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "insert into users_quotes (subscriber_id, quote_id, delivery_date, was_delivered) values(" + subscriber_id + ", " + quote_id + ", now(), 1)";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();

                

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                
                if (connection != null)
                {
                    connection.Close();
                }

            }
        }

        public static Quotes GetUserQuote(Int64 subscriber_id, ref List<LogLines> lines)
        {
            Quotes res = null;
            string mySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select * from quotes q where q.quote_id not in (select uq.quote_id from users_quotes uq, subscribers s where uq.subscriber_id = s.subscriber_id and s.subscriber_id = "+subscriber_id+ ") order by rand() limit 1";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        res = new Quotes()
                        {
                            quote_id = Convert.ToInt32(reader.GetValue(0)),
                            quote = Convert.ToString(reader.GetValue(1))
                        };
                    }
                }
                else
                {
                    res = new Quotes()
                    {
                        quote_id = 1,
                        quote = "Here is your daily Quote: \"We are what we repeatedly do.Excellence, then, is not an act, but a habit\" - Aristotle"
                    };
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }

        public static List<UserQuotes> GetUserQuotes(int is_today, ref List<LogLines> lines)
        {
            string mySql = "";
            List<UserQuotes> res = new List<UserQuotes>();
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                mySql = "select s.subscriber_id, s.msisdn from subscribers s where s.service_id = 2";
                mySql = mySql + (is_today == 1 ? " and date(s.subscription_date) = date(now())" : " and date(s.subscription_date) != date(now())");
                mySql = mySql + " and s.state_id = 1 and s.subscriber_id not in (select uq.subscriber_id from users_quotes uq where date(uq.delivery_date) = date(now()))";
                lines = Add2Log(lines, mySql, 100, lines[0].ControlerName);
                command.CommandText = mySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        Quotes q = GetUserQuote(Convert.ToInt64(reader.GetValue(0)), ref lines);
                        if (q != null)
                        {
                            res.Add(new UserQuotes { subscriber_id = Convert.ToInt64(reader.GetValue(0)) , MSISDN = Convert.ToInt64(reader.GetValue(1)), quote_id = q.quote_id, quote = q.quote });
                        }
                    }
                }
                if (res.Count() == 0)
                {
                    res = null;
                }
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return res;
        }
        public static Int64 UnsubscribeSub(string msisdn, string service_id, string local_datetime, string deactivation_method_id, string deactivation_keyword, string subscription_date, ref List<LogLines> lines)
        {
            Int64 sub_id = 0;
            int state = 0;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select s.subscriber_id, s.state_id from subscribers s where s.msisdn = " + msisdn + " and s.service_id = " + service_id;// + " order by s.subscriber_id desc limit 1; ";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        sub_id = Convert.ToInt64(reader.GetValue(0));
                        state = Convert.ToInt32(reader.GetValue(1));
                        if (state == 1)
                        {
                            break;
                        }
                    }
                    if (state == 1) //deactivate
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                        }
                        connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                        connection.Open();
                        command = connection.CreateCommand();
                        MySql = "update subscribers set state_id = 2, deactivation_date = '" + local_datetime + "', deactivation_method_id = " + deactivation_method_id + ", deactivation_keyword = '" + deactivation_keyword + "' where subscriber_id = " + sub_id;
                        lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                        
                        command.CommandText = MySql;
                        command.CommandTimeout = 600;
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    //connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    //connection.Open();
                    //command = connection.CreateCommand();
                    MySql = "insert into subscribers (msisdn, service_id, subscription_date, deactivation_date, state_id, subscription_method_id, subscription_keyword) values(" + msisdn + ", " + service_id + ", '" + subscription_date + "', '"+ subscription_date + "',2, '2', '')";
                    sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(MySql, ref lines);
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    //command.CommandText = MySql;
                    //command.CommandTimeout = 600;
                    //command.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return sub_id;
        }

        public static Int64 UnsubscribeSub(string msisdn, string service_id, string local_datetime, string deactivation_method_id, string deactivation_keyword, string subscription_date, string connection_string, ref List<LogLines> lines)
        {
            Int64 sub_id = 0;
            int state = 0;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySql = "select s.subscriber_id, s.state_id from subscribers s where s.msisdn = " + msisdn + " and s.service_id = " + service_id;// + " order by s.subscriber_id desc limit 1; ";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        sub_id = Convert.ToInt64(reader.GetValue(0));
                        state = Convert.ToInt32(reader.GetValue(1));
                        if (state == 1)
                        {
                            break;
                        }
                    }
                    if (state == 1) //deactivate
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                        }
                        connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                        connection.Open();
                        command = connection.CreateCommand();
                        MySql = "update subscribers set state_id = 2, deactivation_date = '" + local_datetime + "', deactivation_method_id = " + deactivation_method_id + ", deactivation_keyword = '" + deactivation_keyword + "' where subscriber_id = " + sub_id;
                        lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                        command.CommandText = MySql;
                        command.CommandTimeout = 600;
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();
                    command = connection.CreateCommand();
                    MySql = "insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values(" + msisdn + ", " + service_id + ", '" + subscription_date + "', 1, '2', '')";
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    command.CommandText = MySql;
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return sub_id;
        }

        

        public static Int64 InsertSub(string msisdn, string service_id, int price_id, string subscription_datetime, string local_datetime, string subscription_method_id, string subscription_keyword, ref List<LogLines> lines)
        {
            Int64 sub_id = 0;
            int state_id = 0;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select s.subscriber_id, s.state_id from subscribers s where s.msisdn = " + msisdn + " and s.service_id = " + service_id; //+ " order by s.subscriber_id desc limit 1;";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        sub_id = Convert.ToInt64(reader.GetValue(0));
                        state_id = Convert.ToInt32(reader.GetValue(1));
                        if (state_id == 1)
                        {
                            break;
                        }
                    }
                }
                sub_id = (state_id == 2 ? 0 : sub_id);


                if (sub_id == 0)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();
                    MySql = "insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values(" + msisdn + ", " + service_id + ", '" + subscription_datetime + "', 1, '1', '" + subscription_keyword + "')";
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    command = connection.CreateCommand();
                    command.CommandText = MySql;
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                    sub_id = command.LastInsertedId;

                    


                }
                if (sub_id > 0 && price_id > 0)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    reader = null;
                    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();

                    command = connection.CreateCommand();
                    //insert billing
                    MySql = "select b.subscriber_id from billing b where b.subscriber_id = " + sub_id + " and date(billing_date_time) = date('"+ local_datetime + "');";
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    command.CommandText = MySql;
                    command.CommandTimeout = 600;
                    reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        MySql = "insert into billing (subscriber_id, billing_date_time, price_id) values (" + sub_id + ", '" + local_datetime + "', " + price_id + ");";
                        DBQueries.ExecuteQuery(MySql, ref lines);
                    }
                    
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return sub_id;
        }

        public static Int64 InsertSub(string msisdn, string service_id, int price_id, string subscription_datetime, string local_datetime, string subscription_method_id, string subscription_keyword, string connection_string, ref List<LogLines> lines)
        {
            Int64 sub_id = 0;
            int state_id = 0;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySql = "select s.subscriber_id, s.state_id from subscribers s where s.msisdn = " + msisdn + " and s.service_id = " + service_id; //+ " order by s.subscriber_id desc limit 1;";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        sub_id = Convert.ToInt64(reader.GetValue(0));
                        state_id = Convert.ToInt32(reader.GetValue(1));
                        if (state_id == 1)
                        {
                            break;
                        }
                    }
                }
                sub_id = (state_id == 2 ? 0 : sub_id);



                if (sub_id == 0)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                    connection.Open();
                    MySql = "insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values(" + msisdn + ", " + service_id + ", '" + subscription_datetime + "', 1, '" + subscription_method_id + "', '" + subscription_keyword + "')";
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    command = connection.CreateCommand();
                    command.CommandText = MySql;
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                    sub_id = command.LastInsertedId;




                }
                if (sub_id > 0 && price_id > 0)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    reader = null;
                    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                    connection.Open();

                    command = connection.CreateCommand();
                    //insert billing
                    MySql = "select b.subscriber_id from billing b where b.subscriber_id = " + sub_id + " and date(billing_date_time) = date('" + local_datetime + "');";
                    lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                    command.CommandText = MySql;
                    command.CommandTimeout = 600;
                    reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        MySql = "insert into billing (subscriber_id, billing_date_time, price_id) values (" + sub_id + ", '" + local_datetime + "', " + price_id + ");";
                        DBQueries.ExecuteQuery(MySql, connection_string, ref lines);
                    }

                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }


            return sub_id;
        }

        public static List<FulfillmentInfo> GetFulfillmentInfo(ref List<LogLines> lines)
        {
            List<FulfillmentInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from fulfillment_info su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<FulfillmentInfo>();
                    while (reader.Read())
                    {
                        result.Add(new FulfillmentInfo { service_id = Convert.ToInt32(reader.GetValue(0)), operationType = Convert.ToString(reader.GetValue(1)), iname = Convert.ToString(reader.GetValue(2)), productCode = Convert.ToString(reader.GetValue(3)), splitno = Convert.ToString(reader.GetValue(4)), d_productCode = Convert.ToString(reader.GetValue(5)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<ECWServiceInfo> GetECWServiceInfo(ref List<LogLines> lines)
        {
            List<ECWServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_ecw_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<ECWServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new ECWServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), cacert_path = Convert.ToString(reader.GetValue(1)), cert_path = Convert.ToString(reader.GetValue(2)), key_path = Convert.ToString(reader.GetValue(3)), username = Convert.ToString(reader.GetValue(4)), password = Convert.ToString(reader.GetValue(5)), validateaccountholder = Convert.ToString(reader.GetValue(6)), transfer = Convert.ToString(reader.GetValue(7)), gettransactionstatus = Convert.ToString(reader.GetValue(8)), debit = Convert.ToString(reader.GetValue(9)), getaccountholderinfo = Convert.ToString(reader.GetValue(10)), output_path = Convert.ToString(reader.GetValue(11)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<NJService> GetNJServices(ref List<LogLines> lines)
        {
            List<NJService> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "SELECT * FROM service_configuration_nj";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<NJService>();
                    while (reader.Read())
                    {
                        result.Add(new NJService { nj_service_id = Convert.ToInt32(reader.GetValue(0)), service_id = Convert.ToInt32(reader.GetValue(1)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<FlutterwaveServiceInfo> GetFlutterWaveServiceInfo(ref List<LogLines> lines)
        {
            List<FlutterwaveServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_flutterwave_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<FlutterwaveServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new FlutterwaveServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), bearear = Convert.ToString(reader.GetValue(1)), currency = Convert.ToString(reader.GetValue(2)), callback_url = Convert.ToString(reader.GetValue(3)), banktransfer_url = Convert.ToString(reader.GetValue(4)), country = Convert.ToString(reader.GetValue(5)), momo_receive_url = Convert.ToString(reader.GetValue(6)), momo_transfer_url = Convert.ToString(reader.GetValue(7)), momo_validate_receive_url = Convert.ToString(reader.GetValue(8)), momo_validate_transfer_url = Convert.ToString(reader.GetValue(9)), balances_url = Convert.ToString(reader.GetValue(10)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<OrangeServiceInfo> GetOrangeServiceInfo(ref List<LogLines> lines)
        {
            List<OrangeServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_orange_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<OrangeServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new OrangeServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), client_id = Convert.ToString(reader.GetValue(1)), client_secret = Convert.ToString(reader.GetValue(2)), base_url = Convert.ToString(reader.GetValue(3)), token_url = Convert.ToString(reader.GetValue(4)), bearer = Convert.ToString(reader.GetValue(5)), bearer_date = reader.GetDateTime(6).ToString("yyyy-MM-dd HH:mm:ss"), currency = Convert.ToString(reader.GetValue(7)), posId = Convert.ToString(reader.GetValue(8)), sendsms_url = Convert.ToString(reader.GetValue(9)), webpay_url = Convert.ToString(reader.GetValue(10)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class PisiMobileServiceInfo
        {
            public int service_id { get; set; }
            public int sms_id { get; set; }
            public int subscription_id { get; set; }
        }

        public static List<PisiMobileServiceInfo> GetPisiMobileServiceInfo(ref List<LogLines> lines)
        {
            List<PisiMobileServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_pisi_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<PisiMobileServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new PisiMobileServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), sms_id = Convert.ToInt32(reader.GetValue(1)), subscription_id = Convert.ToInt32(reader.GetValue(2)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class AirTelCGServiceInfo
        {
            public int service_id { get; set; }
            public string user_id { get; set; }
            public string password { get; set; }
            public string credit_url { get; set; }
            public string deposit_url { get; set; }
            public string callback_url { get; set; }
            public string check_status_url { get; set; }
            
        }

        public static List<AirTelCGServiceInfo> GetAirTelServiceInfo(ref List<LogLines> lines)
        {
            List<AirTelCGServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from airtel_cg_conf su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<AirTelCGServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new AirTelCGServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), user_id = Convert.ToString(reader.GetValue(1)), password = Convert.ToString(reader.GetValue(2)), credit_url = Convert.ToString(reader.GetValue(3)), deposit_url = Convert.ToString(reader.GetValue(4)), callback_url = Convert.ToString(reader.GetValue(5)), check_status_url = Convert.ToString(reader.GetValue(6)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class PisiMobileNewServiceInfo
        {
            public int service_id { get; set; }
            public int pisi_service_id { get; set; }
            public string password { get; set; }
        }
        public static List<PisiMobileNewServiceInfo> GetPisiMobileNewServiceInfo(ref List<LogLines> lines)
        {
            List<PisiMobileNewServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_newpisi_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<PisiMobileNewServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new PisiMobileNewServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), pisi_service_id = Convert.ToInt32(reader.GetValue(1)), password = Convert.ToString(reader.GetValue(2)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<DusnServiceInfo> GetDusanServiceInfo(ref List<LogLines> lines)
        {
            List<DusnServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from lotto_service_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<DusnServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new DusnServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), token_id = Convert.ToString(reader.GetValue(1)), exp_datetime = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss") });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<SayThanksServiceInfo> GetSayThanksServiceInfo(ref List<LogLines> lines)
        {
            List<SayThanksServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select ssc.service_id, ssc.username, ssc.password, ssc.bearer_token, ssc.bearer_token_expiration_date, ssc.base_url, ssc.campaign_id from saythanks_service_configuration ssc;";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<SayThanksServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new SayThanksServiceInfo 
                        { 
                            service_id = Convert.ToInt32(reader.GetValue(0)), 
                            username = Convert.ToString(reader.GetValue(1)), 
                            password = Convert.ToString(reader.GetValue(2)), 
                            bearer_token = Convert.ToString(reader.GetValue(3)), 
                            bearer_token_expiration_date = reader.GetDateTime(4).ToString("yyyy-MM-dd HH:mm:ss"), 
                            base_url = Convert.ToString(reader.GetValue(5)), 
                            campaign_id = Convert.ToInt32(reader.GetValue(6))
                            
                            
                        });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<BereloServiceInfo> GetBereloServiceInfo(ref List<LogLines> lines)
        {
            List<BereloServiceInfo> result_list = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from yellowdot.berelo_service_configuration bsc;";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result_list = new List<BereloServiceInfo>();
                    while (reader.Read())
                    {
                        result_list.Add(new BereloServiceInfo
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            api_key = Convert.ToString(reader.GetValue(1)),
                            campaign_id = Convert.ToInt32(reader.GetValue(2)),
                            base_url = Convert.ToString(reader.GetValue(3))

                        });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result_list;
        }



        public static List<MADAPIServiceInfo> GetMADAPIServiceInfo(ref List<LogLines> lines)
        {
            List<MADAPIServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_madapi_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<MADAPIServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new MADAPIServiceInfo { 
                            service_id = Convert.ToInt32(reader.GetValue(0)), 
                            consumer_key = Convert.ToString(reader.GetValue(1)), 
                            consumer_secret = Convert.ToString(reader.GetValue(2)), 
                            base_url = Convert.ToString(reader.GetValue(3)), 
                            access_token = Convert.ToString(reader.GetValue(4)), 
                            access_token_validdate = reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm:ss"), 
                            sender_address = Convert.ToString(reader.GetValue(6)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<OpenAPIServiceInfo> GetOpenAPIServiceInfo(ref List<LogLines> lines)
        {
            List<OpenAPIServiceInfo> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_openapi_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<OpenAPIServiceInfo>();
                    while (reader.Read())
                    {
                        result.Add(new OpenAPIServiceInfo { service_id = Convert.ToInt32(reader.GetValue(0)), transfer_apiuser = Convert.ToString(reader.GetValue(1)), transfer_apikey = Convert.ToString(reader.GetValue(2)), transfer_ocp_ask = Convert.ToString(reader.GetValue(3)), transfer_b_token = Convert.ToString(reader.GetValue(4)), transfer_b_date = reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm:ss"), receive_apiuser = Convert.ToString(reader.GetValue(6)), receive_apikey = Convert.ToString(reader.GetValue(7)), receive_ocp_ask = Convert.ToString(reader.GetValue(8)), receive_b_token = Convert.ToString(reader.GetValue(9)), receive_b_date = reader.GetDateTime(10).ToString("yyyy-MM-dd HH:mm:ss"), base_url = Convert.ToString(reader.GetValue(11)), curency = Convert.ToString(reader.GetValue(12)), x_target_environment = Convert.ToString(reader.GetValue(13)), callback_url = Convert.ToString(reader.GetValue(14)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<KKiaPay> GetKKiaPayServiceInfo(ref List<LogLines> lines)
        {
            List<KKiaPay> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_kkiapay_configuration su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<KKiaPay>();
                    while (reader.Read())
                    {
                        result.Add(new KKiaPay { service_id = Convert.ToInt32(reader.GetValue(0)), api_key = Convert.ToString(reader.GetValue(1)), logo_url = Convert.ToString(reader.GetValue(2)), callback_url = Convert.ToString(reader.GetValue(3)), is_staging = Convert.ToBoolean(reader.GetValue(4)), position = Convert.ToString(reader.GetValue(5)), theme = Convert.ToString(reader.GetValue(6)) });
                    }
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<ServiceURLS> GetServiceURLS(ref List<LogLines> lines)
        {
            List<ServiceURLS> result = null;
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySql = "select * from service_urls su";
                lines = Add2Log(lines, MySql, 100, lines[0].ControlerName);
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<ServiceURLS>();
                    while (reader.Read())
                    {
                        result.Add(new ServiceURLS { service_id = Convert.ToInt32(reader.GetValue(0)), billing_url = Convert.ToString(reader.GetValue(1)), subscription_url = Convert.ToString(reader.GetValue(2)), mo_url = Convert.ToString(reader.GetValue(3)), unsubscription_url = Convert.ToString(reader.GetValue(4)), sms_dlr_url = Convert.ToString(reader.GetValue(5)), dya_receive_url = Convert.ToString(reader.GetValue(6)), chargeamount_url = Convert.ToString(reader.GetValue(7)), notification_method_id = Convert.ToInt32(reader.GetValue(8)), service_url = Convert.ToString(reader.GetValue(9)), error_url = Convert.ToString(reader.GetValue(10)), dtt_url = Convert.ToString(reader.GetValue(11)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<Int64> GetPromoBlockedUsers(ref List<LogLines> lines)
        {
            List<Int64> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT msisdn FROM promo.blocked_users";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<Int64>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToInt64(reader.GetValue(0)));
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<Int64> GetUSSDSavedGamesID(string user_session_id, ref List<LogLines> lines)
        {
            List<Int64> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id from ussd_saved_games u1 WHERE u1.user_session_id = '"+user_session_id+"'";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<Int64>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToInt64(reader.GetValue(0)));
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<Int64> GetUSSDSavedGamesID(string user_session_id, string prefix, ref List<LogLines> lines)
        {
            List<Int64> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_104", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id from ussd_saved_games"+prefix+" u1 WHERE u1.user_session_id = '" + user_session_id + "'";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<Int64>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToInt64(reader.GetValue(0)));
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<IMIServiceClass> GetIMIServices(ref List<LogLines> lines)
        {
            List<IMIServiceClass> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select ic.service_id, ic.svcid from imi_configuration ic";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<IMIServiceClass>();
                    while (reader.Read())
                    {
                        result.Add(new IMIServiceClass { service_id = Convert.ToInt32(reader.GetValue(0)), svcid = Convert.ToString(reader.GetValue(1)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<MSISDNPrefixList> GetMSISDNPrefixList(ref List<LogLines> lines)
        {
            List<MSISDNPrefixList> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select sr.*, sc.token from sms_msisdn_range sr, service_configuration sc WHERE sc.service_id = sr.service_id";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<MSISDNPrefixList>();
                    while (reader.Read())
                    {
                        result.Add(new MSISDNPrefixList { prefix_id = Convert.ToInt32(reader.GetValue(0)), prefix = Convert.ToInt32(reader.GetValue(1)), service_id = Convert.ToInt32(reader.GetValue(2)), token_id = Convert.ToString(reader.GetValue(3)) });
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public class MADAPISubPlan
        {
            public int service_id { get; set; }
            public string plan { get; set; }
            public int root_service_id { get; set; }
            public string node_id { get; set; }
        }

        public static List<MADAPISubPlan> GetMADAPISubPlanService(ref List<LogLines> lines)
        {
            List<MADAPISubPlan> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * from service_madapi_sub_conf";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<MADAPISubPlan>();
                    while (reader.Read())
                    {
                        result.Add(new MADAPISubPlan { service_id = Convert.ToInt32(reader.GetValue(0)), plan = Convert.ToString(reader.GetValue(1)), root_service_id = Convert.ToInt32(reader.GetValue(2)), node_id = Convert.ToString(reader.GetValue(3))});
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }
        public static List<ServiceClass> GetServices(ref List<LogLines> lines)
        {
            List<ServiceClass> result = null;
            ServiceClass res = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select s.service_id, s.service_name, sc.spid, sc.real_service_id, sc.product_id, sc.sms_mt_code, "
                                    + "o.operator_name, c.country_name, sc.service_password, sc.is_ondemand, sc.allow_dya, sc.allow_airtimerefund, "
                                    + "sc.is_staging, sc.operator_id, sc.subscribe_wo_serviceid, sc.allow_airtimecharge, sc.password, sc.airtime_amount, "
                                    + "sc.airtime_code, if(ns.service_name is null,'',ns.service_name), sc.momo_service_id, sc.dya_type, sc.airtime_type, "
                                    + "if(sm.daily_limitation IS NULL,0,sm.daily_limitation), sc.timezone_code, if(sm.user_w_limit IS NULL,0,sm.user_w_limit), "
                                    + "if(sm.user_limitation IS NULL, 0, sm.user_limitation), if(sm.hour_diff1 IS NULL, 0, sm.hour_diff1), "
                                    + "if(sm.timeout_between_failed_withdraw IS NULL, 0, sm.timeout_between_failed_withdraw), "
                                    + "if(sm.hour_diff IS NULL, '00:00', sm.hour_diff), sc.use_fulfilment, if(sm.delay_withdraw IS NULL, 0, sm.delay_withdraw), "
                                    + "if(sm.delay_withdraw_amount IS NULL, 0, sm.delay_withdraw_amount), if(sm.user_f_limit IS NULL,0,sm.user_f_limit), "
                                    + "if(sm.whitelist_only IS NULL, 0, sm.whitelist_only), sc.add_zero, if(sm.block_withdraw is null, 0, sm.block_withdraw), "
                                    + "if(sm.block_deposit is null, 0, sm.block_deposit), "
                                    + "if(s.subscription_amount is null, 0, s.subscription_amount) "
                                    + "from service_configuration sc, "
                                    + "operators o, "
                                    + "countries c, "
                                    + "services s "
                                    + "left join nine_mobile_service_name ns on (ns.service_id = s.service_id) "
                                    + "left JOIN service_momo_limitation sm ON (sm.service_id = s.service_id) "
                                    + "where s.service_id = sc.service_id and c.country_id = o.country_id and o.operator_id = sc.operator_id"
                                    ;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<ServiceClass>();
                    while (reader.Read())
                    {
                        res = new ServiceClass()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            service_name = Convert.ToString(reader.GetValue(1)),
                            spid = Convert.ToString(reader.GetValue(2)),
                            real_service_id = Convert.ToString(reader.GetValue(3)),
                            product_id = Convert.ToString(reader.GetValue(4)),
                            sms_mt_code = Convert.ToString(reader.GetValue(5)),
                            operator_name = Convert.ToString(reader.GetValue(6)),
                            country_name = Convert.ToString(reader.GetValue(7)),
                            service_password = Convert.ToString(reader.GetValue(8)),
                            is_ondemand = Convert.ToBoolean(reader.GetValue(9)),
                            allow_dya = Convert.ToBoolean(reader.GetValue(10)),
                            allow_refundairtime = Convert.ToBoolean(reader.GetValue(11)),
                            is_staging = Convert.ToBoolean(reader.GetValue(12)),
                            operator_id = Convert.ToInt32(reader.GetValue(13)),
                            subscribe_wo_serviceid = Convert.ToBoolean(reader.GetValue(14)),
                            allow_chargeairtime = Convert.ToBoolean(reader.GetValue(15)),
                            spid_password = Convert.ToString(reader.GetValue(16)),
                            airtimr_amount = Convert.ToInt32(reader.GetValue(17)),
                            airtime_code = Convert.ToInt32(reader.GetValue(18)),
                            nine_mobile_service_name = Convert.ToString(reader.GetValue(19)),
                            momo_service_id = Convert.ToString(reader.GetValue(20)),
                            dya_type = Convert.ToInt32(reader.GetValue(21)),
                            airtime_type = Convert.ToInt32(reader.GetValue(22)),
                            daily_momo_limit = Convert.ToInt64(reader.GetValue(23)),
                            timezone_code = Convert.ToString(reader.GetValue(24)),
                            user_w_limit = Convert.ToInt32(reader.GetValue(25)),
                            user_limit_amount = Convert.ToInt32(reader.GetValue(26)),
                            hour_diff = Convert.ToString(reader.GetValue(27)),
                            timeout_between_failed_withdraw = Convert.ToInt32(reader.GetValue(28)),
                            convert_tz = Convert.ToString(reader.GetValue(29)),
                            use_fulfilment = Convert.ToBoolean(reader.GetValue(30)),
                            delay_withdraw = Convert.ToBoolean(reader.GetValue(31)),
                            delay_withdraw_amount = Convert.ToInt32(reader.GetValue(32)),
                            user_f_limit = Convert.ToInt32(reader.GetValue(33)),
                            whitelist_only = Convert.ToBoolean(reader.GetValue(34)),
                            add_zero = Convert.ToBoolean(reader.GetValue(35)),
                            block_withdraw = Convert.ToBoolean(reader.GetValue(36)),
                            block_deposit = Convert.ToBoolean(reader.GetValue(37)),
                            subscription_amount = Convert.ToDouble(reader.GetValue(38))
                        };

                        result.Add(res);
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<ServiceClass> GetServices(ref List<LogLines> lines, ref List<object> logMessages, string app_name)
        {
            List<ServiceClass> result = null;
            ServiceClass res = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                string mysql = "select s.service_id, s.service_name, sc.spid, sc.real_service_id, sc.product_id, sc.sms_mt_code, o.operator_name, c.country_name, sc.service_password, sc.is_ondemand, sc.allow_dya, sc.allow_airtimerefund, sc.is_staging, sc.operator_id, sc.subscribe_wo_serviceid, sc.allow_airtimecharge, sc.password, sc.airtime_amount, sc.airtime_code, if(ns.service_name is null,'',ns.service_name), sc.momo_service_id, sc.dya_type, sc.airtime_type, if(sm.daily_limitation IS NULL,0,sm.daily_limitation), sc.timezone_code, if(sm.user_w_limit IS NULL,0,sm.user_w_limit), if(sm.user_limitation IS NULL, 0, sm.user_limitation), if(sm.hour_diff1 IS NULL, 0, sm.hour_diff1), if(sm.timeout_between_failed_withdraw IS NULL, 0, sm.timeout_between_failed_withdraw), if(sm.hour_diff IS NULL, '00:00', sm.hour_diff), sc.use_fulfilment, if(sm.delay_withdraw IS NULL, 0, sm.delay_withdraw), if(sm.delay_withdraw_amount IS NULL, 0, sm.delay_withdraw_amount), if(sm.user_f_limit IS NULL,0,sm.user_f_limit), if(sm.whitelist_only IS NULL, 0, sm.whitelist_only), sc.add_zero, if(sm.block_withdraw is null, 0, sm.block_withdraw), if(sm.block_deposit is null, 0, sm.block_deposit) from service_configuration sc, operators o, countries c, services s left join nine_mobile_service_name ns on (ns.service_id = s.service_id) left JOIN service_momo_limitation sm ON (sm.service_id = s.service_id) where s.service_id = sc.service_id and c.country_id = o.country_id and o.operator_id = sc.operator_id";
                logMessages.Add(new { message = mysql, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueris.GetServices" });
                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<ServiceClass>();
                    while (reader.Read())
                    {
                        res = new ServiceClass()
                        {
                            service_id = Convert.ToInt32(reader.GetValue(0)),
                            service_name = Convert.ToString(reader.GetValue(1)),
                            spid = Convert.ToString(reader.GetValue(2)),
                            real_service_id = Convert.ToString(reader.GetValue(3)),
                            product_id = Convert.ToString(reader.GetValue(4)),
                            sms_mt_code = Convert.ToString(reader.GetValue(5)),
                            operator_name = Convert.ToString(reader.GetValue(6)),
                            country_name = Convert.ToString(reader.GetValue(7)),
                            service_password = Convert.ToString(reader.GetValue(8)),
                            is_ondemand = Convert.ToBoolean(reader.GetValue(9)),
                            allow_dya = Convert.ToBoolean(reader.GetValue(10)),
                            allow_refundairtime = Convert.ToBoolean(reader.GetValue(11)),
                            is_staging = Convert.ToBoolean(reader.GetValue(12)),
                            operator_id = Convert.ToInt32(reader.GetValue(13)),
                            subscribe_wo_serviceid = Convert.ToBoolean(reader.GetValue(14)),
                            allow_chargeairtime = Convert.ToBoolean(reader.GetValue(15)),
                            spid_password = Convert.ToString(reader.GetValue(16)),
                            airtimr_amount = Convert.ToInt32(reader.GetValue(17)),
                            airtime_code = Convert.ToInt32(reader.GetValue(18)),
                            nine_mobile_service_name = Convert.ToString(reader.GetValue(19)),
                            momo_service_id = Convert.ToString(reader.GetValue(20)),
                            dya_type = Convert.ToInt32(reader.GetValue(21)),
                            airtime_type = Convert.ToInt32(reader.GetValue(22)),
                            daily_momo_limit = Convert.ToInt64(reader.GetValue(23)),
                            timezone_code = Convert.ToString(reader.GetValue(24)),
                            user_w_limit = Convert.ToInt32(reader.GetValue(25)),
                            user_limit_amount = Convert.ToInt32(reader.GetValue(26)),
                            hour_diff = Convert.ToString(reader.GetValue(27)),
                            timeout_between_failed_withdraw = Convert.ToInt32(reader.GetValue(28)),
                            convert_tz = Convert.ToString(reader.GetValue(29)),
                            use_fulfilment = Convert.ToBoolean(reader.GetValue(30)),
                            delay_withdraw = Convert.ToBoolean(reader.GetValue(31)),
                            delay_withdraw_amount = Convert.ToInt32(reader.GetValue(32)),
                            user_f_limit = Convert.ToInt32(reader.GetValue(33)),
                            whitelist_only = Convert.ToBoolean(reader.GetValue(34)),
                            add_zero = Convert.ToBoolean(reader.GetValue(35)),
                            block_withdraw = Convert.ToBoolean(reader.GetValue(36)),
                            block_deposit = Convert.ToBoolean(reader.GetValue(37))
                        };

                        result.Add(res);
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueris.GetServices" });
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<AirtelTigoInfo> GetAirtelTigoInfoByServiceID(int service_id, ref List<LogLines> lines)
        {
            List<AirtelTigoInfo> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * from yellowdot.airtel_tigo_config atc where atc.service_id = " + service_id;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<AirtelTigoInfo>();
                    while (reader.Read())
                    {
                        result.Add(new AirtelTigoInfo 
                        { 
                            service_id = Convert.ToInt32(reader.GetValue(0)), 
                            base_url = Convert.ToString(reader.GetValue(1)), 
                            product_id_sub = Convert.ToInt64(reader.GetValue(2)),  
                            api_key = Convert.ToString(reader.GetValue(3)), 
                            largeAccount = Convert.ToInt64(reader.GetValue(4)), 
                            preSharedKey = Convert.ToString(reader.GetValue(5)), 
                            mnc = Convert.ToInt32(reader.GetValue(6)), 
                            mcc = Convert.ToInt32(reader.GetValue(7)) 
                        });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<PriceClass> GetPrices(ref List<LogLines> lines)
        {
            List<PriceClass> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select p.price_id, p.service_id, p.price, p.curency_code from prices p;";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<PriceClass>();
                    while (reader.Read())
                    {
                        result.Add(new PriceClass
                        {
                            price_id = Convert.ToInt32(reader.GetValue(0)),
                            service_id = Convert.ToInt32(reader.GetValue(1)),
                            price = Convert.ToDouble(reader.GetValue(2)),
                            curency_code = Convert.ToString(reader.GetValue(3))
                        });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<USSDBonus> GetUSSDBonus(ref List<LogLines> lines)
        {
            List<USSDBonus> result = null;
            //MySql.Data.MySqlClient.MySqlConnection connection = null;
            //MySql.Data.MySqlClient.MySqlDataReader reader = null;
            //try
            //{
            //    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
            //    connection.Open();

            //    MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            //    command.CommandText = "SELECT u.bonus_name, u.amount, u.min_selections, u.min_odd_per_selection, u.min_total_odd, ubu.id, ubu.msisdn, u.service_id FROM ussd_bonus u, ussd_bonus_users ubu WHERE ubu.bonus_id = u.id AND ubu.id NOT IN (SELECT ubb.ussd_bonus_user_id FROM ussd_bonus_bets ubb) AND u.`status` = 1 AND NOW() BETWEEN u.start_date AND u.end_date";
            //    command.CommandTimeout = 600;
            //    reader = command.ExecuteReader();

            //    if (reader.HasRows == true)
            //    {
            //        result = new List<USSDBonus>();
            //        while (reader.Read())
            //        {
            //            result.Add(new USSDBonus { bonus_name = Convert.ToString(reader.GetValue(0)), amount = Convert.ToInt32(reader.GetValue(1)), min_selections = Convert.ToInt32(reader.GetValue(2)), min_odd_per_selections = reader.GetDouble(3), min_total_odd = reader.GetDouble(4), bonus_user_id = Convert.ToString(reader.GetValue(5)), msisdn = Convert.ToString(reader.GetValue(6)), service_id = Convert.ToString(reader.GetValue(7)) });
            //        }
            //    }


            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }
            //    if (connection != null)
            //    {
            //        connection.Close();
            //    }

            //}
            return result;
        }

        public static List<USSDBonus> GetUSSDBonus(string service_id, ref List<LogLines> lines)
        {
            List<USSDBonus> result = null;
            //MySql.Data.MySqlClient.MySqlConnection connection = null;
            //MySql.Data.MySqlClient.MySqlDataReader reader = null;
            //try
            //{
            //    connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
            //    connection.Open();

            //    MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
            //    command.CommandText = "SELECT u.bonus_name, u.amount, u.min_selections, u.min_odd_per_selection, u.min_total_odd, ubu.id, ubu.msisdn, u.service_id FROM ussd_bonus u, ussd_bonus_users ubu WHERE ubu.bonus_id = u.id AND ubu.id NOT IN (SELECT ubb.ussd_bonus_user_id FROM ussd_bonus_bets ubb) AND u.`status` = 1 AND NOW() BETWEEN u.start_date AND u.end_date AND u.service_id = " + service_id;
            //    command.CommandTimeout = 600;
            //    reader = command.ExecuteReader();

            //    if (reader.HasRows == true)
            //    {
            //        result = new List<USSDBonus>();
            //        while (reader.Read())
            //        {
            //            result.Add(new USSDBonus { bonus_name = Convert.ToString(reader.GetValue(0)), amount = Convert.ToInt32(reader.GetValue(1)), min_selections = Convert.ToInt32(reader.GetValue(2)), min_odd_per_selections = reader.GetDouble(3), min_total_odd = reader.GetDouble(4), bonus_user_id = Convert.ToString(reader.GetValue(5)), msisdn = Convert.ToString(reader.GetValue(6)), service_id = Convert.ToString(reader.GetValue(7)) });
            //        }
            //    }


            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            //}
            //finally
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }
            //    if (connection != null)
            //    {
            //        connection.Close();
            //    }

            //}
            return result;
        }

        public static List<USSDMainCode> GetUSSDMainCode(ref List<LogLines> lines)
        {
            List<USSDMainCode> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from ussd_main_codes umc;";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<USSDMainCode>();
                    while (reader.Read())
                    {
                        result.Add(new USSDMainCode { ussd_id = Convert.ToInt32(reader.GetValue(0)), SPID = Convert.ToString(reader.GetValue(1)), service_code = Convert.ToString(reader.GetValue(2)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<USSDMenu> GetUSSDMenus(ref List<LogLines> lines)
        {
            List<USSDMenu> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                // - FEB 2025: change ussd messages to escape the \n to \\n
                // command.CommandText = "select um.*, ua.action_name, tc.topic_name, if(ul.menu_2_display IS NULL,'',ul.menu_2_display), if(ul.lang IS NULL,'',ul.lang),tc.is_maintenance, tc.allowed_msisdns, tc.maintenance_msg from ussd_menu um LEFT JOIN ussd_menu_lang ul ON (ul.menu_id = um.menu_id), ussd_actions ua, ussd_menu_topics tc where tc.ussd_menu_topic_id = um.ussd_menu_topic_id and ua.action_id = um.action_id";
                command.CommandText = "select um.menu_id,um.ussd_id,um.ussd_string,um.menu_2_display, um.action_id,um.prev_action_id,um.service_id,um.ussd_menu_topic_id" +
                                      " ,ua.action_name, tc.topic_name, if (ul.menu_2_display IS NULL,'',ul.menu_2_display), if (ul.lang IS NULL,'',ul.lang),tc.is_maintenance, tc.allowed_msisdns, tc.maintenance_msg" +
                                      " from ussd_menu um LEFT JOIN ussd_menu_lang ul ON(ul.menu_id = um.menu_id), ussd_actions ua, ussd_menu_topics tc" +
                                      " where tc.ussd_menu_topic_id = um.ussd_menu_topic_id and ua.action_id = um.action_id" +
                                      ";"
                                      ;

                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<USSDMenu>();
                    while (reader.Read())
                    {
                        result.Add(
                            new USSDMenu { 
                                 menu_id = Convert.ToInt32(reader.GetValue(0))
                                ,ussd_id = Convert.ToInt32(reader.GetValue(1))
                                ,ussd_string = Convert.ToString(reader.GetValue(2))
                                ,menu_2_display = Convert.ToString(reader.GetValue(3))
                                ,action_id = Convert.ToInt32(reader.GetValue(4))
                                ,prev_action_id = Convert.ToInt32(reader.GetValue(5))
                                ,service_id = Convert.ToInt32(reader.GetValue(6))
                                ,action_name = Convert.ToString(reader.GetValue(8))
                                ,topic_id = Convert.ToInt32(reader.GetValue(7))
                                ,topic_name = Convert.ToString(reader.GetValue(9))
                                ,is_special = false
                                ,menu_2_display_ln = Convert.ToString(reader.GetValue(10))
                                ,is_maintenance = Convert.ToBoolean(reader.GetValue(12))
                                ,allowed_msisdns = Convert.ToString(reader.GetValue(13))
                                ,maintenance_msg = Convert.ToString(reader.GetValue(14)) 
                            }
                        );
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<SMSTexts> GetSMSTexts(ref List<LogLines> lines)
        {
            List<SMSTexts> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from service_text";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<SMSTexts>();
                    while (reader.Read())
                    {
                        result.Add(new SMSTexts { service_id = Convert.ToInt32(reader.GetValue(0)), welcome_sms = Convert.ToString(reader.GetValue(1)), deactivation_sms = Convert.ToString(reader.GetValue(2)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<SavedGames> GetiDoBetSavedGames(string user_session_id, ref List<LogLines> lines)
        {

            List<SavedGames> result = null;

            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "select us.selected_odd, us.msisdn, us.amount, us.game_id, us.selected_bet_type_id, us.selected_odd_name, us.selected_odd_line, us.status, us.time_out_guid, us.event_id, us.selected_ussd_string from ussd_saved_games us where us.user_session_id = '" + user_session_id + "' GROUP BY us.game_id";
                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);

                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<SavedGames>();

                    while (reader.Read())
                    {
                        int status = Convert.ToInt32(reader.GetValue(7));
                        if (status != 3)
                        {
                            result.Add(new SavedGames { selected_odd = reader.GetDouble(0), msisdn = Convert.ToString(reader.GetValue(1)), amount = Convert.ToInt32(reader.GetValue(2)), game_id = Convert.ToString(reader.GetValue(3)), selected_bet_type_id = Convert.ToInt32(reader.GetValue(4)), selected_odd_name = Convert.ToString(reader.GetValue(5)), selected_odd_line = Convert.ToString(reader.GetValue(6)), time_out_guid = Convert.ToString(reader.GetValue(8)), event_id = Convert.ToString(reader.GetValue(9)), selected_ussd_string = Convert.ToString(reader.GetValue(10)) });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<SavedGames> GetiDoBetSavedGames(string user_session_id, string prefix, ref List<LogLines> lines)
        {

            List<SavedGames> result = null;

            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "select us.selected_odd, us.msisdn, us.amount, us.game_id, us.selected_bet_type_id, us.selected_odd_name, us.selected_odd_line, us.status, us.time_out_guid, us.event_id, us.selected_ussd_string from ussd_saved_games"+prefix+" us where us.user_session_id = '" + user_session_id + "' GROUP BY us.game_id";
                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);

                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_104", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<SavedGames>();

                    while (reader.Read())
                    {
                        int status = Convert.ToInt32(reader.GetValue(7));
                        if (status != 3)
                        {
                            result.Add(new SavedGames { selected_odd = reader.GetDouble(0), msisdn = Convert.ToString(reader.GetValue(1)), amount = Convert.ToInt32(reader.GetValue(2)), game_id = Convert.ToString(reader.GetValue(3)), selected_bet_type_id = Convert.ToInt32(reader.GetValue(4)), selected_odd_name = Convert.ToString(reader.GetValue(5)), selected_odd_line = Convert.ToString(reader.GetValue(6)), time_out_guid = Convert.ToString(reader.GetValue(8)), event_id = Convert.ToString(reader.GetValue(9)), selected_ussd_string = Convert.ToString(reader.GetValue(10)) });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<Api.CommonFuncations.B2Tech142.SavedGames> GetiDoBetSavedGames142(string user_session_id, string prefix, ref List<LogLines> lines)
        {

            List<Api.CommonFuncations.B2Tech142.SavedGames> result = null;

            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "select us.selected_odd, us.msisdn, us.amount, us.game_id, us.selected_bet_type_id, us.selected_odd_name, us.selected_odd_line, us.status, us.time_out_guid, us.event_id, us.selected_ussd_string from ussd_saved_games" + prefix + " us where us.user_session_id = '" + user_session_id + "' GROUP BY us.game_id";
                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);

                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_104", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<Api.CommonFuncations.B2Tech142.SavedGames>();

                    while (reader.Read())
                    {
                        int status = Convert.ToInt32(reader.GetValue(7));
                        if (status != 3)
                        {
                            result.Add(new Api.CommonFuncations.B2Tech142.SavedGames { selected_odd = reader.GetDouble(0), msisdn = Convert.ToString(reader.GetValue(1)), amount = Convert.ToInt32(reader.GetValue(2)), game_id = Convert.ToString(reader.GetValue(3)), selected_bet_type_id = Convert.ToInt32(reader.GetValue(4)), selected_odd_name = Convert.ToString(reader.GetValue(5)), selected_odd_line = Convert.ToString(reader.GetValue(6)), time_out_guid = Convert.ToString(reader.GetValue(8)), event_id = Convert.ToString(reader.GetValue(9)), selected_ussd_string = Convert.ToString(reader.GetValue(10)) });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<SavedGames> GetiDoBetSavedGamesTest(ref List<LogLines> lines)
        {

            List<SavedGames> result = null;

            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                string mysql = "SELECT * FROM (SELECT us.selected_odd, us.msisdn, us.amount, us.game_id, us.selected_bet_type_id, us.selected_odd_name, us.selected_odd_line, us.status, us.time_out_guid FROM ussd_saved_games us WHERE us.barcode <> 0 AND us.selected_bet_type_id <> 320320 ORDER BY us.id DESC LIMIT 100) t GROUP BY t.game_id order by rand()";

                lines = Add2Log(lines, mysql, 100, lines[0].ControlerName);

                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = mysql;

                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<SavedGames>();

                    while (reader.Read())
                    {
                        int status = Convert.ToInt32(reader.GetValue(7));
                        if (status != 3)
                        {
                            result.Add(new SavedGames { selected_odd = reader.GetDouble(0), msisdn = Convert.ToString(reader.GetValue(1)), amount = Convert.ToInt32(reader.GetValue(2)), game_id = Convert.ToString(reader.GetValue(3)), selected_bet_type_id = Convert.ToInt32(reader.GetValue(4)), selected_odd_name = Convert.ToString(reader.GetValue(5)), selected_odd_line = Convert.ToString(reader.GetValue(6)), time_out_guid = Convert.ToString(reader.GetValue(8)) });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static USSDSession GetLastUSSDSession(string session_id, ref List<LogLines> lines)
        {
            USSDSession result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString_104", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                string mysql_q = "select date_time, status, page_number, odd_page, game_id, action_id, selected_ussdstring, session_id, topic_id, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, selected_league_id, amount_2_pay, bar_code, selected_subagent_name, event_id, rapidos_string from ussd_sessions us where us.user_session_id = '" + session_id + "' order by session_id desc limit 1;";
                lines = Add2Log(lines, mysql_q, 100, "ussd_mo");
                command.CommandText = mysql_q;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        DateTime date_time = reader.GetDateTime(0);
                        int status_id = Convert.ToInt32(reader.GetValue(1));
                        lines = Add2Log(lines, "date_time = " + date_time + ", status_id = " + status_id + " 2 mintues ago = " + DateTime.Now.AddMinutes(-2), 100, "ussd_mo");
                        if (status_id == 0 && date_time > DateTime.Now.AddMinutes(-2))
                        {
                            lines = Add2Log(lines, "Last Session was found", 100, lines[0].ControlerName);
                            result = new USSDSession()
                            {
                                session_id = Convert.ToInt64(reader.GetValue(7)),
                                ussd_string = Convert.ToString(reader.GetValue(6)),
                                action_id = Convert.ToInt32(reader.GetValue(5)),
                                game_id = Convert.ToInt64(reader.GetValue(4)),
                                odd_page = Convert.ToInt32(reader.GetValue(3)),
                                page_number = Convert.ToInt32(reader.GetValue(2)),
                                topic_id = Convert.ToInt32(reader.GetValue(8)),
                                user_seesion_id = Convert.ToString(reader.GetValue(9)),
                                selected_odd = reader.GetDouble(10),
                                selected_bet_type_id = Convert.ToInt32(reader.GetValue(11)),
                                selected_odd_name = Convert.ToString(reader.GetValue(12)),
                                selected_odd_line = Convert.ToString(reader.GetValue(13)),
                                amount = Convert.ToString(reader.GetValue(14)),
                                selected_league_id = Convert.ToInt32(reader.GetValue(15)), 
                                amount_2_pay = Convert.ToInt32(reader.GetValue(16)),
                                bar_code = Convert.ToString(reader.GetValue(17)),
                                selected_subagent_name = Convert.ToString(reader.GetValue(18)),
                                event_id = Convert.ToInt64(reader.GetValue(19)),
                                rapidos_string = Convert.ToString(reader.GetValue(20))
                            };
                        }
                        else
                        {
                            lines = Add2Log(lines, "Last Session was not found", 100, lines[0].ControlerName);
                        }
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public class USSDUserBankDetails
        {
            public int bank_id { get; set; }
            public string bank_account { get; set; }
        }

        public static USSDUserBankDetails GetUssdUserBankDetails(string msisdn, ref List<LogLines> lines)
        {
            USSDUserBankDetails result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));

                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                string mysql_q = "select bank_id, bank_account from ussd_users_bank_details u where u.msisdn = " + msisdn;
                lines = Add2Log(lines, mysql_q, 100, "ussd_mo");
                command.CommandText = mysql_q;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new USSDUserBankDetails();
                    while (reader.Read())
                    {
                        result.bank_id = Convert.ToInt32(reader.GetValue(0));
                        result.bank_account = Convert.ToString(reader.GetValue(1));
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return result;
        }

        public static USSDSession GetLastUSSDSession(string msisdn, int ussd_code_id, ref List<LogLines> lines)
        {
            USSDSession result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(
                    // Ivory Coast is stored on 161 server
                    ServerSettings.GetServerSettings((ussd_code_id == 16 || ussd_code_id == 21) ? "DBConnectionString_161" : "DBConnectionString_104", ref lines)
                );
 
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                string mysql_q = "select date_time, status, page_number, odd_page, game_id, action_id, selected_ussdstring, session_id, topic_id, user_session_id, selected_odd, selected_bet_type_id, selected_odd_name, selected_odd_line, amount, selected_league_id, amount_2_pay, bar_code, selected_subagent_name, event_id, rapidos_string from ussd_sessions us where us.msisdn = " + msisdn + " and ussd_id = " + ussd_code_id + " AND us.date_time >= DATE_ADD(NOW(), INTERVAL -2 minute) order by session_id desc limit 1;";
                lines = Add2Log(lines, mysql_q, 100, "ussd_mo");
                command.CommandText = mysql_q;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        DateTime date_time = reader.GetDateTime(0);
                        
                        int status_id = Convert.ToInt32(reader.GetValue(1));
                        lines = Add2Log(lines, "date_time = " + date_time + ", status_id = " + status_id + " 2 mintues ago = " + DateTime.Now.AddMinutes(-2), 100, "ussd_mo");
                        if (status_id == 0 /*&& date_time > DateTime.Now.AddMinutes(-2)*/)
                        {
                            lines = Add2Log(lines, "Last Session was found", 100, lines[0].ControlerName);
                            result = new USSDSession()
                            {
                                session_id = Convert.ToInt64(reader.GetValue(7)),
                                ussd_string = Convert.ToString(reader.GetValue(6)),
                                action_id = Convert.ToInt32(reader.GetValue(5)),
                                game_id = Convert.ToInt64(reader.GetValue(4)),
                                odd_page = Convert.ToInt32(reader.GetValue(3)),
                                page_number = Convert.ToInt32(reader.GetValue(2)),
                                topic_id = Convert.ToInt32(reader.GetValue(8)),
                                user_seesion_id = Convert.ToString(reader.GetValue(9)),
                                selected_odd = reader.GetDouble(10),
                                selected_bet_type_id = Convert.ToInt32(reader.GetValue(11)),
                                selected_odd_name = Convert.ToString(reader.GetValue(12)),
                                selected_odd_line = Convert.ToString(reader.GetValue(13)),
                                amount = Convert.ToString(reader.GetValue(14)),
                                selected_league_id = Convert.ToInt32(reader.GetValue(15)),
                                amount_2_pay = Convert.ToInt32(reader.GetValue(16)),
                                bar_code = Convert.ToString(reader.GetValue(17)),
                                selected_subagent_name = Convert.ToString(reader.GetValue(18)),
                                event_id = Convert.ToInt64(reader.GetValue(19)),
                                rapidos_string = Convert.ToString(reader.GetValue(20))
                            };
                        }
                        else
                        {
                            lines = Add2Log(lines, "Last Session was not found", 100, lines[0].ControlerName);
                        }
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }
        public static int GetLastUSSDMenu(string msisdn, int ussd_code_id, ref List<LogLines> lines, out int page_number, out int odd_page, out Int64 game_id)
        {
            int menu_id = 0;
            page_number = 0;
            odd_page = 0;
            game_id = 0;

            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                string mysql_q = "select date_time, menu_id, status, page_number, odd_page, game_id from ussd_sessions us where us.msisdn = " + msisdn + " and ussd_id = "+ussd_code_id+" order by session_id desc limit 1;";
                lines = Add2Log(lines, mysql_q, 100, "ussd_mo");
                command.CommandText = mysql_q;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        DateTime date_time = reader.GetDateTime(0);
                        int status_id = Convert.ToInt32(reader.GetValue(2));
                        lines = Add2Log(lines, "date_time = " + date_time + ", status_id = " + status_id, 100, "ussd_mo");
                        if (status_id == 0 && date_time > DateTime.Now.AddMinutes(-2))
                        {
                            menu_id = Convert.ToInt32(reader.GetValue(1));
                            page_number = Convert.ToInt32(reader.GetValue(3));
                            odd_page = Convert.ToInt32(reader.GetValue(4));
                            game_id = Convert.ToInt64(reader.GetValue(5));
                        }
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return menu_id;
        }

        public static List<PriceClass> Get9MobilePrices(ref List<LogLines> lines)
        {
            List<PriceClass> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select p.price_id, p.service_id, p.price, p.curency_code, np.service_code, np.duration from prices p, nine_mobile_pricesinfo np where np.price_id = p.price_id;";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<PriceClass>();
                    while (reader.Read())
                    {
                        result.Add(new PriceClass { price_id = Convert.ToInt32(reader.GetValue(0)), service_id = Convert.ToInt32(reader.GetValue(1)), price = reader.GetDouble(2), curency_code = Convert.ToString(reader.GetValue(3)), ninemobile_service_code = Convert.ToString(reader.GetValue(4)), ninemobile_duration = Convert.ToString(reader.GetValue(5)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }


        public static List<CPToolGrossRevenu> CPToolGetGrossRevenu(string service_id, string start_date, string end_date, ref List<LogLines> lines)
        {
            List<CPToolGrossRevenu> result = new List<CPToolGrossRevenu>();
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                MySql = "select da, service_name, operator_name, country_name, sum(new_sub), sum(price) from (select date(b.billing_date_time) da, s.service_name, o.operator_name, c.country_name,  0 new_sub, sum(p.real_price) price from billing b, services s, prices p, operators o, countries c where b.price_id = p.price_id and date(b.billing_date_time) between '" + start_date + "' and '" + end_date + "' and s.service_id = p.service_id and s.operator_id = o.operator_id and o.country_id = c.country_id and s.service_id in (" + service_id + ") group by date(b.billing_date_time), s.service_name, o.operator_name, c.country_name union all select date(s.subscription_date) da, s1.service_name, o.operator_name, c.country_name, count(s.subscriber_id) new_sub, 0 price from subscribers s, services s1, operators o, countries c where s.service_id = s1.service_id and date(s.subscription_date) between '" + start_date + "' and '" + end_date + "' and s1.operator_id = o.operator_id and o.country_id = c.country_id and s.service_id in (" + service_id + ") group by date(s.subscription_date), s1.service_name, o.operator_name, c.country_name) t group by da, service_name, operator_name, country_name";
                lines = Add2Log(lines, MySql, 100, "");
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        result.Add(new CPToolGrossRevenu { date = reader.GetDateTime(0).ToString("yyyy-MM-dd"), service_name = Convert.ToString(reader.GetValue(1)), operator_name = Convert.ToString(reader.GetValue(2)), country = Convert.ToString(reader.GetValue(3)), new_sub = Convert.ToInt32(reader.GetValue(4)), gross_revenu = Convert.ToInt32(reader.GetValue(5)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static List<CPToolGrossRevForLastMonths> CPToolGetGrossRevForLastMonths(string service_id, ref List<LogLines> lines)
        {
            List<CPToolGrossRevForLastMonths> result = new List<CPToolGrossRevForLastMonths>();
            string MySql = "";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                MySql = "select year(b.billing_date_time),month(b.billing_date_time), sum(p.real_price) from billing b, prices p where b.price_id = p.price_id and p.service_id in ("+ service_id + ") and b.billing_date_time >= concat(year(date_add(now(), interval - 3 month)), '-', month(date_add(now(), interval - 3 month)), '-01') group by year(b.billing_date_time),month(b.billing_date_time)";
                lines = Add2Log(lines, MySql, 100, "");
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        result.Add(new CPToolGrossRevForLastMonths { year = Convert.ToString(reader.GetValue(0)), month = Convert.ToString(reader.GetValue(1)), volume = Convert.ToString(reader.GetValue(2)) });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }
        public static List<CPToolUserDetails> CPToolGetUserDetails(int user_id, ref List<LogLines> lines)
        {
            string MySql = "";
            List<CPToolUserDetails> result = new List<CPToolUserDetails>();
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                MySql = "select s.service_id, s.service_name, u.user_name, u.last_logindate from services s, users u, video_cp_services v where s.service_id = v.service_id and u.user_id = v.user_id and u.user_id = " + user_id;
                lines = Add2Log(lines, MySql, 100, "");
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = MySql;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        result.Add(new CPToolUserDetails { service_id = Convert.ToInt32(reader.GetValue(0)), service_name = Convert.ToString(reader.GetValue(1)), user_name = Convert.ToString(reader.GetValue(2)), last_login = reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss") });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;


        }

        public static int DoLogin(string username, string password, string ip, int tool_id, ref List<LogLines> lines)
        {
            int result = 0;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string MySql = "";

            try
            {
                MySql = "call sp_Login('" + username + "', '" + password + "', '" + ip + "', "+ tool_id + ")";
                lines = Add2Log(lines, MySql, 100, "");
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "sp_Login";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@_user_name", username);
                command.Parameters.AddWithValue("@_password", password);
                command.Parameters.AddWithValue("@_ip", ip);
                command.Parameters.AddWithValue("@_tool_id", tool_id);

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt32(reader.GetValue(0));
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }
        public static Int64 InsertUpdateMerchentSub(string msisdn, string real_service_id, string real_product_id, string subscription_method_id, bool was_billed, bool to_deactivate, string keyword, ref List<LogLines> lines)
        {
            Int64 sub_id = 0;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;


            try
            {
                lines = Add2Log(lines, " call sp_InsertUpdateSub(" + msisdn + "," + real_service_id + "," + real_product_id + "," + subscription_method_id + "," + was_billed + "," + to_deactivate + ");", 100, "");
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "sp_InsertUpdateSub";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@_msisdn", msisdn);
                command.Parameters.AddWithValue("@_real_service_id", real_service_id);
                command.Parameters.AddWithValue("@_real_product_id", real_product_id);
                command.Parameters.AddWithValue("@_subscription_method_id", subscription_method_id);
                command.Parameters.AddWithValue("@_was_billed", was_billed);
                command.Parameters.AddWithValue("@_to_deactivate", to_deactivate);
                command.Parameters.AddWithValue("@_keyword", keyword);

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sub_id = Convert.ToInt64(reader.GetValue(0));
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, " ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, "");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return sub_id;
        }

        public static List<CBPerDay> GetDataByServiceID(int service_id, int revshare, double usdvalue)
        {

            List<CBPerDay> result = new List<CBPerDay>();
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                List<LogLines> lines = new List<LogLines>();
                lines = Add2Log(lines, "*****************************", 100, "GetDataByServiceID");
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select * from vw_cbperday v where v.service_id = " + service_id + " order by v.da";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {

                    while (reader.Read())
                    {
                        result.Add(new CBPerDay { Day = reader.GetDateTime(0).ToString("yyyy-MM-dd"), ServiceID = Convert.ToInt32(reader.GetValue(1)), ServiceName = Convert.ToString(reader.GetValue(2)), TotalSubs = Convert.ToInt32(reader.GetValue(3)), TotalDeactivation = Convert.ToInt32(reader.GetValue(4)), Churns = Convert.ToInt32(reader.GetValue(5)), BilledUsers = Convert.ToInt32(reader.GetValue(7)), UserSpentLocalCurency = reader.GetDouble(8), CBGrowth = Convert.ToInt32(reader.GetValue(3)) - Convert.ToInt32(reader.GetValue(4)), YDRevenuLocalCurency = Convert.ToInt32(reader.GetValue(8)) * revshare / 100, YDRevenuLocalUSD = (Convert.ToInt32(reader.GetValue(8)) * revshare / 100) / usdvalue });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;


        }

        public static void InsertLog(List<LogLines> lines)
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                string MyGuid = Guid.NewGuid().ToString();
                foreach (LogLines line in lines)
                {
                    string query = "insert into controler_logs (date_time, data, controler_name, log_level, guid) values('" + line.CurrentTime + "','" + line.Data.Replace("'", "''") + "','" + lines[0].ControlerName + "'," + line.LogLevel + ",'" + MyGuid + "')";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                
                
            }
            catch (MySqlException ex)
            {
                
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        public static DLValidateGetMSISDN ValidateGetMSISDN(GetMSISDNRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateGetMSISDN result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_id, s1.service_name, o.operator_name, c.country_name, o.header_name, sc.token, sc.token_experation, o.operator_id from services s1, operators o, service_configuration sc, countries c where sc.service_id = s1.service_id and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = " + RequestBody.ServiceID;
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                result = new DLValidateGetMSISDN()
                {
                    OperatorID = 0,
                    OperatorName = "",
                    HeaderName = "0",
                    Description = "Data was not found",
                    RetCode = 5000
                };
                while (reader.Read())
                {
                    if ((Convert.ToString(reader.GetValue(5)) != RequestBody.TokenID) || (Convert.ToString(reader.GetValue(6)) == "0000-00-00 00:00:00"))
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(6)) != "0000-00-00 00:00:00")
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(6)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        if (Convert.ToString(reader.GetValue(4)) == "0")
                        {
                            ret_code = 1050;
                            description = "MSISDN header enrichment is not supported for this operator.";
                        }
                        else
                        {
                            ret_code = 1000;
                            description = "Request was executed successfully.";
                        }
                        result = new DLValidateGetMSISDN()
                        {
                            OperatorID = Convert.ToInt32(reader.GetValue(7)),
                            OperatorName = Convert.ToString(reader.GetValue(2)),
                            HeaderName = Convert.ToString(reader.GetValue(4)),
                            RetCode = ret_code,
                            Description = description
                        };
                    }
                    else
                    {
                        result = new DLValidateGetMSISDN()
                        {
                            OperatorID = 0,
                            OperatorName = "",
                            HeaderName = "0",
                            Description = description,
                            RetCode = ret_code
                        };
                    }
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateGetMSISDN()
                {
                    OperatorID = 0,
                    OperatorName = "",
                    HeaderName = "0",
                    Description = "Internal Error",
                    RetCode = 5001
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static CheckUserStateResponse CheckUserState(CheckUserStateRequest RequestBody, ref List<LogLines> lines)
        {
            CheckUserStateResponse result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string my_serviceid = RequestBody.ServiceID.ToString();
                switch (RequestBody.ServiceID)
                {
                    case 914:
                        my_serviceid = "914,946,947,948,949";
                        break;
                }
                List<Int64> sub_ids = SelectQueryReturnListInt64("SELECT s.subscriber_id FROM subscribers s WHERE s.msisdn = "+ RequestBody.MSISDN + " AND s.service_id in ("+ my_serviceid + ")", "DBConnectionString", ref lines);
                Int64 sub_id = 0;
                if (sub_ids != null)
                {
                    int max_len = sub_ids.Count();
                    sub_id = sub_ids[max_len - 1];
                }
                if (sub_id > 0)
                {
                    string query = "select s.msisdn, s.service_id, s.subscription_date, s.deactivation_date, s.state_id, max(b.billing_date_time) , sc.token, sc.token_experation, s.subscriber_id from service_configuration sc, subscribers s left join billing b on (b.subscriber_id = s.subscriber_id) where sc.service_id = s.service_id and s.subscriber_id = " + sub_id + " order by s.state_id asc, s.subscriber_id desc limit 1";
                    lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                    connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();

                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    reader = command.ExecuteReader();
                    int ret_code = 5000;
                    string description = "User was not found";
                    result = new CheckUserStateResponse()
                    {
                        MSISDN = 0,
                        ServiceID = 0,
                        SubscriptionDate = "",
                        DeactivationDate = "",
                        State = "",
                        LastBillingDate = "",
                        ResultCode = ret_code,
                        Description = description,
                        SubscriberID = 0
                    };
                    while (reader.Read())
                    {
                        if ((Convert.ToString(reader.GetValue(6)) != RequestBody.TokenID) || (Convert.ToString(reader.GetValue(7)) == "0000-00-00 00:00:00"))
                        {
                            ret_code = 2001;
                            description = "Invalid Token";
                        }
                        if (Convert.ToString(reader.GetValue(7)) != "0000-00-00 00:00:00")
                        {
                            DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(7)));
                            if (DateTime.Now > token_exp)
                            {
                                ret_code = 2002;
                                description = "Token Expired";
                            }
                        }
                        if (ret_code == 5000)
                        {
                            result = new CheckUserStateResponse()
                            {
                                MSISDN = Convert.ToInt64(reader.GetValue(0)),
                                ServiceID = Convert.ToInt32(reader.GetValue(1)),
                                SubscriptionDate = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss"),
                                DeactivationDate = (reader.IsDBNull(3) ? "0" : reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss")),
                                State = (Convert.ToInt32(reader.GetValue(4)) == 1 ? "Active" : "Deactivated"),
                                LastBillingDate = (reader.IsDBNull(5) ? "0" : reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm:ss")),
                                ResultCode = 1000,
                                Description = "Request was executed successfully",
                                SubscriberID = Convert.ToInt64(reader.GetValue(8))
                            };
                        }
                        else
                        {
                            result = new CheckUserStateResponse()
                            {
                                MSISDN = 0,
                                ServiceID = 0,
                                SubscriptionDate = "",
                                DeactivationDate = "",
                                State = "",
                                LastBillingDate = "",
                                ResultCode = ret_code,
                                Description = description,
                                SubscriberID = 0
                            };
                        }
                    }
                }
                else
                {
                    int ret_code = 5000;
                    string description = "User was not found";
                    result = new CheckUserStateResponse()
                    {
                        MSISDN = 0,
                        ServiceID = 0,
                        SubscriptionDate = "",
                        DeactivationDate = "",
                        State = "",
                        LastBillingDate = "",
                        ResultCode = ret_code,
                        Description = description,
                        SubscriberID = 0
                    };
                }
                

            }
            catch (MySqlException ex)
            {
                result = new CheckUserStateResponse()
                {
                    MSISDN = 0,
                    ServiceID = 0,
                    SubscriptionDate = "",
                    DeactivationDate = "",
                    State = "",
                    LastBillingDate = "",
                    ResultCode = 5001,
                    Description = "Internal Error",
                    SubscriberID = 0
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }


        public static DLValidateSMS ValidateSMSRequest1(SendSMSRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging from service_configuration sc, services s1, operators o, countries c where s1.service_id = " + RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id limit 1";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    
                    if (Convert.ToString(reader.GetValue(8)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(9)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(9)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLValidateSMS()
                    {
                        SubscriberID = 0,
                        MSISDN = RequestBody.MSISDN,
                        ServiceName = Convert.ToString(reader.GetValue(0)),
                        OperatorName = Convert.ToString(reader.GetValue(1)),
                        CountryName = Convert.ToString(reader.GetValue(2)),
                        StateID = 1,
                        SPID = Convert.ToInt64(reader.GetValue(3)),
                        Password = Convert.ToString(reader.GetValue(4)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(5)),
                        RealProductID = Convert.ToInt64(reader.GetValue(6)),
                        SMSMTCode = Convert.ToInt64(reader.GetValue(7)),
                        Token = Convert.ToString(reader.GetValue(8)),
                        TokenExperation = Convert.ToString(reader.GetValue(9)),
                        RetCode = ret_code,
                        Description = description,
                        operator_id = Convert.ToInt32(reader.GetValue(10)),
                        is_staging = Convert.ToBoolean(reader.GetValue(11))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateSMS ValidateSMSRequestLight(SendSMSRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                //string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging from service_configuration sc, services s1, operators o, countries c where s1.service_id = " + RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id limit 1";
                string query = "SELECT '', '', '', sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging FROM service_configuration sc WHERE sc.service_id = " + RequestBody.ServiceID;
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {

                    if (Convert.ToString(reader.GetValue(8)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(9)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(9)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLValidateSMS()
                    {
                        SubscriberID = 0,
                        MSISDN = RequestBody.MSISDN,
                        ServiceName = Convert.ToString(reader.GetValue(0)),
                        OperatorName = Convert.ToString(reader.GetValue(1)),
                        CountryName = Convert.ToString(reader.GetValue(2)),
                        StateID = 1,
                        SPID = Convert.ToInt64(reader.GetValue(3)),
                        Password = Convert.ToString(reader.GetValue(4)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(5)),
                        RealProductID = Convert.ToInt64(reader.GetValue(6)),
                        SMSMTCode = Convert.ToInt64(reader.GetValue(7)),
                        Token = Convert.ToString(reader.GetValue(8)),
                        TokenExperation = Convert.ToString(reader.GetValue(9)),
                        RetCode = ret_code,
                        Description = description,
                        operator_id = Convert.ToInt32(reader.GetValue(10)),
                        is_staging = Convert.ToBoolean(reader.GetValue(11))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static List<DLValidateSMS> ValidateSMSRequestList(ref List<LogLines> lines)
        {
            List<DLValidateSMS> result = null;
            DLValidateSMS res = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging, sc.service_id from service_configuration sc, services s1, operators o, countries c where c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id";
                //string query = "SELECT '', '', '', sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging FROM service_configuration sc WHERE sc.service_id = " + RequestBody.ServiceID;
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                if (reader.HasRows == true)
                {
                    result = new List<DLValidateSMS>();
                    while (reader.Read())
                    {
                        res = new DLValidateSMS()
                        {
                            SubscriberID = 0,
                            MSISDN = 0,
                            ServiceName = Convert.ToString(reader.GetValue(0)),
                            OperatorName = Convert.ToString(reader.GetValue(1)),
                            CountryName = Convert.ToString(reader.GetValue(2)),
                            StateID = 1,
                            SPID = Convert.ToInt64(reader.GetValue(3)),
                            Password = Convert.ToString(reader.GetValue(4)),
                            RealServiceID = Convert.ToInt64(reader.GetValue(5)),
                            RealProductID = Convert.ToInt64(reader.GetValue(6)),
                            SMSMTCode = Convert.ToInt64(reader.GetValue(7)),
                            Token = Convert.ToString(reader.GetValue(8)),
                            TokenExperation = Convert.ToString(reader.GetValue(9)),
                            RetCode = ret_code,
                            Description = description,
                            operator_id = Convert.ToInt32(reader.GetValue(10)),
                            is_staging = Convert.ToBoolean(reader.GetValue(11)),
                            service_id = Convert.ToInt32(reader.GetValue(12))
                        };
                        result.Add(res);
                    }
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateSMS ValidateSMSRequest(SendSMSRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s.subscriber_id, s.msisdn, s1.service_name, o.operator_name, c.country_name, s.state_id, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation), sc.operator_id, sc.is_staging from subscribers s, service_configuration sc, services s1, operators o, countries c where s.msisdn = " + RequestBody.MSISDN + " and s.service_id = "+ RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and s1.service_id = s.service_id and sc.service_id = s1.service_id order by s.subscriber_id desc limit 1";
                lines = Add2Log(lines, query , 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader.GetValue(5)) == 2)
                    {
                        ret_code = 3000;
                        description = "User is already deactivated";
                    }
                    if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(12)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }
                    
                    result = new DLValidateSMS()
                    {
                        SubscriberID = Convert.ToInt64(reader.GetValue(0)),
                        MSISDN = Convert.ToInt64(reader.GetValue(1)),
                        ServiceName = Convert.ToString(reader.GetValue(2)),
                        OperatorName = Convert.ToString(reader.GetValue(3)),
                        CountryName = Convert.ToString(reader.GetValue(4)),
                        StateID = Convert.ToInt32(reader.GetValue(5)),
                        SPID = Convert.ToInt64(reader.GetValue(6)),
                        Password = Convert.ToString(reader.GetValue(7)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                        RealProductID = Convert.ToInt64(reader.GetValue(9)),
                        SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                        Token = Convert.ToString(reader.GetValue(11)),
                        TokenExperation = Convert.ToString(reader.GetValue(12)),
                        RetCode = ret_code,
                        Description = description,
                        operator_id = Convert.ToInt32(reader.GetValue(13)),
                        is_staging = Convert.ToBoolean(reader.GetValue(14))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code , 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLDYAValidateAccount ValidateDYARequest(DYACheckAccountRequest RequestBody, ref List<LogLines> lines)
        {
            DLDYAValidateAccount result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.real_service_id,  sc.token, if(sc.token_experation is null,0,sc.token_experation), sc.allow_dya, sc.operator_id from service_configuration sc, services s1, operators o, countries c where sc.service_id = " + RequestBody.ServiceID+" and s1.service_id = sc.service_id and o.operator_id = sc.operator_id and c.country_id = o.country_id";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    if (Convert.ToString(reader.GetValue(5)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(6)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(6)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (Convert.ToBoolean(reader.GetValue(7)) == false)
                    {
                        ret_code = 2020;
                        description = "DYA not allowed for this service";
                    }

                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLDYAValidateAccount()
                    {
                        ServiceName = Convert.ToString(reader.GetValue(0)),
                        OperatorName = Convert.ToString(reader.GetValue(1)),
                        CountryName = Convert.ToString(reader.GetValue(2)),
                        SPID = Convert.ToInt64(reader.GetValue(3)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(4)),
                        Token = Convert.ToString(reader.GetValue(5)),
                        TokenExperation = Convert.ToString(reader.GetValue(6)),
                        AllowDYA = Convert.ToBoolean(reader.GetValue(7)),
                        RetCode = ret_code,
                        Description = description,
                        OperatorID = Convert.ToInt32(reader.GetValue(8))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLDYAValidateAccount()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLDYAValidateAccount ValidateDYARequest(DYACheckAccountRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            DLDYAValidateAccount result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.real_service_id,  sc.token, if(sc.token_experation is null,0,sc.token_experation), sc.allow_dya, sc.operator_id from service_configuration sc, services s1, operators o, countries c where sc.service_id = " + RequestBody.ServiceID + " and s1.service_id = sc.service_id and o.operator_id = sc.operator_id and c.country_id = o.country_id";
                logMessages.Add(new { message = query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ValidateDYARequest", logz_id = logz_id });
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    if (Convert.ToString(reader.GetValue(5)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(6)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(6)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (Convert.ToBoolean(reader.GetValue(7)) == false)
                    {
                        ret_code = 2020;
                        description = "DYA not allowed for this service";
                    }

                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLDYAValidateAccount()
                    {
                        ServiceName = Convert.ToString(reader.GetValue(0)),
                        OperatorName = Convert.ToString(reader.GetValue(1)),
                        CountryName = Convert.ToString(reader.GetValue(2)),
                        SPID = Convert.ToInt64(reader.GetValue(3)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(4)),
                        Token = Convert.ToString(reader.GetValue(5)),
                        TokenExperation = Convert.ToString(reader.GetValue(6)),
                        AllowDYA = Convert.ToBoolean(reader.GetValue(7)),
                        RetCode = ret_code,
                        Description = description,
                        OperatorID = Convert.ToInt32(reader.GetValue(8))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                    logMessages.Add(new { message = "Description = " + description + ", RetCode = " + ret_code, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ValidateDYARequest", logz_id = logz_id });
                }

            }
            catch (MySqlException ex)
            {
                result = new DLDYAValidateAccount()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ValidateDYARequest", logz_id = logz_id });
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }


        public static DLDYAValidateAccount ValidateDYARequest(DYACheckTransactionRequest RequestBody, ref List<LogLines> lines)
        {
            DLDYAValidateAccount result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select s1.service_name, o.operator_name, c.country_name, sc.spid, sc.real_service_id,  sc.token, if(sc.token_experation is null,0,sc.token_experation), sc.allow_dya, sc.operator_id from service_configuration sc, services s1, operators o, countries c where sc.service_id = " + RequestBody.ServiceID + " and s1.service_id = sc.service_id and o.operator_id = sc.operator_id and c.country_id = o.country_id";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    if (Convert.ToString(reader.GetValue(5)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(6)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(6)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (Convert.ToBoolean(reader.GetValue(7)) == false)
                    {
                        ret_code = 2020;
                        description = "DYA not allowed for this service";
                    }

                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLDYAValidateAccount()
                    {
                        ServiceName = Convert.ToString(reader.GetValue(0)),
                        OperatorName = Convert.ToString(reader.GetValue(1)),
                        CountryName = Convert.ToString(reader.GetValue(2)),
                        SPID = Convert.ToInt64(reader.GetValue(3)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(4)),
                        Token = Convert.ToString(reader.GetValue(5)),
                        TokenExperation = Convert.ToString(reader.GetValue(6)),
                        AllowDYA = Convert.ToBoolean(reader.GetValue(7)),
                        RetCode = ret_code,
                        Description = description,
                        OperatorID = Convert.ToInt32(reader.GetValue(8))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLDYAValidateAccount()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateSMS ValidateSubscribeRequest(SubscribeRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                Int64 sub_id = 0;
                List<Int64> sub_ids = SelectQueryReturnListInt64("SELECT s.subscriber_id FROM subscribers s WHERE s.msisdn = "+RequestBody.MSISDN+" AND s.service_id = "+RequestBody.ServiceID, "DBConnectionString",  ref lines);
                if (sub_ids != null)
                {
                    int max_len = sub_ids.Count();
                    sub_id = sub_ids[max_len - 1];
                }

                if (sub_id > 0)
                {
                    string query = "select s.subscriber_id, s.msisdn, s1.service_name, o.operator_name, c.country_name, s.state_id, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, sc.token_experation, sc.operator_id, sc.is_staging, sc.subscribe_wo_serviceid from subscribers s, service_configuration sc, services s1, operators o, countries c where s.subscriber_id = "+sub_id+" and c.country_id = o.country_id and o.operator_id = s1.operator_id and sc.service_id = s.service_id and s1.service_id = s.service_id order by s.subscriber_id desc limit 1";
                    lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                    connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();

                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    reader = command.ExecuteReader();
                    int ret_code = 5000;
                    string description = "Data was not found";
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader.GetValue(5)) == 1)
                            {
                                ret_code = 3010;
                                description = "User is already subscribed ";
                            }
                            if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                            {
                                ret_code = 2001;
                                description = "Invalid Token";
                            }
                            if (Convert.ToString(reader.GetValue(12)) == "0")
                            {
                                ret_code = 2002;
                                description = "Token Expired";
                            }
                            else
                            {
                                DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                                if (DateTime.Now > token_exp)
                                {
                                    ret_code = 2002;
                                    description = "Token Expired";
                                }
                            }
                            if (ret_code == 5000)
                            {
                                ret_code = 1000;
                                description = "User was Validated";
                            }

                            result = new DLValidateSMS()
                            {
                                SubscriberID = Convert.ToInt64(reader.GetValue(0)),
                                MSISDN = Convert.ToInt64(reader.GetValue(1)),
                                ServiceName = Convert.ToString(reader.GetValue(2)),
                                OperatorName = Convert.ToString(reader.GetValue(3)),
                                CountryName = Convert.ToString(reader.GetValue(4)),
                                StateID = Convert.ToInt32(reader.GetValue(5)),
                                SPID = Convert.ToInt64(reader.GetValue(6)),
                                Password = Convert.ToString(reader.GetValue(7)),
                                RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                                RealProductID = Convert.ToInt64(reader.GetValue(9)),
                                SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                                Token = Convert.ToString(reader.GetValue(11)),
                                TokenExperation = Convert.ToString(reader.GetValue(12)),
                                RetCode = ret_code,
                                Description = description,
                                operator_id = Convert.ToInt32(reader.GetValue(13)),
                                is_staging = Convert.ToBoolean(reader.GetValue(14)),
                                subscribe_wo_service_id = Convert.ToBoolean(reader.GetValue(15))
                            };
                            lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                        }
                    }
                    else
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                        }
                        query = "select 0, 0, s1.service_name, o.operator_name, c.country_name, 0, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, sc.token_experation, sc.operator_id, sc.is_staging, sc.subscribe_wo_serviceid from service_configuration sc, services s1, operators o, countries c where c.country_id = o.country_id and sc.service_id = s1.service_id and sc.service_id = " + RequestBody.ServiceID + " and o.operator_id = s1.operator_id group by s1.service_id";
                        lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                        connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                        connection.Open();

                        command = connection.CreateCommand();
                        command.CommandText = query;
                        reader = command.ExecuteReader();
                        ret_code = 5000;
                        description = "Data was not found";
                        if (reader.HasRows == true)
                        {
                            while (reader.Read())
                            {
                                if (Convert.ToInt32(reader.GetValue(5)) == 1)
                                {
                                    ret_code = 3010;
                                    description = "User is already subscribed ";
                                }
                                if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                                {
                                    ret_code = 2001;
                                    description = "Invalid Token";
                                }
                                if (Convert.ToString(reader.GetValue(12)) == "0")
                                {
                                    ret_code = 2002;
                                    description = "Token Expired";
                                }
                                else
                                {
                                    DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                                    if (DateTime.Now > token_exp)
                                    {
                                        ret_code = 2002;
                                        description = "Token Expired";
                                    }
                                }
                                if (ret_code == 5000)
                                {
                                    ret_code = 1000;
                                    description = "User was Validated";
                                }

                                result = new DLValidateSMS()
                                {
                                    SubscriberID = Convert.ToInt64(reader.GetValue(0)),
                                    MSISDN = Convert.ToInt64(reader.GetValue(1)),
                                    ServiceName = Convert.ToString(reader.GetValue(2)),
                                    OperatorName = Convert.ToString(reader.GetValue(3)),
                                    CountryName = Convert.ToString(reader.GetValue(4)),
                                    StateID = Convert.ToInt32(reader.GetValue(5)),
                                    SPID = Convert.ToInt64(reader.GetValue(6)),
                                    Password = Convert.ToString(reader.GetValue(7)),
                                    RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                                    RealProductID = Convert.ToInt64(reader.GetValue(9)),
                                    SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                                    Token = Convert.ToString(reader.GetValue(11)),
                                    TokenExperation = Convert.ToString(reader.GetValue(12)),
                                    RetCode = ret_code,
                                    Description = description,
                                    operator_id = Convert.ToInt32(reader.GetValue(13)),
                                    is_staging = Convert.ToBoolean(reader.GetValue(14)),
                                    subscribe_wo_service_id = Convert.ToBoolean(reader.GetValue(15))
                                };
                                lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                            }
                        }
                    }
                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }
                    string query = "select 0, 0, s1.service_name, o.operator_name, c.country_name, 0, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, sc.token_experation, sc.operator_id, sc.is_staging, sc.subscribe_wo_serviceid from service_configuration sc, services s1, operators o, countries c where c.country_id = o.country_id and sc.service_id = s1.service_id and sc.service_id = " + RequestBody.ServiceID + " and o.operator_id = s1.operator_id group by s1.service_id";
                    lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                    connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();
                    int ret_code = 5000;
                    string description = "Data was not found";

                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    reader = command.ExecuteReader();
                    ret_code = 5000;
                    description = "Data was not found";
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader.GetValue(5)) == 1)
                            {
                                ret_code = 3010;
                                description = "User is already subscribed ";
                            }
                            if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                            {
                                ret_code = 2001;
                                description = "Invalid Token";
                            }
                            if (Convert.ToString(reader.GetValue(12)) == "0")
                            {
                                ret_code = 2002;
                                description = "Token Expired";
                            }
                            else
                            {
                                DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                                if (DateTime.Now > token_exp)
                                {
                                    ret_code = 2002;
                                    description = "Token Expired";
                                }
                            }
                            if (ret_code == 5000)
                            {
                                ret_code = 1000;
                                description = "User was Validated";
                            }

                            result = new DLValidateSMS()
                            {
                                SubscriberID = Convert.ToInt64(reader.GetValue(0)),
                                MSISDN = Convert.ToInt64(reader.GetValue(1)),
                                ServiceName = Convert.ToString(reader.GetValue(2)),
                                OperatorName = Convert.ToString(reader.GetValue(3)),
                                CountryName = Convert.ToString(reader.GetValue(4)),
                                StateID = Convert.ToInt32(reader.GetValue(5)),
                                SPID = Convert.ToInt64(reader.GetValue(6)),
                                Password = Convert.ToString(reader.GetValue(7)),
                                RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                                RealProductID = Convert.ToInt64(reader.GetValue(9)),
                                SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                                Token = Convert.ToString(reader.GetValue(11)),
                                TokenExperation = Convert.ToString(reader.GetValue(12)),
                                RetCode = ret_code,
                                Description = description,
                                operator_id = Convert.ToInt32(reader.GetValue(13)),
                                is_staging = Convert.ToBoolean(reader.GetValue(14)),
                                subscribe_wo_service_id = Convert.ToBoolean(reader.GetValue(15))
                            };
                            lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                        }
                    }

                    lines = Add2Log(lines, $"validate request for msisdn={RequestBody.MSISDN}, serviceID={RequestBody.ServiceID}: retcode={ret_code}, description={description}", 100, "");
                }
                
                

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateBill ValidateBill(BillRequest requestBody, ref List<LogLines> lines)
        {
            int retCode = 5000;
            string description = "Data was not found";
            bool isStaging = false;
            int operatorId = 0;
            long subscriberId = 0;

            try
            {
                string query = $"SELECT sc.token, sc.token_experation, sc.is_staging, sc.operator_id "
                             + $"FROM service_configuration sc "
                             + $"JOIN prices p ON p.service_id = sc.service_id "
                             + $"WHERE sc.service_id = {requestBody.ServiceID} "
                             + $"LIMIT 1"
                             ;

                lines = Add2Log(lines, query, 100, lines[0].ControlerName);

                using (var connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines)))
                {

                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            retCode = 2020;
                            description = "Service and Price do not match";
                        }
                        else
                        {
                            string token = Convert.ToString(reader.GetValue(0));
                            string tokenExpirationStr = Convert.ToString(reader.GetValue(1));
                            isStaging = Convert.ToBoolean(reader.GetValue(2));
                            operatorId = Convert.ToInt32(reader.GetValue(3));

                            if (token != requestBody.TokenID)
                            {
                                retCode = 2001;
                                description = "Invalid Token";
                            }
                            else if (tokenExpirationStr == "0" || DateTime.Now > Convert.ToDateTime(tokenExpirationStr))
                            {
                                retCode = 2002;
                                description = "Token Expired";
                            }
                            else
                            {
                                retCode = 1000;
                                description = "User was Validated";

                                // fetch subscriber ID
                                string sql_subscriber = $"SELECT s.subscriber_id "
                                                      + $"FROM subscribers s "
                                                      + $"WHERE s.msisdn = {requestBody.MSISDN} "
                                                      + $"AND s.service_id = {requestBody.ServiceID} "
                                                      + $"AND s.state = 1 "
                                                      + $"ORDER BY s.subscriber_id DESC "
                                                      + $"LIMIT 1"
                                                      ;

                                lines = Add2Log(lines, sql_subscriber, 100, lines[0].ControlerName);
                                subscriberId = DBQueries.SelectQueryReturnInt64(sql_subscriber, ref lines);

                                // if no active subscriber, but have billing, then insert a subscriber entry
                                if (subscriberId == 0)
                                {
                                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    subscriberId = DBQueries.InsertSub(
                                        requestBody.MSISDN.ToString(),
                                        requestBody.ServiceID.ToString(),
                                        0, now, now, "1", " ", ref lines
                                    );
                                }
                            }
                        }
                    } // using
                } // using

                return new DLValidateBill
                {
                    sub_id = subscriberId,
                    ret_code = retCode,
                    description = description,
                    is_staging = isStaging,
                    operator_id = operatorId
                };
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines,
                    $"ErrorCode={ex.ErrorCode}, InnerException={ex.InnerException}, Message={ex.Message}",
                    100, lines[0].ControlerName);
            }

            return new DLValidateBill
            {
                sub_id = 0,
                ret_code = 5001,
                description = "Internal Error"
            };
        } // DLValidateBill

        public static DLValidateBill ValidateBillWithEncMSISDN(BillRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateBill result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            Int64 sub_id = 0;
            int operator_id = 0;
            bool is_staging = false;
            try
            {
                string query = "select sc.token, sc.token_experation, sc.is_staging, sc.operator_id from service_configuration sc, prices p where sc.service_id = " + RequestBody.ServiceID + " and p.service_id = sc.service_id limit 1";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        is_staging = Convert.ToBoolean(reader.GetValue(2));
                        operator_id = Convert.ToInt32(reader.GetValue(3));
                        if (Convert.ToString(reader.GetValue(0)) != RequestBody.TokenID)
                        {
                            ret_code = 2001;
                            description = "Invalid Token";
                        }
                        if (Convert.ToString(reader.GetValue(1)) == "0")
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                        else
                        {
                            DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(1)));
                            if (DateTime.Now > token_exp)
                            {
                                ret_code = 2002;
                                description = "Token Expired";
                            }
                        }
                        if (ret_code == 5000)
                        {
                            ret_code = 1000;
                            description = "User was Validated";
                        }
                    }
                }
                else
                {
                    ret_code = 2020;
                    description = "Service and Price do not match";
                }
                if (ret_code == 1000)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }

                    query = "select s.subscriber_id, s.state_id from subscribers s, subscribers_ocm so where so.subscriber_id = s.subscriber_id and so.encrypted_msisdn = '"+RequestBody.EncryptedMSISDN+"' and s.service_id = " + RequestBody.ServiceID;
                    lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                    connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();

                    command = connection.CreateCommand();
                    command.CommandText = query;
                    reader = command.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            if (Convert.ToInt32(reader.GetValue(1)) == 1)
                            {
                                sub_id = Convert.ToInt64(reader.GetValue(0));
                            }
                        }
                        if (sub_id == 0)
                        {
                            ret_code = 1020;
                            description = "User is already deactivated";
                        }
                    }
                    else
                    {
                        ret_code = 1021;
                        description = "User was not found";
                    }

                }
                result = new DLValidateBill()
                {
                    sub_id = sub_id,
                    ret_code = ret_code,
                    description = description,
                    is_staging = is_staging,
                    operator_id = operator_id
                };

            }
            catch (MySqlException ex)
            {
                result = new DLValidateBill()
                {
                    sub_id = 0,
                    ret_code = 5001,
                    description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }


        public static CLSendUSSD ValidateUSSDPush(SendUSSDPushRequest RequestBody, ref List<LogLines> lines)
        {
            CLSendUSSD result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            Int64 cu_id = 0, msisdn = 0;
            int campaign_id = 0, service_id = 0, operator_id = 0;
            string campaign_msg = "";
            bool is_staging = false;
            try
            {
                string query = "select sc.token, sc.token_experation, sc.is_staging, sc.operator_id from service_configuration sc where sc.service_id = " + RequestBody.ServiceID + " limit 1";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        is_staging = Convert.ToBoolean(reader.GetValue(2));
                        operator_id = Convert.ToInt32(reader.GetValue(3));
                        if (Convert.ToString(reader.GetValue(0)) != RequestBody.TokenID)
                        {
                            ret_code = 2001;
                            description = "Invalid Token";
                        }
                        if (Convert.ToString(reader.GetValue(1)) == "0")
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                        else
                        {
                            DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(1)));
                            if (DateTime.Now > token_exp)
                            {
                                ret_code = 2002;
                                description = "Token Expired";
                            }
                        }
                        if (ret_code == 5000)
                        {
                            ret_code = 1000;
                            description = "User was Validated";
                        }
                    }
                }
                else
                {
                    ret_code = 2020;
                    description = "Service was not found";
                }
                if (ret_code == 1000)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (connection != null)
                    {
                        connection.Close();
                    }

                    
                    query = "select cu.cu_id, cu.msisdn, cu.campaign_id, c.service_id, c.campaign_msg from ussd_push.campaign_users cu, ussd_push.campaign_schedule cs, ussd_push.campaigns c where cu.date_time between cs.start_datetime and cs.end_datetime and cs.campaign_id = cu.campaign_id and c.campaign_id = cs.campaign_id and cu.cu_id = " + RequestBody.CUID;
                    lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                    connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                    connection.Open();

                    command = connection.CreateCommand();
                    command.CommandText = query;
                    reader = command.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        while (reader.Read())
                        {
                            cu_id = Convert.ToInt64(reader.GetValue(0));
                            msisdn = Convert.ToInt64(reader.GetValue(1));
                            campaign_id = Convert.ToInt32(reader.GetValue(2));
                            service_id = Convert.ToInt32(reader.GetValue(3));
                            campaign_msg = Convert.ToString(reader.GetValue(4));

                        }
                    }
                    else
                    {
                        ret_code = 1050;
                        description = "User was not found";
                    }
                }
                result = new CLSendUSSD()
                {
                    cu_id = cu_id,
                    ret_code = ret_code,
                    description = description,
                    is_staging = is_staging,
                    campaign_id = campaign_id,
                    msg = campaign_msg,
                    msisdn = msisdn,
                    service_id = service_id,
                    operator_id = operator_id
                };

            }
            catch (MySqlException ex)
            {
                result = new CLSendUSSD()
                {
                    cu_id = 0,
                    ret_code = 5001,
                    description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }


        public static DLValidateSMS ValidateSubscribeRequestWithEncMSISDN(SubscribeRequest RequestBody, int is_subscribe, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select 0, 0, s1.service_name, o.operator_name, c.country_name, 0, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if (sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation) te, sc.operator_id, sc.is_staging from service_configuration sc, services s1, operators o, countries c where s1.service_id = "+RequestBody.ServiceID+" and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id and sc.use_2_subscribe = 1";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    string subscriber_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT so.subscriber_id FROM subscribers s, subscribers_ocm so WHERE so.subscriber_id = s.subscriber_id AND s.service_id = "+RequestBody.ServiceID+" AND so.encrypted_msisdn = '"+RequestBody.EncryptedMSISDN+"' ORDER BY s.subscriber_id DESC limit 1", ref lines);
                    string msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT so.msisdn FROM subscribers s, subscribers_ocm so WHERE so.subscriber_id = s.subscriber_id AND s.service_id = " + RequestBody.ServiceID + " AND so.encrypted_msisdn = '" + RequestBody.EncryptedMSISDN + "' ORDER BY s.subscriber_id DESC limit 1", ref lines);
                    string state_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT s.state_id FROM subscribers s, subscribers_ocm so WHERE so.subscriber_id = s.subscriber_id AND s.service_id = " + RequestBody.ServiceID + " AND so.encrypted_msisdn = '" + RequestBody.EncryptedMSISDN + "' ORDER BY s.subscriber_id DESC limit 1", ref lines);
                    msisdn = (!String.IsNullOrEmpty(msisdn) ? msisdn : "0");
                    subscriber_id = (!String.IsNullOrEmpty(subscriber_id) ? subscriber_id : "0");
                    state_id = (!String.IsNullOrEmpty(state_id) ? state_id : "0");

                    if (is_subscribe == 2 && state_id == "2")
                    {
                        ret_code = 3020;
                        description = "User is already deactivated ";
                    }
                    if (is_subscribe == 1 && state_id == "1")
                    {
                        ret_code = 3020;
                        description = "User is already subscribed ";
                    }
                    if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(12)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLValidateSMS()
                    {
                        SubscriberID = Convert.ToInt64(subscriber_id),
                        MSISDN = Convert.ToInt64(msisdn),
                        ServiceName = Convert.ToString(reader.GetValue(2)),
                        OperatorName = Convert.ToString(reader.GetValue(3)),
                        CountryName = Convert.ToString(reader.GetValue(4)),
                        StateID = Convert.ToInt32(state_id),
                        SPID = Convert.ToInt64(reader.GetValue(6)),
                        Password = Convert.ToString(reader.GetValue(7)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                        RealProductID = Convert.ToInt64(reader.GetValue(9)),
                        SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                        Token = Convert.ToString(reader.GetValue(11)),
                        TokenExperation = Convert.ToString(reader.GetValue(12)),
                        RetCode = ret_code,
                        Description = description,
                        operator_id = Convert.ToInt32(reader.GetValue(13)),
                        is_staging = Convert.ToBoolean(reader.GetValue(14))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateSMS ValidateSubscribeRequest(SubscribeRequest RequestBody, int is_subscribe, ref List<LogLines> lines)
        {
            DLValidateSMS result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select if(sum(subscriber_id) IS NULL,0,SUM(subscriber_id)), " +
                                "if(sum(msisdn) IS NULL,0,SUM(msisdn)), " +
                                "if(service_name IS NULL,'0',service_name), " +
                                "if(operator_name IS NULL,'0',operator_name), " +
                                "if(country_name IS NULL,'0',country_name), " +
                                "if(sum(state_id) IS NULL,0,SUM(state_id)), " +
                                "if(spid IS NULL,0,spid), " +
                                "if(password IS NULL,'0',password), " +
                                "if(real_service_id IS NULL,0,real_service_id), " +
                                "if(product_id IS NULL,0,product_id), " +
                                "if(sms_mt_code IS NULL,0,sms_mt_code), " +
                                "if(token IS NULL,'0',token), " +
                                "if(te IS NULL,'0',te), " +
                                "if(operator_id IS NULL, 0, operator_id), " +
                                "if(is_staging IS NULL,0,is_staging) " +
                                "from((select s.subscriber_id, s.msisdn, s1.service_name, o.operator_name, c.country_name, s.state_id, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if (sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation) te, sc.operator_id, sc.is_staging from service_configuration sc, services s1 left join subscribers s on(s1.service_id = s.service_id), " +
                                "operators o, countries c where s.msisdn = "+RequestBody.MSISDN+" and s.service_id = "+RequestBody.ServiceID+" and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id and s.state_id = 1 and sc.use_2_subscribe = 1 order by s.subscriber_id desc limit 1) union all(select 0, 0, s1.service_name, o.operator_name, c.country_name, 0, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if(sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation) te, sc.operator_id, sc.is_staging " +
                                "from service_configuration sc, services s1, operators o, countries c where s1.service_id = "+ RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id and sc.use_2_subscribe = 1)) as t;";
                   /*"select sum(subscriber_id), sum(msisdn), service_name, operator_name, country_name, sum(state_id), spid, password, real_service_id, product_id, sms_mt_code, token, te, operator_id, is_staging from ((select s.subscriber_id, s.msisdn, s1.service_name, o.operator_name, c.country_name, s.state_id, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if (sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation) te, sc.operator_id, sc.is_staging from service_configuration sc, services s1 left join subscribers s on(s1.service_id = s.service_id), operators o, countries c where s.msisdn = " + RequestBody.MSISDN + " and s.service_id = " + RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id and s.state_id = 1 and sc.use_2_subscribe = 1 order by s.subscriber_id desc limit 1) union all (select 0, 0, s1.service_name, o.operator_name, c.country_name, 0, sc.spid, sc.password, sc.real_service_id, sc.product_id, sc.sms_mt_code, sc.token, if (sc.token_experation = '0000-00-00 00:00:00',0,sc.token_experation) te, sc.operator_id, sc.is_staging from service_configuration sc, services s1, operators o, countries c where s1.service_id = " + RequestBody.ServiceID + " and c.country_id = o.country_id and o.operator_id = sc.operator_id and sc.service_id = s1.service_id and sc.use_2_subscribe = 1)) as t";*/
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader.GetValue(5)) == 2)
                    {
                        ret_code = 3020;
                        description = "User is already deactivated ";
                    }
                    if (Convert.ToString(reader.GetValue(11)) != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (Convert.ToString(reader.GetValue(12)) == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(12)));
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }
                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result = new DLValidateSMS()
                    {
                        SubscriberID = Convert.ToInt64(reader.GetValue(0)),
                        MSISDN = Convert.ToInt64(reader.GetValue(1)),
                        ServiceName = Convert.ToString(reader.GetValue(2)),
                        OperatorName = Convert.ToString(reader.GetValue(3)),
                        CountryName = Convert.ToString(reader.GetValue(4)),
                        StateID = Convert.ToInt32(reader.GetValue(5)),
                        SPID = Convert.ToInt64(reader.GetValue(6)),
                        Password = Convert.ToString(reader.GetValue(7)),
                        RealServiceID = Convert.ToInt64(reader.GetValue(8)),
                        RealProductID = Convert.ToInt64(reader.GetValue(9)),
                        SMSMTCode = Convert.ToInt64(reader.GetValue(10)),
                        Token = Convert.ToString(reader.GetValue(11)),
                        TokenExperation = Convert.ToString(reader.GetValue(12)),
                        RetCode = ret_code,
                        Description = description,
                        operator_id = Convert.ToInt32(reader.GetValue(13)),
                        is_staging = Convert.ToBoolean(reader.GetValue(14))
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateSMS()
                {
                    RetCode = 5001,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public class SentSMSInfo
        {
            public string msisdn { get; set; }
            public string partner_transaction_id { get; set; }
            public string date_time { get; set; }
        }

        public static SentSMSInfo GetSentSMSInfo(string msg_id, ref List<LogLines> lines)
        {
            SentSMSInfo result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "SELECT s.msisdn, s.partner_transaction_id, NOW() FROM send_sms s WHERE s.id = " + msg_id;
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result = new SentSMSInfo()
                    {
                        msisdn = Convert.ToString(reader.GetValue(0)),
                        partner_transaction_id = Convert.ToString(reader.GetValue(1)),
                        date_time = reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        public static DLValidateLogin ValidateLoginRequest(LoginRequest RequestBody, ref List<LogLines> lines)
        {
            DLValidateLogin result = null;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            try
            {
                string query = "select sc.service_id, sc.service_password, sc.token, sc.token_validity, if(sc.token_experation = '0000-00-00 00:00:00', 0, sc.token_experation) from service_configuration sc where sc.service_id = " + RequestBody.ServiceID +" limit 1";
                lines = Add2Log(lines, query, 100, lines[0].ControlerName);
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                reader = command.ExecuteReader();
                int ret_code = 5000;
                string description = "Data was not found";
                while (reader.Read())
                {
                    int service_id = Convert.ToInt32(reader.GetValue(0));
                    string token = Convert.ToString(reader.GetValue(2));
                    string token_validaty = Convert.ToString(reader.GetValue(3));
                    string token_exper = Convert.ToString(reader.GetValue(4));
                    
                    string user_password = Convert.ToString(reader.GetValue(1));
                    if (user_password != RequestBody.Password)
                    {
                        ret_code = 2003;
                        description = $"Invalid Password";
                        token = "";
                        token_exper = "";
                        lines = Add2Log(lines, $"Invalid Password - got [{RequestBody.Password}] - expected [{user_password}]", 100, "");
                    }
                    if (ret_code == 5000)
                    {
                        if (Convert.ToString(reader.GetValue(4)) == "0")
                        {
                            //need to update token
                            token = Guid.NewGuid().ToString();
                            token_exper = DateTime.Now.AddDays(Convert.ToDouble(token_validaty)).ToString("yyyy-MM-dd HH:mm:ss");
                            string update_q = "update service_configuration set token = '" + token + "', token_experation = '" + token_exper + "' where service_id = " + RequestBody.ServiceID;
                            bool res = ExecuteQuery(update_q, ref lines);
                            if (res == false)
                            {
                                ret_code = 50011;
                                description = "Internal Error";
                                token = "";
                                token_exper = "";
                            }
                        }
                        else
                        {
                            // regenerate a token that is set to expire in the next 15 minutes
                            DateTime token_exp = Convert.ToDateTime(Convert.ToString(reader.GetValue(4)));
                            if ((DateTime.Now > token_exp) || (service_id != 1116 && DateTime.Now > token_exp.AddMinutes(-15)))
                            {
                                if(service_id == 1116)
                                {
                                    token = Guid.NewGuid().ToString();
                                    token_exper = DateTime.Now.AddMinutes(Convert.ToDouble(5)).ToString("yyyy-MM-dd HH:mm:ss");
                                    string update_p = "update service_configuration set token = '" + token + "', token_experation = '" + token_exper + "' where service_id = " + RequestBody.ServiceID;
                                    bool res_p = ExecuteQuery(update_p, ref lines);
                                    if (res_p == false)
                                    {
                                        ret_code = 50012;
                                        description = "Internal Error";
                                        token = "";
                                        token_exper = "";

                                    }
                                }
                                else 
                                {
                                    //need to update token
                                    token = Guid.NewGuid().ToString();
                                    token_exper = DateTime.Now.AddDays(Convert.ToDouble(token_validaty)).ToString("yyyy-MM-dd HH:mm:ss");
                                    string update_q = "update service_configuration set token = '" + token + "', token_experation = '" + token_exper + "' where service_id = " + RequestBody.ServiceID;
                                    bool res = ExecuteQuery(update_q, ref lines);
                                    if (res == false)
                                    {
                                        ret_code = 50012;
                                        description = "Internal Error";
                                        token = "";
                                        token_exper = "";

                                    }
                                }
                               

                            }
                        }
                    }
                    
                     if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "Login was Validated";
                    }

                    result = new DLValidateLogin()
                    {
                        
                        Token = token,
                        TokenExperation = token_exper,
                        RetCode = ret_code,
                        Description = description
                    };
                    lines = Add2Log(lines, "Description = " + description + ", RetCode = " + ret_code, 100, lines[0].ControlerName);
                }

            }
            catch (MySqlException ex)
            {
                result = new DLValidateLogin()
                {
                    RetCode = 50013,
                    Description = "Internal Error"
                };
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }

            return result;
        }

        

        public static bool ExecuteQuery(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            bool result = true;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static bool ExecuteQuery(string execute_query, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            bool result = true;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }


        public static bool ExecuteQuery(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            bool result = true;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = execute_query;
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "exception on connection " + ex1.ToString(), 100, lines[0].ControlerName);
                }
                

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static bool ExecuteQuery(string execute_query, string connection_string, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            bool result = true;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = execute_query;
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "exception on connection " + ex1.ToString(), 100, lines[0].ControlerName);
                    logMessages.Add(new { message = ex1.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
                }


            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static Int64 SelectQueryReturnInt64(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            Int64 result = 0;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt64(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;
        }

        public static Int64 SelectQueryReturnInt64(string execute_query, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            Int64 result = 0;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt64(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQuery", logz_id = logz_id });
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static List<Int64> SelectQueryReturnListInt64(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<Int64> result = null;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<long>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToInt64(reader.GetValue(0)));
                    }
                }
                
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static List<String> SelectQueryReturnListString(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<String> result = null;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<String>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToString(reader.GetValue(0)));
                    }
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static List<String> SelectQueryReturnListString(string execute_query, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<String> result = null;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.SelectQueryReturnListString", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<String>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToString(reader.GetValue(0)));
                    }
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.SelectQueryReturnListString", logz_id = logz_id });
                result = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static List<String> SelectQueryReturnListString(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<String> result = null;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<String>();
                    while (reader.Read())
                    {
                        result.Add(Convert.ToString(reader.GetValue(0)));
                    }
                }

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static string SelectQueryReturnString(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string result = "";
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToString(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = "" ;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static string SelectQueryReturnString(string execute_query, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string result = "";
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.SelectQueryReturnString", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToString(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.SelectQueryReturnString", logz_id = logz_id });
                result = "";
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static string SelectQueryReturnStringE(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string result = "";
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToString(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = "Error";
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static string SelectQueryReturnString(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string result = "";
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToString(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = "";
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static Int64 SelectQueryReturnInt64(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            Int64 result = 0;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt64(reader.GetValue(0));
                }
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static List<Int64> SelectQueryReturnListInt64(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            List<Int64> result = null;
            lines = Add2Log(lines, "select_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();
                if (reader.HasRows == true)
                {
                    result = new List<Int64>();
                    
                    while (reader.Read())
                    {
                        Int64 m = Convert.ToInt64(reader.GetValue(0));
                        result.Add(m);
                    }
                }
                
            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static Int64 ExecuteQueryReturnInt64(string execute_query, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            Int64 result = 0;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                result = command.LastInsertedId;

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }

        public static Int64 ExecuteQueryReturnInt64(string execute_query, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            MySqlConnection connection = null;
            Int64 result = 0;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            logMessages.Add(new { message = execute_query, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQueryReturnInt64", logz_id = logz_id });
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                result = command.LastInsertedId;

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "DBQueries.ExecuteQueryReturnInt64", logz_id = logz_id });
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }


        public static Int64 ExecuteQueryReturnInt64(string execute_query, string connection_string, ref List<LogLines> lines)
        {
            MySqlConnection connection = null;
            Int64 result = 0;
            lines = Add2Log(lines, "execute_query = " + execute_query, 100, lines[0].ControlerName);
            try
            {
                connection = new MySqlConnection(ServerSettings.GetServerSettings(connection_string, ref lines));
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = execute_query;
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
                result = command.LastInsertedId;

            }
            catch (MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
                result = 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return result;

        }


        public static bool CheckDNDStatus(Int64 msisdn, int operatorId, ref List<LogLines> lines)
        {

            // Execute the query using the existing selectquery method
            Int64 dndCount = SelectQueryReturnInt64($"SELECT 1 FROM dnd WHERE msisdn = {msisdn} AND operator_id = { operatorId };", ref lines);

            // If we found any records, the number is on DND
            return dndCount > 0;
        }
    }
}