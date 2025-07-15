using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Flutterwave
    {
        public static GetBalanceResponse GetFlutterwaveBalances(GetBalanceRequest RequestBody)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "BankTransfer");
            GetBalanceResponse ret = new GetBalanceResponse()
            {
                RetCode = 1030,
                Description = "Request has failed",
                AvailableBalance = ""
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(RequestBody.ServiceID, ref lines);
            if (flutterwave_service != null)
            {
                string url = flutterwave_service.balances_url + RequestBody.Currency;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });
                string error = "";
                string body = CallSoap.GetURLWithHeader(url,headers, ref lines);
                if (!String.IsNullOrEmpty(body))
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        int balance = json_response.data.available_balance;
                        string message = json_response.message;
                        switch (status)
                        {
                            case "error":
                                ret = new GetBalanceResponse()
                                {
                                    RetCode = 1050,
                                    Description = "Request has failed with the following error - " + message,
                                    AvailableBalance = ""

                                };
                                break;
                            case "success":
                                ret = new GetBalanceResponse()
                                {
                                    RetCode = 1000,
                                    Description = message,
                                    AvailableBalance = balance.ToString() + " " + RequestBody.Currency
                                };
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                    }
                };
                

        }
            return ret;
        }
        public static BankTransferResponse TransferMoney(BankTransferRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            BankTransferResponse ret = new BankTransferResponse()
            {
                ResultCode = 1030,
                Description = "Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);

            if (flutterwave_service != null)
            {
                string url = flutterwave_service.banktransfer_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                string json = "{";
                json = json + "    \"account_bank\": \""+RequestBody.AccountNumber+"\",";
                json = json + "    \"account_number\": \""+ RequestBody.BankID + "\",";
                json = json + "    \"amount\": "+RequestBody.Amount+",";
                json = json + "    \"narration\": \""+RequestBody.TransactionID+"\",";
                json = json + "    \"currency\": \""+flutterwave_service.currency+"\",";
                json = json + "    \"reference\": \""+dya_id+"\",";
                json = json + "    \"callback_url\": \"" + flutterwave_service.callback_url + "\",";
                json = json + "    \"debit_currency\": \"" + flutterwave_service.currency + "\"";
                json = json + "}";

                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (String.IsNullOrEmpty(body) && error == "")
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        string code = json_response.code;
                        string message = json_response.message;
                        switch (status)
                        {
                            case "error":
                                ret = new BankTransferResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Request has failed with the following error - " + code,
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", code, ref lines);
                                break;
                            case "success":
                                ret = new BankTransferResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success",
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", code, ref lines);
                                break;
                            //TODO - check if there is a pending mode
                        }
                        //TODO - extract external id and update db
                        string external_id = "";
                        if (!String.IsNullOrEmpty(external_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values("+dya_id+","+external_id+") ", ref lines);
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

        //MOMO
        public static DYATransferMoneyResponse TransferMoney(DYATransferMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);


            if (flutterwave_service != null)
            {
                string url = flutterwave_service.momo_transfer_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                //string json = "{\"amount\": "+RequestBody.Amount+",\"currency\": \""+flutterwave_service.currency+"\",\"phone_number\": \""+RequestBody.MSISDN+"\",\"email\": \""+ RequestBody.MSISDN + "@flw.com\",\"tx_ref\": \""+dya_id+"\",\"country\": \""+flutterwave_service.country+"\"}";
                string my_msisdn = RequestBody.MSISDN.ToString().Substring(3);
                string json = "{\"account_bank\": \"FMM\",\"account_number\": \""+ my_msisdn + "\",\"amount\": "+RequestBody.Amount+",\"narration\": \""+service.service_name+"\",\"currency\": \""+flutterwave_service.currency+"\",\"beneficiary_name\": \""+ service.service_name + "\",\"reference\": \""+dya_id+"\",\"debit_currency\": \""+flutterwave_service.currency+"\"}";

                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (!String.IsNullOrEmpty(body) && error == "")
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        string external_id = json_response.data.id;
                        if (!String.IsNullOrEmpty(external_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + "," + external_id + ") ", ref lines);
                        }
                        if (status == "success")
                        {
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
                                ct_response = CheckTransaction(ct_request, service, ref lines);
                                if (ct_response.ResultCode == 1015)
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

        public static BankTransferResponse TransferToBank(BankTransferRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            BankTransferResponse ret = new BankTransferResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);


            if (flutterwave_service != null)
            {
                string url = flutterwave_service.banktransfer_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                //string json = "{\"amount\": "+RequestBody.Amount+",\"currency\": \""+flutterwave_service.currency+"\",\"phone_number\": \""+RequestBody.MSISDN+"\",\"email\": \""+ RequestBody.MSISDN + "@flw.com\",\"tx_ref\": \""+dya_id+"\",\"country\": \""+flutterwave_service.country+"\"}";
                string my_msisdn = RequestBody.MSISDN.ToString().Substring(3);
                string json = "{\"account_bank\": \""+RequestBody.BankID+"\",\"account_number\": \""+RequestBody.AccountNumber+"\",\"amount\": "+RequestBody.Amount+",\"narration\": \""+RequestBody.FullName+" "+RequestBody.Amount+"\",\"currency\": \""+flutterwave_service.currency+"\",\"reference\": \""+dya_id+"\",\"callback_url\": \""+flutterwave_service.callback_url+"\",\"debit_currency\": \""+flutterwave_service.currency+"\"}";
                //string json = "{\"account_bank\": \"FMM\",\"account_number\": \"" + my_msisdn + "\",\"amount\": " + RequestBody.Amount + ",\"narration\": \"" + service.service_name + "\",\"currency\": \"" + flutterwave_service.currency + "\",\"beneficiary_name\": \"" + service.service_name + "\",\"reference\": \"" + dya_id + "\",\"debit_currency\": \"" + flutterwave_service.currency + "\"}";

                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (!String.IsNullOrEmpty(body) && error == "")
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        string message = json_response.message;
                        string external_id = json_response.data.id;
                        if (!String.IsNullOrEmpty(external_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + "," + external_id + ") ", ref lines);
                            
                        }
                        if (status == "success")
                        {
                            if (message == "Transfer Queued Successfully")
                            {
                                ret = new BankTransferResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending",
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
                                };
                            }
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
                                ct_response = CheckTransaction(ct_request, service, ref lines);
                                if (ct_response.ResultCode == 1015)
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
                                
                            }
                            else
                            {
                                ret = new BankTransferResponse()
                                {
                                    ResultCode = ct_response.ResultCode,
                                    Description = ct_response.Description,
                                    TransactionID = dya_id.ToString(),
                                    Timestamp = datetime
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

            return ret;
        }

        public static ValidateBankAccountResponse ValidateBankAccount(ValidateBankAccountRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            ValidateBankAccountResponse ret = new ValidateBankAccountResponse()
            {
                ResultCode = 1030,
                Description = "Request has failed - Network problem",
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);

            if (flutterwave_service != null)
            {
                string url = "https://api.flutterwave.com/v3/accounts/resolve"; //flutterwave_service.momo_transfer_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                //string json = "{\"amount\": "+RequestBody.Amount+",\"currency\": \""+flutterwave_service.currency+"\",\"phone_number\": \""+RequestBody.MSISDN+"\",\"email\": \""+ RequestBody.MSISDN + "@flw.com\",\"tx_ref\": \""+dya_id+"\",\"country\": \""+flutterwave_service.country+"\"}";
                string json = "{\"account_bank\": \"" + RequestBody.BankID + "\",\"account_number\": \"" + RequestBody.AccountNumber + "\"}";
                //string json = "{\"account_bank\": \"FMM\",\"account_number\": \"" + my_msisdn + "\",\"amount\": " + RequestBody.Amount + ",\"narration\": \"" + service.service_name + "\",\"currency\": \"" + flutterwave_service.currency + "\",\"beneficiary_name\": \"" + service.service_name + "\",\"reference\": \"" + dya_id + "\",\"debit_currency\": \"" + flutterwave_service.currency + "\"}";

                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (!String.IsNullOrEmpty(body) && error == "")
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        string message = json_response.message;
                        
                        if (!String.IsNullOrEmpty(status))
                        {
                            if (status == "success")
                            {
                                string account_holder_name = json_response.data.account_name;
                                ret = new ValidateBankAccountResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success",
                                    AccountHolderName = account_holder_name
                                };
                            }
                            else
                            {
                                ret = new ValidateBankAccountResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Request has failed with the following error " + message
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

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);


            if (flutterwave_service != null)
            {
                string url = flutterwave_service.momo_receive_url;
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                string json = "{\"amount\": "+RequestBody.Amount+",\"currency\": \""+flutterwave_service.currency+"\",\"phone_number\": \""+RequestBody.MSISDN+"\",\"email\": \""+ RequestBody.MSISDN + "@flw.com\",\"tx_ref\": \""+dya_id+"\",\"country\": \""+flutterwave_service.country+"\"}";
                
                string error = "";
                string body = CallSoap.CallSoapRequest(url, json, headers, 4, out error, ref lines);
                if (!String.IsNullOrEmpty(body) && error == "")
                {
                    dynamic json_response = JsonConvert.DeserializeObject(body);
                    try
                    {
                        string status = json_response.status;
                        string external_id = json_response.data.id;
                        string real_status = json_response.data.status;
                        if (!String.IsNullOrEmpty(external_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + "," + external_id + ") ", ref lines);
                        }
                        if (status == "success" && real_status == "pending")
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
                            string message = json_response.message;
                            ret = new DYAReceiveMoneyResponse()
                            {
                                ResultCode = 1050,
                                Description = "Failed " + message,
                                TransactionID = dya_id,
                                Timestamp = datetime
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

        public static DYACheckTransactionResponse CheckTransaction(DYACheckTransactionRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckTransactionResponse ret = new DYACheckTransactionResponse()
            {
                ResultCode = 1015,
                Description = "Pending"
            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);

            if (flutterwave_service != null)
            {
                Int64 method_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.dya_method FROM dya_transactions d WHERE d.dya_id = " + RequestBody.TransactionID, ref lines);
                string url = "";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });
                string external_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select external_id from  dya_transaction_external_id d where d.dya_id = " + RequestBody.TransactionID, ref lines);
                string response_body = "";
                if (!String.IsNullOrEmpty(external_id))
                {
                    switch (method_id)
                    {
                        case 1:
                            url = flutterwave_service.momo_validate_transfer_url + external_id;
                            break;
                        case 2:
                            url = flutterwave_service.momo_validate_receive_url + external_id + "/verify";
                            break;
                    }
                    response_body = CallSoap.GetURL(url, headers, ref lines);
                    if (!String.IsNullOrEmpty(response_body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        try
                        {
                            //2022-02-01 20:12:07.826: Body = {"status":"success","message":"Transaction fetched successfully","data":{"id":573641665,"tx_ref":"46586624","flw_ref":"RYJS8386316437384787","device_fingerprint":"N/A","amount":800,"currency":"XAF","charged_amount":800,"app_fee":20,"merchant_fee":0,"processor_response":"Transaction Successful","auth_model":"AUTH","ip":"52.18.161.235","narration":"yellowbet","status":"successful","payment_type":"mobilemoneysn","created_at":"2022-02-01T18:01:18.000Z","account_id":1402432,"meta":null,"amount_settled":780,"customer":{"id":333793854,"name":"Anonymous customer","phone_number":"237678570034","email":"237678570034@flw.com","created_at":"2022-02-01T18:01:18.000Z"}}}


                            string status = json_response.status;
                            string real_status = json_response.data.status;
                            string desc1 = json_response.data.processor_response;
                            string desc2 = json_response.data.complete_message;
                            if (status == "success")
                            {
                                switch(real_status.ToLower())
                                {
                                    case "failed":
                                        ret = new DYACheckTransactionResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "Request has failed " + desc1 + " - " + desc2
                                        };
                                        DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed " + desc1 + " - " + desc2, ref lines);
                                        break;
                                    case "new":
                                    case "pending":
                                        ret = new DYACheckTransactionResponse()
                                        {
                                            ResultCode = 1015,
                                            Description = "Pending"
                                        };
                                        break;
                                    case "successful":
                                    case "success":
                                        ret = new DYACheckTransactionResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "Success"
                                        };
                                        DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                        break;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            lines = Add2Log(lines, "Exception =" + ex.ToString(), 100, lines[0].ControlerName);
                        }
                    }
                }
                

                
            }

            return ret;
        }

        public static BankTransferResponse CheckTransaction(BankTransferRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            BankTransferResponse ret = new BankTransferResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"

            };

            FlutterwaveServiceInfo flutterwave_service = GetFlutterwaveInfo(service.service_id, ref lines);

            if (flutterwave_service != null)
            {
                Int64 method_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.dya_method FROM dya_transactions d WHERE d.dya_id = " + RequestBody.TransactionID, ref lines);
                string url = "";
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });
                string external_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select external_id from  dya_transaction_external_id d where d.dya_id = " + RequestBody.TransactionID, ref lines);
                string response_body = "";
                if (!String.IsNullOrEmpty(external_id))
                {
                    switch (method_id)
                    {
                        case 1:
                            url = flutterwave_service.momo_validate_transfer_url + external_id;
                            break;
                        case 2:
                            url = flutterwave_service.momo_validate_receive_url + external_id + "/verify";
                            break;
                    }
                    response_body = CallSoap.GetURL(url, headers, ref lines);
                    if (!String.IsNullOrEmpty(response_body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(response_body);
                        try
                        {
                            //2022-02-01 20:12:07.826: Body = {"status":"success","message":"Transaction fetched successfully","data":{"id":573641665,"tx_ref":"46586624","flw_ref":"RYJS8386316437384787","device_fingerprint":"N/A","amount":800,"currency":"XAF","charged_amount":800,"app_fee":20,"merchant_fee":0,"processor_response":"Transaction Successful","auth_model":"AUTH","ip":"52.18.161.235","narration":"yellowbet","status":"successful","payment_type":"mobilemoneysn","created_at":"2022-02-01T18:01:18.000Z","account_id":1402432,"meta":null,"amount_settled":780,"customer":{"id":333793854,"name":"Anonymous customer","phone_number":"237678570034","email":"237678570034@flw.com","created_at":"2022-02-01T18:01:18.000Z"}}}


                            string status = json_response.status;
                            string real_status = json_response.data.status;
                            string desc1 = json_response.data.processor_response;
                            string desc2 = json_response.data.complete_message;
                            if (status == "success")
                            {
                                switch (real_status.ToLower())
                                {
                                    case "failed":
                                        ret = new BankTransferResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "Request has failed " + desc1 + " - " + desc2
                                        };
                                        DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed " + desc1 + " - " + desc2, ref lines);
                                        break;
                                    case "new":
                                    case "pending":
                                        ret = new BankTransferResponse()
                                        {
                                            ResultCode = 1015,
                                            Description = "Pending"
                                        };
                                        break;
                                    case "successful":
                                    case "success":
                                        ret = new BankTransferResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "Success"
                                        };
                                        DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", "Success", ref lines);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, "Exception =" + ex.ToString(), 100, lines[0].ControlerName);
                        }
                    }
                }



            }

            return ret;
        }
    }
}