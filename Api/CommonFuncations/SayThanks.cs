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
    public class SayThanks

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
        public static string GetSayThanksBearerToken(ServiceClass service, ref List<LogLines> lines)
        {
            string bearer = "";
            List<SayThanksServiceInfo> result_list = Api.DataLayer.DBQueries.GetSayThanksServiceInfo(ref lines);
            if (result_list != null)
            {
                SayThanksServiceInfo say_thanks_service = result_list.Find(x => x.service_id == service.service_id);
                if (say_thanks_service != null)

                {
                    string json = "";
                    json = "{\"email\":" + "\"" + say_thanks_service.username + "\"" + ", \"password\":" + "\"" + say_thanks_service.password + "\"}";
                    List<Headers> headers = new List<Headers>();
                    //headers.Add(new Headers
                    //{
                    //    key = "Accept",
                    //    value = "*/*"
                    //});
                    
                    //headers.Add(new Headers 
                    //{ 
                    //    key = "Authorization", 
                    //    value = "Bearer "
                    //});
                    string response_body = CallSoap.CallSoapRequestUA(say_thanks_service.base_url + "/api/auth/login", json, headers, 4, "postman", ref lines);
                    if (!String.IsNullOrEmpty(response_body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        try
                        {
                            bearer = json_response.access_token;
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                        }
                    }
                }
            }
            return bearer;
        }
        public static CreateVoucherResponse CreateVoucher(CreateVoucherRequest RequestBody) 
        {
            
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "CreateSayThanksVoucher");
            CreateVoucherResponse ret = new CreateVoucherResponse() 
            {
                ResultCode = 1050,
                Description = "Voucher was not created",
                TransactionID = "-1"
            };
            string bearer = "";
            
            if (ValidateRequest(RequestBody, ref lines))
            {
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                if (service != null)
                {
                    CheckLogin cl = CheckLoginToken(RequestBody.ServiceID, RequestBody.TokenID, ref lines);
                    if (cl == null)
                    {
                        {
                            SayThanksServiceInfo result_list = Api.Cache.Services.GetSayThanksServiceInfo(service, ref lines);
                            string json = "";
                            string query_real_campaign_id = "select sci.campaign_id from saythanks_campaign_info sci where sci.id = " + RequestBody.CampaignID + ";";
                            Int64 real_campaign_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64(query_real_campaign_id, ref lines);
                            string query_real_amount = "select sci.amount from saythanks_campaign_info sci where sci.campaign_id = " + real_campaign_id + ";";
                            Int64 real_amount = Api.DataLayer.DBQueries.SelectQueryReturnInt64(query_real_amount, ref lines);
                            json = "{\"campaign_id\":" + real_campaign_id + ", \"customer_msisdn\":" + "\"" + RequestBody.MSISDN + "\"" + ", \"customer_name\":" + "\"" + (RequestBody.MSISDN) + "\"" + ", \"value\":" + 100*(real_amount)  + ", \"send\":true}";
                            List<Headers> headers = new List<Headers>();
                            headers.Add(new Headers
                            {
                                key = "Authorization",
                                value = "bearer " + result_list.bearer_token
                            });
                            string query = "INSERT INTO saythanks_requests (msisdn, service_id, date_time, amount, transaction_id, description) VALUES( " + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), " + real_amount + ",'"+ RequestBody.TransactionID +"','"+ RequestBody.Description +"')";
                            Int64 id =  Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(query, ref lines);
                            string response_body = CallSoap.CallSoapRequestUA(result_list.base_url + "/api/voucher", json, headers, 4, "postman", ref lines);
                            if (!String.IsNullOrEmpty(response_body))
                            {

                                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                                try
                                {
                                    string voucher_id = json_response.id;
                                    string status = json_response.status;
                                    if (!String.IsNullOrEmpty(voucher_id) && !String.IsNullOrEmpty(status))
                                    {
                                        if (status == "generated")
                                        {
                                            query = "UPDATE saythanks_requests set voucher_id = '" + voucher_id + "' WHERE id =" + id;
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