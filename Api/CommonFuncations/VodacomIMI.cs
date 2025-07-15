using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class VodacomIMI
    {
        public class SubResult
        {
            public int code { get; set; }
            public string description { get; set; }
        }
        public static string valitadeMSISDN(bool is_stg, Int64 msisdn, ref List<LogLines> lines)
        {
            string api_key = Cache.ServerSettings.GetServerSettings("VodacomAPIKey" + (is_stg == true ? "_STG" : ""), ref lines);
            string soap = "{\"api_key\": \""+ api_key + "\", \"msisdn\": \""+ msisdn +"\"}";
            lines = Add2Log(lines, "valitadeMSISDN Soap = " + soap, 100, lines[0].ControlerName);
            string vodacom_url = "VodacomValidateMSISDNURL" + (is_stg == true ? "_STG" : "");
            string soap_url = Cache.ServerSettings.GetServerSettings(vodacom_url, ref lines);

            string username = Cache.ServerSettings.GetServerSettings("VodacomUserName" + (is_stg == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("VodacomPassword" + (is_stg == true ? "_STG" : ""), ref lines);
            List<Headers> headers = new List<Headers>();
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, username, password, ref lines);
            string emsisdn = "";
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    emsisdn = auth_json_response.customer.emsisdn;
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return emsisdn;

        }

        public static string DecryptMSISDN(bool is_stg, Int64 emsisdn, ref List<LogLines> lines)
        {
            string api_key = Cache.ServerSettings.GetServerSettings("VodacomAPIKey" + (is_stg == true ? "_STG" : ""), ref lines);
            string soap = "{\"api_key\": \"" + api_key + "\", \"emsisdn\": \"" + emsisdn + "\"}";
            lines = Add2Log(lines, "VodacomDecryptMSISDNURL Soap = " + soap, 100, lines[0].ControlerName);
            string vodacom_url = "VodacomDecryptMSISDNURL" + (is_stg == true ? "_STG" : "");
            string soap_url = Cache.ServerSettings.GetServerSettings(vodacom_url, ref lines);

            string username = Cache.ServerSettings.GetServerSettings("VodacomUserName" + (is_stg == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("VodacomPassword" + (is_stg == true ? "_STG" : ""), ref lines);
            List<Headers> headers = new List<Headers>();
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, username, password, ref lines);
            string msisdn = "";
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    msisdn = auth_json_response.customer.msisdn;
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return msisdn;

        }

        public static SubResult Subscribe(ServiceClass service, string emsisdn, ref List<LogLines> lines)
        {
            SubResult res = new SubResult()
            {
                code = -1,
                description = ""
            };
            string api_key = Cache.ServerSettings.GetServerSettings("VodacomAPIKey" + (service.is_staging == true ? "_STG" : ""), ref lines);
            string soap = "{\"emsisdn\": \""+ emsisdn + "\", \"api_key\": \"" + api_key + "\", \"service_key\": \"" + service.spid_password + "\"}";
            lines = Add2Log(lines, "SubscribeMSISDN Soap = " + soap, 100, lines[0].ControlerName);
            string vodacom_url = "VodacomSubscribeURL" + (service.is_staging == true ? "_STG" : "");
            string soap_url = Cache.ServerSettings.GetServerSettings(vodacom_url, ref lines);

            string username = Cache.ServerSettings.GetServerSettings("VodacomUserName" + (service.is_staging == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("VodacomPassword" + (service.is_staging == true ? "_STG" : ""), ref lines);
            List<Headers> headers = new List<Headers>();
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, username, password, ref lines);
            string code = "", description = "" ;
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    code = auth_json_response.code;
                    description = auth_json_response.description;
                    res = new SubResult()
                    {
                        code = Convert.ToInt32(code),
                        description = description
                    };
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return res;

        }

        public static bool SendSMS(ServiceClass service, string msisdn, string text, ref List<LogLines> lines)
        {
            bool result = false;
            string emsisdn = valitadeMSISDN(service.is_staging, Convert.ToInt64(msisdn), ref lines);
            service.is_staging = false;
            string api_key = Cache.ServerSettings.GetServerSettings("VodacomAPIKey" + (service.is_staging == true ? "_STG" : ""), ref lines);
            string soap = "{\"emsisdn\": \"" + emsisdn + "\", \"api_key\": \"" + api_key + "\", \"message\": \"" + text + "\"}";
            lines = Add2Log(lines, "SendSMS Soap = " + soap, 100, lines[0].ControlerName);
            string vodacom_url = "VodacomSendSMSURL" + (service.is_staging == true ? "_STG" : "");
            string soap_url = Cache.ServerSettings.GetServerSettings(vodacom_url, ref lines);
            lines = Add2Log(lines, "soap_url = " + soap_url, 100, lines[0].ControlerName);

            string username = Cache.ServerSettings.GetServerSettings("VodacomUserName" + (service.is_staging == true ? "_STG" : ""), ref lines);
            string password = Cache.ServerSettings.GetServerSettings("VodacomPassword" + (service.is_staging == true ? "_STG" : ""), ref lines);
            List<Headers> headers = new List<Headers>();
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, username, password, ref lines);
            string code = "", description = "";
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    code = auth_json_response.code;
                    description = auth_json_response.description;
                    if (code == "200")
                    {
                        result = true;
                    }
                    
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return result;

        }



        public static string IdentifySubscribe(ServiceClass service, string msisdn, string ret_url, ref List<LogLines> lines)
        {
            
            string api_key = Cache.ServerSettings.GetServerSettings("VodacomAPIKey" + (service.is_staging == true ? "_STG" : ""), ref lines);

            Int64 ref_id = DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, result, response, subscriber_id) values (" + msisdn + ", " + service.service_id + ", now(), 1000, '', 0)", ref lines);

            string vodacom_url = Cache.ServerSettings.GetServerSettings("VodacomIdentifySubscribeURL" + (service.is_staging == true ? "_STG" : ""), ref lines);
            vodacom_url = vodacom_url + "?service_key=" + service.spid_password + "&api_key=" + api_key + "&ext_reference=" + ref_id + "&redirect=" + ret_url;

            lines = Add2Log(lines, "vodacom_url = " + vodacom_url, 100, lines[0].ControlerName);


            return vodacom_url;

        }
    }
}