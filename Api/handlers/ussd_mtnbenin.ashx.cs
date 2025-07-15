using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ussd_mtnbenin
    /// </summary>
    public class ussd_mtnbenin : IHttpHandler
    {

        

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ussd_mtnbenin");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_mo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_mtnbenin");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_mtnbenin");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_mtnbenin");

            string MSISDN = "", ussdString = "", time_stamp = "";
            string ussd_soap = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;

            if (!String.IsNullOrEmpty(xml))
            {
                MSISDN = ProcessXML.GetXMLNode(xml, "msisdn", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mtnbenin");

                ussdString = ProcessXML.GetXMLNode(xml, "subscriberInput", ref lines);
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mtnbenin");

                time_stamp = ProcessXML.GetXMLNode(xml, "dateFormat", ref lines);
                lines = Add2Log(lines, " time_stamp = " + time_stamp, 100, "ussd_mtnbenin");

                ServiceClass service = new ServiceClass();
                string spID = "2290110011184", ServiceID = "", serviceCode = "365", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                service = GetServiceByServiceID(682, ref lines);
                //service = GetServiceInfo(spID, ServiceID, "", ref lines);
                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                if (umc != null && !xml.Contains("cleanup"))
                {
                    lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                    int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                    USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                    if (ussd_menu != null)
                    {
                        GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "BJ", "pageview", "", "", "", "/"+ussd_menu.action_name,  ref lines);

                        lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                        ussd_soap = handlers.ussd_mo.USSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                        freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                        freeflowState = (is_close == true ? "FB" : "FC");
                        menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0,menu_2_display.Length -1) : "");
                        
                    }
                }

                if (xml.Contains("cleanup"))
                {
                    lines = Add2Log(lines, " its clean up time " + MSISDN, 100, "ussd_mtnbenin");
                    menu_2_display = "";
                    freeflowState = "FB";
                    USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);
                    if (ussd_session != null)
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("update ussd_sessions set `status` = 1 where session_id = " + ussd_session.session_id, "DBConnectionString_104", ref lines);
                    }
                        

                    //USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, 2, ref lines);
                    //if (ussd_session != null)
                    //{
                    //    Api.DataLayer.DBQueries.ExecuteQuery("update ussd_sessions set `status` = 1 where user_session_id = '" + ussd_session.user_seesion_id + "'", ref lines);
                    //}
                    //else
                    //{
                    //    Api.DataLayer.DBQueries.ExecuteQuery("update ussd_sessions set `status` = 1 where msisdn = " + MSISDN, ref lines);
                    //}
                }
            }

            

            if (!String.IsNullOrEmpty(menu_2_display))
            {
                if (menu_2_display.Length > 160)
                {
                    lines = Add2Log(lines, "Menu too long", 100, "ussd_mo");
                    string mail_body = "<p><h2>Menu too long</h2><b>MSISDN:</b> " + MSISDN + "<br><b>Menu:</b><br><pre>" + menu_2_display + "</pre><br><h1>We need to shorten something</h1></p>";
                    string mail_subject = "Menu too long for user - " + MSISDN;
                    string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                    string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                    string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                    string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                    int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                    string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                    //CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);
                    menu_2_display = menu_2_display.Substring(0, 160);
                }
            }

            string soap = "";
            soap = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";
            soap = soap + "<response>";
            soap = soap + "<msisdn>"+ MSISDN + "</msisdn>";
            soap = soap + "<applicationResponse>"+ menu_2_display + "</applicationResponse>";
            //soap = soap + "<appDrivenMenuCode>menuCode</appDrivenMenuCode>";
            soap = soap + "<freeflow>";
            soap = soap + "<freeflowState>"+ freeflowState + "</freeflowState>";
            soap = soap + "<freeflowCharging>N</freeflowCharging>";
            soap = soap + "<freeflowChargingAmount>0</freeflowChargingAmount>";
            soap = soap + "</freeflow>";
            soap = soap + "</response>";
            lines = Add2Log(lines, "soap = " + soap, 100, "ussd_mtnbenin");
            
            if (momo_request != null)
            {
                momo_request.Delay = "5000";
                string postBody = JsonConvert.SerializeObject(momo_request);
                string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                List<Headers> headers = new List<Headers>();
                lines = Add2Log(lines, "Sending momo request async with delay " , 100, "ussd_mo");
                CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
            }
            context.Response.ContentType = "text/xml";
            context.Response.Write(soap);
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