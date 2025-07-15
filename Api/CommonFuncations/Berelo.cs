using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Berelo
    {
        public static bool ValidateRequest(CreateVoucherRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Description = " + RequestBody.Description + ", TransactionID = " + RequestBody.TransactionID + ", CampaignID = " + RequestBody.CampaignID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (!String.IsNullOrEmpty(RequestBody.Description)) && (!String.IsNullOrEmpty(RequestBody.TransactionID)) && (RequestBody.CampaignID > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
                    lines = Add2Log(lines, "Bad Params", log_level, lines[0].ControlerName);
                }
            }
            return result;
        }
        
        public static CreateVoucherResponse CreateVoucher(CreateVoucherRequest RequestBody)
        {
            List<BereloServiceInfo> result_list = null;
            BereloServiceInfo result = null;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "CreateSayThanksVoucher");
            CreateVoucherResponse ret = new CreateVoucherResponse()
            {
                ResultCode = 1050,
                Description = "Voucher was not created",
                TransactionID = "-1"
            };

            if (ValidateRequest(RequestBody, ref lines))
            {
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                if (service != null)
                {
                    CheckLogin cl = CheckLoginToken(RequestBody.ServiceID, RequestBody.TokenID, ref lines);
                    if (cl == null)
                    {
                        {
                            result = Api.Cache.Services.GetBereloServiceInfo(service, ref lines);
                            string json = "";
                            string query_real_campaign_id = "select bci.campaign_id from berelo_campaign_info bci where bci.id = " + RequestBody.CampaignID + ";";
                            Int64 real_campaign_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64(query_real_campaign_id, ref lines);
                            

                            json = "{\"api_key\":\"" + result.api_key + "\", \"voucher_id\":" + "\"" + real_campaign_id + "\"" + ", \"msisdn\":" + "\"" + RequestBody.MSISDN + "\"}";

                            string query = "INSERT INTO berelo_requests (msisdn, service_id, date_time, amount, transaction_id, description) VALUES( " + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), 0," + RequestBody.TransactionID + ",'" + RequestBody.Description + "')";
                            Int64 id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(query, ref lines);
                            string response_body = CallSoap.CallSoapRequestBerelo(result.base_url + "/send_voucher_code", json, ref lines);
                            if (!String.IsNullOrEmpty(response_body))
                            {

                                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                                try
                                {
                                    string voucher_id = json_response.voucher_code_id;
                                    string status = json_response.success;
                                    if (!String.IsNullOrEmpty(voucher_id) && !String.IsNullOrEmpty(status))
                                    {
                                        if (status == "True")
                                        {
                                            query = "UPDATE berelo_requests set voucher_id = '" + voucher_id + "' WHERE id =" + id;
                                            Api.DataLayer.DBQueries.ExecuteQuery(query, ref lines);
                                            ret = new CreateVoucherResponse()
                                            {
                                                ResultCode = 1000,
                                                Description = "Voucher was created",
                                                TransactionID = id.ToString()
                                            };
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);

                                }
                            }
                        }
                    }
                    else
                    {
                        ret = new CreateVoucherResponse()
                        {
                            ResultCode = cl.ResultCode,
                            Description = cl.Description,
                            TransactionID = "-1"
                        };
                    }

                }
                else
                {
                    ret = new CreateVoucherResponse()
                    {
                        ResultCode = 2021,
                        Description = "Service not found",
                        TransactionID = "-1"
                    };
                }
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID;
            lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
            lines = Write2Log(lines);

            return ret;
        }
    }
}
