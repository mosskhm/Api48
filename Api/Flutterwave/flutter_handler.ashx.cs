using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Flutterwave
{
    /// <summary>
    /// Summary description for flutter_handler
    /// </summary>
    public class flutter_handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "flutterwave_handler");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "flutterwave_handler");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "flutterwave_handler");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "flutterwave_handler");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "flutterwave_handler");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "flutterwave_handler");
            string auth = context.Request.ServerVariables["verif-hash"];
            bool cont = true;
            //if (String.IsNullOrEmpty(auth))
            //{
            //    context.Response.Status = "403 Forbidden";
            //    context.Response.StatusCode = 403;
            //    context.ApplicationInstance.CompleteRequest();
            //    cont = false;
            //}
            //2022-01-09 15:26:50.218: Incomming XML = {"event":"charge.completed","data":{"id":562412944,"tx_ref":"1111","flw_ref":"BKYA9879516417308123","device_fingerprint":"N/A","amount":200,"currency":"XAF","charged_amount":200,"app_fee":7,"merchant_fee":0,"processor_response":"Transaction Failed","auth_model":"AUTH","ip":"52.18.161.235","narration":"yellowbet","status":"failed","payment_type":"mobilemoneysn","created_at":"2022-01-09T12:20:12.000Z","account_id":1402432,"customer":{"id":326437680,"name":"Anonymous customer","phone_number":"237675263665","email":"675263665@flw.com","created_at":"2022-01-09T12:20:12.000Z"}},"event.type":"MOBILEMONEYSN_TRANSACTION"}
            //2022-01-09 16:33:30.989: Incomming XML = {"event":"transfer.completed","event.type":"Transfer","data":{"id":19015073,"account_number":"698492329","bank_name":"FA-BANK","bank_code":"FMM","fullname":"Thomas","created_at":"2022-01-09T13:14:38.000Z","currency":"XAF","debit_currency":"XAF","amount":165,"fee":3.3000000000000003,"status":"FAILED","reference":"1116","meta":null,"narration":"Test","approver":null,"complete_message":"DISBURSE FAILED: Insufficient funds in customer wallet","requires_approval":0,"is_approved":1}}


            if (cont)
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                string dya_transid = "", reason = "", status_code = "";
                int response_code = 0;
                if (xml.Contains("charge.completed"))
                {
                    dya_transid = json_response.data.tx_ref;
                    reason = json_response.data.processor_response;
                }
                else
                {
                    dya_transid = json_response.data.reference;
                    reason = json_response.data.complete_message;
                }

                
                string check_res = Api.DataLayer.DBQueries.SelectQueryReturnString("select d.result FROM dya_transactions d WHERE d.dya_id = " + dya_transid, ref lines);
                lines = Add2Log(lines, " Result before update " + check_res, 100, "MOMO");
                cont = false;
                if (check_res == "-1" || check_res == "1010")
                {
                    cont = true;
                }

                

                try
                {
                    DYATransactions dya_trans = null;
                    string status = json_response.data.status;
                    switch (status)
                    {
                        case "successful":
                        case "SUCCESSFUL":
                            dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "01", status, ref lines);
                            status_code = "01";
                            response_code = 1000;
                            break;
                        case "failed":
                        case "FAILED":
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
                            lines = Add2Log(lines, " looks like transaction was already updated ", 100, "MOMO");
                        }
                        string my_service = dya_trans.service_id.ToString();
                        if (my_service == "754")
                        {
                            my_service = "720";
                        }
                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), my_service, dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);
                    }

                    
                }     
                catch(Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
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