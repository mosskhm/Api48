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
    public class uan_glo
    {
        public static RefundAirTimeResponse RefundAirTime(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };

            Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody,"5", ref lines);

            //string network_id = (service.operator_id == 10 ? "4" : "1");


            string refund_url = Api.Cache.ServerSettings.GetServerSettings("Uan_GLO_URL", ref lines);
            //string postBody = "network=" + network_id + "&amount=" + RequestBody.Amount * 100 + "&phone_number=" + RequestBody.MSISDN;
            string new_msisdn = "0" + RequestBody.MSISDN.ToString().Substring(3);
            string ak = Api.Cache.ServerSettings.GetServerSettings("Uan_GLO_AK", ref lines);
            string json = "{\"ogn\":\"" + new_msisdn + "\",\"apid\":\"pisidot\",\"amt\":\""  + RequestBody.Amount + "\",\"ak\":\"" + ak + "\",\"config\":\"api\",\"mk\":false,\"sid\":\"" + refund_id + "\"}";

            
            lines = Add2Log(lines, " Json = " + json, 100, lines[0].ControlerName);
            refund_url = refund_url + Api.CommonFuncations.Base64.EncodeDecodeBase64(json,1);
            lines = Add2Log(lines, " Sending to URL = " + refund_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            

            string response_body = CallSoap.CallSoapRequest(refund_url, "", headers, 1, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    //{"balance":999998,"ogn":"09057820447","message":"Your http to credit 09057820447 with 1.0 is
                    // processed","status":200,"sid":"Irede - 1 - 2022 - 01 - 04"}
                    dynamic refund_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = refund_json_response.status;
                    string message = refund_json_response.message;
                    if (!String.IsNullOrEmpty(status))
                    {
                        status = (status == "200" ? "01" : "500");
                        int result_code = (status == "01" ? 1000 : 1050);
                        DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "' where dya_id = " + refund_id, ref lines);
                        ret = new RefundAirTimeResponse()
                        {
                            ResultCode = result_code,
                            Description = message,
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

        public static string RefundAirTimeSkip(string msisdn, string dya_id, string amount, out string res, ref List<LogLines> lines)
        {
            res = "";
            string status = "0";

            string refund_url = Api.Cache.ServerSettings.GetServerSettings("Uan_GLO_URL", ref lines);
            //string postBody = "network=" + network_id + "&amount=" + RequestBody.Amount * 100 + "&phone_number=" + RequestBody.MSISDN;
            string new_msisdn = "0" + msisdn.Substring(3);
            string ak = Api.Cache.ServerSettings.GetServerSettings("Uan_GLO_AK", ref lines);
            string json = "{\"ogn\":\"" + new_msisdn + "\",\"apid\":\"pisidot\",\"amt\":\"" + amount + "\",\"ak\":\"" + ak + "\",\"config\":\"api\",\"mk\":false,\"sid\":\"" + dya_id + "\"}";


            lines = Add2Log(lines, " Json = " + json, 100, lines[0].ControlerName);
            refund_url = refund_url + Api.CommonFuncations.Base64.EncodeDecodeBase64(json, 1);
            lines = Add2Log(lines, " Sending to URL = " + refund_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();


            string response_body = CallSoap.CallSoapRequest(refund_url, "", headers, 1, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    //{"balance":999998,"ogn":"09057820447","message":"Your http to credit 09057820447 with 1.0 is
                    // processed","status":200,"sid":"Irede - 1 - 2022 - 01 - 04"}
                    dynamic refund_json_response = JsonConvert.DeserializeObject(response_body);
                    status = refund_json_response.status;
                    string message = refund_json_response.message;
                    res = message;
                    if (!String.IsNullOrEmpty(status))
                    {
                        
                        status = (status == "200" ? "01" : "500");
                        int result_code = (status == "01" ? 1000 : 1050);
                        DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '" + status + "', result_desc = '" + message.Replace("'", "") + "' where dya_id = " + dya_id, ref lines);
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return status;

        }
    }
}