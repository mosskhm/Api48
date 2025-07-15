using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.idobet_cache
{
    /// <summary>
    /// Summary description for reload_tokens
    /// </summary>
    public class reload_tokens : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            context.Response.ContentType = "text/plain";
            lines = Add2Log(lines, "*****************************", 100, "reload_event_cache");
            string service_id = context.Request.QueryString["service_id"];


            switch (service_id)
            {
                case "716":
                    Api.Cache.iDoBet.GetMTNGuineiDoBetUserTokenNew(1, ref lines);
                    Api.Cache.iDoBet.GetMTNGuineaiDoBetUserTokenCheckTicket(1, ref lines);
                    break;
                case "732":
                    Api.Cache.iDoBet.GetOrangeGuineiDoBetUserTokenNew(1, ref lines);
                    Api.Cache.iDoBet.GetOrangeGuineaiDoBetUserTokenCheckTicket(1, ref lines);
                    break;
                case "733":
                    Api.Cache.iDoBet.GetMTNCongoiDoBetUserTokenNew(1, ref lines);
                    Api.Cache.iDoBet.GetMTNCongoiDoBetUserTokenCheckTicket(1, ref lines);
                    break;
                case "726":
                    Api.Cache.iDoBet.GetMTNBeninLNBPiDoBetUserTokenNew(1, ref lines);
                    Api.Cache.iDoBet.GetMTNBeninLNBPiDoBetUserTokenCheckTicket(1, ref lines);
                    break;
                case "730":
                    Api.Cache.iDoBet.GetMoovBeninLNBPiDoBetUserTokenNew(1, ref lines);
                    Api.Cache.iDoBet.GetMoovBeninLNBPiDoBetUserTokenCheckTicket(1, ref lines);
                    break;
            }



            lines = Write2Log(lines);


            context.Response.Write("Done");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}