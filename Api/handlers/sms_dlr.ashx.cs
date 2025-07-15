using Api.CommonFuncations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for sms_dlr
    /// </summary>
    public class sms_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            context.Response.ContentType = "text/xml";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "sms_dlr");
            
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "sms_dlr");
            }
            string trans_id = context.Request.QueryString["trans_id"];

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            string MSISDN = "";
            string spID = "", ServiceID = "", deliveryStatus = "" ;
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "sms_dlr");
            if (!String.IsNullOrEmpty(xml))
            {
                MSISDN = ProcessXML.GetXMLNode(xml, "address", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "sms_dlr");
                spID = (ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:spId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines));
                lines = Add2Log(lines, " spID = " + spID, 100, "sms_dlr");

                ServiceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines));
                lines = Add2Log(lines, " ServiceID = " + ServiceID, 100, "sms_dlr");

                string time_stamp = (ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:timeStamp", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines));
                lines = Add2Log(lines, " time_stamp = " + time_stamp, 100, "sms_dlr");
                if (!String.IsNullOrEmpty(time_stamp))
                {
                    var date = DateTime.ParseExact(time_stamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    time_stamp = date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                deliveryStatus = ProcessXML.GetXMLNode(xml, "deliveryStatus", ref lines);
                lines = Add2Log(lines, " deliveryStatus = " + deliveryStatus, 100, "sms_dlr");

                ServiceClass service = GetServiceInfo(spID, ServiceID, "", ref lines);
                if (service != null)
                {
                    Notifications.SendSMSDLRNotification(MSISDN, service.service_id.ToString(), time_stamp, trans_id, deliveryStatus, ref lines);
                }
                if (trans_id != "12345")
                {
                    if (deliveryStatus == "DeliveredToTerminal")
                    {
                        DataLayer.DBQueries.ExecuteQuery("update marketing_camp set dlr_date = now() where id = " + trans_id, "DBConnectionString_104", ref lines);
                    }
                    
                }


            }
            //TODO: Parse XML
            //TODO: Send Notification.

            
            string text = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/sms/notification/v2_2/local\"><soapenv:Header/><soapenv:Body><loc:notifySmsDeliveryReceiptResponse/></soapenv:Body></soapenv:Envelope>";
            lines = Add2Log(lines, "Response = " + text, 100, "sms_dlr");
            lines = Write2Log(lines);
            
            context.Response.Write(text);
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