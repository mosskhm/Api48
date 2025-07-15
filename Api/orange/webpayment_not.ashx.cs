using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for webpayment_not
    /// </summary>
    public class webpayment_not : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Orange_WebPaymentNot");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "Orange_WebPaymentNot");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Orange_WebPaymentNot");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "Orange_WebPaymentNot");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "sub_callbacks");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "sub_callbacks");
            }
            string dya_id_str = context.Request.QueryString["dya_id"];

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);

//                {
//                    "status":"SUCCESS",
                //   "notif_token":"dd497bda3b250e536186fc0663f32f40",
                //   "txnid": "MP150709.1341.A00073"
                //}
                try
                {
                    string status = json_response.status;
                    string notif_token = json_response.notif_token;
                    string txnid = json_response.txnid;

                    string notif_token_check = Api.DataLayer.DBQueries.SelectQueryReturnString("select notif_token from dya_webpay_data where dya_id = " + dya_id_str , ref lines);
                    Int64 dya_id = (notif_token_check == notif_token ? Convert.ToInt64(dya_id_str) : 0);
                    int response_code = (status == "SUCCESS" ? 1000 : 1050);
                    string status_code = (status == "SUCCESS" ? "01" : "500");

                    if (dya_id > 0)
                    {
                        DYATransactions dya_trans = null;
                        if (status == "SUCCESS")
                        {
                            dya_trans = UpdateGetDYAReciveTrans(dya_id, "01", status, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + txnid + "');", ref lines);
                        }
                        else
                        {
                            dya_trans = UpdateGetDYAReciveTrans(dya_id, "1050", status, ref lines);
                        }
                        if (dya_trans != null)
                        {
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

                            CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_id.ToString(), dya_trans.partner_transid, status, response_code, mytime, ref lines);
                        }
                            
                    }

                    

                    if (status == "SUCCESS")
                    {
                        Api.DataLayer.DBQueries.UpdateDYATrans(dya_id, "01", "Success", ref lines);
                        
                    }
                    else
                    {
                        Api.DataLayer.DBQueries.UpdateDYATrans(dya_id, "1050", "Failed", ref lines);
                    }

                    


                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
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