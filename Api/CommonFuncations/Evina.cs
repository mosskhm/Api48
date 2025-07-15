using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using Newtonsoft.Json;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Evina
    {
        //public static string Hash(string str2hash)
        //{
        //    string result = "";
        //    string encoded = UrlEncode(str2hash);
        //    result = ComputeStringToSha256Hash(encoded);
        //    return result;
        //}

        //static string UrlEncode(string plainText)
        //{
        //    return Uri.EscapeDataString(plainText).Replace("%20", "+");
        //}

        //static string ComputeStringToSha256Hash(string plainText)
        //{
        //    // Create a SHA256 hash from string   
        //    using (SHA256 sha256Hash = SHA256.Create())
        //    {
        //        // Computing Hash - returns here byte array
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

        //        // now convert byte array to a string   
        //        StringBuilder stringbuilder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            stringbuilder.Append(bytes[i].ToString("x2"));
        //        }
        //        return stringbuilder.ToString();
        //    }
        //}


        public static string GetJS(string seo_name, ref List<LogLines> lines)
        {
            string js_script = "";
        //ti=123456&te=sub_button&ts=1667903663&adc=0&so=false&cjv=false&clf=false&pl=
        //Login: yellowdot_africa
        //Password: bJfoJyKKGysWnnWl9B7Pojg6cKE3J9sZ
            string user_name = "yellowdot_africa";
            string password = "bJfoJyKKGysWnnWl9B7Pojg6cKE3J9sZ";

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;
            Int64 ti = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into evina_requests (date_time, seo_name) values (now(), '"+seo_name+"')", ref lines);

            string string_2_encode = user_name + "ti=" + ti + "&te=sub_button&ts=" + secondsSinceEpoch + "&adc=0&so=false&cjv=false&clf=false&pl=" + password;
            string encoded_string = Api.CommonFuncations.CallSoap.GetURL("http://80.241.216.32/myhash.php?val=" + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(string_2_encode)), ref lines);

            //http://api.dcbprotect.com/yellowdot_africa/script?ti=123456&te=sub_button&ts=1667903663&adc=0&so=false&cjv=false&clf=false&pl=&s=65fc4ce09951a5803de09d58e8e63496a38986f33860a3f4a32628405a2b2475
            string url = "http://api.dcbprotect.com/yellowdot_africa/script?ti=" + ti + "&te=sub_button&ts=" + secondsSinceEpoch + "&adc=0&so=false&cjv=false&clf=false&pl=&s=" + encoded_string;
            string js_body = Api.CommonFuncations.CallSoap.GetURL(url, ref lines);
            dynamic json_response = JsonConvert.DeserializeObject(js_body);
            try
            {
                js_script = json_response.s;
            }
            catch(Exception ex)
            {

            }


            return js_script;
        }
        public static string GetJSSA(string seo_name, ref List<LogLines> lines)
        {
            string js_script = "";
            //ti=123456&te=sub_button&ts=1667903663&adc=0&so=false&cjv=false&clf=false&pl=
            //Login: yellowdot_africa
            //Password: bJfoJyKKGysWnnWl9B7Pojg6cKE3J9sZ
            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);
            CampaignTracking campaign = campaigns.Find(x => x.seo_name.ToLower() == seo_name.ToLower());
            string user_name = Cache.ServerSettings.GetServerSettings("EvinaUser", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("EvinaPass", ref lines);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;
            Int64 ti = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into evina_requests (date_time, seo_name) values (now(), '" + seo_name + "')", ref lines);
            string carrier = "";
            string operator_name = "";
            ServiceClass service = new ServiceClass();
            if (campaign != null)
            {
                service = Api.Cache.Services.GetServiceByServiceID(campaign.subscribe_service_id, ref lines);
                operator_name = service.operator_name;
            }
            carrier = Cache.ServerSettings.GetServerSettings("EvinaCarrier" + operator_name, ref lines);
            string base_url = Cache.ServerSettings.GetServerSettings("EvinaTrackingBaseUrl" + operator_name, ref lines);
            string auth_traffic = base_url + seo_name;
            string bad_traffic = "https://tracksa.ydot.co/track/BadTraffic";
            string string_2_encode = "ti=" + ti + "&ru=" + HttpUtility.UrlEncode(auth_traffic) + "&rfu=" + HttpUtility.UrlEncode(bad_traffic) + "&ts=" + secondsSinceEpoch + "&country=ZA&carrier="+carrier+"&service="+service.service_name.Replace(" ","")+"&arg1="+campaign.partner_id;
            //string encoded_string = Api.CommonFuncations.CallSoap.GetURL("http://80.241.216.32/myhashEvina_SA.php?val=" + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(string_2_encode)), ref lines);
            


        //http://api.dcbprotect.com/yellowdot_africa/script?ti=123456&te=sub_button&ts=1667903663&adc=0&so=false&cjv=false&clf=false&pl=&s=65fc4ce09951a5803de09d58e8e63496a38986f33860a3f4a32628405a2b2475
        // http://api.dcbprotect.com/my_evina_user/script?ti=uniqueId1&ru=https%3A%2F%2Fmy-link.com%2Fauthentic%3Ftoken%3DmyUniqueToken&rfu=https%3A%2F%2Fmy-link.com%2Ffraud%3Ftoken%3DmyUniqueToken&ts=1615189391&country=US&s=2d15cb5f40112f32adff581d1f71723593bb0be3878eaa2f7f81e6bf1e96a6da HTTP/1.1
            string url = "https://api.dcbprotect.com/v1/script";
            
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "X-EVINA-APIKEY", value = "KQchiBpJBnuvoZWfCkpPxXsQa2Zg7d5p1FQizL44NphR8aiF1hGG3MFwlqkxti2p" });
            string js_body = Api.CommonFuncations.CallSoap.CallSoapRequest(url,string_2_encode,headers,3, ref lines);
            dynamic json_response = JsonConvert.DeserializeObject(js_body);
            try
            {
                js_script = json_response.s;
                string evina_t_id = json_response.t;
                if (!String.IsNullOrEmpty(evina_t_id))
                {
                    string update_query = "UPDATE evina_requests SET evina_t_id =" +evina_t_id + " Where id="+ ti;
                    Api.DataLayer.DBQueries.ExecuteQuery(update_query, ref lines);
                }
                
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            return js_script;
        }
    }
}