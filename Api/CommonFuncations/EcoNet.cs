using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class EcoNet
    {
        public static bool SendSMS(string msisdn, string text, ref List<LogLines> lines)
        {
            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("EcoNetSendSMSURL", ref lines) + "?DA=" + msisdn + "&SMS=" + System.Uri.EscapeDataString(text);
            lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (!String.IsNullOrEmpty(send_sms))
            {
                dynamic json_response = JsonConvert.DeserializeObject(send_sms);
                string id = json_response.id;
                if (!String.IsNullOrEmpty(id))
                {
                    sms_sent = true;
                }

            }
            return sms_sent;
        }

        public static bool SendSMSSL(string msisdn, string text, ref List<LogLines> lines)
        {
            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("EcoNetSendSMSURLSL", ref lines);
            lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMS");
            string json = "{\"msisdn\":\""+ msisdn + "\",\"message\":\""+text+"\"}";
            List<Headers> headers = new List<Headers>();
            string send_sms = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            if (!String.IsNullOrEmpty(send_sms))
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(send_sms);
                    string response = json_response.response;
                    if (!String.IsNullOrEmpty(response))
                    {
                        if (response == "Request Saved Successfully")
                        {
                            sms_sent = true;
                        }
                        
                    }
                }
                catch(Exception ex)
                {

                }
                

            }
            return sms_sent;
        }
    }
}