using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.PiSi
{
    /// <summary>
    /// Summary description for airtime_billing_cb
    /// </summary>
    public class airtime_billing_cb : IHttpHandler
    {
        
        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "pisi_airtimebilling_cb");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "pisi_airtimebilling_cb");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "pisi_airtimebilling_cb");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "pisi_airtimebilling_cb");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "pisi_airtimebilling_cb");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "pisi_airtimebilling_cb");
            }

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                string msisdn = json_response.msisdn;
                string charge_amount = json_response.amount;
                string subscription_id = json_response.pisipid;
                string StatusDesc = json_response.updatetype;
                string dya_transid = json_response.trxid;
                string StatusCode = "";


                //{"network":"MTNN","aggregator":"Pisi Mobile Services","trxid":"79046303","msisdn":"2347048130055","amount":"60.0","pisisid":32,"pisipid":74,"chargedate":"2022-11-30 17:37:40","updatetype":"charged","channel":"USSD","package":"234102200006424 | 23410220000022678"}
                if (StatusDesc == "charged")
                {
                    StatusCode = "01";
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                    if (dya_trans != null)
                    {
                        int response_code = (StatusCode == "01" ? 1000 : 1050);
                        //TODO check if decline is 02 
                        ServiceClass service = new ServiceClass();
                        service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                        string mytime = dya_trans.datetime;
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        if (service.hour_diff != "0")
                        {
                            mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                            dya_trans.datetime = mytime;
                        }
                        ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, StatusCode, ref lines);
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, StatusDesc, response_code, mytime, ref lines);

                    }
                }
                else
                {
                    StatusCode = "1050";
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                }

            }

            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"OK\"}");




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