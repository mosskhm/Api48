using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.MADAPI
{
    /// <summary>
    /// Summary description for subscribe
    /// </summary>
    public class subscribe : IHttpHandler
    {
        public void ComVivaController(string operation_id, string msisdn, string applieded_plan, string charge_amount, string requested_plan, ref List<LogLines> lines)
        {
            ServiceClass service = null;
            try
            {
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

                if (service != null)
                {
                    //{"serviceType":"Yello_Yello_18586","contentId":"-1","resultCode":"0","renFlag":"Y","requestNo":"2207121323378509004","result":"Success","OptionalParameter3":"STOP GD","validityType":"DD","sequenceNo":"20220712132337176","callingParty":"237670998900","newContentId":"","bearerId":"SMS","operationId":"SN","requestedPlan":"23701220000029245_R","appliededPlan":"F_Yello_Yello_4517_R","chargeAmount":"50.0","serviceNode":"Yellowdot","serviceId":"237012000024956","category":"-1","validityDays":"20"}
                    Int64 sub_id = 0;
                    Int64 status = 0;
                    //string sub_id_str = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id + " order by subscriber_id desc limit 1", ref lines);
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
                    string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    switch (operation_id)
                    {
                        case "ES":
                            if (charge_amount != "0.0")
                            {
                                charge_amount = charge_amount.Replace(".0", "");
                                PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                if (service_price == null)
                                {
                                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge_amount + ",'',''," + charge_amount + ")", ref lines);
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
                        case "SN":

                            Api.CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), dt, ref lines);
                            Api.CommonFuncations.ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                            if (charge_amount != "0.0")
                            {
                                charge_amount = charge_amount.Replace(".0", "");
                                PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                if (service_price == null)
                                {
                                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge_amount + ",'',''," + charge_amount + ")", ref lines);
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
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
            }
        }

        public void SixDController(string msisdn, string applieded_plan, string charge_amount, string operation_id, string client_trans_id, string type,  ref List<LogLines> lines)
        {
            ServiceClass service = null;
            string orig_trans = "";
            try
            {
                applieded_plan = (String.IsNullOrEmpty(applieded_plan) ? "" : applieded_plan.Replace("_R", ""));


                List<MADAPISubPlan> madapi_services = GetMADAPISubPlan(ref lines);
                if (madapi_services != null && !String.IsNullOrEmpty(applieded_plan))
                {
                    MADAPISubPlan madapi_service = madapi_services.Find(x => x.plan == applieded_plan);
                    if (madapi_service != null)
                    {
                        service = GetServiceByServiceID(madapi_service.service_id, ref lines);
                    }
                }

                if (!String.IsNullOrEmpty(client_trans_id))
                {
                    try
                    {
                        string real_service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_id from subscription_requests where id = " + client_trans_id, ref lines);
                        orig_trans = Api.DataLayer.DBQueries.SelectQueryReturnString("select transaction_id from subscription_requests where id = " + client_trans_id, ref lines);
                        service = GetServiceByServiceID(Convert.ToInt32(real_service_id), ref lines);
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }


                bool is_rebilling = (String.IsNullOrEmpty(orig_trans) ? false : (orig_trans == "12345" ? false : (orig_trans.StartsWith("ReBill") ? true : false)));

                if (service != null)
                {
                    //{"externalServiceId":"2250595017480","requestId":"76477958502567675","requestTimeStamp":"20221113220012","channel":"1","requestParam":{"data":[{"name":"ChargeAmount","value":"0"},{"name":"SubscriptionStatus","value":"G"},{"name":"SubscriberLifeCycle","value":"REN2"},{"name":"TransactionId","value":"76477958502567675"},{"name":"NextBillingDate","value":"2022-11-14 22:00:12"}],"planId":"1012910002","command":"NotifyRenewal"},"featureId":"CallBack"}
                    Int64 sub_id = 0;
                    Int64 status = 0;
                    //string sub_id_str = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from subscribers where msisdn = " + msisdn + " and service_id = " + service.service_id + " order by subscriber_id desc limit 1", ref lines);
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
                        if (status == 2 && operation_id == "A")
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
                        if (operation_id == "A")
                        {
                            sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (" + msisdn + "," + service.service_id + ",now(),1)", ref lines);
                        }
                    }
                    string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    switch (operation_id)
                    {
                        case "A":
                            if (!is_rebilling && service.is_ondemand == false)
                            {
                                Api.CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service.service_id.ToString(), dt, ref lines);
                            }
                            Api.CommonFuncations.ServiceBehavior.DecideBehavior(service, "1", msisdn, sub_id, ref lines);
                            if (charge_amount != "0.00")
                            {
                                charge_amount = charge_amount.Replace(".00", "");
                                PriceClass service_price = GetPricesInfo(service.service_id, Convert.ToDouble(charge_amount), ref lines);
                                if (service_price == null)
                                {
                                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into prices (service_id, price, curency_code, curency_symbol, real_price) values (" + service.service_id + "," + charge_amount + ",'',''," + charge_amount + ")", ref lines);
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
                                    Api.CommonFuncations.Notifications.SendBillingNotificationIsRebilling(msisdn, service.service_id.ToString(), dt, service_price.price.ToString(), is_rebilling, ref lines);
                                }
                            }
                            else
                            {
                                Api.CommonFuncations.Notifications.SendBillingNotificationIsRebilling(msisdn, service.service_id.ToString(), dt, "0", false, ref lines);
                                
                            }
                            break;
                        case "D":
                            if (type == "DEACTIVATION")
                            {
                                if (sub_id > 0)
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + sub_id, ref lines);
                                }
                                else
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where msisdn = " + msisdn + " and service_id = " + service.service_id, ref lines);
                                }
                                Api.CommonFuncations.Notifications.SendUnSubscriptionNotification(msisdn, service.service_id.ToString(), dt, ref lines);
                            }
                            
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "MADApi_subscribe");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MADApi_subscribe");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MADApi_subscribe");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MADApi_subscribe");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MADApi_subscribe");


            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "MADApi_subscribe");
            }
            if (!String.IsNullOrEmpty(xml))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string operation_id = json_response.operationId;
                    ServiceClass service = null;
                    string msisdn = json_response.callingParty;
                    string applieded_plan = json_response.appliededPlan;
                    string charge_amount = json_response.chargeAmount;
                    string requested_plan = json_response.requestedPlan;
                    string client_trans_id = "";
                    string my_type = "";

                    if (!String.IsNullOrEmpty(operation_id))
                    {
                        ComVivaController(operation_id, msisdn, applieded_plan, charge_amount, requested_plan, ref lines);
                    }
                    else
                    {
                        /* externalServiceId = msisdn
                             “name”:"SubscriptionStatus","value": “A”/”D”
                 "name":"ChargeAmount,""value":"0.00"
                 "planId":"211910002,*/
                        /*{"externalServiceId":"2250565610503","requestId":"a12405191544342800","requestTimeStamp":"20220802193609","channel":"2","requestParam":{"data":[{"name":"TransactionId","value":"a12405191544342800"},
                         * {"name":"ClientTransactionId","value":"sdfgfdssdf"},{"name":"SubscriberLifeCycle","value":"SUB1"},{"name":"SubscriptionStatus","value":"A"},{"name":"NextBillingDate","value":"2022-08-03 19:36:09"},
                         * {"name":"Type","value":"ACTIVATION"},{"name":"ShortCode","value":"479"},
                         * {"name":"BillingId","value":"a12405191564408408"},{"name":"ChargeAmount","value":"100.00"}],"planId":"1012910003","command":"NotifyActivation"},"featureId":"CallBack"}*/
                        msisdn = json_response.externalServiceId;
                        foreach (dynamic s in json_response.requestParam.data)
                        {
                            if (s.name == "SubscriptionStatus")
                            {
                                operation_id = s.value;
                            }
                            if (s.name == "ChargeAmount")
                            {
                                charge_amount = s.value;
                            }
                            if (s.name == "ClientTransactionId")
                            {
                                client_trans_id = s.value;
                            }
                            if (s.name == "ClientTransactionId")
                            {
                                client_trans_id = s.value;
                            }
                            if (s.name == "Type")
                            {
                                my_type = s.value;
                            }
                        }
                        applieded_plan = json_response.requestParam.planId;
                        if (!String.IsNullOrEmpty(operation_id))
                        {
                            SixDController(msisdn, applieded_plan, charge_amount, operation_id, client_trans_id, my_type, ref lines);
                        }


                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "MADApiSubCallBack");
                }


            }


            lines = Write2Log(lines);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"OK\"}");
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