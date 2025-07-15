using Api.DataLayer;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Paga
    {
        public static string BuildTransferMoneySoap(DLDYAValidateAccount result, DYATransferMoneyRequest RequestBody, Int64 dya_trans)
        {
            string json = "{\"referenceNumber\": \"refp_"+dya_trans+"\",\"amount\": \""+RequestBody.Amount.ToString()+".0\",\"sendWithdrawalCode\":\"true\",\"destinationAccount\": \"+"+RequestBody.MSISDN+"\",\"alternateSenderName\": \"Yellowdot Africa Nigeria\"}";

//            {
//                "referenceNumber":"paga-2349038125089",
//   "amount":"3000.00",
//   "currency":"NGN",
//   "destinationAccount":"09038125089",
//   "withdrawalCode":"paga-2349038125089",
//   "sourceOfFunds":"PAGA",
//   "transferReference":"paga-2349038125089",
//   "suppressRecipientMsg":false,
//   "alternateSenderName":"4445",
//   "minRecipientKYCLevel":"KYC1",
//   "holdingPeriod":31
//}
            return json;
        }

        public static bool ValidateRequest(DYATransferMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "Login");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "Login");
                    lines = Add2Log(lines, "Bad Params", log_level, "Login");
                }
            }
            return result;
        }

        public static DYATransferMoneyResponse DoTransfer(DYATransferMoneyRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "MyPaga");
            DYATransferMoneyResponse ret = null;
            if (ValidateRequest(RequestBody, ref lines))
            {
                DYACheckAccountRequest req = new DYACheckAccountRequest()
                {
                    MSISDN = RequestBody.MSISDN,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID
                };
                DLDYAValidateAccount result = DBQueries.ValidateDYARequest(req, ref lines);
                if ((result != null) && (result.RetCode == 1000))
                {
                    Int64 dya_trans = DBQueries.InsertDYATrans(RequestBody, ref lines);
                    if (dya_trans > 0)
                    {
                        string soap = BuildTransferMoneySoap(result, RequestBody, dya_trans);
                        lines = Add2Log(lines, "Transfer Money json = " + soap, 100, "MyPaga");
                        string soap_url = Cache.ServerSettings.GetServerSettings("TransferMoneyPagaURL", ref lines);
                        List<Headers> headers = new List<Headers>();
                        lines = Add2Log(lines, "Headers:", 100, "MyPaga");
                        lines = Add2Log(lines, "principal: " + Cache.ServerSettings.GetServerSettings("PagaPrincipal", ref lines), 100, "MyPaga");
                        headers.Add(new Headers { key = "principal", value = Cache.ServerSettings.GetServerSettings("PagaPrincipal", ref lines) });
                        lines = Add2Log(lines, "credentials: " + Cache.ServerSettings.GetServerSettings("PagaCredentials", ref lines), 100, "MyPaga");
                        headers.Add(new Headers { key = "credentials", value = Cache.ServerSettings.GetServerSettings("PagaCredentials", ref lines) });
                        string string_2_encode = "refp_"+dya_trans + RequestBody.Amount.ToString() + ".0+" + RequestBody.MSISDN + Cache.ServerSettings.GetServerSettings("PagaHMAC", ref lines);
                        lines = Add2Log(lines, "before hash " + string_2_encode, 100, "MyPaga");
                        string encoded_string = SHAEnc512.GenerateSHA512String(string_2_encode);
                        lines = Add2Log(lines, "hash: " + encoded_string, 100, "MyPaga");
                        headers.Add(new Headers { key = "hash", value = encoded_string });


                        string response_Code = "";
                        string message = "";
                        string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, ref lines);
                        if (response != "")
                        {
                            lines = Add2Log(lines, "Response = " + response, 100, "MyPaga");
                            
                            try
                            {
                                dynamic auth_json_response = JsonConvert.DeserializeObject(response);
                                response_Code = auth_json_response.responseCode;
                                message = auth_json_response.message;

                                if (response_Code == "0")
                                {
                                    ret = new DYATransferMoneyResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = message
                                    };
                                }
                                else
                                {
                                    ret = new DYATransferMoneyResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "Transfer has failed with the following error: " + response_Code + " - " + message
                                    };
                                }
                                DBQueries.UpdateDYATrans(dya_trans, response_Code, message, ref lines);
                            }
                            catch (Exception ex)
                            {
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 1030,
                                    Description = "Paga Request has failed - Network problem"
                                };
                            }

                        }
                        else
                        {
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 1030,
                                Description = "Paga Request has failed - Network problem"
                            };
                        }
                    }
                    else
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 5002,
                            Description = "Internal Error"
                        };
                    }

                }
                else
                {
                    if (result == null)
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description
                        };
                    }
                }
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            lines = Add2Log(lines, text, log_level, "DYATransferMoney");
            lines = Write2Log(lines);
            return ret;
        }
    }
}