using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Bill
    {
        public static bool ValidateRequest1(BillRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID + ", Token = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.TransactionID + ", PriceID = " + RequestBody.PriceID;
                if ((RequestBody.MSISDN > 0) && (RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (!String.IsNullOrEmpty(RequestBody.TransactionID)) && (RequestBody.PriceID > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "Bill");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "Bill");
                    lines = Add2Log(lines, "Bad Params", log_level, "Bill");
                }
            }
            return result;
        }

        public static BillResponse DoBill(BillRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            BillResponse ret = null;
            string bill_response = "";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Bill");
            if (ValidateRequest1(RequestBody, ref lines))
            {
                DLValidateBill result = string.IsNullOrEmpty(RequestBody.EncryptedMSISDN)
                                        ? DBQueries.ValidateBill(RequestBody, ref lines)
                                        : DBQueries.ValidateBillWithEncMSISDN(RequestBody, ref lines) 
                                        ;


                if ((result != null) && (result.ret_code == 1000))
                {

                    ServiceClass services = Cache.Services.GetServiceByServiceID(RequestBody.ServiceID, ref lines);

                    switch (result.operator_id)
                    {
                        case 10:
                        case 26:
                        case 2:
                            ret = PISIMobile.Bill(RequestBody, services, ref lines);
                            break;
                        case 3:
                            ret = MADAPI.Bill(RequestBody, services, ref lines);
                            break;
                        case 18:
                            ret = Orange.Bill(RequestBody, services, result, ref lines);
                            break;
                        case 32: // Orange Ivory Coast - Airtime
                            ret = Orange.billIvoryCoastAirTime(RequestBody.MSISDN.ToString(), services.subscription_amount, result.sub_id, services.service_id, ref lines);
                            lines = Add2Log(lines, $"Ivory Coast - Orange - BILL: subscriberID={result.sub_id} for serviceID = {services.service_name}, amount={services.subscription_amount}, result={ret.Description}", 100, "");
                            break;
                        case 25:
                            string real_sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT subc.real_sub_id FROM yellowdot.subscribers_cellc subc where subc.subscriber_id = " + result.sub_id + ";", ref lines);
                            ret = CellC.Bill(RequestBody,services,real_sub_id, result,ref lines);
                            break;
                        case 8:
                            PriceClass price_info = Get9MobileInfo(RequestBody.ServiceID, RequestBody.PriceID, ref lines);
                            if (price_info != null)
                            {
                                string env = (result.is_staging == true ? "stg" : "prod");
                                Int64 ninemobile_bilingid = DBQueries.ExecuteQueryReturnInt64("insert into nine_mobile_billingattempt (subscriber_id, date_time, price_id) values(" + result.sub_id + ", now(), " + RequestBody.PriceID + ")", ref lines);
                                string response = CommonFuncations.NineMobile.BillUser(env, "async", RequestBody.MSISDN.ToString(), price_info.ninemobile_service_code, (price_info.price / 100).ToString(), ninemobile_bilingid.ToString(), ref lines);

                                if (response != "")
                                {
                                    try
                                    {
                                        dynamic json_response = JsonConvert.DeserializeObject(response);
                                        string code = json_response.code;
                                        string description = json_response.description;
                                        string contextMsg = json_response.contextMsg;
                                        string tranxId = json_response.tranxId;
                                        string extTxnId = json_response.extTxnId;
                                        switch (code)
                                        {
                                            case "0":
                                                ret = new BillResponse()
                                                {
                                                    ResultCode = 1010,
                                                    Description = "Pending",
                                                    TransactionID = ninemobile_bilingid.ToString()
                                                };
                                                break;
                                            case "303":
                                                ret = new BillResponse()
                                                {
                                                    ResultCode = 1010,
                                                    Description = "Pending",
                                                    TransactionID = ninemobile_bilingid.ToString()
                                                };
                                                break;
                                            default:
                                                ret = new BillResponse()
                                                {
                                                    ResultCode = 1050,
                                                    Description = "Request has failed with the following error: " + code + " - " + description,
                                                    TransactionID = ninemobile_bilingid.ToString()
                                                };
                                                int number;
                                                if (int.TryParse(code, out number))
                                                {
                                                    DBQueries.ExecuteQuery("update nine_mobile_billingattempt set code = " + code + ", description = '" + description + "', result_date_time = now() where id = " + ninemobile_bilingid, ref lines);
                                                }
                                                else
                                                {
                                                    DBQueries.ExecuteQuery("update nine_mobile_billingattempt set code = -1, description = '', result_date_time = now() where id = " + ninemobile_bilingid, ref lines);
                                                }

                                                break;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ret = new BillResponse()
                                        {
                                            ResultCode = 1020,
                                            Description = "Request has failed",
                                            TransactionID = ninemobile_bilingid.ToString()
                                        };
                                        DBQueries.ExecuteQuery("update nine_mobile_billingattempt set code = 1020, description = '', result_date_time = now() where id = " + ninemobile_bilingid, ref lines);
                                    }
                                }
                                else
                                {
                                    ret = new BillResponse()
                                    {
                                        ResultCode = 1020,
                                        Description = "Request has failed",
                                        TransactionID = ninemobile_bilingid.ToString()
                                    };
                                    DBQueries.ExecuteQuery("update nine_mobile_billingattempt set code = 1020, description = 'Request has failed', result_date_time = now() where id = " + ninemobile_bilingid, ref lines);
                                }
                            }
                            else
                            {
                                ret = new BillResponse()
                                {
                                    ResultCode = 2021,
                                    Description = "Price not found",
                                    TransactionID = "-1"
                                };
                            }
                            break;
                    }
                    
                }
                else
                {
                    if (result == null)
                    {
                        ret = new BillResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error",
                            TransactionID = "-1"
                        };
                    }
                    else
                    {
                        ret = new BillResponse()
                        {
                            ResultCode = result.ret_code,
                            Description = result.description,
                            TransactionID = "-1"
                        };
                    }
                    
                }
                
                
            }
            else
            {
                ret = new BillResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters",
                    TransactionID = "-1"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID;
            lines = Add2Log(lines, text, log_level, "Bill");
            lines = Write2Log(lines);
            return ret;
        }
    }
}