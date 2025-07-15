using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class CheckUserState
    {
        public static bool ValidateRequest(CheckUserStateRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID + ", Token = " + RequestBody.TokenID;
                if ((RequestBody.MSISDN > 0) && (RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "CheckUserState");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "CheckUserState");
                    lines = Add2Log(lines, "Bad Params", log_level, "CheckUserState");
                }
            }
            return result;
        }

        public static CheckUserStateResponse DoCheckUserState(CheckUserStateRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            CheckUserStateResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", log_level, "CheckUserState");

            if (ValidateRequest(RequestBody, ref lines))
            {
                ret = DBQueries.CheckUserState(RequestBody, ref lines);
                if (ret == null)
                {
                    ret = new CheckUserStateResponse()
                    {
                        MSISDN = 0,
                        ServiceID = 0,
                        SubscriptionDate = "",
                        DeactivationDate = "",
                        State = "",
                        LastBillingDate = "",
                        ResultCode = 5001,
                        Description = "Internal Error"
                    };
                }

            }
            else
            {
                ret = new CheckUserStateResponse()
                {
                    MSISDN = 0,
                    ServiceID = 0,
                    SubscriptionDate = "",
                    DeactivationDate = "",
                    State = "",
                    LastBillingDate = "",
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", MSISDN = " + ret.MSISDN + ", State = " + ret.State + ", SubscriptionDate = " + ret.SubscriptionDate + ", DeactivationDate = " + ret.DeactivationDate + ", LastBillingDate = " + ret.LastBillingDate;
            lines = Add2Log(lines, text, log_level, "CheckUserState");
            lines = Write2Log(lines);

            return ret;
        }
    }
}