using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for get_balance
    /// </summary>
    public class get_balance : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "get_balance");

            string service_id = context.Request.QueryString["service_id"];
            ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
            string balance_withdraw = Api.CommonFuncations.MTNOpeanAPI.CheckBalance(service, 1, ref lines);
            string balance_deposit = Api.CommonFuncations.MTNOpeanAPI.CheckBalance(service, 2, ref lines);

            context.Response.ContentType = "text/plain";
            context.Response.Write(service.service_name + ", Deposit: " + balance_deposit + ", Withdraw:" + balance_withdraw);

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