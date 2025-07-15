using Api.CommonFuncations;
using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CellC
{
    /// <summary>
    /// Summary description for sub_callback
    /// </summary>
    public class sub_callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            string response_soap = "OK";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "cellc_subcallback");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "cellc_subcallback");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "cellc_subcallback");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "cellc_subcallback");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "cellc_subcallback");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "cellc_subcallback");
            }

            foreach (String key in context.Request.ServerVariables.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.ServerVariables[key], 100, "cellc_subcallback");
            }

            ServiceClass service = null;
            Int64 sub_id = 0;
            if (xml != null)
            {
                string msisdn = ProcessXML.GetXMLNode(xml, "msisdn", ref lines);
                lines = Add2Log(lines, " msisdn = " + msisdn, 100, "sub_callbacks");

                string status = ProcessXML.GetXMLNode(xml, "status", ref lines);
                lines = Add2Log(lines, " status = " + status, 100, "sub_callbacks");

                string smsReply = ProcessXML.GetXMLNode(xml, "smsReply", ref lines);
                lines = Add2Log(lines, " smsReply = " + smsReply, 100, "sub_callbacks");

                string waspReference = ProcessXML.GetXMLNode(xml, "waspReference", ref lines);
                lines = Add2Log(lines, " waspReference = " + waspReference, 100, "sub_callbacks");

                string serviceID = ProcessXML.GetXMLNode(xml, "serviceID", ref lines);
                lines = Add2Log(lines, " serviceID = " + serviceID, 100, "sub_callbacks");

                string serviceName = ProcessXML.GetXMLNode(xml, "serviceName", ref lines);
                lines = Add2Log(lines, " serviceName = " + serviceName, 100, "sub_callbacks");

                if (status == "ACTIVE-PUSH-FORCED" && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(serviceName)) //satPush flow
                {
                    List<ServiceClass> services = Api.Cache.Services.GetServiceList(ref lines);
                    service = services.Find(x => x.operator_id == 25 && x.service_name == serviceName);
                    if (service != null)
                    {
                        List<string> sub_id_str1 = Api.DataLayer.DBQueries.SelectQueryReturnListString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                        string sub_id_str = "";
                        if (sub_id_str1 != null)
                        {
                            int max_len = sub_id_str1.Count();
                            sub_id_str = sub_id_str1[max_len - 1];
                        }
                        if (!String.IsNullOrEmpty(sub_id_str))
                        {
                            Int64 status1 = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select state_id from subscribers where subscriber_id = " + sub_id_str, ref lines);
                            if (status1 == 2)
                            {
                                sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                            }
                            else
                            {
                                sub_id = Convert.ToInt64(sub_id_str);
                            }
                        }
                        else
                        {
                            sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                        }
                        if (sub_id > 0)
                        {
                            string mydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            Api.CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), mydate, ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into yellowdot.subscribers_cellc(subscriber_id,real_sub_id, channel) values (" + sub_id + ", " + serviceID + ", 'SatPush');", ref lines);
                            LoginRequest LoginRequestBody = new LoginRequest()
                            {
                                ServiceID = service.service_id,
                                Password = service.service_password
                            };
                            LoginResponse res = Login.DoLogin(LoginRequestBody);
                            if (res != null)
                            {
                                if (res.ResultCode == 1000)
                                {
                                    string token_id = res.TokenID;
                                    List<PriceClass> prices = Api.Cache.Prices.GetAllPricesInfo(ref lines);
                                    if (prices != null)
                                    {
                                        PriceClass price = prices.Find(x => x.service_id == service.service_id);
                                        if (price != null)
                                        {
                                            BillRequest RequestBody = new BillRequest()
                                            {
                                                MSISDN = Convert.ToInt64(msisdn),
                                                ServiceID = service.service_id,
                                                TokenID = token_id,
                                                PriceID = price.price_id,
                                                TransactionID = sub_id.ToString()
                                            };
                                            BillResponse bill_response = Api.CommonFuncations.Bill.DoBill(RequestBody);
                                            if (bill_response != null)
                                            {
                                                if (bill_response.ResultCode == 1000)
                                                {
                                                    lines = Add2Log(lines, "User was billed ", 100, "ussd_mo");
                                                    Api.CommonFuncations.Notifications.SendBillingNotificationIsRebilling(msisdn, service.service_id.ToString(), mydate, price.price.ToString(), false, ref lines);
                                                }
                                                else
                                                {
                                                    lines = Add2Log(lines, "User was not billed ", 100, "ussd_mo");
                                                }

                                            }
                                            else
                                            {
                                                lines = Add2Log(lines, "Bill Response is null ", 100, "ussd_mo");
                                            }
                                            Api.CommonFuncations.ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                                        }
                                    }


                                }
                            }

                        }
                    }
                }
                if (status == "CANCELLED" && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(serviceName)) //unsub
                {
                    List<ServiceClass> services = Api.Cache.Services.GetServiceList(ref lines);
                    service = services.Find(x => x.operator_id == 25 && x.service_name == serviceName);
                    if (service != null)
                    {
                        List<string> sub_id_str1 = Api.DataLayer.DBQueries.SelectQueryReturnListString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                        string sub_id_str = "";
                        if (sub_id_str1 != null)
                        {
                            int max_len = sub_id_str1.Count();
                            sub_id_str = sub_id_str1[max_len - 1];
                        }
                        if (!String.IsNullOrEmpty(sub_id_str))
                        {
                            Int64 status1 = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select state_id from subscribers where subscriber_id = " + sub_id_str, ref lines);
                            if (status1 == 1)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + sub_id_str , ref lines);
                            }
                            
                        }
                        else
                        {
                           Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers (msisdn, service_id, subscription_date,deactivation_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),now(),2)", ref lines);
                        }
                        
                    }
                }

                if (!String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(status) && !String.IsNullOrEmpty(smsReply) && !String.IsNullOrEmpty(waspReference))
                {
                    if (status == "ACTIVE" && smsReply == "Yes")
                    {
                        string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select s.service_id from subscription_requests s where id = " + waspReference, ref lines);
                        string campaign_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select s.transaction_id from subscription_requests s where id = " + waspReference, ref lines);
                        if (service_id != null)
                        {
                            service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);

                            List<string> sub_id_str1 = Api.DataLayer.DBQueries.SelectQueryReturnListString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                            string sub_id_str = "";
                            if (sub_id_str1 != null)
                            {
                                int max_len = sub_id_str1.Count();
                                sub_id_str = sub_id_str1[max_len - 1];
                            }
                            if (!String.IsNullOrEmpty(sub_id_str))
                            {
                                Int64 status1 = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select state_id from subscribers where subscriber_id = " + sub_id_str, ref lines);
                                if (status1 == 2)
                                {
                                    sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                                }
                                else
                                {
                                    sub_id = Convert.ToInt64(sub_id_str);
                                }
                            }
                            else
                            {
                                sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                            }
                            if (sub_id > 0)
                            {
                                if (!String.IsNullOrEmpty(campaign_id))
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests set subscriber_id = " + sub_id + ", msisdn = " + msisdn + " where id = " + campaign_id, ref lines);
                                }
                                string mydate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                Api.CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), mydate, ref lines);
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into yellowdot.subscribers_cellc(subscriber_id,real_sub_id, channel) values (" + sub_id + ", " + serviceID + ", 'SMS');", ref lines);
                                LoginRequest LoginRequestBody = new LoginRequest()
                                {
                                    ServiceID = service.service_id,
                                    Password = service.service_password
                                };
                                LoginResponse res = Login.DoLogin(LoginRequestBody);
                                if (res != null)
                                {
                                    if (res.ResultCode == 1000)
                                    {
                                        string token_id = res.TokenID;
                                        List<PriceClass> prices = Api.Cache.Prices.GetAllPricesInfo(ref lines);
                                        if (prices != null)
                                        {
                                            PriceClass price = prices.Find(x => x.service_id == service.service_id);
                                            if (price != null)
                                            {
                                                BillRequest RequestBody = new BillRequest()
                                                {
                                                    MSISDN = Convert.ToInt64(msisdn),
                                                    ServiceID = service.service_id,
                                                    TokenID = token_id,
                                                    PriceID = price.price_id,
                                                    TransactionID = sub_id.ToString()
                                                };
                                                BillResponse bill_response = Api.CommonFuncations.Bill.DoBill(RequestBody);
                                                if (bill_response != null)
                                                {
                                                    if (bill_response.ResultCode == 1000)
                                                    {
                                                        lines = Add2Log(lines, "User was billed ", 100, "ussd_mo");
                                                        Api.CommonFuncations.Notifications.SendBillingNotificationIsRebilling(msisdn, service.service_id.ToString(), mydate, price.price.ToString(), false, ref lines);
                                                    }
                                                    else
                                                    {
                                                        lines = Add2Log(lines, "User was not billed ", 100, "ussd_mo");
                                                    }

                                                }
                                                else
                                                {
                                                    lines = Add2Log(lines, "Bill Response is null " , 100, "ussd_mo");
                                                }
                                                Api.CommonFuncations.ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                                            }
                                        }

                                        
                                    }
                                }
                                
                            }
                        }
                    }
                }



            }
            

            lines = Add2Log(lines, "Response = " + response_soap, 100, "ussd_mo");
            context.Response.ContentType = "application/json";
            context.Response.Write(response_soap);

            

            lines = Write2Log(lines);
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