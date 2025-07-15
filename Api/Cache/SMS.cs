using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class SMS
    {

        public class SMSTexts
        {
            public int service_id { get; set; }
            public string welcome_sms { get; set; }
            public string deactivation_sms { get; set; }
        }


        public static SMSTexts GetSMSText(ServiceClass service, ref List<LogLines> lines)
        {
            SMSTexts result = new SMSTexts();
            List<SMSTexts> result_list = new List<SMSTexts>();
            lines = Add2Log(lines, " GetGetSMSText()", 100, "");
            try
            {
                if (HttpContext.Current.Application["SMSTextList"] != null)
                {
                    lines = Add2Log(lines, " SMSTextList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["SMSTextList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["SMSTextList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<SMSTexts>)HttpContext.Current.Application["SMSTextList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing SMSTextList Cache ", 100, "");
                            result_list = DBQueries.GetSMSTexts(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["SMSTextList"] = result_list;
                                HttpContext.Current.Application["SMSTextList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " SMSTextList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing SMSTextList Cache ", 100, "");
                    result_list = DBQueries.GetSMSTexts(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["SMSTextList"] = result_list;
                        HttpContext.Current.Application["SMSTextList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application SMSTextList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing SMSTextList From DB ", 100, "");
                result_list = DBQueries.GetSMSTexts(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                }
            }
            return result;
        }
    }
}