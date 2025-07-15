using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.orange_cm
{
    /// <summary>
    /// Summary description for sms_dlr
    /// </summary>
    public class sms_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "orange_vas_smsdlr");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "orange_vas_smsdlr");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "orange_vas_smsdlr");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "orange_vas_smsdlr");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "orange_vas_smsdlr");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "orange_vas_smsdlr");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "orange_guinea_billing");
            }

            string headers = string.Empty;
            foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                headers = key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                //lines = Add2Log(lines, headers, 100, "orange_guinea_billing");
            }
            //2022-06-11 10:21:10.285: HTTP_ORANGE_HEADERS={"host":"cam.ydafrica.com","x-real-ip":"90.84.199.35","x-forwarded-for":"90.84.199.35","remote-host":"90.84.199.35","connection":"close","content-length":"301","accept":"application/json, application/*+json","x-orange-ise2":"PDKSUB-200-xHBLXFLfGr/yfQkZLV5XkD4A2V84gTC7CyHLpyQy9uM=","orangeapitoken":"B64OqpaPfOynjIjEF++4F9M/M6+E2Vdf7i5Su36vK9FsMsCSc00VHOZYPY8OWhOWtKY94tiwgJUl/XBjLeiAPHz6w==|MCO=OCM|tcd=1654935667|ted=1654935767|ExCiP6e0oj9fX8sw9pO2HKFonWU=","content-type":"application/json","user-agent":"Apache-HttpClient/4.5.13 (Java/11.0.4)","accept-encoding":"gzip,deflate"}
            //2022-06-11 12:33:15.066: Incomming XML = {"inboundSMSMessageNotification":{"callbackData":"YELLOWDOT LoveTips - OCM - 2378979","inboundSMSMessage":{"dateTime":"2022-06-11T12:33:14","destinationAddress":"+2378979","messageId":"62a46f6a84b11976b345237f","message":"http://www.whatsapp.com/","senderAddress":"acr:OrangeAPIToken"}}}

            string enc_msisdn = context.Request.ServerVariables["HTTP_X_ORANGE_ISE2"];
            lines = Add2Log(lines, "enc_msisdn = " + enc_msisdn, 100, "orange_guinea_billing");
            
            //2022-06-13 08:43:19.374: Incomming XML = {"inboundSMSMessageNotification":{"callbackData":"YELLOWDOT LoveTips - OCM - 2378979","inboundSMSMessage":{"dateTime":"2022-06-13T08:43:20","destinationAddress":"+2378979","messageId":"62a6dc8884b11976b34c05ad","message":"stop","senderAddress":"acr:OrangeAPIToken"}}}


            
            dynamic json_response = JsonConvert.DeserializeObject(xml);
            try
            {
                if (xml.Contains("inboundSMSMessageNotification"))
                {
                    string shortcode = json_response.inboundSMSMessageNotification.inboundSMSMessage.destinationAddress;
                    string message = json_response.inboundSMSMessageNotification.inboundSMSMessage.message;

                    if (!String.IsNullOrEmpty(shortcode))
                    {
                        shortcode = shortcode.Replace("+237", "");
                        List<ServiceClass> sc = GetServiceList(ref lines);
                        ServiceClass service = sc.Find(x => x.sms_mt_code == shortcode);
                        if (service != null)
                        {
                            lines = Add2Log(lines, "Service was found " + service.service_name, 100, "orange_guinea_billing");
                            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            Api.CommonFuncations.Notifications.SendMONotificationWithEncMSISDN("2371", service.service_id.ToString(), dt, message, enc_msisdn, ref lines);
                        }
                        else
                        {
                            lines = Add2Log(lines, "Service was not found", 100, "orange_guinea_billing");
                        }
                    }
                }
                //2022-06-13 09:02:35.406: Incomming XML = {"deliveryInfoNotification":{"callbackData":"94baf1b1-1670-452d-b61e-502e40112080","deliveryInfo":{"address":"tel:+237658089501","deliveryStatus":"DeliveredToNetwork"}}}


                if (xml.Contains("deliveryInfoNotification"))
                {
                    string callbackData = json_response.deliveryInfoNotification.callbackData;
                    string msisdn = json_response.deliveryInfoNotification.deliveryInfo.address;
                    if (!String.IsNullOrEmpty(msisdn))
                    {
                        msisdn = msisdn.Replace("tel:+", "");
                        string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_id from orange_vas_dlr_sms where callback_id = '"+callbackData+"'" , ref lines);
                        string enc_msisdn1 = Api.DataLayer.DBQueries.SelectQueryReturnString("select enc_msisdn from orange_vas_dlr_sms where callback_id = '" + callbackData + "'", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("delete from orange_vas_dlr_sms where callback_id = '" + callbackData + "'", ref lines);
                        string sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT GROUP_CONCAT(so.subscriber_id) FROM subscribers_ocm so, subscribers s WHERE s.subscriber_id = so.subscriber_id AND s.service_id = "+service_id+" AND so.encrypted_msisdn = '"+enc_msisdn1+"'", ref lines);
                        if (!String.IsNullOrEmpty(sub_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update subscribers_ocm set msisdn = " + msisdn + " where subscriber_id in (" + sub_id + ")", ref lines);
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                lines = Add2Log(lines, "Exception: " + ex.ToString(), 100, "orange_guinea_billing");
            }




            




            lines = Write2Log(lines);


            context.Response.ContentType = "text/plain";
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