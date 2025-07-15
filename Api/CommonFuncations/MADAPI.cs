using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class MADAPI
    {
        public static string GetBearerToken(ServiceClass service, ref List<LogLines> lines)
        {
            string bearer = "";
            List<MADAPIServiceInfo> result_list = Api.DataLayer.DBQueries.GetMADAPIServiceInfo(ref lines);
            if (result_list != null)
            {
                MADAPIServiceInfo madapi_service = result_list.Find(x => x.service_id == service.service_id);
                if (madapi_service != null)
                {
                    List<Headers> headers = new List<Headers>();
                    string b_url = (service.operator_id == 27 ? "https://api.mtn.com/" : madapi_service.base_url);
                    string body = CallSoap.CallSoapRequest(b_url + "v1/oauth/access_token?grant_type=client_credentials", "client_id="+madapi_service.consumer_key+ "&client_secret=" + madapi_service.consumer_secret, headers, 3, ref lines);
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

        public static bool SendSMS(SendSMSRequest RequestBody, ServiceClass service, Int64 msg_id, ref List<LogLines> lines)
        {
            bool result = false;
            MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(service, ref lines);
            ServiceClass real_service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
            string sender_address = (RequestBody.ServiceID == 733 ? "YellowBet" : (RequestBody.ServiceID == 1101 ? "YellowBet":real_service.sms_mt_code));
            if (madapi_service != null)
            {
                string mytext = RequestBody.Text = RequestBody.Text.Replace(Environment.NewLine, " ").Replace("\n", " ");
                string json = "{ \"senderAddress\": \""+ sender_address + "\", \"receiverAddress\": [ \""+ RequestBody.MSISDN + "\" ], \"message\": \""+ mytext + "\", \"clientCorrelator\": \""+ msg_id + "\" }";
                string url = madapi_service.base_url + "v2/messages/sms/outbound";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                headers.Add(new Headers { key = "x-api-key", value = madapi_service.consumer_key });
                
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string requestId = json_response.data.requestId;
                        string statusCode = json_response.statusCode;
                        if (statusCode == "0000" & !String.IsNullOrEmpty(requestId))
                        {
                            if (RequestBody.TransactionID != null)
                            {
                                //Api.DataLayer.DBQueries.ExecuteQuery("insert into orange_dlr_sms (id, callback_id) values(" + RequestBody.TransactionID + ",'" + id + "');", ref lines);
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

        public static bool SendSMSV3(SendSMSRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            bool result = false;
            MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(service, ref lines);
            ServiceClass real_service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
            string sender_address = service.sms_mt_code;
            if (madapi_service != null)
            {
                string mytext = RequestBody.Text = RequestBody.Text.Replace(Environment.NewLine, " ").Replace("\n", " ");
                string json = "{\"clientCorrelatorId\": \""+RequestBody.TransactionID+"\", \"message\": \"" + mytext + "\", \"receiverAddress\": [ \"" + RequestBody.MSISDN + "\" ], \"senderAddress\": \"" + sender_address + "\", \"serviceCode\": \"" + sender_address + "\", \"keyword\": \"STOP\", \"requestDeliveryReceipt\": true}";
                //string json = "{ \"senderAddress\": \"" + sender_address + "\", \"receiverAddress\": [ \"" + RequestBody.MSISDN + "\" ], \"message\": \"" + mytext + "\", \"clientCorrelator\": \"string\" }";
                string url = madapi_service.base_url + "v3/sms/messages/sms/outbound";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "X-API-KEY", value = madapi_service.consumer_key });

                string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string requestId = json_response.data.requestId;
                        string statusCode = json_response.statusCode;
                        if (statusCode == "0000" & !String.IsNullOrEmpty(requestId))
                        {
                            if (RequestBody.TransactionID != null)
                            {
                                //Api.DataLayer.DBQueries.ExecuteQuery("insert into orange_dlr_sms (id, callback_id) values(" + RequestBody.TransactionID + ",'" + id + "');", ref lines);
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

        public static BillResponse Bill(BillRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            BillResponse ret = new BillResponse()
            {
                ResultCode = 1030,
                Description = "Billing Request failed"
            };

            List<PriceClass> price_info = GetPrices(ref lines);
            PriceClass p = price_info.Find(x => x.price_id == RequestBody.PriceID);

            string registrationChannel = "", subscriptionProviderId = "", nodeId = "";
            switch (service.operator_id)
            {
                case 3:
                    registrationChannel = "MADAPI";
                    subscriptionProviderId = "SM6D";
                    nodeId = "129";
                    break;
                case 4:
                    registrationChannel = "API";
                    subscriptionProviderId = "CSM";
                    break;
            }
            MADAPISubPlan madapi_sub_plan = GetMADAPISubPlanByServiceID(RequestBody.ServiceID, ref lines);
            if (madapi_sub_plan != null)
            {
                ServiceClass root_service = GetServiceByServiceID(madapi_sub_plan.root_service_id, ref lines);
                MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(root_service, ref lines);
                if (madapi_service != null)
                {
                    Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                    if (trans_id > 0)
                    {
                        string json = "{ \"registrationChannel\": \"" + registrationChannel + "\", \"subscriptionId\": \"" + madapi_sub_plan.plan + "\", \"subscriptionProviderId\": \"" + subscriptionProviderId + "\" " + (String.IsNullOrEmpty(nodeId) ? "" : ",\"nodeId\": " + nodeId + " ") + ", \"amountCharged\": "+p.price+".00}";
                        string url = madapi_service.base_url + "v2/customers/" + RequestBody.MSISDN + "/subscriptions";
                        List<Headers> headers = new List<Headers>();
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                        headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                        string error = "";
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                        lines = Add2Log(lines, "error = " + error, 100, "Subscribe");
                        
                        
                        if (!String.IsNullOrEmpty(body) && String.IsNullOrEmpty(error))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {

                                string statusCode = json_response.statusCode;
                                int ResultCode = 1030;
                                string Description = "Subscription Request failed";

                                if (statusCode == "0000")
                                {
                                    ResultCode = 1010;
                                    Description = "Pending";
                                }
                                else
                                {
                                    ResultCode = 1020;
                                    Description = "Billing failed with the following error " + statusCode;
                                }

                                ret = new BillResponse()
                                {
                                    ResultCode = ResultCode,
                                    Description = Description
                                };
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                            }

                        }
                        if (!String.IsNullOrEmpty(error))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(error);
                            try
                            {

                                string statusCode = json_response.error;
                                int ResultCode = 1030;
                                string Description = "Subscription Request failed";

                                if (statusCode == "0000")
                                {
                                    ResultCode = 1010;
                                    Description = "Pending";
                                }
                                else
                                {
                                    ResultCode = 1020;
                                    Description = "Billing failed with the following error " + statusCode;
                                }

                                ret = new BillResponse()
                                {
                                    ResultCode = ResultCode,
                                    Description = Description
                                };
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                            }
                        }
                        

                    }

                }
            }

            return ret;
        }

        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            string registrationChannel = "", subscriptionProviderId = "", nodeId = "";
            List<Headers> headers = new List<Headers>();
            MADAPISubPlan madapi_sub_plan = GetMADAPISubPlanByServiceID(RequestBody.ServiceID, ref lines);

            
            
            if (madapi_sub_plan != null)
            {
                ServiceClass root_service = GetServiceByServiceID(madapi_sub_plan.root_service_id, ref lines);
                MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(root_service, ref lines);
                if (madapi_service != null)
                {
                    Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN+","+RequestBody.ServiceID+",now(),'"+RequestBody.TransactionID+"')", ref lines);
                    if (trans_id > 0)
                    {
                        switch (service.operator_id)
                        {
                            case 23:
                                registrationChannel = "API";
                                subscriptionProviderId = "CSM";
                                nodeId = (madapi_sub_plan.node_id == "0" ? "YellowDot Africa Nigeria Limited" : madapi_sub_plan.node_id);
                                headers.Add(new Headers { key = "x-api-key", value = madapi_service.consumer_key });
                                headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                                registrationChannel = (String.IsNullOrEmpty(RequestBody.ActivationID) ? registrationChannel : RequestBody.ActivationID);
                                break;
                            case 3:
                                registrationChannel = "MADAPI";
                                subscriptionProviderId = "SM6D";
                                nodeId = "129";
                                headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                                headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                                break;
                            case 4:
                                registrationChannel = "API";
                                subscriptionProviderId = "CSM";
                                headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                                headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                                break;
                            case 27:
                                registrationChannel = "USSD";
                                subscriptionProviderId = "SM6D";
                                nodeId = "211";
                                headers.Add(new Headers { key = "x-api-key", value = madapi_service.consumer_key });
                                headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                                headers.Add(new Headers { key = "x-country-code", value = "GHA" });

                                // Check if the number is on DND using the existing database method
                                try
                                {
                                    bool isOnDND = DBQueries.CheckDNDStatus(RequestBody.MSISDN, service.operator_id, ref lines);
                                    if (isOnDND)
                                    {
                                        ret = new SubscribeResponse
                                        {
                                            ResultCode = 1025,
                                            Description = "Subscription blocked: Number is on DND"
                                        };
                                        return ret;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lines = Add2Log(lines, $"Error in DND check: {ex.Message}", 100, lines[0].ControlerName);
                                    ret = new SubscribeResponse
                                    {
                                        ResultCode = 1031,
                                        Description = "Unable to verify DND status. Subscription blocked as a precaution."
                                    };
                                    return ret;
                                }
                                break;
                        }

                        string json = "{ \"registrationChannel\": \"" + registrationChannel + "\", \"subscriptionId\": \"" + madapi_sub_plan.plan + "\", \"subscriptionProviderId\": \"" + subscriptionProviderId + "\" " + (String.IsNullOrEmpty(nodeId) ? "" : ",\"nodeId\": \"" + nodeId + "\" ") + "}";
                        if (service.operator_id == 23)
                        {
                            json = "{ \"registrationChannel\": \"" + registrationChannel + "\", \"subscriptionId\": \"" + service.real_service_id + "\", \"subscriptionProviderId\": \"" + subscriptionProviderId + "\" " + (String.IsNullOrEmpty(nodeId) ? "" : ",\"nodeId\": \"" + nodeId + "\" ") + ",\"subscriptionDescription\":\"" + madapi_sub_plan.plan + "\"}";
                        }
                        

                        string url = madapi_service.base_url + "v2/customers/" + RequestBody.MSISDN + "/subscriptions";
                        string error = "";
                  
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4,out error, ref lines);
                        lines = Add2Log(lines, "error = " + error, 100, "Subscribe");
                        if (error.Contains("You have Already Subscribed Requested Services") || error.Contains("Subscription already exists"))
                        {
                            //insert to sub
                            Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + RequestBody.MSISDN + "," + service.service_id + ",now(),1)", ref lines);
                            if (sub_id > 0)
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "User was subscribed"
                                };
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(body))
                            {
                                dynamic json_response = JsonConvert.DeserializeObject(body);
                                try
                                {

                                    string statusCode = json_response.statusCode;
                                    int ResultCode = 1030;
                                    string Description = "Subscription Request failed";

                                    if (statusCode == "0000")
                                    {
                                        ResultCode = 1010;
                                        Description = "Pending";
                                    }
                                    else
                                    {
                                        ResultCode = 1020;
                                        Description = "Subscription failed with the following error " + statusCode;
                                    }

                                    ret = new SubscribeResponse()
                                    {
                                        ResultCode = ResultCode,
                                        Description = Description
                                    };
                                }
                                catch (Exception ex)
                                {
                                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                                }

                            }
                        }
                        
                    }
                    
                }
            }
            

            

            return ret;
        }

        public static SubscribeResponse SubscribeV3(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            string registrationChannel = "", subscriptionProviderId = "", nodeId = "";
            switch (service.operator_id)
            {
                case 3:
                    registrationChannel = "MADAPI";
                    subscriptionProviderId = "SM6D";
                    nodeId = "129";
                    break;
                case 4:
                    registrationChannel = "API";
                    subscriptionProviderId = "CSM";
                    break;
                case 23:
                    registrationChannel = "API";
                    subscriptionProviderId = "CSM";
                    nodeId = "YellowDot Africa Nigeria Limited";
                    registrationChannel = (String.IsNullOrEmpty(RequestBody.ActivationID) ? registrationChannel : RequestBody.ActivationID);
                    break;
            }
            MADAPISubPlan madapi_sub_plan = GetMADAPISubPlanByServiceID(RequestBody.ServiceID, ref lines);
            if (madapi_sub_plan != null)
            {
                ServiceClass root_service = GetServiceByServiceID(madapi_sub_plan.root_service_id, ref lines);
                MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(root_service, ref lines);
                if (madapi_service != null)
                {
                    Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                    if (trans_id > 0)
                    {
                        string json = "{ \"registrationChannel\": \"" + registrationChannel + "\", \"subscriptionId\": \"" + madapi_sub_plan.plan + "\", \"subscriptionProviderId\": \"" + subscriptionProviderId + "\" " + (String.IsNullOrEmpty(nodeId) ? "" : ",\"nodeId\": " + nodeId + " ") + "}";
                        if (service.operator_id == 23)
                        {
                            json = "{ \"registrationChannel\": \"" + registrationChannel + "\", \"subscriptionId\": \"" + service.real_service_id + "\", \"subscriptionProviderId\": \"" + subscriptionProviderId + "\" " + (String.IsNullOrEmpty(nodeId) ? "" : ",\"nodeId\": \"" + nodeId + "\" ") + ",\"subscriptionDescription\":\"" + madapi_sub_plan.plan + "\"}";
                        }
                        string url = madapi_service.base_url + "v2/customers/" + RequestBody.MSISDN + "/subscriptions";
                        List<Headers> headers = new List<Headers>();
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                        headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                        string error = "";
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                        lines = Add2Log(lines, "error = " + error, 100, "Subscribe");
                        if (error.Contains("You have Already Subscribed Requested Services") || error.Contains("Subscription already exists"))
                        {
                            //insert to sub
                            Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + RequestBody.MSISDN + "," + service.service_id + ",now(),1)", ref lines);
                            if (sub_id > 0)
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "User was subscribed"
                                };
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(body))
                            {
                                dynamic json_response = JsonConvert.DeserializeObject(body);
                                try
                                {

                                    string statusCode = json_response.statusCode;
                                    int ResultCode = 1030;
                                    string Description = "Subscription Request failed";

                                    if (statusCode == "0000")
                                    {
                                        ResultCode = 1010;
                                        Description = "Pending";
                                    }
                                    else
                                    {
                                        ResultCode = 1020;
                                        Description = "Subscription failed with the following error " + statusCode;
                                    }

                                    ret = new SubscribeResponse()
                                    {
                                        ResultCode = ResultCode,
                                        Description = Description
                                    };
                                }
                                catch (Exception ex)
                                {
                                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                                }

                            }
                        }

                    }

                }
            }




            return ret;
        }

        public static SubscribeResponse UnSubscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "UnSubscription Request failed"
            };
            List<Headers> headers = new List<Headers>();
            MADAPISubPlan madapi_sub_plan = GetMADAPISubPlanByServiceID(RequestBody.ServiceID, ref lines);
            ServiceClass root_service = GetServiceByServiceID(madapi_sub_plan.root_service_id, ref lines);
            ServiceClass orig_service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
            MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(root_service, ref lines);
            
            string registrationChannel = "", subscriptionProviderId = "", nodeId = "";
            Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
            string url = madapi_service.base_url + "v2/customers/" + RequestBody.MSISDN + "/subscriptions/" + madapi_sub_plan.plan;
            switch (service.operator_id)
            {
                case 3:
                    registrationChannel = "MADAPI";
                    subscriptionProviderId = "SM6D";
                    nodeId = "129";
                    headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                    headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                    url = url + "?nodeId=" + nodeId + "&subscriptionProviderId=" + subscriptionProviderId + "&registrationChannel=" + registrationChannel;

                    break;
                case 4:
                    registrationChannel = "API";
                    subscriptionProviderId = "CSM";
                    headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                    headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                    headers.Add(new Headers { key = "x-country-code", value = "237" });
                    url = url + "?subscriptionProviderId=" + subscriptionProviderId;

                    break;
                case 23:
                    registrationChannel = "API";
                    subscriptionProviderId = "CSM";
                    nodeId = (madapi_sub_plan.node_id == "0" ? "YellowDot Africa Nigeria Limited" : madapi_sub_plan.node_id);
                    headers.Add(new Headers { key = "x-api-key", value = madapi_service.consumer_key });
                    headers.Add(new Headers { key = "transactionId", value = trans_id.ToString() });
                    url = url + "?nodeId=" + nodeId + "&subscriptionProviderId=" + subscriptionProviderId + "&description=" + orig_service.product_id;
                    break;
            }

            
            if ((madapi_sub_plan != null) && (madapi_service != null) && (trans_id > 0))
            {
                string json = "";
                string error = "";
                string body = CallSoap.CallSoapRequestWithMethod(url, json, headers, 4, "DELETE", ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    try
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        string statusMessage = json_response.statusMessage;

                        string statusCode = json_response.statusCode;
                        int ResultCode = 1020;
                        string Description = "Subscription failed with the following error " + statusCode + " " + statusMessage;

                        if (statusCode == "0000" || statusCode == "5000" || statusCode == "3001" || statusCode == "1003")
                        {
                            ResultCode = 1000;
                            Description = "UnSubscription was suucess";
                        }

                        ret = new SubscribeResponse()
                        {
                            ResultCode = ResultCode,
                            Description = Description
                        };
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }

                }
            }

            return ret;
        }


        public static DYACheckAccountResponse CheckAccount(DYACheckAccountRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckAccountResponse ret = new DYACheckAccountResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"
            };
            MADAPIServiceInfo madapi_service = GetMADAPIServiceInfo(service, ref lines);
            if (madapi_service != null)
            {
                string url = madapi_service.base_url + "v1/accountholders/" + RequestBody.MSISDN + "/validate";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + madapi_service.access_token });
                headers.Add(new Headers { key = "x-api-key", value = madapi_service.consumer_key });
                headers.Add(new Headers { key = "transactionId", value = Guid.NewGuid().ToString() });
                headers.Add(new Headers { key = "X-Authorization", value = madapi_service.consumer_key });

                string body = CallSoap.GetURL(url, headers, "application/json", ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string statusCode = json_response.statusCode;
                        string customerId = json_response.customerId;
                        bool isvalid = json_response.data.isValid;
                        if (statusCode == "200" && isvalid == true)
                        {
                            ret = new DYACheckAccountResponse()
                            {
                                ResultCode = 1000,
                                Description = "User has an account."
                            };
                        }
                        else
                        {
                            ret = new DYACheckAccountResponse()
                            {
                                ResultCode = 1020,
                                Description = "User does not have an account."
                            };
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

    }
}