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
using static Api.CommonFuncations.USSD;
using static Api.Logger.Logger;

namespace Api.DJS
{
    /// <summary>
    /// Summary description for GetDynamicOptions
    /// </summary>
    public class GetDynamicOptions : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "GetDynamicOptions");
            lines = Add2Log(lines, "Incomming Json = " + xml, 100, "GetDynamicOptions");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "GetDynamicArguments");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "GetDynamicArguments");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "GetDynamicArguments");

            DYAReceiveMoneyRequest momo_request = null;
            ServiceClass service = GetServiceByServiceID(733, ref lines);
            string MSISDN = "", UserInput = "", ActionID = "", RapidosUserInput = "", RapidosPrice = "";
            string result = "";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    var arguments = json_response.arguments;
                    foreach (var a in arguments)
                    {
                        if (a.key == "ACCOUNT_HOLDER_MSISDN")
                        {
                            MSISDN = a.value;
                        }
                        if (a.key == "UserInput")
                        {
                            UserInput = a.value;
                        }
                        if (a.key == "ActionID")
                        {
                            ActionID = a.value;
                        }
                        if (a.key == "RapidosUserInput")
                        {
                            RapidosUserInput = a.value;
                        }
                        if (a.key == "RapidosPrice")
                        {
                            RapidosPrice = a.value;
                        }
                    }
                    string session_id = json_response.sessionid;

                    if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(UserInput) && !String.IsNullOrEmpty(ActionID) && !String.IsNullOrEmpty(session_id))
                    {
                        string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                        USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                        if (umc != null)
                        {
                            string ussdString = UserInput;
                            lines = Add2Log(lines, " USSD Main Code ID = " + umc.ussd_id, 100, "ivr_subscribe");
                            USSDSession ussd_session = DataLayer.DBQueries.GetLastUSSDSession(session_id, ref lines);
                            int action_id = Convert.ToInt32(ActionID);
                            USSDMenu ussd_menu = GetUSSDMenu(umc.ussd_id, ussdString, action_id, ussd_session, ref lines);
                            result = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer105(service, MSISDN, UserInput, ActionID, RapidosUserInput, RapidosPrice, ussd_menu, ussd_session, out momo_request, ref lines, session_id, false);




                            //if (ussd_menu != null)
                            //{
                            //    lines = Add2Log(lines, " USSD Menu topic = " + ussd_menu.topic_name + ", ActionID = " + ussd_menu.action_id + " Action = " + ussd_menu.action_name, 100, "ivr_subscribe");
                            //    string menu_2_display = "";
                            //    bool is_close = false;
                            //    string ussd_soap = "";
                            //    ussd_soap = Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer(service, ussdString, ServiceID, MSISDN, linkid, receiveCB, senderCB, serviceCode, ussd_menu, ussd_session, out momo_request, ref lines, out menu_2_display, out is_close, session_id);
                            //    lines = Add2Log(lines, " Display Menu = " + (String.IsNullOrEmpty(menu_2_display) ? "" : menu_2_display), 100, "ivr_subscribe");
                            //}
                        }
                        if (momo_request != null)
                        {
                            momo_request.Delay = "5000";
                            string postBody = JsonConvert.SerializeObject(momo_request);
                            string url = "https://api.ydplatform.com/api/DYAReceiveMoney";
                            List<Headers> headers = new List<Headers>();
                            lines = Add2Log(lines, "Sending momo request async with delay ", 100, "ussd_mo");
                            CommonFuncations.CallSoap.CallSoapRequestAsync(url, postBody, headers, 2, ref lines);
                            lines = Add2Log(lines, "Finished Sending momo request async with delay ", 100, "ussd_mo");
                        }

                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception " + ex.ToString(), 100, "ussd_wallet_mtncongo");
                }


            }

            context.Response.ContentType = "application/json";
            string myResponse = JsonConvert.SerializeObject(result);
            context.Response.Write("["+myResponse+"]");
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