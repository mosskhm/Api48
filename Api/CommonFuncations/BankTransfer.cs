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
    public class BankTransfer
    {
        public static bool ValidateRequest(BankTransferRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount + ", TransactionID = " + (String.IsNullOrEmpty(RequestBody.TransactionID) ? "" : RequestBody.TransactionID) + ", FullName = " + RequestBody.FullName + ", AccountNumber = " + RequestBody.AccountNumber + ", BankID = " + RequestBody.BankID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0) && (!String.IsNullOrEmpty(RequestBody.FullName)) && (!String.IsNullOrEmpty(RequestBody.AccountNumber)) && (!String.IsNullOrEmpty(RequestBody.BankID)))
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

        public static BankTransferResponse TransferMoney(BankTransferRequest RequestBody)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "BankTransfer");
            BankTransferResponse ret = null;
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
                Int64 prev_dyaID = String.IsNullOrEmpty(check_trans) ? 0 : Convert.ToInt64(check_trans);
                if (prev_dyaID > 0)
                {
                    lines = Add2Log(lines, $"Found existing transaction dya_id={check_trans} for transactionID={RequestBody.TransactionID}", 100, "BankTransfer");

                    ret = new BankTransferResponse()
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
                    bool delay_withdraw = (service.delay_withdraw == true ? true : false);
                    RequestBody.OverrideDelay = (String.IsNullOrEmpty(RequestBody.OverrideDelay) ? "" : RequestBody.OverrideDelay);
                    delay_withdraw = (RequestBody.OverrideDelay == "1" ? false : delay_withdraw);
                    lines = Add2Log(lines, " delay_withdraw = " + delay_withdraw, 100, "MOMO");
                    //lines = Add2Log(lines, RequestBody.Amount + " | " + service.delay_withdraw_amount, 100, "MOMO");
                    if (delay_withdraw)
                    {
                        lines = Add2Log(lines, "Delaying Withdraw", 100, "DYATransferMoney");
                        Int64 delay_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into delayed_withdraw_transactions (msisdn, service_id, date_time, amount, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), " + RequestBody.Amount + ", '" + RequestBody.TransactionID + "')", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into delayed_banktransfer_transactions (id, full_name, bank_id, account_number) values("+delay_id+",'"+RequestBody.FullName+"','"+RequestBody.BankID+"','"+RequestBody.AccountNumber+"')", ref lines);
                        string mytime1 = mytime;
                        if (service.hour_diff != "0")
                        {
                            mytime1 = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        ret = new BankTransferResponse()
                        {
                            ResultCode = 1000,
                            Description = "Delayed",
                            TransactionID = delay_id.ToString(),
                            Timestamp = mytime1
                        };
                    }
                    else
                    {
                        dya_trans = DBQueries.InsertBankTrans(RequestBody, mytime, ref lines);
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        if (service.hour_diff != "0")
                        {
                            mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                            mydate = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd");
                        }
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        if (dya_trans > 0)
                        {
                            if (service.daily_momo_limit > 0)
                            {
                                Int64 current_momo_daily_amount = DBQueries.SelectQueryReturnInt64("SELECT s.current_momo FROM service_momo_limitation s WHERE s.service_id = " + RequestBody.ServiceID, ref lines);
                                if (current_momo_daily_amount >= service.daily_momo_limit)
                                {
                                    lines = Add2Log(lines, "Limit has reached", 100, "DYATransferMoney");
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = 1040,
                                        Description = "Transfer has failed with the following error: withdraw limit reached",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                    DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'Withdraw limit reached' where dya_id = " + dya_trans, ref lines);
                                }
                                else
                                {
                                    lines = Add2Log(lines, "current_momo_daily_amount = " + current_momo_daily_amount + " < " + service.daily_momo_limit, 100, "DYATransferMoney");
                                }
                            }
                            if (ret == null && service.user_w_limit > 0)
                            {
                                Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_w FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines);
                                if (user_withdraw_limit >= service.user_w_limit)
                                {
                                    lines = Add2Log(lines, "User Limit has reached", 100, "DYATransferMoney");
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = 1040,
                                        Description = "Transfer has failed with the following error: user withdraw limit reached",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                    DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw limit reached' where dya_id = " + dya_trans, ref lines);
                                }
                                else
                                {
                                    lines = Add2Log(lines, "user_withdraw_limit = " + user_withdraw_limit + " < " + service.user_w_limit, 100, "DYATransferMoney");
                                }
                            }

                            if (ret == null && service.user_f_limit > 0)
                            {
                                Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_f FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines);
                                if (user_withdraw_limit >= service.user_f_limit)
                                {
                                    lines = Add2Log(lines, "User Failed Limit has reached", 100, "DYATransferMoney");
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = 1040,
                                        Description = "Transfer has failed with the following error: user withdraw failed limit reached",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                    DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw failed limit reached' where dya_id = " + dya_trans, ref lines);
                                }
                                else
                                {
                                    lines = Add2Log(lines, "user_failed_withdraw_limit = " + user_withdraw_limit + " < " + service.user_f_limit, 100, "DYATransferMoney");
                                }
                            }

                            if (ret == null && service.user_limit_amount > 0)
                            {
                                Int64 user_withdraw_limit_amount = DBQueries.SelectQueryReturnInt64("SELECT d.amount FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines);
                                if (user_withdraw_limit_amount >= service.user_limit_amount)
                                {
                                    lines = Add2Log(lines, "User Limit amount has reached", 100, "DYATransferMoney");
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = 1040,
                                        Description = "Transfer has failed with the following error: user withdraw amount limit reached",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                    DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw amount limit reached' where dya_id = " + dya_trans, ref lines);
                                }
                                else
                                {
                                    lines = Add2Log(lines, "user_withdraw_limit_amount = " + user_withdraw_limit_amount + " < " + service.user_limit_amount, 100, "DYATransferMoney");
                                }
                            }
                            if (ret == null && service.timeout_between_failed_withdraw > 0)
                            {
                                Int64 last_failed_withdraw = DBQueries.SelectQueryReturnInt64("SELECT TIMESTAMPDIFF(SECOND,d.date_time, NOW())+1 FROM daily_failed_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " order by d.id desc limit 1", ref lines);
                                if (last_failed_withdraw > 0 && last_failed_withdraw < service.timeout_between_failed_withdraw)
                                {
                                    lines = Add2Log(lines, "Suspending request timeout failed withdraw " + last_failed_withdraw + " < " + service.timeout_between_failed_withdraw, 100, "DYATransferMoney");
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = 1040,
                                        Description = "Transfer has failed with the following error: Timeout failed withdraw",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                    DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'Timeout failed withdraw' where dya_id = " + dya_trans, ref lines);
                                }
                                else
                                {
                                    lines = Add2Log(lines, "last_failed_withdraw = " + last_failed_withdraw, 100, "DYATransferMoney");
                                }
                            }
                            if (ret == null)
                            {
                                switch (service.dya_type)
                                {
                                    case 3:
                                    case 8: //flutterwave

                                        ret = Flutterwave.TransferToBank(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                        break;
                                    case 15: //interswitch
                                        ret = Interswitch.TransferToBank(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ret = new BankTransferResponse()
                            {
                                ResultCode = 5002,
                                Description = "Internal Error",
                                TransactionID = dya_trans.ToString(),
                                Timestamp = mytime
                            };
                        }
                    }

                    

                }
                else
                {
                    if (result == null)
                    {
                        ret = new BankTransferResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error",
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                    else
                    {
                        ret = new BankTransferResponse()
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
                ret = new BankTransferResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Params",
                    TransactionID = "-1",
                };
            }
            try
            {
                string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID + ", Timestamp = " + ret.Timestamp;
                string ts = Convert.ToDateTime(ret.Timestamp).ToString("yyyy-MM-dd");
                if (ret.ResultCode == 1000 && ret.Description != "Delayed")
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("INSERT INTO daily_withdrawls (`msisdn`, number_of_w, amount, idobet_id, date_time, service_id) VALUES (" + RequestBody.MSISDN + ", 1, " + RequestBody.Amount + ",0,'" + ts + "'," + RequestBody.ServiceID + ") ON DUPLICATE KEY UPDATE number_of_w = number_of_w + 1, amount = amount + " + RequestBody.Amount + ", date_time = '" + ts + "', service_id = " + RequestBody.ServiceID + "", ref lines);
                }
                else
                {
                    if (ret.ResultCode == 1030)
                    {
                        //lines = Add2Log(lines, "Delaying Withdraw due to Network problems", 100, "DYATransferMoney");
                        //Int64 delay_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into delayed_withdraw_transactions (msisdn, service_id, date_time, amount, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), " + RequestBody.Amount + ", '" + RequestBody.TransactionID + "')", ref lines);
                        //ret = new DYATransferMoneyResponse()
                        //{
                        //    ResultCode = 1000,
                        //    Description = "Delayed",
                        //    TransactionID = delay_id.ToString(),
                        //    Timestamp = mytime
                        //};
                        DBQueries.UpdateDYATrans(dya_trans, "1030", "DYA Request has failed - Network problem", ref lines);
                        //DBQueries.ExecuteQuery("insert into daily_failed_withdrawls (msisdn, amount, date_time) values(" + RequestBody.MSISDN + "," + RequestBody.Amount + ",now())", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("INSERT INTO daily_withdrawls (`msisdn`, idobet_id, number_of_f, date_time, service_id) VALUES (" + RequestBody.MSISDN + ", 0, 1,'" + ts + "'," + RequestBody.ServiceID + ") ON DUPLICATE KEY UPDATE number_of_f = number_of_f + 1, date_time = '" + ts + "', service_id = " + RequestBody.ServiceID + "", ref lines);
                    }
                }

                lines = Add2Log(lines, text, 100, "DYATransferMoney");
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"failed: {ex.ToString()}", 100, "DYATransferMoney");
                ret = new BankTransferResponse()
                {
                    ResultCode = 5001,
                    Description = "Internal Error",
                    TransactionID = "-1",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            lines = Write2Log(lines);

            return ret;
        }
    }
}