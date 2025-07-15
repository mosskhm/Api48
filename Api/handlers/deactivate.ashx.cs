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
    /// Summary description for deactivate
    /// </summary>
    public class deactivate : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "unsubscribe");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "unsubscribe");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "unsubscribe");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "unsubscribe");
            string msisdn = context.Request.QueryString["msisdn"];
            string service_id = context.Request.QueryString["service_id"];
            ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
            LoginRequest LoginRequestBody = new LoginRequest()
            {
                ServiceID = Convert.ToInt32(service_id),
                Password = service.service_password
            };
            LoginResponse res = Login.DoLogin(LoginRequestBody);
            if (res != null)
            {
                if (res.ResultCode == 1000)
                {
                    SubscribeRequest SubReq = new SubscribeRequest()
                    {
                        MSISDN = Convert.ToInt64(msisdn),
                        TokenID = res.TokenID,
                        ServiceID = Convert.ToInt32(service_id),
                        TransactionID = "DeactivationHandler",
                        ActivationID = "9999"
                    };
                    SubscribeResponse result = UnSubscribe.DoUnSubscribe(SubReq);
                    lines = Add2Log(lines, " description = " + result.Description, 100, "unsubscribe");
                    lines = Add2Log(lines, " code = " + result.ResultCode, 100, "unsubscribe");

                }
            }
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