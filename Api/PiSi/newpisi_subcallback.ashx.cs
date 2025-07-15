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
    /// Summary description for newpisi_subcallback
    /// </summary>
    public class newpisi_subcallback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            //DeactivationDate
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "NewPisiSubCallbacks");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "NewPisiSubCallbacks");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "NewPisiSubCallbacks");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "NewPisiSubCallbacks");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "NewPisiSubCallbacks");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "PisiSubCallbacks");
            }
            if (!String.IsNullOrEmpty(xml))
            {
                Int64 sub_id = 0;
                Int64 status = 0;
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string ServiceID = json_response.ServiceID;
                    string MSISDN = json_response.MSISDN;
                    string DeactivationDate = json_response.DeactivationDate;
                    List<PisiMobileNewServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileNewServiceInfo(ref lines);
                    if (pisi_services != null)
                    {
                        PisiMobileNewServiceInfo pisi_service = pisi_services.Find(x => x.pisi_service_id == Convert.ToInt32(ServiceID));
                        if (pisi_service != null)
                        {
                            List<string> sub_id_str1 = Api.DataLayer.DBQueries.SelectQueryReturnListString("select subscriber_id from subscribers where msisdn = " + MSISDN + " and service_id = " + pisi_service.service_id, ref lines);
                            string sub_id_str = "";
                            if (sub_id_str1 != null)
                            {
                                int max_len = sub_id_str1.Count();
                                sub_id_str = sub_id_str1[max_len - 1];
                            }
                            if (!String.IsNullOrEmpty(sub_id_str))
                            {
                                status = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select state_id from subscribers where subscriber_id = " + sub_id_str, ref lines);
                                sub_id = Convert.ToInt64(sub_id_str);
                            }
                            else
                            {
                                if (xml.Contains("DeactivationDate"))
                                {
                                    sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id, deactivation_date) values (" + MSISDN + "," + pisi_service.service_id + ",now(),2,'"+DeactivationDate+"')", ref lines);
                                }
                            }
                        }
                        if (xml.Contains("DeactivationDate"))
                        {
                            if (sub_id > 0)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = '"+DeactivationDate+"' where subscriber_id = " + sub_id, ref lines);
                            }
                            else
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = '" + DeactivationDate + "' where msisdn = " + MSISDN + " and service_id = " + pisi_service.service_id, ref lines);
                            }
                            Api.CommonFuncations.Notifications.SendUnSubscriptionNotification(MSISDN, pisi_service.service_id.ToString(), DeactivationDate, sub_id, ref lines);
                        }
                        //2024-02-05 23:41:17.694: Incomming XML = {"MSISDN":  2348085802521, "ServiceID": 1336, "DeliveryDate": "2024-02-05 23:41:24", "Text": "A"}
                        if (xml.Contains("DeliveryDate"))
                        {
                            string DeliveryDate = json_response.DeliveryDate;
                            string Text = json_response.Text;
                            Api.CommonFuncations.Notifications.SendMONotification(MSISDN, pisi_service.service_id.ToString(), DeliveryDate, Text, ref lines);
                            ServiceClass service = GetServiceByServiceID(pisi_service.service_id, ref lines);
                            if (service != null)
                            {
                                Api.CommonFuncations.ServiceBehavior.DecideBehaviorMO(service, MSISDN, Text, DeliveryDate, service.sms_mt_code.ToString(), "", ref lines);
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "NewPisiSubCallbacks");
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