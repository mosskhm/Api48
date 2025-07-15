using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Vodacom
    {

        public static string ConvertHextoASCII(String hexString, ref List<LogLines> line)
        {
            string base_output = Guid.NewGuid().ToString().Replace("-", "");
            string output_file = @"C:\Apps\Vodacom\" + base_output + ".bin";
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                string[] mylines = { ascii, "" };
                using (StreamWriter outputFile = new StreamWriter(output_file))
                {
                    foreach (string l in mylines)
                        outputFile.WriteLine(l);

                }

                return output_file;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return output_file;
        }

        public static string Hex2Bin(string header, ref List<LogLines> lines)
        {
            string base_output = Guid.NewGuid().ToString().Replace("-", "");
            string input_file = @"C:\Apps\Vodacom\" + base_output + ".hex";
            string[] mylines = { header, "" };
            using (StreamWriter outputFile = new StreamWriter(input_file))
            {
                foreach (string l in mylines)
                    outputFile.WriteLine(l);

            }
            string output_file = @"C:\Apps\Vodacom\" + base_output + ".bin";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Apps\Vodacom\Hex2Bin.pl";
            startInfo.Arguments = input_file + " " + output_file;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            try
            {
                File.Delete(input_file);
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "exception = " + ex.ToString(), 100, "track1");
            }

            return output_file;
        }

        public static string OpenSSL(string bin_filename, ref List<LogLines> lines)
        {
            string msisdn = "-1";
            string base_output = Guid.NewGuid().ToString().Replace("-", "");
            string dec_file = @"C:\Apps\Vodacom\" + base_output + ".txt";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Apps\Vodacom\openssl.exe";//Cache.ServerSettings.GetServerSettings("Hex2Bin", ref lines);
            startInfo.Arguments = "rsautl -in \"" + bin_filename + "\" -inkey \"C:\\Apps\\Vodacom\\dcb_yellowdot_prvkey.der\" -keyform DER -decrypt -out \"" + dec_file + "\"";
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            try
            {
                msisdn = File.ReadAllText(dec_file);
                File.Delete(bin_filename);
                File.Delete(dec_file);
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "exception = " + ex.ToString(), 100, "track1");
            }
            return msisdn;
        }

        public class VodSub
        {
            public ServiceClass service { get; set; }
            public string subscription_date { get; set; }
            public string subscription_id { get; set; }
            public string subscription_state { get; set; }
            public string pricepoint_id { get; set; }


        }
        public static VodSub GetServiceOffers(ServiceClass service, string msisdn, ref List<LogLines> lines)
        {
            VodSub vod_sub = null;
            string client_application_id = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string soap = "<er-request id=\"120054\" client-application-id=\"" + client_application_id + "\" purchase_locale=\"en_ZA\" language_locale=\"en_ZA\">";
            soap = soap + "<payload>";
            soap = soap + "<get-service-offers>";
            soap = soap + "<charging-id type=\"msisdn\">" + msisdn + "</charging-id>";
            soap = soap + "<service-ids>" + service.spid_password + "</service-ids>";
            soap = soap + "</get-service-offers>";
            soap = soap + "</payload>";
            soap = soap + "</er-request>";

            List<Headers> headers = new List<Headers>();
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL", ref lines));
            string username = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string password = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodPassword_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodPassword", ref lines));
            //string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 1, username, password, ref lines);
            //lines = Add2Log(lines, "auth_response = " + auth_response, 100, "track1");
            string pricepoint_id = "";
            switch (service.spid_password)
            {
                case "vc-yellowdot-games-01":
                    pricepoint_id = "package:p-yellowdot-games-c-01_TAX_3_8_999_999_999_TRIAL_*_*_false_false_*";
                    //package:p-yellowdot-tv-c-01_TAX_3_8_999_999_999_*_*_*_false_false_*
                    break;

            }
            vod_sub = new VodSub()
            {
                service = service,
                subscription_date = "",
                subscription_id = "",
                subscription_state = "",
                pricepoint_id = pricepoint_id
            };

            //if (auth_response != null)
            //{
            //    string subscription_id = ProcessXML.GetXMLNode(auth_response, "subscription", "id", ref lines);
            //    string pricepoint_id = ProcessXML.GetXMLNode(auth_response, "pricepoint", "id", ref lines);
            //    string subscription_state = ProcessXML.GetXMLNode(auth_response, "subscription", "status", ref lines);
            //    string subscription_date = ProcessXML.GetXMLNode(auth_response, "purchase-date", ref lines);
            //    try
            //    {
            //        subscription_date = (!String.IsNullOrEmpty(DateTime.Parse(subscription_date).ToString("yyyy-MM-dd HH:mm:ss")) ? subscription_date : "");
            //    }
            //    catch (Exception ex)
            //    {
            //        lines = Add2Log(lines, " exception = " + ex.ToString(), 100, "track1");
            //        subscription_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    }

            //    if (!String.IsNullOrEmpty(subscription_id) && !String.IsNullOrEmpty(subscription_state) && !String.IsNullOrEmpty(subscription_date))
            //    {
            //        vod_sub = new VodSub()
            //        {
            //            service = service,
            //            subscription_date = subscription_date,
            //            subscription_id = subscription_id,
            //            subscription_state = subscription_state,
            //            pricepoint_id = pricepoint_id
            //        };
            //    }
            //    if (vod_sub == null && !String.IsNullOrEmpty(pricepoint_id))
            //    {
            //        vod_sub = new VodSub()
            //        {
            //            service = service,
            //            subscription_date = "",
            //            subscription_id = "",
            //            subscription_state = "",
            //            pricepoint_id = pricepoint_id
            //        };
            //    }

            //}
            return vod_sub;

        }

        public static VodSub GetServiceOffers(ServiceClass service, string msisdn, string enc_msisdn, ref List<LogLines> lines)
        {
            VodSub vod_sub = null;
            string client_application_id = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string soap = "<er-request id=\"120054\" client-application-id=\"" + client_application_id + "\" purchase_locale=\"en_ZA\" language_locale=\"en_ZA\">";
            soap = soap + "<payload>";
            soap = soap + "<get-service-offers>";
            soap = soap + "<charging-id type=\"msisdn\">" + enc_msisdn + "</charging-id>";
            soap = soap + "<service-ids>" + service.spid_password + "</service-ids>";
            soap = soap + "</get-service-offers>";
            soap = soap + "</payload>";
            soap = soap + "</er-request>";

            List<Headers> headers = new List<Headers>();
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL", ref lines));
            string username = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string password = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodPassword_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodPassword", ref lines));
            //string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 1, username, password, ref lines);
            //lines = Add2Log(lines, "auth_response = " + auth_response, 100, "track1");
            string pricepoint_id = "";
            switch (service.spid_password)
            {
                case "vc-yellowdot-games-01":
                    pricepoint_id = "package:p-yellowdot-games-c-01_TAX_3_8_999_999_999_TRIAL_*_*_false_false_*";
                    //package:p-yellowdot-tv-c-01_TAX_3_8_999_999_999_*_*_*_false_false_*
                    break;

            }
            vod_sub = new VodSub()
            {
                service = service,
                subscription_date = "",
                subscription_id = "",
                subscription_state = "",
                pricepoint_id = pricepoint_id
            };

            //if (auth_response != null)
            //{
            //    string subscription_id = ProcessXML.GetXMLNode(auth_response, "subscription", "id", ref lines);
            //    string pricepoint_id = ProcessXML.GetXMLNode(auth_response, "pricepoint", "id", ref lines);
            //    string subscription_state = ProcessXML.GetXMLNode(auth_response, "subscription", "status", ref lines);
            //    string subscription_date = ProcessXML.GetXMLNode(auth_response, "purchase-date", ref lines);
            //    try
            //    {
            //        subscription_date = (!String.IsNullOrEmpty(DateTime.Parse(subscription_date).ToString("yyyy-MM-dd HH:mm:ss")) ? subscription_date : "");
            //    }
            //    catch (Exception ex)
            //    {
            //        lines = Add2Log(lines, " exception = " + ex.ToString(), 100, "track1");
            //        subscription_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //    }

            //    if (!String.IsNullOrEmpty(subscription_id) && !String.IsNullOrEmpty(subscription_state) && !String.IsNullOrEmpty(subscription_date))
            //    {
            //        vod_sub = new VodSub()
            //        {
            //            service = service,
            //            subscription_date = subscription_date,
            //            subscription_id = subscription_id,
            //            subscription_state = subscription_state,
            //            pricepoint_id = pricepoint_id
            //        };
            //    }
            //    if (vod_sub == null && !String.IsNullOrEmpty(pricepoint_id))
            //    {
            //        vod_sub = new VodSub()
            //        {
            //            service = service,
            //            subscription_date = "",
            //            subscription_id = "",
            //            subscription_state = "",
            //            pricepoint_id = pricepoint_id
            //        };
            //    }

            //}
            return vod_sub;

        }

        public static VodSub GetServiceOffers(ServiceClass service, string msisdn, DLValidateSMS result, string enc_msisdn, ref List<LogLines> lines)
        {
            VodSub vod_sub = null;
            string client_application_id = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string soap = "<er-request id=\"120054\" client-application-id=\"" + client_application_id + "\" purchase_locale=\"en_ZA\" language_locale=\"en_ZA\">";
            soap = soap + "<payload>";
            soap = soap + "<get-service-offers>";
            soap = soap + "<charging-id type=\"msisdn\">" + enc_msisdn + "</charging-id>";
            soap = soap + "<service-ids>" + service.spid_password + "</service-ids>";
            soap = soap + "</get-service-offers>";
            soap = soap + "</payload>";
            soap = soap + "</er-request>";

            List<Headers> headers = new List<Headers>();
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL", ref lines));
            string username = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string password = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodPassword_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodPassword", ref lines));
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 1, username, password, ref lines);
            lines = Add2Log(lines, "auth_response = " + auth_response, 100, "track1");
            //          <package-id>p-yellowdot-games-c-01</package-id>

            if (auth_response != null)
            {
                string subscription_id = ProcessXML.GetXMLNode(auth_response, "subscription", "id", ref lines);
                string pricepoint_id = ProcessXML.GetXMLNode(auth_response, "package-id", ref lines);
                if (String.IsNullOrEmpty(pricepoint_id))
                {
                    pricepoint_id = ProcessXML.GetXMLNode(auth_response, "pricepoint", "id", ref lines);
                }

                string subscription_state = ProcessXML.GetXMLNode(auth_response, "subscription", "status", ref lines);
                string subscription_date = ProcessXML.GetXMLNode(auth_response, "purchase-date", ref lines);
                try
                {
                    subscription_date = (!String.IsNullOrEmpty(DateTime.Parse(subscription_date).ToString("yyyy-MM-dd HH:mm:ss")) ? subscription_date : "");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " exception = " + ex.ToString(), 100, "track1");
                    subscription_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                vod_sub = new VodSub()
                {
                    service = service,
                    subscription_date = subscription_date,
                    subscription_id = subscription_id,
                    subscription_state = subscription_state,
                    pricepoint_id = pricepoint_id
                };

                

            }

            return vod_sub;

        }

        public static bool Deactivate(ServiceClass service, string subscriber_id, string msisdn, ref List<LogLines> lines)
        {
            bool res = false;
            string client_application_id = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string soap = "<er-request id=\"100002\" client-application-id=\"" + client_application_id + "\" purchase_locale=\"en_ZA\" language_locale=\"en_ZA\">";
            soap = soap + "<payload>";
            soap = soap + "<inactivate-subscription>";
            //soap = soap + "<msisdn>" + msisdn + "</msisdn>";
            soap = soap + "<charging-id type=\"msisdn\">" + msisdn + "</charging-id>";
            soap = soap + "<subscription-id>" + subscriber_id + "</subscription-id>";
            soap = soap + "<csr-id>" + client_application_id + "</csr-id>";
            soap = soap + "<reason>Client Request</reason>";
            soap = soap + "</inactivate-subscription>";
            soap = soap + "</payload>";
            soap = soap + "</er-request>";

            List<Headers> headers = new List<Headers>();
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL", ref lines));
            string username = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string password = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodPassword_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodPassword", ref lines));
            string deactivation_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 1, username, password, ref lines);
            lines = Add2Log(lines, "deactivation_response = " + deactivation_response, 100, "track1");
            if (deactivation_response != null)
            {
                string result = ProcessXML.GetXMLNode(deactivation_response, "success", ref lines);
                res = (result == "true" ? true : false);
            }
            return res;

        }

        public static bool Deactivate(ServiceClass service, string subscriber_id, string msisdn, string enc_msisdn, ref List<LogLines> lines)
        {
            bool res = false;
            string client_application_id = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string soap = "<er-request id=\"100002\" client-application-id=\"" + client_application_id + "\" purchase_locale=\"en_ZA\" language_locale=\"en_ZA\">";
            soap = soap + "<payload>";
            soap = soap + "<inactivate-subscription>";
            soap = soap + "<charging-id type=\"msisdn\">"+ enc_msisdn + "</charging-id>";
            soap = soap + "<subscription-id>" + subscriber_id + "</subscription-id>";
            soap = soap + "<csr-id>" + client_application_id + "</csr-id>";
            soap = soap + "<reason>Client Request</reason>";
            soap = soap + "</inactivate-subscription>";
            soap = soap + "</payload>";
            soap = soap + "</er-request>";

            List<Headers> headers = new List<Headers>();
            string soap_url = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodGetServiceOfferURL", ref lines));
            string username = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string password = (service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodPassword_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodPassword", ref lines));
            string deactivation_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, 1, username, password, ref lines);
            lines = Add2Log(lines, "deactivation_response = " + deactivation_response, 100, "track1");
            if (deactivation_response != null)
            {
                string result = ProcessXML.GetXMLNode(deactivation_response, "success", ref lines);
                res = (result == "true" ? true : false);
            }
            return res;

        }

        public static string CreateChargeURL(VodSub vod_sub, string msisdn, string transaction_id, ref List<LogLines> lines)
        {
            string url = "";
            Int64 req_id = DBQueries.ExecuteQueryReturnInt64("insert into yellowdot.requests (msisdn, date_time, subscription_method_id, pin_code, service_id, transaction_id) values (" + msisdn + ", now(), 0, 0, " + vod_sub.service.service_id + ",'"+ transaction_id + "')", ref lines);

            string client_application_id = (vod_sub.service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodUserName_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodUserName", ref lines));
            string base_url = (vod_sub.service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodChargeURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodChargeURL", ref lines));
            string redirect_url = HttpUtility.UrlEncode(vod_sub.service.is_staging == true ? Cache.ServerSettings.GetServerSettings("VodRedirectURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("VodRedirectURL", ref lines)) + "%3Freq_id%3D" + req_id;
            url = base_url + "?partner-id=" + client_application_id + "&package-id=" + vod_sub.pricepoint_id + "&client-txn-id=" + req_id + "&token=" + transaction_id + "&partner-redirect-url=" + redirect_url;
            //http://fusion-test.vodacom.co.za:8080/enterprise-services/ppd/service/partner/verify/v1?
            //partner - id = xxxxxxxx & token = xxxxxxxx & package - id = xxxxxxxx & client - txn - id = xxxxxxx & partner - redirecturl =
            //http://www.partnersite.com

            return url;
        }


        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("VodacomKannelURL", ref lines) + "from=YDG&to=" + msisdn + "&text=" + text + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "");
            lines = Add2Log(lines, "SMS Url = " + url, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }

        
    }
}