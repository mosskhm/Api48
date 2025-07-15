using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.HML
{
    /// <summary>
    /// Summary description for mo_sms
    /// </summary>
    public class mo_sms : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "hml_smsmo");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "hml_smsmo");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "hml_smsmo");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "hml_smsmo");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "hml_smsmo");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "hml_smsmo");
            }

            //2022-07-17 00:01:20.834: Incomming XML = {"serviceType":"HML_Yello_5974","chargingMode":"E","appliedPlan":"23401220000031158","contentId":"NA","resultCode":"0","renFlag":"N","processingTime":"220716230121","result":"Success","validityType":"DD","sequenceNo":"202207162301079826","callingParty":"2348131045487","bearerId":"SMS","operationId":"ES","requestedPlan":"23401220000031158","chargeAmount":"100.0","serviceNode":"HML","serviceId":"234012000026228","keyword":"FF","category":"-1","validityDays":"0"}

            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                ServiceClass service = null;
                try
                {
                    string operation_id = json_response.operationId;
                    string msisdn = json_response.callingParty;
                    string applieded_plan = json_response.appliededPlan;
                    string charge_amount = json_response.chargeAmount;
                    string requested_plan = json_response.requestedPlan;
                    string resultCode = json_response.resultCode;
                    string keyword = json_response.keyword;
                    string tracking_id = json_response.sequenceNo;

                    applieded_plan = (String.IsNullOrEmpty(applieded_plan) ? "" : applieded_plan.Replace("_R", ""));
                    requested_plan = (String.IsNullOrEmpty(requested_plan) ? "" : requested_plan.Replace("_R", ""));

                    List<MADAPISubPlan> madapi_services = GetMADAPISubPlan(ref lines);
                    if (madapi_services != null && !String.IsNullOrEmpty(applieded_plan))
                    {
                        MADAPISubPlan madapi_service = madapi_services.Find(x => x.plan == applieded_plan);
                        if (madapi_service != null)
                        {
                            service = GetServiceByServiceID(madapi_service.service_id, ref lines);
                        }
                    }
                    if (service == null && !String.IsNullOrEmpty(requested_plan))
                    {
                        MADAPISubPlan madapi_service = madapi_services.Find(x => x.plan == requested_plan);
                        if (madapi_service != null)
                        {
                            service = GetServiceByServiceID(madapi_service.service_id, ref lines);
                        }
                    }

                    if (service != null && resultCode == "0")
                    {
                        Int64 sub_id = 0;
                        Int64 status = 0;
                        //string sub_id_str = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id + " order by subscriber_id desc limit 1", ref lines);
                        //if (operation_id == "SN" || operation_id == "SCI" || operation_id == "ACI")
                        //{
                            List<string> sub_id_str1 = Api.DataLayer.DBQueries.SelectQueryReturnListString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                            string sub_id_str = "";
                            if (sub_id_str1 != null)
                            {
                                int max_len = sub_id_str1.Count();
                                sub_id_str = sub_id_str1[max_len - 1];
                            }
                            if (!String.IsNullOrEmpty(sub_id_str))
                            {
                                status = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select state_id from subscribers where subscriber_id = " + sub_id_str, ref lines);
                                if (status == 2 && operation_id == "SN")
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
                                if (operation_id == "SN" || operation_id == "ES")
                                {
                                    sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                                }
                            }
                            if (sub_id > 0 && !String.IsNullOrEmpty(tracking_id))
                            {
                                Int64 num;
                                bool success = Int64.TryParse(tracking_id, out num);
                                if (success)
                                {
                                    string update_query = "UPDATE tracking.tracking_requests SET msisdn = " + msisdn + " , subscriber_id = " + sub_id + " WHERE id = " + num;
                                    Api.DataLayer.DBQueries.ExecuteQuery(update_query, ref lines);
                                }
                            }
                        //}
                        
                        string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        switch (operation_id)
                        {
                            case "YR":
                            case "RR":
                            case "ES":
                            case "GR":
                                if (charge_amount != "0.0")
                                {
                                    charge_amount = charge_amount.Replace(".0", "");
                                    PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                    if (service_price == null)
                                    {
                                        Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge_amount + ",'NGN','NGN'," + charge_amount + ")", ref lines);
                                        if (price_id > 0)
                                        {
                                            List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                            if (result_list != null)
                                            {
                                                HttpContext.Current.Application["PriceList"] = result_list;
                                                HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                            }
                                            service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                        }
                                    }
                                    if (service_price != null)
                                    {
                                        //Api.DataLayer.DBQueries.ExecuteQuery("insert into sub_callbacks_req (msisdn, service_id, price_id, effective_time, update_time, channel_id, keyword, update_type, fee, completed) values(" + msisdn + "," + service.service_id.ToString() + ", " + service_price.price_id + ", now(), now(),'1','" + keyword + "',3," + charge_amount + ",0)", "DBConnectionString_104", ref lines);
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ", now()," + service_price.price_id + ")", ref lines);
                                        Api.CommonFuncations.Notifications.SendBillingNotification(msisdn, service.service_id.ToString(), dt, service_price.price.ToString(), ref lines);
                                        Api.CommonFuncations.ServiceBehavior.DecideBillingBehavior(service, "1", msisdn, sub_id, ref lines);
                                    }
                                }
                                else
                                {
                                    Api.CommonFuncations.Notifications.SendBillingNotification(msisdn, service.service_id.ToString(), dt, "0", ref lines);
                                }
                                if (!String.IsNullOrEmpty(keyword))
                                {
                                    Api.CommonFuncations.Notifications.SendMONotification(msisdn, service.service_id.ToString(), dt, keyword, ref lines);
                                }
                                break;
                            case "SN":
                                Api.CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), dt, ref lines);
                                Api.CommonFuncations.ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                                if (charge_amount != "0.0")
                                {
                                    charge_amount = charge_amount.Replace(".0", "");
                                    PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                    if (service_price == null)
                                    {
                                        Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge_amount + ",'NGN','NGN'," + charge_amount + ")", ref lines);
                                        if (price_id > 0)
                                        {
                                            List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                                            if (result_list != null)
                                            {
                                                HttpContext.Current.Application["PriceList"] = result_list;
                                                HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                                            }
                                            service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                        }
                                    }
                                    if (service_price != null)
                                    {
                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values(" + sub_id + ", now()," + service_price.price_id + ")", ref lines);
                                        Api.CommonFuncations.Notifications.SendBillingNotification(msisdn, service.service_id.ToString(), dt, service_price.price.ToString(), ref lines);
                                    }
                                }
                                break;
                            case "SCI":
                            case "ACI":
                                if (sub_id > 0)
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + sub_id, ref lines);
                                }
                                else
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                                }
                                Api.CommonFuncations.Notifications.SendUnSubscriptionNotification(msisdn, service.service_id.ToString(), dt, ref lines);
                                break;
                        }
                        

                        //{"serviceType":"Yello_Yello_18586","contentId":"-1","resultCode":"0","renFlag":"Y","requestNo":"2207121323378509004","result":"Success","OptionalParameter3":"STOP GD","validityType":"DD","sequenceNo":"20220712132337176","callingParty":"237670998900","newContentId":"","bearerId":"SMS","operationId":"SN","requestedPlan":"23701220000029245_R","appliededPlan":"F_Yello_Yello_4517_R","chargeAmount":"50.0","serviceNode":"Yellowdot","serviceId":"237012000024956","category":"-1","validityDays":"20"}
                    }

                    //{ "network":"MTNN","aggregator":"Pisi Mobile Services","pisisid":54,"id":"9d0a03ac-5eb4-4572-84ea-800296548a48","senderAddress":"2348131609970","receiverAddress":"205","message":"E","created":1660217191036}
                    msisdn = json_response.senderAddress;
                    string message = json_response.message;
                    string pisisid = json_response.pisisid;

                    if (pisisid == "54" && !String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(message))
                    {
                        Api.CommonFuncations.Notifications.SendMONotification(msisdn, "897", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message, ref lines);
                    }

                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
                }
                
            }

            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"Response\":\"OK\"}");
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