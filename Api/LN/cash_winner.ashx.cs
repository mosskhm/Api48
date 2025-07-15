using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.LN
{
    /// <summary>
    /// Summary description for cash_winner
    /// </summary>
    public class cash_winner : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "LNCashWinners");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "LNCashWinners");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "LNCashWinners");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "LNCashWinners");
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "LNCashWinners");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "LNCashWinners");
            }
            //ClientEndpoint?type={}&date={}action={}&msisdn={}network={}guid={}&subscriberid={}&ref={}
            Int64 transaction_id = -1;
            string cash_winner_id = "";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    string msisdn = json_response.msisdn;
                    lines = Add2Log(lines, " msisdn = " + msisdn, 100, "LNCashWinners");
                    string amount = json_response.amount;
                    lines = Add2Log(lines, " amount = " + amount, 100, "LNCashWinners");
                    string service_name = json_response.service_name;
                    lines = Add2Log(lines, " service_name = " + service_name, 100, "LNCashWinners");
                    string date_time = json_response.date_time;
                    lines = Add2Log(lines, " date_time = " + date_time, 100, "LNCashWinners");
                    

                    if (!String.IsNullOrEmpty(service_name) && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(date_time))
                    {
                        String service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT s.service_id FROM yellowdot.services s WHERE s.service_name LIKE '%" + service_name+"%' limit 1", "DBConnectionString_32", ref lines);
                        service_id = (String.IsNullOrEmpty(service_id) ? Api.DataLayer.DBQueries.SelectQueryReturnString("select service_id from services where service_name = '" + service_name + "' limit 1", "DBConnectionString_32", ref lines) : service_id);
                        if (!String.IsNullOrEmpty(service_id))
                        {
                            string subscriber_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service_id +" order by subscriber_id desc limit 1" , "DBConnectionString_32", ref lines);
                            subscriber_id = (String.IsNullOrEmpty(subscriber_id) ? "0" : subscriber_id);
                            cash_winner_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT lcw.id from yellowdot.ln_cash_winners lcw where lcw.msisdn = " + msisdn + " AND lcw.service_id = " + service_id + " AND lcw.date_time = " + date_time + " AND lcw.cash_amount = " + amount + ";", ref lines);
                            if (String.IsNullOrEmpty(cash_winner_id))
                            {
                                transaction_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into ln_cash_winners (msisdn, service_id, cash_amount, date_time, subscriber_id, dya_id) values(" + msisdn + "," + service_id + "," + amount + ",'" + date_time + "', " + subscriber_id + ", 0)", ref lines);
                            }
                            
                            lines = Add2Log(lines, " transaction_id = " + transaction_id, 100, "LNCashWinners");
                        }
                    }
                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, " Exception  = " + ex.ToString(), 100, "LNCashWinners");
                }
            }

            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"ok\", \"transaction_id\":"+ transaction_id + "}");
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