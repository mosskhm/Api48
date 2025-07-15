using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class STS
    {
        public static string CreateURL(ServiceClass service_sts, SubscribeRequest RequestBody, ref List<LogLines> lines)
        {
            string url = "";
            Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into requests (msisdn, date_time, subscription_method_id, pin_code, service_id, transaction_id) values (" + RequestBody.MSISDN + ", now(), 0, 0, " + RequestBody.ServiceID + ",'" + RequestBody.TransactionID + "')", ref lines);
            if (req_id > 0)
            {
               url = Cache.ServerSettings.GetServerSettings("STSBaseURL", ref lines) + "Traffic/" + service_sts.spid_password + "/?ref=" + req_id;
            }

            return url;
        }

        public static bool SubscribeUser(ServiceClass service_sts, SubscribeRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            string url = "";
            Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into requests (msisdn, date_time, subscription_method_id, pin_code, service_id, transaction_id) values (" + RequestBody.MSISDN + ", now(), 0, 0, " + RequestBody.ServiceID + ",'" + RequestBody.TransactionID + "')", ref lines);
            if (req_id > 0)
            {
                url = Cache.ServerSettings.GetServerSettings("STSBaseURL", ref lines) + "SMS/" + service_sts.spid_password + "/"+RequestBody.MSISDN+"/?ref=" + req_id;
                string response = CallSoap.GetURL(url, ref lines);
                if (response.ToLower() == "true")
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool SendSMS(ServiceClass service, SendSMSRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            //http://sdp.smartcalltech.co.za/SENDSMS?id=6c677e15-d5b6-49e1-af27-ec5893d05d4e&msisdn=27784164170&message=This%20is%20a%20test%20with%20a%20URL%20https%3A%2F%2Fedition.cnn.com%2F
            string url = Cache.ServerSettings.GetServerSettings("STSBaseURL", ref lines) + "SENDSMS?id=" + service.spid_password + "&msisdn=" + RequestBody.MSISDN + "&message=" + HttpUtility.UrlEncode(RequestBody.Text);
            string response = CallSoap.GetURL(url, ref lines);
            if (response.ToLower() == "true")
            {
                result = true;
            }

            return result;
        }



    }
}