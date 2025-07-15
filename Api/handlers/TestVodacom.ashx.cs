using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.Vodacom;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for TestVodacom
    /// </summary>
    public class TestVodacom : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "TesterVodacom");
            //string header = "26334542e7accbe90aa83485dc02e9580d2f1e5c987c59e1618ccabead5190a88f996ba80f13086fdfb652b6f901994775ff3743c4478289a788865bfd4fc882a51d27ea96a234197d710012ef6abaac97dca1f033702c3a5bf590aefff7c0f9b5297320b0b687b49da28a357ec5c2d36fc121ecfc350d0a3c93497468be5ddfa5145c85a45a958c0b915edc5123c92c298ea394d62c3faf9f0c264e8bfadf32502a7b634d5fce4609674a8e1c437d74ed2acca95642d0d37132e2110b98ca7f4ec7a9e55645e6e548c382a68ee0e5381c859dc473ee4ec7ede441775e3a46bee00c5acb779c77a337c724ab5e02244a94dcddff36fb871c2152ec263791b517";
            //string output_file = Vodacom.Hex2Bin(header, ref lines);
            //string msisdn = Vodacom.OpenSSL(output_file, ref lines);

            //string a = CommonFuncations.ProcessXML.GetXMLNode("<?xml version=\"1.0\" encoding=\"UTF-8\"?><er-response id=\"120055\"><payload><get-service-offers-response><service id=\"vc-yellowdot-games-01\"><pricepoint id=\"package:p-yellowdot-games-c-01_TAX_3_8_999_999_999_TRIAL_*_*_false_false_*\">                    <rate resource=\"ZAR\" tax-rate=\"0.15\">0.0</rate>                    <charging-method>3</charging-method>                    <duration>8</duration>                    <promo-code>TRIAL</promo-code>                    <renewals-until-linked-pricepoint>1</renewals-until-linked-pricepoint>                    <short-package-id>p-yellowdot-games-c-01</short-package-id>                </pricepoint>                <pricepoint id=\"package:p-yellowdot-games-c-01_TAX_3_8_999_999_999_*_*_*_false_false_*\">                    <rate resource=\"ZAR\" tax-rate=\"0.15\">3.0</rate>                    <charging-method>3</charging-method>                    <duration>8</duration>                    <short-package-id>p-yellowdot-games-c-01</short-package-id>                </pricepoint>            </service>        </get-service-offers-response>    </payload></er-response>", "service", "id", ref lines);

            //string str_2encode = "20170110130236000012017-01-10T13:02:36ZHuawei321";
            //string has265 = md5.ComputeSha256Hash(str_2encode);
            //string bas64_has265 = Base64.EncodeDecodeBase64(has265, 1);



            string msisdn = context.Request.QueryString["msisdn"];
            ServiceClass subscriber_service = GetServiceByServiceID(435, ref lines);
            VodSub service_offer = GetServiceOffers(subscriber_service, msisdn, ref lines);

            ServiceClass subscriber_service1 = GetServiceByServiceID(704, ref lines);
            VodSub service_offer1 = GetServiceOffers(subscriber_service1, msisdn, ref lines);


            //LoginRequest LoginRequestBody = new LoginRequest()
            //{
            //    ServiceID = subscriber_service.service_id,
            //    Password = subscriber_service.service_password
            //};
            //LoginResponse res = Login.DoLogin(LoginRequestBody);
            //if (res != null)
            //{
            //    if (res.ResultCode == 1000)
            //    {
            //        string token_id = res.TokenID;
            //        SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
            //        {
            //            ServiceID = subscriber_service.service_id,
            //            TokenID = token_id,
            //            MSISDN = Convert.ToInt64(msisdn),
            //            TransactionID = "ussd_mo",
            //            ActivationID = "3"
            //        };
            //        SubscribeResponse subscribe_response = CommonFuncations.UnSubscribe.DoUnSubscribe(subscribe_RequestBody);
            //        if (subscribe_response != null)
            //        {
            //            lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");
            //        }
            //    }
            //}



            //string emsisdn = context.Request.QueryString["emsisdn"];
            //string action = context.Request.QueryString["action"];
            //context.Response.ContentType = "text/plain";
            //if (action == "validate")
            //{
            //    if (!String.IsNullOrEmpty(msisdn))
            //    {
            //        //string emsisdn1 = CommonFuncations.VodacomIMI.valitadeMSISDN(true, Convert.ToInt64(msisdn), ref lines);
            //        //context.Response.Write(emsisdn1 + "<br>");
            //        //ServiceClass service = GetServiceByServiceID(435, ref lines);
            //        //CommonFuncations.VodacomIMI.SubResult sub_result = CommonFuncations.VodacomIMI.Subscribe(service, emsisdn1, ref lines);
            //        //context.Response.Write(sub_result.code + " : " + sub_result.description);

            //    }
            //}
            //if (action == "subscribe")
            //{
            //    LoginRequest LoginRequestBody = new LoginRequest()
            //    {
            //        ServiceID = 435,
            //        Password = "B2sRjxQCPSGm3iT"
            //    };
            //    LoginResponse res = Login.DoLogin(LoginRequestBody);
            //    if (res != null)
            //    {
            //        if (res.ResultCode == 1000)
            //        {
            //            string token_id = res.TokenID;
            //            SubscribeRequest subscribe_RequestBody = new SubscribeRequest()
            //            {
            //                ServiceID = 435,
            //                TokenID = token_id,
            //                MSISDN = Convert.ToInt64(msisdn),
            //                TransactionID = "Test",
            //                ActivationID = "1"
            //            };
            //            SubscribeResponse subscribe_response = Api.CommonFuncations.Subscribe.DoSubscribe(subscribe_RequestBody);
            //            lines = Add2Log(lines, " ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "track.ydot.co");
            //            context.Response.Write("Subscription Result: " + subscribe_response.ResultCode + " : " + subscribe_response.Description);
            //        }
            //    }
            //}
            //if (!String.IsNullOrEmpty(emsisdn))
            //{
            //    //msisdn = CommonFuncations.VodacomIMI.DecryptMSISDN(true, Convert.ToInt64(emsisdn), ref lines);
            //    context.Response.Write("MSISDN = " + msisdn);
            //}
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