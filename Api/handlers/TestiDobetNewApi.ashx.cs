using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for TestiDobetNewApi
    /// </summary>
    public class TestiDobetNewApi : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "TestiDoBetNewApi");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "TestiDoBetNewApi");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "TestiDoBetNewApi");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "TestiDoBetNewApi");

            context.Response.Write("Getting iDoBetToken<br>");
            string token_id = Api.Cache.iDoBet.GetiDoBetUserTokenNew(1, ref lines);
            context.Response.Write("token_id = " + token_id + "<br>");
            string action = HttpContext.Current.Request.QueryString["action"];
            string amount = HttpContext.Current.Request.QueryString["amount"];
            string msisdn = HttpContext.Current.Request.QueryString["msisdn"];
            if (action == "withdraw" && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(msisdn))
            {
                IdoBetUser user = SearchUserNew(msisdn, ref lines);
                string postbody = "", response_body = "";
                string transactionId = StartWithdrawNew(user, msisdn, Convert.ToInt32(amount), out postbody, out response_body, ref lines);
            }

            if (action == "deposit" && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(msisdn))
            {
                IdoBetUser user = SearchUserNew1(msisdn, ref lines);
            }

            if (action == "placebet" && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(msisdn))
            {

                List<SportEvents> events = Api.Cache.iDoBet.GetEventsFromCacheNew(31, ref lines);
                List<SavedGames> saved_g = Api.DataLayer.DBQueries.GetiDoBetSavedGamesTest(ref lines);
                List<SavedGames> selected_saved_d = new List<SavedGames>();
                int counter = 0;
                foreach (SavedGames s in saved_g)
                {
                    SportEvents e = events.Find(x => x.game_id.ToString() == s.game_id);
                    if (e != null)
                    {
                        selected_saved_d.Add(s);
                        counter = counter + 1;
                    }
                    if (counter == 3)
                    {
                        break;
                    }
                }

                string session_id = "Test_" + Guid.NewGuid().ToString();
                foreach (SavedGames s in selected_saved_d)
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into ussd_saved_games (msisdn,game_id,odd_page,selected_ussd_string,date_time,status,user_session_id,selected_odd,selected_bet_type_id,selected_odd_name,selected_odd_line,amount) " +
                        "values("+ msisdn + "," + s.game_id + ",0,2,now(),0,'" + session_id + "'," + s.selected_odd + "," + s.selected_bet_type_id + ",'" + s.selected_odd_name + "','" + s.selected_odd_line + "',"+ amount + ")", ref lines);
                }
                USSDSession ussd_session = new USSDSession()
                {
                    user_seesion_id = session_id
                };

                bool placebet = PlaceBet(ussd_session, ref lines);

                DYATransactions dya_trans = new DYATransactions()
                {
                    partner_transid = session_id,
                    dya_trans = 123987,
                    datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string postBody = "", response_body = "";
                ExecuteOrderDetails exe_order = GetExecuteOrderNew(dya_trans, out postBody, out response_body, ref lines);
                if (exe_order != null)
                {
                    context.Response.Write("barcode = " + exe_order.barcode);
                }
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