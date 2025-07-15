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

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for ussd_ic479_m
    /// </summary>
    public class ussd_ic479_m : IHttpHandler
    {

        public static string BuildMMenu(string msisdn, string menu_2_display, bool is_close, int action_id, USSDSession ussd_session, USSDMenu ussd_menu, ref List<LogLines> lines)
        {
            string menu = "";
            if (!String.IsNullOrEmpty(menu_2_display))
            {

                if (is_close)
                {
                    menu = "{";
                    menu = menu + "\"page\":{";
                    menu = menu + "\"session_end\":\"true\"";
                    menu = menu + "},";
                    menu = menu + "\"message\":\""+ menu_2_display + "\"";
                    menu = menu + "}";
                }
                else
                {
                    String[] spearator = { Environment.NewLine };
                    String[] spearator1 = { ")" };
                    String[] strlist = menu_2_display.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                    string menu_2_display_options = "";
                    string form_type = "menu", form_menu = "";
                    string my_url = (HttpContext.Current.Request.ServerVariables["SERVER_PORT"] == "443" ? "https://" : "http://") + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"] + "/madapi/ussd_ic479_m.ashx?ussd_string=";

                    menu = "{";
                    menu = menu + "	\"title\":\"\",";
                    string msg_menu = "\"message\":\"";
                    form_menu = "	\"form\": {";
                    form_menu = form_menu + "		\"url\": \""+ my_url + "\",";
                    form_menu = form_menu + "		\"type\": \"text\",";
                    form_menu = form_menu + "		\"method\": \"get\"";
                    form_menu = form_menu + "	},";
                    form_menu = form_menu + "	\"links\": [";

                    


                    string txt_full = "";
                    string link_menu = "";
                    foreach (string s in strlist)
                    {
                        if (!s.Contains(")"))
                        {
                            msg_menu = msg_menu + s+"\n";
                        }
                        else
                        {
                            String[] strlist1 = s.Split(spearator1, StringSplitOptions.RemoveEmptyEntries);
                            if (strlist1.Length > 0)
                            {
                                link_menu = link_menu + "{\"content\":\"" + strlist1[1] + "\",\"url\":\"" + my_url + strlist1[0] + "\"},";
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(link_menu))
                    {
                        link_menu = link_menu.Substring(0, link_menu.Length - 1);
                    }
                    menu = menu + msg_menu + "\"," + form_menu + link_menu + "],";
                    menu = menu + "	\"page\":{";
                    
                    
                    
                        menu = menu + "		\"session_end\":\"false\",";
                    
                    
                    menu = menu + "		\"menu\":\"true\",";
                    menu = menu + "		\"history\":\"true\",";
                    menu = menu + "		\"volatile\":\"true\",";
                    menu = menu + "		\"navigation_keywords\":\"true\"";
                    menu = menu + "	}";
                    menu = menu + "}";
                }

            }
            return menu;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
        
            
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_ic479_m");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_ic479_m");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_ic479_m");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_ic479_m");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_ic479_m");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ussd_ic479_m");
            }

            foreach (String key in context.Request.ServerVariables.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.ServerVariables[key], 100, "ussd_ic479_m");
            }

            string messageType = "", MSISDN = "", traceUniqueID = "", msgType = "", ussdString = "", cu_id = "", abort_soap = "";
            string ussd_soap = "";
            cu_id = context.Request.QueryString["cu_id"];

            

            string m_type = "1";

            string time_stamp = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            //DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;
            ServiceClass service = GetServiceByServiceID(939, ref lines);

            //{"sessionId":"16565763332210550","messageType":"2","msisdn":"242068535312","serviceCode":"1047","ussdString":"3","cellId":"14921","language":"en","imsi":"629100127926623"}

            MSISDN = context.Request.ServerVariables["HTTP_USER_MSISDN"];
            MSISDN = (String.IsNullOrEmpty(MSISDN) ? "25205123456" : MSISDN);
            lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo");

            ussdString = context.Request.QueryString["ussd_string"];
            ussdString = (String.IsNullOrEmpty(ussdString) ? "479" : ussdString);
            lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo");

            string spID = "479", ServiceID = "", serviceCode = "479", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";

            USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

            string response_soap = "";
            if (service != null && ussdString != "")
            {
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);
                    if (ussdString == "479")
                    {
                        ussd_session = null;
                    }
                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "BJ", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                        //GoogleAnalytics.SendData2GoogleAnalytics("UA-198534595-2", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "CG", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                        //freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                        //freeflowState = (is_close == true ? "FB" : "FC");
                        m_type = (is_close == true ? "2" : m_type);
                        menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");
                        response_soap = BuildMMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                    }
                }
            }






            //gq0TNIiTfKAsv+G192ph+ETjJhdFRvfAPvUEJdtq6MjUdI6VKxdPaG6fAr7OwKtQ2fOal7WgYE9MOz2HNBFH+dkaKVFOKVaxOds8TztMs5oCHgneuxvx6HzIrFfWQo/ZwGqNAFk6pDvId+AJOFh4llDQ8oTCPcTCFixetQLMEFEuT/qIHj+K5HTX4xGzdbiuxmRMu4fkfuPwmYTvTitycG4mU8i3dBAqt2szWqszDoQQiLwginVZJkxYDjzV4WA+Q+hG4k8WdKw7qJwyyGifdhhoHuajwH/v4HnwKzqGnPf/H/FaMxfTT9PkC193gbkBlGEWWTNInx1v5FiDYUemfQ==
            //context.Response.AddHeader("sid", "f8fa405c-45ba-3328-8a1b-e67cc9a05230");
            //context.Response.AddHeader("auth", "gq0TNIiTfKAsv+G192ph+ETjJhdFRvfAPvUEJdtq6MjUdI6VKxdPaG6fAr7OwKtQ2fOal7WgYE9MOz2HNBFH+dkaKVFOKVaxOds8TztMs5oCHgneuxvx6HzIrFfWQo/ZwGqNAFk6pDvId+AJOFh4llDQ8oTCPcTCFixetQLMEFEuT/qIHj+K5HTX4xGzdbiuxmRMu4fkfuPwmYTvTitycG4mU8i3dBAqt2szWqszDoQQiLwginVZJkxYDjzV4WA+Q+hG4k8WdKw7qJwyyGifdhhoHuajwH/v4HnwKzqGnPf/H/FaMxfTT9PkC193gbkBlGEWWTNInx1v5FiDYUemfQ==");


            context.Response.AddHeader("sid", "ba9e0e91-1e71-323f-9651-90ea4b217ee7");
            context.Response.AddHeader("auth", "zjM7+SYMqw9+4K/ulrcHDQMXgMN+5pCbhlPulmhL3fBWUn76lLjmhsVC46gwEdRgPJl+adV12rG+6zbusHd4r3tbclDiIlPmJ4Y59zRpSXmvUY4gweMVqUfe/pfb1nZIKDUOxjd9F/YAqhhmJjUxmNgxe8Q47CgWCUbDkioHuNE0PflD4Z0H/sIOr2pg7lOfOHgkvR0pWCijdhnMNzA60EEPWLqbJvWJA/J9qngEEoBlyCFML2jW5L370EVuR20BjOqlD0IDdSRZqQSeXDLnB6+tGjwyiiRVVFy4TFMnA5CMu3J8s81Op+rzjCtBFeu56lWFbHbKZx3n0suzHM3mkA==");

            context.Response.AddHeader("cache-control", "no-cache");

            //context.Response.AddHeader("sid", "ba9e0e91-1e71-323f-9651-90ea4b217ee7");
            //cache-control: no-cache
            //Volatile: true

            //            Service ID (sid)
            //ba9e0e91-1e71-323f-9651-90ea4b217ee7
            //Primary token (auth)
            //a457a17d-38cf-43e4-ac21-74c5af708d8e


            lines = Add2Log(lines, "Response = " + response_soap, 100, "ussd_ic479");
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

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