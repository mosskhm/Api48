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
    /// Summary description for ussd_mo_benin_benin
    /// </summary>
    public class ussd_mo_benin_benin : IHttpHandler
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
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ussd_mo_benin");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_mo_benin");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_mo_benin");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_mo_benin");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_mo_benin");
            string MSISDN = "", spID = "", ServiceID = "", linkid = "", traceUniqueID = "", msgType = "", senderCB = "", ussdString = "", cu_id = "";
            string ussd_soap = "";
            ServiceClass service = new ServiceClass();
            cu_id = context.Request.QueryString["cu_id"];
            if (!String.IsNullOrEmpty(xml))
            {
                linkid = ProcessXML.GetXMLNode(xml, "ns1:linkid", ref lines);
                lines = Add2Log(lines, " linkid = " + linkid, 100, "ussd_mo_benin");

                string receiveCB = ProcessXML.GetXMLNode(xml, "ns2:receiveCB", ref lines);
                lines = Add2Log(lines, " receiveCB = " + receiveCB, 100, "ussd_mo_benin");



                traceUniqueID = ProcessXML.GetXMLNode(xml, "ns1:traceUniqueID", ref lines);
                lines = Add2Log(lines, " traceUniqueID = " + traceUniqueID, 100, "ussd_mo_benin");

                msgType = ProcessXML.GetXMLNode(xml, "ns2:msgType", ref lines);
                lines = Add2Log(lines, " msgType = " + msgType, 100, "ussd_mo_benin");

                senderCB = ProcessXML.GetXMLNode(xml, "ns2:senderCB", ref lines);
                lines = Add2Log(lines, " senderCB = " + senderCB, 100, "ussd_mo_benin");

                ussdString = ProcessXML.GetXMLNode(xml, "ns2:ussdString", ref lines);
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo_benin");

                string serviceCode = ProcessXML.GetXMLNode(xml, "ns2:serviceCode", ref lines);
                lines = Add2Log(lines, " serviceCode = " + serviceCode, 100, "ussd_mo_benin");

                MSISDN = ProcessXML.GetXMLNode(xml, "ns2:msIsdn", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo_benin");

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

                string abortReason = ProcessXML.GetXMLNode(xml, "ns2:abortReason", ref lines);
                lines = Add2Log(lines, " abortReason = " + abortReason, 100, "ussd_mo_benin");

                service = GetServiceInfo(spID, ServiceID, "", ref lines);

                if (service != null & abortReason == "")
                {
                    string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

                    string final_password = md5.Encode_md5(service.spid + "Yellow21" + timeStamp).ToUpper();


                    ussd_soap = ussd_soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/ussd/send/v1_0/local\">";
                    ussd_soap = ussd_soap + "<soapenv:Header><tns:RequestSOAPHeader xmlns:tns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    ussd_soap = ussd_soap + "<tns:spId>" + service.spid + "</tns:spId>";
                    ussd_soap = ussd_soap + "<tns:spPassword>" + final_password + "</tns:spPassword>";
                    ussd_soap = ussd_soap + "<tns:serviceId>" + ServiceID + "</tns:serviceId>";
                    ussd_soap = ussd_soap + "<tns:timeStamp>" + timeStamp + "</tns:timeStamp>";
                    ussd_soap = ussd_soap + "<tns:OA>" + MSISDN + "</tns:OA>";
                    ussd_soap = ussd_soap + "<tns:FA>" + MSISDN + "</tns:FA>";
                    ussd_soap = ussd_soap + "<tns:linkid>" + linkid + "</tns:linkid>";
                    ussd_soap = ussd_soap + "</tns:RequestSOAPHeader>";
                    ussd_soap = ussd_soap + "</soapenv:Header>";
                    ussd_soap = ussd_soap + "<soapenv:Body>";
                    ussd_soap = ussd_soap + "<loc:sendUssd>";
                    ussd_soap = ussd_soap + "<loc:msgType>1</loc:msgType>";
                    ussd_soap = ussd_soap + "<loc:senderCB>" + senderCB + "</loc:senderCB>";
                    ussd_soap = ussd_soap + "<loc:receiveCB>" + senderCB + "</loc:receiveCB>";
                    //ussd_soap = ussd_soap + "<loc:receiveCB>"+ receiveCB + "</loc:receiveCB>";
                    ussd_soap = ussd_soap + "<loc:ussdOpType>1</loc:ussdOpType>";
                    ussd_soap = ussd_soap + "<loc:msIsdn>" + MSISDN + "</loc:msIsdn>";
                    ussd_soap = ussd_soap + "<loc:serviceCode>" + serviceCode + "</loc:serviceCode>";
                    ussd_soap = ussd_soap + "<loc:codeScheme>68</loc:codeScheme>";
                    ussd_soap = ussd_soap + "<loc:ussdString>This is a test! click 1</loc:ussdString>";
                    ussd_soap = ussd_soap + "</loc:sendUssd>";
                    ussd_soap = ussd_soap + "</soapenv:Body>";
                    ussd_soap = ussd_soap + "</soapenv:Envelope>";




                }
            }

            string response_soap = "";
            response_soap = response_soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/ussd/notification/v1_0/local\">";
            response_soap = response_soap + "<soapenv:Header/>";
            response_soap = response_soap + "<soapenv:Body>";
            response_soap = response_soap + "<loc:notifyUssdReceptionResponse>";
            response_soap = response_soap + "<loc:result>0</loc:result>";
            response_soap = response_soap + "</loc:notifyUssdReceptionResponse>";
            response_soap = response_soap + "</soapenv:Body>";
            response_soap = response_soap + "</soapenv:Envelope>";
            lines = Add2Log(lines, " Response = " + response_soap, 100, "ussd_mo_benin");
            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);

            if (ussd_soap != "")
            {
                lines = Add2Log(lines, "Soap = " + ussd_soap, 100, "ussd_mo_benin");
                string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_"+service.operator_id+(service.is_staging == true ? "_STG" : ""), ref lines);
                lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo_benin");
                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, ussd_soap, ref lines);
                lines = Add2Log(lines, "SendUSSD Response = " + response, 100, "ussd_mo_benin");
            }
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