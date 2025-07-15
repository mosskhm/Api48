using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class USSD
    {
        public class USSDMainCode
        {
            public int ussd_id { get; set; }
            public string SPID { get; set; }
            public string service_code { get; set; }
        }

        public class USSDMenu
        {
            public int menu_id { get; set; }
            public int ussd_id { get; set; }
            public string ussd_string { get; set; }
            public string menu_2_display { get; set; }
            public int action_id { get; set; }
            public string action_name { get; set; }
            public int prev_action_id { get; set; }
            public int service_id { get; set; }
            public int topic_id { get; set; }
            public string topic_name { get; set; }
            public bool is_special { get; set; }
            public string menu_2_display_ln { get; set; }


        }

        public class USSDBonus
        {
            public string bonus_name { get; set; }
            public int amount { get; set; }
            public int min_selections { get; set; }
            public double min_odd_per_selections { get; set; }
            public double min_total_odd { get; set; }
            public string bonus_user_id { get; set; }
            public string msisdn { get; set; }
            public string service_id { get; set; }
        }

        public static List<USSDBonus> GetUSSDBonusPerService(string service_id, ref List<LogLines> lines)
        {
            List<USSDBonus> result_list = new List<USSDBonus>();
            lines = Add2Log(lines, " GetUSSDBonusPerService()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetUSSDBonusPerService"] != null)
                {
                    lines = Add2Log(lines, " GetUSSDBonusPerService Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetUSSDBonusPerService_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetUSSDBonusPerService_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<USSDBonus>)HttpContext.Current.Application["GetUSSDBonusPerService"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetUSSDBonusPerService Cache ", 100, "");
                            result_list = DBQueries.GetUSSDBonus(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetUSSDBonusPerService"] = result_list;
                                HttpContext.Current.Application["GetUSSDBonusPerService_expdate"] = DateTime.Now.AddMinutes(1);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetUSSDBonusPerService Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetUSSDBonusPerService Cache ", 100, "");
                    result_list = DBQueries.GetUSSDBonus(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetUSSDBonusPerService"] = result_list;
                        HttpContext.Current.Application["GetUSSDBonusPerService_expdate"] = DateTime.Now.AddMinutes(1);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetUSSDBonusPerService Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetUSSDBonusPerService From DB ", 100, "");
                result_list = DBQueries.GetUSSDBonus(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result_list = result_list.Where(x => x.service_id == service_id ).ToList();
                    
                }
            }
            return result_list;
        }


        public static USSDMainCode GetUSSDMainCodeID(string SPID, string service_code, ref List<LogLines> lines)
        {
            USSDMainCode result = new USSDMainCode();
            List<USSDMainCode> result_list = new List<USSDMainCode>();
            lines = Add2Log(lines, " GetUSSDMainCodeInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["USSDMainCodeList"] != null)
                {
                    lines = Add2Log(lines, " USSDMainCodeList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["USSDMainCodeList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["USSDMainCodeList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<USSDMainCode>)HttpContext.Current.Application["USSDMainCodeList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing PriceList Cache ", 100, "");
                            result_list = DBQueries.GetUSSDMainCode(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["USSDMainCodeList"] = result_list;
                                HttpContext.Current.Application["USSDMainCodeList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " USSDMainCodeList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing USSDMainCodeList Cache ", 100, "");
                    result_list = DBQueries.GetUSSDMainCode(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["USSDMainCodeList"] = result_list;
                        HttpContext.Current.Application["USSDMainCodeList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application USSDMainCodeList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing USSDMainCodeList From DB ", 100, "");
                result_list = DBQueries.GetUSSDMainCode(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.SPID == SPID && x.service_code == service_code);
                    if (result == null)
                    {
                        result = result_list.Find(x => x.SPID == SPID);
                    }
                }
            }
            return result;
        }


        

        public static USSDMenu GetUSSDMenu(int ussd_id, string ussd_string, int action_id, USSDSession ussd_session, ref List<LogLines> lines)
        {
            USSDMenu result = new USSDMenu();
            List<USSDMenu> result_list = new List<USSDMenu>();
            lines = Add2Log(lines, " GetUSSDMenuInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["USSDMenuList"] != null)
                {
                    lines = Add2Log(lines, " USSDMenuList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["USSDMenuList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["USSDMenuList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<USSDMenu>)HttpContext.Current.Application["USSDMenuList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing USSDMenuList Cache ", 100, "");
                            result_list = DBQueries.GetUSSDMenus(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["USSDMenuList"] = result_list;
                                HttpContext.Current.Application["USSDMenuList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " USSDMenuList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing USSDMenuList Cache ", 100, "");
                    result_list = DBQueries.GetUSSDMenus(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["USSDMenuList"] = result_list;
                        HttpContext.Current.Application["USSDMenuList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application USSDMenuList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing USSDMenuList From DB ", 100, "");
                result_list = DBQueries.GetUSSDMenus(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    
                    result = (ussd_session != null ? result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.ussd_string.Contains(ussd_string) && x.topic_id == ussd_session.topic_id) : result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.ussd_string.Contains(ussd_string)));

                    if (action_id == 43 && ussd_string != "*")
                    {
                        lines = Add2Log(lines, " Special scenario barcode ", 100, "");
                        Int64 num1;
                        bool res = Int64.TryParse(ussd_string, out num1);
                        if (res == true)
                        {
                            lines = Add2Log(lines, " Ussd is numeric", 100, "");
                            result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "barcode");
                            result.is_special = true;
                        }
                        else
                        {
                            result = result_list.Find(x => x.action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string == "1");
                            lines = Add2Log(lines, " Ussd is not numeric", 100, "");
                        }
                    }

                    if ((action_id == 57 || action_id ==58 || action_id == 71) && ussd_string.Contains(" "))
                    {
                        lines = Add2Log(lines, " Special scenario Rapidos", 100, "");
                        result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "rapidos");
                        result.is_special = true;
                    }

                    if (((action_id >= 96 && action_id <= 99) || (action_id >= 101 && action_id <= 103) || (action_id >= 105 && action_id <= 108)) && ussd_string.Contains(" "))
                    {
                        lines = Add2Log(lines, " Special scenario Dusan Lotto", 100, "");
                        result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "lotto");
                        result.is_special = true;
                    }



                    if ((action_id == 82 || action_id == 83) && ussd_string.Contains(" "))
                    {
                        lines = Add2Log(lines, " Special scenario Lotto", 100, "");
                        result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "lotto");
                        result.is_special = true;
                    }


                    if (action_id == 78 && (ussd_string.Substring(0, 2) == "06" || ussd_string.Substring(0, 1) == "6"))
                    {
                        lines = Add2Log(lines, " Special scenario Foot Soldiers MSISDN", 100, "");
                        result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "msisdn");
                        result.is_special = true;
                    }

                    if (action_id == 80 && ussd_string.Contains(" "))
                    {
                        lines = Add2Log(lines, " Special scenario Foot Soldiers Full Name", 100, "");
                        result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "fullname");
                        result.is_special = true;
                    }



                    if (((action_id >= 19 && action_id <= 21) || (action_id>= 33 && action_id <= 34) || (action_id >= 72 && action_id <= 74)  || (action_id == 55) || (action_id == 89) || (action_id>=84 && action_id <=85) || (action_id >= 109 && action_id <= 119)) && ussd_string != "*" && ussd_string != "8" && ussd_string.ToLower() != "m")
                    {
                        lines = Add2Log(lines, " Special scenario ", 100, "");
                        double num1;
                        bool res = double.TryParse(ussd_string, out num1);
                        if (res == true)
                        {
                            lines = Add2Log(lines, " Ussd is numeric", 100, "");
                            result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "amount");
                            result.is_special = true;
                        }
                        else
                        {
                            if (action_id>=19 && action_id <= 21)
                            {
                                result = result_list.Find(x => x.action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string == "2");
                                result.is_special = true;
                            }
                            else
                            {
                                result = result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id && x.ussd_string.ToLower() == "m");
                            }
                            lines = Add2Log(lines, " Ussd is not numeric", 100, "");
                            
                        }

                    }
                    if (result == null)
                    {
                        result = (ussd_session != null ? result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id) : result_list.Find(x => x.prev_action_id == action_id && x.ussd_id == ussd_id));
                    }
                    if (result == null)
                    {

                        result = (ussd_session != null ? result_list.Find(x => x.action_id == 1 && x.ussd_id == ussd_id && x.topic_id == ussd_session.topic_id) : result_list.Find(x => x.action_id == 1 && x.ussd_id == ussd_id));
                    }
                    
                }
            }
            return result;
        }

        //public static USSDMenu GetUSSDMenu(int ussd_id, string ussd_string, int last_menu_id, ref List<LogLines> lines)
        //{
        //    USSDMenu result = new USSDMenu();
        //    List<USSDMenu> result_list = new List<USSDMenu>();
        //    lines = Add2Log(lines, " GetUSSDMenuInfo()", 100, "");
        //    try
        //    {
        //        if (HttpContext.Current.Application["USSDMenuList"] != null)
        //        {
        //            lines = Add2Log(lines, " USSDMenuList Cache contains Info", 100, "");
        //            if (HttpContext.Current.Application["USSDMenuList_expdate"] != null)
        //            {
        //                DateTime expdate = (DateTime)HttpContext.Current.Application["USSDMenuList_expdate"];
        //                lines = Add2Log(lines, " expdate = " + expdate, 100, "");
        //                if (DateTime.Now < expdate)
        //                {
        //                    result_list = (List<USSDMenu>)HttpContext.Current.Application["USSDMenuList"];
        //                }
        //                else
        //                {
        //                    lines = Add2Log(lines, " Renewing USSDMenuList Cache ", 100, "");
        //                    result_list = DBQueries.GetUSSDMenus(ref lines);
        //                    if (result_list != null)
        //                    {
        //                        HttpContext.Current.Application["USSDMenuList"] = result_list;
        //                        HttpContext.Current.Application["USSDMenuList_expdate"] = DateTime.Now.AddHours(10);
        //                    }

        //                }
        //            }

        //        }
        //        else
        //        {
        //            lines = Add2Log(lines, " USSDMenuList Cache does not contain Info", 100, "");
        //            lines = Add2Log(lines, " Renewing USSDMenuList Cache ", 100, "");
        //            result_list = DBQueries.GetUSSDMenus(ref lines);
        //            if (result_list != null)
        //            {
        //                HttpContext.Current.Application["USSDMenuList"] = result_list;
        //                HttpContext.Current.Application["USSDMenuList_expdate"] = DateTime.Now.AddHours(10);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        lines = Add2Log(lines, " Exception on HttpContext.Current.Application USSDMenuList Cache does not contain Info", 100, "");
        //        lines = Add2Log(lines, " Renewing USSDMenuList From DB ", 100, "");
        //        result_list = DBQueries.GetUSSDMenus(ref lines);
        //    }
        //    if (result_list != null)
        //    {
        //        if (result_list.Count() > 0)
        //        {
        //            result = result_list.Find(x => x.last_menu_id == last_menu_id && x.ussd_id == ussd_id && x.ussd_string.Contains(ussd_string));
        //            if (result == null)
        //            {
        //                result = result_list.Find(x => x.last_menu_id == 0 && x.ussd_id == ussd_id && x.ussd_string == ussd_string);
        //            }
        //            if (result == null)
        //            {
        //                result = result_list.Find(x => x.last_menu_id == 0 && x.ussd_id == ussd_id);
        //            }
        //        }
        //    }
        //    return result;
        //}

        public static List<Int64> GetUSSDBlackListUsersNoRefund(ref List<LogLines> lines)
        {
            List<Int64> result = null;

            try
            {
                if (HttpContext.Current.Application["USSDBLUsersNR"] != null)
                {
                    lines = Add2Log(lines, " USSDBLUsersNR Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["USSDBLUsersNR_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["USSDBLUsersNR_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<Int64>)HttpContext.Current.Application["USSDBLUsersNR"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing USSDBLUsersNR Cache ", 100, "");
                            result = DBQueries.SelectQueryReturnListInt64("SELECT u.msisdn FROM ussd_blacklist u WHERE u.block_refund = TRUE", ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["USSDBLUsersNR"] = result;
                                HttpContext.Current.Application["USSDBLUsersNR_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " USSDBLUsersNR Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing USSDBLUsersNR Cache ", 100, "");
                    result = DBQueries.SelectQueryReturnListInt64("SELECT u.msisdn FROM ussd_blacklist u WHERE u.block_refund = TRUE", ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["USSDBLUsersNR"] = result;
                        HttpContext.Current.Application["USSDBLUsersNR_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application USSDBLUsersNR Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing USSDBLUsersNR From DB ", 100, "");
                result = DBQueries.SelectQueryReturnListInt64("SELECT u.msisdn FROM ussd_blacklist u WHERE u.block_refund = TRUE", ref lines);
            }

            return result;
        }

        public static List<Int64> GetWhiteListUsersPerService(int service_id, ref List<LogLines> lines)
        {
            List<Int64> result = null;

            try
            {
                if (HttpContext.Current.Application["WhiteListUsersPerService_"+service_id.ToString()] != null)
                {
                    lines = Add2Log(lines, " WhiteListUsersPerService Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["WhiteListUsersPerService_expdate_" + service_id.ToString()] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["WhiteListUsersPerService_expdate_" + service_id.ToString()];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (List<Int64>)HttpContext.Current.Application["WhiteListUsersPerService_" + service_id.ToString()];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing WhiteListUsersPerService Cache ", 100, "");
                            result = DBQueries.SelectQueryReturnListInt64("SELECT msisdn FROM whitelist_users w WHERE w.service_id = " + service_id, ref lines);
                            if (result != null)
                            {
                                HttpContext.Current.Application["WhiteListUsersPerService_" + service_id.ToString()] = result;
                                HttpContext.Current.Application["WhiteListUsersPerService_expdate_" + service_id.ToString()] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " WhiteListUsersPerService Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing WhiteListUsersPerService Cache ", 100, "");
                    result = DBQueries.SelectQueryReturnListInt64("SELECT msisdn FROM whitelist_users w WHERE w.service_id = " + service_id, ref lines);
                    if (result != null)
                    {
                        HttpContext.Current.Application["WhiteListUsersPerService_" + service_id.ToString()] = result;
                        HttpContext.Current.Application["WhiteListUsersPerService_expdate_" + service_id.ToString()] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application WhiteListUsersPerService Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing WhiteListUsersPerService From DB ", 100, "");
                result = DBQueries.SelectQueryReturnListInt64("SELECT msisdn FROM whitelist_users w WHERE w.service_id = " + service_id, ref lines);
            }

            return result;
        }


    }
}