using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.BtoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class BtoBet
    {
        public static List<EventOdds> GetEventOddFromCache(ServiceClass service, string game_id, ref List<LogLines> lines)
        {

            List<EventOdds> game_events = null;

            lines = Add2Log(lines, " GetGameOdds()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id] != null)
                {
                    lines = Add2Log(lines, " GetGameOdds_" + service.service_id + "_" + game_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            game_events = (List<EventOdds>)HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetGameOdds_" + service.service_id + "_" + game_id + " Cache ", 100, "");
                            game_events = GetGameOdds(service, game_id, ref lines);
                            if (game_events != null)
                            {
                                HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id] = game_events;
                                HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetGameOdds_" + service.service_id + "_" + game_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetGameOdds_" + service.service_id + "_" + game_id + " Cache ", 100, "");
                    game_events = GetGameOdds(service, game_id, ref lines);
                    if (game_events != null)
                    {
                        HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id] = game_events;
                        HttpContext.Current.Application["GetGameOdds_" + service.service_id + "_" + game_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetGameOdds_" + service.service_id + "_" + game_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetGameOdds_" + service.service_id + "_" + game_id + " From DB ", 100, "");
                game_events = GetGameOdds(service, game_id, ref lines);
            }

            return game_events;

        }

        public static List<SportEvents> GetEventsFromCache(ServiceClass service, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetBtoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetEvents(service, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetEvents(service, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBtoBetEvents_" + service.service_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetEvents(service, sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetHighLightsEventsFromCache(ServiceClass service, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetBtoBetHighLightsEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetHighlightsEvents(service, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetHighlightsEvents(service, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBtoBetHighLightsEvents_" + service.service_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetHighlightsEvents(service, sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetHighLightsEventsWithLeagueIDFromCache(ServiceClass service, int sport_type_id, int selected_league_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetHighLightsEventsWithLeagueIDFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetHighlightsEvents(service, sport_type_id, selected_league_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetHighlightsEvents(service, sport_type_id, selected_league_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetHighLightsEventsWithLeagueIDFromCache_" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetHighlightsEvents(service, sport_type_id, selected_league_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetEventsWithLeagueIDFromCache(ServiceClass service, int sport_type_id, int selected_league_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetBtoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetEvents(service, sport_type_id, selected_league_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetEvents(service, sport_type_id, selected_league_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBtoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetEvents(service, sport_type_id, selected_league_id, ref lines);
            }

            return sports_events;

        }

        public static List<BtoBetLeague> GetBtoBetLeagues(ref List<LogLines> lines)
        {
            List<BtoBetLeague> result = null;
            try
            {
                if (HttpContext.Current.Application["GetBtoBetLeagues"] != null)
                {
                    lines = Add2Log(lines, " GetBtoBetLeagues Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBtoBetLeagues_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBtoBetLeagues_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<BtoBetLeague>)HttpContext.Current.Application["GetBtoBetLeagues"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBtoBetLeagues Cache ", 100, "");
                            result = GetBtoBetMainLegues(ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["GetBtoBetLeagues"] = result;
                                HttpContext.Current.Application["GetBtoBetLeagues_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetBtoBetLeagues Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBtoBetLeagues Cache ", 100, "");
                    result = GetBtoBetMainLegues(ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["GetBtoBetLeagues"] = result;
                        HttpContext.Current.Application["GetBtoBetLeagues_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBtoBetLeagues Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBtoBetLeagues From DB ", 100, "");
                result = GetBtoBetMainLegues(ref lines);
            }

            return result;
        }
    }
}