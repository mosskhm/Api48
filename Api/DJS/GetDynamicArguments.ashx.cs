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
    /// Summary description for GetDynamicArguments
    /// </summary>
    public class GetDynamicArguments : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "GetDynamicArguments");
            lines = Add2Log(lines, "Incomming Json = " + xml, 100, "GetDynamicArguments");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "GetDynamicArguments");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "GetDynamicArguments");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "GetDynamicArguments");

            DYAReceiveMoneyRequest momo_request = null;
            ServiceClass service = GetServiceByServiceID(733, ref lines);
            string MSISDN = "", UserInput = "", ActionID = "", RapidosUserInput = "", RapidosPrice = "";
            string result = "{";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    var arguments = json_response.arguments;
                    foreach (var a in arguments)
                    {
                        if (a.key.value == "ACCOUNT_HOLDER_MSISDN")
                        {
                            MSISDN = a.value.value;
                        }
                        if (a.key.value == "UserInput")
                        {
                            UserInput = a.value.value;
                        }
                        if (a.key.value == "ActionID")
                        {
                            ActionID = a.value.value;
                        }
                        if (a.key.value == "RapidosUserInput")
                        {
                            RapidosUserInput = a.value.value;
                        }
                        if (a.key.value == "RapidosPrice")
                        {
                            RapidosPrice = a.value.value;
                        }
                    }
                    string session_id = json_response.sessionIdentifier;
                    string languageCode = json_response.languageCode;
                    string journeyIdentifier = json_response.journeyIdentifier;


                    if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(UserInput) && !String.IsNullOrEmpty(ActionID) && !String.IsNullOrEmpty(session_id) && !String.IsNullOrEmpty(languageCode) && !String.IsNullOrEmpty(journeyIdentifier))
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
                            result = result + Api.CommonFuncations.USSD.MTNCongoUSSDBehaviuer105(service, MSISDN, UserInput, ActionID, RapidosUserInput, RapidosPrice, ussd_menu, ussd_session, out momo_request, ref lines, session_id, true);

                            result = result + "\"languagecode\": \"" + languageCode + "\", \"sessionIdentifier\": \""+session_id+ "\", \"journeyIdentifier\": \""+journeyIdentifier+"\"} ";




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
            context.Response.Write(result);
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