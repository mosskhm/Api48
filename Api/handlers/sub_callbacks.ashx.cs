using Api.CommonFuncations;
using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for sub_callbacks
    /// </summary>
    public class sub_callbacks : IHttpHandler
    {
        public class Items
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            context.Response.ContentType = "text/xml";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "sub_callbacks");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "sub_callbacks");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "sub_callbacks");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "sub_callbacks");

            var spID = "";
            var MSISDN = "";
            var productID = "";
            var serviceID = "";
            var updateType = "";
            var updateTime = "";
            var effectiveTime = "";
            var expiryTime = "";
            var keyword = "";
            string channelID = "";
            string fee = "0";
            Int64 sub_id = 0;
            bool resume = true;
            ServiceClass service = null;
            int db_service_id = 0, db_price_id = 0;
            string soap_respone = "";
            string description = "";

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "sub_callbacks");

            if (!String.IsNullOrEmpty(xml))
            {


                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.LoadXml(xml);

                var root = xmlDocument.DocumentElement;



                MSISDN = ProcessXML.GetXMLNode(xml, "ID", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "sub_callbacks");
                spID = (ProcessXML.GetXMLNode(xml, "ns1:spID", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:spID", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:spID", ref lines));
                lines = Add2Log(lines, " spID = " + spID, 100, "sub_callbacks");



                productID = (ProcessXML.GetXMLNode(xml, "ns1:productID", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:productID", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:productID", ref lines));
                lines = Add2Log(lines, " productID = " + productID, 100, "sub_callbacks");

                serviceID = (ProcessXML.GetXMLNode(xml, "ns1:serviceID", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceID", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceID", ref lines));
                lines = Add2Log(lines, " serviceID = " + serviceID, 100, "sub_callbacks");

                var serviceList = (ProcessXML.GetXMLNode(xml, "ns1:serviceList", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:serviceList", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:serviceList", ref lines));
                lines = Add2Log(lines, " serviceList = " + serviceList, 100, "sub_callbacks");

                updateType = (ProcessXML.GetXMLNode(xml, "ns1:updateType", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:updateType", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:updateType", ref lines));
                lines = Add2Log(lines, " updateType = " + updateType, 100, "sub_callbacks");

                //local updatetime
                updateTime = (ProcessXML.GetXMLNode(xml, "ns1:updateTime", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:updateTime", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:updateTime", ref lines));
                lines = Add2Log(lines, " updateTime = " + updateTime, 100, "sub_callbacks");
                if (!String.IsNullOrEmpty(updateTime))
                {
                    var date = DateTime.ParseExact(updateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    updateTime = date.ToString("yyyy-MM-dd HH:mm:ss");
                }

                var updateDesc = (ProcessXML.GetXMLNode(xml, "ns1:updateDesc", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:updateDesc", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:updateDesc", ref lines));
                lines = Add2Log(lines, " updateDesc = " + updateDesc, 100, "sub_callbacks");

                //subscription_time
                effectiveTime = (ProcessXML.GetXMLNode(xml, "ns1:effectiveTime", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:effectiveTime", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:effectiveTime", ref lines));
                if (!String.IsNullOrEmpty(effectiveTime))
                {
                    var date = DateTime.ParseExact(effectiveTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    effectiveTime = date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                lines = Add2Log(lines, " effectiveTime = " + effectiveTime, 100, "sub_callbacks");
                
                expiryTime = (ProcessXML.GetXMLNode(xml, "ns1:expiryTime", ref lines) == "" ? ProcessXML.GetXMLNode(xml, "ns2:expiryTime", ref lines) : ProcessXML.GetXMLNode(xml, "ns1:expiryTime", ref lines));
                lines = Add2Log(lines, " expiryTime = " + expiryTime, 100, "sub_callbacks");

                
                List<Items> xml_items = new List<Items>();
                foreach (XmlNode zz in xmlDocument.GetElementsByTagName("item"))
                {
                    var key = zz.ChildNodes[0].InnerText;
                    var value = zz.ChildNodes[1].InnerText;
                    xml_items.Add(new Items() { Key = key, Value = value });
                    lines = Add2Log(lines, " Key = " + key + ", Value = " + value, 100, "sub_callbacks");
                    if (key == "channelID")
                    {
                        channelID = value;
                    }
                    if (key == "fee")
                    {
                        fee = value;
                    }
                    if (key == "keyword")
                    {
                        keyword = value;
                    }

                }

                service = GetServiceInfo(spID, serviceID, productID, ref lines);
                

                channelID = (channelID == "" ? "3" : channelID);
                
                if (service != null)
                {
                    db_service_id = service.service_id;
                    lines = Add2Log(lines, service.operator_name + " user from " + service.country_name + " service name = " + service.service_name, 100, "sub_callbacks");
                    if (fee != "0")
                    {
                        PriceClass price_c = GetPricesInfo(db_service_id, Convert.ToDouble(fee), ref lines);
                        if (price_c != null)
                        {
                            db_price_id = price_c.price_id;
                            lines = Add2Log(lines, " price_id = " + price_c.price_id + ", " + price_c.price + " " + price_c.curency_code, 100, "sub_callbacks");
                        }
                        else
                        {

                            Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + fee + ",'NGN','N'," + (Convert.ToDouble(fee) / 100) + ")", ref lines);
                            if (price_id > 0)
                            {
                                List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                if (result_list != null)
                                {
                                    HttpContext.Current.Application["PriceList"] = result_list;
                                    HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                }
                                price_c = GetPricesInfo(service.service_id, Convert.ToDouble(fee), ref lines);
                            }


                        }
                    }
                    
                    
                }
                else
                {
                    resume = false;
                    lines = Add2Log(lines, " Failed to load services or service not found", 100, "sub_callbacks");
                }

            }

            


            

            if (MSISDN != "" && resume == true)
            {
                ////if (!MSISDN.StartsWith("234"))
                ////{

                ////}
                //soap_respone = "0";
                //description = "OK";
                ////Try to subscribe
                switch (updateType)
                {
                    case "1":
                        if (service.is_ondemand)
                        {
                            sub_id = DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id, subscription_keyword) values(" + MSISDN + ", " + db_service_id.ToString() + ", '" + updateTime + "', 3, '" + channelID + "', '" + keyword + "')", ref lines);
                        }
                        else
                        {
                            sub_id = DBQueries.InsertSub(MSISDN, db_service_id.ToString(), db_price_id, updateTime, updateTime, channelID, keyword, ref lines);
                        }

                        soap_respone = (sub_id > 0 ? "0" : "-1");
                        description = (sub_id > 0 ? "OK" : "NOK");

                        lines = Add2Log(lines, "Adding Subscriber " + MSISDN + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                        Api.DataLayer.DBQueries.ExecuteQuery("insert ignore into subscribers_misc (subscriber_id, channel_name) values(" + sub_id + ",'" + channelID + "')", ref lines);
                        //CommonFuncations.Notifications.SendSubscriptionNotification(MSISDN, db_service_id.ToString(), effectiveTime, ref lines);
                        if (fee != "0")
                        {
                            //CommonFuncations.Notifications.SendBillingNotification(MSISDN, db_service_id.ToString(), effectiveTime, (Convert.ToDouble(fee) / 100).ToString(), ref lines);
                        }
                        break;
                    case "2":
                        sub_id = DBQueries.UnsubscribeSub(MSISDN, db_service_id.ToString(), updateTime, channelID, keyword, effectiveTime, ref lines);
                        soap_respone = (sub_id > 0 ? "0" : "-1");
                        description = (sub_id > 0 ? "OK" : "NOK");
                        lines = Add2Log(lines, "Deleting Subscriber " + MSISDN + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                        //CommonFuncations.Notifications.SendUnSubscriptionNotification(MSISDN, db_service_id.ToString(), updateTime, ref lines);
                        break;
                    case "3":
                        //sub_id = DBQueries.InsertSub(MSISDN, db_service_id.ToString(), db_price_id, effectiveTime, updateTime, channelID, keyword, ref lines);
                        //soap_respone = (sub_id > 0 ? "0" : "-1");
                        //description = (sub_id > 0 ? "OK" : "NOK");
                        //lines = Add2Log(lines, "Billing Subscriber " + MSISDN + ", sub_id = " + sub_id + " description = " + description, 100, "sub_callbacks");
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into sub_callbacks_req (msisdn, service_id, price_id, effective_time, update_time, channel_id, keyword, update_type, fee, completed) values(" + MSISDN + "," + db_service_id.ToString() + ", " + db_price_id + ", '" + effectiveTime + "', '" + updateTime + "','" + channelID + "','" + keyword + "'," + updateType + "," + fee + ",0)", "DBConnectionString_104", ref lines);
                        //CommonFuncations.Notifications.SendBillingNotification(MSISDN, db_service_id.ToString(), updateTime, (Convert.ToDouble(fee) / 100).ToString(), ref lines);
                        break;
                }

            }
            else
            {
                soap_respone = "-1";
                description = "NOK";
            }

            string response = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/data/sync/v1_0/local\"> <soapenv:Header/> <soapenv:Body> <loc:syncOrderRelationResponse> <loc:result>" + soap_respone + "</loc:result> <loc:resultDescription>" + description + "</loc:resultDescription> </loc:syncOrderRelationResponse> </soapenv:Body> </soapenv:Envelope>";
            lines = Add2Log(lines, " Response = " + response, 100, "sub_callbacks");
            //if (Cache.ServerSettings.GetServerSettings("DoCallBack", ref lines) == "1")
            //{
            //    if (!String.IsNullOrEmpty(xml))
            //    {
            //        lines = Add2Log(lines, " Calling additional server " + Cache.ServerSettings.GetServerSettings("CallBackURL", ref lines), 100, "sub_callbacks");
            //        string r = CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("CallBackURL", ref lines), xml, ref lines);
            //        lines = Add2Log(lines, " Response = " + r, 100, "sub_callbacks");

            //    }
            //}

            //if (MSISDN != "" && resume == true && service != null)
            //{
            //    ServiceBehavior.DecideBehavior(service, updateType, MSISDN, sub_id, ref lines);
            //    if (updateType == "3")
            //    {
            //        string errmsg = "";
            //        bool full_res = Fulfillment.CallFulfillment(service, MSISDN, updateTime, true, ref lines, out errmsg);
            //    }
            //    if (updateType == "2")
            //    {
            //        string errmsg = "";
            //        bool full_res = Fulfillment.CallFulfillment(service, MSISDN, updateTime, false, ref lines, out errmsg);
            //    }


            //}



            lines = Write2Log(lines);
            
            context.Response.Write(response);

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