using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class IC
    {
        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            //msisdn = msisdn.Substring(3);
            string url = Cache.ServerSettings.GetServerSettings("MTNICKannelURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + System.Uri.EscapeDataString(text) + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "");
            lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }
    }
}