using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.idobet_cache
{
    /// <summary>
    /// Summary description for reload_cache
    /// </summary>
    
    public class reload_cache : IHttpHandler
    {
        private int reload_time_in_seconds = 30;

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            context.Response.ContentType = "text/plain";
            lines = Add2Log(lines, "*****************************", 100, "reload_event_cache");
            string service_id = context.Request.QueryString["service_id"];

            List<SportEvents> sports_events = null;

            string sport_type_id = "31";
            switch (service_id)
            {
                case "732":
                case "716":
                    sport_type_id = "31";
                    sports_events = CommonFuncations.B2TechGNMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "32";
                    sports_events = CommonFuncations.B2TechGNMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "35";
                    sports_events = CommonFuncations.B2TechGNMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    break;
                case "733":
                    sport_type_id = "31";
                    sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "32";
                    sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "35";
                    sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }

                    sport_type_id = "31";
                    sports_events = CommonFuncations.B2TechCGMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "32";
                    sports_events = CommonFuncations.B2TechCGMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "35";
                    sports_events = CommonFuncations.B2TechCGMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }

                    break;
                case "730":
                case "726":
                    lines = Add2Log(lines, "Reloading New Cache", 100, "reload_event_cache");
                    sport_type_id = "31";
                    sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "32";
                    sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }
                    sport_type_id = "35";
                    sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(Convert.ToInt32(sport_type_id), ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(reload_time_in_seconds);
                        context.Response.Write(service_id + "_" + sport_type_id + " is loaded <br>");
                    }

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