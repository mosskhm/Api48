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
    public class MyHisa
    {
        public static RefundAirTimeResponse RefundAirTime(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };

            //Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, ref lines);
            Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, "4", ref lines);
            string network_id = (service.operator_id == 10 ? "4" : "1");
            

            string refund_url = Api.Cache.ServerSettings.GetServerSettings("MyHisaurl", ref lines);
            string postBody = "network="+ network_id + "&amount="+ RequestBody.Amount*100 + "&phone_number="+RequestBody.MSISDN;
            string refund_key = Api.Cache.ServerSettings.GetServerSettings("MyHisaAuth", ref lines);
            lines = Add2Log(lines, " Json = " + postBody, 100, lines[0].ControlerName);
            lines = Add2Log(lines, " Sending to URL = " + refund_url, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            headers.Add(new Headers { key = "api-auth", value = refund_key });

            string response_body = CallSoap.CallSoapRequest(refund_url, postBody, headers, 3, ref lines);
            lines = Add2Log(lines, " Response " + response_body, 100, lines[0].ControlerName);
            if (response_body != null)
            {
                try
                {
                    dynamic refund_json_response = JsonConvert.DeserializeObject(response_body);
                    string status = refund_json_response.status_code;
                    string message = refund_json_response.message;
                    if (!String.IsNullOrEmpty(status))
                    {
                        status = (status == "0" ? "01" : "500");
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
    }
}