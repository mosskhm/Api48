using Api.CommonFuncations;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.B2Tech142;
using static Api.CommonFuncations.DusanLotto;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for Tester
    /// </summary>
    public class Tester : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Tester");
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;

            string result = CommonFuncations.Evina.GetJSSA("YDGTSH", ref lines);

            //string token = Api.CommonFuncations.B2Tech142.GetTokenNew("mtn-payout", "1q2w9o0p", "9", "121", "https://yellowbet.com.gn/services/b2bapi/api/dynamic/Execute", false, ref lines);
            //List<Api.CommonFuncations.iDoBet.Tickets> unpaid_tickets = null;
            //unpaid_tickets = Api.CommonFuncations.B2TechGNMTN.GetUnPaidTickets(ref lines);
            //List<SportEvents> sports_events = Api.CommonFuncations.B2Tech142.GetEvents("https://yellowbet.com.gn/services/evapi/event/getevents", "9", "121", "yellowbet.com.gn", false, ref lines);

            //string secret_id = Cache.ServerSettings.GetServerSettings("SharpVision_SecretKey", ref lines);
            //DYAReceiveMoneyRequest RequestBody = new DYAReceiveMoneyRequest()
            //{
            //    MSISDN = 22996691258,
            //    ServiceID = 726,
            //    TokenID = "",
            //    Amount = 1000,
            //    TransactionID = "12345"
            //};
            //ServiceClass service = GetServiceByServiceID(726, ref lines);

            //DYACheckTransactionRequest RequstBody_CT = new DYACheckTransactionRequest()
            //{
            //    TransactionID = "12345",
            //    MSISDN = 22996691258,
            //    TokenID = "",
            //    ServiceID = 726
            //};
            //DYACheckTransactionResponse ResponseBody_CT = Api.CommonFuncations.SharpVision.CheckTranaction(RequstBody_CT, service, ref lines);

            //DYAReceiveMoneyResponse response = Api.CommonFuncations.SharpVision.ReceiveMoney(RequestBody, "12345", service, ref lines);




            //foreach (String key in context.Request.QueryString.AllKeys)
            //{
            //    lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "Tester");
            //}

            //ServiceClass service = GetServiceByServiceID(732, ref lines);
            //string text = "S1*5000*338545567*FT1*338687639*FTX";
            //text = "PB|R2*2000*1,3";
            //string my_text = text.Substring(3).ToLower();
            //Api.handlers.smsreceive.PlaceBet("224627007626", service, my_text, ref lines);

            //string js = Api.CommonFuncations.Evina.GetJS("supercash", ref lines);
            //ServiceClass service = GetServiceByServiceID(669, ref lines);
            //string new_url = Cache.ServerSettings.GetServerSettings("FulfillmentURLCIS_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
            //string msisdn = "22996691258";
            //FulfillmentInfo fulfillment_info = GetFulfillmentInfo(service.service_id, ref lines);
            //string product_id = "";
            //if (fulfillment_info != null)
            //{
            //    product_id = fulfillment_info.d_productCode;
            //}
            //new_url = new_url + "&msisdn=" + msisdn + "&input=" + product_id;
            //lines = Add2Log(lines, "soap_url = " + new_url, 100, lines[0].ControlerName);
            //string response = CommonFuncations.CallSoap.GetURLIgnoreCertificate(new_url, ref lines);

            //ServiceClass service = GetServiceByServiceID(913, ref lines);
            //string enc_password = Api.Cache.ServerSettings.GetServerSettings("DusanLottoEncP_" + service.service_id, ref lines);
            //string myboday = "{\"data\":\"g2YU1csS2bjZPr3tXAYA0hik9KNEb67iu+7OW5MrPipa/dZMq7EoKID/A4mLHxEIGlbceN21haheHxfgufEOobYUtMYJl+2EvCwkEXTim7xq6Dejyl6pITSUwthbFr0+\"}";
            //dynamic json_response = JsonConvert.DeserializeObject(myboday);
            //string data = json_response.data;
            //string decoded_json = DecodeAndDecrypt(data, enc_password);
            //lines = Add2Log(lines, "json after decoding" + decoded_json, 100, "");



            //ServiceClass service = GetServiceByServiceID(777, ref lines);
            //string token_id = Api.CommonFuncations.DusanLotto.GetToken(service, ref lines);
            //LottoTicket ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "738874090528", ref lines);
            //ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "778864628518", ref lines);
            //ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "768871444850", ref lines);
            //ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "708872727189", ref lines);
            //ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "578869540737", ref lines);
            //ticket = Api.CommonFuncations.DusanLotto.SearchTicket("777", "578869540736", ref lines);



            //CreateVoucherRequest RequestBody = new CreateVoucherRequest()
            //{
            //    TokenID = "0e75a6da-8c48-4a6f-9a10-6538923524c2",
            //    ServiceID = service.service_id,
            //    MSISDN = 27762410705,
            //    TransactionID = "2345678",
            //    Description = "Test",
            //    Amount = 100
            //};
            //CreateVoucherResponse response = SayThanks.CreateVoucher(RequestBody);



            context.Response.Write("Hello");

            lines = Write2Log(lines);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}