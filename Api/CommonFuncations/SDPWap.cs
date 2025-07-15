using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class SDPWap
    {
        public static string CreateWapPushURL(ServiceClass service, string msisdn, ref List<LogLines> lines)
        {
            string url = "";
            Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into yellowdot.requests (msisdn, date_time, subscription_method_id, pin_code, service_id) values ("+msisdn+", now(), 0, 0, "+service.service_id+")", ref lines);
            string wap_url = (service.is_staging == true? Cache.ServerSettings.GetServerSettings("SDPWAPURL_" + service.operator_id + "_STG", ref lines) : Cache.ServerSettings.GetServerSettings("SDPWAPURL_" + service.operator_id, ref lines));
            string notification_url = Cache.ServerSettings.GetServerSettings("SDPWAPNotificationURL_" + service.operator_id, ref lines) + "%3Fy_id%3D" + req_id;
            string utctime = DateTime.UtcNow.ToString("s") + "Z";
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();
            
            //url = @"http://196.11.240.222/portalone/otp/southafrica?spAccount=270110001345&spPasswd=lYmBi8CIZuPfG2f4RYm/65Se8MWGSnaFWGkL1Posbxs=&endUserIdentifier=" + MSISDN + @"&scope=99&transactionId=" + Guid.NewGuid().ToString() + "&notificationUrl=http://www.yellowdotgames.co.za/callbacks/notify.ashx%3Fy_id%3D" + req_id + @"&productID=2701220000003415&nonce=201705051302360000&created=2017-05-05T13:02:36Z&tokenValidity=1&tokenType=1&amount=10&totalAmount=200&currency=ZAR";
            url = wap_url + "spAccount=" + service.spid+"&spPasswd="+ final_password + "&endUserIdentifier="+msisdn+"&scope=99&transactionId="+ req_id + "&notificationUrl="+ notification_url + "&productID="+service.product_id+"&nonce=201701101302360000&created=" + utctime;


            return url;
        }

        public static string CreateWapPushURLNigeria(ServiceClass service, string msisdn, ref List<LogLines> lines)
        {
            PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(service.airtimr_amount * 100), ref lines);
            string url = "";
            Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into yellowdot.requests (msisdn, date_time, subscription_method_id, pin_code, service_id) values (" + msisdn + ", now(), 0, 0, " + service.service_id + ")", ref lines);
            string wap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("SDPWAPURL_" + service.operator_id + "_STG", ref lines) : Cache.ServerSettings.GetServerSettings("SDPWAPURL_" + service.operator_id, ref lines));
            string notification_url = Cache.ServerSettings.GetServerSettings("SDPWAPNotificationURL_" + service.operator_id, ref lines) + "y_id%3D" + req_id;
            string utctime = DateTime.UtcNow.ToString("s") + "Z";
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string str_2encode = req_id + utctime + service.spid_password;

            string has265 = md5.ComputeSha256Hash(str_2encode);
            string final_password = Base64.EncodeDecodeBase64(has265, 1);


            //http://10.199.249.141:11100/portalone/otp/nigeria?spAccount=2340110001199&spPasswd=tvF5JGWgrTGDGw83yNG06m2aV8FRczbBuSpkpgmlNH4=&endUserIdentifier=2347067450201&scope=99&transactionId=002311111112222202211112212211100&notificationUrl=http://10.199.198.18:9002/notify&productID=23401220000026676&nonce=20170110130236000101&created=2014-01-15T18:03:36Z&serviceID=234012000023049

            //41.206.4.159/portalone/otp/nigeria?spAccount=2340110001199&spPasswd=tvF5JGWgrTGDGw83yNG06m2aV8FRczbBuSpkpgmlNH4=&endUserIdentifier=2349038125089&scope=79&transactionId=0144849970&productID=23401220000027633&nonce=20170110130236000101&created=2014-01-15T18:03:36Z&serviceID=234012000023847&notificationUrl=http://api.ydplatform.com/handlers/wapnotify.ashx?y_id%3D4320
            url = wap_url + "spAccount=" + service.spid + "&spPasswd=tvF5JGWgrTGDGw83yNG06m2aV8FRczbBuSpkpgmlNH4=&endUserIdentifier=" + msisdn + "&scope="+ (service.is_ondemand == true ? "79" : "99") + "&transactionId=0128"+ req_id + "&productID=" + service.product_id + "&nonce=20170110130236000101&created=2014-01-15T18:03:36Z&serviceID=" + service.real_service_id + "&notificationUrl=" + notification_url;

            return url;
        }


    }
}