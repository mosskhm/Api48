using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class crbt
    {
        public static string GetToken(ref List<LogLines> lines)
        {
            string bearer = "";
            string user_name = Api.Cache.ServerSettings.GetServerSettings("CRBT_Username", ref lines);
            string password = Api.Cache.ServerSettings.GetServerSettings("CRBT_Password", ref lines);
            string url = Api.Cache.ServerSettings.GetServerSettings("CRBT_LoginURL", ref lines);
            string json = "{\"username\": \""+user_name+"\",\"password\": \""+ password + "\"}";
            List<Headers> headers = new List<Headers>();
            string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    bearer = json_response.jwtToken;

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }
            return bearer;
        }

        public static string SubscribeUser(int service_id, string crbt_id, string msisdn, ref List<LogLines> lines)
        {
            string status_code = "";
            string token = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT c.token_id from crbt_service_settings c WHERE c.service_id = "+service_id+" AND NOW() <= c.experation_time", ref lines);
            if (String.IsNullOrEmpty(token))
            {
                token = GetToken(ref lines);
                if (!String.IsNullOrEmpty(token))
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("update crbt_service_settings set experation_time = date_add(now(), interval 1 hour), token_id = '"+token+"' where service_id = " + service_id, ref lines);
                }
                else
                {
                    status_code = "Authentication Failed";
                }
            }
            string url = Api.Cache.ServerSettings.GetServerSettings("CRBT_SubscribeURL", ref lines);
            msisdn = "0" + msisdn.Substring(3);
            string json = "{\"msisdn\": \"" + msisdn + "\",\"toneCode\": \"" + crbt_id + "\"}";
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers() { key = "Authorization", value = token});
            string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    status_code = json_response.statusCode;

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }
            return status_code;

        }
    
    }
}
