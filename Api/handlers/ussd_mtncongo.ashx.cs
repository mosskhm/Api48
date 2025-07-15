using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ussd_mtncongo
    /// </summary>
    public class ussd_mtncongo : IHttpHandler
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
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ussd_mtncongo");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_mtncongo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_mtncongo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_mtncongo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_mtncongo");

            string MSISDN = "", spID = "", ServiceID = "", linkid = "", traceUniqueID = "", msgType = "", senderCB = "", ussdString = "", cu_id = "", receiveCB = "", abort_soap = "";
            string ussd_soap = "";
            cu_id = context.Request.QueryString["cu_id"];

            string time_stamp = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;
            ServiceClass service = GetServiceByServiceID(733, ref lines);
            if (!String.IsNullOrEmpty(xml))
            {
                linkid = ProcessXML.GetXMLNode(xml, "ns1:linkid", ref lines);
                //lines = Add2Log(lines, " linkid = " + linkid, 100, "ussd_mo");

                receiveCB = ProcessXML.GetXMLNode(xml, "ns2:receiveCB", ref lines);
                //lines = Add2Log(lines, " receiveCB = " + receiveCB, 100, "ussd_mo");

                traceUniqueID = ProcessXML.GetXMLNode(xml, "ns1:traceUniqueID", ref lines);
                //lines = Add2Log(lines, " traceUniqueID = " + traceUniqueID, 100, "ussd_mo");

                msgType = ProcessXML.GetXMLNode(xml, "ns2:msgType", ref lines);
                //lines = Add2Log(lines, " msgType = " + msgType, 100, "ussd_mo");

                senderCB = ProcessXML.GetXMLNode(xml, "ns2:senderCB", ref lines);
                //lines = Add2Log(lines, " senderCB = " + senderCB, 100, "ussd_mo");

                ussdString = ProcessXML.GetXMLNode(xml, "ns2:ussdString", ref lines);
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo");

                string serviceCode = ProcessXML.GetXMLNode(xml, "ns2:serviceCode", ref lines);
                //lines = Add2Log(lines, " serviceCode = " + serviceCode, 100, "ussd_mo");

                MSISDN = ProcessXML.GetXMLNode(xml, "ns2:msIsdn", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo");

                spID = (ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:spId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines));
                //lines = Add2Log(lines, " spID = " + spID, 100, "sms_dlr");

                ServiceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceId", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines));
                //lines = Add2Log(lines, " ServiceID = " + ServiceID, 100, "sms_dlr");

                time_stamp = (ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:timeStamp", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines));
                //lines = Add2Log(lines, " time_stamp = " + time_stamp, 100, "sms_dlr");
                if (!String.IsNullOrEmpty(time_stamp))
                {
                    var date = DateTime.ParseExact(time_stamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    time_stamp = date.ToString("yyyy-MM-dd HH:mm:ss");
                }

                string abortReason = ProcessXML.GetXMLNode(xml, "ns2:abortReason", ref lines);
                //lines = Add2Log(lines, " abortReason = " + abortReason, 100, "ussd_mo");


                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);

                
                if (service != null && abortReason == "" && ussdString != "")
                {
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            GoogleAnalytics.SendData2GoogleAnalytics("UA-198534595-2", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "CG", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);

                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");

                            if (ussd_menu.is_maintenance)
                            {
                                if (ussd_menu.allowed_msisdns.Contains(MSISDN))
                                {
                                    ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                                }
                                else
                                {
                                    menu_2_display = ussd_menu.maintenance_msg;
                                    is_close = true;
                                }
                            }
                            else
                            {
                                ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            }

                            
                            freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                            freeflowState = (is_close == true ? "FB" : "FC");
                            menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");

                        }
                    }
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
            lines = Add2Log(lines, " Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);

            if (ussd_soap != "")
            {
                //lines = Add2Log(lines, "Soap = " + ussd_soap, 100, "ussd_mo");
                string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                //lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo");
                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, ussd_soap, ref lines);
                //lines = Add2Log(lines, "SendUSSD Response = " + response, 100, "ussd_mo");
            }
            if (abort_soap != "")
            {
                //lines = Add2Log(lines, "Abort Soap = " + abort_soap, 100, "ussd_mo");
                string soap_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                //lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ussd_mo");
                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, ussd_soap, ref lines);
                //lines = Add2Log(lines, "SendAbortUSSD Response = " + response, 100, "ussd_mo");
            }

            if (momo_request != null)
            {
                momo_request.Delay = "5000";
                string postBody = JsonConvert.SerializeObject(momo_request);
                string url = "https://api125.ydplatform.com/api/DYAReceiveMoney";
                List<Headers> headers = new List<Headers>();
                lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
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