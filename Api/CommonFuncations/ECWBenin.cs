using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ECWBenin
    {
        public static DYACheckAccountResponse CheckAccount(DYACheckAccountRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckAccountResponse ret = new DYACheckAccountResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"
            };

            ret = new DYACheckAccountResponse()
            {
                ResultCode = 1000,
                Description = "User has an account.",
                FirstName = "",
                LastName = ""
            };

            //ECWServiceInfo service_info = null;
            //service_info = GetECWServiceInfo(service.service_id, ref lines);
            //if (service_info != null)
            //{
            //    string soap_url = service_info.getaccountholderinfo;
            //    string soap = "<ns0:getaccountholderinforequest xmlns:ns0=\"http://www.ericsson.com/em/emm/provisioning/v1_0\"><identity>ID:"+RequestBody.MSISDN+"/MSISDN</identity></ns0:getaccountholderinforequest>";
            //    string response = Curl.CallECWCurl(soap_url, service_info, soap, ref lines);
            //    if (!String.IsNullOrEmpty(response))
            //    {
            //        if (response.Contains("errorResponse") || response.Contains("faultcode"))
            //        {
            //            ret = new DYACheckAccountResponse()
            //            {
            //                ResultCode = 1020,
            //                Description = "User does not have an account."
            //            };

            //        }
            //        else
            //        {
            //            string first_name = CommonFuncations.ProcessXML.GetXMLNode(response, "firstname", ref lines);
            //            string last_name = CommonFuncations.ProcessXML.GetXMLNode(response, "surname", ref lines);
            //            string status = CommonFuncations.ProcessXML.GetXMLNode(response, "accountholderstatus", ref lines);
            //            if (status == "ACTIVE")
            //            {
            //                ret = new DYACheckAccountResponse()
            //                {
            //                    ResultCode = 1000,
            //                    Description = "User has an account.",
            //                    FirstName = first_name,
            //                    LastName = last_name
            //                };

            //            }
            //            else
            //            {
            //                ret = new DYACheckAccountResponse()
            //                {
            //                    ResultCode = 1020,
            //                    Description = "User does not have an account."
            //                };
            //            }
            //        }
            //    }

            //}
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

            ECWServiceInfo service_info = null;
            
            service_info = GetECWServiceInfo(service.service_id, ref lines);
            
            if (service_info != null)
            {
                string soap_url = service_info.transfer;
                string soap = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><ns0:sptransferrequest xmlns:ns0=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend\"><sendingfri>FRI:" + service_info.username + "/USER</sendingfri><receivingfri>FRI:" + RequestBody.MSISDN + "/MSISDN</receivingfri><amount><amount>" + RequestBody.Amount + "</amount><currency>XOF</currency></amount><providertransactionid>" + dya_id + "</providertransactionid><name><firstname></firstname><lastname></lastname></name><sendernote></sendernote><receivermessage>Received</receivermessage><referenceid>" + dya_id + "</referenceid></ns0:sptransferrequest>";
                
                string response = Curl.CallECWCurl(soap_url, service_info, soap, ref lines);
                if (!String.IsNullOrEmpty(response))
                {
                    if (response.Contains("errorResponse") || response.Contains("faultcode"))
                    {
                        string status = CommonFuncations.ProcessXML.GetXMLNode(response, "ns0:errorResponse", "errorcode", ref lines);
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1050,
                            Description = "Transfer has failed - internal error code: " + status,
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };
                        DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "500", status, ref lines);

                    }
                    else
                    {
                        string transactionid = CommonFuncations.ProcessXML.GetXMLNode(response, "transactionid", ref lines);
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1000,
                            Description = "Transfer was successful",
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };
                        DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", "Success", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);

                    }
                }
            }
            
            
            

            return ret;
        }


        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1050,
                Description = "Failed.",
                TransactionID = dya_id,
                Timestamp = datetime
            };

            ECWServiceInfo service_info = null;
            service_info = GetECWServiceInfo(service.service_id, ref lines);

           
            
            if (service_info != null)
            {
                string soap_url = service_info.debit;
                string soap = "<ns0:debitrequest xmlns:ns0=\"http://www.ericsson.com/em/emm/financial/v1_0\"><fromfri>fri:"+RequestBody.MSISDN+"/msisdn</fromfri><tofri>fri:"+service_info.username+"/USER</tofri> <amount>    <amount>"+RequestBody.Amount+"</amount>    <currency>XOF</currency>    </amount>    <externaltransactionid>"+dya_id+ "</externaltransactionid>    <frommessage>LNBPari</frommessage>    <tomessage>LNBPari</tomessage>    <referenceid>" + dya_id + "</referenceid>  </ns0:debitrequest>";
                string response = Curl.CallECWCurl(soap_url, service_info, soap, ref lines);
                if (response != "")
                {
                    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                    if (response.Contains("errorResponse") || response.Contains("faultcode"))
                    {
                        string description = CommonFuncations.ProcessXML.GetXMLNode(response, "ns0:errorResponse", "errorcode", ref lines);
                        string status = "500";

                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 1050,
                            Description = "Request has failed with the following error: " + status + " - " + description,
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };
                        DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, description, ref lines);

                    }
                    else
                    {
                        string transactionid = CommonFuncations.ProcessXML.GetXMLNode(response, "transactionid", ref lines);
                        string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);

                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 1010,
                            Description = status,
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };
                        DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1010", status, ref lines);
                        if (!String.IsNullOrEmpty(transactionid))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);
                        }
                        
                    }
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

            ECWServiceInfo service_info = null;
            service_info = GetECWServiceInfo(service.service_id, ref lines);
            
            string dya_id = RequestBody.TransactionID;
            if (service_info != null)
            {
                string soap_url = service_info.gettransactionstatus;
                string soap = "<ns2:gettransactionstatusrequest xmlns:ns2=\"http://www.ericsson.com/em/emm/financial/v1_0\"><referenceid>"+ dya_id + "</referenceid></ns2:gettransactionstatusrequest>";
                string response = Curl.CallECWCurl(soap_url, service_info, soap, ref lines);

                if (response != "")
                {
                    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                    if (response.Contains("errorResponse") || response.Contains("faultcode"))
                    {
                        string description = CommonFuncations.ProcessXML.GetXMLNode(response, "description", "errorcode", ref lines);
                        string status = "500";
                        ret = new DYACheckTransactionResponse()
                        {
                            ResultCode = 1050,
                            Description = "Request has failed with the following error: " + status + " - " + description
                        };
                        DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, description, ref lines);
                    }
                    else
                    {
                        string transactionid = CommonFuncations.ProcessXML.GetXMLNode(response, "transactionid", ref lines);
                        string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);
                        //2020-09-19 02:03:26.486: Response = <?xml version="1.0" encoding="UTF-8"?><ns0:gettransactionstatusresponse xmlns:ns0="http://www.ericsson.com/em/emm/financial/v1_0"><transactionid>1201206292</transactionid><status>SUCCESSFUL</status></ns0:gettransactionstatusresponse>


                        if (!String.IsNullOrEmpty(transactionid) && status == "SUCCESSFUL")
                        {
                            ret = new DYACheckTransactionResponse()
                            {
                                ResultCode = 1000,
                                Description = "Success"
                            };

                            DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1000", "Success", ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transactionid + "');", ref lines);
                        }
                        else
                        {
                            if (status == "PENDING")
                            {
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1010", "Pending", ref lines);
                            }
                            else
                            {
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Request has failed"
                                };
                                DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1050", "Request has failed", ref lines);
                            }
                                
                            
                        }
                    }
                }
            }
            return ret;
        }

    }
}