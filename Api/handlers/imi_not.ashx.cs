using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for imi_not
    /// </summary>
    public class imi_not : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "imi_not");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "imi_not");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "imi_not");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "imi_not");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "imi_not");
            }

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            if (!String.IsNullOrEmpty(xml))
            {
                lines = Add2Log(lines, "Incomming XML = " + xml, 100, "imi_not");
            }

            //2018 - 02 - 10 23:21:30.826: Key: msisdn Value: 27782321000
            //2018 - 02 - 10 23:21:30.826: Key: svcid Value: YDGAMES
            //2018 - 02 - 10 23:21:30.826: Key: status Value: SUCCESS
            //2018 - 02 - 10 23:21:30.826: Key: action Value: REN
            //2018 - 02 - 10 23:21:30.826: Key: Nextrenewaldate Value: 2018 - 02 - 12 00:19:55
            //2018 - 02 - 10 23:21:30.826: Key: transid Value: 63653905195240431827782321000
            //2018 - 02 - 10 23:21:30.826: Key: channel Value: WAP
            //2018 - 02 - 10 23:21:30.826: Key: price Value: 2

            string msisdn = context.Request.QueryString["msisdn"];
            string svcid = context.Request.QueryString["svcid"];
            string action = context.Request.QueryString["action"];
            string transid = context.Request.QueryString["transid"];
            string price = context.Request.QueryString["price"];
            string channel = context.Request.QueryString["channel"];

            

            string channel_id = "2";
            channel_id = (channel == "USSD" ? "2" : (channel == "WAP" ? "7" : (channel == "CUSTOMERCARE" ? "5": channel_id)));

            if (!String.IsNullOrEmpty(action) && !String.IsNullOrEmpty(svcid) && !String.IsNullOrEmpty(msisdn))
            {
                msisdn = (msisdn.Contains(",") ? msisdn.Substring(0,msisdn.IndexOf(",")) : msisdn);
                action = (action.Contains(",") ? action.Substring(0, action.IndexOf(",")) : action);
                svcid = (svcid.Contains(",") ? svcid.Substring(0, svcid.IndexOf(",")) : svcid);


                IMIServiceClass imi_services = GetIMIServiceID(svcid, ref lines);
                if (imi_services != null)
                {
                    int service_id = imi_services.service_id;
                    ServiceClass service = GetServiceByServiceID(service_id, ref lines);
                    LoginRequest LoginRequestBody = new LoginRequest()
                    {
                        ServiceID = service_id,
                        Password = service.service_password
                    };
                    LoginResponse res = Login.DoLogin(LoginRequestBody);
                    //CheckUserStateResponse res1 = new CheckUserStateResponse();
                    if (res != null)
                    {
                        if (res.ResultCode == 1000)
                        {
                            CheckUserStateRequest CheckUserStateBody = new CheckUserStateRequest()
                            {
                                MSISDN = Convert.ToInt64(msisdn),
                                ServiceID = service_id,
                                TokenID = res.TokenID
                            };
                            //res1 = CheckUserState.DoCheckUserState(CheckUserStateBody);
                            //lines = Add2Log(lines, "DoCheckUserState = " + res1.State + ", " + res1.ResultCode, 100, "imi_not");

                        }
                    }
                    Int64 sub_id = 0;
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into imi_not_actions (creation_date, action, price, service_id, msisdn, channel_id, handeled) values(now(), '" + action + "','" + price + "', " + service_id + "," + msisdn + ",'" + channel_id + "',0)", ref lines);
                    //switch (action)
                    //{
                    //    case "REN":
                    //        //update billing
                    //        int db_price_id = 0;
                    //        if (!String.IsNullOrEmpty(price))
                    //        {
                    //            if (price != "0")
                    //            {
                    //                price = (price.Contains(".") == true ? (Convert.ToDouble(price) * 100).ToString() : price + "00");
                    //                lines = Add2Log(lines, "price = " + price, 100, "imi_not");
                    //                PriceClass price_c = GetPricesInfo(service_id, Convert.ToDouble(price), ref lines);
                    //                if (price_c != null)
                    //                {
                    //                    db_price_id = price_c.price_id;
                    //                    lines = Add2Log(lines, " price_id = " + price_c.price_id + ", " + price_c.price + " " + price_c.curency_code, 100, "imi_not");
                    //                    sub_id = DataLayer.DBQueries.InsertSub(msisdn, service_id.ToString(), db_price_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), channel_id, "IMI", ref lines);
                    //                    lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                    //                    CommonFuncations.Notifications.SendBillingNotification(msisdn, service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (Convert.ToDouble(price_c.price) / 100).ToString(), ref lines);
                    //                }
                    //                else
                    //                {
                    //                    lines = Add2Log(lines, " Failed to load prices or price not found", 100, "imi_not");
                    //                }
                    //            }
                    //        }
                    //        break;
                    //    case "SUB":
                    //        //insert sub
                    //        //if (res1 != null)
                    //        //{
                    //            //if (res1.ResultCode == 1000)
                    //            //{
                    //            //    if (res1.State == "Deactivated")
                    //            //    {
                    //            //        //add user
                    //            //        sub_id = DataLayer.DBQueries.InsertSub(msisdn, service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", channel_id, "IMI", ref lines);
                    //            //        lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");

                    //            //    }
                    //            //}
                    //            //if (res1.ResultCode == 5000)
                    //            //{
                    //            //    sub_id = DataLayer.DBQueries.InsertSub(msisdn, service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", channel_id, "IMI", ref lines);
                    //            //    lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                    //            //}
                    //            sub_id = DataLayer.DBQueries.InsertSub(msisdn, service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", channel_id, "IMI", ref lines);
                    //            lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");

                    //            if (price != "0" || !String.IsNullOrEmpty(price))
                    //            {
                    //                price = (price.Contains(".") == true ? (Convert.ToDouble(price)*100).ToString() : price + "00");
                    //                lines = Add2Log(lines, "price = " + price, 100, "imi_not");
                    //            }
                    //            else
                    //            {
                    //                price = "0";
                    //            }
                    //            CommonFuncations.Notifications.SendSubscriptionNotification(msisdn, service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                    //            CommonFuncations.Notifications.SendBillingNotification(msisdn, service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), (Convert.ToDouble(price) / 100).ToString(), ref lines);
                    //        //}
                    //        break;
                    //    case "UNSUB":
                    //        //deactivate
                    //        sub_id = DataLayer.DBQueries.UnsubscribeSub(msisdn, service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), channel_id, "IMI", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                    //        CommonFuncations.Notifications.SendUnSubscriptionNotification(msisdn, service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                    //        break;
                    //}
                }
                else
                {
                    lines = Add2Log(lines, " svcid was not found!", 100, "imi_not");
                }
                
            }








            lines = Write2Log(lines);


            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
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