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
    public class Moov
    {

        public static DYATransferMoneyResponse TransferMoney(DYATransferMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id.ToString(),
                Timestamp = datetime
            };
            
            
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovURL_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovURL_14", ref lines));
            string token = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovToken_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovToken_14", ref lines));

            string soap = "<s11:Envelope xmlns:s11=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            soap = soap + "  <s11:Body>";
            soap = soap + "    <ns1:transferFlooz xmlns:ns1=\"http://api.merchant.tlc.com/\">";
            soap = soap + "      <token>"+token+"</token>";
            soap = soap + "      <request>";
            soap = soap + "        <destination>"+RequestBody.MSISDN+"</destination>";
            soap = soap + "        <amount>"+RequestBody.Amount+"</amount>";
            soap = soap + "        <referenceid>"+dya_id+"</referenceid>";
            soap = soap + "        <walletid>0</walletid>";
            soap = soap + "        <extendeddata>money transfer</extendeddata>";
            soap = soap + "      </request>";
            soap = soap + "    </ns1:transferFlooz>";
            soap = soap + "  </s11:Body>";
            soap = soap + "</s11:Envelope>";

            lines = Add2Log(lines, "SoapURL = " + soap_url, 100, "DYATransferMoney");
            lines = Add2Log(lines, "Transfer Money Soap = " + soap, 100, "DYATransferMoney");
            List<Headers> headers = new List<Headers>();

            string response = CommonFuncations.CallSoap.CallSoapRequestIgnoreCertificate(soap_url, soap, headers, ref lines);
            if (response != "")
            {
                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);
                string referenceid = CommonFuncations.ProcessXML.GetXMLNode(response, "referenceid", ref lines);
                if (status == "0")
                {
                    ret = new DYATransferMoneyResponse()
                    {
                        ResultCode = 1000,
                        Description = "Transfer was successful",
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", "Success", ref lines);
                }
                else
                {
                    string msg = CommonFuncations.ProcessXML.GetXMLNode(response, "message", ref lines);
                    msg = (String.IsNullOrEmpty(msg) ? status : msg.Replace("'",""));
                    
                    ret = new DYATransferMoneyResponse()
                    {
                        ResultCode = 1050,
                        Description = "Transfer has failed - internal error code: " + msg,
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, "Failed - " + msg, ref lines);
                }
                if (!String.IsNullOrEmpty(referenceid))
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + referenceid + "');", ref lines);
                }
            }
            else
            {
                DYACheckTransactionResponse ret1 = CheckTranaction(dya_id, service, ref lines);
                if (ret1.ResultCode == 1000)
                {
                    ret = new DYATransferMoneyResponse()
                    {
                        ResultCode = 1000,
                        Description = "Transfer was successful",
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", "Success", ref lines);
                }
                else
                {
                    ret = new DYATransferMoneyResponse()
                    {
                        ResultCode = 1050,
                        Description = "Transfer has failed - internal error code: " + ret1.Description,
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1050", "Failed", ref lines);
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
            ret = new DYACheckAccountResponse()
            {
                ResultCode = 1000,
                Description = "User has an account.",
                FirstName = "",
                LastName = ""
            };
            

            //string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovURL_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovURL_14", ref lines));
            //string token = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovToken_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovToken_14", ref lines));

            //string soap = "<s11:Envelope xmlns:s11=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            //soap = soap + "  <s11:Body>";
            //soap = soap + "    <ns1:getMobileAccountStatus xmlns:ns1=\"http://api.merchant.tlc.com/\">";
            //soap = soap + "      <token>" + token + "</token>";
            //soap = soap + "     <http>";
            //soap = soap + "      <msisdn>" + RequestBody.MSISDN + "</msisdn>";
            //soap = soap + "     </http>";
            //soap = soap + "    </ns1:getMobileAccountStatus>";
            //soap = soap + "  </s11:Body>";
            //soap = soap + "</s11:Envelope>";

            //List<Headers> headers = new List<Headers>();
            ////string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
            //string response = CommonFuncations.CallSoap.CallSoapRequestIgnoreCertificate(soap_url, soap, headers, ref lines);

            //if (response != "")
            //{
            //    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
            //    string status = CommonFuncations.ProcessXML.GetXMLNode(response, "subscriberstatus", ref lines);
            //    string first_name = CommonFuncations.ProcessXML.GetXMLNode(response, "firstname", ref lines);
            //    string last_name = CommonFuncations.ProcessXML.GetXMLNode(response, "lastname", ref lines);

            //    if (status == "ACTIVE")
            //    {
            //        ret = new DYACheckAccountResponse()
            //        {
            //            ResultCode = 1000,
            //            Description = "User has an account.",
            //            FirstName = first_name,
            //            LastName = last_name
            //        };
                    
            //    }
            //    else
            //    {
            //        ret = new DYACheckAccountResponse()
            //        {
            //            ResultCode = 1020,
            //            Description = "User does not have an account."
            //        };
            //    }
                
            //}

            return ret;
        }
        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1030,
                Description = "Failed.",
                TransactionID = dya_id,
                Timestamp = datetime
            };

            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovURL_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovURL_14", ref lines));
            string token = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovToken_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovToken_14", ref lines));

            string soap = "<s11:Envelope xmlns:s11=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            soap = soap + "  <s11:Body>";
            soap = soap + "    <ns1:Push xmlns:ns1=\"http://api.merchant.tlc.com/\">";
            soap = soap + "      <token>"+token+"</token>";
            soap = soap + "      <msisdn>"+RequestBody.MSISDN+"</msisdn>";
            soap = soap + "      <message>Veuillez entrer votre code PIN pour confirmer votre depot de "+RequestBody.Amount+" CFA sur "+service.service_name+"</message>";
            soap = soap + "      <amount>"+RequestBody.Amount+"</amount>";
            //soap = soap + "      <amount>0</amount>";
            soap = soap + "      <externaldata1>"+dya_id+"</externaldata1>";
            soap = soap + "      <fee>0</fee>";
            soap = soap + "    </ns1:Push>";
            soap = soap + "  </s11:Body>";
            soap = soap + "</s11:Envelope>";

            List<Headers> headers = new List<Headers>();
            //string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
            string response = CommonFuncations.CallSoap.CallSoapRequestIgnoreCertificate(soap_url, soap, headers, ref lines);
            //CommonFuncations.CallSoap.CallSoapRequestAsync(soap_url, soap, headers, 1, ref lines);
            //ret = new DYAReceiveMoneyResponse()
            //{
            //    ResultCode = 1010,
            //    Description = "Pending.",
            //    TransactionID = dya_id.ToString(),
            //    Timestamp = datetime
            //};
            //DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1010", "Pending", ref lines);

            if (response != "")
            {
                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                string description = CommonFuncations.ProcessXML.GetXMLNode(response, "description", ref lines);
                string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);

                string referenceid = CommonFuncations.ProcessXML.GetXMLNode(response, "referenceid", ref lines);
                if (status == "0" && description == "SUCCESS")
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1010,
                        Description = "Pending.",
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1010", "SUCCESS", ref lines);
                }
                else
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1050,
                        Description = "Request has failed with the following error: " + status + " - " + description,
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, description, ref lines);
                }
                if (!String.IsNullOrEmpty(referenceid))
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + referenceid + "');", ref lines);
                }
            }

            return ret;
        }

        public static DYAReceiveMoneyResponse CheckTranaction(string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_id,
                Timestamp = datetime
            };

            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovURL_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovURL_14", ref lines));
            string token = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovToken_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovToken_14", ref lines));

            string soap = "<s11:Envelope xmlns:s11=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            soap = soap + "  <s11:Body>";
            soap = soap + "    <ns1:getTransactionStatus xmlns:ns1=\"http://api.merchant.tlc.com/\">";
            soap = soap + "      <token>"+token+"</token>";
            soap = soap + "      <request>";
            soap = soap + "        <transid>"+dya_id+"</transid>";
            soap = soap + "      </request>";
            soap = soap + "    </ns1:getTransactionStatus>";
            soap = soap + "  </s11:Body>";
            soap = soap + "</s11:Envelope>";


            List<Headers> headers = new List<Headers>();
            //string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
            string response = CommonFuncations.CallSoap.CallSoapRequestIgnoreCertificate(soap_url, soap, headers, ref lines);

            if (response != "")
            {
                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                string description = CommonFuncations.ProcessXML.GetXMLNode(response, "description", ref lines);
                string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);

                string referenceid = CommonFuncations.ProcessXML.GetXMLNode(response, "referenceid", ref lines);
                if (status == "0" && description == "SUCCESS")
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1000,
                        Description = "Success",
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", description, ref lines);
                }
                else
                {
                    ret = new DYAReceiveMoneyResponse()
                    {
                        ResultCode = 1050,
                        Description = "Request has failed with the following error: " + status + " - " + description,
                        TransactionID = dya_id.ToString(),
                        Timestamp = datetime
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, description, ref lines);
                }
                if (!String.IsNullOrEmpty(referenceid))
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + referenceid + "');", ref lines);
                }
            }

            return ret;
        }


        public static DYACheckTransactionResponse CheckTranaction(string dya_id, ServiceClass service, ref List<LogLines> lines)
        {
            DYACheckTransactionResponse ret = new DYACheckTransactionResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem"
                
            };

            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovURL_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovURL_14", ref lines));
            string token = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("MoovToken_14_STG", ref lines) : Cache.ServerSettings.GetServerSettings("MoovToken_14", ref lines));

            string soap = "<s11:Envelope xmlns:s11=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            soap = soap + "  <s11:Body>";
            soap = soap + "    <ns1:getTransactionStatus xmlns:ns1=\"http://api.merchant.tlc.com/\">";
            soap = soap + "      <token>" + token + "</token>";
            soap = soap + "      <request>";
            soap = soap + "        <transid>" + dya_id + "</transid>";
            soap = soap + "      </request>";
            soap = soap + "    </ns1:getTransactionStatus>";
            soap = soap + "  </s11:Body>";
            soap = soap + "</s11:Envelope>";


            List<Headers> headers = new List<Headers>();
            //string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
            string response = CommonFuncations.CallSoap.CallSoapRequestIgnoreCertificate(soap_url, soap, headers, ref lines);

            if (response != "")
            {
                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                string description = CommonFuncations.ProcessXML.GetXMLNode(response, "description", ref lines);
                string status = CommonFuncations.ProcessXML.GetXMLNode(response, "status", ref lines);

                string referenceid = CommonFuncations.ProcessXML.GetXMLNode(response, "referenceid", ref lines);
                if (status == "0" && description == "SUCCESS")
                {
                    ret = new DYACheckTransactionResponse()
                    {
                        ResultCode = 1000,
                        Description = "Success"
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", description, ref lines);
                }
                else
                {
                    ret = new DYACheckTransactionResponse()
                    {
                        ResultCode = 1050,
                        Description = "Request has failed with the following error: " + status + " - " + description
                    };
                    DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), status, description, ref lines);
                }
                if (!String.IsNullOrEmpty(referenceid))
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + referenceid + "');", ref lines);
                }
            }

            return ret;
        }
    }
}