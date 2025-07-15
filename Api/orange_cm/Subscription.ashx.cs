using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.orange_cm
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
            lines = Add2Log(lines, "*****************************", 100, "orange_vas_subscription");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "orange_vas_subscription");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "orange_vas_subscription");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "orange_vas_subscription");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "orange_vas_subscription");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "orange_vas_subscription");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "orange_guinea_billing");
            }
            string headers = string.Empty;
            foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                headers = key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                //lines = Add2Log(lines, headers, 100, "orange_guinea_billing");
            }

            //2022-06-11 12:13:16.490: Incomming XML = {"note":{"text":"1000"},"event":{"id":"62a46abb62a5c9507f16149f","relatedParty":[{"id":"PDKSUB-200-YFnngyOr5ARlBxtap6AK2EjKzKrf0gamHZzyPZLx59Y=","name":"ISE2","role":"subscriber","aliases":null,"individual":null},{"id":"YELLOWDOT","name":"YELLOWDOT","role":"partner","aliases":null,"individual":null},{"id":"YELLOWDOT","name":"YELLOWDOT","role":"retailer","aliases":null,"individual":null}],"order":{"id":"6065615bf59e4e66ac604ffc","creationDate":"2022-02-18T05:53:24.408Z","state":"Completed","orderItem":{"chargedAmount":51,"currency":"XAF","validityDate":"2022-06-11T07:00:31.672Z","nextCharge":"2022-06-11T10:13:15.509Z","product":{"id":"LoveTips","href":"na","productCharacteristic":[{"name":"periodicity","value":"86400"},{"name":"startDateTime","value":"2022-06-11T10:13:15.509Z[UTC]"},{"name":"endDateTime","value":"2022-06-11T07:00:31.672Z[UTC]"},{"name":"country","value":"CMR"}]}}}},"eventTime":"2022-06-11T10:13:15.542058Z","eventType":"orderCreation"}

            if (xml.Contains("note"))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string id = json_response.note.text;
                    string orange_id = json_response.@event.id;
                    string state = json_response.@event.order.state;
                    string order_id = json_response.@event.order.id;
                    orange_id = (String.IsNullOrEmpty(orange_id) ? "" : orange_id);
                    string eventtype = json_response.eventType;



                    if (state == "Completed" && !String.IsNullOrEmpty(id))
                    {
                        if (eventtype == "orderDeletion")
                        {
                            string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("select enc_msisdn from orange_sub_requests where id = " + id, ref lines);
                            if (!String.IsNullOrEmpty(enc_msisdn))
                            {
                                string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                Int64 service_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select service_id from orange_sub_requests where id = " + id, ref lines);
                                string sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select if(GROUP_CONCAT(s.subscriber_id) is NULL,0,GROUP_CONCAT(s.subscriber_id)) from subscribers_ocm so, subscribers s WHERE s.subscriber_id = so.subscriber_id AND so.encrypted_msisdn = '"+enc_msisdn+"' AND s.service_id = "+service_id+" AND s.state_id = 1", ref lines);
                                if (sub_id != "0")
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update subscribers set state_id = 2, deactivation_date = now() where subscriber_id in (" + sub_id + ")", ref lines);
                                    Api.CommonFuncations.Notifications.SendUnSubscriptionNotification("2371", service_id.ToString(), dt, enc_msisdn, ref lines);
                                }
                                
                            }
                        }
                        if (eventtype == "orderCreation")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update orange_sub_requests set orange_id2 = '" + orange_id + "', response_datetime = now() where id = " + id, ref lines);
                            //insert to sub
                            Int64 service_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select service_id from orange_sub_requests where id = " + id, ref lines);
                            string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("select enc_msisdn from orange_sub_requests where id = " + id, ref lines);
                            Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values (2371," + service_id + ",now(),1)", ref lines);
                            if (sub_id > 0)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into subscribers_ocm (subscriber_id, encrypted_msisdn, ocm_sub_id) values(" + sub_id + ", '" + enc_msisdn + "', '" + order_id + "')", ref lines);
                                //Send notification
                                string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                string trans_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select trans_id from orange_sub_requests where id = " + id, ref lines);
                                Api.CommonFuncations.Notifications.SendSubscriptionNotificationWithEncMSISDN("2371", service_id.ToString(), dt, trans_id, enc_msisdn, ref lines);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "exception: " + ex.ToString(), 100, "orange_guinea_billing");
                }
            }

            if (xml.Contains("deliveryInfoNotification"))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string callbackData = json_response.deliveryInfoNotification.callbackData;
                    string msisdn = json_response.deliveryInfoNotification.deliveryInfo.address;
                    if (!String.IsNullOrEmpty(msisdn))
                    {
                        msisdn = msisdn.Replace("tel:+", "");
                        string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_id from orange_vas_dlr_sms where callback_id = '" + callbackData + "'", ref lines);
                        string enc_msisdn1 = Api.DataLayer.DBQueries.SelectQueryReturnString("select enc_msisdn from orange_vas_dlr_sms where callback_id = '" + callbackData + "'", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("delete from orange_vas_dlr_sms where callback_id = '" + callbackData + "'", ref lines);
                        string sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT GROUP_CONCAT(so.subscriber_id) FROM subscribers_ocm so, subscribers s WHERE s.subscriber_id = so.subscriber_id AND s.service_id = " + service_id + " AND so.encrypted_msisdn = '" + enc_msisdn1 + "'", ref lines);
                        if (!String.IsNullOrEmpty(sub_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update subscribers_ocm set msisdn = " + msisdn + " where subscriber_id = " + sub_id, ref lines);
                        }
                    }
                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, "exception: " + ex.ToString(), 100, "orange_guinea_billing");
                }
                
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