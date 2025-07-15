using Api.Cache;
using Api.DataLayer;
using Api.HttpItems;
using Api.Logger;
using Api.MADAPI;
using Microsoft.Web.Services2.Addressing;
using Mysqlx.Crud;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Orange
    {
        // Gets OAuth bearer tokens for API access
        public static string GetBearerToken(ServiceClass service, ref List<LogLines> lines)
        {
            string bearer = "";
            List<OrangeServiceInfo> result_list = Api.DataLayer.DBQueries.GetOrangeServiceInfo(ref lines);
            if (result_list != null)
            {
                OrangeServiceInfo orange_service = result_list.Find(x => x.service_id == service.service_id);
                if (orange_service != null)
                {
                    List<Headers> headers = new List<Headers>();
                    string body = CallSoap.CallSoapRequest(orange_service.token_url, "grant_type=client_credentials", headers, 3, orange_service.client_id, orange_service.client_secret, "POST", ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
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

        // Gets service-specific tokens for Orange Money operations
        public static string GetOMRServiceToken(ServiceClass service, OrangeServiceInfo orange_service, ref List<LogLines> lines)
        {
            string token = "";
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });


            string body = CallSoap.GetURL(orange_service.base_url + "/services", headers, ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    token = json_response.token.value;
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }

            return token;
        }

        public class OMRFormsToken
        {
            public string token { get; set; }
            public string endpoint { get; set; }
            public string version { get; set; }
        }

        // Gets form tokens for cash-in/cash-out operations
        public static OMRFormsToken GetOMRFormsToken(ServiceClass service, OrangeServiceInfo orange_service, string xomrservice_token, ref List<LogLines> lines)
        {
            OMRFormsToken token = null;
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
            headers.Add(new Headers { key = "x-omr-services-token", value = xomrservice_token });

            string body = CallSoap.GetURL(orange_service.base_url + "/forms/cashin", headers, "application/json; charset=utf-8", ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    token = new OMRFormsToken()
                    {
                        token = json_response.token.value,
                        version = json_response.version,
                        endpoint = json_response.endpoint
                    };

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }

            return token;
        }

        public static OMRFormsToken GetOMRFormsTokenReceive(ServiceClass service, OrangeServiceInfo orange_service, string xomrservice_token, ref List<LogLines> lines)
        {
            OMRFormsToken token = null;
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
            headers.Add(new Headers { key = "x-omr-services-token", value = xomrservice_token });

            string body = CallSoap.GetURL(orange_service.base_url + "/forms/cashout", headers, "application/json; charset=utf-8", ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    token = new OMRFormsToken()
                    {
                        token = json_response.token.value,
                        version = json_response.version,
                        endpoint = json_response.endpoint
                    };

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }

            return token;
        }



        //  Creates web payment URLs for online transactions
        public static DYAReceiveMoneyResponse GetWebPayURL(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id,
                Timestamp = datetime,
                RedirectURL = ""
            };
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service, ref lines);
            if (orange_service != null)
            {
                string not_url = "https://api.ydplatform.com/orange/webpayment_not.ashx?dya_id=" + dya_id;
                string cancel_url = "https://api.ydplatform.com/orange/cancel.ashx?dya_id=" + dya_id;
                string ret_url = "https://api.ydplatform.com/orange/ret_url.ashx?dya_id=" + dya_id;


                string json = "{\"merchant_key\": \"" + orange_service.posId + "\", \"currency\": \"" + orange_service.currency + "\", \"order_id\": \"" + dya_id + "\",\"amount\": " + RequestBody.Amount + ",\"return_url\": \"" + ret_url + "\",\"cancel_url\": \"" + cancel_url + "\", \"notif_url\": \"" + not_url + "\", \"lang\": \"fr\", \"reference\": \"" + dya_id + "\"}";
                string url = orange_service.webpay_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;

                        switch (status)
                        {
                            case "201":
                                DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1000", "Pending", ref lines);
                                string pay_token = json_response.pay_token;
                                string payment_url = json_response.payment_url;
                                string notif_token = json_response.notif_token;
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_webpay_data (dya_id, pay_token, notif_token) values(" + dya_id + ",'" + pay_token + "','" + notif_token + "')", ref lines);
                                ret = new DYAReceiveMoneyResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending",
                                    TransactionID = dya_id,
                                    Timestamp = datetime,
                                    RedirectURL = payment_url
                                };
                                break;
                            default:

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }

            }


            return ret;
        }

        // Sends money to a phone number via Orange Money
        public static DYATransferMoneyResponse TransferMoney(DYATransferMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            ServiceClass orig_service = service;
            if (service.service_id == 888)
            {
                orig_service = GetServiceByServiceID(732, ref lines);
            }

            OrangeServiceInfo orange_service = GetOrangeServiceInfo(orig_service, ref lines);

            if (orange_service != null)
            {
                string omr_service_token = GetOMRServiceToken(orig_service, orange_service, ref lines);
                if (!String.IsNullOrEmpty(omr_service_token))
                {
                    OMRFormsToken token = GetOMRFormsToken(orig_service, orange_service, omr_service_token, ref lines);
                    if (token != null)
                    {
                        string msisdn = RequestBody.MSISDN.ToString().Substring(3);
                        string json = "{\"peerId\":\"" + msisdn + "\", \"amount\": " + RequestBody.Amount + ", \"currency\":\"" + orange_service.currency + "\", \"posId\":\"" + orange_service.posId + "\",\"transactionId\":\"" + dya_id + "\"}";
                        string url = orange_service.base_url + token.endpoint;
                        List<Headers> headers = new List<Headers>();
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                        headers.Add(new Headers { key = "x-omr-forms-token", value = token.token });
                        headers.Add(new Headers { key = "x-omr-forms-version", value = token.version });
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines, ref logMessages, app_name, logz_id);
                        if (!String.IsNullOrEmpty(body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {
                                string status = json_response.status;
                                switch (status)
                                {
                                    case "PENDING":
                                        DYACheckTransactionRequest ct_request = new DYACheckTransactionRequest()
                                        {
                                            MSISDN = RequestBody.MSISDN,
                                            ServiceID = RequestBody.ServiceID,
                                            TokenID = RequestBody.TokenID,
                                            TransactionID = dya_id
                                        };
                                        bool result_found = false;
                                        DYACheckTransactionResponse ct_response = null;
                                        for (int i = 0; i <= 16; i++)
                                        {
                                            ct_response = CheckTranaction(ct_request, service, ref lines);
                                            if (ct_response.ResultCode == 1010)
                                            {
                                                Thread.Sleep(2500);
                                            }
                                            else
                                            {
                                                result_found = true;
                                                break;
                                            }
                                        }
                                        if (!result_found)
                                        {
                                            ret = new DYATransferMoneyResponse()
                                            {
                                                ResultCode = 1010,
                                                Description = "Pending",
                                                TransactionID = dya_id.ToString(),
                                                Timestamp = datetime
                                            };
                                        }
                                        else
                                        {
                                            ret = new DYATransferMoneyResponse()
                                            {
                                                ResultCode = ct_response.ResultCode,
                                                Description = ct_response.Description,
                                                TransactionID = dya_id.ToString(),
                                                Timestamp = datetime
                                            };

                                        }

                                        break;
                                    case "SUCCESS":
                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "SUCCESS",
                                            TransactionID = dya_id,
                                            Timestamp = datetime
                                        };
                                        string MySql = "update dya_transactions set result = '01', result_desc = 'Success' where dya_id = " + dya_id;
                                        Api.DataLayer.DBQueries.ExecuteQuery(MySql, ref lines, ref logMessages, app_name, logz_id);
                                        //DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                        //2021-11-08 14:19:18.733: Body = {"status":"SUCCESS","transactionData":{"transactionId":"39845083","type":"CASHIN","peerId":"625512651","peerIdType":"msisdn","amount":600,"currency":"GNF","creationDate":1636370408947,"posId":"VdKTa1h","txnId":"CI211108.1120.C33823"}}


                                        break;
                                    default:

                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "Orange.TransferMoney", logz_id = logz_id, msisdn = RequestBody.MSISDN });
                            }
                        }


                    }
                }
            }

            return ret;
        }

        //  Receives money from a phone number
        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            ServiceClass orig_service = service;
            if (service.service_id == 888)
            {
                orig_service = GetServiceByServiceID(759, ref lines);
            }
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(orig_service, ref lines);

            if (orange_service != null)
            {
                string omr_service_token = GetOMRServiceToken(orig_service, orange_service, ref lines);
                if (!String.IsNullOrEmpty(omr_service_token))
                {
                    OMRFormsToken token = GetOMRFormsTokenReceive(orig_service, orange_service, omr_service_token, ref lines);
                    if (token != null)
                    {
                        string msisdn = RequestBody.MSISDN.ToString().Substring(3);
                        string json = "{\"peerId\":\"" + msisdn + "\", \"amount\": " + RequestBody.Amount + ", \"currency\":\"" + orange_service.currency + "\", \"posId\":\"" + orange_service.posId + "\",\"transactionId\":\"" + dya_id + "\"}";
                        string url = orange_service.base_url + token.endpoint;
                        List<Headers> headers = new List<Headers>();
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                        headers.Add(new Headers { key = "x-omr-forms-token", value = token.token });
                        headers.Add(new Headers { key = "x-omr-forms-version", value = token.version });
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines, ref logMessages, app_name, logz_id);
                        if (!String.IsNullOrEmpty(body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {
                                string status = json_response.status;
                                switch (status)
                                {
                                    case "PENDING":
                                        ret = new DYAReceiveMoneyResponse()
                                        {
                                            ResultCode = 1010,
                                            Description = "Pending",
                                            TransactionID = dya_id,
                                            Timestamp = datetime
                                        };

                                        break;
                                    case "SUCCESS":
                                        ret = new DYAReceiveMoneyResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "SUCCESS",
                                            TransactionID = dya_id,
                                            Timestamp = datetime
                                        };
                                        //DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                        string MySql = "update dya_transactions set result = '01', result_desc = 'Success' where dya_id = " + dya_id;
                                        Api.DataLayer.DBQueries.ExecuteQuery(MySql, ref lines, ref logMessages, app_name, logz_id);
                                        //2021-11-08 14:19:18.733: Body = {"status":"SUCCESS","transactionData":{"transactionId":"39845083","type":"CASHIN","peerId":"625512651","peerIdType":"msisdn","amount":600,"currency":"GNF","creationDate":1636370408947,"posId":"VdKTa1h","txnId":"CI211108.1120.C33823"}}


                                        break;
                                    default:

                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, method = "Orange.ReceiveMoney", logz_id = logz_id, msisdn = RequestBody.MSISDN });
                            }
                        }


                    }
                }
            }

            return ret;
        }

        public class SMSMonitoring
        {
            public string country { get; set; }
            public int availableUnits { get; set; }
            public string expires { get; set; }
        }

        // Checks SMS balance and contract status
        public static List<SMSMonitoring> GetSMSMonitoring(ServiceClass service, ref List<LogLines> lines)
        {

            List<SMSMonitoring> result = null;

            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service, ref lines);
            if (orange_service != null)
            {
                string url = "https://api.orange.com/sms/admin/v1/contracts";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.GetURL(url, headers, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        //2022-02-02 18:19:07.631: Body = {"partnerContracts":{"partnerId":"oren@yellowbet.com","contracts":[{"service":"SMS_OCB","contractDescription":"SMS API: your balance (per country)","serviceContracts":[{"country":"CMR","service":"SMS_OCB","contractId":"82e56e1d-111a-4444-b8c7-4786e054f709","availableUnits":99,"expires":"2022-03-02T20:00:00","scDescription":"Cameroon - 99 units to consume before March 2, 2022 8:00 PM"},{"country":"GIN","service":"SMS_OCB","contractId":"cf2326a2-066a-4105-9c63-2e525b063d95","availableUnits":301634,"expires":"2022-03-27T14:00:01","scDescription":"Guinea - 301634 units to consume before March 27, 2022 2:00 PM"}]}]}}


                        string country = json_response.partnerContracts.contracts[0].serviceContracts[0].country;
                        string availableUnits = json_response.partnerContracts.contracts[0].serviceContracts[0].availableUnits;
                        string expires = json_response.partnerContracts.contracts[0].serviceContracts[0].expires;
                        SMSMonitoring r = new SMSMonitoring()
                        {
                            country = country,
                            availableUnits = Convert.ToInt32(availableUnits),
                            expires = expires.Replace("T", "")
                        };
                        result = new List<SMSMonitoring>();
                        result.Add(r);

                        country = json_response.partnerContracts.contracts[0].serviceContracts[1].country;
                        availableUnits = json_response.partnerContracts.contracts[0].serviceContracts[1].availableUnits;
                        expires = json_response.partnerContracts.contracts[0].serviceContracts[1].expires;
                        r = new SMSMonitoring()
                        {
                            country = country,
                            availableUnits = Convert.ToInt32(availableUnits),
                            expires = expires.Replace("T", "")
                        };
                        result.Add(r);

                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
            }
            return result;
        }

        public static List<SMSMonitoring> GetSMSMonitoringOrangeOnlyCM(ServiceClass service, ref List<LogLines> lines)
        {

            List<SMSMonitoring> result = null;

            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service, ref lines);
            if (orange_service != null)
            {
                string url = "https://api.orange.com/sms/admin/v1/contracts?resource_type_parameter_management=SMS_OCB2";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.GetURL(url, headers, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        //2022-02-02 18:19:07.631: Body = {"partnerContracts":{"partnerId":"oren@yellowbet.com","contracts":[{"service":"SMS_OCB","contractDescription":"SMS API: your balance (per country)","serviceContracts":[{"country":"CMR","service":"SMS_OCB","contractId":"82e56e1d-111a-4444-b8c7-4786e054f709","availableUnits":99,"expires":"2022-03-02T20:00:00","scDescription":"Cameroon - 99 units to consume before March 2, 2022 8:00 PM"},{"country":"GIN","service":"SMS_OCB","contractId":"cf2326a2-066a-4105-9c63-2e525b063d95","availableUnits":301634,"expires":"2022-03-27T14:00:01","scDescription":"Guinea - 301634 units to consume before March 27, 2022 2:00 PM"}]}]}}


                        string country = json_response.partnerContracts.contracts[0].serviceContracts[0].country;
                        string availableUnits = json_response.partnerContracts.contracts[0].serviceContracts[0].availableUnits;
                        string expires = json_response.partnerContracts.contracts[0].serviceContracts[0].expires;
                        SMSMonitoring r = new SMSMonitoring()
                        {
                            country = country,
                            availableUnits = Convert.ToInt32(availableUnits),
                            expires = expires.Replace("T", "")
                        };
                        result = new List<SMSMonitoring>();
                        result.Add(r);
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
            }
            return result;
        }

        // Checks the status of a money transfer transaction
        public static DYACheckTransactionResponse CheckTranaction(DYACheckTransactionRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckTransactionResponse ret = new DYACheckTransactionResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"

            };

            ServiceClass orig_service = service;
            if (service.service_id == 888)
            {
                orig_service = GetServiceByServiceID(732, ref lines);
            }

            OrangeServiceInfo orange_service = GetOrangeServiceInfo(orig_service, ref lines);
            if (orange_service != null)
            {
                string url = orange_service.base_url + "/transactions/" + RequestBody.TransactionID;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.GetURL(url, headers, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        try
                        {
                            string orange_trans_id = json_response.transactionData.txnId;
                            lines = Add2Log(lines, " orange_trans_id = " + orange_trans_id, 100, lines[0].ControlerName);
                            if (!String.IsNullOrEmpty(orange_trans_id))
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id,external_id) values(" + RequestBody.TransactionID + ",'" + orange_trans_id + "')", ref lines);
                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                        }

                        switch (status)
                        {
                            case "SUCCESS":
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                break;
                            case "PENDING":
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed", ref lines);
                                break;
                            case "FAILED":
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Failed"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed", ref lines);
                                break;
                            default:
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed", ref lines);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
            }
            return ret;
        }

        // Sends SMS via Orange's SMS API
        public static bool SendSMS(SendSMSRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            bool result = false;
            ServiceClass orig_service = service;
            if (service.service_id == 888)
            {
                orig_service = GetServiceByServiceID(732, ref lines);
            }

            OrangeServiceInfo orange_service = GetOrangeServiceInfo(orig_service, ref lines);
            if (orange_service != null)
            {
                string json = "{\"outboundSMSMessageRequest\": {\"address\": \"tel:+" + RequestBody.MSISDN + "\",\"senderAddress\":\"tel:+2240000\",\"outboundSMSTextMessage\": {\"message\": \"" + RequestBody.Text + "\"}}}";
                string url = orange_service.sendsms_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string id = json_response.outboundSMSMessageRequest.resourceURL;
                        if (!String.IsNullOrEmpty(id))
                        {
                            string[] words = id.Split('/');
                            id = words[words.Length - 1];
                            if (RequestBody.TransactionID != null)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into orange_dlr_sms (id, callback_id) values(" + RequestBody.TransactionID + ",'" + id + "');", ref lines);
                            }
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }

                }
            }
            return result;
        }


        //  Cameroon-specific SMS sending
        public static bool SendSMSCM(SendSMSRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            bool result = false;
            service = GetServiceByServiceID(732, ref lines);
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service, ref lines);
            if (orange_service != null)
            {
                string json = "{\"outboundSMSMessageRequest\": {\"address\": \"tel:+" + RequestBody.MSISDN + "\",\"senderAddress\":\"tel:+2370000\",\"outboundSMSTextMessage\": {\"message\": \"" + RequestBody.Text + "\"},\"senderAddress\":\"tel:+2370000\",\"senderName\":\"YellowBet\"}}";
                //https://api.orange.com/smsmessaging/v1/outbound/tel%3A%2B{{dev_phone_number}}/requests?resource_type_parameter_management=SMS_OCB2 
                string url = "https://api.orange.com/smsmessaging/v1/outbound/tel%3A%2B2370000/requests?resource_type_parameter_management=SMS_OCB2";//  orange_service.sendsms_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string id = json_response.outboundSMSMessageRequest.resourceURL;
                        if (!String.IsNullOrEmpty(id))
                        {
                            string[] words = id.Split('/');
                            id = words[words.Length - 1];
                            if (RequestBody.TransactionID != null)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into orange_dlr_sms (id, callback_id) values(" + RequestBody.TransactionID + ",'" + id + "');", ref lines);
                            }
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }

                }
            }
            return result;
        }

        //  Value-added service SMS for Cameroon
        public static bool SendSMSCMVAS(SendSMSRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            bool result = false;
            ServiceClass service1 = GetServiceByServiceID(762, ref lines);
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service1, ref lines);
            if (orange_service != null)
            {
                string mytext = RequestBody.Text;
                string json = "{\"outboundSMSMessageRequest\": {\"address\": \"acr:X-Orange-ISE2\",\"senderAddress\": \"tel:+23700000\",\"senderName\": \"" + service.sms_mt_code + "\",\"outboundSMSTextMessage\": {\"message\": \"" + mytext + "\"}}}";

                string url = orange_service.sendsms_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                headers.Add(new Headers { key = "x-orange-mco", value = "OCM" });
                headers.Add(new Headers { key = "x-orange-ise2", value = RequestBody.EncryptedMSISDN });

                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string id = json_response.outboundSMSMessageRequest.resourceURL;
                        if (!String.IsNullOrEmpty(id))
                        {
                            string[] words = id.Split('/');
                            id = words[words.Length - 1];
                            if (RequestBody.TransactionID != null)
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into orange_vas_dlr_sms (transaction_id, callback_id, enc_msisdn, service_id) values('" + RequestBody.TransactionID + "','" + id + "','" + RequestBody.EncryptedMSISDN + "', " + RequestBody.ServiceID + ");", ref lines);
                            }
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }

                }
            }
            return result;
        }

        // Subscribes users to services
        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            ServiceClass service1 = GetServiceByServiceID(762, ref lines);
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service1, ref lines);

            if (orange_service != null)
            {
                string mydate = DateTime.Now.ToString("yyyy-MM-dd");
                List<PriceClass> pricelist = GetAllPricesInfo(ref lines);

                PriceClass price = pricelist.Find(x => x.service_id == RequestBody.ServiceID);

                Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into yellowdot.orange_sub_requests (enc_msisdn, service_id, date_time, trans_id) values ('" + RequestBody.EncryptedMSISDN + "', " + service.service_id + ", now(),'" + RequestBody.TransactionID + "')", ref lines);
                if (req_id > 0)
                {
                    string json = "{\"note\":{\"text\":\"" + req_id + "\"},\"relatedPublicKey\":{\"id\":\"" + RequestBody.EncryptedMSISDN + "\",\"name\":\"ISE2\"},\"relatedParty\":[{\"id\":\"YELLOWDOT\",\"name\":\"YELLOWDOT\",\"role\":\"partner\"},{\"id\":\"YELLOWDOT\",\"name\":\"YELLOWDOT\",\"role\":\"retailer\"}],\"orderItem\":{\"action\":\"add\",\"state\":\"Completed\",\"product\":{\"id\":\"" + service.service_name + "\",\"href\":\"na\",\"productCharacteristic\":[{\"name\":\"taxAmount\",\"value\":\"0\"},{\"name\":\"amount\",\"value\":\"" + price.price + ".0\"},{\"name\":\"currency\",\"value\":\"XAF\"},{\"name\":\"periodicity\",\"value\":\"86400\"},{\"name\":\"startDate\",\"value\":\"" + mydate + "\"},{\"name\":\"country\",\"value\":\"CMR\"},{\"name\":\"language\",\"value\":\"fr\"},{\"name\":\"mode\",\"value\":\"hybrid\"}]}}}";
                    string url = "https://api.orange.com/payment/mea/v1/digipay_sub/productOrder/";
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                    string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            //"id": "6024153ef59e4e66ac180669",
                            //"state": "Completed",
                            string id = json_response.id;
                            string state = json_response.state;
                            if (!String.IsNullOrEmpty(id))
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("update yellowdot.orange_sub_requests o set orange_id1 = '" + id + "', state = '" + state + "' where id = " + req_id, ref lines);
                            }
                            if (state == "Completed")
                            {

                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Subscription Request was sent"
                                };
                            }
                            else
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1020,
                                    Description = "Subscription Request failed - " + state
                                };
                            }

                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                        }

                    }
                }



            }

            return ret;
        }


        // Unsubscribes users from services
        public static SubscribeResponse UnSubscribe(SubscribeRequest RequestBody, ServiceClass service, DLValidateSMS result, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            ServiceClass service1 = GetServiceByServiceID(762, ref lines);
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service1, ref lines);

            if (orange_service != null)
            {
                string mydate = DateTime.Now.ToString("yyyy-MM-dd");
                string orange_sub = Api.DataLayer.DBQueries.SelectQueryReturnString("select ocm_sub_id from subscribers_ocm where subscriber_id = " + result.SubscriberID, ref lines);
                string url = "https://api.orange.com/payment/mea/v1/digipay_sub/productOrder/" + orange_sub;

                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                string body = CallSoap.CallSoapRequestWithMethod(url, "", headers, 4, "DELETE", ref lines);
                if (body == "")
                {
                    ret = new SubscribeResponse()
                    {
                        ResultCode = 1000,
                        Description = "UnSubscription was successful"
                    };
                }
                else
                {

                }
            }

            return ret;
        }


        //  Bills users for services via Orange's payment API
        public static BillResponse Bill(BillRequest RequestBody, ServiceClass service, DLValidateBill result, ref List<LogLines> lines)
        {
            BillResponse ret = new BillResponse()
            {
                ResultCode = 1030,
                Description = "Request has failed"
            };

            ServiceClass service1 = GetServiceByServiceID(762, ref lines);
            OrangeServiceInfo orange_service = GetOrangeServiceInfo(service1, ref lines);


            if (orange_service != null)
            {
                string mydate = DateTime.Now.ToString("yyyyMMddHHmmss") + Guid.NewGuid().ToString();

                PriceClass price = GetPricesInfo(service.service_id, 0, ref lines);


                string amount = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT real_price FROM prices p WHERE p.service_id = " + RequestBody.ServiceID + " and p.price_id = " + RequestBody.PriceID, ref lines);
                if (String.IsNullOrEmpty(amount))
                {
                    amount = (price.price_id == RequestBody.PriceID ? price.price.ToString() : price.price.ToString());
                }

                string ocm_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select ocm_sub_id from subscribers_ocm where subscriber_id = " + result.sub_id, ref lines);
                string purchaseCategoryCode = (service.service_id == 766 ? "Sports" : "other");
                string url = "https://api.orange.com/payment/mea/v1/acr:X-Orange-ISE2/transactions/amount";
                string json = "{\"amountTransaction\":{\"endUserId\":\"acr:X-Orange-ISE2\",\"paymentAmount\":{\"chargingInformation\":{\"amount\":\"" + amount + "\",\"currency\": \"XAF\",\"description\": \"" + service.service_name + " SMS\"},\"chargingMetaData\" : {\"onBehalfOf\" : \"" + service.service_name + "\",\"purchaseCategoryCode\":\"" + purchaseCategoryCode + "\",\"serviceId\":\"YELLOWDOT\"}},\"transactionOperationStatus\":\"Charged\",\"referenceCode\":\"" + ocm_id + "\",\"clientCorrelator\": \"" + mydate + "\"}}";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + orange_service.bearer });
                headers.Add(new Headers { key = "x-orange-mco", value = "OCM" });
                headers.Add(new Headers { key = "x-orange-ise2", value = RequestBody.EncryptedMSISDN });
                string error = "";


                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string totalAmountCharged = json_response.amountTransaction.paymentAmount.totalAmountCharged;
                        if (!String.IsNullOrEmpty(totalAmountCharged))
                        {
                            if (price.price.ToString() == totalAmountCharged.Replace(".0", ""))
                            {
                                Int64 req_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into billing (subscriber_id, billing_date_time, price_id) values(" + result.sub_id + ",now(), " + price.price_id + ")", ref lines);
                                ret = new BillResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "User was billed",
                                    TransactionID = req_id.ToString()
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
                lines = Add2Log(lines, " error = " + error, 100, lines[0].ControlerName);
                if (!String.IsNullOrEmpty(error))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(error);
                    try
                    {
                        string my_error = json_response.requestError.policyException.text;
                        if (!String.IsNullOrEmpty(my_error))
                        {
                            ret = new BillResponse()
                            {
                                ResultCode = 1050,
                                Description = "Billing Failed with the following error " + my_error,
                                TransactionID = "-1"
                            };

                        }

                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                    if (error.Contains("Subscription not active"))
                    {
                        ret = new BillResponse()
                        {
                            ResultCode = 1051,
                            Description = "Billing Failed with the following error Subscription not active",
                            TransactionID = "-1"
                        };
                    }
                }
            }

            return ret;
        }

        // Validates user accounts
        public static DYACheckAccountResponse CheckAccount(DYACheckAccountRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckAccountResponse ret = new DYACheckAccountResponse()
            {
                ResultCode = 1000,
                Description = "User has an account"
            };
            return ret;
        }


        // Sends SMS via Kannel gateway (different infrastructure)
        public static bool SendSMSKannel(string msisdn, string text, bool is_flash, string msg_id, string service_id, ref List<LogLines> lines)
        {

            bool sms_sent = false;
           
            try
            {
                //msisdn = msisdn.Substring(3);
                string kannel = Cache.ServerSettings.GetServerSettings("OrangeG_SendSMSURL", ref lines);
                if (String.IsNullOrEmpty(kannel)) throw new Exception($"invalid setting for OrangeG_SendSMSURL");

                string from = "YellowBet";      // default sender
                 


                switch (Convert.ToInt32(service_id))
                {
                    case 1184:
                    case 1185:
                    case 1186:
                        // Ivory Coast - change the port to 26013
                        kannel = kannel.Replace(":22013", ":26013");
                        from = "7717";      // seems to be an access list on Orange side that blocks anything NOT from 7717
                        break;
                }

                string url = kannel
                                + "from=" + from 
                                + "&to=" + msisdn
                                + "&text=" + Uri.EscapeDataString(text)
                                + "&dlr-mask=24"
                                + (is_flash == true ? "&mclass=0" : "")
                                + "&dlr-url=http%3A%2F%2F192.168.1.2%2Fdlr%2Fkannel_dlr.ashx%3Fstatus%3D%25d%26msg_id%3D"
                                + msg_id
                                + "%26service_id%3D" + service_id
                                ;

                lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMS");
                // send request and return whether the reponse contains "delivery" 
                sms_sent = CallSoap.GetURL(url, ref lines).Contains("delivery");

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! exception: {ex.Message}", 100, "");
                lines = Add2Log(lines, ex.StackTrace, 100, "");
            }

            return sms_sent;
        }

        public static string NormalizeMsisdn(string msisdn, string dialingCountryCode, int len)
        {
            // Create pattern to match leading + or 00 followed by country code
            string pattern = $"^(\\+|00)?{Regex.Escape(dialingCountryCode)}";

            // Remove the country code from the beginning
            string stripped = Regex.Replace(msisdn, pattern, "");

            // Take the last (len - 1) digits
            string lastDigits = stripped.Length >= len - 1
                ? stripped.Substring(stripped.Length - (len - 1))
                : stripped;

            // Prepend '0'
            return "0" + lastDigits;
        }


        private static PriceClass FetchOrInsertPriceID(int serviceId, double amount, ref List<LogLines> lines)
        {
            PriceClass price_c = GetPricesInfo(serviceId, amount, ref lines);

            // if not found, insert a new price entry for this service
            if (price_c == null)
            {
                // price_id must ben an int, not an Int64, otherwise the GetPriceInfo overloads to the wrong function
                int price_id = Convert.ToInt32(
                                    DBQueries.ExecuteQueryReturnInt64(
                                        "insert into prices (service_id, price, curency_code, curency_symbol, real_price) " +
                                        "values (" + serviceId + "," + amount + ",'XAF','CAF'," + amount.ToString("F2") + ")"
                                        , ref lines
                                        )
                                    );

                if (price_id > 0)
                {
                    // update the cache
                    List<PriceClass> result_list = DBQueries.GetPrices(ref lines);
                    if (result_list != null)
                    {
                        HttpContext.Current.Application["PriceList"] = result_list;
                        HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                    }

                    // fetch our current entry
                    price_c = GetPricesInfo(serviceId, price_id, ref lines);
                }
            }

            if (price_c == null) throw new Exception($"failed to insert new price entry for serviceID={serviceId}, amount={amount}");
            return price_c;
        }



        // Special billing for Ivory Coast airtime
        public static BillResponse billIvoryCoastAirTime(string msisdn, double amount, Int64 subscriberId, int serviceId, ref List<LogLines> lines)
        {

            try
            {
                // check inputs
                if (amount <= 0) throw new Exception($"invalid amount = {amount}");
                if (serviceId <= 0) throw new Exception($"invalid serviceID = {serviceId}");
                if (subscriberId <= 0) throw new Exception($"invalid subscriberID = {subscriberId}");

                // fetch or create a priceId for this
                PriceClass price = FetchOrInsertPriceID(serviceId, amount, ref lines);

                // normalise msisdn - strip out country code
                string localMsisdn = NormalizeMsisdn(msisdn, "225", 10);

                // Orange credentials and endpoint
                const string SESSION = "a5874c9fb6da4548a35928bfbf437a6b34dd7873359c419e8c8f308f8f9492b9";
                const int ID = 170;
                string url = $"http://192.168.233.146:8181/engine-plasma/payments?action=Facturation&telephone={localMsisdn}&session={SESSION}&id={ID}&montant={amount}";

                lines = Add2Log(lines, $"Sending request to Orange billing API: {url}", 100, "");

                var content = CallSoap.GetURL(url, ref lines);
                var json = JObject.Parse(content);
                var status = json["status"]?.ToString();
                var txnId = json["txnId"]?.ToString();
                var message = json["zteDebitReturnMsg"]?.ToString();
               

                if (string.IsNullOrEmpty(status) || status != "1")
                {
                    lines = Add2Log(lines, $"!! Failed to bill msisdn = {msisdn}, amount = {amount}, status = {status}, message = {message}", 100, "");

                    /* 27 JUN 2025 - Benji requested to stop sending sms on billing faiure
                    SendSMSKannel(msisdn
                        , "Nous n'avons pas pu renouveler votre souscription a Numero d'Or car votre solde est insufisant. Veuillez recharger votre compte. Pour vous desabonner, repondez avec Stop NO (1;2;3)"
                        , false
                        , "OrgICBilling_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                        , serviceId.ToString()
                        , ref lines
                    );
                    */

                    // failed -- return reason
                    return new BillResponse()
                    {
                        ResultCode = 1050,
                        Description = "Billing Failed: " + (String.IsNullOrEmpty(message) ? "unknown" : message),
                        TransactionID = "-1"
                    };

                }
                else
                {
                    lines = Add2Log(lines, $"++ successfully billed msisdn = {msisdn}, amount = {amount}, status = {status}, transactionId = {txnId}, message={message}", 100, "");

                    // success - insert billing entry
                    Int64 req_id = DBQueries.ExecuteQueryReturnInt64(
                                    "insert into billing (subscriber_id, billing_date_time, price_id) "
                                    + "values(" + subscriberId + ",now(), " + price.price_id + ")"
                                    , ref lines);

                    lines = Add2Log(lines, $"inserted BILLING event bill_id={req_id}, for subscriberID={subscriberId}, priceID={price.price_id}", 100, "");

                    if (!SendSMSKannel(msisdn
                           , $"Vous avez été inscrit au prochain tirage de Numero d'Or après un débit réussi de {amount} F depuis votre compte. Croisez les doigts et suivez le lien pour voir le résultat du prochain tirage de Numero d'Or: http://icorgtwn.ydot.co . Bonne chance"
                           , false
                           , "OrgICBilling_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                           , serviceId.ToString()
                           , ref lines
                   )) lines = Add2Log(lines, $"failed to send welcome SMS", 100, "");


                    return new BillResponse()
                    {
                        ResultCode = 1000,
                        Description = $"Successfully billed: {amount}, ref: {txnId}",
                        TransactionID = req_id.ToString()
                    };

                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Exception in billIvoryCostAirTime: " + ex.Message, 100, "");
            }

            // if we get here we have had a problem
            return new BillResponse()
            {
                ResultCode = 1050,
                Description = "System Error",
                TransactionID = "-1"
            };
        }
    }
}