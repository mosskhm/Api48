using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class iDoBet
    {

        public static List<CashierInfo> GetCashierInfo(ref List<LogLines> lines)
        {
            List<CashierInfo> result = null;
            lines = Add2Log(lines, " GetCashierInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetCashierInfo"] != null)
                {
                    lines = Add2Log(lines, " GetCashierInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetCashierInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetCashierInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<CashierInfo>)HttpContext.Current.Application["GetCashierInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetCashierInfo Cache ", 100, "");
                            result = Api.DataLayer.DBQueries.GetCashierInfo(ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["GetCashierInfo"] = result;
                                HttpContext.Current.Application["GetCashierInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetCashierInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetCashierInfo Cache ", 100, "");
                    result = Api.DataLayer.DBQueries.GetCashierInfo(ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["GetCashierInfo"] = result;
                        HttpContext.Current.Application["GetCashierInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetCashierInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetCashierInfo From DB ", 100, "");
                result = Api.DataLayer.DBQueries.GetCashierInfo(ref lines);
            }

            return result;
        }

        public static List<iDoBetAgents> GetiDoBetAgents(ref List<LogLines> lines)
        {
            List<iDoBetAgents> result = null;
            lines = Add2Log(lines, " GetiDoBetAgents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetiDoBetAgents"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetAgents Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetAgents_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetAgents_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<iDoBetAgents>)HttpContext.Current.Application["GetiDoBetAgents"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetAgents Cache ", 100, "");
                            result = Api.DataLayer.DBQueries.GetiDoBetAgents(ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["GetiDoBetAgents"] = result;
                                HttpContext.Current.Application["GetiDoBetAgents_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetAgents Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetAgents Cache ", 100, "");
                    result = Api.DataLayer.DBQueries.GetiDoBetAgents(ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["GetiDoBetAgents"] = result;
                        HttpContext.Current.Application["GetiDoBetAgents_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetAgents Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetAgents From DB ", 100, "");
                result = Api.DataLayer.DBQueries.GetiDoBetAgents(ref lines);
            }

            return result;
        }

        public static List<FootSoldiers> GetFootSoldiers(ref List<LogLines> lines)
        {
            List<FootSoldiers> result = null;
            lines = Add2Log(lines, " GetFootSoldiers()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetFootSoldiers"] != null)
                {
                    lines = Add2Log(lines, " GetFootSoldiers Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetFootSoldiers_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetFootSoldierss_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<FootSoldiers>)HttpContext.Current.Application["GetFootSoldiers"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetFootSoldiers Cache ", 100, "");
                            result = Api.DataLayer.DBQueries.GetFootSoldiers(ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["GetFootSoldiers"] = result;
                                HttpContext.Current.Application["GetFootSoldiers_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetFootSoldiers Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetFootSoldiers Cache ", 100, "");
                    result = Api.DataLayer.DBQueries.GetFootSoldiers(ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["GetFootSoldiers"] = result;
                        HttpContext.Current.Application["GetFootSoldiers_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetFootSoldiers Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetFootSoldiers From DB ", 100, "");
                result = Api.DataLayer.DBQueries.GetFootSoldiers(ref lines);
            }

            return result;
        }

        public static List<iDoBetLeague> GetiDoBetLeagues(ref List<LogLines> lines)
        {
            List<iDoBetLeague> result = null;
            lines = Add2Log(lines, " GetiDoBetLeagues()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetiDoBetLeagues"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetLeagues Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetLeagues_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetLeagues_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<iDoBetLeague>)HttpContext.Current.Application["GetiDoBetLeagues"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetLeagues Cache ", 100, "");
                            result = GetiDobetMainLegues(ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["GetiDoBetLeagues"] = result;
                                HttpContext.Current.Application["GetiDoBetLeagues_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetLeagues Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetLeagues Cache ", 100, "");
                    result = GetiDobetMainLegues(ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["GetiDoBetLeagues"] = result;
                        HttpContext.Current.Application["GetiDoBetLeagues_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetLeagues Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetLeagues From DB ", 100, "");
                result = GetiDobetMainLegues(ref lines);
            }

            return result;
        }



        public static List<SportEvents> GetEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {
            
            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetEvents_"+sport_type_id+" Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetEvents_"+sport_type_id+"_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id +" Cache ", 100, "");
                            sports_events = GetEvents(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                if (sports_events.Count > 0)
                                {
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] = sports_events;
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                else
                                {
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetEvents_" + sport_type_id +" Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id +" Cache ", 100, "");
                    sports_events = GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        if (sports_events.Count > 0)
                        {
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] = sports_events;
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        else
                        {
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetEvents_"+sport_type_id+" Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetEvents(sport_type_id, ref lines);
            }

            return sports_events;
            
        }

        public static List<SportEvents> GetEventsFromCacheNew(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetEventsNew(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                if (sports_events.Count > 0)
                                {
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] = sports_events;
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                else
                                {
                                    HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetEventsNew(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        if (sports_events.Count > 0)
                        {
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id] = sports_events;
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        else
                        {
                            HttpContext.Current.Application["GetiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetEventsNew(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" +selected_league_id +"_"+ sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                if (sports_events.Count > 0)
                                {
                                    HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id] = sports_events;
                                    HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                else
                                {
                                    HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        if (sports_events.Count > 0)
                        {
                            HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id] = sports_events;
                            HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        else
                        {
                            HttpContext.Current.Application["GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetEventsWithLeaguID_" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }



        public static string GetiDoBetUserToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetUserToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetUserToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBet.GetToken(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserToken"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetUserToken"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetUserToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetUserToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetUserToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetUserToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetUserToken Cache ", 100, "");
                            token_id = CommonFuncations.iDoBet.GetToken(ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetUserToken"] = token_id;
                                HttpContext.Current.Application["GetiDoBetUserToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetUserToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetUserToken Cache ", 100, "");
                    token_id = CommonFuncations.iDoBet.GetToken(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserToken"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetUserToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing USSDMenuList From DB ", 100, "");
                token_id = CommonFuncations.iDoBet.GetToken(ref lines);
            }
            
            return token_id;
        }

        public static string GetiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetUserTokenNEW()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetUserTokenNEW Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBet.GetTokenNew(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserTokenNEW"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserTokenNEW_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetUserTokenNEW"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetUserTokenNEW Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetUserTokenNEW_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetUserTokenNEW_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetUserTokenNEW"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetUserTokenNEW Cache ", 100, "");
                            token_id = CommonFuncations.iDoBet.GetTokenNew(ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetUserTokenNEW"] = token_id;
                                HttpContext.Current.Application["GetiDoBetUserTokenNEW_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetUserTokenNEW Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetUserTokenNEW Cache ", 100, "");
                    token_id = CommonFuncations.iDoBet.GetTokenNew(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserTokenNEW"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserTokenNEW_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetUserTokenNEW Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetUserTokenNEW From DB ", 100, "");
                token_id = CommonFuncations.iDoBet.GetTokenNew(ref lines);
            }

            return token_id;
        }

        public static string GetiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBet.GetTokenCheckTicketNew(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBet.GetTokenCheckTicketNew(ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBet.GetTokenCheckTicketNew(ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBet.GetTokenCheckTicketNew(ref lines);
            }

            return token_id;
        }

        public static string GetiMoovDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiMoovDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiMoovDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetiMoovDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiMoovDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiMoovDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiMoovDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiMoovDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetiMoovDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiMoovDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiMoovDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMooviDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetMoovUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword", ref lines), ref lines);
            }

            return token_id;
        }




        public static string GetiMoovDoBetUserTokenNew_STG(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiMoovDoBetUserTokenNewSTG()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNewSTG Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG"] = token_id;
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG"] != null)
                {
                    lines = Add2Log(lines, " GetiMoovDoBetUserTokenNewSTG Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNewSTG Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword_STG", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG"] = token_id;
                                HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiMoovDoBetUserTokenNewSTG Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNewSTG Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG"] = token_id;
                        HttpContext.Current.Application["GetiMoovDoBetUserTokenNewSTG_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiMoovDoBetUserTokenNewSTG Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiMoovDoBetUserTokenNewSTG From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovLoginPassword_STG", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMooviDoBetUserTokenCheckTicket_STG(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicketSTG()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicketSTG Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword_STG", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicketSTG Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicketSTG Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword_STG", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG"] = token_id;
                                HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetMoovUserTokenCheckTicketSTG Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicketSTG Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovUserTokenCheckTicketSTG_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetMoovUserTokenCheckTicketSTG Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetMoovUserTokenCheckTicketSTG From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetMoovCheckTicketLoginPassword_STG", ref lines), ref lines);
            }

            return token_id;
        }


        //IdoBet QA 
        public static string GetSTGiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetSTGiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetSTGiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetSTGiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSTGiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSTGiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetSTGiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword_STG", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetSTGiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetSTGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetSTGiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSTGiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPassword_STG", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetSTGiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetSTGiDoBetUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword_STG", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetSTGiDoBetUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword_STG", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetSTGiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword_STG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetSTGiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSTGiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSTGiDoBetUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMoovQA.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserName_STG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPassword_STG", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetSTGEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetSTGEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetSTGEventsFromCache_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSTGEventsFromCache_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetSTGEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSTGEventsFromCache_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetSTGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSTGEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSTGEventsFromCache_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetSTGEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetSTGEventsWithLeagueIDFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSTGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMoovQA.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }


        //MTN Guinea
        public static string GetMTNGuineiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNGuineaiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNGuineaiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNGNCOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNGNCOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNGNCOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGNCOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNGNCOToken"] != null)
                {
                    lines = Add2Log(lines, " GetMTNGNCOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNGNCOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGNCOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNGNCOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNGNCOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142CO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNGNCOToken"] = token_id;
                                HttpContext.Current.Application["GetMTNGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNGNCOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNGNCOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGNCOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNGNCOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNGNCOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142CO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNCGCOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCGCOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCGCOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCGCOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCGCOToken"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCGCOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCGCOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCGCOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCGCOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCGCOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142CO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCGCOToken"] = token_id;
                                HttpContext.Current.Application["GetMTNCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCGCOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCGCOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCGCOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCGCOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCGCOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142CO", ref lines), ref lines);
            }

            return token_id;
        }
        public static string GetAirtelCGCOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetAirtelCGCOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetAirtelCGCOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetAirtelCGCOToken"] = token_id;
                        HttpContext.Current.Application["GetAirtelCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetAirtelCGCOToken"] != null)
                {
                    lines = Add2Log(lines, " GetAirtelCGCOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetAirtelCGCOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetAirtelCGCOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetAirtelCGCOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetAirtelCGCOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142CO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetAirtelCGCOToken"] = token_id;
                                HttpContext.Current.Application["GetAirtelCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetAirtelCGCOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetAirtelCGCOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetAirtelCGCOToken"] = token_id;
                        HttpContext.Current.Application["GetAirtelCGCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetAirtelCGCOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetAirtelCGCOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142CO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNBNLNBToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNBNLNBToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNBNLNBToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechLNBMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB142", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNBNLNBToken"] = token_id;
                        HttpContext.Current.Application["GetMTNBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNBNLNBToken"] != null)
                {
                    lines = Add2Log(lines, " GetMTNBNLNBToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNBNLNBToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNBNLNBToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNBNLNBToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNBNLNBToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechLNBMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB142", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNBNLNBToken"] = token_id;
                                HttpContext.Current.Application["GetMTNBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNBNLNBToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNBNLNBToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechLNBMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB142", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNBNLNBToken"] = token_id;
                        HttpContext.Current.Application["GetMTNBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNBNLNBToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNBNLNBToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechLNBMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB142", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMoovBNLNBToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMoovBNLNBToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMoovBNLNBToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechLNBMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB142", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMoovBNLNBToken"] = token_id;
                        HttpContext.Current.Application["GetMoovBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMoovBNLNBToken"] != null)
                {
                    lines = Add2Log(lines, " GetMoovBNLNBToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMoovBNLNBToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMoovBNLNBToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMoovBNLNBToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMoovBNLNBToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechLNBMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB142", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMoovBNLNBToken"] = token_id;
                                HttpContext.Current.Application["GetMoovBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMoovBNLNBToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMoovBNLNBToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechLNBMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB142", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMoovBNLNBToken"] = token_id;
                        HttpContext.Current.Application["GetMoovBNLNBToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMoovBNLNBToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMoovBNLNBToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechLNBMoov.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB142", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB142", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetOrangeGNCOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetOrangeGNCOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetOrangeGNCOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGNCOToken"] = token_id;
                        HttpContext.Current.Application["GetOrangeGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetOrangeGNCOToken"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeGNCOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeGNCOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeGNCOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetOrangeGNCOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeGNCOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142CO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetOrangeGNCOToken"] = token_id;
                                HttpContext.Current.Application["GetOrangeGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeGNCOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeGNCOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142CO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGNCOToken"] = token_id;
                        HttpContext.Current.Application["GetOrangeGNCOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeGNCOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeGNCOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142CO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142CO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetOrangeGNPOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetOrangeGNPOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetOrangeGNPOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGNPOToken"] = token_id;
                        HttpContext.Current.Application["GetOrangeGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetOrangeGNPOToken"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeGNPOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeGNPOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeGNPOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetOrangeGNPOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeGNPOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142PO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetOrangeGNPOToken"] = token_id;
                                HttpContext.Current.Application["GetOrangeGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeGNPOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeGNPOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGNPOToken"] = token_id;
                        HttpContext.Current.Application["GetOrangeGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeGNPOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeGNPOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea142PO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNGNPOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNGNPOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNGNPOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGNPOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNGNPOToken"] != null)
                {
                    lines = Add2Log(lines, " GetMTNGNPOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNGNPOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGNPOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNGNPOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNGNPOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142PO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNGNPOToken"] = token_id;
                                HttpContext.Current.Application["GetMTNGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNGNPOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNGNPOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNGNPOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNGNPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNGNPOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNGNPOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechGNMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNGuinea142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNGuinea142PO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNCGPOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCGPOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCGPOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCGPOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCGPOToken"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCGPOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCGPOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCGPOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCGPOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCGPOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142PO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCGPOToken"] = token_id;
                                HttpContext.Current.Application["GetMTNCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCGPOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCGPOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCGPOToken"] = token_id;
                        HttpContext.Current.Application["GetMTNCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCGPOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCGPOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechCGMTN.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo142PO", ref lines), ref lines);
            }

            return token_id;
        }
        public static string GetAirtelCGPOToken(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetAirtelCGPOToken()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetAirtelCGPOToken Cache (force renew)", 100, "");
                    token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetAirtelCGPOToken"] = token_id;
                        HttpContext.Current.Application["GetAirtelCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetAirtelCGPOToken"] != null)
                {
                    lines = Add2Log(lines, " GetAirtelCGPOToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetAirtelCGPOToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetAirtelCGPOToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetAirtelCGPOToken"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetAirtelCGPOToken Cache ", 100, "");
                            token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142PO", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetAirtelCGPOToken"] = token_id;
                                HttpContext.Current.Application["GetAirtelCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetAirtelCGPOToken Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetAirtelCGPOToken Cache ", 100, "");
                    token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142PO", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetAirtelCGPOToken"] = token_id;
                        HttpContext.Current.Application["GetAirtelCGPOToken_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetAirtelCGPOToken Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetAirtelCGPOToken From DB ", 100, "");
                token_id = CommonFuncations.B2TechCGAirtel.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameAirtelCongo142PO", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordAirtelCongo142PO", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNGuineaiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetMTNGuineaUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetMTNGuineaUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNGuinea", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetMTNGuineaUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetMTNGuineaUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNGuinea", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetMTNGuineaUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetMTNGuineaUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMTNGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetMTNGuineaUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetMTNGuineaUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNGuinea", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetMTNGuineaEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNGuineaiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id];
                    //if (HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetGNEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNGuineaiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetGNEventsFromCache_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id];
                    //if (HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetGNEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetGNEventsFromCache_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechGNMTN.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetGNEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(2);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetGNEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetGNEventsFromCache_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechGNMTN.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetCGEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetCGEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetCGEventsFromCache_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id];
                    //if (HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetCGEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetCGEventsFromCache_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechCGMTN.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetCGEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(2);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetCGEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetCGEventsFromCache_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechCGMTN.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetLNBEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetLNBEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetLNBEventsFromCache_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id];
                    //if (HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMTNGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetLNBEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetLNBEventsFromCache_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetLNBEventsFromCache_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(2);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetLNBEventsFromCache_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetLNBEventsFromCache_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetMTNGuineaEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNGuineaiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetGNEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetGNEventsWithLeagueIDFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.B2TechGNMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechGNMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetGNEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechGNMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetCGEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetCGEventsWithLeagueIDFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.B2TechCGMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechCGMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetCGEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechCGMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetLNBEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetLNBEventsWithLeagueIDFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetLNBEventsWithLeagueIDFromCache" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.B2TechLNBMTN.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        //Orange Guinea
        public static string GetOrangeGuineiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetOrangeGuineaiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeGuineaiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeGuinea", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetOrangeGuineaiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetOrangeGuineaUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetOrangeGuineaUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeGuinea", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetOrangeGuineaUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetOrangeGuineaUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeGuinea", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetOrangeGuineaUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetOrangeGuineaUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeGuinea", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetOrangeGuineaUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetOrangeGuineaUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetOrangeGuineaUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetOrangeGuinea.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeGuinea", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeGuinea", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetOrangeGuineaEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetOrangeGuineaiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id];
                    //if (HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeGuineaiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetOrangeGuineaEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetOrangeGuineaiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeGuineaiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetOrangeGuinea.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }


        //MTNBenin LNB
        public static string GetMTNBeninLNBPiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing iDoBetLoginPasswordMTNBeninLNB Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNBeninLNBPiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNBeninLNB", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNBeninLNBPiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetMTNBeninLNBPUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNBeninLNB", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNBeninLNB", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMTNBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetMTNBeninLNBPUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetMTNBeninLNBPUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetLNBPariMTNBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNBeninLNB", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetMTNBeninLNBPEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id];

                    //if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetMTNBeninLNBPEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetLNBPariMTNBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }


        //MTNMoov LNB
        public static string GetMoovBeninLNBPiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMoovBeninLNBPiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMoovBeninLNB", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMoovBeninLNBPiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetiDoBetMoovBeninLNBPUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMoovBeninLNB", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMoovBeninLNB", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMoovBeninLNB", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetiDoBetMoovBeninLNBPUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetiDoBetMoovBeninLNBPUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetiDoBetMoovBeninLNBPUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetLNBPariMoovBenin.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMoovBeninLNB", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMoovBeninLNB", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetMoovBeninLNBPEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEvents()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id];

                    //if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    //{
                    //    DateTime expdate = (DateTime)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"];
                    //    lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                    //    if (DateTime.Now < expdate)
                    //    {
                    //        sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id];
                    //    }
                    //    else
                    //    {
                    //        lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    //        sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(sport_type_id, ref lines);
                    //        if (sports_events != null)
                    //        {
                    //            HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id] = sports_events;
                    //            HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    //        }

                    //    }
                    //}

                }
                else
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetMoovBeninLNBPEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMoovBeninLNBPiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetLNBPariMoovBenin.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        //MTN Cameroon
        public static string GetMTNCamerooniDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCameroon", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCamerooniDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCameroon", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNCamerooniDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCameroon", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCameroon", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCamerooniDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCameroon", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetMTNCameroonEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNCameroonEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCamerooniDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetMTNCameroonEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNCamerooniDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        //Orange Orange
        public static string GetOrangeCamerooniDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeCameroon", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeCamerooniDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordOrangeCameroon", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetOrangeCamerooniDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeCameroon", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeCameroon", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeCameroon", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeCamerooniDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetOrangeCameroon.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameOrangeCameroon", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordOrangeCameroon", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetOrangeCameroonEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetOrangeCameroonEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeCamerooniDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetOrangeCameroonEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetOrangeCamerooniDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                        HttpContext.Current.Application["GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeCamerooniDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetOrangeCameroon.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }

        //MTN NG
        public static string GetMTNNGiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNNGiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNNGiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNNG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNNG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMTNNGiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNNGiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNNG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNNG", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNNGiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNNGiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNNG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNNG", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNNGiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNNGiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNNGiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNNG", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNNG", ref lines), ref lines);
            }

            return token_id;
        }

        //MTN Congo
        public static string GetMTNCongoiDoBetUserTokenNew(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenNew()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenNew Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenNew Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenNew Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew"] = token_id;
                                HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenNew Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenNew Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew"] = token_id;
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenNew_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCongoiDoBetUserTokenNew Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenNew From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetLoginPasswordMTNCongo", ref lines), ref lines);
            }

            return token_id;
        }

        public static string GetMTNCongoiDoBetUserTokenCheckTicket(int force_renew, ref List<LogLines> lines)
        {
            string token_id = "";
            lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenCheckTicket()", 100, "");
            try
            {
                if (force_renew == 1)
                {
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenCheckTicket Cache (force renew)", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCongo", ref lines), ref lines);

                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
                if (HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket"] != null)
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenCheckTicket Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token_id = (string)HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenCheckTicket Cache ", 100, "");
                            token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCongo", ref lines), ref lines);
                            if (token_id != null)
                            {
                                HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket"] = token_id;
                                HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenCheckTicket Cache ", 100, "");
                    token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCongo", ref lines), ref lines);
                    if (token_id != null)
                    {
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket"] = token_id;
                        HttpContext.Current.Application["GetMTNCongoiDoBetUserTokenCheckTicket_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCongoiDoBetUserTokenCheckTicket Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetUserTokenCheckTicket From DB ", 100, "");
                token_id = CommonFuncations.iDoBetMTNCongo.GetTokenNew(Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginUserNameMTNCongo", ref lines), Cache.ServerSettings.GetServerSettings("iDoBetCheckTicketLoginPasswordMTNCongo", ref lines), ref lines);
            }

            return token_id;
        }


        public static List<SportEvents> GetMTNCongoEventsFromCache(int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNCongoEventsFromCache()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetEvents_" + sport_type_id + " Cache contains Info", 100, "");
                    sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id];

                    if (HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                if (sports_events.Count > 0)
                                {
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] = sports_events;
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                else
                                {
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEvents_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        if (sports_events.Count > 0)
                        {
                            HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id] = sports_events;
                            HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        else
                        {
                            HttpContext.Current.Application["GetMTNCongoiDoBetEvents_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCongoiDoBetEvents_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEvents_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(sport_type_id, ref lines);
            }

            return sports_events;

        }

        public static List<SportEvents> GetMTNCongoEventsWithLeagueIDFromCache(int selected_league_id, int sport_type_id, ref List<LogLines> lines)
        {

            List<SportEvents> sports_events = null;

            lines = Add2Log(lines, " GetMTNCongoiDoBetEventsWithLeageID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] != null)
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            sports_events = (List<SportEvents>)HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                            sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(selected_league_id, sport_type_id, ref lines);
                            if (sports_events != null)
                            {
                                if (sports_events.Count > 0)
                                {
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                else
                                {
                                    HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                                }
                                
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache ", 100, "");
                    sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(selected_league_id, sport_type_id, ref lines);
                    if (sports_events != null)
                    {
                        if (sports_events.Count > 0)
                        {
                            HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id] = sports_events;
                            HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        else
                        {
                            HttpContext.Current.Application["GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + "_expdate"] = DateTime.Now.AddMinutes(5);
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMTNCongoiDoBetEventsWithLeageID" + selected_league_id + "_" + sport_type_id + " From DB ", 100, "");
                sports_events = CommonFuncations.iDoBetMTNCongo.GetEvents(selected_league_id, sport_type_id, ref lines);
            }

            return sports_events;

        }



    }
}