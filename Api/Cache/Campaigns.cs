using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class Campaigns
    {
        public static List<CampaignTracking> GetCampaigns(ref List<LogLines> lines)
        {
            List<CampaignTracking> result = null;
            try
            {
                if (HttpContext.Current.Application["CamapignList"] != null)
                {
                    lines = Add2Log(lines, "Cache contains info", 100, lines[0].ControlerName);
                    result = (List<CampaignTracking>)HttpContext.Current.Application["CamapignList"];
                }
                else
                {
                    result = DataLayer.DBQueries.GetCampaigns(ref lines);
                    if (result != null)
                    {
                        lines = Add2Log(lines, "Cache does not contains info", 100, lines[0].ControlerName);
                        HttpContext.Current.Application["CamapignList"] = result;
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, ex.InnerException + " - " + ex.Message, 100, lines[0].ControlerName);
                result = GetCampaigns(ref lines);
            }

            return result;
        }
    }
}