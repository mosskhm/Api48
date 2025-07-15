using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Cache.USSD;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ussd_wallet_mtncongo
    /// </summary>
    public class ussd_wallet_mtncongo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ussd_wallet_mtncongo");
            lines = Add2Log(lines, "Incomming Json = " + xml, 100, "ussd_wallet_mtncongo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ussd_wallet_mtncongo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ussd_wallet_mtncongo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ussd_wallet_mtncongo");

            ServiceClass service = GetServiceByServiceID(733, ref lines);
            string MSISDN = "", UserInput = "", ActionID = "";
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(xml);
                    var arguments = json_response.arguments;
                    foreach(var a in arguments)
                    {
                        if (a.key == "MSISDN")
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
                    }
                    string sessionid = json_response.sessionid;
                    if (!String.IsNullOrEmpty(MSISDN) && !String.IsNullOrEmpty(UserInput) && !String.IsNullOrEmpty(ActionID))
                    {
                        string spID = "2420110012641", ServiceID = "", serviceCode = "104", linkid = "", receiveCB = "FFFFFFFF", senderCB = "186597739";
                        USSDMainCode umc = GetUSSDMainCodeID(spID, serviceCode, ref lines);
                        if (umc != null)
                        {

                        }

                    }

                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, "Exception " + ex.ToString(), 100, "ussd_wallet_mtncongo");
                }
                

            }

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"param\":\"value\"}");
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