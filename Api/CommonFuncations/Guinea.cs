using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Guinea
    {
        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, string transaction_id, string service_id, ref List<LogLines> lines)
        {
            switch (short_code)
            {
                case "343":
                    short_code = "YELLOWBETGN";
                    break;
            }
            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("GuineaKannelURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + System.Uri.EscapeDataString(text) + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "") + "&dlr-url=http%3A%2F%2F192.168.1.2%2Fdlr%2Fkannel_dlr.ashx%3Fstatus%3D%25d%26msg_id%3D" + transaction_id + "%26service_id%3D" + service_id;
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