using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.LN
{
    /// <summary>
    /// Summary description for airtime_winner
    /// </summary>
    public class airtime_winner : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "LNairtimeWinners");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "LNairtimeWinners");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "LNairtimeWinners");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "LNairtimeWinners");
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "LNairtimeWinners");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "LNairtimeWinners");
            }
            //ClientEndpoint?type={}&date={}action={}&msisdn={}network={}guid={}&subscriberid={}&ref={}
            Int64 transaction_id = -1;
            string airtime_winner_id = "";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    string msisdn = json_response.msisdn;
                    lines = Add2Log(lines, " msisdn = " + msisdn, 100, "LNairtimeWinners");
                    string amount = json_response.amount;
                    lines = Add2Log(lines, " amount = " + amount, 100, "LNairtimeWinners");
                    string service_id = json_response.service_id;
                    lines = Add2Log(lines, " service_id = " + service_id, 100, "LNairtimeWinners");
                    string date_time = json_response.date_time;
                    lines = Add2Log(lines, " date_time = " + date_time, 100, "LNairtimeWinners");
                    string dya_id = json_response.dya_id;
                    lines = Add2Log(lines, " dya_id = " + dya_id, 100, "LNairtimeWinners");


                    if (!String.IsNullOrEmpty(service_id) && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(date_time) && !String.IsNullOrEmpty(dya_id))
                    {

                        string subscriber_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service_id + " order by subscriber_id desc limit 1", "DBConnectionString_32", ref lines);
                        subscriber_id = (String.IsNullOrEmpty(subscriber_id) ? "0" : subscriber_id);
                        airtime_winner_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT law.id from yellowdot.ln_airtime_winners law where law.msisdn = " + msisdn + " AND law.service_id = " + service_id + " AND law.date_time = '" + date_time + "' AND law.airtime_amount = " + amount + " LIMIT 1;", ref lines);
                        if (String.IsNullOrEmpty(airtime_winner_id))
                        {
                            transaction_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into ln_airtime_winners (msisdn, service_id, airtime_amount, date_time, subscriber_id, dya_id) values(" + msisdn + "," + service_id + "," + amount + ",'" + date_time + "', " + subscriber_id + ", " + dya_id + ")", ref lines);
                        }

                        lines = Add2Log(lines, " transaction_id = " + transaction_id, 100, "LNairtimeWinners");

                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception  = " + ex.ToString(), 100, "LNairtimeWinners");
                }
            }

            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"ok\", \"transaction_id\":" + transaction_id + "}");
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