using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class FS
    {
        public static List<FootSoldiers> GetFootSoldiers(ref List<LogLines> lines)
        {
            List<FootSoldiers> result_list = new List<FootSoldiers>();
            lines = Add2Log(lines, " GetFootSoldiers()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetFootSoldiers"] != null)
                {
                    lines = Add2Log(lines, " GetFootSoldiers Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetFootSoldiers_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetFootSoldiers_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<FootSoldiers>)HttpContext.Current.Application["GetFootSoldiers"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetFootSoldiers Cache ", 100, "");
                            result_list = DBQueries.GetFootSoldiers(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetFootSoldiers"] = result_list;
                                HttpContext.Current.Application["GetFootSoldiers_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetFootSoldiers Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetFootSoldiers Cache ", 100, "");
                    result_list = DBQueries.GetFootSoldiers(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetFootSoldiers"] = result_list;
                        HttpContext.Current.Application["GetFootSoldiers_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetFootSoldiers Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetFootSoldiers From DB ", 100, "");
                result_list = DBQueries.GetFootSoldiers(ref lines);
            }

            return result_list;
        }
    }
}