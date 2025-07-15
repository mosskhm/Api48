using Api.CommonFuncations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.NG
{
    /// <summary>
    /// Summary description for Subscription
    /// </summary>
    public class Subscription : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "NG_subscription");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "NG_subscription");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "NG_subscription");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "NG_subscription");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "NG_subscription");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "NG_subscription");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "NG_subscription");
            }
            dynamic json_response = JsonConvert.DeserializeObject(xml);
            try
            {
                string update_type = json_response.updatetype;
                string nj_serviceid = json_response.serviceid;
                string amount = json_response.amount;
                string msisdn = json_response.msisdn;
                string channel = json_response.channel;

                if (!String.IsNullOrEmpty(update_type) && !String.IsNullOrEmpty(nj_serviceid) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(msisdn))
                {
                    channel = (channel == "SMS" ? "2" : "1");
                    NJService nj = GetNJServiceInfo(Convert.ToInt32(nj_serviceid), ref lines);
                    if (nj != null)
                    {
                        ServiceClass service = GetServiceByServiceID(nj.service_id, ref lines);
                        if (service != null)
                        {
                            lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                            Int64 db_price_id = 0;
                            if (amount != "0")
                            {
                                PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount) * 100, ref lines);
                                if (price_c != null)
                                {
                                    db_price_id = price_c.price_id;
                                    lines = Add2Log(lines, " price_id = " + price_c.price_id + ", " + price_c.price + " " + price_c.curency_code, 100, "sub_callbacks");
                                }
                                else
                                {

                                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + (Convert.ToDouble(amount) * 100) + ",'NGN','N'," + amount + ")", ref lines);
                                    if (price_id > 0)
                                    {
                                        List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                        if (result_list != null)
                                        {
                                            HttpContext.Current.Application["PriceList"] = result_list;
                                            HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                        }
                                        price_c = GetPricesInfo(service.service_id, Convert.ToDouble(amount), ref lines);
                                        db_price_id = price_id;
                                    }
                                }

                            }
                            string effectiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string updateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string keyword = "";
                            string updateType = "";
                            switch(update_type)
                            {
                                case "Addition":
                                    updateType = "1";
                                    break;
                                case "Deletion":
                                    updateType = "2";
                                    break;
                                case "Modification":
                                    updateType = "3";
                                    break;
                            }
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into sub_callbacks_req (msisdn, service_id, price_id, effective_time, update_time, channel_id, keyword, update_type, fee, completed) values(" + msisdn + "," + service.service_id + ", " + db_price_id + ", '" + effectiveTime + "', '" + updateTime + "','" + channel + "','" + keyword + "'," + updateType + "," + amount + ",0)", ref lines);
                        }
                        else
                        {
                            lines = Add2Log(lines, " Failed to load services or service not found", 100, "sub_callbacks");
                        }
                    }
                }

                //string raw_data = json_response.rawdata;
                //string serviceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceID", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceID", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceID", ref lines));
                //lines = Add2Log(lines, " serviceID = " + serviceID, 100, "sub_callbacks");
                //if (serviceID != "234012000017271" && !String.IsNullOrEmpty(raw_data))
                //{
                //    //send to subcallbacs 
                //    string url = "https://api.ydplatform.com/handlers/sub_callbacks.ashx";
                //    List<Headers> headers = new List<Headers>();
                //    lines = Add2Log(lines, "Sending sub_callbacks async with delay ", 100, "ussd_mo");
                //    CommonFuncations.CallSoap.CallSoapRequestAsync(url, raw_data, headers, 1, ref lines);
                //    lines = Add2Log(lines, "Finished Sending sub_callbacks http async ", 100, "ussd_mo");
                //}

            }
            catch (Exception ex)
            {

            }

            lines = Write2Log(lines);




            context.Response.ContentType = "text/plain";
            context.Response.Write("ok");
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