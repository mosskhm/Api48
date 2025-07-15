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
    public class TrustVas
    {
        public class TrustVasRefundRequest
        {
            public string username { get; set; }
            public string password { get; set; }
            public string accountid { get; set; }
            public string network { get; set; }
            public string msisdn { get; set; }
            public int amount { get; set; }
            public string utilityType { get; set; }
        }

        public class TrustVasTransferRequest
        {
            public string username { get; set; }
            public string password { get; set; }
            public string accountid { get; set; }
            public string msisdn { get; set; }
            public int amount { get; set; }
        }



        public static RefundAirTimeResponse RefundAirTime(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };

            Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, ref lines);

            TrustVasRefundRequest request = new TrustVasRefundRequest()
            {
                username = Api.Cache.ServerSettings.GetServerSettings("TrueVasUserName", ref lines),
                password = Api.Cache.ServerSettings.GetServerSettings("TrueVasPassword", ref lines),
                accountid = Api.Cache.ServerSettings.GetServerSettings("TrueVasAccountID", ref lines),
                network = Api.Cache.ServerSettings.GetServerSettings("TrueVasNetworkID", ref lines),
                msisdn = RequestBody.MSISDN.ToString(),
                amount = RequestBody.Amount
            };

            string refund_url = Api.Cache.ServerSettings.GetServerSettings("TrueVasVendURL", ref lines);
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Sending to URL = " + refund_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(refund_url, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    dynamic refund_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = refund_json_response.status;
                    string message = refund_json_response.message;
                    DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "' where dya_id = " + refund_id, ref lines);
                    if (status == "200" && message == "Successful")
                    {
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = 1000,
                            Description = "Refund AirTime was successful",
                            TransactionID = refund_id.ToString()
                        };
                    }
                    else
                    {
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = 1050,
                            Description = "Refund AirTime has failed - internal error code: " + status + " - " + message,
                            TransactionID = refund_id.ToString()
                        };
                    }
                    
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return ret;

        }
        public static RefundAirTimeResponse RefundAirTimeNew(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };

            Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, ref lines);

            TrustVasRefundRequest request = new TrustVasRefundRequest()
            {
                username = Api.Cache.ServerSettings.GetServerSettings("TrustVasUserName", ref lines),
                password = Api.Cache.ServerSettings.GetServerSettings("TrustVasPassword", ref lines),
                accountid = Api.Cache.ServerSettings.GetServerSettings("TrustVasAccountID", ref lines),
                network = Api.Cache.ServerSettings.GetServerSettings("TrustVasNetworkID", ref lines),
                utilityType = Api.Cache.ServerSettings.GetServerSettings("TrustVasUtilityType", ref lines),
                msisdn = RequestBody.MSISDN.ToString(),
                amount = RequestBody.Amount
            };

            string refund_url = Api.Cache.ServerSettings.GetServerSettings("TrustVasVendURL", ref lines);
            string postBody = "{" + "\"airtimeUtilityDist\": {\"accountID\":\"" + request.accountid + "\", \"transReference\":\"" + refund_id + "\", \"amountRequested\":\"" +
                request.amount + "\", \"packageID\":\"\", \"utilityType\":\"" + request.utilityType + "\",\"subscriberMsisdn\":\"" + request.msisdn + "\", \"subscriberNetwork\":\"" + request.network + "\"}}";
               
            lines = Add2Log(lines, " Json = " + postBody, 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Sending to URL = " + refund_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "username", value = request.username });
            headers.Add(new Headers { key = "password", value = request.password });
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(refund_url, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    dynamic refund_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = refund_json_response.airtimeUtilityDist.statusCode == "00" ? "01" : refund_json_response.airtimeUtilityDist.statusCode;
                    string message = refund_json_response.airtimeUtilityDist.statusMessage;
                    DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "' where dya_id = " + refund_id, ref lines);
                    if (status == "01" && message == "Successful")
                    {
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = 1000,
                            Description = "Refund AirTime was successful",
                            TransactionID = refund_id.ToString()
                        };
                    }
                    else
                    {
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = 1050,
                            Description = "Refund AirTime has failed - internal error code: " + status + " - " + message,
                            TransactionID = refund_id.ToString()
                        };
                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return ret;

        }

        public static DYATransferMoneyResponse DoTransfer(DYATransferMoneyRequest RequestBody, Int64 dya_trans, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_trans.ToString()
            };

            TrustVasTransferRequest request = new TrustVasTransferRequest()
            {
                username = Api.Cache.ServerSettings.GetServerSettings("TrueVasUserName", ref lines),
                password = Api.Cache.ServerSettings.GetServerSettings("TrueVasPassword", ref lines),
                accountid = Api.Cache.ServerSettings.GetServerSettings("TrueVasAccountID", ref lines),
                msisdn = RequestBody.MSISDN.ToString(),
                amount = RequestBody.Amount
            };
            string payout_url = Api.Cache.ServerSettings.GetServerSettings("TrueVasPayoutURL", ref lines);
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Sending to URL = " + payout_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(payout_url, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    dynamic payout_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = payout_json_response.status;
                    string message = payout_json_response.message;
                    string reference = payout_json_response.reference;
                    DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "', transaction_id = '"+reference+"'  where dya_id = " + dya_trans, ref lines);
                    if (status == "200")
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1000,
                            Description = "Transfer was successful",
                            TransactionID = dya_trans.ToString()
                        };
                    }
                    else
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1050,
                            Description = "Transfer has failed - internal error code: " + status,
                            TransactionID = dya_trans.ToString()
                        };
                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return ret;
        }

        public static DYATransferMoneyResponse DoTransfer(DYATransferMoneyRequest RequestBody, Int64 dya_trans, string datetime, ref List<LogLines> lines)
        {
            DYATransferMoneyResponse ret = new DYATransferMoneyResponse()
            {
                ResultCode = 1030,
                Description = "DYA Request has failed - Network problem",
                TransactionID = dya_trans.ToString(),
                Timestamp = datetime
            };

            TrustVasTransferRequest request = new TrustVasTransferRequest()
            {
                username = Api.Cache.ServerSettings.GetServerSettings("TrueVasUserName", ref lines),
                password = Api.Cache.ServerSettings.GetServerSettings("TrueVasPassword", ref lines),
                accountid = Api.Cache.ServerSettings.GetServerSettings("TrueVasAccountID", ref lines),
                msisdn = RequestBody.MSISDN.ToString(),
                amount = RequestBody.Amount
            };
            string payout_url = Api.Cache.ServerSettings.GetServerSettings("TrueVasPayoutURL", ref lines);
            string postBody = JsonConvert.SerializeObject(request);
            lines = Add2Log(lines, " Json = " + postBody, 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Sending to URL = " + payout_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(payout_url, postBody, headers, 2, true, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    dynamic payout_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = payout_json_response.status;
                    string message = payout_json_response.message;
                    string reference = payout_json_response.reference;
                    DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "', transaction_id = '" + reference + "'  where dya_id = " + dya_trans, ref lines);
                    if (status == "200")
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1000,
                            Description = "Transfer was successful",
                            TransactionID = dya_trans.ToString(),
                            Timestamp = datetime
                        };
                    }
                    else
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 1050,
                            Description = "Transfer has failed - internal error code: " + status,
                            TransactionID = dya_trans.ToString(),
                            Timestamp = datetime
                        };
                    }

                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return ret;
        }
    }
}