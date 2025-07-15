using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class IMI
    {
        
        public static bool Subscribe(string msisdn, string service_id, ref List<LogLines> lines)
        {
            bool result = false;
            IMIServiceClass imi_service = Api.Cache.Services.GetIMIService(service_id, ref lines);
            if(imi_service != null)
            {
                string post = "{";
                post = post + "\"service\": {";
                post = post + "\"reqtype\": \"SUB\",";
                post = post + "\"msisdn\": \""+msisdn+"\",";
                post = post + "\"serviceid\": \"" + imi_service.svcid + "\",";
                post = post + "\"scode\": \"2783123686\",";
                post = post + "\"chnl\": \"sms\"}}";

                string subscribe_url = Api.Cache.ServerSettings.GetServerSettings("IMISbscribeURL", ref lines);
                lines = Add2Log(lines, " Json = " + post, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "X-IMI-SIGNATURE", value = "549D1B1B291F1FA6B616D05EB6592D5462B62C9A3B5AEF7F773BAE3D2B4EDB1B8A5B0C4734DED81E9DA23E709B2FAA6C5BE8925CB6C91A02D2C2ABC16B2FF8DF" });
                headers.Add(new Headers { key = "X-IMI-REQINIT", value = "20191121105811254" });
                headers.Add(new Headers { key = "Authorization", value = "Basic WUVMTE9XRE9UOkRvVFR2IyQ3ODk=" });
                headers.Add(new Headers { key = "X-FORWARDIP", value = "5.189.136.17" });

                string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(subscribe_url, post, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        string rescode = json_response.service.rescode;
                        string status = json_response.service.status;
                        if (rescode == "91")
                        {
                            //user already subscribed 
                            result = true;
                        }
                        if (rescode == "0" && status == "0")
                        {
                            //user was subscribed 
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }
                

            }

            return result;
        }

        public static bool UnSubscribe(string msisdn, string service_id, ref List<LogLines> lines)
        {
            bool result = false;
            IMIServiceClass imi_service = Api.Cache.Services.GetIMIService(service_id, ref lines);
            if (imi_service != null)
            {
                string post = "{";
                post = post + "\"service\": {";
                post = post + "\"reqtype\": \"UNSUB\",";
                post = post + "\"msisdn\": \"" + msisdn + "\",";
                post = post + "\"serviceid\": \"" + imi_service.svcid + "\",";
                post = post + "\"scode\": \"2783123686\","; 
                post = post + "\"chnl\": \"sms\"}}";

                string subscribe_url = Api.Cache.ServerSettings.GetServerSettings("IMIUnSbscribeURL", ref lines);
                lines = Add2Log(lines, " Json = " + post, 100, "ivr_subscribe");
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "X-IMI-SIGNATURE", value = "549D1B1B291F1FA6B616D05EB6592D5462B62C9A3B5AEF7F773BAE3D2B4EDB1B8A5B0C4734DED81E9DA23E709B2FAA6C5BE8925CB6C91A02D2C2ABC16B2FF8DF" });
                headers.Add(new Headers { key = "X-IMI-REQINIT", value = "20191121105811254" });
                headers.Add(new Headers { key = "Authorization", value = "Basic WUVMTE9XRE9UOkRvVFR2IyQ3ODk=" });
                headers.Add(new Headers { key = "X-FORWARDIP", value = "5.189.136.17" });

                string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(subscribe_url, post, headers, 2, true, ref lines);
                lines = Add2Log(lines, " Response " + response_body, 100, "ivr_subscribe");
                if (!String.IsNullOrEmpty(response_body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);

                    try
                    {
                        
                        string rescode = json_response.service.rescode;
                        string status = json_response.service.status;
                        if (rescode == "0" && status == "0")
                        {
                            //user was unsubscribed 
                            result = true;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                    }
                }


            }

            return result;
        }
    }
}