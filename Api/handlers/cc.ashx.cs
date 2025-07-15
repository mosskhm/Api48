using Api.CommonFuncations;
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
    /// Summary description for cc
    /// </summary>
    public class cc : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "CC");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "CC");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "CC");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "CC");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "CC");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "CC");
            }

            string id = context.Request.QueryString["trans_id"];
            string transaction_id = context.Request.QueryString["transaction_id"];

            string redirect_url = "https://m.lnbpari.com/#/user/deposit";

            if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(transaction_id))
            {
                string real_id = Api.CommonFuncations.Base64.EncodeDecodeBase64(id, 2);
                Int64 number;
                bool success = Int64.TryParse(real_id, out number);
                if (success)
                {
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(number, "01", "Success", ref lines);

                    ServiceClass service = new ServiceClass();
                    service = GetServiceByServiceID(dya_trans.service_id, ref lines);

                    ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, "01", ref lines);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + number + ",'" + transaction_id + "');", ref lines);
                    string mytime = dya_trans.datetime;
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (service.hour_diff != "0")
                    {
                        mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                        dya_trans.datetime = mytime;
                    }
                    CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), real_id, dya_trans.partner_transid, "Successfully processed transaction", 1000, mytime, ref lines);
                }
            }

            lines = Add2Log(lines, "Redirecting to " + redirect_url, 100, "CC");
            lines = Write2Log(lines);
            context.Response.Redirect(redirect_url);
            //string response_soap = "ok";
            //context.Response.ContentType = "text/plain";
            //context.Response.Write(response_soap);
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