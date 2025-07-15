using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.SV
{
    /// <summary>
    /// Summary description for call_back
    /// </summary>
    public class call_back : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "SharpVision_Callback");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "SharpVision_Callback");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "SharpVision_Callback");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "SharpVision_Callback");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "SharpVision_Callback");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "SharpVision_Callback");
            }

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string dya_transid = json_response.requestRef;
                    string status = json_response.status;
                    string StatusCode = (status == "Validated" ? "01" : "1050");

                    DYATransactions dya_trans = Api.DataLayer.DBQueries.UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, status, ref lines);
                    if (dya_trans != null)
                    {
                        int response_code = (StatusCode == "01" ? 1000 : 1050);
                        //TODO check if decline is 02 
                        ServiceClass service = new ServiceClass();
                        service = GetServiceByServiceID(dya_trans.service_id, ref lines);

                        string mytime = dya_trans.datetime;
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "ECW_debit_response");
                        if (service.hour_diff != "0")
                        {
                            mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                            dya_trans.datetime = mytime;
                        }
                        ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, response_code.ToString(), ref lines);

                        lines = Add2Log(lines, " mytime = " + mytime, 100, "ECW_debit_response");
                        if (dya_trans.dya_method == 2)
                        {
                            CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);
                        }
                        if (dya_trans.dya_method == 1)
                        {
                            Int64 delayed = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.id FROM delayed_withdraw_transactions d where d.service_id = " + dya_trans.service_id + " and d.msisdn = " + dya_trans.msisdn + " and d.amount = " + dya_trans.amount + " and d.transaction_id = '"+dya_trans.partner_transid+"' and dya_id = 0 limit 1", ref lines);
                            if (delayed > 0)
                            {
                                if (StatusCode == "01")
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update delayed_withdraw_transactions set dya_id = " + dya_transid + " where id = " + delayed, ref lines);
                                }
                                else
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update delayed_withdraw_transactions set last_failed_dya_id = " + dya_transid + " where id = " + delayed, ref lines);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
                }

            }
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"Response\":\"OK\"}");
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