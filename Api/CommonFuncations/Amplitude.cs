/* intergrate with Amplitude CDP
 * https://amplitude.com/docs/apis/analytics/http-v2
 * 
 * CDP endpoint = https://api2.amplitude.com/2/httpapi
 * 
    POST https//api2.amplitude.com/2/httpapi HTTP/1.1
    Host: api2.amplitude.com
    Content-Type: application/json
    Body: {
        "api_key": "YOUR_API_KEY",
            "events": [{
            "user_id": "203201202",
                "device_id": "C8F9E604-F01A-4BD9-95C6-8E5357DF265D",
                "event_type": "watch_tutorial"
                }]
        }
*/

using Antlr.Runtime;
using Api.CommonFuncations;
using Microsoft.Web.Services2.Referral;
using Mysqlx.Prepare;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using static Api.CommonFuncations.CallSoap;
using static Api.Logger.Logger;
using UAParser;


namespace Api.CommonFuncations
{

    public static class Amplitude
    {
        public static string Amplitude_API_KEY = "311cf50fe6b24d454ddfc81a7e71aa82";
        public static string Amplitude_API_URL = "https://api2.amplitude.com/2/httpapi";

        private static readonly HttpClient httpClient = new HttpClient();

        public class AmplitudeRequest{

            public long msisdn {  get; set; }
            public string task {  get; set; }
            public int service_id { get; set; }
            public double amount { get; set; }
            public int retcode { get; set; }
            public string result_msg { get; set; }
            public HttpRequest http { get; set; }

            // added Mrach 2025 for CDP
            public DateTime? billing_date { get; set; }
            public string campaign_name { get; set; }
            public string service_name { get; set; }
            public string tracking_id { get; set; }
            public string channel { get; set; }
            

        }
        public static bool Call_Amplitude(AmplitudeRequest request)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, $"sending {request.task} -> {request.msisdn}, ip = {request.http.UserHostAddress}, url = {request.http.Url}, status = {request.retcode} - {request.result_msg}", 100, $"amplitude_{request.service_id}");
            bool flag = false;
            try
            {
                // extract details from the userAgent string
                var parser = UAParser.Parser.GetDefault();
                ClientInfo clientInfo = parser.Parse(request.http.UserAgent);

                // define our payload
                string json_payload =
                          "{" 
                        +   $" \"api_key\": \"{Amplitude_API_KEY}\""
                        +    ",\"events\":"
                        +       "[" 
                        +           "{"
                        +               $" \"user_id\": \"{request.msisdn}\""
                        +               $",\"event_type\": \"{request.task}\""
                        // +               $",\"ip\": \"{request.http.UserHostAddress}\""
                        +                ",\"event_properties\":"
                        +                   "{"
                        +                       $" \"status\": \"{request.retcode}\""
                        +                       $",\"result\": \"{request.result_msg}\""
                        +                       $",\"amount\": \"{request.amount}\""
                        +                       $",\"referrer\": \"{request.http.UrlReferrer?.ToString() ?? ""}\""
                        +                       $",\"url\": \"{request.http.Url}\""
                        +                       $",\"serviceID\": \"{request.service_id}\""
                        +                       $",\"platform\": \"{request.http.UserAgent}\""
                        +                       $",\"ip\": \"{request.http.UserHostAddress}\""
                        +                       $",\"user_agent\": \"{request.http.UserAgent}\""
                        +                       $",\"device_brand\": \"{clientInfo.Device.Brand}\""
                        +                       $",\"device_manufacturer\": \"{clientInfo.Device.Family}\""
                        +                       $",\"os_name\": \"{clientInfo.OS.Family}\""
                        +                       $",\"os_version\": \"{clientInfo.OS.Major}\""
                        +                       $",\"device_model\": \"{clientInfo.Device.Model}\""
                        // added Mrach 2025 for CDP
                        +                       $",\"Billing_date\": \"{request.billing_date}\""
                        +                       $",\"campaign\": \"{request.campaign_name}\""
                        +                       $",\"Tracking_id\": \"{request.tracking_id}\""
                        +                       $",\"ServiceName\": \"{request.service_name}\""
                        +                       $",\"channel\": \"{request.channel}\""
                        +                   "}"
                        +           "}"
                        +       "]"
                        + "}"
                        ;

                var content = new StringContent(json_payload, Encoding.UTF8, "application/json");

                // post http
                Task<HttpResponseMessage> responseTask = httpClient.PostAsync(Amplitude_API_URL, content);

                // Wait for the task to complete and get the result (blocking)
                HttpResponseMessage response = responseTask.Result;  // Blocks here until the response is received
                string responseBody = response.Content.ReadAsStringAsync().Result;  // Block again to read the response body

                // record result
                lines = Add2Log(lines, $"result: {response.StatusCode} - {responseBody}", 100, "");
                flag = response.IsSuccessStatusCode;
                
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! failed to send message to {Amplitude_API_URL} - reason: {ex.Message}", 100, "");
            }

            Write2Log(lines);       // log output
            return flag;
        } // call_amplitude

    }

}