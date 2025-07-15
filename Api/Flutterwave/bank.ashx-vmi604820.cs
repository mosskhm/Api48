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
                    CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), my_service, dya_transid, dya_trans.partner_transid, status, response_code, mytime, ref lines);
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