using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class CashToken
    {
        public static bool SendCashToken(string MSISDN, int service_id, string token_source, ref List<LogLines> lines)
        {
            bool result = true;
            string json = "{\"msisdn\": \""+ MSISDN + "\",\"serviceID\":" + service_id+",\"tokenSource\": \""+ token_source + "\"}";
            string url = "https://cashtoken.ydplatform.com/api/CashToken/Gifting/GiftCustomer";
            List<Headers> headers = new List<Headers>();
            string error = "";
            string body = CallSoap.CallSoapRequest(url, json, headers, 5, out error, ref lines);
            return result;
        }

        public static bool SendWinBackCampaign(string MSISDN, int service_id, ref List<LogLines> lines)
        {
            bool result = true;
            string json = "{\"msisdn\": \"" + MSISDN + "\",\"serviceID\":\"" + service_id + "\",\"subscriptionDate\": \"" + DateTime.Now.ToString("yyyy-MM-dd") + "\"}";
            string url = "https://winback.ydplatform.com/api/GloWinbackRewards/Rewards/RewardSubscriber";
            List<Headers> headers = new List<Headers>();
            string error = "";
            string body = CallSoap.CallSoapRequest(url, json, headers, 5, out error, ref lines);
            return result;
        }
    }
}