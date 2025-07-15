using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class BankCollectionInLine
    {
        public static bool ValidateRequest(BankCollectionInLineRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount + ", TransactionID = " + (String.IsNullOrEmpty(RequestBody.TransactionID) ? "" : RequestBody.TransactionID) + ", FullName = " + RequestBody.FullName + ", Email = " + RequestBody.Email + ", RedirectURL = " + RequestBody.RedirectURL + ", LogoURL = " + RequestBody.LogoURL;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0) && (!String.IsNullOrEmpty(RequestBody.FullName)) && (!String.IsNullOrEmpty(RequestBody.Email)) && (!String.IsNullOrEmpty(RequestBody.TransactionID)) && (!String.IsNullOrEmpty(RequestBody.RedirectURL)) && (!String.IsNullOrEmpty(RequestBody.LogoURL)))
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

        public static BankCollectionInLineResponse CollectMoney(BankCollectionInLineRequest RequestBody)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "CollectMoneyInLine");
            BankCollectionInLineResponse ret = null;
            string mytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string mydate = DateTime.Now.ToString("yyyy-MM-dd");
            Int64 dya_trans = -1;

            if (ValidateRequest(RequestBody, ref lines))
            {
                DYACheckAccountRequest req = new DYACheckAccountRequest()
                {
                    MSISDN = RequestBody.MSISDN,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID
                };
                DLDYAValidateAccount result = DBQueries.ValidateDYARequest(req, ref lines);

                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);

                string check_trans = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT d.dya_id FROM dya_transactions d WHERE d.service_id = " + RequestBody.ServiceID + " AND d.msisdn = " + RequestBody.MSISDN + " AND d.transaction_id = '" + RequestBody.TransactionID + "' and d.result in ('01','-1') LIMIT 1", ref lines);
                if (!String.IsNullOrEmpty(check_trans))
                {
                    ret = new BankCollectionInLineResponse()
                    {
                        ResultCode = 1041,
                        Description = "Transaction Already Exists for this user",
                        TransactionID = "-1",
                    };
                    result.RetCode = 1041;
                    result.Description = "Transaction Already Exists for this user";
                }
                if ((result != null) && (result.RetCode == 1000))
                {
                    dya_trans = DBQueries.InsertBankCollectionInline(RequestBody, mytime, ref lines);
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (service.hour_diff != "0")
                    {
                        mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                        mydate = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd");
                    }
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (dya_trans > 0)
                    {
                        if (ret == null)
                        {
                            ret = new BankCollectionInLineResponse()
                            {
                                ResultCode = 1302,
                                Description = "Redirect User",
                                TransactionID = dya_trans.ToString(),
                                Timestamp = mytime,
                                URL = "https://api.ydplatform.com/FWInline/Check/" + dya_trans  
                            };
                        }
                    }
                    else
                    {
                        ret = new BankCollectionInLineResponse()
                        {
                            ResultCode = 5002,
                            Description = "Internal Error",
                            TransactionID = dya_trans.ToString(),
                            Timestamp = mytime
                        };
                    }

                }
                else
                {
                    if (result == null)
                    {
                        ret = new BankCollectionInLineResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error",
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                    else
                    {
                        ret = new BankCollectionInLineResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description,
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                }
            }
            else
            {
                ret = new BankCollectionInLineResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Params",
                    TransactionID = "-1",
                };
            }

            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID + ", URL = " + (String.IsNullOrEmpty(ret.URL) ? "" : ret.URL);
            lines = Add2Log(lines, text, 100, "DYATransferMoney");
            

            lines = Write2Log(lines);

            return ret;
        }
    }
}