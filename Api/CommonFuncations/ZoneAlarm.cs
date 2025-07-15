using Api.DataLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ZoneAlarm
    {
        public static string CallZAGetAccessToken(ref List<LogLines> lines)
        {
            string zAuthToken = "";
            string auth_json = "{ \"accountType\":\""+ Cache.ServerSettings.GetServerSettings("ZA_AccountType", ref lines) + "\", \"userEmail\":\"" + Cache.ServerSettings.GetServerSettings("ZA_Mail", ref lines) + "\", \"password\":\"" + Cache.ServerSettings.GetServerSettings("ZA_Password", ref lines) + "\", \"clientVersion\":\"" + Cache.ServerSettings.GetServerSettings("ZA_ClientVersion", ref lines) + "\"}";
            List<Headers> headers = new List<Headers>();

            lines = Add2Log(lines, " Sending GetAccessToken to : " + Cache.ServerSettings.GetServerSettings("ZA_GETTokenURL", ref lines), 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Body : " + auth_json, 100, lines[0].ControlerName);

            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("ZA_GETTokenURL", ref lines), auth_json, headers, 2, ref lines);
            lines = Add2Log(lines, " Response : " + auth_response, 100, lines[0].ControlerName);
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    zAuthToken = auth_json_response.zAuthToken;
                    lines = Add2Log(lines, " zAuthToken : " + zAuthToken, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return zAuthToken;
        }

        public static string CallZAGenerateKey(Int64 subscriber_id, string msisdn, ref List<LogLines> lines)
        {
            string zAuthToken = CallZAGetAccessToken(ref lines);
            string url = "";
            string auth_json1 = "{\"id\":\""+msisdn+"\", \"sku\":\""+ Cache.ServerSettings.GetServerSettings("ZA_SKU", ref lines) + "\"}";
            List<Headers> headers = new List<Headers>();
            lines = Add2Log(lines, " Sending GenerateKey to : " + Cache.ServerSettings.GetServerSettings("ZA_GenerateKey", ref lines), 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Body for GenerateKey : " + auth_json1, 100, lines[0].ControlerName);
            headers.Add(new Headers { key = "Z-Access-Token", value = zAuthToken });
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("ZA_GenerateKey", ref lines), auth_json1, headers, 2, ref lines);
            lines = Add2Log(lines, " Response : " + auth_response, 100, lines[0].ControlerName);
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    string long_url = auth_json_response.url;
                    lines = Add2Log(lines, " url : " + long_url, 100, lines[0].ControlerName);
                    if (!String.IsNullOrEmpty(long_url))
                    {
                        long_url = "https://play.google.com/store/apps/details?id=com.mtn.mobilesecurity";
                        url = DBQueries.CreateShortURL(subscriber_id, long_url, ref lines);
                    }
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return url;
        }

        public static string CallZADeleteUser(string msisdn, ref List<LogLines> lines)
        {
            string zAuthToken = CallZAGetAccessToken(ref lines);
            string status = "";
            string auth_json = "{\"id\":\"" + msisdn + "\"}";
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "Z-Access-Token", value = zAuthToken });
            lines = Add2Log(lines, " Sending DeleteUser to : " + Cache.ServerSettings.GetServerSettings("ZA_DeleteUserURL", ref lines), 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Body : " + auth_json, 100, lines[0].ControlerName);
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("ZA_DeleteUserURL", ref lines), auth_json, headers, 2, ref lines);
            lines = Add2Log(lines, " Response : " + auth_response, 100, lines[0].ControlerName);
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    status = auth_json_response.status.status;
                    lines = Add2Log(lines, " status : " + status, 100, lines[0].ControlerName);
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return status;
        }


    }
}