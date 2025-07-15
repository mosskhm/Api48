using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.MTNOpeanAPI;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class Services
    {
        public class ServiceClass
        {
            public int service_id { get; set; }
            public string service_name { get; set; }
            public string spid { get; set; }
            public string real_service_id { get; set; }
            public string product_id { get; set; }
            public string sms_mt_code { get; set; }
            public string operator_name { get; set; }
            public string country_name { get; set; }
            public string service_password { get; set; }
            public bool is_ondemand { get; set; }
            public bool allow_dya { get; set; }
            public bool allow_refundairtime { get; set; }
            public bool is_staging { get; set; }
            public int operator_id { get; set; }
            public bool allow_chargeairtime { get; set; }
            public bool subscribe_wo_serviceid { get; set; }
            public string spid_password { get; set; }
            public int airtimr_amount { get; set; }
            public int airtime_code { get; set; }
            public string nine_mobile_service_name { get; set; }
            public string momo_service_id { get; set; }
            public int dya_type { get; set; }
            public int airtime_type { get; set; }
            public Int64 daily_momo_limit { get; set; }
            public string timezone_code { get; set; }
            public int user_w_limit { get; set; }
            public int user_f_limit { get; set; }
            public int user_limit_amount { get; set; }
            public string hour_diff { get; set; }
            public int timeout_between_failed_withdraw { get; set; }
            public string convert_tz { get; set; }
            public bool use_fulfilment { get; set; }
            public bool delay_withdraw { get; set; }
            public int delay_withdraw_amount { get; set; }
            public bool whitelist_only { get; set; }
            public bool add_zero { get; set; }
            public bool block_withdraw { get; set; }
            public bool block_deposit { get; set; }
            public double subscription_amount {  get; set; }
        }

        public class IMIServiceClass
        {
            public int service_id { get; set; }
            public string svcid { get; set; }
        }
        public class AirtelTigoInfo
        {
            public int service_id { get; set; }
            public string base_url { get; set; }
            public string api_key { get; set; }
            public Int64 product_id_sub { get; set; }
            public Int64 largeAccount { get; set; }
            public string preSharedKey { get; set; }

            public int mnc { get; set; }
            public int mcc { get; set; }



        }

        public class ServiceURLS
        {
            public int service_id { get; set; }
            public string billing_url { get; set; }
            public string subscription_url { get; set; }
            public string mo_url { get; set; }
            public string unsubscription_url { get; set; }
            public string sms_dlr_url { get; set; }
            public string dya_receive_url { get; set; }
            public int notification_method_id { get; set; }
            public string chargeamount_url { get; set; }
            public string service_url { get; set; }
            public string error_url { get; set; }
            public string dtt_url { get; set; }

        }

        public class FulfillmentInfo
        {
            public int service_id { get; set; }
            public string operationType { get; set; }
            public string iname { get; set; }
            public string productCode { get; set; }
            public string splitno { get; set; }
            public string d_productCode { get; set; }
        }

        public class ECWServiceInfo
        {
            public int service_id { get; set; }
            public string cacert_path { get; set; }
            public string cert_path { get; set; }
            public string key_path { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string validateaccountholder { get; set; }
            public string transfer { get; set; }
            public string gettransactionstatus { get; set; }
            public string debit { get; set; }
            public string getaccountholderinfo { get; set; }
            public string output_path { get; set; }
        }

        public class FlutterwaveServiceInfo
        {
            public int service_id { get; set; }
            public string bearear { get; set; }
            public string currency { get; set; }
            public string callback_url { get; set; }
            public string banktransfer_url { get; set; }
            public string country { get; set; }
            public string momo_receive_url { get; set; }
            public string momo_transfer_url { get; set; }
            public string momo_validate_receive_url { get; set; }
            public string momo_validate_transfer_url { get; set; }
            public string balances_url { get; set; }

        }

        public class NJService
        {
            public int nj_service_id { get; set; }
            public int service_id { get; set; }
        }


        public static string GetPISIBearer(ref List<LogLines> lines)
        {
            string result = null;
            DateTime exp_date1 = DateTime.Now.AddHours(10);
            lines = Add2Log(lines, " GetPISIBearer()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetPISIBearer"] != null)
                {
                    lines = Add2Log(lines, " GetPISIBearer Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetPISIBearer_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetPISIBearer_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = (string)HttpContext.Current.Application["GetPISIBearer"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetPISIBearer Cache ", 100, "");
                            string exp_date = "";
                            result = Api.CommonFuncations.PISIMobile.GetBearerToken(ref lines, out exp_date);
                            if (!String.IsNullOrEmpty(exp_date))
                            {
                                
                                try
                                {
                                    if (Convert.ToDateTime(exp_date) >= DateTime.Now.AddHours(10))
                                    {
                                        exp_date1 = Convert.ToDateTime(exp_date);
                                    }
                                }
                                catch(Exception ex)
                                {

                                }
                            }
                            if (!String.IsNullOrEmpty(result))
                            {
                                HttpContext.Current.Application["GetPISIBearer"] = result;
                                HttpContext.Current.Application["GetPISIBearer_expdate"] = exp_date1;
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetPISIBearer Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetPISIBearer Cache ", 100, "");
                    string exp_date = "";
                    result = Api.CommonFuncations.PISIMobile.GetBearerToken(ref lines, out exp_date);
                    if (!String.IsNullOrEmpty(exp_date))
                    {

                        try
                        {
                            if (Convert.ToDateTime(exp_date) >= DateTime.Now.AddHours(10))
                            {
                                exp_date1 = Convert.ToDateTime(exp_date);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (!String.IsNullOrEmpty(result))
                    {
                        HttpContext.Current.Application["GetPISIBearer"] = result;
                        HttpContext.Current.Application["GetPISIBearer_expdate"] = exp_date1;
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetPISIBearer Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetPISIBearer From DB ", 100, "");
                string exp_date = "";
                result = Api.CommonFuncations.PISIMobile.GetBearerToken(ref lines, out exp_date);
            }

            
            return result;
        }

        public static NJService GetNJServiceInfo(int nj_service_id, ref List<LogLines> lines)
        {
            NJService result = null;
            List<NJService> result_list = new List<NJService>();
            lines = Add2Log(lines, " GetNJServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetNJServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetNJServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetNJServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetNJServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<NJService>)HttpContext.Current.Application["GetNJServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetNJServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetNJServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetNJServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetNJServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetNJServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetNJServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetNJServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetNJServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetNJServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetNJServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetNJServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetNJServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.nj_service_id == nj_service_id);

                }
            }
            return result;
        }

        public static List<SMSServiceInfo> GetSMSServiceInfo(ref List<LogLines> lines)
        {

            List<SMSServiceInfo> result_list = new List<SMSServiceInfo>();
            lines = Add2Log(lines, " GetSMSServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetSMSServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetSMSServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSMSServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSMSServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<SMSServiceInfo>)HttpContext.Current.Application["GetSMSServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSMSServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetSMSServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetSMSServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetSMSServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetSMSServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSMSServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetSMSServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetSMSServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetSMSServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSMSServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSMSServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetSMSServiceInfo(ref lines);
            }


            return result_list;
        }

        public static List<NJService> GetAllNJServiceInfo(ref List<LogLines> lines)
        {
            
            List<NJService> result_list = new List<NJService>();
            lines = Add2Log(lines, " GetNJServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetNJServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetNJServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetNJServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetNJServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<NJService>)HttpContext.Current.Application["GetNJServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetNJServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetNJServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetNJServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetNJServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetNJServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetNJServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetNJServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetNJServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetNJServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetNJServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetNJServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetNJServices(ref lines);
            }

            
            return result_list;
        }


        public static FlutterwaveServiceInfo GetFlutterwaveInfo(int service_id, ref List<LogLines> lines)
        {
            FlutterwaveServiceInfo result = null;
            List<FlutterwaveServiceInfo> result_list = new List<FlutterwaveServiceInfo>();
            lines = Add2Log(lines, " GetFlutterwaveInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetFlutterwaveInfo"] != null)
                {
                    lines = Add2Log(lines, " GetFlutterwaveInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetFlutterwaveInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetFlutterwaveInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<FlutterwaveServiceInfo>)HttpContext.Current.Application["GetFlutterwaveInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetFlutterwaveInfo Cache ", 100, "");
                            result_list = DBQueries.GetFlutterWaveServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetFlutterwaveInfo"] = result_list;
                                HttpContext.Current.Application["GetFlutterwaveInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetFlutterwaveInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetFlutterwaveInfo Cache ", 100, "");
                    result_list = DBQueries.GetFlutterWaveServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetFlutterwaveInfo"] = result_list;
                        HttpContext.Current.Application["GetFlutterwaveInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetFlutterwaveInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetFlutterwaveInfo From DB ", 100, "");
                result_list = DBQueries.GetFlutterWaveServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);

                }
            }
            return result;
        }

        public class OrangeServiceInfo
        {
            public int service_id { get; set; }
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string base_url { get; set; }
            public string token_url { get; set; }
            public string bearer { get; set; }
            public string bearer_date { get; set; }
            public string currency { get; set; }
            public string posId { get; set; }
            public string sendsms_url { get; set; }
            public string webpay_url { get; set; }
        }

        public class SayThanksServiceInfo
        {
            public int service_id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string bearer_token { get; set; }
            public string bearer_token_expiration_date { get; set; }
            public string base_url { get; set; }
            public int campaign_id { get; set; } 
        }

        public class BereloServiceInfo
        {
            public int service_id { get; set; }
            public int campaign_id { get; set; }

            public string api_key { get; set; }
            public string base_url { get; set; }
        }

        public class MADAPIServiceInfo
        {
            public int service_id { get; set; }
            public string consumer_key { get; set; }
            public string consumer_secret { get; set; }
            public string base_url { get; set; }
            public string access_token { get; set; }
            public string access_token_validdate { get; set; }
            public string sender_address { get; set; }
        }

        public class OpenAPIServiceInfo
        {
            public int service_id { get; set; }
            public string transfer_apiuser { get; set; }
            public string transfer_apikey { get; set; }
            public string transfer_ocp_ask { get; set; }
            public string transfer_b_token { get; set; }
            public string transfer_b_date { get; set; }
            public string receive_apiuser { get; set; }
            public string receive_apikey { get; set; }
            public string receive_ocp_ask { get; set; }
            public string receive_b_token { get; set; }
            public string receive_b_date { get; set; }
            public string base_url { get; set; }
            public string curency { get; set; }
            public string x_target_environment { get; set; }
            public string callback_url { get; set; }
        }

        public class KKiaPay
        {
            public int service_id { get; set; }
            public string api_key { get; set; }
            public string logo_url { get; set; }
            public string callback_url { get; set; }
            public bool is_staging { get; set; }
            public string position { get; set; }
            public string theme { get; set; }

        }

        public static KKiaPay GetKKiaPayServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            KKiaPay result = null;
            List<KKiaPay> result_list = new List<KKiaPay>();
            lines = Add2Log(lines, " GetKKiaPayServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetKKiaPayServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetKKiaPayServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetKKiaPayServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetKKiaPayServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<KKiaPay>)HttpContext.Current.Application["GetKKiaPayServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetKKiaPayServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetKKiaPayServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetKKiaPayServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetKKiaPayServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetKKiaPayServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetKKiaPayServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetKKiaPayServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetKKiaPayServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetKKiaPayServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetKKiaPayServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetKKiaPayServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetKKiaPayServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                }
            }
            return result;
        }

        public static OpenAPIServiceInfo GetOpenApiServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            OpenAPIServiceInfo result = null;
            List<OpenAPIServiceInfo> result_list = new List<OpenAPIServiceInfo>();
            lines = Add2Log(lines, " GetOpenApiServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOpenApiServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetOpenApiServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOpenApiServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOpenApiServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<OpenAPIServiceInfo>)HttpContext.Current.Application["GetOpenApiServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOpenApiServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetOpenAPIServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetOpenApiServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetOpenApiServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetOpenApiServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOpenApiServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetOpenAPIServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetOpenApiServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetOpenApiServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetOpenApiServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOpenApiServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetOpenAPIServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                    if (result != null)
                    {
                        if (Convert.ToDateTime(result.transfer_b_date) < DateTime.Now)
                        {
                            OpenAPIBearers bearers = CommonFuncations.MTNOpeanAPI.GetBearerToken(service, ref lines);
                            if (bearers != null)
                            {
                                int my_service_id = (service.service_id == 777 ? 716 : service.service_id);
                                string mydate = DateTime.Now.AddMinutes(45).ToString("yyyy-MM-dd HH:mm:ss");
                                result.receive_b_token = bearers.receive_b_token;
                                result.transfer_b_token = bearers.transfer_b_token;
                                result.transfer_b_date = mydate;
                                result.receive_b_date = mydate;
                                Api.DataLayer.DBQueries.ExecuteQuery("update service_openapi_configuration set transfer_b_token = '" + bearers.transfer_b_token + "', transfer_b_token_datetime = '" + mydate + "', receive_b_token = '"+bearers.receive_b_token+ "', receive_b_token_datetime = '"+mydate+"' where service_id = " + my_service_id, ref lines);
                            }
                        }
                    }

                }
            }
            return result;
        }

        public static OrangeServiceInfo GetOrangeServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            OrangeServiceInfo result = null;
            List<OrangeServiceInfo> result_list = new List<OrangeServiceInfo>();
            lines = Add2Log(lines, " GetOrangeServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetOrangeServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetOrangeServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetOrangeServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetOrangeServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<OrangeServiceInfo>)HttpContext.Current.Application["GetOrangeServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetOrangeServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetOrangeServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetOrangeServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetOrangeServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetOrangeServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetOrangeServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetOrangeServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetOrangeServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetOrangeServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetECWServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetOrangeServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetOrangeServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                    if (result != null)
                    {
                        if (Convert.ToDateTime(result.bearer_date) < DateTime.Now)
                        {
                            string bearer = CommonFuncations.Orange.GetBearerToken(service, ref lines);
                            if (!String.IsNullOrEmpty(bearer))
                            {
                                string mydate = DateTime.Now.AddMinutes(45).ToString("yyyy-MM-dd HH:mm:ss");
                                result.bearer = bearer;
                                result.bearer_date = mydate;
                                Api.DataLayer.DBQueries.ExecuteQuery("update service_orange_configuration set bearer = '"+bearer+"', bearer_valid_date = '"+mydate+"' where service_id = " + service.service_id, ref lines);
                            }
                        }
                    }

                }
            }
            return result;
        }

        public class DusnServiceInfo
        {
            public int service_id { get; set; }
            public string token_id { get; set; }
            public string exp_datetime { get; set; }
        }

        public static DusnServiceInfo GetDusanLottoServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            DusnServiceInfo result = null;
            List<DusnServiceInfo> result_list = new List<DusnServiceInfo>();
            lines = Add2Log(lines, " GetDusanLottoServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetDusanLottoServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetDusanLottoServiceInfo Cache contains Info", 100, "");
                    result_list = (List<DusnServiceInfo>)HttpContext.Current.Application["GetDusanLottoServiceInfo"];
                }
                else
                {
                    // lines = Add2Log(lines, " GetDusanLottoServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetDusanLottoServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetDusanServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetDusanLottoServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetDusanLottoServiceInfo_expdate"] = DateTime.Now.AddHours(5);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetDusanLottoServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetDusanLottoServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetDusanServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    int filter_service = service.service_id;

                    if (filter_service == 888)
                    {
                        filter_service = 777;
                    }

                    result = result_list.Find(x => x.service_id == filter_service);
                    if (result != null)
                    {
                        if (Convert.ToDateTime(result.exp_datetime) < DateTime.Now)
                        {
                            string token_id = CommonFuncations.DusanLotto.GetToken(service, ref lines);
                            if (!String.IsNullOrEmpty(token_id))
                            {
                                string mydate = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
                                result.token_id = token_id;
                                result.exp_datetime = mydate;
                                Api.DataLayer.DBQueries.ExecuteQuery("update lotto_service_configuration set token_id = '" + token_id + "', exp_date = '" + mydate + "' where service_id = " + service.service_id, ref lines);
                            }
                        }
                    }

                }
            }
            return result;
        }

        public static SayThanksServiceInfo GetSayThanksServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            SayThanksServiceInfo result = null;
            List<SayThanksServiceInfo> result_list = new List<SayThanksServiceInfo>();
            lines = Add2Log(lines, " GetSayThanksServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetSayThanksServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetSayThanksServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetSayThanksServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetSayThanksServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<SayThanksServiceInfo>)HttpContext.Current.Application["GetSayThanksServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetSayThanksServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetSayThanksServiceInfo(ref lines);
                            
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetSayThanksServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetSayThanksServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetSayThanksServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetSayThanksServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetSayThanksServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetSayThanksServiceInfo"] = result_list;
                       
                        HttpContext.Current.Application["GetSayThanksServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetSayThanksServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetSayThanksServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetSayThanksServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                    if (result != null)
                    {
                        if (Convert.ToDateTime(result.bearer_token_expiration_date) < DateTime.Now)
                        {
                            string bearer = CommonFuncations.SayThanks.GetSayThanksBearerToken(service, ref lines);
                            if (!String.IsNullOrEmpty(bearer))
                            {
                                string mydate = DateTime.Now.AddMinutes(45).ToString("yyyy-MM-dd HH:mm:ss");
                                result.bearer_token = bearer;
                                result.bearer_token_expiration_date = mydate;
                                Api.DataLayer.DBQueries.ExecuteQuery("update saythanks_service_configuration set bearer_token = '" + bearer + "', bearer_token_expiration_date = '" + mydate + "' where service_id = " + service.service_id, ref lines);
                            }
                        }
                    }

                }
            }
            return result;
        }

        public static BereloServiceInfo GetBereloServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            List<BereloServiceInfo> result_list = null;
            BereloServiceInfo result = null;
            lines = Add2Log(lines, " GetBereloServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetBereloServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetBereloServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetBereloServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetBereloServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<BereloServiceInfo>)HttpContext.Current.Application["GetBereloServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetBereloServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetBereloServiceInfo(ref lines);

                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetBereloServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetBereloServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetBereloServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetBereloServiceInfo Cache ", 100, "");
                    result_list =  DBQueries.GetBereloServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetBereloServiceInfo"] = result_list;

                        HttpContext.Current.Application["GetBereloServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetBereloServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetBereloServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetBereloServiceInfo(ref lines);
            }
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);

                }
            }
            return result;
        }
        public static MADAPIServiceInfo GetMADAPIServiceInfo(ServiceClass service, ref List<LogLines> lines)
        {
            MADAPIServiceInfo result = null;
            List<MADAPIServiceInfo> result_list = new List<MADAPIServiceInfo>();
            lines = Add2Log(lines, " GetMADAPIServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMADAPIServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetMADAPIServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMADAPIServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMADAPIServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<MADAPIServiceInfo>)HttpContext.Current.Application["GetMADAPIServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetMADAPIServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetMADAPIServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetMADAPIServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetMADAPIServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetMADAPIServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMADAPIServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetMADAPIServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetMADAPIServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetMADAPIServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMADAPIServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMADAPIServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetMADAPIServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service.service_id);
                    if (result != null)
                    {
                        if (Convert.ToDateTime(result.access_token_validdate) < DateTime.Now)
                        {
                            string access_token = CommonFuncations.MADAPI.GetBearerToken(service, ref lines);
                            if (!String.IsNullOrEmpty(access_token))
                            {
                                string mydate = DateTime.Now.AddMinutes(45).ToString("yyyy-MM-dd HH:mm:ss");
                                result.access_token = access_token;
                                result.access_token_validdate = mydate;
                                Api.DataLayer.DBQueries.ExecuteQuery("update service_madapi_configuration set access_token = '" + access_token + "', access_token_validdate = '" + mydate + "' where service_id = " + service.service_id, ref lines);
                            }
                        }
                    }

                }
            }
            return result;
        }

        public static List<PisiMobileServiceInfo> GetPisiMobileServiceInfo(ref List<LogLines> lines)
        {
            List<PisiMobileServiceInfo> result_list = new List<PisiMobileServiceInfo>();
            lines = Add2Log(lines, " GetPisiMobileServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetPisiMobileServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetPisiMobileServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetPisiMobileServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetPisiMobileServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PisiMobileServiceInfo>)HttpContext.Current.Application["GetPisiMobileServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetPisiMobileServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetPisiMobileServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetPisiMobileServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetPisiMobileServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetPisiMobileServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetPisiMobileServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetPisiMobileServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetPisiMobileServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetPisiMobileServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetPisiMobileServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetPisiMobileServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetPisiMobileServiceInfo(ref lines);
            }

            return result_list;
        }


        public static List<AirTelCGServiceInfo> GetAirTelCGServiceInfo(ref List<LogLines> lines)
        {
            List<AirTelCGServiceInfo> result_list = new List<AirTelCGServiceInfo>();
            lines = Add2Log(lines, " GetAirTelCGServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetAirTelCGServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetAirTelCGServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetAirTelCGServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetAirTelCGServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<AirTelCGServiceInfo>)HttpContext.Current.Application["GetAirTelCGServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetAirTelCGServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetAirTelServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetAirTelCGServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetAirTelCGServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetAirTelCGServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetAirTelCGServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetAirTelServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetAirTelCGServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetAirTelCGServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetAirTelCGServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetAirTelCGServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetAirTelServiceInfo(ref lines);
            }

            return result_list;
        }



        public static List<PisiMobileNewServiceInfo> GetPisiMobileNewServiceInfo(ref List<LogLines> lines)
        {
            List<PisiMobileNewServiceInfo> result_list = new List<PisiMobileNewServiceInfo>();
            lines = Add2Log(lines, " GetPisiMobileNewServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetPisiMobileNewServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetPisiMobileNewServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetPisiMobileNewServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetPisiMobileNewServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<PisiMobileNewServiceInfo>)HttpContext.Current.Application["GetPisiMobileNewServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetPisiMobileNewServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetPisiMobileNewServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetPisiMobileNewServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetPisiMobileNewServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetPisiMobileNewServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetPisiMobileNewServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetPisiMobileNewServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetPisiMobileNewServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetPisiMobileNewServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetPisiMobileNewServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetPisiMobileNewServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetPisiMobileNewServiceInfo(ref lines);
            }

            return result_list;
        }

        public static string GetPisiMobileNewServiceTokenID(int service_id, string password, bool renew, ref List<LogLines> lines)
        {
            string token_id = "";
            if (renew)
            {
                lines = Add2Log(lines, " GetPisiMobileNewServiceTokenID_" + service_id + " FORCE RENEW", 100, "");
                token_id = Api.CommonFuncations.PISIMobileNew.Login(service_id, password, ref lines);
                if (!String.IsNullOrEmpty(token_id))
                {
                    HttpContext.Current.Application["GetPisiMobileNewServiceTokenID_" + service_id] = token_id;
                }
                return token_id;

            }
            lines = Add2Log(lines, " GetPisiMobileNewServiceTokenID_" + service_id, 100, "");
            try
            {
                if (HttpContext.Current.Application["GetPisiMobileNewServiceTokenID_" + service_id] != null)
                {
                    lines = Add2Log(lines, " GetPisiMobileNewServiceTokenID_"+service_id+" Cache contains Info", 100, "");
                    token_id = (string)HttpContext.Current.Application["GetPisiMobileNewServiceTokenID_" + service_id];
                }
                else
                {
                    // lines = Add2Log(lines, " GetPisiMobileNewServiceTokenID_"+service_id+" Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetPisiMobileNewServiceTokenID_"+service_id+" Cache ", 100, "");
                    token_id = Api.CommonFuncations.PISIMobileNew.Login(service_id, password, ref lines);
                    if (!String.IsNullOrEmpty(token_id))
                    {
                        HttpContext.Current.Application["GetPisiMobileNewServiceTokenID_" + service_id] = token_id;
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetPisiMobileNewServiceTokenID_"+service_id+" Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetPisiMobileNewServiceTokenID_"+service_id+" From DB ", 100, "");
                token_id = Api.CommonFuncations.PISIMobileNew.Login(service_id, password, ref lines);
            }

            return token_id;
        }

        public static ECWServiceInfo GetECWServiceInfo(int service_id, ref List<LogLines> lines)
        {
            ECWServiceInfo result = null;
            List<ECWServiceInfo> result_list = new List<ECWServiceInfo>();
            lines = Add2Log(lines, " GetECWServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetECWServiceInfo"] != null)
                {
                    lines = Add2Log(lines, " GetECWServiceInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetECWServiceInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetECWServiceInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ECWServiceInfo>)HttpContext.Current.Application["GetECWServiceInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing GetECWServiceInfo Cache ", 100, "");
                            result_list = DBQueries.GetECWServiceInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetECWServiceInfo"] = result_list;
                                HttpContext.Current.Application["GetECWServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " GetECWServiceInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetECWServiceInfo Cache ", 100, "");
                    result_list = DBQueries.GetECWServiceInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetECWServiceInfo"] = result_list;
                        HttpContext.Current.Application["GetECWServiceInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetECWServiceInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetECWServiceInfo From DB ", 100, "");
                result_list = DBQueries.GetECWServiceInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);

                }
            }
            return result;
        }

        public static FulfillmentInfo GetFulfillmentInfo(int service_id, ref List<LogLines> lines)
        {
            FulfillmentInfo result = null;
            List<FulfillmentInfo> result_list = new List<FulfillmentInfo>();
            lines = Add2Log(lines, " GetFulfillmentInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["FulfillmentInfo"] != null)
                {
                    lines = Add2Log(lines, " FulfillmentInfo Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["FulfillmentInfo_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["FulfillmentInfo_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<FulfillmentInfo>)HttpContext.Current.Application["FulfillmentInfo"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing FulfillmentInfo Cache ", 100, "");
                            result_list = DBQueries.GetFulfillmentInfo(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["FulfillmentInfo"] = result_list;
                                HttpContext.Current.Application["FulfillmentInfo_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " FulfillmentInfo Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing FulfillmentInfo Cache ", 100, "");
                    result_list = DBQueries.GetFulfillmentInfo(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["FulfillmentInfo"] = result_list;
                        HttpContext.Current.Application["FulfillmentInfo_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application FulfillmentInfo Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing FulfillmentInfo From DB ", 100, "");
                result_list = DBQueries.GetFulfillmentInfo(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);

                }
            }
            return result;
        }

        public static ServiceURLS GetServiceURLS(int service_id, ref List<LogLines> lines)
        {
            ServiceURLS result = null;
            List<ServiceURLS> result_list = new List<ServiceURLS>();
            lines = Add2Log(lines, " GetServiceURLS()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceURLSList"] != null)
                {
                    lines = Add2Log(lines, " ServiceURLSList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceURLSList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceURLSList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceURLS>)HttpContext.Current.Application["ServiceURLSList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceURLSList Cache ", 100, "");
                            result_list = DBQueries.GetServiceURLS(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceURLSList"] = result_list;
                                HttpContext.Current.Application["ServiceURLSList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceURLSList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceURLSList Cache ", 100, "");
                    result_list = DBQueries.GetServiceURLS(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceURLSList"] = result_list;
                        HttpContext.Current.Application["ServiceURLSList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceURLSList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceURLSList From DB ", 100, "");
                result_list = DBQueries.GetServiceURLS(ref lines);
            }
            
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);
                    
                }
            }
            return result;
        }

        public static IMIServiceClass GetIMIServiceID(string svcid, ref List<LogLines> lines)
        {
            IMIServiceClass result = null;
            List<IMIServiceClass> result_list = new List<IMIServiceClass>();
            lines = Add2Log(lines, " GetIMIServiceID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["IMIServiceList"] != null)
                {
                    lines = Add2Log(lines, " IMIServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["IMIServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["IMIServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<IMIServiceClass>)HttpContext.Current.Application["IMIServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetIMIServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["IMIServiceList"] = result_list;
                                HttpContext.Current.Application["IMIServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetIMIServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["IMIServiceList"] = result_list;
                        HttpContext.Current.Application["IMIServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application IMIServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from the DB ", 100, "");
                result_list = DBQueries.GetIMIServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.svcid == svcid);
                }
            }

            return result;
        }

        public static IMIServiceClass GetIMIService(string service_id, ref List<LogLines> lines)
        {
            IMIServiceClass result = null;
            List<IMIServiceClass> result_list = new List<IMIServiceClass>();
            lines = Add2Log(lines, " GetIMIServiceID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["IMIServiceList"] != null)
                {
                    lines = Add2Log(lines, " IMIServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["IMIServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["IMIServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<IMIServiceClass>)HttpContext.Current.Application["IMIServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetIMIServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["IMIServiceList"] = result_list;
                                HttpContext.Current.Application["IMIServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetIMIServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["IMIServiceList"] = result_list;
                        HttpContext.Current.Application["IMIServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application IMIServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from the DB ", 100, "");
                result_list = DBQueries.GetIMIServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id.ToString() == service_id);
                }
            }

            return result;
        }


        public static ServiceClass GetServiceInfo(string spid, string real_service_id, string product_id, ref List<LogLines> lines)
        {
            ServiceClass result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from the DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines);
            }
            
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x=> x.spid == spid && x.real_service_id == real_service_id && x.product_id == product_id);
                    if (product_id == "")
                    {
                        result = result_list.FindLast(x => x.spid == spid && x.real_service_id == real_service_id);
                    }
                    if (real_service_id == "")
                    {
                        result = result_list.FindLast(x => x.spid == spid);
                    }
                    if (spid == "000")
                    {
                        result = result_list.Find(x => x.real_service_id == real_service_id && x.product_id == product_id);
                    }
                }
            }

            return result;
        }

        public class MSISDNPrefixList
        {
            public int prefix_id { get; set; }
            public int prefix { get; set; }
            public int service_id { get; set; }
            public string token_id { get; set; }

        }
        public static int GetServiceByMSISDNPrefix(string msisdn, string orig_service_id, out string token_id, ref List<LogLines> lines)
        {
            token_id = "";
            int service_id = 0;
            string real_service_id = orig_service_id;
            switch (orig_service_id)
            {
                case "777":
                    orig_service_id = "716";
                    break;
                case "888":
                    orig_service_id = "732";
                    break;
            }
            List<MSISDNPrefixList> result_list = new List<MSISDNPrefixList>();
            if (orig_service_id == "726" || orig_service_id == "730")
            {
                lines = Add2Log(lines, " GetServiceByMSISDNPrefix()", 100, "");
                try
                {
                    if (HttpContext.Current.Application["GetServiceByMSISDNPrefix"] != null)
                    {
                        lines = Add2Log(lines, " GetServiceByMSISDNPrefix Cache contains Info", 100, "");
                        if (HttpContext.Current.Application["GetServiceByMSISDNPrefix_expdate"] != null)
                        {
                            DateTime expdate = (DateTime)HttpContext.Current.Application["GetServiceByMSISDNPrefix_expdate"];
                            lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                            if (DateTime.Now < expdate)
                            {
                                result_list = (List<MSISDNPrefixList>)HttpContext.Current.Application["GetServiceByMSISDNPrefix"];
                            }
                            else
                            {
                                lines = Add2Log(lines, " Renewing GetServiceByMSISDNPrefix Cache ", 100, "");
                                result_list = DBQueries.GetMSISDNPrefixList(ref lines);
                                if (result_list != null)
                                {
                                    HttpContext.Current.Application["GetServiceByMSISDNPrefix"] = result_list;
                                    HttpContext.Current.Application["GetServiceByMSISDNPrefix_expdate"] = DateTime.Now.AddHours(10);
                                }

                            }
                        }

                    }
                    else
                    {
                        // lines = Add2Log(lines, " GetServiceByMSISDNPrefix Cache does not contain Info", 100, "");
                        lines = Add2Log(lines, " Renewing GetServiceByMSISDNPrefix Cache ", 100, "");
                        result_list = DBQueries.GetMSISDNPrefixList(ref lines);
                        if (result_list != null)
                        {
                            HttpContext.Current.Application["GetServiceByMSISDNPrefix"] = result_list;
                            HttpContext.Current.Application["GetServiceByMSISDNPrefix_expdate"] = DateTime.Now.AddHours(10);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetServiceByMSISDNPrefix Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetServiceByMSISDNPrefix from the DB ", 100, "");
                    result_list = DBQueries.GetMSISDNPrefixList(ref lines);
                }
            }
            
            

            if (result_list != null)
            {
                if (result_list.Count() > 0 && ((orig_service_id == "726" || orig_service_id == "730") || (orig_service_id == "716" || orig_service_id == "732")))
                {
                    var service_q = result_list.Find(x => msisdn.Substring(0,5) == x.prefix.ToString());

                    if (service_q != null)
                    {
                        service_id = service_q.service_id;
                        token_id = service_q.token_id;
                    }
                }

                if (result_list.Count() > 0 && (orig_service_id == "720" || orig_service_id == "755"))
                {
                    var service_q = result_list.Find(x => msisdn.Substring(0, 6) == x.prefix.ToString());
                    if (service_q != null)
                    {
                        service_id = service_q.service_id;
                        token_id = service_q.token_id;
                    }
                }
            }
            

            return service_id;
        }

        public static ServiceClass GetVodacomService(string service_key, ref List<LogLines> lines)
        {
            ServiceClass result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from the DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.spid_password == service_key);
                }
            }

            return result;
        }

        public static List<ServiceClass> GetSimilarServices(int service_id, ref List<LogLines> lines)
        {
            List<ServiceClass> result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    ServiceClass result1 = result_list.Find(x => x.service_id == service_id);
                    if (result1 != null)
                    {
                        result = result_list.Where(x => x.real_service_id == result1.real_service_id).ToList();
                    }
                }
            }

            return result;
        }

        public static ServiceClass GetServiceByServiceID(int service_id, ref List<LogLines> lines)
        {
            ServiceClass result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    // lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                // lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines);
            }
            
            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);
                }
            }

            return result;
        }

        public static ServiceClass GetServiceByServiceID(int service_id, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            ServiceClass result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    //logMessages.Add(new { message = "ServiceList Cache contains Info", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "Services.GetServiceByServiceID", logz_id = logz_id });
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        //logMessages.Add(new { message = "expdate = " + expdate, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "Services.GetServiceByServiceID" });
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            logMessages.Add(new { message = "Renewing ServiceList Cache = " + expdate, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "Services.GetServiceByServiceID", logz_id = logz_id });
                            result_list = DBQueries.GetServices(ref lines, ref logMessages, app_name);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    logMessages.Add(new { message = "ServiceList Cache does not contain Info - Renewing", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, method = "Services.GetServiceByServiceID", logz_id = logz_id });
                    lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines, ref logMessages, app_name);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                logMessages.Add(new { message = "Exception on HttpContext.Current.Application ServiceList Cache does not contain Info - Renewing", application = app_name, environment = "production", level = "WARNING", timestamp = DateTime.UtcNow, method = "Services.GetServiceByServiceID", logz_id = logz_id });
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines, ref logMessages, app_name);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);
                }
            }

            return result;
        }

        public static MADAPISubPlan GetMADAPISubPlanByServiceID(int service_id, ref List<LogLines> lines)
        {
            MADAPISubPlan result = null;
            List<MADAPISubPlan> result_list = new List<MADAPISubPlan>();
            lines = Add2Log(lines, " GetMADAPISubPlanByServiceID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] != null)
                {
                    lines = Add2Log(lines, " GetMADAPISubPlanByServiceID Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<MADAPISubPlan>)HttpContext.Current.Application["GetMADAPISubPlanByServiceID"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetMADAPISubPlanService(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] = result_list;
                                HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMADAPISubPlanByServiceID Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMADAPISubPlanByServiceID Cache ", 100, "");
                    result_list = DBQueries.GetMADAPISubPlanService(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] = result_list;
                        HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMADAPISubPlanByServiceID Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMADAPISubPlanByServiceID from DB ", 100, "");
                result_list = DBQueries.GetMADAPISubPlanService(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id);
                }
            }

            return result;
        }

        public static List<MADAPISubPlan> GetMADAPISubPlan(ref List<LogLines> lines)
        {
            List<MADAPISubPlan> result_list = null;
            lines = Add2Log(lines, " GetMADAPISubPlanByServiceID()", 100, "");
            try
            {
                if (HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] != null)
                {
                    lines = Add2Log(lines, " GetMADAPISubPlanByServiceID Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<MADAPISubPlan>)HttpContext.Current.Application["GetMADAPISubPlanByServiceID"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetMADAPISubPlanService(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] = result_list;
                                HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " GetMADAPISubPlanByServiceID Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing GetMADAPISubPlanByServiceID Cache ", 100, "");
                    result_list = DBQueries.GetMADAPISubPlanService(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["GetMADAPISubPlanByServiceID"] = result_list;
                        HttpContext.Current.Application["GetMADAPISubPlanByServiceID_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application GetMADAPISubPlanByServiceID Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing GetMADAPISubPlanByServiceID from DB ", 100, "");
                result_list = DBQueries.GetMADAPISubPlanService(ref lines);
            }
            return result_list;
        }

        public static bool ReloadServices(ref List<LogLines> lines)
        {

            bool result = false;
            List<ServiceClass> result_list = new List<ServiceClass>();
            result_list = DBQueries.GetServices(ref lines);
            if (result_list != null)
            {
                HttpContext.Current.Application["ServiceList"] = result_list;
                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                result = true;
            }
            return result;

            

            
        }

        public static ServiceClass GetServiceByServiceIDAndAmount(int service_id, int amount, ref List<LogLines> lines)
        {
            ServiceClass result = null;
            List<ServiceClass> result_list = new List<ServiceClass>();
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = DBQueries.GetServices(ref lines);
            }

            if (result_list != null)
            {
                if (result_list.Count() > 0)
                {
                    result = result_list.Find(x => x.service_id == service_id && x.airtimr_amount == amount);
                }
            }

            return result;
        }

        public static List<ServiceClass> GetServiceList(ref List<LogLines> lines)
        {

            List<ServiceClass> result_list = null;
            lines = Add2Log(lines, " GetServiceInfo()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ServiceList"] != null)
                {
                    lines = Add2Log(lines, " ServiceList Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ServiceList_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ServiceList_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = new List<ServiceClass>();
                            result_list = (List<ServiceClass>)HttpContext.Current.Application["ServiceList"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                            result_list = new List<ServiceClass>();
                            result_list = DBQueries.GetServices(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ServiceList"] = result_list;
                                HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " ServiceList Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ServiceList Cache ", 100, "");
                    result_list = new List<ServiceClass>();
                    result_list = DBQueries.GetServices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ServiceList"] = result_list;
                        HttpContext.Current.Application["ServiceList_expdate"] = DateTime.Now.AddHours(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application ServiceList Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = new List<ServiceClass>();
                result_list = DBQueries.GetServices(ref lines);
            }

            return result_list;
        }

        public static DLDYAValidateAccount ValidateDYARequestLight(DYACheckAccountRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            DLDYAValidateAccount result = null;
            lines = Add2Log(lines, " ValidateDYARequestLight()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ValidateDYARequestLight"] != null)
                {
                    lines = Add2Log(lines, " ValidateDYARequestLight Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ValidateDYARequestLight_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ValidateDYARequestLight_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result = new DLDYAValidateAccount();
                            result = (DLDYAValidateAccount)HttpContext.Current.Application["ValidateDYARequestLight"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ValidateDYARequestLight Cache ", 100, "");
                            result = new DLDYAValidateAccount();
                            result = DBQueries.ValidateDYARequest(RequestBody, ref lines, ref logMessages, app_name, logz_id);
                            if (result != null)
                            {
                                HttpContext.Current.Application["ValidateDYARequestLight"] = result; ;
                                HttpContext.Current.Application["ValidateDYARequestLight_expdate"] = DateTime.Now.AddMinutes(2);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " ValidateDYARequestLight Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ValidateDYARequestLight Cache ", 100, "");
                    result = new DLDYAValidateAccount();
                    result = DBQueries.ValidateDYARequest(RequestBody, ref lines, ref logMessages, app_name, logz_id);
                    if (result != null)
                    {
                        HttpContext.Current.Application["ValidateDYARequestLight"] = result;
                        HttpContext.Current.Application["ValidateDYARequestLight_expdate"] = DateTime.Now.AddMinutes(2);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application ValidateDYARequestLight Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ValidateDYARequestLight from DB ", 100, "");
                result = new DLDYAValidateAccount();
                result = DBQueries.ValidateDYARequest(RequestBody, ref lines, ref logMessages, app_name, logz_id);
            }
            return result;
        }

        public static DLValidateSMS ValidateSMSRequestLight(SendSMSRequest RequestBody, ref List<LogLines> lines)
        {

            List<DLValidateSMS> result_list = null;
            DLValidateSMS result = null;
            lines = Add2Log(lines, " ValidateSMSRequestLight()", 100, "");
            try
            {
                if (HttpContext.Current.Application["ValidateSMSRequestLight"] != null)
                {
                    lines = Add2Log(lines, " ValidateSMSRequestLight Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["ValidateSMSRequestLight_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["ValidateSMSRequestLight_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            result_list = new List<DLValidateSMS>();
                            result_list = (List<DLValidateSMS>)HttpContext.Current.Application["ValidateSMSRequestLight"];
                        }
                        else
                        {
                            lines = Add2Log(lines, " Renewing ValidateSMSRequestLight Cache ", 100, "");
                            result_list = new List<DLValidateSMS>();
                            result_list = DBQueries.ValidateSMSRequestList(ref lines);
                            if (result_list != null)
                            {
                                HttpContext.Current.Application["ValidateSMSRequestLight"] = result_list;
                                HttpContext.Current.Application["ValidateSMSRequestLight_expdate"] = DateTime.Now.AddMinutes(10);
                            }

                        }
                    }

                }
                else
                {
                    lines = Add2Log(lines, " ValidateSMSRequestLight Cache does not contain Info", 100, "");
                    lines = Add2Log(lines, " Renewing ValidateSMSRequestLight Cache ", 100, "");
                    result_list = new List<DLValidateSMS>();
                    result_list = DBQueries.ValidateSMSRequestList(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["ValidateSMSRequestLight"] = result_list;
                        HttpContext.Current.Application["ValidateSMSRequestLight_expdate"] = DateTime.Now.AddMinutes(10);
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception on HttpContext.Current.Application ValidateSMSRequestLight Cache does not contain Info", 100, "");
                lines = Add2Log(lines, " Renewing ServiceList from DB ", 100, "");
                result_list = new List<DLValidateSMS>();
                result_list = DBQueries.ValidateSMSRequestList(ref lines);
            }
            int ret_code = 5000;
            string description = "Data was not found";
            if (result_list != null)
            {
                result = result_list.Find(x => x.service_id == RequestBody.ServiceID);
                if (result != null)
                {
                    if (result.Token != RequestBody.TokenID)
                    {
                        ret_code = 2001;
                        description = "Invalid Token";
                    }
                    if (result.TokenExperation == "0")
                    {
                        ret_code = 2002;
                        description = "Token Expired";
                    }
                    else
                    {
                        DateTime token_exp = Convert.ToDateTime(result.TokenExperation);
                        if (DateTime.Now > token_exp)
                        {
                            ret_code = 2002;
                            description = "Token Expired";
                        }
                    }

                    if (ret_code == 5000)
                    {
                        ret_code = 1000;
                        description = "User was Validated";
                    }

                    result.RetCode = ret_code;
                    result.Description = description;

                }
            }
            return result;
        }
    }
}