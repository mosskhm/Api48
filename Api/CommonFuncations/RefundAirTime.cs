using Api.HttpItems;
using Api.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class RefundAirTime
    {
	    public static bool ValidateRequest(RefundAirTimeRequest RequestBody, ref List<LogLines> lines,
		    ref List<object> logMessages, string app_name, string logz_id)
	    {
		    bool result = false;
		    int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
		    if (RequestBody != null)
		    {
			    string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN +
			                  ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount +
			                  ", TransactionID = " + RequestBody.TransactionID;

			    logMessages.Add(new
			    {
				    message = text,
				    msisdn = RequestBody.MSISDN,
				    application = app_name,
				    environment = "production",
				    level = "INFO",
				    timestamp = DateTime.UtcNow,
				    logz_id = logz_id
			    });

			    if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) &&
			        (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0) &&
			        (!String.IsNullOrEmpty(RequestBody.TransactionID)))
			    {
				    result = true;
				    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
			    }
			    else
			    {
				    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
				    lines = Add2Log(lines, "Bad Params", log_level, lines[0].ControlerName);

				    logMessages.Add(new
				    {
					    message = "Bad Params",
					    msisdn = RequestBody.MSISDN,
					    application = app_name,
					    environment = "production",
					    level = "INFO",
					    timestamp = DateTime.UtcNow,
					    logz_id = logz_id
				    });
			    }
		    }

		    return result;
	    }

	    public static RefundAirTimeResponse DoRefundAirTime(RefundAirTimeRequest RequestBody)
        {
	        var logMessages = new List<object>();
	        var logz_id = Guid.NewGuid().ToString().Replace("-", "");
	        var app_name = "DoRefundAirTime";

			int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "RefundAirTime");
            RefundAirTimeResponse ret = null;
            Random rand = new Random();
            int num = rand.Next(1, 11);
            ServiceClass service = new ServiceClass();
            string mytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (ValidateRequest(RequestBody, ref lines, ref logMessages, app_name, logz_id))
            {
                service = GetServiceByServiceID(RequestBody.ServiceID, ref lines, ref logMessages, app_name, logz_id);
                if (service != null)
                {
                    CheckLogin cl = CheckLoginToken(RequestBody.ServiceID, RequestBody.TokenID, ref lines);
                    if (cl == null)
                    {
                        if (service.allow_refundairtime == false)
                        {
                            ret = new RefundAirTimeResponse()
                            {
                                ResultCode = 2020,
                                Description = "Refund AirTime is not allowed for this service",
                                TransactionID = "-1"
                            };
                        }
                        else
                        {
                            //check delay
                            bool delay_withdraw = (service.delay_withdraw == true ? true : false);
                            RequestBody.OverrideDelay = (String.IsNullOrEmpty(RequestBody.OverrideDelay) ? "" : RequestBody.OverrideDelay);
                            delay_withdraw = (RequestBody.OverrideDelay == "1" ? false : delay_withdraw);
                            lines = Add2Log(lines, "delay_withdraw = " + delay_withdraw, 100, "MOMO");
                            logMessages.Add(new { message = "delay_withdraw = " + delay_withdraw, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                            
							if (delay_withdraw)
                            {
                                lines = Add2Log(lines, "Delaying Withdraw", 100, "DYATransferMoney");
                                logMessages.Add(new { message = "Delaying Withdraw", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });

								Int64 delay_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into delayed_withdraw_transactions (msisdn, service_id, date_time, amount, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), " + RequestBody.Amount + ", '" + RequestBody.TransactionID + "')", ref lines);
                                string mytime1 = mytime;
                                if (service.hour_diff != "0")
                                {
                                    mytime1 = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");

                                }
                                ret = new RefundAirTimeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Delayed",
                                    TransactionID = delay_id.ToString()
                                };
                            }
                            else
                            {
                                string check_trans = Api.DataLayer.DBQueries.CheckAirTimeRefundTransaction(RequestBody, ref lines);
                                if (String.IsNullOrEmpty(check_trans))
                                {
                                    if (service.airtime_type == 3 || service.airtime_type == 4)
                                    {
                                        service.airtime_type = num % 2 == 0 ? 4 : 3;
                                        service.airtime_type = 4;
                                    }

                                    if (service.airtime_type == 5)
                                    {
                                        service.airtime_type = num % 2 == 0 ? 5 : 4;
                                        if (num == 5 || num == 1 || num == 9)
                                        {
                                            service.airtime_type = 5;
                                        }

                                    }

                                    lines = Add2Log(lines, "Random number = " + num + ", Airtime type = " + service.airtime_type, log_level, lines[0].ControlerName);
                                    switch (service.airtime_type)
                                    {
                                        case 2:
                                            ret = Mobifingg.DoMobiFinggRefundNew(RequestBody, service, ref lines);
                                            break;
                                        case 3:
                                            ret = TrustVas.RefundAirTimeNew(RequestBody, service, ref lines);
                                            break;
                                        case 4:
                                            ret = MyHisa.RefundAirTime(RequestBody, service, ref lines);
                                            break;
                                        case 5:
                                            ret = uan_glo.RefundAirTime(RequestBody, service, ref lines);
                                            break;
                                        default:
                                            ret = new RefundAirTimeResponse()
                                            {
                                                ResultCode = 2021,
                                                Description = "Refund AirTime is currently not configured",
                                                TransactionID = "-1"
                                            };
                                            break;
                                    }
                                }
                                else
                                {
                                    ret = new RefundAirTimeResponse()
                                    {
                                        ResultCode = 1041,
                                        Description = "Transaction Already Exists for this user",
                                        TransactionID = "-1"
                                    };
                                }
                            }
                        }
                    }
                    else
                    {
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = cl.ResultCode,
                            Description = cl.Description,
                            TransactionID = "-1"
                        };
                    }
                    
                }
                else
                {
                    ret = new RefundAirTimeResponse()
                    {
                        ResultCode = 2021,
                        Description = "Service not found",
                        TransactionID = "-1"
                    };
                }
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID;

			logMessages.Add(new { message = text, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
			LogzIOHelper.SendLogsAsync(logMessages, "API");

			lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
            lines = Write2Log(lines);

            return ret;
        }
    }
}