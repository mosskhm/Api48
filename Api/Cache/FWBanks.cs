using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    
    public class FWBanks
    {
        public static List<Banks> GetBanks(ref List<LogLines> lines)
        {
            List<Banks> result_list = new List<Banks>();
            lines = Add2Log(lines, " GetBanks()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBanks"] != null)
                {
                    lines = Add2Log(lines, " GetBanks Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBanks_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBanks_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<Banks>)HttpContext.Current.Application["GetBanks"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBanks Cache ", 100, "");
                            result_list = DBQueries.GetBanks(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetBanks"] = result_list;
                                HttpContext.Current.Application["GetBanks_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBanks Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBanks Cache ", 100, "");
                    result_list = DBQueries.GetBanks(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetBanks"] = result_list;
                        HttpContext.Current.Application["GetBanks_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBanks Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBanks From DB ", 100, "");
                result_list = DBQueries.GetBanks(ref lines);
            }

            return result_list;
        }
    }
}