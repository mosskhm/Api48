using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ValidateBankAccount
    {
        public static bool ValidateRequest(ValidateBankAccountRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", TokenID = " + RequestBody.TokenID + ", AccountNumber = " + RequestBody.AccountNumber + ", BankID = " + RequestBody.BankID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (!String.IsNullOrEmpty(RequestBody.AccountNumber)) && (!String.IsNullOrEmpty(RequestBody.BankID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, 100, "Login");
                }
                else
                {
                    lines = Add2Log(lines, text, 100, "Login");
                    lines = Add2Log(lines, "Bad Params", 100, "Login");
                }
            }
            return result;
        }

        public static ValidateBankAccountResponse DoValidation(ValidateBankAccountRequest RequestBody)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ValidateBankAccount");
            ValidateBankAccountResponse ret = null;

            if (ValidateRequest(RequestBody, ref lines))
            {
                SendUSSDPushRequest new_req = new SendUSSDPushRequest()
                {
                    CampaignID = 0,
                    MSISDN = 234,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID,
                    CUID = "1"
                };
                CheckLogin cl = CheckLoginToken(RequestBody.ServiceID, RequestBody.TokenID, ref lines);
                if (cl == null)
                {
                    
                        ServiceClass service = Api.Cache.Services.GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                        ret = Flutterwave.ValidateBankAccount(RequestBody, service, ref lines);
                }
                else
                {
                    ret = new ValidateBankAccountResponse()
                    {
                        ResultCode = cl.ResultCode,
                        Description = cl.Description
                    };
                }
            }
            else
            {
                ret = new ValidateBankAccountResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", AccountHolderName = " + ret.AccountHolderName;
            lines = Add2Log(lines, text, 100, "SendUSSD");
            lines = Write2Log(lines);
            return ret;
        }
    }
}