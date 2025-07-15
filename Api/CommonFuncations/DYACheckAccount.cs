using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class DYACheckAccount
    {
        

        public static bool ValidateRequest(DYACheckAccountRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0))
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

        public static string BuildValidateAccountHolderSoap(string msisdn)
        {
            string soap = "<ns2:validateaccountholderrequest xmlns:ns2=\"http://www.ericsson.com/em/emm/sp/backend\"><accountholderid>ID:"+msisdn+"/MSISDN</accountholderid></ns2:validateaccountholderrequest>";
            return soap;
        }

        public static DYACheckAccountResponse DODYACheckAccount(DYACheckAccountRequest RequestBody)
        {
            DYACheckAccountResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DYACheckAccount");
            if (ValidateRequest(RequestBody, ref lines))
            {
                //dosdp login?
                //validateaccountholder
                DLDYAValidateAccount result = DBQueries.ValidateDYARequest(RequestBody, ref lines);
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
                    soap_url = Cache.ServerSettings.GetServerSettings("ValidateAccountHolderURL_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                    lines = Add2Log(lines, "SoapURL = " + soap_url, 100, "DYACheckAccount");
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
                            headers.Add(new Headers { key = "X-RequestHeader", value = "request TransId=\"\",ServiceId=\"\",LinkId=\"\",PresentId=\"\""});
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
                        case 1:
                            string soap = BuildValidateAccountHolderSoap(RequestBody.MSISDN.ToString());
                            lines = Add2Log(lines, "Validate Account Holder Soap = " + soap, 100, "DYACheckAccount");

                            string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
                            if (response != "")
                            {
                                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                                string dya_response = CommonFuncations.ProcessXML.GetXMLNode(response, "valid", ref lines);
                                if (dya_response != "")
                                {
                                    if (dya_response == "true" || dya_response == "false")
                                    {
                                        ret = new DYACheckAccountResponse()
                                        {
                                            ResultCode = (dya_response == "true" ? 1000 : 1010),
                                            Description = (dya_response == "true" ? "User has a DYA Account" : "User does not have a DYA Account")
                                        };
                                    }
                                    else
                                    {
                                        ret = new DYACheckAccountResponse()
                                        {
                                            ResultCode = 1030,
                                            Description = "DYA Request has failed - Network problem"
                                        };
                                    }
                                }
                                else
                                {
                                    ret = new DYACheckAccountResponse()
                                    {
                                        ResultCode = 1030,
                                        Description = "DYA Request has failed - Network problem"
                                    };
                                }
                            }
                            else
                            {
                                ret = new DYACheckAccountResponse()
                                {
                                    ResultCode = 1030,
                                    Description = "DYA Request has failed - Network problem"
                                };
                            }
                            break;
                        case 12:
                        case 4:
                            if (service.service_id == 730)
                            {
                                ret = Moov.CheckAccount(RequestBody, service, ref lines);
                            }
                            else
                            {
                                ret = ECWBenin.CheckAccount(RequestBody, service, ref lines);
                            }
                            break;
                        case 5:
                            ret = ECWBenin.CheckAccount(RequestBody, service, ref lines);
                            break;
                        case 6:
                            ret = Orange.CheckAccount(RequestBody, service, ref lines);
                            break;
                        case 7:
                            ret = MTNOpeanAPI.CheckAccount(RequestBody, service, ref lines);
                            break;
                        case 9:
                            ret = MADAPI.CheckAccount(RequestBody, service, ref lines);
                            break;
                        case 10:
                            
                            break;
                    }

                    //if (service.operator_id == 14)
                    //{
                    //    ret = Moov.CheckAccount(RequestBody, service, ref lines);
                    //}
                    //else
                    //{
                    //    if (service.operator_id == 15)
                    //    {
                    //        ret = Orange.CheckAccount(RequestBody, service, ref lines);
                    //    }
                    //    else
                    //    {
                    //        if (service.dya_type == 5)
                    //        {
                    //            ret = ECWBenin.CheckAccount(RequestBody, service, ref lines);
                    //        }
                    //        else
                    //        {
                    //            if (service.dya_type == 7)
                    //            {
                    //                ret = MTNOpeanAPI.CheckAccount(RequestBody, service, ref lines);
                    //            }
                    //            else
                    //            {
                    //                string soap = BuildValidateAccountHolderSoap(RequestBody.MSISDN.ToString());
                    //                lines = Add2Log(lines, "Validate Account Holder Soap = " + soap, 100, "DYACheckAccount");

                    //                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
                    //                if (response != "")
                    //                {
                    //                    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                    //                    string dya_response = CommonFuncations.ProcessXML.GetXMLNode(response, "valid", ref lines);
                    //                    if (dya_response != "")
                    //                    {
                    //                        if (dya_response == "true" || dya_response == "false")
                    //                        {
                    //                            ret = new DYACheckAccountResponse()
                    //                            {
                    //                                ResultCode = (dya_response == "true" ? 1000 : 1010),
                    //                                Description = (dya_response == "true" ? "User has a DYA Account" : "User does not have a DYA Account")
                    //                            };
                    //                        }
                    //                        else
                    //                        {
                    //                            ret = new DYACheckAccountResponse()
                    //                            {
                    //                                ResultCode = 1030,
                    //                                Description = "DYA Request has failed - Network problem"
                    //                            };
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        ret = new DYACheckAccountResponse()
                    //                        {
                    //                            ResultCode = 1030,
                    //                            Description = "DYA Request has failed - Network problem"
                    //                        };
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    ret = new DYACheckAccountResponse()
                    //                    {
                    //                        ResultCode = 1030,
                    //                        Description = "DYA Request has failed - Network problem"
                    //                    };
                    //                }
                    //            }

                    //        }
                    //    }
                        
                        
                    //}
                    
                }
                else
                {
                    if (result == null)
                    {
                        ret = new DYACheckAccountResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        ret = new DYACheckAccountResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description
                        };
                    }
                }
                
            }
            else
            {
                ret = new DYACheckAccountResponse()
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