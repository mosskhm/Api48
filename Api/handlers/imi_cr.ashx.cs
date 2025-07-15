using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for imi_cr
    /// </summary>
    public class imi_cr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "imi_cr");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "imi_cr");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "imi_cr");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "imi_cr");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "imi_cr");
            }

            string redirect_url = "";
            string secret_key = "peKe0ZaSsv";
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "imi_cr");
            }
            string e, h, m, ydtrack_id;
            e = context.Request.QueryString["e"];
            h = context.Request.QueryString["h"];
            m = context.Request.QueryString["m"];
            ydtrack_id = context.Request.QueryString["ydtrack_id"];

            if (!String.IsNullOrEmpty(e) && !String.IsNullOrEmpty(h) && !String.IsNullOrEmpty(m))
            {
                //check user
                ServiceClass service = GetServiceByServiceID(352, ref lines);
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
                        CheckUserStateRequest CheckUserStateBody = new CheckUserStateRequest()
                        {
                            MSISDN = Convert.ToInt64(m),
                            ServiceID = service.service_id,
                            TokenID = res.TokenID
                        };
                        CheckUserStateResponse res1 = CheckUserState.DoCheckUserState(CheckUserStateBody);
                        if (res1 != null)
                        {
                            Int64 sub_id = 0;
                            if (res1.ResultCode == 1000)
                            {
                                if (res1.State == "Deactivated")
                                {
                                    //add user
                                    sub_id = DataLayer.DBQueries.InsertSub(m, service.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "7", "IMI", ref lines);
                                    lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                                    CommonFuncations.Notifications.SendSubscriptionNotification(m, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                                }
                                if (res1.State == "Active")
                                {
                                    sub_id = res1.SubscriberID;
                                    //sub_id = DataLayer.DBQueries.SelectQueryReturnInt64("select s.subscriber_id from .subscribers s where s.service_id = "+ service.service_id.ToString() + " and s.msisdn = "+ m + " and s.state_id = 1 limit 1", ref lines);
                                    lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");

                                }
                            }
                            if (res1.ResultCode == 5000)
                            {
                                sub_id = DataLayer.DBQueries.InsertSub(m, service.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "7", "IMI", ref lines);
                                lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                                CommonFuncations.Notifications.SendSubscriptionNotification(m, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                            }
                            if (sub_id > 0)
                            {
                                ydtrack_id = CommonFuncations.YDTracking.GetYDTrackingID(context, service.service_id, ref lines);
                                lines = Add2Log(lines, "ydtrack_id = " + ydtrack_id, 100, "imi_cr");
                                if (!String.IsNullOrEmpty(ydtrack_id))
                                {
                                    if (ydtrack_id != "0")
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests tr set tr.msisdn = " + m + ", tr.subscriber_id = " + sub_id + " where tr.id = " + ydtrack_id, ref lines);
                                    }
                                }
                            }
                            
                        }
                    }
                }
                redirect_url = "http://sports7.ydot.co/sports7/index?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(m, 1);
                lines = Add2Log(lines, " redirecting to " + redirect_url, 100, "imi_ydgames");
            }
            else
            {
                //add cookie 
                if (!String.IsNullOrEmpty(ydtrack_id))
                {
                    HttpCookie aCookie = new HttpCookie("ydtrack_id");
                    aCookie.Value = ydtrack_id;
                    aCookie.Expires = DateTime.Now.AddDays(1);
                    context.Response.Cookies.Add(aCookie);
                    
                }
                lines = Add2Log(lines, " params are empty redirecting to http://mtnplay.co.za/portal/jqm.aspx?typxm=subscribecheckCR7Portal", 100, "imi_ydgames");
                redirect_url = "http://mtnplay.co.za/portal/jqm.aspx?typxm=subscribecheckCR7Portal";
            }
            lines = Write2Log(lines);
            context.Response.Redirect(redirect_url);
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