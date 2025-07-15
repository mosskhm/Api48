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
    /// Summary description for mo_sms
    /// </summary>
    public class mo_sms : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "pisi_smsmo");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "pisi_smsmo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "pisi_smsmo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "pisi_smsmo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "pisi_smsmo");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "pisi_smsmo");
            }

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    //{ "network":"MTNN","aggregator":"Pisi Mobile Services","pisisid":54,"id":"9d0a03ac-5eb4-4572-84ea-800296548a48","senderAddress":"2348131609970","receiverAddress":"205","message":"E","created":1660217191036}
                    string msisdn = json_response.senderAddress;
                    string message = json_response.message;
                    string pisisid = json_response.pisisid;

                    if (pisisid == "54" && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(message))
                    {
                        Api.CommonFuncations.Notifications.SendMONotification(msisdn, "897", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message, ref lines);
                    }
                    else
                    {
                        List<PisiMobileServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileServiceInfo(ref lines);
                        if (pisi_services != null)
                        {
                            PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.sms_id == Convert.ToInt32(pisisid));
                            if (pisi_service != null)
                            {
                                ServiceClass service = GetServiceByServiceID(pisi_service.service_id, ref lines);
                                string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                Api.CommonFuncations.ServiceBehavior.DecideBehaviorMO(service, msisdn, message, timestamp, "0", "", ref lines);
                                Api.CommonFuncations.Notifications.SendMONotification(msisdn, pisi_service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message, ref lines);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
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