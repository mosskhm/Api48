using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Notifications
    {
        public static void SendSubscriptionNotification(string msisdn, string service_id, string subscription_date, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('"+service_urls.subscription_url+"','"+json_xml+"',now(),'',0,"+service_id+")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);

                }

            }
        }

        public static void SendSubscriptionNotification(string msisdn, string service_id, string subscription_date, Int64 sub_id, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.subscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.dtt_url, 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                                 //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                        }
                    }
                    
                }
            }
        }

        public static void SendSubscriptionNotificationIAR(string msisdn, string service_id, string subscription_date, Int64 sub_id, string auto_renew, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.subscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            bool auto_renew_b = (auto_renew == "YES" ? true : false);
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\", \"AutoRenew\": \""+auto_renew_b+"\"}");
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.dtt_url, 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                                 //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                        }
                    }

                }
            }
        }

        public static void SendSubscriptionNotificationIAR(string msisdn, string service_id, string subscription_date, Int64 sub_id, string auto_renew, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.subscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines, ref logMessages, app_name, logz_id);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines, ref logMessages, app_name, logz_id);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            bool auto_renew_b = (auto_renew == "YES" ? true : false);
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\", \"AutoRenew\": \"" + auto_renew_b + "\"}");
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.dtt_url, 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                                 //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                        }
                    }

                }
            }
        }

        public static void SendSubscriptionNotification(string msisdn, string service_id, string subscription_date, string transaction_id, string description, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate><TransactionID>" + transaction_id + "</TransactionID><Description>" + description + "</Description></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\", \"TransactionID\": \"" + transaction_id + "\", \"Description\": \"" + description + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.subscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

            }
        }

        public static void SendSubscriptionNotificationWithEncMSISDN(string msisdn, string service_id, string subscription_date, string transaction_id, string enc_msisdn, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.subscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySubscription><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><SubscriptionDate>" + subscription_date + "</SubscriptionDate><TransactionID>" + transaction_id + "</TransactionID><EncryptedMSISDN>" + enc_msisdn + "</EncryptedMSISDN></NotifySubscription>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"SubscriptionDate\": \"" + subscription_date + "\", \"TransactionID\": \"" + transaction_id + "\", \"EncryptedMSISDN\": \"" + enc_msisdn + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Subscription Notification to : " + service_urls.subscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.subscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.subscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }
        }

        public static void SendUnSubscriptionNotification(string msisdn, string service_id, string deactivation_date, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.unsubscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.unsubscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.unsubscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }



        }

        public static void SendUnSubscriptionNotification(string msisdn, string service_id, string deactivation_date, Int64 sub_id, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.unsubscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.unsubscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.unsubscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\"}");
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.dtt_url, 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                        }
                    }
                    
                }


            }
        }

        public static void SendUnSubscriptionNotification(string msisdn, string service_id, string deactivation_date, Int64 sub_id, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.unsubscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.unsubscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.unsubscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines, ref logMessages, app_name, logz_id);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines, ref logMessages, app_name, logz_id);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\"}");
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.dtt_url, 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines, ref logMessages, app_name, logz_id);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                        }
                    }
                }
            }
        }

        public static void SendUnSubscriptionNotification(string msisdn, string service_id, string deactivation_date, string enc_msisdn, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.unsubscription_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyUserDeactivation><MSISDN>" + msisdn + "</MSISDN><EncryptedMSISDN>"+enc_msisdn+"</EncryptedMSISDN><ServiceID>" + service_id + "</ServiceID><DeactivationDate>" + deactivation_date + "</DeactivationDate></NotifyUserDeactivation>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeactivationDate\": \"" + deactivation_date + "\", \"EncryptedMSISDN\": \"" + enc_msisdn + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending UnSubscription Notification to : " + service_urls.unsubscription_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";//CallSoap.CallSoapRequest(service_urls.unsubscription_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.unsubscription_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }



        }

        public static void SendBillingNotification(string msisdn, string service_id, string billing_date, string fee, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + "}");
                    List<Headers> headers = new List<Headers>();
                    //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

            }



        }



        public static void SendBillingNotificationIsRebilling(string msisdn, string service_id, string billing_date, string fee, bool is_rebilling, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            string ir = Convert.ToString(is_rebilling).ToLower();
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><IsRebilling>"+ir+"</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": "+ir+"}");
                    List<Headers> headers = new List<Headers>();
                    //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    //if (is_rebilling == true)
                    //{
                        string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                             //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                        lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}
                    //else
                    //{
                    //    string response = CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}

                }

            }
        }

        public static void SendBillingNotificationIsRebilling(string msisdn, string service_id, string billing_date, string fee, bool is_rebilling, Int64 sub_id, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            string ir = Convert.ToString(is_rebilling).ToLower();
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><IsRebilling>" + ir + "</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": " + ir + "}");
                    List<Headers> headers = new List<Headers>();
                    //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    //if (is_rebilling == true)
                    //{
                    string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                         //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}
                    //else
                    //{
                    //    string response = CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}

                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><IsRebilling>" + ir + "</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": " + ir + "}");
                            List<Headers> headers = new List<Headers>();
                            //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            //if (is_rebilling == true)
                            //{
                            string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                                 //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                            //}
                            //else
                            //{
                            //    string response = CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            //    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                            //}
                        }
                    }


                }


            }
        }

        public static void SendBillingNotificationIsRebilling(string msisdn, string service_id, string billing_date, string fee, bool is_rebilling, Int64 sub_id, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            string ir = Convert.ToString(is_rebilling).ToLower();
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><IsRebilling>" + ir + "</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": " + ir + "}");
                    List<Headers> headers = new List<Headers>();
                    //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    //if (is_rebilling == true)
                    //{
                    string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                         //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines, ref logMessages, app_name, logz_id);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}
                    //else
                    //{
                    //    string response = CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    //}

                }

                if (service_urls.dtt_url.ToLower().Contains("http"))
                {
                    string channel_name = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT sm.channel_name FROM subscribers_misc sm WHERE sm.subscriber_id = " + sub_id, ref lines, ref logMessages, app_name, logz_id);
                    if (!String.IsNullOrEmpty(channel_name))
                    {
                        if (channel_name == "DTT")
                        {
                            json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><IsRebilling>" + ir + "</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": " + ir + "}");
                            List<Headers> headers = new List<Headers>();
                            //lines = Add2Log(lines, " Sending Billing Notification to : https://lnb.idobet.com/services/B2B/api/payments/Yellowbet/NotifyDYAReceive", 100, lines[0].ControlerName);
                            lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                            //if (is_rebilling == true)
                            //{
                            string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                                                 //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.dtt_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines, ref logMessages, app_name, logz_id);
                            lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                            //}
                            //else
                            //{
                            //    string response = CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                            //    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                            //}
                        }
                    }


                }


            }
        }

        public static void SendBillingNotificationIsRebilling(string msisdn, string service_id, string billing_date, string fee, bool is_rebilling, string enc_msisdn, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            string ir = Convert.ToString(is_rebilling).ToLower();
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><EncryptedMSISDN>"+enc_msisdn+"</EncryptedMSISDN><Price>" + fee + "</Price><IsRebilling>" + ir + "</IsRebilling></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"IsRebilling\": " + ir + ", \"EncryptedMSISDN\": \"" + enc_msisdn + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = "";// CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }
        }

        public static void SendBillingNotification(string msisdn, string service_id, string billing_date, string fee, string result, string reason, string subscription_date, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.billing_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyBilling><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><BillingDate>" + billing_date + "</BillingDate><Price>" + fee + "</Price><Result>" + result + "</Result><Reason>" + reason + "</Reason><SubscriptionDate>" + subscription_date + "</SubscriptionDate></NotifyBilling>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"BillingDate\": \"" + billing_date + "\", \"Price\": " + fee + ", \"Result\": \"" + result + "\", \"Reason\": \"" + reason + "\", \"SubscriptionDate\": \"" + subscription_date + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Billing Notification to : " + service_urls.billing_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = ""; //CallSoap.CallSoapRequest(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //CallSoap.CallSoapRequestAsync(service_urls.billing_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.billing_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }



        }

        public static void SendMONotification(string msisdn, string service_id, string delivery_date, string text, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.mo_url.ToLower().Contains("http") || service_urls.mo_url.ToLower().Contains("https"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyMO><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeliveryDate>" + delivery_date + "</DeliveryDate><Text>" + text + "</Text></NotifyMO>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeliveryDate\": \"" + delivery_date + "\", \"Text\": \"" + text + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending MO Notification to : " + service_urls.mo_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(service_urls.mo_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
                    

            }



        }

        public static void SendMONotificationWithEncMSISDN(string msisdn, string service_id, string delivery_date, string text, string enc_msisdn, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.mo_url.ToLower().Contains("http") || service_urls.mo_url.ToLower().Contains("https"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyMO><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeliveryDate>" + delivery_date + "</DeliveryDate><Text>" + text + "</Text><EncryptedMSISDN>" + enc_msisdn + "</EncryptedMSISDN></NotifyMO>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeliveryDate\": \"" + delivery_date + "\", \"Text\": \"" + text + "\", \"EncryptedMSISDN\": \"" + enc_msisdn + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending MO Notification to : " + service_urls.mo_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(service_urls.mo_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
            }
        }


        public static void SendSMSDLRNotification(string msisdn, string service_id, string delivery_date, string trans_id, string deliver_result, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.sms_dlr_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySendSMS><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeliveryDate>" + delivery_date + "</DeliveryDate><TransactionID>" + trans_id + "</TransactionID><Response>" + deliver_result + "</Response></NotifySendSMS>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeliveryDate\": \"" + delivery_date + "\", \"TransactionID\": \"" + trans_id + "\", \"Response\": \"" + deliver_result + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending SMS DLR Notification to : " + service_urls.sms_dlr_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(service_urls.sms_dlr_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
                    

            }



        }

        public static void SendSMSDLRNotification(string msisdn, string service_id, string delivery_date, string trans_id, string deliver_result, string description, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.sms_dlr_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifySendSMS><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><DeliveryDate>" + delivery_date + "</DeliveryDate><TransactionID>" + trans_id + "</TransactionID><Response>" + deliver_result + "</Response></NotifySendSMS>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"DeliveryDate\": \"" + delivery_date + "\", \"TransactionID\": \"" + trans_id + "\", \"Response\": \"" + deliver_result + "\", \"Description\": \"" + description + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending SMS DLR Notification to : " + service_urls.sms_dlr_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    //string response = CallSoap.CallSoapRequest(service_urls.sms_dlr_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    //lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into callbacks_q (url, json, date_time, response, process, service_id) values('" + service_urls.sms_dlr_url + "','" + json_xml + "',now(),'',0," + service_id + ")", "DBConnectionString_104", ref lines);
                }
            }
        }

        public static void SendDYAReceiveNotification(string msisdn, string service_id, string trans_id, string partner_trans, string dya_result, int dya_code, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.dya_receive_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyDYAReceive><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><TransactionID>" + trans_id + "</TransactionID><PartnerTransactionID>" + partner_trans + "</PartnerTransactionID><Description>" + dya_result + "</Description><ResultCode>" + dya_code + "</ResultCode></NotifyDYAReceive>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"TransactionID\": \"" + trans_id + "\", \"PartnerTransactionID\": \"" + partner_trans + "\", \"Description\": \"" + dya_result + "\", \"ResultCode\": " + dya_code + "}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending DYA Receive Notification to : " + service_urls.dya_receive_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(service_urls.dya_receive_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
                    
            }
        }

        public static void SendDYAReceiveNotification(string msisdn, string service_id, string trans_id, string partner_trans, string dya_result, int dya_code, string datetime, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            bool is_failed = true;
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.dya_receive_url.ToLower().Contains("http"))
                {
                    
                    dya_result = Api.CommonFuncations.Base64.RemoveAccents(dya_result);
                    string url = service_urls.dya_receive_url;
                    //switch (msisdn)
                    //{
                    //    case "22996754408":
                    //    case "22996743244":
                    //    case "22960004547":
                    //        url = "https://api.b2tech.com/services/b2bapi/api/payments/yellowbet/NotifyDYAReceiveV2";
                    //        break;
                    //}
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyDYAReceive><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><TransactionID>" + trans_id + "</TransactionID><PartnerTransactionID>" + partner_trans + "</PartnerTransactionID><Description>" + dya_result + "</Description><ResultCode>" + dya_code + "</ResultCode><Timestamp>" + datetime + "</Timestamp></NotifyDYAReceive>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"TransactionID\": \"" + trans_id + "\", \"PartnerTransactionID\": \"" + partner_trans + "\", \"Description\": \"" + dya_result + "\", \"ResultCode\": " + dya_code + ", \"Timestamp\": \"" + datetime + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending DYA Receive Notification to : " + url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(url, json_xml, headers, service_urls.notification_method_id, out is_failed, ref lines);
                    if (is_failed == true)
                    {
                        ServiceBehavior.DecideDYAFailedNotification(Convert.ToInt64(trans_id), Convert.ToInt32(service_id), partner_trans, url, json_xml, service_urls.notification_method_id, ref lines);
                    }

                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }
                    
            }
        }

        public static void SendDYAReceiveNotification(string msisdn, string service_id, string trans_id, string partner_trans, string dya_result, int dya_code, string datetime, out string error, ref List<LogLines> lines)
        {
            error = "";
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            bool is_failed = true;
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.dya_receive_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyDYAReceive><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><TransactionID>" + trans_id + "</TransactionID><PartnerTransactionID>" + partner_trans + "</PartnerTransactionID><Description>" + dya_result + "</Description><ResultCode>" + dya_code + "</ResultCode><Timestamp>" + datetime + "</Timestamp></NotifyDYAReceive>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"TransactionID\": \"" + trans_id + "\", \"PartnerTransactionID\": \"" + partner_trans + "\", \"Description\": \"" + dya_result + "\", \"ResultCode\": " + dya_code + ", \"Timestamp\": \"" + datetime + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending DYA Receive Notification to : " + service_urls.dya_receive_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName); 
                    string response = CallSoap.CallSoapRequest(service_urls.dya_receive_url, json_xml, headers, service_urls.notification_method_id, out is_failed, ref lines);
                    if (is_failed == true)
                    {
                        ServiceBehavior.DecideDYAFailedNotification(Convert.ToInt64(trans_id), Convert.ToInt32(service_id), partner_trans, service_urls.dya_receive_url, json_xml, service_urls.notification_method_id, ref lines);
                        error = response;
                    }

                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

            }
        }

        public static void SendChargeAmountNotification(string msisdn, string service_id, string trans_id, string partner_trans, string dya_result, ref List<LogLines> lines)
        {
            ServiceURLS service_urls = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
            string json_xml = "";
            if (service_urls != null)
            {
                if (service_urls.chargeamount_url.ToLower().Contains("http"))
                {
                    json_xml = (service_urls.notification_method_id == 1 ? "<?xml version=\"1.0\" encoding=\"utf-8\"?><NotifyChargeAmount><MSISDN>" + msisdn + "</MSISDN><ServiceID>" + service_id + "</ServiceID><TransactionID>" + trans_id + "</TransactionID><PartnerTransactionID>" + partner_trans + "</PartnerTransactionID><Response>" + dya_result + "</Response></NotifyChargeAmount>" : "{\"MSISDN\": " + msisdn + ", \"ServiceID\": " + service_id + ", \"TransactionID\": \"" + trans_id + "\", \"PartnerTransactionID\": \"" + partner_trans + "\", \"Response\": \"" + dya_result + "\"}");
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, " Sending Charge Amount Notification to : " + service_urls.chargeamount_url, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, " Body : " + json_xml, 100, lines[0].ControlerName);
                    string response = CallSoap.CallSoapRequest(service_urls.chargeamount_url, json_xml, headers, service_urls.notification_method_id, ref lines);
                    lines = Add2Log(lines, " Response : " + response, 100, lines[0].ControlerName);
                }

                    
            }
        }


    }
}