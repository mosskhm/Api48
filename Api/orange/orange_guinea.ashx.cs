using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for orange_guinea
    /// </summary>
    public class orange_guinea : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "orange_guinea");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "orange_guinea");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "orange_guinea");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "orange_guinea");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "orange_guinea");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "orange_guinea");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
            bool cont = true;
            if (String.IsNullOrEmpty(auth))
            {
                context.Response.Status = "403 Forbidden";
                context.Response.StatusCode = 403;
                context.ApplicationInstance.CompleteRequest();
                cont = false;
            }
            if (cont)
            {
                if (auth.ToLower().Contains("basic "))
                {
                    //ServiceID732:OrangeGuinea U2VydmljZUlENzMyOk9yYW5nZUd1aW5lYQ==
                    string[] words = auth.Split(' ');
                    lines = Add2Log(lines, "Auth = " + words[1], 100, "orange_guinea");
                    
                    //{"status":"FAILED","transactionData":{"transactionId":"25514318","type":"CASHIN","peerId":"622178782","amount":1000,"currency":"GNF","posId":"VdKTa1h","creationDate":1612956718390},"message":"Transaction identique."}
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    try
                    {
                        string transactionId = json_response.transactionData.transactionId;
                        string status = json_response.status;
                        string message = json_response.message;
                        string txnId = json_response.transactionData.txnId;
                        string msisdn = json_response.transactionData.peerId;

                        string StatusCode = (status == "SUCCESS" ? "01" : "1050");
                        string StatusDesc = message.Replace("'", "''");
                        DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(transactionId), StatusCode, StatusDesc, ref lines);
                        int response_code = (StatusCode == "01" ? 1000 : 1050);

                        if (status == "FAILED")
                        {
                            lines = Add2Log(lines, "Sleeping for 3 sec ", 100, "orange_guinea");
                            Thread.Sleep(3000);
                            Api.DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result_desc = '"+message.Replace("'","''")+"' where dya_id = " + transactionId, ref lines);
                        }
                        if (status == "SUCCESS")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '01', result_desc = 'SUCCESS' where dya_id = " + transactionId, ref lines);
                            if (!String.IsNullOrEmpty(msisdn))
                            {
                                string partner_transid = dya_trans.partner_transid;// Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT d.transaction_id FROM dya_transactions d WHERE d.dya_id = " + transactionId, ref lines);
                                if (!String.IsNullOrEmpty(partner_transid) && dya_trans.dya_method == 1)
                                {
                                    //string delayed_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT d.id FROM delayed_withdraw_transactions d WHERE d.service_id = 732 AND d.is_blocked = 0 AND d.dya_id = 0 AND d.transaction_id = '"+partner_transid+"' AND d.msisdn = 224"+msisdn+" LIMIT 1", ref lines);
                                    string delayed_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT t.id FROM (SELECT * FROM delayed_withdraw_transactions dw ORDER BY dw.id DESC LIMIT 100000) t WHERE t.transaction_id = '"+ partner_transid + "' AND t.msisdn = 224"+msisdn+ " AND t.is_blocked = 0 LIMIT 1", ref lines);
                                    if (!String.IsNullOrEmpty(delayed_id))
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("update delayed_withdraw_transactions set dya_id = " + transactionId + " where id = " + delayed_id, ref lines);
                                    }
                                }
                            }
                        }
                        
                        if (dya_trans.dya_method == 2)
                        {
                            ServiceClass service = new ServiceClass();
                            service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                            string mytime = dya_trans.datetime;
                            if (service.hour_diff != "0")
                            {
                                mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                                dya_trans.datetime = mytime;
                            }

                            CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), transactionId, dya_trans.partner_transid, StatusDesc, response_code, mytime, ref lines);
                        }
                        

                        if (!String.IsNullOrEmpty(transactionId) && !String.IsNullOrEmpty(txnId))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values("+transactionId+",'"+txnId+"')", ref lines);
                            if (status == "SUCCESS")
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '01', result_desc = 'SUCCESS' where dya_id = " + transactionId, ref lines);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");

                    }
                }
                else
                {
                    cont = false;
                }
            }

            lines = Write2Log(lines);

            if (cont)
            {
                context.Response.ContentType = "application/json";
                context.Response.Write("ok");
            }
            
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