using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class HML
    {
        public static bool SendSMS(string msisdn, string text, string short_code, ref List<LogLines> lines)
        {
            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("HMLSENDSMSURL", ref lines);
            msisdn = "0" + msisdn.Substring(3);
            string json = "shortcode="+ short_code + "&type=SMS&telco=MTN&phone="+ msisdn + "&message=" + text;
            List<Headers> headers = new List<Headers>();
            string bearear_key = Cache.ServerSettings.GetServerSettings("HMLSENDSMSKEY", ref lines);
            headers.Add(new Headers { key = "Authorization", value = "Bearer " + bearear_key });

            string body = CallSoap.CallSoapRequest(url, json, headers, 3, ref lines);

            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    
                    string message = json_response.message;
                    if (!String.IsNullOrEmpty(message))
                    {
                        if (message.ToLower().Contains("success"))
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

        public static string CreateFamilyFuedLink(string msisdn, ref List<LogLines> lines)
        {
            string text = "";
            string url = Cache.ServerSettings.GetServerSettings("FamilyFuedURL", ref lines);
            string json = "{\"phone\":\"+" + msisdn+"\"}";
            List<Headers> headers = new List<Headers>();
            string bearear_key = Cache.ServerSettings.GetServerSettings("FamilyFuedKEY", ref lines);
            headers.Add(new Headers { key = "Authorization", value = "Bearer " + bearear_key });

            string body = CallSoap.CallSoapRequest(url, json, headers, 2, ref lines);

            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {

                    string unique_link = json_response.unique_link;
                    string code = json_response.code;
                    if (!String.IsNullOrEmpty(unique_link) && !String.IsNullOrEmpty(code))
                    {
                        text = "Yello from family feud. Click this link "+ unique_link + " to complete your application with code "+ code + ". You will be called if selected for the auditions or try again. Thank you!";
                    }
                }
                catch (Exception ex)
                {

                }


            }

            return text;
        }
    }
}