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
    /// Summary description for imi_playwin
    /// </summary>
    public class imi_playwin : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "imi_playwin");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "imi_playwin");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "imi_playwin");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "imi_playwin");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "imi_playwin");
            }

            string redirect_url = "";
            string secret_key = "dAPbEoPmCf";
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "imi_bigcash");
            }
            string e, h, m;
            e = context.Request.QueryString["e"];
            h = context.Request.QueryString["h"];
            m = context.Request.QueryString["m"];

            if (!String.IsNullOrEmpty(e) && !String.IsNullOrEmpty(h) && !String.IsNullOrEmpty(m))
            {
                //check user
                ServiceClass service = GetServiceByServiceID(5, ref lines);
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
                            if (res1.ResultCode == 1000)
                            {
                                if (res1.State == "Deactivated")
                                {
                                    //add user
                                    Int64 sub_id = DataLayer.DBQueries.InsertSub(m, service.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "7", "IMI", ref lines);
                                    lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                                    CommonFuncations.Notifications.SendSubscriptionNotification(m, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);

                                }
                            }
                            if (res1.ResultCode == 5000)
                            {
                                Int64 sub_id = DataLayer.DBQueries.InsertSub(m, service.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "7", "IMI", ref lines);
                                lines = Add2Log(lines, "sub_id = " + sub_id, 100, "imi_ydgames");
                                CommonFuncations.Notifications.SendSubscriptionNotification(m, service.service_id.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ref lines);
                            }
                        }
                    }
                }
                redirect_url = "https://mtn.gamewin.co.za/Redirect?cli=" + CommonFuncations.Base64.EncodeDecodeBase64(m, 1);
                lines = Add2Log(lines, " redirecting to " + redirect_url, 100, "imi_ydgames");
            }
            else
            {
                lines = Add2Log(lines, " params are empty redirecting to http://mtnplay.co.za/portal/jqm.aspx?typxm=subscribecheckBigCash", 100, "imi_ydgames");
                redirect_url = "http://mtnplay.co.za/portal/jqm.aspx?typxm=subscribecheckGameWin";
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