using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class Prices
    {
        public class PriceClass
        {
            public int price_id { get; set; }
            public int service_id { get; set; }
            public double price { get; set; }
            public string curency_code { get; set; }
            public string ninemobile_service_code { get; set; }
            public string ninemobile_duration { get; set; }
        }

        public static List<PriceClass> GetAllPricesInfo(ref List<LogLines> lines)
        {
            List<PriceClass> result_list = new List<PriceClass>();
            lines = Add2Log(lines, " GetPricesInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["PriceList"] != null)
                {
                    lines = Add2Log(lines, " PriceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["PriceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["PriceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PriceClass>)HttpContext.Current.Application["PriceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing PriceList Cache ", 100, "");
                            result_list = DBQueries.GetPrices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["PriceList"] = result_list;
                                HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " PriceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing PricesList Cache ", 100, "");
                    result_list = DBQueries.GetPrices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["PriceList"] = result_list;
                        HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application PriceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing PricesList From DB ", 100, "");
                result_list = DBQueries.GetPrices(ref lines);
            }
            return result_list;
        }

        public static PriceClass GetPricesInfo(int service_id, double price, ref List<LogLines> lines)
        {
            PriceClass result = null;
            List<PriceClass> result_list = GetAllPricesInfo(ref lines);
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id && Math.Round(x.price,3) == Math.Round(price,3));
                }
            }
            if (result == null) lines = Add2Log(lines, $"GetPriceInfo: amount={Math.Round(price,3)} does not exist under service_id={service_id}", 100, "");

            return result;
        }
        public static PriceClass GetPricesInfo(int service_id, int price_id, ref List<LogLines> lines)
        {
            PriceClass result = null;
            List<PriceClass> result_list = GetAllPricesInfo(ref lines);
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id && x.price_id == price_id);
                }
            }

            if (result == null) lines = Add2Log(lines, $"GetPriceInfo: price_id={price_id} does not exist under service_id={service_id}", 100, "");
            return result;
        }

        public static PriceClass Get9MobileInfo(int service_id, int price_id, ref List<LogLines> lines)
        {
            PriceClass result = null;
            List<PriceClass> result_list = new List<PriceClass>();
            lines = Add2Log(lines, " Get9MobileInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["NineMobilePriceList"] != null)
                {
                    lines = Add2Log(lines, " PriceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["NineMobilePriceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["NineMobilePriceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PriceClass>)HttpContext.Current.Application["NineMobilePriceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing PriceList Cache ", 100, "");
                            result_list = DBQueries.Get9MobilePrices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                                HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " PriceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing PricesList Cache ", 100, "");
                    result_list = DBQueries.Get9MobilePrices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                        HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application PriceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing PricesList From DB ", 100, "");
                result_list = DBQueries.Get9MobilePrices(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id && x.price_id == price_id);
                }
            }
            return result;
        }

        public static PriceClass Get9MobileInfo(int service_id, ref List<LogLines> lines)
        {
            PriceClass result = null;
            List<PriceClass> result_list = new List<PriceClass>();
            lines = Add2Log(lines, " Get9MobileInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["NineMobilePriceList"] != null)
                {
                    lines = Add2Log(lines, " PriceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["NineMobilePriceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["NineMobilePriceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PriceClass>)HttpContext.Current.Application["NineMobilePriceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing PriceList Cache ", 100, "");
                            result_list = DBQueries.Get9MobilePrices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                                HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " PriceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing PricesList Cache ", 100, "");
                    result_list = DBQueries.Get9MobilePrices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                        HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application PriceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing PricesList From DB ", 100, "");
                result_list = DBQueries.Get9MobilePrices(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.OrderByDescending(x => x.price).ToList().Find(x=> x.service_id == service_id);
                }
            }
            return result;
        }

        public static PriceClass Get9MobileInfo(int service_id, bool asecnding, ref List<LogLines> lines)
        {
            PriceClass result = null;
            List<PriceClass> result_list = new List<PriceClass>();
            lines = Add2Log(lines, " Get9MobileInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["NineMobilePriceList"] != null)
                {
                    lines = Add2Log(lines, " PriceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["NineMobilePriceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["NineMobilePriceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PriceClass>)HttpContext.Current.Application["NineMobilePriceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing PriceList Cache ", 100, "");
                            result_list = DBQueries.Get9MobilePrices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                                HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " PriceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing PricesList Cache ", 100, "");
                    result_list = DBQueries.Get9MobilePrices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["NineMobilePriceList"] = result_list;
                        HttpContext.Current.Application["NineMobilePriceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }

            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application PriceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing PricesList From DB ", 100, "");
                result_list = DBQueries.Get9MobilePrices(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.OrderBy(x => x.price).ToList().Find(x => x.service_id == service_id);
                }
            }
            return result;
        }
    }
}