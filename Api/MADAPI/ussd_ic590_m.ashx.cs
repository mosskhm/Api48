using Api.CommonFuncations;
using Api.HttpItems;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text.Json;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI.WebControls.Adapters;
using static Api.Cache.Services;
using static Api.Cache.SMS;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for ussd_ic590_m - Orange Ivory Coast
    /// </summary>
    public class ussd_ic590_m : IHttpHandler
    {

        public static string BuildMMenu(string msisdn, string menu_2_display, bool is_close, int action_id, USSDSession ussd_session, USSDMenu ussd_menu, ref List<LogLines> lines)
        {
            string menu = "";

            if (String.IsNullOrEmpty(menu_2_display))
            {
                lines = Add2Log(lines, "!! BuildMenu: menu2display is blank", 100, "");
                menu = "{"
                     + "\"page\":{"
                     + "\"session_end\":\"true\""
                     + "},"
                     + "\"message\":\"an error has occurred\""
                     + "}"
                     ;

            }
            else if (is_close)
            {
                lines = Add2Log(lines, "-- BuildMenu: closing session", 100, "");
                menu    = "{"
                        + "\"page\":{"
                        + "\"session_end\":\"true\""
                        + "},"
                        + "\"message\":\"" + menu_2_display + "\""
                        + "}"
                        ;
            }
            else
            {
                string my_url = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://")
                                + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":"
                                + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/madapi/ussd_ic590_m.ashx?ussd_string=";


                string msg_menu = "";
                string link_menu = "";

                String[] link_seperator = { ")" };
                String[] spearator = { "\n", "\r\n" };
                String[] strlist = menu_2_display.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in strlist)
                {
                    lines = Add2Log(lines, "-- BuildMenu: processing: " + s, 100, "");
                    // check if this contains details of our lings
                    String[] strlist1 = s.Split(link_seperator, StringSplitOptions.RemoveEmptyEntries);
                    if (strlist1.Length > 1)
                    {
                        if (link_menu.Length != 0) link_menu += ",";            // if not the first entry, add a comma
                        link_menu += "{\"content\":\"" + strlist1[1] + "\",\"url\":\"" + my_url + strlist1[0] + "\"}";
                    }
                    else
                    {
                        msg_menu += s + "\\n";       // escape the new lines
                        lines = Add2Log(lines, "-- BuildMenu: menu = [" + msg_menu + "]", 100, "");
                    }
                }


                // generate full reply
                menu    = "{"
                        +  " \"title\":\"\""
                        +  ",\"message\":\"" + msg_menu + "\""
                        +  ",\"form\":"
                        +  "{"
                        +  " \"url\": \"" + my_url + "\""
                        +  ",\"type\": \"text\""
                        +  ",\"method\": \"get\""
                        + "}"
                        +  ",\"links\": [" + link_menu + "]"
                        +  ",\"page\":"
                        +  "{"
                        +  " \"session_end\":\"false\""
                        +  ",\"menu\":\"true\""
                        +  ",\"history\":\"true\""
                        +  ",\"volatile\":\"true\""
                        +  ",\"navigation_keywords\":\"true\""
                        +  "}"
                        + "}"
                        ;
            }

            return menu;
        }

        private int sendSMS(ServiceClass service, string MSISDN, string msg)
        {

            // fetch auth token
            LoginResponse login = Login.DoLogin(new LoginRequest()
            {
                ServiceID = service.service_id,
                Password = service.service_password
            }
            );

            // if we have the login then use it to send the sms
            if (login != null && login.ResultCode == 1000)
            {
                SendSMSResponse sms = SendSMS.DoSMS(new SendSMSRequest()
                {
                    ServiceID = service.service_id,
                    MSISDN = Convert.ToInt64(MSISDN),
                    Text = msg,
                    TokenID = login.TokenID,
                    TransactionID = "ussd_ic590_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                });

                return sms.ResultCode;
            }
            else return 1050;
        } // sendSMS

        public void ProcessRequest(HttpContext context)
        {
            // open logging
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_ic590_m");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_ic590_m");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_ic590_m");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_ic590_m");

            // format response
            context.Response.AddHeader("sid", "3e4a5a16-3d41-3bd5-a132-d94650bb31af");
            context.Response.AddHeader("auth", "FrqgXVhyf6xgInHe0BxtsyEOaG2AMA2Q0Lm92jraxUCic+d3HJMmTZFuRVXfaVww27sHS6Z3lNiKpS5A3wUO/PwKzVRDtFiGyRnXATY3A2ZDwgPPhz7CtLXqIcgiasfDNk3a5cKUbBAS9T/pqbaROnUAR4KLQtFBPeRnkM7aUUnQXS0t8yDoSVc+bLnDFKzRAGb6BrxzI/Y8HZFo8zLAXWnk34wia+jnxhLHDc0E3AOO/11Bt4X7VcMvYAaaqvDDxpUCcsLoXa3ZqB7u1tvgOFx/g2Dcm0tI1G291DmuWx8+SNs9/ZtauwmvroA8UmYX2v2kDd0TOr71W3rjb2IpLA==");
            context.Response.AddHeader("cache-control", "no-cache");
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";

            try
            {
                var stream = context.Request.InputStream;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                string xml = System.Text.Encoding.UTF8.GetString(buffer);
                lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_ic590_m");

                
                
                string ussdString = "", cu_id = "", ussd_soap = ""; ;
                string menu_2_display = "";
                cu_id = context.Request.QueryString["cu_id"];

                string m_type = "1";

                DYAReceiveMoneyRequest momo_request = null;
                bool is_close = false;
                ServiceClass service = GetServiceByServiceID(1184, ref lines);

                string MSISDN = context.Request.ServerVariables["HTTP_USER_MSISDN"];
                MSISDN = (String.IsNullOrEmpty(MSISDN) ? "25205123456" : MSISDN);   // default to a test number if not specified -- TODO! should actually fail
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "");

                            ussdString = context.Request.QueryString["ussd_string"];
            
            // Check for Myriad's user-entry header (primary method for user input)
            string user_entry_header = context.Request.Headers["user-entry"];
            string user_entry_server = context.Request.ServerVariables["HTTP_USER_ENTRY"];
                
            // Use Myriad's user-entry header if available, otherwise use query string
            if (!String.IsNullOrEmpty(user_entry_header))
            {
                ussdString = user_entry_header;
                lines = Add2Log(lines, "Using user-entry header: " + ussdString, 100, "ussd_ic590_m");
            }
            else if (!String.IsNullOrEmpty(user_entry_server))
            {
                ussdString = user_entry_server;
                lines = Add2Log(lines, "Using HTTP_USER_ENTRY: " + ussdString, 100, "ussd_ic590_m");
            }
                
                // If not found in query string, try to extract from request body (JSON format)
                if (String.IsNullOrEmpty(ussdString) && !String.IsNullOrEmpty(xml))
                {
                    try
                    {
                        // Try to parse as JSON
                        if (xml.Trim().StartsWith("{"))
                        {
                            var jsonDoc = JsonDocument.Parse(xml);
                            if (jsonDoc.RootElement.TryGetProperty("ussdString", out var ussdStringElement))
                            {
                                ussdString = ussdStringElement.GetString();
                                lines = Add2Log(lines, "Extracted ussdString from JSON body: " + ussdString, 100, "ussd_ic590_m");
                            }
                            else if (jsonDoc.RootElement.TryGetProperty("user_input", out var userInputElement))
                            {
                                ussdString = userInputElement.GetString();
                                lines = Add2Log(lines, "Extracted user_input from JSON body: " + ussdString, 100, "ussd_ic590_m");
                            }
                            else if (jsonDoc.RootElement.TryGetProperty("input", out var inputElement))
                            {
                                ussdString = inputElement.GetString();
                                lines = Add2Log(lines, "Extracted input from JSON body: " + ussdString, 100, "ussd_ic590_m");
                            }
                            else
                            {
                                lines = Add2Log(lines, "JSON body found but no ussdString/user_input/input field: " + xml, 100, "ussd_ic590_m");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, "Failed to parse JSON body: " + ex.Message, 100, "ussd_ic590_m");
                    }
                }
                
                            ussdString = (String.IsNullOrEmpty(ussdString) ? "590" : ussdString);
            lines = Add2Log(lines, "Final ussdString = " + ussdString, 100, "ussd_ic590_m");

                string spID = "590", ServiceID = "", serviceCode = "590", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";

                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

                if (service != null && ussdString != "" && umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");

                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);
                    if (ussdString == "590")
                    {
                        ussd_session = null;
                    }
                    
                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu == null) 
                    {
                        lines = Add2Log(lines, " !! FAILED to fetch ussd_menu ID=" + umc.ussd_id, 100, "");
                        lines = Add2Log(lines, "Debug info - ussd_id: " + umc.ussd_id + ", ussdString: " + ussdString + ", action_id: " + action_id, 100, "");
                    }
                    else
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        
                        // using MTN CONGO for Orange because they also speak French
                        ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                        lines = Add2Log(lines, "MTNCongoUSSDBehaviuer returned - menu_2_display length: " + (menu_2_display?.Length ?? 0) + ", is_close: " + is_close, 100, "");

                        // add info if actionID=190
                        //if (ussd_menu.action_id == 190 && ussd_menu.prev_action_id == 1)
                        if (ussd_menu.action_name == "Display35" || ussd_menu.action_id == 190)
                        {
                           // menu_2_display += "\n" + LN.LNservice.sms_last_results(ref lines, 32, 2);

                            // send sms with more info to the user
                            if (MSISDN.StartsWith("225") || MSISDN.StartsWith("+225"))
                            {
                                string message = @"Pour souscrire à Numéro d'Or, envoyez NO1/ NO2 ou NO3) au 7717 ou via *590*5#. 
                                                    Pour se désabonner, envoyez STOP NO1 ( NO2 ou STOP NO3) au 7717. 
                                                    Pour accéder au portail cliquez sur ce lien: (https://orglnic.ydaplatform.com)";

                                sendSMS(service, MSISDN, message);
                                //sendSMS(service, MSISDN, LN.LNservice.sms_subscription_status(ref lines, 32, MSISDN));
                            }
                        }
                        // add info if actionID=191
                        //if (ussd_menu.action_id == 191 && ussd_menu.prev_action_id == 2)
                        if (ussd_menu.action_name == "Display36" || ussd_menu.action_id == 191)
                        {
                            //menu_2_display += "\n" + LN.LNservice.sms_last_results(ref lines, 32, 2);

                            // send combined SMS with subscription status and last 5 draw results
                            if (MSISDN.StartsWith("225") || MSISDN.StartsWith("+225"))
                            {
                                string combinedMessage = LN.LNservice.sms_combined_status_and_results(ref lines, 32, MSISDN, 5);
                                sendSMS(service, MSISDN, combinedMessage);
                            }
                        }

                        m_type = (is_close == true ? "2" : m_type);
                        menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");
                        
                        string response_soap = BuildMMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                        context.Response.Write(response_soap);

                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!!! HELP: {ex.Message}", 100, "");
                lines = Add2Log(lines, ex.StackTrace, 100, "");
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