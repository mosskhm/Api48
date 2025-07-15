using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class MTNOpeanAPI
    {
        public class OpenAPIBearers
        {
            public string transfer_b_token { get; set; }
            public string receive_b_token { get; set; }
        }

        public static OpenAPIBearers GetBearerToken(ServiceClass service, ref List<LogLines> lines)
        {
            string bearer = "";
            OpenAPIBearers openapi_b = new OpenAPIBearers()
            {
                receive_b_token = "",
                transfer_b_token = ""
            };
            List<OpenAPIServiceInfo> result_list = Api.DataLayer.DBQueries.GetOpenAPIServiceInfo(ref lines);
            if (result_list != null)
            {
                int my_service_id = service.service_id;
                if (my_service_id == 777)
                {
                    my_service_id = 716;
                }
                if (my_service_id == 913)
                {
                    my_service_id = 733;
                }

                OpenAPIServiceInfo openapi_service = result_list.Find(x => x.service_id == my_service_id);
                if (openapi_service != null)
                {
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.receive_ocp_ask });

                    
                    string url = openapi_service.base_url + (service.service_id == 741 ? "collectionwithnewcapabilities/" : "collection/") + "token/";
                    string body = CallSoap.CallSoapRequest(url, "", headers, 3, openapi_service.receive_apiuser, openapi_service.receive_apikey, "POST", ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            bearer = json_response.access_token;
                            if (!String.IsNullOrEmpty(bearer))
                            {
                                openapi_b.receive_b_token = bearer;
                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception GetBearerToken = " + ex.ToString(), 100, lines[0].ControlerName);
                            
                        }
                    }
                    //if (!String.IsNullOrEmpty(bearer))
                    //{
                        headers = new List<Headers>();
                        headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.transfer_ocp_ask });
                        url = openapi_service.base_url + "disbursement/token/";
                        body = CallSoap.CallSoapRequest(url, "", headers, 3, openapi_service.transfer_apiuser, openapi_service.transfer_apikey, "POST", ref lines);
                        if (!String.IsNullOrEmpty(body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {
                                bearer = json_response.access_token;
                                if (!String.IsNullOrEmpty(bearer))
                                {
                                    openapi_b.transfer_b_token = bearer;
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception GetBearerToken = " + ex.ToString(), 100, lines[0].ControlerName);
                            }
                        }
                    //}
                }
            }
            return openapi_b;
        }

        public static DYATransferMoneyResponse TransferMoney(DYATransferMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            OpenAPIServiceInfo openapi_service = null;
            if (service.service_id == 777)
            {
                ServiceClass my_service = GetServiceByServiceID(716, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }
            else
            {
                openapi_service = GetOpenApiServiceInfo(service, ref lines);
            }
            if (service.service_id == 913)
            {
                ServiceClass my_service = GetServiceByServiceID(733, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }

            if (openapi_service != null)
            {
                string url = openapi_service.base_url + "disbursement/v1_0/transfer";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.transfer_b_token });
                headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.transfer_ocp_ask });
                string g = "8d1fc753-e70d-4d2e-9866-";

                for (int i = 1; i <= (12-dya_id.Length); i++)
                {
                    g = g + "0";
                }
                g = g + dya_id;
                
                headers.Add(new Headers { key = "X-Reference-Id", value = g });
                headers.Add(new Headers { key = "X-Callback-Url", value = openapi_service.callback_url });
                string json = "{\"amount\": \""+RequestBody.Amount+"\",\"currency\": \""+openapi_service.curency+"\",\"externalId\": \""+dya_id+"\",\"payee\": {\"partyIdType\": \"MSISDN\",\"partyId\": \""+RequestBody.MSISDN+"\"},\"payerMessage\": \"YellowBbet\",\"payeeNote\": \"string\"}";
                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (String.IsNullOrEmpty(body) && error == "")
                {
                    System.Threading.Thread.Sleep(4500);
                    DYACheckTransactionRequest check_trans_req = new DYACheckTransactionRequest()
                    {
                        MSISDN = RequestBody.MSISDN,
                        ServiceID = RequestBody.ServiceID,
                        TokenID = RequestBody.TokenID,
                        TransactionID = dya_id
                    };
                    DYACheckTransactionResponse check_trans_resp = CheckTranaction(check_trans_req, service, ref lines);
                    if (check_trans_resp != null)
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1010,
                            Description = "Pending",
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };

                        switch (check_trans_resp.ResultCode)
                        {
                            case 1000:
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success",
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                break;
                            case 1050:
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Failed - " + check_trans_resp.Description,
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                break;
                        }
                    }
                }
            }

            return ret;
        }

        public static DYATransferMoneyResponse TransferMoneyBetweenMOMO(DYATransferMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            OpenAPIServiceInfo openapi_service = GetOpenApiServiceInfo(service, ref lines);

            if (openapi_service != null)
            {
                string url = openapi_service.base_url + "disbursement/v1_0/transfer";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.receive_b_token });
                headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.receive_ocp_ask });
                string g = "8d1fc753-e70d-4d2e-9866-";

                for (int i = 1; i <= (12 - dya_id.Length); i++)
                {
                    g = g + "0";
                }
                g = g + dya_id;

                headers.Add(new Headers { key = "X-Reference-Id", value = g });
                //headers.Add(new Headers { key = "X-Callback-Url", value = "https://api.ydplatform.com/handlers/mtnopenapi.ashx" });
                string json = "{\"amount\": \"" + RequestBody.Amount + "\",\"currency\": \"" + openapi_service.curency + "\",\"externalId\": \"" + dya_id + "\",\"payee\": {\"partyIdType\": \"MSISDN\",\"partyId\": \"" + RequestBody.MSISDN + "\"},\"payerMessage\": \"YellowBbet\",\"payeeNote\": \"string\"}";
                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (String.IsNullOrEmpty(body) && error == "")
                {
                    System.Threading.Thread.Sleep(4500);
                    DYACheckTransactionRequest check_trans_req = new DYACheckTransactionRequest()
                    {
                        MSISDN = RequestBody.MSISDN,
                        ServiceID = RequestBody.ServiceID,
                        TokenID = RequestBody.TokenID,
                        TransactionID = dya_id
                    };
                    DYACheckTransactionResponse check_trans_resp = CheckTranaction(check_trans_req, service, ref lines);
                    if (check_trans_resp != null)
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1010,
                            Description = "Pending",
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };

                        switch (check_trans_resp.ResultCode)
                        {
                            case 1000:
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success",
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                break;
                            case 1050:
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Failed - " + check_trans_resp.Description,
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                break;
                        }
                    }
                }
            }

            return ret;
        }


        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1010,
                Description = "Pending.",
                TransactionID = dya_id,
                Timestamp = datetime
            };

            OpenAPIServiceInfo openapi_service = null;
            if (service.service_id == 777)
            {
                ServiceClass my_service = GetServiceByServiceID(716, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }
            else
            {
                openapi_service = GetOpenApiServiceInfo(service, ref lines);
            }
            if (service.service_id == 913)
            {
                ServiceClass my_service = GetServiceByServiceID(733, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }
            

            if (openapi_service != null)
            {
                string url = openapi_service.base_url + (service.service_id == 741 ? "collectionwithnewcapabilities/" : "collection/") + "v1_0/requesttopay";
                //string url = openapi_service.base_url + "collectionwithnewcapabilities/v1_0/requesttopay";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.receive_b_token });
                headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.receive_ocp_ask });
                string g = "8d1fc753-e70d-4d2e-9866-";

                for (int i = 1; i <= (12 - dya_id.Length); i++)
                {
                    g = g + "0";
                }
                g = g + dya_id;

                headers.Add(new Headers { key = "X-Reference-Id", value = g });
                headers.Add(new Headers { key = "X-Callback-Url", value = openapi_service.callback_url});
                string json = "{\"amount\": \"" + RequestBody.Amount + "\",\"currency\": \"" + openapi_service.curency + "\",\"externalId\": \"" + dya_id + "\",\"payer\": {\"partyIdType\": \"MSISDN\",\"partyId\": \"" + RequestBody.MSISDN + "\"},\"payerMessage\": \"YellowBet\",\"payeeNote\": \"string\"}";
                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (String.IsNullOrEmpty(body) && error == "")
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1010,
                        Description = "Pending.",
                        TransactionID = dya_id,
                        Timestamp = datetime
                    };
                }
                else
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1050,
                        Description = "Failed.",
                        TransactionID = dya_id,
                        Timestamp = datetime
                    };

                }
            }
            return ret;
        }


        public static DYACheckTransactionResponse CheckTranaction(DYACheckTransactionRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckTransactionResponse ret = new DYACheckTransactionResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"

            };

            OpenAPIServiceInfo openapi_service = null;
            if (service.service_id == 777)
            {
                ServiceClass my_service = GetServiceByServiceID(716, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }
            else
            {
                openapi_service = GetOpenApiServiceInfo(service, ref lines);
            }
            if (service.service_id == 913)
            {
                ServiceClass my_service = GetServiceByServiceID(733, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }

            if (openapi_service != null)
                {
                    string g = "8d1fc753-e70d-4d2e-9866-";
                    for (int i = 1; i <= (12 - RequestBody.TransactionID.Length); i++)
                    {
                        g = g + "0";
                    }
                    g = g + RequestBody.TransactionID;
                    string url = openapi_service.base_url;
                    List<Headers> headers = new List<Headers>();

                    Int64 method_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.dya_method FROM dya_transactions d WHERE d.dya_id = " + RequestBody.TransactionID, ref lines);

                    switch (method_id)
                    {
                        case 1:
                            url = url + "disbursement/v1_0/transfer/" + g;
                            headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.transfer_b_token });
                            headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.transfer_ocp_ask });
                            break;
                        case 2:
                            url = url + (service.service_id == 741 ? "collectionwithnewcapabilities/" : "collection/") + "v1_0/requesttopay/" + g;
                            //url = url + "collection/v1_0/requesttopay/" + g;
                            headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.receive_b_token });
                            headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.receive_ocp_ask });
                            break;
                    }
                    headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });

                    string body = CallSoap.GetURL(url, headers, ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string status = json_response.status;
                            if (String.IsNullOrEmpty(status))
                            {
                                status = json_response.code;
                            }
                            string reason = "";
                            string financialTransactionId = "";

                            switch (status)
                            {
                                case "RESOURCE_NOT_FOUND":
                                    reason = json_response.reason;
                                    ret = new DYACheckTransactionResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "Request has failed " + status
                                    };
                                    DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", status, ref lines);
                                    break;
                                case "FAILED":
                                    reason = json_response.reason;
                                    ret = new DYACheckTransactionResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "Request has failed " + reason
                                    };
                                    DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed", ref lines);
                                    break;
                                case "SUCCESSFUL":
                                    financialTransactionId = json_response.financialTransactionId;
                                    ret = new DYACheckTransactionResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "Success"
                                    };
                                    DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + RequestBody.TransactionID + ",'" + financialTransactionId + "');", ref lines);
                                    break;
                                case "PENDING":
                                    ret = new DYACheckTransactionResponse()
                                    {
                                        ResultCode = 1015,
                                        Description = "Pending"
                                    };
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


        public static string CheckBalance(ServiceClass service, int method_id, ref List<LogLines> lines)
        {
            string balance = "";

            OpenAPIServiceInfo openapi_service = GetOpenApiServiceInfo(service, ref lines);
            if (openapi_service != null)
            {
                string url = openapi_service.base_url;
                List<Headers> headers = new List<Headers>();

                switch (method_id)
                {
                    case 1:
                        url = url + "disbursement/v1_0/account/balance";
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.transfer_b_token });
                        headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.transfer_ocp_ask });
                        break;
                    case 2:
                        url = url + (service.service_id == 741 ? "collectionwithnewcapabilities/" : "collection/") + "v1_0/account/balance";
                        //url = url + "collection/v1_0/account/balance";
                        headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.receive_b_token });
                        headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.receive_ocp_ask });
                        break;
                }
                headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });

                string body = CallSoap.GetURL(url, headers, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        balance = json_response.availableBalance;
                        
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                }
            }
            return balance;
        }


        public static DYACheckAccountResponse CheckAccount(DYACheckAccountRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckAccountResponse ret = new DYACheckAccountResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"
            };

            OpenAPIServiceInfo openapi_service = null;
            if (service.service_id == 777)
            {
                ServiceClass my_service = GetServiceByServiceID(716, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }
            else
            {
                openapi_service = GetOpenApiServiceInfo(service, ref lines);
            }
            if (service.service_id == 913)
            {
                ServiceClass my_service = GetServiceByServiceID(733, ref lines);
                openapi_service = GetOpenApiServiceInfo(my_service, ref lines);
            }

            if (openapi_service != null)
            {
                string soap_url = openapi_service.base_url + "disbursement/v1_0/accountholder/msisdn/"+RequestBody.MSISDN+"/active";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + openapi_service.transfer_b_token });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = openapi_service.transfer_ocp_ask });
                headers.Add(new Headers { key = "X-Target-Environment", value = openapi_service.x_target_environment });
                int status_code = 0;
                string body = CallSoap.GetURL(soap_url, ref lines, out status_code, headers);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        //2020-10-20 19:48:44.901: Response = {"result":false}
                        string result = json_response.result;
                        switch (result.ToLower())
                        {
                            case "true":
                                ret = new DYACheckAccountResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "User has an account."
                                };
                                break;
                            case "false":
                                ret = new DYACheckAccountResponse()
                                {
                                    ResultCode = 1020,
                                    Description = "User does not have an account."
                                };
                                break;
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return ret;
        }
    }
}