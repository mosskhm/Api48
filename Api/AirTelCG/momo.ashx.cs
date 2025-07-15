using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.AirTelCG
{
    /// <summary>
    /// Summary description for momo
    /// </summary>
    public class momo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "AirTelCG_MOMO");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "AirTelCG_MOMO");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "AirTelCG_MOMO");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "AirTelCG_MOMO");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "AirTelCG_MOMO");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "AirTelCG_MOMO");
            }

            try
            {
                //{"transaction_date":"2023-11-15 13:30:26","transDATE":"2023-11-15 13:30:26","msisdn":"242056549450","amount":"100","currency":"CFA","status":"200","paymentID":"MP231115.1430.A69807","transaction_id":"123464","token":"15c7f9ef83bb11ee87dbac1f6be4442c"}
                //{"transaction_date":"2024-01-15 13:31:09","transDATE":"2024-01-15 13:31:09","msisdn":"242055174326","amount":"515","currency":"CFA","status":"200","paymentID":"MP240115.1431.A88663","transaction_id":"YELLOBET-158058989","token":"5033b571b3aa11eea787ac1f6be4442c"}
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                string dya_transid = "", ext_transaction_id = "", status = "";

                dya_transid = json_response.transaction_id;
                if (!String.IsNullOrEmpty(dya_transid))
                {
                    dya_transid = dya_transid.Replace("YELLOBET-", "");
                }
                status = json_response.status;
                int response_code = (status == "200" ? 1000 : 1050);
                ext_transaction_id = json_response.paymentID;
                DYATransactions dya_trans = null;
                string status_code = "";
                if (status == "200")
                {
                    dya_trans = Api.DataLayer.DBQueries.UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "01", status, ref lines);
                    string insert_query = "INSERT INTO yellowdot.dya_transaction_external_id (dya_id,external_id) VALUES(" + dya_trans.dya_trans + ",'" + ext_transaction_id.ToString() + "')";
                    Api.DataLayer.DBQueries.ExecuteQuery(insert_query, ref lines);
                    status_code = "01";
                }
                else
                {
                    dya_trans = Api.DataLayer.DBQueries.UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "1050", status, ref lines);
                    string insert_query = "INSERT INTO yellowdot.dya_transaction_external_id (dya_id,external_id) VALUES(" + dya_trans.dya_trans + "," + ext_transaction_id.ToString() + ")";
                    status_code = "1050";
                };

                ServiceClass service = new ServiceClass();
                service = GetServiceByServiceID(dya_trans.service_id, ref lines);

                string mytime = dya_trans.datetime;
                lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                if (service.hour_diff != "0")
                {
                    mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                    dya_trans.datetime = mytime;
                }
                ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, status_code, ref lines);
                CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);

                context.Response.Write("{\"result\":\"OK\"}");

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!!! Failed: reason = {ex.Message}", 100, "MOMO");
                context.Response.Write("{\"result\":\"FAILED\"}");
            }


            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";


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