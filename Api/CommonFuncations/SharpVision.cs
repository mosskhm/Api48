using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class SharpVision
    {

        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1030,
                Description = "Failed.",
                TransactionID = dya_id,
                Timestamp = ""
            };

            string payment_provider = "TestPayment";
            switch (service.operator_id)
            {
                case 12:
                    payment_provider = "MTNBenin";
                    break;
                case 14:
                    payment_provider = "MoovBenin";
                    break;
                case 31:
                    payment_provider = "Celtiis";
                    break;
                case 7:
                    payment_provider = "MomoMTN";
                    break;
                case 15:
                    payment_provider = "OrangeB2B";
                    break;
            }

            string url = Cache.ServerSettings.GetServerSettings("SharpVision_URL_" +RequestBody.ServiceID, ref lines) + "payment/deposit";
            string client_id = Cache.ServerSettings.GetServerSettings("SharpVision_ClientID_" + RequestBody.ServiceID, ref lines);
            string secret_id = Cache.ServerSettings.GetServerSettings("SharpVision_SecretKey_" + RequestBody.ServiceID, ref lines);

            string soap = "{";
            soap = soap + "\"paymentProvider\": \""+ payment_provider + "\",";
            soap = soap + "\"requestRef\": \""+ dya_id + "\",";
            soap = soap + "\"amount\": " +RequestBody.Amount+ ",";
            soap = soap + "\"customerRef\": \"+"+RequestBody.MSISDN+"\",";
            soap = soap + "\"redirectUrl\": \"string\"";
            soap = soap + "}";

            string api_key = EncryptTimeStamp(secret_id, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "HR-Client-Id", value = client_id });
            headers.Add(new Headers { key = "HR-Api-Key", value = api_key });
            string response = CommonFuncations.CallSoap.CallSoapRequest(url, soap, headers, 4, ref lines);
            
            if (response != "")
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    string status = json_response.status;
                    string transactionid = json_response.operationRef;
                    // { "action":{ "display":{ "validationType":"Done","value":"","firstTimeDeposit":false,"id":0} },"operationRef":"17163941860609001432","requestRef":"178806713","dateTime":"2024-05-22T16:09:46.0608423Z","type":"Withdrawal","status":"Initiated","customerRef":"+22940109329","amount":190.0,"comment":"Process service http successfully."}
                    if (status == "Initiated")
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 1010,
                            Description = "Pending.",
                            TransactionID = dya_id,
                            Timestamp = datetime
                        };
                        Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1010", status, ref lines);
                        if (!String.IsNullOrEmpty(transactionid))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);
                        }
                    }
                    else
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 1050,
                            Description = "Failed - " + status,
                            TransactionID = dya_id,
                            Timestamp = datetime
                        };
                        Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1050", status, ref lines);
                        if (!String.IsNullOrEmpty(transactionid))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);
                        }
                    }

                }
                catch(Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "DYACheckAccount");
                }

                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
            }

            return ret;
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

            string payment_provider = "TestPayment";
            switch (service.operator_id)
            {
                case 12:
                    payment_provider = "MTNBenin";
                    break;
                case 14:
                    payment_provider = "MoovBenin";
                    break;
                case 31:
                    payment_provider = "Celtiis";
                    break;
                case 7:
                    payment_provider = "MomoMTN";
                    break;
                case 15:
                    payment_provider = "OrangeB2B";
                    break;
            }
            string url = Cache.ServerSettings.GetServerSettings("SharpVision_URL_" + RequestBody.ServiceID, ref lines) + "payment/withdrawal";
            string client_id = Cache.ServerSettings.GetServerSettings("SharpVision_ClientID_" + RequestBody.ServiceID, ref lines);
            string secret_id = Cache.ServerSettings.GetServerSettings("SharpVision_SecretKey_" + RequestBody.ServiceID, ref lines);

            string soap = "{";
            soap = soap + "\"paymentProvider\": \""+ payment_provider + "\",";
            soap = soap + "\"requestRef\": \"" + dya_id + "\",";
            soap = soap + "\"amount\": " + RequestBody.Amount + ",";
            soap = soap + "\"customerRef\": \"+" + RequestBody.MSISDN + "\"";
            soap = soap + "}";

            string api_key = EncryptTimeStamp(secret_id, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "HR-Client-Id", value = client_id });
            headers.Add(new Headers { key = "HR-Api-Key", value = api_key });
            string response = CommonFuncations.CallSoap.CallSoapRequest(url, soap, headers, 4, ref lines);

            if (response != "")
            {
                try
                {
                    dynamic json_response = JsonConvert.DeserializeObject(response);
                    string status = json_response.status;
                    string transactionid = json_response.operationRef;
                    switch (status)
                    {
                        case "Initiated":
                            DYACheckTransactionRequest ct_request = new DYACheckTransactionRequest()
                            {
                                MSISDN = RequestBody.MSISDN,
                                ServiceID = RequestBody.ServiceID,
                                TokenID = RequestBody.TokenID,
                                TransactionID = dya_id
                            };
                            bool result_found = false;
                            DYACheckTransactionResponse ct_response = null;
                            for (int i = 0; i <= 20; i++)
                            {
                                ct_response = CheckTranaction(ct_request, service, ref lines);
                                if (ct_response.ResultCode == 1010)
                                {
                                    Thread.Sleep(4500);
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
                    case "Validated":
                        {
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 1000,
                                Description = "Validated",
                                TransactionID = dya_id,
                                Timestamp = datetime
                            };
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", status, ref lines);
                            if (!String.IsNullOrEmpty(transactionid))
                            {
                                Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);
                            }
                        }
                        break;       
                        default:
                        break;
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Exception = " + ex.ToString(), 100, "DYACheckAccount");
                }

                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
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

            string url = Cache.ServerSettings.GetServerSettings("SharpVision_URL_" + RequestBody.ServiceID, ref lines);
            string client_id = Cache.ServerSettings.GetServerSettings("SharpVision_ClientID_" + RequestBody.ServiceID, ref lines);
            string secret_id = Cache.ServerSettings.GetServerSettings("SharpVision_SecretKey_" + RequestBody.ServiceID, ref lines);


            

            Int64 method_id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.dya_method FROM dya_transactions d WHERE d.dya_id = " + RequestBody.TransactionID, ref lines);
            string external_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select d.external_id from dya_transaction_external_id d where d.dya_id = " + RequestBody.TransactionID, ref lines);

            switch (method_id)
            {
                case 1:
                    url = url + "operations/withdrawal/status";
                    break;
                case 2:
                    url = url + "operations/deposit/status";
                    break;
            }
            url = url + "?operationRef=" + external_id + "&requestRef=" + RequestBody.TransactionID;
            string api_key = EncryptTimeStamp(secret_id, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "HR-Client-Id", value = client_id });
            headers.Add(new Headers { key = "HR-Api-Key", value = api_key });
            string body = CallSoap.GetURL(url, headers, "applicaton/json", ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    string status = json_response.status;
                    switch (status)

                    {
                        case "Validated":
                            ret = new DYACheckTransactionResponse()
                            {
                                ResultCode = 1000,
                                Description = "Success"
                            };
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "01", status, ref lines);
                            break;

                        case "Initiated":
                            ret = new DYACheckTransactionResponse()
                            {
                                ResultCode = 1010,
                                Description = status
                            };
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1010", status, ref lines);
                            break;
                        case "Cancelled":
                            ret = new DYACheckTransactionResponse()
                            {
                                ResultCode = 1030,
                                Description = "DYA Request has failed - " + status

                            };
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1030", status, ref lines);
                            break;
                        default:
                            ret = new DYACheckTransactionResponse()
                            {
                                ResultCode = 1010,
                                Description = status
                            };
                            DBQueries.UpdateDYATrans(Convert.ToInt64(RequestBody.TransactionID), "1050", "Request has failed " + status, ref lines);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }
            return ret;
        }


        public static string EncryptTimeStamp(string secretKey, string time_sec)
        {
            var key = "";

            try
            {
                var sign = time_sec; //DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                byte[] iv;
                byte[] encrypted;
                byte[] secret = Convert.FromBase64String(secretKey);

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = secret;

                    aesAlg.GenerateIV();
                    iv = aesAlg.IV;

                    aesAlg.Mode = CipherMode.CBC;

                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(sign);
                            }

                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                var ivAndData = new byte[iv.Length + encrypted.Length];
                Array.Copy(iv, 0, ivAndData, 0, iv.Length);
                Array.Copy(encrypted, 0, ivAndData, iv.Length, encrypted.Length);

                key = Convert.ToBase64String(ivAndData);
            }
            catch(Exception ex)
            {

            }
            

            return key;
        }
    }
}