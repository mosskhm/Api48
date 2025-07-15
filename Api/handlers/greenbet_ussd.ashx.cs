using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for greenbet_ussd
    /// </summary>
    public class greenbet_ussd : IHttpHandler
    {
        public static string BuildMoovMenu(string action_id, string menu_2_display, bool is_close)
        {
            string menu = "";
            if (!String.IsNullOrEmpty(menu_2_display))
            {
                menu_2_display = menu_2_display.Replace("yellowbet.com","greenwin.bj");
                if (is_close)
                {
                    menu = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    menu = menu + "<response>";
                    menu = menu + "	<screen_type>form</screen_type>";
                    menu = menu + "	<text>"+ menu_2_display + "</text>";
                    menu = menu + "	<session_op>end</session_op>";
                    menu = menu + "</response>";
                }
                else
                {
                    String[] spearator = { Environment.NewLine };
                    String[] spearator1 = { ")" };
                    String[] strlist = menu_2_display.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    string menu_2_display_text = "", menu_2_display_options = "";
                    string menu_type = "";
                    string form_type = "form";
                    string txt_full = "";
                    foreach (string s in strlist)
                    {
                        if (s.Contains(")"))
                        {
                            String[] strlist1 = s.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                            string option = "", text = "";
                            foreach (string s1 in strlist1)
                            {
                                if (String.IsNullOrEmpty(option))
                                {
                                    string tmp_str = s1.Replace("*", "8").Replace("#", "9").Replace("M", "0");
                                    int number;
                                    bool success = Int32.TryParse(tmp_str, out number);
                                    option = (success ? tmp_str : "");
                                    form_type = (success ? "menu" : form_type);
                                }
                                else
                                {
                                    string tmp_s1 = (s1.Substring(0, 1) == " " ? s1.Substring(1) : s1);
                                    text = text + tmp_s1 + ")";
                                }
                            }
                            if (option != "")
                            {
                                if (text.Length > 0)
                                {
                                    text = (text.Contains("(") ? text : text.Substring(0, text.Length - 1));
                                }
                                
                                menu_2_display_options = menu_2_display_options + "<option choice=\"" + option + "\">" + text + "</option>" + Environment.NewLine;
                            }
                            
                        }
                        else
                        {
                            txt_full = txt_full + s + Environment.NewLine;
                            //menu_2_display_text = menu_2_display_text + "<text>" + s + "</text>" + Environment.NewLine;
                        }
                    }
                    menu = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    menu = menu + "<response>";
                    menu = menu + "	<screen_type>" + form_type + "</screen_type>";
                    if (!String.IsNullOrEmpty(txt_full))
                    {
                        menu = menu + "<text>" + txt_full + "</text>" + Environment.NewLine;
                    }
                    if (!String.IsNullOrEmpty(menu_2_display_options))
                    {
                        menu = menu + "	<options>";
                        menu = menu + menu_2_display_options;
                        menu = menu + "	</options>";
                    }
                    //menu = menu + "	<back_link>1</back_link>";
                    menu = menu + "	<session_op>continue</session_op>";
                    menu = menu + "	<screen_id>"+action_id+"</screen_id>";
                    menu = menu + "</response>";

                }
                
            }
            return menu;
        }

        public void ProcessRequest(HttpContext context)
        {
            //GET /handlers/greenbet_ussd.ashx lang=eng&msisdn=22994558779&req_no=1&sc=551&screen_id=&session_id=677906224&user_input=
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "moov_ussd");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "moov_ussd");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "moov_ussd");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "moov_ussd");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "moov_ussd");
            }

            string msisdn = context.Request.QueryString["msisdn"];
            string user_input = context.Request.QueryString["user_input"];
            string sc = context.Request.QueryString["sc"];
            user_input = (String.IsNullOrEmpty(user_input) ? sc : user_input);



            ServiceClass service = new ServiceClass();
            string spID = sc, ServiceID = "", serviceCode = sc, linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
            service = GetServiceByServiceID(715, ref lines);
            USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
            string ussd_soap = "";
            string menu_2_display = "";
            DYAReceiveMoneyRequest momo_request = null;
            string moov_xml = "";
            bool is_close = false;
            if (umc != null && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(user_input))
            {
                lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(msisdn, umc.ussd_id, ref lines);

                int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, user_input, action_id, ussd_session, ref lines);
                if (ussd_menu != null)
                {
                    GoogleAnalytics.SendData2GoogleAnalytics("UA-154843894-1", "ussd", Base64.Reverse(msisdn), context.Request.ServerVariables["REMOTE_ADDR"], "BJ", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);

                    lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                    ussd_soap = Api.CommonFuncations.USSD.MoovUSSDBehaviuer(service, user_input, ServiceID, msisdn, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                    moov_xml = BuildMoovMenu(ussd_menu.action_id.ToString(), menu_2_display, is_close);

                }
            }

            if (is_close)
            {
                lines = Add2Log(lines, " its clean up time " + msisdn, 100, "ussd_mtnbenin");
                
                USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(msisdn, umc.ussd_id, ref lines);
                if (ussd_session != null)
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("update ussd_sessions set `status` = 1 where session_id = " + ussd_session.session_id, "DBConnectionString_104", ref lines);
                }
            }

            string xml = "";
            if (!String.IsNullOrEmpty(moov_xml))
            {
                xml = moov_xml;
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                xml = xml + "<response>";
                xml = xml + "	<screen_type>menu</screen_type>";
                xml = xml + "		<text>Bienvenue à GreenBet!</text>";
                xml = xml + "		<options>";
                xml = xml + "			<option choice=\"1\">Faites votre Pari</option>";
                xml = xml + "			<option choice=\"2\">Depot</option>";
                xml = xml + "			<option choice=\"3\">Retrait</option>";
                xml = xml + "			<option choice=\"4\">Statut du Ticket</option>";
                xml = xml + "			<option choice=\"5\">Service Clients</option>";
                xml = xml + "			<option choice=\"6\">Termes et Conditions</option>";
                xml = xml + "		</options>";
                xml = xml + "		<session_op>continue</session_op>";
                xml = xml + "		<screen_id>15</screen_id>";
                xml = xml + "</response>";
            }


            if (momo_request != null)
            {
                momo_request.Delay = "5000";
                string postBody = JsonConvert.SerializeObject(momo_request);
                string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                List<Headers> headers = new List<Headers>();
                lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
            }
            lines = Add2Log(lines, " Response: " + xml, 100, "ussd_mo");
            context.Response.ContentType = "text/xml";
            context.Response.Write(xml);
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