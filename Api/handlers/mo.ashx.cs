using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for mo
    /// </summary>
    public class mo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "MO");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MO");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MO");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MO");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MO");
            string spID = "", ServiceID = "", linkID = "";
            string MSISDN = "";
            string message = "";
            string smsServiceActivationNumber = "";
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var root = xmlDocument.DocumentElement;

                MSISDN = ProcessXML.GetXMLNode(xml, "senderAddress", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "MO");
                MSISDN = (MSISDN.Contains("tel:") ? MSISDN.Replace("tel:", "") : MSISDN);
                spID = (ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:spId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines));
                lines = Add2Log(lines, " spID = " + spID, 100, "MO");

                ServiceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines));
                lines = Add2Log(lines, " ServiceID = " + ServiceID, 100, "MO");

                linkID = (ProcessXML.GetXMLNode(xml, "ns1:linkid", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:linkid", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:linkid", ref lines));
                lines = Add2Log(lines, " linkID = " + linkID, 100, "MO");

                message = ProcessXML.GetXMLNode(xml, "message", ref lines).ToLower();
                lines = Add2Log(lines, " message = " + message, 100, "MO");

                string time_stamp = (ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:timeStamp", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines));
                lines = Add2Log(lines, " time_stamp = " + time_stamp, 100, "MO");
                if (!String.IsNullOrEmpty(time_stamp))
                {
                    var date = DateTime.ParseExact(time_stamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    time_stamp = date.ToString("yyyy-MM-dd HH:mm:ss");
                }


                smsServiceActivationNumber = ProcessXML.GetXMLNode(xml, "smsServiceActivationNumber", ref lines);
                lines = Add2Log(lines, " smsServiceActivationNumber = " + smsServiceActivationNumber, 100, "MO");
                smsServiceActivationNumber = (smsServiceActivationNumber.Contains("tel:") ? smsServiceActivationNumber.Replace("tel:", "") : smsServiceActivationNumber);
                if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(spID) && !String.IsNullOrEmpty(smsServiceActivationNumber))
                {
                    ServiceClass service = GetServiceInfo(spID, ServiceID, "", ref lines);
                    if (service != null)
                    {
                        ServiceBehavior.DecideBehaviorMO(service, MSISDN, message, time_stamp, smsServiceActivationNumber, linkID, ref lines);
                    }
                }
            }
            lines = Write2Log(lines);
            string response_soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/sms/notification/v2_2/local\"><soapenv:Header/><soapenv:Body><loc:notifySmsReceptionResponse/></soapenv:Body></soapenv:Envelope>";
            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);
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