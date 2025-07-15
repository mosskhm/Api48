using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for ussd_congo
    /// </summary>
    public class ussd_congo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //{
            //  "sessionId": "01235",
            //  "messageType": "0",
            //  "msisdn": "2252312345",
            //  "serviceCode": "321123",
            //  "ussdString": "Please vote for xxx.",
            //  "cellId": "string",
            //  "language": "string",
            //  "imsi": "string"
            //}

            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_madapicongo");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_madapicongo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_madapicongo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_madapicongo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_madapicongo");

            string messageType = "", MSISDN = "", traceUniqueID = "", msgType = "", ussdString = "", cu_id = "", abort_soap = "";
            string ussd_soap = "";
            cu_id = context.Request.QueryString["cu_id"];

            string m_type = "1";

            string time_stamp = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;
            ServiceClass service = GetServiceByServiceID(733, ref lines);

            //{"sessionId":"16565763332210550","messageType":"2","msisdn":"242068535312","serviceCode":"1047","ussdString":"3","cellId":"14921","language":"en","imsi":"629100127926623"}
            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);

                MSISDN = json_response.msisdn;
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo");

                ussdString = json_response.ussdString;
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo");

                //Message type. 0-Begin|1-Continue|2-End|3-Notification|4-Cancel|5-Timeout.

                msgType = json_response.messageType;
                lines = Add2Log(lines, " messageType = " + msgType, 100, "ussd_mo");

                string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";

                USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);


                if (service != null && ussdString != "")
                {
                    if (umc != null)
                    {
                        lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                        USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(MSISDN, umc.ussd_id, ref lines);

                        int action_id = (ussd_session == null ? 0 : ussd_session.action_id);
                        USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                        if (ussd_menu != null)
                        {
                            //GoogleAnalytics.SendData2GoogleAnalytics("UA-135957841-1", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "BJ", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                            GoogleAnalytics.SendData2GoogleAnalytics("UA-198534595-2", "ussd", Base64.Reverse(MSISDN), context.Request.ServerVariables["REMOTE_ADDR"], "CG", "pageview", "", "", "", "/" + ussd_menu.action_name, ref lines);
                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");

                            if (ussd_menu.is_maintenance)
                            {
                                if (ussd_menu.allowed_msisdns.Contains(MSISDN))
                                {
                                    ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                                }
                                else
                                {
                                    menu_2_display = ussd_menu.maintenance_msg;
                                    is_close = true;
                                }
                            }
                            else
                            {
                                ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer142(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            }

                            
                            //freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                            //freeflowState = (is_close == true ? "FB" : "FC");
                            m_type = (is_close == true ? "2" : m_type);
                            menu_2_display = (!String.IsNullOrEmpty(menu_2_display) ? menu_2_display.Substring(0, menu_2_display.Length - 1) : "");

                        }
                    }
                }
            }

            string response_soap = "";
            if (!String.IsNullOrEmpty(menu_2_display))
            {
                menu_2_display = menu_2_display.Replace(Environment.NewLine, "\\n");
                response_soap = response_soap + "{";
                response_soap = response_soap + "  \"statusCode\": \"0000\",";
                response_soap = response_soap + "  \"statusMessage\": \"\",";
                response_soap = response_soap + "  \"transactionId\": \"\",";
                response_soap = response_soap + "  \"data\": {";
                response_soap = response_soap + "    \"inboundResponse\": \""+menu_2_display+"\",";
                string user_input = (m_type == "2" ? "false" : "true");
                response_soap = response_soap + "    \"userInputRequired\": \""+ user_input + "\",";
                response_soap = response_soap + "    \"messageType\": \""+ m_type + "\",";
                response_soap = response_soap + "    \"serviceCode\": \"1047\",";
                response_soap = response_soap + "    \"msisdn\": \""+MSISDN+"\"";
                response_soap = response_soap + "  },";
                response_soap = response_soap + "  \"_link\": {";
                response_soap = response_soap + "    \"self\": {";
                response_soap = response_soap + "      \"href\": \"https://api.mtn.com/v1/messages/ussd/send\"";
                response_soap = response_soap + "    }";
                response_soap = response_soap + "  }";
                response_soap = response_soap + "}";
            }
            else
            {
                response_soap = response_soap + "{";
                response_soap = response_soap + "  \"statusCode\": \"0000\",";
                response_soap = response_soap + "  \"statusMessage\": \"\",";
                response_soap = response_soap + "  \"transactionId\": \"\",";
                response_soap = response_soap + "  \"data\": {";
                response_soap = response_soap + "    \"inboundResponse\": \"Hi This is a test\",";
                response_soap = response_soap + "    \"userInputRequired\": \"true\",";
                response_soap = response_soap + "    \"messageType\": \"2\",";
                response_soap = response_soap + "    \"serviceCode\": \"1047\",";
                response_soap = response_soap + "    \"msisdn\": \"" + MSISDN + "\"";
                response_soap = response_soap + "  },";
                response_soap = response_soap + "  \"_link\": {";
                response_soap = response_soap + "    \"self\": {";
                response_soap = response_soap + "      \"href\": \"https://api.mtn.com/v1/messages/ussd/send\"";
                response_soap = response_soap + "    }";
                response_soap = response_soap + "  }";
                response_soap = response_soap + "}";
            }

            
            lines = Add2Log(lines, "Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

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