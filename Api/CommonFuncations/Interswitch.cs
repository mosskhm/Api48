using Antlr.Runtime;
using Api.Cache;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Services.Description;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Interswitch
    {
        public static BankTransferResponse TransferToBank(BankTransferRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            BankTransferResponse ret = new BankTransferResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };
            List<Headers> headers = new List<Headers>();
            string password = Api.Cache.ServerSettings.GetServerSettings("PisiInterswitchpass", ref lines);
            string sid = Api.Cache.ServerSettings.GetServerSettings("PisiInterswitchsid", ref lines);
            string url_login = Api.Cache.ServerSettings.GetServerSettings("PisiInterswitchsloginurl", ref lines);
            string json_login = "{\"ServiceID\":" + Convert.ToInt32(sid) + ",\"Password\":\"" + password + "\"}";
            string response_login = CallSoap.CallSoapRequest(url_login, json_login, headers, 4, ref lines);

            if (!String.IsNullOrEmpty(response_login))
            {
                dynamic json_response_login = JsonConvert.DeserializeObject(response_login);
                try
                {
                    string TokenID = json_response_login.TokenID;
                    if (!String.IsNullOrEmpty(TokenID))
                    {
                        string interswitch_pisi_transfer_url = Api.Cache.ServerSettings.GetServerSettings("PisiInterswitchsbanktransferurl", ref lines);
                        headers = new List<Headers>();
                        //headers.Add(new Headers { key = "Authorization", value = "Bearer " + flutterwave_service.bearear });

                        //string json = "{\"amount\": "+RequestBody.Amount+",\"currency\": \""+flutterwave_service.currency+"\",\"phone_number\": \""+RequestBody.MSISDN+"\",\"email\": \""+ RequestBody.MSISDN + "@flw.com\",\"tx_ref\": \""+dya_id+"\",\"country\": \""+flutterwave_service.country+"\"}";
                        string my_msisdn = RequestBody.MSISDN.ToString().Substring(3);


                        //string json = "{\"account_bank\": \"" + RequestBody.BankID + "\",\"account_number\": \"" + RequestBody.AccountNumber + "\",\"amount\": " + RequestBody.Amount + ",\"narration\": \"" + RequestBody.FullName + " " + RequestBody.Amount + "\",\"currency\": \"" + flutterwave_service.currency + "\",\"reference\": \"" + dya_id + "\",\"callback_url\": \"" + flutterwave_service.callback_url + "\",\"debit_currency\": \"" + flutterwave_service.currency + "\"}";
                        //string json = "{\"account_bank\": \"FMM\",\"account_number\": \"" + my_msisdn + "\",\"amount\": " + RequestBody.Amount + ",\"narration\": \"" + service.service_name + "\",\"currency\": \"" + flutterwave_service.currency + "\",\"beneficiary_name\": \"" + service.service_name + "\",\"reference\": \"" + dya_id + "\",\"debit_currency\": \"" + flutterwave_service.currency + "\"}";
                        string json = "{\"MSISDN\":" + my_msisdn + ",\"BankID\":\"" + RequestBody.BankID + "\",\"AccountNumber\":\"" + RequestBody.AccountNumber + "\",\"FullName\":\"" + RequestBody.FullName + "\",\"ServiceID\":" + sid + ",\"TokenID\":\"" + TokenID + "\",\"Amount\":" + RequestBody.Amount + ",\"TransactionID\":\"" + RequestBody.TransactionID + "\"}";
                        string body = CallSoap.CallSoapRequest(interswitch_pisi_transfer_url, json, headers, 4, ref lines);
                        if (!String.IsNullOrEmpty(body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {
                                string status = json_response.ResultCode;
                                string message = json_response.Description;
                                string external_id = json_response.TransactionID;
                                if (!String.IsNullOrEmpty(external_id))
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + "," + external_id + ") ", ref lines);

                                }
                                if (!String.IsNullOrEmpty(message))
                                {
                                    ret = new BankTransferResponse()
                                    {
                                        ResultCode = Convert.ToInt32(status),
                                        Description = message,
                                        TransactionID = dya_id.ToString(),
                                        Timestamp = datetime
                                    };
                                }
                                //DYACheckTransactionRequest ct_request = new DYACheckTransactionRequest()
                                //{
                                //    MSISDN = RequestBody.MSISDN,
                                //    ServiceID = RequestBody.ServiceID,
                                //    TokenID = RequestBody.TokenID,
                                //    TransactionID = dya_id
                                //};
                                //bool result_found = false;
                                //DYACheckTransactionResponse ct_response = null;
                                //for (int i = 0; i <= 16; i++)
                                //{
                                //    ct_response = CheckTransaction(ct_request, service, ref lines);
                                //    if (ct_response.ResultCode == 1015)
                                //    {
                                //        Thread.Sleep(2500);
                                //    }
                                //    else
                                //    {
                                //        result_found = true;
                                //        break;
                                //    }
                                //}
                                //if (!result_found)
                                //{

                                //}
                                //else
                                //{
                                //    ret = new BankTransferResponse()
                                //    {
                                //        ResultCode = ct_response.ResultCode,
                                //        Description = ct_response.Description,
                                //        TransactionID = dya_id.ToString(),
                                //        Timestamp = datetime
                                //    };

                                //}
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }
            return ret;
        }
    }
}
