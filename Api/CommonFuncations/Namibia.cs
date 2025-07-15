using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Namibia
    {
        public static bool SendSMSNamb(string msisdn, string text, ref List<LogLines> lines)
        {
            //switch (short_code)
            //{
            //    case "343":
            //        short_code = "YellowBet";
            //        break;
            //}
            
            bool sms_sent = false;
            string url_status_response = null;
            string url_status_check = null;
            string url = Cache.ServerSettings.GetServerSettings("NamibiaSendSmsURL", ref lines) + "key=" + Cache.ServerSettings.GetServerSettings("NamibiaSmsApiToken", ref lines) + "&action=sendsms&to=" + msisdn + "&msg=" + text;
            lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMSNamb");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("success")/*This success just means it has logged into db successfully*/ && send_sms.Contains("Pending") /*This means it is pending with MNO*/)
            {
                url_status_check = Cache.ServerSettings.GetServerSettings("NamibiaSendSmsURL", ref lines) + "key=" + Cache.ServerSettings.GetServerSettings("NamibiaSmsApiToken", ref lines) + "&action=status&msgid_list=" + send_sms.Substring(8,8);
                lines = Add2Log(lines, "SMS Url = " + url_status_check, 100, "SendSMSNamb");
                url_status_response = CallSoap.GetURL(url_status_check, ref lines);
            }
           
            for (int i = 0; i < 3; i++)
            {
                if (url_status_response.Contains("ACCEPTED"))
                {
                    sms_sent = true;
                    break;
                }
                else
                {
                    Thread.Sleep(2500);
                    url_status_response = CallSoap.GetURL(url_status_check, ref lines);
                 
                }
                
                
            }
            
            
            return sms_sent;
        }
          
    } 
}
        
            
     