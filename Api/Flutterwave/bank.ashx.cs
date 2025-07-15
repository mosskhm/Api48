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
    /// Summary description for bank
    /// </summary>
    public class bank : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            bool send_notification = true;

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "flutterwave_bank");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "flutterwave_bank");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "flutterwave_bank");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "flutterwave_bank");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "flutterwave_bank");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "flutterwave_bank");

            dynamic json_response = JsonConvert.DeserializeObject(xml);
            string dya_transid = "", reason = "", status_code = "";
            int response_code = 0;
            if (xml.Contains("transfer.completed"))
            {
                dya_transid = json_response.data.reference;
                reason = json_response.data.status;
            }

            if (xml.Contains("charge.completed"))
            {
                dya_transid = json_response.data.tx_ref;
                reason = json_response.data.status;
            }

            //{"event":"charge.completed","data":{"id":768910083,"tx_ref":"77566079","flw_ref":"YELLOWDOTAFRICA/APDF78271669221965435160","device_fingerprint":"87193ab35058bb92fe125b4b8bd8a897","amount":200,"currency":"NGN","charged_amount":200,"app_fee":2.8,"merchant_fee":0,"processor_response":"Approved by Financial Institution","auth_model":"PIN","ip":"197.210.8.172","narration":"CARD Transaction ","status":"successful","payment_type":"card","created_at":"2022-11-23T16:46:04.000Z","account_id":760545,"customer":{"id":469588734,"name":"Akinbiyi Usman Olanrewaju","phone_number":null,"email":"faine@yellowdotafrica.com","created_at":"2022-11-23T16:46:04.000Z"},"card":{"first_6digits":"539983","last_4digits":"8728","issuer":"MASTERCARD GUARANTY TRUST BANK Mastercard Naira Debit Card","country":"NG","type":"MASTERCARD","expiry":"10/23"}},"event.type":"CARD_TRANSACTION"}
            //2022-11-20 20:32:40.912: Incomming XML = {"event":"charge.completed","data":{"id":766882393,"tx_ref":"77038923","flw_ref":"URF_1668969156671_3318435","device_fingerprint":"d9dcdde6d4b286a103d5d4cf65fae642","amount":200,"currency":"NGN","charged_amount":200,"app_fee":2.8,"merchant_fee":0,"processor_response":"Value '450074xxxxxx9275' is invalid. The combination of currency, card type and transaction type i","auth_model":"VBVSECURECODE","ip":"63.250.57.75","narration":"CARD Transaction ","status":"failed","payment_type":"card","created_at":"2022-11-20T18:32:37.000Z","account_id":760545,"customer":{"id":467986640,"name":"Oren Ein Gal","phone_number":null,"email":"oreneingal@gmail.com","created_at":"2022-11-20T18:32:36.000Z"},"card":{"first_6digits":"450074","last_4digits":"9275","issuer":"VISA ZENITH BANK PLC CREDITPREMIER","country":"NG","type":"VISA","expiry":"08/26"}},"event.type":"CARD_TRANSACTION"}


            bool cont = false;
            
            if (!String.IsNullOrEmpty(dya_transid) && (!String.IsNullOrEmpty(reason)))
            {
                string check_res = Api.DataLayer.DBQueries.SelectQueryReturnString("select d.result FROM dya_transactions d WHERE d.dya_id = " + dya_transid, ref lines);
                lines = Add2Log(lines, " Result before update " + check_res, 100, "MOMO");
                cont = false;
                if (check_res == "-1" || check_res == "1015")
                {
                    cont = true;
                }
            }

            try
            {
                DYATransactions dya_trans = null;
                string status = json_response.data.status;
                string processor_response = json_response.data.processor_response;
                processor_response = String.IsNullOrEmpty(processor_response) ? "" : processor_response;
                if (status.ToLower() == "successful")
                {
                     dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "01", status, ref lines);
                    string delayed_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT d.id FROM delayed_withdraw_transactions d WHERE d.service_id = " + dya_trans.service_id + " AND d.last_failed_dya_id = " + dya_transid + " AND d.dya_id = 0", ref lines);
                    if (!String.IsNullOrEmpty(delayed_id))
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("update delayed_withdraw_transactions set dya_id = " + dya_transid + " where id = " + delayed_id, ref lines);
                    }
                    status_code = "01";
                    response_code = 1000;
                }
                else if (status.ToLower() == "failed" && processor_response.ToLower() == "transaction is currently processing. you will be notified once it's completed")
                {
                    lines = Add2Log(lines, " reason = " + reason, 100, "MOMO");
                    dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "500", status + " - " + reason, ref lines);
                    status_code = "500";
                    response_code = 1050;
                    send_notification = false;
                }
                //switch (status)
                //{
                //    case "successful":
                //    case "SUCCESSFUL":
                //        dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "01", status, ref lines);
                //        string delayed_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT d.id FROM delayed_withdraw_transactions d WHERE d.service_id = " + dya_trans.service_id + " AND d.last_failed_dya_id = " + dya_transid + " AND d.dya_id = 0", ref lines);
                //        if (!String.IsNullOrEmpty(delayed_id))
                //        {
                //            Api.DataLayer.DBQueries.ExecuteQuery("update delayed_withdraw_transactions set dya_id = " + dya_transid + " where id = " + delayed_id, ref lines);
                //        }
                //        status_code = "01";
                //        response_code = 1000;
                //        break;
                //    case "failed":
                //    case "FAILED":
                //        lines = Add2Log(lines, " reason = " + reason, 100, "MOMO");
                //        dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), "500", status + " - " + reason, ref lines);
                //        status_code = "500";
                //        response_code = 1050;
                //        break;
                //}


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
                    if (send_notification)
                    {
                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), my_service, dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);
                    }
                    
                }


            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
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