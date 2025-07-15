using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for MTNOpenAPI
    /// </summary>
    public class MTNOpenAPI : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "MTNOpenAPI");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MTNOpenAPI");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MTNOpenAPI");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MTNOpenAPI");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MTNOpenAPI");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "REFERER = " + key + ": " + context.Request.Form[key], 100, "MTNOpenAPI");
            }
            try
            {
                if (!String.IsNullOrEmpty(xml))
                {
                    //2020-10-29 13:25:47.567: Incomming XML = {"financialTransactionId":"1884710816","externalId":"20950601","amount":"50","currency":"XAF","payer":{"partyIdType":"MSISDN","partyId":"237683746252"},"payeeNote":"string","status":"SUCCESSFUL"}
                    //2020-10-29 13:17:45.554: Incomming XML = {"financialTransactionId":"1884670102","externalId":"20950173","amount":"20","currency":"XAF","payer":{"partyIdType":"MSISDN","partyId":"237683746252"},"payeeNote":"string","status":"FAILED","reason":"INTERNAL_PROCESSING_ERROR"}
                    //2021-05-27 11:21:24.579: Incomming XML = {"financialTransactionId":"2573677780","externalId":"30860324","amount":"200","currency":"XAF","payer":{"partyIdType":"MSISDN","partyId":"237676156428"},"payeeNote":"string","status":"SUCCESSFUL"}


                    dynamic json_response = JsonConvert.DeserializeObject(xml);

                    string dya_transid = json_response.externalId; 
                    lines = Add2Log(lines, " dya_transid = " + dya_transid, 100, "MOMO");

                    string status = json_response.status;  
                    lines = Add2Log(lines, " status = " + status, 100, "MOMO");

                    string financialTransactionId = json_response.financialTransactionId;  
                    lines = Add2Log(lines, " financialTransactionId = " + financialTransactionId, 100, "MOMO");

                    int response_code = 0;
                    string status_code = "";

                    DYATransactions dya_trans = null;
                    string check_res = Api.DataLayer.DBQueries.SelectQueryReturnString("select d.result FROM dya_transactions d WHERE d.dya_id = " + dya_transid, ref lines);
                    lines = Add2Log(lines, " Result before update " + check_res, 100, "MOMO");
                    bool cont = false;
                    if (check_res == "-1" || check_res == "1010")
                    {
                        cont = true;
                    }


                    switch (status)
                    {
                        case "SUCCESSFUL":
                            dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "01", status, ref lines);
                            response_code = 1000;
                            status_code = "01";
                            break;
                        case "FAILED":
                            string reason = json_response.reason;
                            lines = Add2Log(lines, " reason = " + reason, 100, "MOMO");
                            dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "500", status + " - " + reason, ref lines);
                            status_code = "500";
                            response_code = 1050;
                            break;
                    }

                    
                    ServiceClass service = new ServiceClass();
                    service = GetServiceByServiceID(dya_trans.service_id, ref lines);

                    string mytime = dya_trans.datetime;
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (service.hour_diff != "0")
                    {
                        mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                        dya_trans.datetime = mytime;
                    }
                    if (dya_trans.dya_method == 2)
                    {
                        if (cont)
                        {
                            ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, status_code, ref lines);
                        }
                        else
                        {
                            lines = Add2Log(lines, " looks like transaction was already updated " , 100, "MOMO");
                        }
                        

                        if (!String.IsNullOrEmpty(financialTransactionId))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_transid + ",'" + financialTransactionId + "');", ref lines);
                        }

                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);
                    }

                }
            }
            catch(Exception ex)
            {
                lines = Add2Log(lines, " exception = " + ex.ToString(), 100, "MOMO");
            }
            

            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";
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