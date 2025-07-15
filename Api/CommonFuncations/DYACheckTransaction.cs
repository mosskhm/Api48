using Api.DataLayer;
using Api.HttpItems;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class DYACheckTransaction
    {
        public static bool ValidateRequest(DYACheckTransactionRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            bool result = false;
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.TransactionID;
                logMessages.Add(new { message = text, msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, 100, "DYACheckTransaction");
                }
                else
                {
                    lines = Add2Log(lines, text, 100, "DYACheckTransaction");
                    lines = Add2Log(lines, "Bad Params", 100, "DYACheckTransaction");
                    logMessages.Add(new { message = "Bad Params", msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });
                }
            }
            return result;
        }

        public static string BuildCheckTransactionSoap(string transaction_id)
        {
            string soap = "<ns2:gettransactionstatusrequest xmlns:ns2=\"http://www.ericsson.com/em/emm\"><referenceid>"+ transaction_id + "</referenceid></ns2:gettransactionstatusrequest>";
            return soap;
        }

        public static DYACheckTransactionResponse DODYACheckTransaction(DYACheckTransactionRequest RequestBody)
        {
            var logMessages = new List<object>();
            Guid myGuid = Guid.NewGuid();
            string logz_id = myGuid.ToString().Replace("-", "");
            string app_name = "DYACheckTransaction";

            DYACheckTransactionResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DYACheckTransaction");
            if (ValidateRequest(RequestBody, ref lines, ref logMessages, app_name, logz_id))
            {
                //dosdp login?
                //validateaccountholder
                DYACheckAccountRequest req = new DYACheckAccountRequest()
                {
                    MSISDN = RequestBody.MSISDN,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID
                };
                //DLDYAValidateAccount result = DBQueries.ValidateDYARequest(RequestBody, ref lines);
                DLDYAValidateAccount result = Cache.Services.ValidateDYARequestLight(req, ref lines, ref logMessages, app_name, logz_id);

                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                if (service.add_zero)
                {
                    string msisdn = RequestBody.MSISDN.ToString();
                    lines = Add2Log(lines, "4th digit " + msisdn.Substring(3, 1), 100, "SendSMS");
                    if (msisdn.Substring(3, 1) != "0")
                    {
                        msisdn = msisdn.Substring(0, 3) + "0" + msisdn.Substring(3);
                        RequestBody.MSISDN = Convert.ToInt64(msisdn);
                        lines = Add2Log(lines, "adding 0 to MSISDN " + msisdn, 100, "SendSMS");
                    }
                }
                if (service.whitelist_only)
                {
                    List<Int64> whitelist_msisdn = Api.Cache.USSD.GetWhiteListUsersPerService(RequestBody.ServiceID, ref lines);
                    if (whitelist_msisdn != null)
                    {
                        if (!whitelist_msisdn.Contains(RequestBody.MSISDN))
                        {
                            lines = Add2Log(lines, "Service allows whitelist users only. " + RequestBody.MSISDN + " is not in the whitelist numbers", 100, "DYAReceiveMoney");
                            result = null;
                        }
                        else
                        {
                            lines = Add2Log(lines, "User is whitelisted " + RequestBody.MSISDN, 100, "DYAReceiveMoney");
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "no whitelist users - Service is allowed to all", 100, "DYAReceiveMoney");
                    }
                }

                if ((result != null) && (result.RetCode == 1000))
                {
                    string soap_url = "";
                    soap_url = Cache.ServerSettings.GetServerSettings("GetTransactionStatus_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                    lines = Add2Log(lines, "SoapURL = " + soap_url, 100, "DYACheckTransaction");
                    List<Headers> headers = new List<Headers>();
                    switch (service.operator_id)
                    {
                        case 2:
                            headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + result.SPID + "\", PasswordDigest=\"J8tjik8z1gjNcNgTaj7kX/WBYew=\", Nonce=\"2010082108334600001\", Created=\"2016-03-02T11:31:26Z\"" });
                            headers.Add(new Headers { key = "Authorization", value = "WSSE realm=\"SDP\", profile=\"UsernameToken\"" });
                            headers.Add(new Headers { key = "X-RequestHeader", value = "request ServiceId=\"234012000010617\", TransId=\"" + RequestBody.MSISDN + "\" , LinkId=\"A15Y89Z32T99\", FA=\"" + RequestBody.MSISDN + "\"" });
                            headers.Add(new Headers { key = "Msisdn", value = RequestBody.MSISDN.ToString() });
                            headers.Add(new Headers { key = "Signature", value = "43AD232FD45FF" });
                            break;
                        case 12:
                            headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + result.SPID + "\", PasswordDigest=\"kw9A7SOKlUDPXr//JYjGQ3oVC30=\", Nonce=\"2010082108334600001\", Created=\"2010-08-21T08:33:46Z\"" });
                            headers.Add(new Headers { key = "Authorization", value = "WSSE realm=\"SDP\", profile=\"UsernameToken\"" });
                            headers.Add(new Headers { key = "X-RequestHeader", value = "request TransId=\"\",ServiceId=\"\",LinkId=\"\",PresentId=\"\"" });
                            headers.Add(new Headers { key = "Msisdn", value = RequestBody.MSISDN.ToString() });
                            break;
                        case 7:
                            headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + result.SPID + "\", PasswordDigest=\"Ia4fn9U88AVJ43SQ+04Vo61vztA=\",Nonce=\"1234\", Created=\"2015-11-19T17:36:57Z\"" });
                            headers.Add(new Headers { key = "Authorization", value = "WSSE realm=\"SDP\", profile=\"UsernameToken\"" });
                            headers.Add(new Headers { key = "X-RequestHeader", value = "request TransId=\"\",ServiceId=\"\",LinkId=\"\",PresentId=\"\"" });
                            headers.Add(new Headers { key = "Msisdn", value = RequestBody.MSISDN.ToString() });
                            break;
                    }
                    switch (service.dya_type)
                    {
                        case 4:
                            ret = Moov.CheckTranaction(RequestBody.TransactionID, service, ref lines);
                            break;
                        case 5:
                            ret = ECWBenin.CheckTranaction(RequestBody, service, ref lines);
                            break;
                        case 6:
                            ret = Orange.CheckTranaction(RequestBody, service, ref lines);
                            break;
                        case 7:
                            ret = MTNOpeanAPI.CheckTranaction(RequestBody, service, ref lines);
                            break;
                        case 10:
                            ret = Flutterwave.CheckTransaction(RequestBody, service, ref lines);
                            break;
                        case 12:
                            ret = SharpVision.CheckTranaction(RequestBody, service, ref lines);
                            break;
                        case 13:
                            ret = AirTelCG.CheckTranaction(RequestBody, service, ref lines);
                            break;
                        
                        default:
                            string soap = BuildCheckTransactionSoap(RequestBody.TransactionID);
                            lines = Add2Log(lines, "Check Transaction Soap = " + soap, 100, "DYACheckTransaction");

                            string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, true, ref lines);
                            if (response != "")
                            {
                                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckTransaction");
                                string dya_response = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);
                                string transactionid = CommonFuncations.ProcessXML.GetXMLNode(response, "transactionid", ref lines);

                                if (!String.IsNullOrEmpty(transactionid))
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + RequestBody.TransactionID + ",'" + transactionid + "') ON DUPLICATE KEY UPDATE external_id = " + transactionid + ";", ref lines);
                                }

                                if (response.Contains("REFERENCE_ID_NOT_FOUND"))
                                {
                                    ret = new DYACheckTransactionResponse()
                                    {
                                        ResultCode = 1005,
                                        Description = "Transaction was not found"
                                    };
                                }
                                else
                                {
                                    if (dya_response != "")
                                    {
                                        if (dya_response == "SUCCESSFUL" || dya_response == "FAILED" || dya_response == "PENDING")
                                        {
                                            ret = new DYACheckTransactionResponse()
                                            {
                                                ResultCode = (dya_response == "SUCCESSFUL" ? 1000 : (dya_response == "FAILED" ? 1050 : 1015)),
                                                Description = (dya_response == "SUCCESSFUL" ? "Transaction was SUCCESSFUL" : (dya_response == "FAILED" ? "Transaction has failed" : "Transaction is pending"))
                                            };

                                        }
                                        else
                                        {

                                            ret = new DYACheckTransactionResponse()
                                            {
                                                ResultCode = 1030,
                                                Description = "DYA Request has failed - Network problem"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        ret = new DYACheckTransactionResponse()
                                        {
                                            ResultCode = 1030,
                                            Description = "DYA Request has failed - Network problem"
                                        };
                                    }
                                }

                            }
                            else
                            {
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1030,
                                    Description = "DYA Request has failed - Network problem"
                                };
                            }
                            break;
                    }
                    //if (service.operator_id == 14)
                    //{
                    //    ret = Moov.CheckTranaction(RequestBody.TransactionID, service, ref lines);
                    //}
                    //else
                    //{
                        
                        //if (service.dya_type == 5)
                        //{
                        //    ret = ECWBenin.CheckTranaction(RequestBody, service, ref lines);
                        //}
                        //else
                        //{
                        //    if (service.dya_type == 6)
                        //    {
                        //        ret = Orange.CheckTranaction(RequestBody, service, ref lines);
                        //    }
                        //    else
                        //    {
                        //        if (service.dya_type == 7)
                        //        {
                        //            ret = MTNOpeanAPI.CheckTranaction(RequestBody, service, ref lines);
                        //        }
                        //        else
                        //        {
                        //            string soap = BuildCheckTransactionSoap(RequestBody.TransactionID);
                        //            lines = Add2Log(lines, "Check Transaction Soap = " + soap, 100, "DYACheckTransaction");

                        //            string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 2, true, ref lines);
                        //            if (response != "")
                        //            {
                        //                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckTransaction");
                        //                string dya_response = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);
                        //                string transactionid = CommonFuncations.ProcessXML.GetXMLNode(response, "transactionid", ref lines);

                        //                if (!String.IsNullOrEmpty(transactionid))
                        //                {
                        //                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + RequestBody.TransactionID + ",'" + transactionid + "') ON DUPLICATE KEY UPDATE external_id = " + transactionid + ";", ref lines);
                        //                }

                        //                if (response.Contains("REFERENCE_ID_NOT_FOUND"))
                        //                {
                        //                    ret = new DYACheckTransactionResponse()
                        //                    {
                        //                        ResultCode = 1005,
                        //                        Description = "Transaction was not found"
                        //                    };
                        //                }
                        //                else
                        //                {
                        //                    if (dya_response != "")
                        //                    {
                        //                        if (dya_response == "SUCCESSFUL" || dya_response == "FAILED" || dya_response == "PENDING")
                        //                        {
                        //                            ret = new DYACheckTransactionResponse()
                        //                            {
                        //                                ResultCode = (dya_response == "SUCCESSFUL" ? 1000 : (dya_response == "FAILED" ? 1010 : 1015)),
                        //                                Description = (dya_response == "SUCCESSFUL" ? "Transaction was SUCCESSFUL" : (dya_response == "FAILED" ? "Transaction has failed" : "Transaction is pending"))
                        //                            };

                        //                        }
                        //                        else
                        //                        {

                        //                            ret = new DYACheckTransactionResponse()
                        //                            {
                        //                                ResultCode = 1030,
                        //                                Description = "DYA Request has failed - Network problem"
                        //                            };
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        ret = new DYACheckTransactionResponse()
                        //                        {
                        //                            ResultCode = 1030,
                        //                            Description = "DYA Request has failed - Network problem"
                        //                        };
                        //                    }
                        //                }

                        //            }
                        //            else
                        //            {
                        //                ret = new DYACheckTransactionResponse()
                        //                {
                        //                    ResultCode = 1030,
                        //                    Description = "DYA Request has failed - Network problem"
                        //                };
                        //            }
                        //        }
                                
                        //    }
                            
                        //}
                        
                    //}
                    
                }
                else
                {
                    if (result == null)
                    {
                        ret = new DYACheckTransactionResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        ret = new DYACheckTransactionResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description
                        };
                    }
                }

            }
            else
            {
                ret = new DYACheckTransactionResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            lines = Add2Log(lines, text, 100, "DYACheckAccount");
            lines = Write2Log(lines);

            return ret;
        }
    }
}