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
using static Api.CommonFuncations.TaskManager;

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for ussd_ng205
    /// </summary>
    public class ussd_ng205 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_ng205");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ussd_ng205");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_ng205");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_ng205");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_ng205");

            string messageType = "", MSISDN = "", traceUniqueID = "", msgType = "", ussdString = "", cu_id = "", abort_soap = "", sessionId = "";
            string ussd_soap = "";
            cu_id = context.Request.QueryString["cu_id"];

            string m_type = "1";

            string time_stamp = "";
            string menu_2_display = "";
            string freeflowState = "FC";
            DYAReceiveMoneyRequest momo_request = null;
            bool is_close = false;
            ServiceClass service = GetServiceByServiceID(956, ref lines);

            //{"network":"MTNN","aggregator":"Pisi Mobile Services","msisdn":"2348149820733","sessionId":"1667829709345","messageType":0,"imsi":"621300400218678","ussdString":"205*447","cellId":null,"language":null,"serviceCode":"205"}
            
            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);

                MSISDN = json_response.msisdn;
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ussd_mo");

                sessionId = json_response.sessionId;
                lines = Add2Log(lines, " sessionId = " + sessionId, 100, "ussd_mo");

                ussdString = json_response.ussdString;
                lines = Add2Log(lines, " ussdString = " + ussdString, 100, "ussd_mo");

                //Message type. 0-Begin|1-Continue|2-End|3-Notification|4-Cancel|5-Timeout.

                msgType = json_response.messageType;
                lines = Add2Log(lines, " messageType = " + msgType, 100, "ussd_mo");

                string spID = "205", ServiceID = "", serviceCode = "205", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";

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

                            lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            ussd_soap = Api.CommonFuncations.USSD.USSDBehaviuerNG205(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, "");
                            m_type = (is_close == true ? "2" : m_type);
                            
                            //freeflowState = (String.IsNullOrEmpty(menu_2_display) ? "FB" : freeflowState);
                            //freeflowState = (is_close == true ? "FB" : "FC");
                            

                        }
                    }
                }
            }

            string response_soap = "";
            if (!String.IsNullOrEmpty(menu_2_display))
            {
                menu_2_display = menu_2_display.Replace(Environment.NewLine, "\\n");
                response_soap = response_soap + "{";
                response_soap = response_soap + "\"response\": \""+menu_2_display+"\",";
                response_soap = response_soap + "\"userInputRequired\": "+Convert.ToString(!is_close).ToLower()+",";
                response_soap = response_soap + "\"serviceCode\": \"205\",";
                response_soap = response_soap + "\"messageType\": "+ m_type + ",";
                response_soap = response_soap + "\"msisdn\": \""+MSISDN+"\",";
                response_soap = response_soap + "\"sessionId\": \""+sessionId+"\"";
                response_soap = response_soap + "}";
            }
            else
            {
                response_soap = response_soap + "{";
                response_soap = response_soap + "\"response\": \"Hi\",";
                response_soap = response_soap + "\"userInputRequired\": false,";
                response_soap = response_soap + "\"serviceCode\": \"205\",";
                response_soap = response_soap + "\"messageType\": 2,";
                response_soap = response_soap + "\"msisdn\": \"" + MSISDN + "\",";
                response_soap = response_soap + "\"sessionId\": \"" + sessionId + "\"";
                response_soap = response_soap + "}";
            }


            lines = Add2Log(lines, "Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

            if (momo_request != null)
            {
                momo_request.Delay = "1000";
                string postBody = JsonConvert.SerializeObject(momo_request);
                string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                List<Headers> headers = new List<Headers>();
                lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
            }

            TaskManager.WaitForAllTasksToFinish();      // wait for all background tasks to finish
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