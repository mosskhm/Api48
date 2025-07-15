using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class USSDEmulatorController : Controller
    {
        public ActionResult MTNIC479(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "2250596911619" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "479" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "479", ServiceID = "", serviceCode = "479", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

                service = GetServiceByServiceID(956, ref lines);
                
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                        string m_menu = Api.MADAPI.ussd_ic479_m.BuildMMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                        lines = Add2Log(lines, "m_menu = " + m_menu, 100, "USSDEmulator");
                    }
                }
                if (momo_request != null)
                {
                    momo_request.Delay = "1000";
                    string postBody = JsonConvert.SerializeObject(momo_request);
                    string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                    CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                    lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                }
            }

            
            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNNGiDoBet(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "2347048130055" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "205" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "205", ServiceID = "", serviceCode = "205", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(956, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.USSDBehaviuerNG205(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
                }
                if (momo_request != null)
                {
                    momo_request.Delay = "1000";
                    string postBody = JsonConvert.SerializeObject(momo_request);
                    string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                    CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                    lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                }
            }
            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNCameroon(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "237671563200" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*166#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();

                string spID = "2370110013381", ServiceID = "", serviceCode = "153", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(720, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = Api.CommonFuncations.USSD.MTNCameroonUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        lines = Add2Log(lines, " SDP ussd_soap = " + ussd_soap, 100, "ivr_subscribe");
                        //string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult YDGamesBenin(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22996691258" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                ServiceClass service = new ServiceClass();
                string spID = "2290110004259", ServiceID = "", serviceCode = "709", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(669, ref lines);


                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*709#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);


                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");

                        //string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult OrangeGuinea(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_orange_guinea");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_orange_guinea");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_orange_guinea");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "ussd_orange_guinea");
            string MSISDN = "", ussdString = "";
            string ussd_soap = "";
            string menu_2_display = "";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;

            MSISDN = Request.QueryString["ANI"];
            ussdString = Request.QueryString["DNIS"];
            //if (String.IsNullOrEmpty(ussdString))
            //{
            //    ussdString = id;
            //}
            MSISDN = (String.IsNullOrEmpty(MSISDN) == true ? Request.Form["ANI"] : MSISDN);
            ussdString = (String.IsNullOrEmpty(ussdString) == true ? Request.Form["DNIS"] : ussdString);
            if(!String.IsNullOrEmpty(ussdString))
            {
                if (ussdString.Contains("."))
                {
                    ussdString = "0";
                }
            }
            lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_orange_guinea");
            lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_orange_guinea");

            if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(ussdString))
            {
                ServiceClass service = new ServiceClass();
                string spID = "3430", ServiceID = "", serviceCode = "3430", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(732, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ussd_orange_guinea");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);
                    

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ussd_orange_guinea");
                        GoogleAnalytics.SendData2GoogleAnalytics("UA-172015213-2", "ussd", Base64.Reverse(MSISDN), Request.ServerVariables["REMOTE_ADDR"], "GN", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);

                        if (ussd_menu.is_maintenance)
                        {
                            if (ussd_menu.allowed_msisdns.Contains(MSISDN))
                            {
                                ussd_soap = Api.CommonFuncations.USSD.OrangeGuineaUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            }
                            else
                            {
                                menu_2_display = ussd_menu.maintenance_msg;
                                is_close = true;
                            }
                        }
                        else
                        {
                            ussd_soap = Api.CommonFuncations.USSD.OrangeGuineaUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                        }
                            
                        
                        
                        menu_2_display = Api.handlers.ussd_orange_guinea.BuildOrangeMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                    }
                }
            }
            lines = Add2Log(lines, " menu_2_display = " + menu_2_display, 100, "ussd_orange_guinea");
            lines = Write2Log(lines);
            return Content(menu_2_display, "text/html");
            
        }


        public ActionResult OrangeGuinea_STG(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "ussd_orange_guinea_stg");
            string MSISDN = "", ussdString = "";
            string ussd_soap = "";
            string menu_2_display = "";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;

            MSISDN = Request.QueryString["ANI"];
            ussdString = Request.QueryString["DNIS"];
            //if (String.IsNullOrEmpty(ussdString))
            //{
            //    ussdString = id;
            //}
            MSISDN = (String.IsNullOrEmpty(MSISDN) == true ? Request.Form["ANI"] : MSISDN);
            ussdString = (String.IsNullOrEmpty(ussdString) == true ? Request.Form["DNIS"] : ussdString);
            lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_orange_guinea_stg");
            lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_orange_guinea_stg");

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
                        menu_2_display = Api.handlers.ussd_orange_guinea_stg.BuildOrangeMenu(MSISDN, menu_2_display, is_close, action_id, ussd_session, ussd_menu, ref lines);
                    }
                }
            }
            lines = Add2Log(lines, " menu_2_display = " + menu_2_display, 100, "ussd_orange_guinea_stg");
            lines = Write2Log(lines);
            return Content(menu_2_display, "text/html");

        }
        // GET: USSDEmulator
        public ActionResult idoBetBenin(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22962456161" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";

            
            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {
                
                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*365#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;
                

                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "2290110011184", ServiceID = "", serviceCode = "365", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(682, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = "+ ussd_menu.action_id + " Action = " + ussd_menu.action_name , 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine,"<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }

            


            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNLNBBenin(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22951820708" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*500#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "2451", ServiceID = "", serviceCode = "500", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(726, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = Api.CommonFuncations.USSD.LNBPariMTNBeninUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
                }
                if (momo_request != null)
                {
                    momo_request.Delay = "5000";
                    string postBody = JsonConvert.SerializeObject(momo_request);
                    string url = "https://interface.lnbpari.com/api/DYAReceiveMoney";
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                    CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                    lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");

                }
            }




            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNLNBBeninV142(string id)
        {
            List<LogLines> lines = new List<LogLines>();

            try
            {
                lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
                lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
                lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
                lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


                string session_id = (String.IsNullOrEmpty(id) ? "" : id);
                lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
                ViewBag.SessionID = session_id;
                ViewBag.Menu2Display = "";
                ViewBag.IsClose = false;

                string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22996743244" : Request.QueryString["MSISDN"]);
                string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
                lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
                ViewBag.MSISDN = MSISDN;

                string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
                string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

                string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
                string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

                lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
                ViewBag.width_p = width_p;
                ViewBag.height_p = height_p;
                ViewBag.widthpx = width_p + "px";
                ViewBag.heightpx = height_p + "px";
                string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
                string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


                ViewBag.margin_top = margin_top;
                ViewBag.table_width = table_width;

                ViewBag.NewSessionID = Guid.NewGuid().ToString();

                if (!String.IsNullOrEmpty(session_id))
                {

                    string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*500#" : Request.Form["ussdString"]);
                    lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                    ViewBag.ussdString = ussdString;


                    DYAReceiveMoneyRequest momo_request = null;

                    ServiceClass service = new ServiceClass();
                    string spID = "2451", ServiceID = "", serviceCode = "500", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                    service = GetServiceByServiceID(726, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                    USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            string menu_2_display = "";
                            bool is_close = false;
                            string ussd_soap = Api.CommonFuncations.USSD.LNBPariMTNBeninUSSDBehaviuerV142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                            ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                            lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                            ViewBag.IsClose = is_close;
                        }
                    }
                    if (momo_request != null)
                    {
                        momo_request.Delay = "5000";
                        string postBody = JsonConvert.SerializeObject(momo_request);
                        string url = "https://interface.lnbpari.com/api/DYAReceiveMoney";
                        List<Headers> headers = new List<Headers>();
                        lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                        CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                        lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");

                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"FAILED!!! {ex.Message}", 100, "");
            }

            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MoovLNBBeninV142(string id)
        {
            
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();

            try
            {
                lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
                lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
                lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
                lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


                string session_id = (String.IsNullOrEmpty(id) ? "" : id);
                lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
                ViewBag.SessionID = session_id;
                ViewBag.Menu2Display = "";
                ViewBag.IsClose = false;

                string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22960004547" : Request.QueryString["MSISDN"]);
                string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
                lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
                ViewBag.MSISDN = MSISDN;

                string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
                string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

                string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
                string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

                lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
                ViewBag.width_p = width_p;
                ViewBag.height_p = height_p;
                ViewBag.widthpx = width_p + "px";
                ViewBag.heightpx = height_p + "px";
                string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
                string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


                ViewBag.margin_top = margin_top;
                ViewBag.table_width = table_width;

                ViewBag.NewSessionID = Guid.NewGuid().ToString();

                if (!String.IsNullOrEmpty(session_id))
                {

                    string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*500#" : Request.Form["ussdString"]);
                    lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                    ViewBag.ussdString = ussdString;


                    DYAReceiveMoneyRequest momo_request = null;

                    ServiceClass service = new ServiceClass();
                    string spID = "500", ServiceID = "", serviceCode = "500", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                    service = GetServiceByServiceID(730, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                    USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            string menu_2_display = "";
                            bool is_close = false;
                            string ussd_soap = Api.CommonFuncations.USSD.LNBPariMoovUSSDBehaviuerV142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                            ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                            lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                            ViewBag.IsClose = is_close;
                        }
                    }
                    if (momo_request != null)
                    {
                        momo_request.Delay = "5000";
                        string postBody = JsonConvert.SerializeObject(momo_request);
                        string url = "https://interface.lnbpari.com/api/DYAReceiveMoney";
                        List<Headers> headers = new List<Headers>();
                        lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                        CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                        lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");

                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"FAILED!!! {ex.Message}", 100, "");
            }


            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }
        public ActionResult MoovLNBBenin(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22999935800" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*500#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "500", ServiceID = "", serviceCode = "500", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(730, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = Api.CommonFuncations.USSD.LNBPariMoovUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
                }
                if (momo_request != null)
                {
                    momo_request.Delay = "5000";
                    string postBody = JsonConvert.SerializeObject(momo_request);
                    string url = "https://interface.lnbpari.com/api/DYAReceiveMoney";
                    List<Headers> headers = new List<Headers>();
                    lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                    CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                    lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");

                }
            }




            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNGuineaiDoBet(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "224664222412" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "551" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "343", ServiceID = "", serviceCode = "343", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(716, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MTNGuineaUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNGuineai142(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "224666010610" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "551" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "343", ServiceID = "", serviceCode = "343", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(716, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MTNGuinea142USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        



        public ActionResult GreenWinBtoBet(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22999992884" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*551#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();

                string spID = "5514", ServiceID = "", serviceCode = "5514", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(720, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = Api.CommonFuncations.USSDBtoBet.MoovUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        //string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNCameroonBtoBet(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "237671563200" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "*166#" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();

                string spID = "166", ServiceID = "", serviceCode = "166", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(720, ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = Api.CommonFuncations.USSDBtoBet.MTNCameroonUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        lines = Add2Log(lines, " SDP ussd_soap = " + ussd_soap, 100, "ivr_subscribe");
                        //string ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }


            
            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MoovBenin_STG(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22994558779" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "551" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "551", ServiceID = "", serviceCode = "551", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(715, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";
                        
                        ussd_soap = Api.CommonFuncations.USSD.MoovUSSDBehaviuer_STG(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                        
                        
                        
                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }




            lines = Write2Log(lines);
            return View();
        }

        public ActionResult CongoB_STG(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "242064661331" : Request.QueryString["MSISDN"]); 
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "104" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();

                string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(733, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MoovUSSDBehaviuer_STG(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNCongoB(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "242064661331" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "104" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();

                string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(733, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }



            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult MTNCongoB142(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");

            try
            {

                string session_id = (String.IsNullOrEmpty(id) ? "" : id);
                lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
                ViewBag.SessionID = session_id;
                ViewBag.Menu2Display = "";
                ViewBag.IsClose = false;

                string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "242068709481" : Request.QueryString["MSISDN"]);
                string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
                lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
                ViewBag.MSISDN = MSISDN;

                string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
                string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

                string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
                string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

                lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
                ViewBag.width_p = width_p;
                ViewBag.height_p = height_p;
                ViewBag.widthpx = width_p + "px";
                ViewBag.heightpx = height_p + "px";
                string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
                string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


                ViewBag.margin_top = margin_top;
                ViewBag.table_width = table_width;

                ViewBag.NewSessionID = Guid.NewGuid().ToString();

                if (!String.IsNullOrEmpty(session_id))
                {

                    string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "104" : Request.Form["ussdString"]);
                    lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                    ViewBag.ussdString = ussdString;


                    DYAReceiveMoneyRequest momo_request = null;

                    ServiceClass service = new ServiceClass();

                    string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                    service = GetServiceByServiceID(733, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                    USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            string menu_2_display = "";
                            bool is_close = false;
                            string ussd_soap = "";

                            ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                            ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                            lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                            ViewBag.IsClose = is_close;
                        }
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
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "ERROR - "+ ex.ToString(), 100, "ussd_mo");
            }

            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult AirTelCongoB142(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");

            try
            {
                string session_id = (String.IsNullOrEmpty(id) ? "" : id);
                lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
                ViewBag.SessionID = session_id;
                ViewBag.Menu2Display = "";
                ViewBag.IsClose = false;

                string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "+242050214436" : Request.QueryString["MSISDN"]);
                string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
                lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
                ViewBag.MSISDN = MSISDN;

                string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
                string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

                string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
                string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

                lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
                ViewBag.width_p = width_p;
                ViewBag.height_p = height_p;
                ViewBag.widthpx = width_p + "px";
                ViewBag.heightpx = height_p + "px";
                string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
                string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


                ViewBag.margin_top = margin_top;
                ViewBag.table_width = table_width;

                ViewBag.NewSessionID = Guid.NewGuid().ToString();

                string freeflowState = "FC";
                DYAReceiveMoneyRequest momo_request = null;
                string m_type = "1";

                if (!String.IsNullOrEmpty(session_id))
                {

                    string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "104" : Request.Form["ussdString"]);
                    lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                    ViewBag.ussdString = ussdString;

                    ServiceClass service = new ServiceClass();

                    string spID = "951753", ServiceID = "", serviceCode = "951753", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739"; //not sure about this line
                    service = GetServiceByServiceID(1101, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                    USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            string menu_2_display = "";
                            bool is_close = false;
                            string ussd_soap = "";
                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "BJ", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-198534595-2", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "CG", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "subscribe");

                            if (ussd_menu.is_maintenance)
                            {
                                if (ussd_menu.allowed_msisdns.Contains(MSISDN))
                                {
                                    ussd_soap = Api.CommonFuncations.USSD.AirtelCongoUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                                }
                                else
                                {
                                    menu_2_display = ussd_menu.maintenance_msg;
                                    is_close = true;
                                }
                            }
                            else
                            {
                                ussd_soap = Api.CommonFuncations.USSD.AirtelCongoUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            }


                            freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                            freeflowState = (is_close == true ? "FB" : "FC");
                            m_type = (is_close == true ? "2" : m_type);
                            menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");
                            ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                            lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                            ViewBag.IsClose = is_close;


                        }


                    }
                    if (momo_request != null)
                    {
                        momo_request.Delay = "5000";
                        string postBody = JsonConvert.SerializeObject(momo_request);
                        string url = "https://api125.ydplatform.com/api/DYAReceiveMoney";
                        List<Headers> headers = new List<Headers>();
                        lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                        CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                        lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "ERROR - " + ex.ToString(), 100, "ussd_mo");
            }


            ViewBag.Lines = lines;
            lines = Write2Log(lines);
            return View();
        }



        public ActionResult MoovBenin(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "USSDEmulator");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "USSDEmulator");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "USSDEmulator");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "USSDEmulator");


            string session_id = (String.IsNullOrEmpty(id) ? "" : id);
            lines = Add2Log(lines, "session_id = " + session_id, 100, "USSDEmulator");
            ViewBag.SessionID = session_id;
            ViewBag.Menu2Display = "";
            ViewBag.IsClose = false;

            string MSISDN_q = (String.IsNullOrEmpty(Request.QueryString["MSISDN"]) ? "22994558779" : Request.QueryString["MSISDN"]);
            string MSISDN = (String.IsNullOrEmpty(Request.Form["MSISDN"]) ? MSISDN_q : Request.Form["MSISDN"]);
            lines = Add2Log(lines, "MSISDN = " + MSISDN, 100, "USSDEmulator");
            ViewBag.MSISDN = MSISDN;

            string width_p_q = (String.IsNullOrEmpty(Request.QueryString["width_p"]) ? "400" : Request.QueryString["width_p"]);
            string height_p_q = (String.IsNullOrEmpty(Request.QueryString["height_p"]) ? "650" : Request.QueryString["height_p"]);

            string width_p = (String.IsNullOrEmpty(Request.Form["width_p"]) ? width_p_q : Request.Form["width_p"]);
            string height_p = (String.IsNullOrEmpty(Request.Form["height_p"]) ? height_p_q : Request.Form["height_p"]);

            lines = Add2Log(lines, "width_p = " + width_p + ";height_p = " + height_p, 100, "USSDEmulator");
            ViewBag.width_p = width_p;
            ViewBag.height_p = height_p;
            ViewBag.widthpx = width_p + "px";
            ViewBag.heightpx = height_p + "px";
            string margin_top = (Convert.ToInt32(height_p) * 0.2).ToString() + "px";
            string table_width = (Convert.ToInt32(width_p) * 0.8).ToString() + "px";


            ViewBag.margin_top = margin_top;
            ViewBag.table_width = table_width;

            ViewBag.NewSessionID = Guid.NewGuid().ToString();

            if (!String.IsNullOrEmpty(session_id))
            {

                string ussdString = (String.IsNullOrEmpty(Request.Form["ussdString"]) ? "551" : Request.Form["ussdString"]);
                lines = Add2Log(lines, "ussdString = " + ussdString, 100, "USSDEmulator");
                ViewBag.ussdString = ussdString;


                DYAReceiveMoneyRequest momo_request = null;

                ServiceClass service = new ServiceClass();
                string spID = "551", ServiceID = "", serviceCode = "551", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(715, ref lines);//GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null)
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        string menu_2_display = "";
                        bool is_close = false;
                        string ussd_soap = "";

                        ussd_soap = Api.CommonFuncations.USSD.MoovUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);



                        ViewBag.Menu2Display = (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display.Replace(Environment.NewLine, "<br>"));
                        lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                        ViewBag.IsClose = is_close;
                    }
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
            }




            lines = Write2Log(lines);
            return View();
        }


        public ActionResult ApproveMOMO()
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ApproveMOMO");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "ApproveMOMO");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "ApproveMOMO");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "ApproveMOMO");

            List<iDoDraftTicket> games_2_approve = GetDraftTickets(ref lines);
            
            ViewBag.Games2Approve = games_2_approve;
            lines = Write2Log(lines);
            return View();
        }

        public ActionResult DoApprove(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DoApprove");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "DoApprove");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "DoApprove");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "DoApprove");

            DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(id), "01", "Successfully processed transaction.", ref lines);
            if(dya_trans != null)
              {
                int response_code = 1000;
                //TODO check if decline is 02 
                ServiceClass service = new ServiceClass();
                service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, "01", ref lines);
                CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), id, dya_trans.partner_transid, "Successfully processed transaction.", response_code, ref lines);
            }
            lines = Write2Log(lines);
            return Redirect("/USSDEmulator/approvemomo");
        }
    }
}