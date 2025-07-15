using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class YDTracking
    {
        public static string GetYDTrackingID(HttpContext context, int service_id, ref List<LogLines> lines)
        {
            string ydtrack_id = "";
            if (context.Request.Cookies["ydtrack_id"] != null)
            {
                if (!String.IsNullOrEmpty(context.Request.Cookies["ydtrack_id"].Value))
                {
                    ydtrack_id = context.Request.Cookies["ydtrack_id"].Value;
                    lines = Add2Log(lines, "YDTrackingID from cookie = " + ydtrack_id, 100, "imi_ydgames");
                }
            }
            
            if (String.IsNullOrEmpty(ydtrack_id))
            {
                string ua = context.Request.ServerVariables["HTTP_USER_AGENT"];
                string ip = context.Request.ServerVariables["REMOTE_ADDR"];
                string myQuery = "select tr.id from tracking.tracking_requests tr, tracking.tracking_campaign tc where tc.campaign_id = tr.campaign_id and tc.subscribe_service_id = " + service_id + " and tr.ip = '" + ip + "' and tr.user_agent = '" + ua + "' and tr.msisdn = -1 and datediff(now(), tr.date_time) between 0 and 1 order by tr.id desc limit 1";
                Int64 ydtrack_idInt64 = DataLayer.DBQueries.SelectQueryReturnInt64(myQuery, ref lines);
                lines = Add2Log(lines, "YDTrackingID from DB = " + ydtrack_idInt64, 100, "imi_ydgames");
                ydtrack_id = ydtrack_idInt64.ToString();
            }
            return ydtrack_id;
        }
    }
}