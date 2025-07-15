using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for handle_billing
    /// </summary>
    public class handle_billing : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "handle_orange_billing");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "handle_orange_billing");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "handle_orange_billing");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "handle_orange_billing");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "handle_orange_billing");
            
            if(!string.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string id = json_response.transactionId;
                    if (!String.IsNullOrEmpty(id))
                    {
                        transactions trans = GetPendingTransactions(id, ref lines);
                        if (trans != null)
                        {
                            DateTime dstart = DateTime.Now;
                            string StatusCode = (trans.status == 1 ? "01" : "1050");
                            string StatusDesc = (trans.status == 1 ? "Success" : "Failed");
                            string transaction_id = (trans.d_type == 1 ? trans.session_id : "iDoBetDeposit_" + Guid.NewGuid().ToString().Substring(0, 10).Replace("-", ""));
                            transaction_id = (trans.d_type == 3 ? "Rapidos_" + trans.session_id : transaction_id);
                            bool cont = true;
                            if (cont)
                            {
                                Int64 dya_transid = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) values(" + trans.msisdn + ", " + trans.service_id + ", now(), " + trans.amount + "," + StatusCode + ",'" + StatusDesc + "',2,'" + transaction_id + "')", ref lines);
                                if (dya_transid > 0)
                                {
                                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                                    if (dya_trans != null)
                                    {
                                        int response_code = (StatusCode == "01" ? 1000 : 1050);
                                        ServiceClass service = new ServiceClass();
                                        service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                                        string mytime = dya_trans.datetime;
                                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                                        if (service.hour_diff != "0")
                                        {
                                            mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                                            dya_trans.datetime = mytime;
                                        }
                                        List<Int64> saved_ids = GetUSSDSavedGamesID(dya_trans.partner_transid, ref lines);
                                        if (saved_ids != null)
                                        {
                                            foreach (Int64 s in saved_ids)
                                            {
                                                //DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set barcode = '" + barcode + "', `status` = 2 where id = " + s, ref lines);
                                                Api.DataLayer.DBQueries.ExecuteQuery("update ussd_saved_games set amount = " + trans.amount + " where id = " + s, ref lines);
                                            }
                                        }



                                        if (trans.d_type == 1)
                                        {
                                            USSDSession ussd_session = new USSDSession()
                                            {
                                                user_seesion_id = trans.session_id
                                            };
                                            try
                                            {
                                                bool req_for_order = Api.CommonFuncations.iDoBetOrangeGuinea.PlaceBet(ussd_session, ref lines);
                                            }
                                            catch (Exception ex)
                                            {
                                                lines = Add2Log(lines, " exception PlaceBet" + ex.ToString(), 100, "MOMO");
                                            }

                                        }


                                        try
                                        {
                                            ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, StatusCode, ref lines);
                                        }
                                        catch (Exception ex)
                                        {
                                            lines = Add2Log(lines, " exception DecideBehaviorMOMO " + ex.ToString(), 100, "MOMO");
                                        }

                                        if (!String.IsNullOrEmpty(trans.billing_id))
                                        {
                                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_transid + ",'" + trans.billing_id + "');", ref lines);
                                        }
                                        Api.DataLayer.DBQueries.ExecuteQuery("update orange_billing_requests set is_complete = 1 where id = " + trans.id, "DBConnectionString_161", ref lines);
                                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                                    }
                                }
                            }
                            DateTime dend = DateTime.Now;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                }
            }

            lines = Write2Log(lines);

            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
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