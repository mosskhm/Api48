using Api.HttpItems;
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
    /// Summary description for ussd_orange_guinea_stg
    /// </summary>
    public class ussd_orange_guinea_stg : IHttpHandler
    {
        public static string BuildOrangeMenu(string msisdn, string menu_2_display, bool is_close, int action_id, USSDSession ussd_session, USSDMenu ussd_menu, ref List<LogLines> lines)
        {
            string menu = "";
            if (!String.IsNullOrEmpty(menu_2_display))
            {

                if (is_close)
                {
                    menu = "<html>";
                    menu = menu + "<head>";
                    menu = menu + "<bearer>FINISH</bearer>";
                    menu = menu + "</head>";
                    menu = menu + "<body>";
                    menu = menu + menu_2_display;
                    menu = menu + "</body>";
                    menu = menu + "</html>";
                    if (ussd_session != null)
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("update ussd_sessions set `status` = 1 where session_id = " + ussd_session.session_id, "DBConnectionString_104", ref lines);
                    }
                    
                }
                else
                {
                    String[] spearator = { Environment.NewLine };
                    String[] spearator1 = { ")" };
                    String[] strlist = menu_2_display.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    string menu_2_display_options = "";
                    string form_type = "menu", form_menu = "";
                    if (ussd_menu.is_special)
                    {
                        form_type = "form";
                    }
                    if (menu_2_display.Contains("Montant incorrect, inroduisez 8 pour retourner et un montant entre") || menu_2_display.Contains("Confirmer votre Pari:") || menu_2_display.Contains("Confirmer votre Rapidos Pari:") || menu_2_display.Contains("Ensemble de nombres invalide. cliquez sur 8 pour revenir au menu precedent"))
                    {
                        form_type = "menu";
                    }
                    if (menu_2_display.Contains("Entrer le montant") || menu_2_display.Contains("Entrer votre montant") || menu_2_display.Contains("Chaque numero est separe par un espace"))
                    {
                        form_type = "form";
                    }


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
                                string my_url = (HttpContext.Current.Request.ServerVariables["SERVER_PORT"] == "443" ? "https://" : "http://") + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/USSDEmulator/OrangeGuinea_stg/" + option + "?ANI=" + msisdn + "&DNIS=" + option;
                                //http://10.173.84.178:8080/yellow-bet/orangeMoneyPayment?msisdn=224626960106&amount=2500&referenceNumbere=1234FGH344
                                if (text.ToLower() == "confirmer" && menu_2_display.Contains("Montant:"))
                                {
                                    //extract price
                                    String[] spearator2 = { "Montant:" };
                                    String[] strlist3 = menu_2_display.Split(spearator2, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strlist4 = strlist3[1].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                                    string amount = strlist4[0];
                                    int d_type = (menu_2_display.Contains("Vous etes sur le point de déposer") == true ? 2 : 1);
                                    d_type = (menu_2_display.Contains("Confirmer votre Rapidos Pari") == true ? 3 : d_type);

                                    Int64 ref_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into orange_billing_requests (msisdn, date_time, amount, session_id, d_type, service_id) values(" + msisdn + ",now()," + amount + ",'" + ussd_session.user_seesion_id + "'," + d_type + ",739)", "DBConnectionString_161", ref lines);
                                    if (ref_id > 0)
                                    {
                                        my_url = "http://10.173.84.178:8080/yellow-bet/orangeMoneyPayment?msisdn=" + msisdn + "&amount=" + amount + "&referenceNumbere=" + ref_id;
                                    }

                                }
                                menu_2_display_options = menu_2_display_options + "<a href=\"" + my_url + "\" key=\"" + option + "\">. " + text + "</a><br/>" + Environment.NewLine;
                            }
                        }
                        else
                        {
                            txt_full = txt_full + s + "<br/>" + Environment.NewLine;
                            //menu_2_display_text = menu_2_display_text + "<text>" + s + "</text>" + Environment.NewLine;
                        }
                    }

                    if (form_type == "form")
                    {

                        string my_url = (HttpContext.Current.Request.ServerVariables["SERVER_PORT"] == "443" ? "https://" : "http://") + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/USSDEmulator/OrangeGuinea_stg/O" + "?ANI=" + msisdn;
                        form_menu = "<form action=\"" + my_url + "\" method=\"POST\">" + Environment.NewLine;
                        form_menu = form_menu + "<input type=\"text\" name=\"DNIS\">" + Environment.NewLine;

                        form_menu = form_menu + "</form>" + Environment.NewLine;
                    }


                    menu = "<html>";
                    menu = menu + "<body>";
                    if (!String.IsNullOrEmpty(txt_full))
                    {
                        menu = menu + txt_full + Environment.NewLine;
                    }

                    if (!String.IsNullOrEmpty(menu_2_display_options))
                    {
                        menu = menu + menu_2_display_options;
                    }
                    if (!String.IsNullOrEmpty(form_menu))
                    {
                        menu = menu + form_menu;
                    }
                    menu = menu + "</body>";
                    menu = menu + "</html>";

                }

            }
            return menu;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "application/xhtml+xml; utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_orange_guinea_stg");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ussd_orange_guinea_stg");
            }

            string MSISDN = "", ussdString = "", time_stamp = "";
            string ussd_soap = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;

            MSISDN = context.Request.QueryString["ANI"];
            MSISDN = (String.IsNullOrEmpty(MSISDN) == true ? context.Request.Form["ANI"] : MSISDN);
            ussdString = context.Request.QueryString["DNIS"];
            ussdString = (String.IsNullOrEmpty(ussdString) == true ? context.Request.Form["DNIS"] : ussdString);

            if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(ussdString))
            {
                ServiceClass service = new ServiceClass();
                string spID = "3430", ServiceID = "", serviceCode = "3430", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(739, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ussd_orange_guinea_stg");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ussd_orange_guinea_stg");
                        ussd_soap = Api.CommonFuncations.USSD.OrangeGuineaUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                        menu_2_display = BuildOrangeMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                    }
                }
            }

            context.Response.Write(menu_2_display);
            lines = Add2Log(lines, " menu_2_display = " + menu_2_display, 100, "ussd_orange_guinea_stg");




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