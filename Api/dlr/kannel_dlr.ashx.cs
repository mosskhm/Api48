using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.dlr
{
    /// <summary>
    /// Summary description for kannel_dlr
    /// </summary>
    public class kannel_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "kannel_dlr");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "kannel_dlr");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "kannel_dlr");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "kannel_dlr");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "kannel_dlr");
            }
            string msg_id = context.Request.QueryString["msg_id"];
            string status = context.Request.QueryString["status"];
            string service_id = context.Request.QueryString["service_id"];

            if (!String.IsNullOrEmpty(msg_id))
            {
                DataLayer.DBQueries.ExecuteQuery("update send_sms set dlr_date_time = now(), dlr_result = '" + status + "' where id = " + msg_id, "DBConnectionString_161", ref lines);
                if (!String.IsNullOrEmpty(service_id))
                {
                    ServiceURLS service_url = GetServiceURLS(Convert.ToInt32(service_id), ref lines);
                    if (service_url != null)
                    {
                        if (!String.IsNullOrEmpty(service_url.sms_dlr_url))
                        {
                            SentSMSInfo msg_info = GetSentSMSInfo(msg_id, ref lines);
                            if (msg_info != null)
                            {
                                string response = "";
                                string description = "";
                                switch (status)
                                {
                                    case "1":
                                        description = "Delivered to phone";
                                        response = "1000";
                                        break;
                                    case "2":
                                        description = "Non-Delivered to Phone";
                                        response = "1050";
                                        break;
                                    case "4":
                                        description = "Queued on SMSC";
                                        response = "1020";
                                        break;
                                    case "8":
                                        description = "Delivered to SMSC";
                                        response = "1000";
                                        break;
                                    case "16":
                                        description = "Non-Delivered to SMSC";
                                        response = "1050";
                                        break;
                                }

                                Api.CommonFuncations.Notifications.SendSMSDLRNotification(msg_info.msisdn, service_id, msg_info.date_time, msg_info.partner_transaction_id, response, description, ref lines);
                            }

                        }
                    }

                }

            }
            lines = Write2Log(lines);
            context.Response.ContentType = "text/html";
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