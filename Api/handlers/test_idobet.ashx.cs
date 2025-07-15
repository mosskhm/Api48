using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for test_idobet
    /// </summary>
    public class test_idobet : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "test_iDoBet");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "test_iDoBet");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "test_iDoBet");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "test_iDoBet");
            string msisdn = context.Request.QueryString["msisdn"];

            string body = "{\"timeoutGuid\":null,\"timeout\":0,\"number\":null,\"authorizationKey\":null,\"authorizationTimeout\":0,\"validationResult\":[{\"orderBetValidationResults\":[{\"validationResult\":241,\"message\":\"Event 327965418 Event: 327965418 ,Odds 2, changed, old price :1.89 , new price 1.87 \",\"event\":{\"eventId\":327965418,\"eventStatus\":0,\"betTypeId\":310310,\"oddName\":\"2\",\"oddLine\":\"\",\"betTypeLine\":null,\"oddPrice\":1.87,\"oddStatus\":0,\"liveTime\":null,\"scoreHomeTeam\":null,\"scoreAwayTeam\":null,\"score\":null},\"extraData\":null,\"selections\":null}]}],\"result\":{\"errorDescription\":\"Bet Odds Price Changed\",\"additionalInfo\":null,\"errorCode\":241,\"resultCode\":1,\"errorCodeDescription\":null},\"isSuccessfull\":false,\"data\":null,\"dataStructure\":null,\"additionalData\":null,\"userInfo\":null}";
            dynamic json_response = JsonConvert.DeserializeObject(body);
            if (json_response.timeoutGuid == null)
            {
                foreach(var a in json_response.validationResult[0].orderBetValidationResults)
                {
                    if (a.validationResult == 241)
                    {
                        string event_id = a.@event.eventId;
                        string bet_type = a.@event.betTypeId;
                        string odd_name = a.@event.oddName;
                        string odd_price = a.@event.oddPrice;
                    }
                    else
                    {
                        //
                    }
                }
            }


                //string mail_body = "<p><h2>This is a test</h2></p>";
                //string mail_subject = "This is an Email Test";
                //string emails = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailRecipients", ref lines);
                //string sender_email = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderEmail", ref lines);
                //string sender_name = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderName", ref lines);
                //string sender_assword = Api.Cache.ServerSettings.GetServerSettings("iDoBetSenderPassword", ref lines);
                //int email_port = Convert.ToInt32(Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailPort", ref lines));
                //string email_host = Api.Cache.ServerSettings.GetServerSettings("iDoBetEmailHost", ref lines);
                //CommonFuncations.Email.SendEmail(mail_body, mail_subject, emails, sender_email, sender_name, sender_assword, email_port, email_host, ref lines);


                //string menu = GetSoccerLeagueMenu(ref lines);

                //string menu = GetCheckTicketsMenu(msisdn, 1, ref lines);
                //lines = Add2Log(lines, "menu = " + menu, 100, "test_iDoBet");
                //context.Response.Write("GetCheckTicketsMenu<br>");
                //context.Response.Write(menu + "<br>");
                //menu = GetCheckTicketMenu(msisdn, 1, "1", ref lines);
                //context.Response.Write("GetCheckTicketMenu<br>");
                //context.Response.Write(menu + "<br>");
                //lines = Add2Log(lines, "menu = " + menu, 100, "test_iDoBet");
                //string result = CommonFuncations.iDoBet.GetEventsMenu(31,1, ref lines);
                //Int64 game_id = 0;
                //USSDSession ussd_session = new USSDSession()
                //{
                //    action_id = 19,
                //    amount = "0",
                //    game_id = 315519082,
                //    selected_bet_type_id = 310310,
                //    selected_odd = 7,
                //    odd_page = 0,
                //    page_number = 1,
                //    selected_odd_line = "",
                //    selected_odd_name = "X",
                //    session_id = 1,
                //    user_seesion_id = "xxxx",
                //    topic_id = 2,
                //    ussd_string = "2000"
                //};
                //USSDMenu ussd_menu = GetUSSDMenu(1, "2000", 19, ussd_session, ref lines);


                lines = Write2Log(lines);
            
            context.Response.Write("ok");
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