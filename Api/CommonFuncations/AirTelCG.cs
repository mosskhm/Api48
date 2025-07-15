using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;
using Api.DataLayer;
using System.Security.Policy;

namespace Api.CommonFuncations
{
    public class AirTelCG
    {
        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1050,
                Description = "Failed.",
                TransactionID = dya_id,
                Timestamp = datetime
            };

            List<AirTelCGServiceInfo> airtelcg_services = GetAirTelCGServiceInfo(ref lines);
            string url = "", username = "", password = "", callback_url = "";

            if (airtelcg_services != null)
            {
                AirTelCGServiceInfo airtelcg_service = airtelcg_services.Find(x => x.service_id == RequestBody.ServiceID);
                if (airtelcg_service != null)
                {
                    url = airtelcg_service.deposit_url;
                    username = airtelcg_service.user_id;
                    password = airtelcg_service.password;
                    callback_url = airtelcg_service.callback_url;

                    Dictionary<string, string> formData = new Dictionary<string, string>
                    {
                        { "merchantID", username },
                        { "merchantPWD", password },
                        { "transID","YELLOBET-" + dya_id.ToString() },
                        { "amount", RequestBody.Amount.ToString() },
                        { "action", "getID" },
                        { "msisdn", RequestBody.MSISDN.ToString() },
                        { "callbackUrl", callback_url },
                        { "name", "Content-Type, value=multipart/form-data" },
                    };
                    List<Headers> headers = new List<Headers>();
                    string error = "";
                    string body = CallSoap.CallSoapForm(url, headers, 5, formData, out error, ref lines);
                    if (!String.IsNullOrEmpty(body) && error == "")
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 1010,
                            Description = "Pending",
                            TransactionID = dya_id.ToString(),
                            Timestamp = datetime
                        };
                    }
                }

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

            List<AirTelCGServiceInfo> airtelcg_services = GetAirTelCGServiceInfo(ref lines);
            string url = "", username = "", password = "";

            if (airtelcg_services != null)
            {
                AirTelCGServiceInfo airtelcg_service = airtelcg_services.Find(x => x.service_id == RequestBody.ServiceID);
                if (airtelcg_service != null)
                {
                    url = airtelcg_service.credit_url;
                    username = airtelcg_service.user_id;
                    password = airtelcg_service.password;

                    Dictionary<string, string> formData = new Dictionary<string, string>
                    {
                        { "merchantID", username },
                        { "merchantPWD", password },
                        { "transID", "YELLOBET-" + dya_id.ToString() },
                        { "amount", RequestBody.Amount.ToString() },
                        { "destinationMSISDN", RequestBody.MSISDN.ToString() },
                    };
                    List<Headers> headers = new List<Headers>();
                    string error = "";
                    string body = CallSoap.CallSoapForm(url, headers, 5, formData, out error, ref lines);
                    if (!String.IsNullOrEmpty(body) && error == "")
                    {
                        string transaction_id = Api.CommonFuncations.ProcessXML.GetXMLNode(body, "TRXID", ref lines);
                        string status = Api.CommonFuncations.ProcessXML.GetXMLNode(body, "TXNSTATUS", ref lines);

                        if (!String.IsNullOrEmpty(transaction_id))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_id + ",'" + transaction_id + "');", ref lines);
                        }
                        if (status == "200")
                        {
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "01", "Success", ref lines);
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 1000,
                                Description = "Success",
                                TransactionID = dya_id.ToString(),
                                Timestamp = datetime
                            };
                        }
                        else
                        {
                            Api.DataLayer.DBQueries.UpdateDYATrans(Convert.ToInt64(dya_id), "1050", "Failed " + status, ref lines);
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 1050,
                                Description = "Failed " + status,
                                TransactionID = dya_id.ToString(),
                                Timestamp = datetime
                            };
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
            List<AirTelCGServiceInfo> airtelcg_services = GetAirTelCGServiceInfo(ref lines);
            AirTelCGServiceInfo airtelcg_service = airtelcg_services.Find(x => x.service_id == RequestBody.ServiceID);


            if (airtelcg_services != null)
            {
                string url = "";
                string body = "";
                string transaction_id  = RequestBody.TransactionID;
                string username = airtelcg_service.user_id;
                string password = airtelcg_service.password;
                if (!String.IsNullOrEmpty(transaction_id) && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    url = airtelcg_service.check_status_url + "transID=YELLOBET-" + transaction_id + "&action=getSTATUS&merchantID=" + username + "&merchantPWD=" + password;
                }
                


                List<Headers> headers = new List<Headers>();
                if(!String.IsNullOrEmpty(url))
                {
                    //body = CallSoap.GetURL(url, headers, ref lines);
                    body = CallSoap.GetURLIgnoreCertificate(url, ref lines);
                }
                
                if (!String.IsNullOrEmpty(body))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(body))
                        {
                            
                            string status = Api.CommonFuncations.ProcessXML.GetXMLNode(body, "AMSTATUS", ref lines);

                            
                            if (status == "200")
                            {
                                
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "Success"
                                };
                            }
                            else
                            {
                                
                                ret = new DYACheckTransactionResponse()
                                {
                                    ResultCode = 1050,
                                    Description = "Failed " + status
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
    }
}