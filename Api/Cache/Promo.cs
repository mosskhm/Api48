using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class Promo
    {
        public static bool FindPromoBlockedUsers(Int64 msisdn, ref List<LogLines> lines)
        {

            bool result = false;
            List<Int64> result_list = new List<Int64>();
            lines = Add2Log(lines, " GetBlockedUsers()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBlockedUsers"] != null)
                {
                    lines = Add2Log(lines, " GetBlockedUsers Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBlockedUsers_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBlockedUsers_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<Int64>)HttpContext.Current.Application["GetBlockedUsers"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBlockedUsers Cache ", 100, "");
                            result_list = DBQueries.GetPromoBlockedUsers(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetBlockedUsers"] = result_list;
                                HttpContext.Current.Application["GetBlockedUsers_expdate"] = DateTime.Now.AddHours(10);
                            }
                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBlockedUsers Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBlockedUsers Cache ", 100, "");
                    result_list = DBQueries.GetPromoBlockedUsers(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetBlockedUsers"] = result_list;
                        HttpContext.Current.Application["GetBlockedUsers_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBlockedUsers Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBlockedUsers From DB ", 100, "");
                result_list = DBQueries.GetPromoBlockedUsers(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    if (result_list.Contains(msisdn))
                    {
                        result = true;
                    }

                }
            }
            return result;
        }
    }
}