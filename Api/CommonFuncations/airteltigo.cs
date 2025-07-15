using Api.Cache;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using System.Web.Services.Description;
using static Api.Logger.Logger;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.Controllers;
using Api.handlers;
using System.Web.Razor.Tokenizer.Symbols;

namespace Api.CommonFuncations
{
    public class airteltigo
    {
        public static string GetAuth(List<AirtelTigoInfo> airteltigoconfig)
        {
            string encrypted = "";
            try
            {
                string presharedKey = airteltigoconfig[0].preSharedKey;
                string phrasetoEncrypt = "7235" + "#" + DateTimeOffset.Now.ToUnixTimeMilliseconds();
                string encryptionAlgorithm = "AES/ECB/PKCS5Padding";


                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aesAlg.CreateEncryptor(Encoding.UTF8.GetBytes(presharedKey), new byte[16]))
                    {
                        byte[] plaintextBytes = Encoding.UTF8.GetBytes(phrasetoEncrypt);
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
                        encrypted = Convert.ToBase64String(encryptedBytes);

                    }
                }
            }
            catch (Exception ex)
            {

                
            }
            
            return encrypted;
        }
        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service_airteltigo, ref List<Logger.Logger.LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };
            
            string userIdentifierType = "", entryChannel = "", subKeyword = "";
            List<Headers> headers = new List<Headers>();


            ServiceClass root_service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
            List<AirtelTigoInfo> airtel_tigo_config = GetAirtelTigoInfoByServiceID(RequestBody.ServiceID,ref lines);

            if (root_service != null)
            {
                Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                if (trans_id > 0)
                {
                    
                userIdentifierType = "MSISDN";
                long productId = airtel_tigo_config[0].product_id_sub;
                int mcc = airtel_tigo_config[0].mcc;
                int mnc = airtel_tigo_config[0].mnc;
                entryChannel = "SMS";
                long largeAccount = airtel_tigo_config[0].largeAccount;
                subKeyword = "WIN";
                headers.Add(new Headers { key = "apikey", value = airtel_tigo_config[0].api_key });
                headers.Add(new Headers { key = "external-tx-id", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                headers.Add(new Headers { key = "authentication", value = GetAuth(airtel_tigo_config) });
              //headers.Add(new Headers { key = "Accept", value = "*/*" });


                string json = "{ \"userIdentifier\": \"" + RequestBody.MSISDN + "\", \"userIdentifierType\": \"" + userIdentifierType + "\", \"productId\": \"" + productId
                                  + "\",\"mcc\": \"" + mcc + "\", \"mnc\": \"0" + mnc + "\", \"entryChannel\": \"" + entryChannel + "\", \"largeAccount\": \"" + largeAccount + "\", \"subKeyword\": \"" + subKeyword + "\", \"clientIp\": \"127.0.0.1\"}";

                    string url = airtel_tigo_config[0].base_url +"/subscription/optin/7200";


                    string error = "";
                    string body = CallSoap.CallSoapRequest(url, json, headers, 5, out error, ref lines);
                    lines = Add2Log(lines, "error = " + error, 100, "Subscribe");

                    //if (error.Contains("You have Already Subscribed Requested Services") || error.Contains("Subscription already exists"))
                    //{
                    //    //insert to sub
                    //    Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),1)", ref lines);
                    //    if (sub_id > 0)
                    //    {
                    //        ret = new SubscribeResponse()
                    //        {
                    //            ResultCode = 1000,
                    //            Description = "User was subscribed"
                    //        };
                    //    }
                    //}
                    
                        if (!String.IsNullOrEmpty(body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {

                                string statusCode = json_response.responseData.subscriptionError;
                                int ResultCode = 1030;
                                string Description = "Subscription Request failed";

                            if (statusCode == "Already Active") 
                            {
                                //insert to sub
                                Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),1)", ref lines);
                                if (sub_id > 0)
                                {
                                    
                                    {
                                        ResultCode = 1000;
                                        Description = "User was subscribed";
                                    };
                                }
                            }
                            else if (statusCode == "Active and Wait Charging")
                            {
                                // Not sure if this is needed here if it is dont forget to add the if sub_id > 0 before resultcode
                                Int64 sub_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),1)", ref lines);
                                if (sub_id > 0) 
                                {
                                    ResultCode = 1010;
                                    Description = "Pending";
                                }
                                    
                            }
                                else
                                {
                                    ResultCode = 1020;
                                    Description = "Subscription failed with the following error " + statusCode;
                                }

                                ret = new SubscribeResponse()
                                {
                                    ResultCode = ResultCode,
                                    Description = Description
                                };
                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                            }

                        }
                    

                }

            }

            return ret;
        }
        public static bool UnSubscribe(SubscribeRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            string existing_sub_query = "SELECT subs.subscriber_id from yellowdot.subscribers subs where subs.service_id = " + RequestBody.ServiceID + " AND subs.msisdn = " + RequestBody.MSISDN + " AND subs.state_id = 1 ORDER BY subs.subscription_date DESC LIMIT 1";

            Int64 existing_subscriber = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(existing_sub_query, ref lines);

            
                ServiceClass root_service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                string userIdentifierType = "", entryChannel = "", subKeyword = "";
                List<Headers> headers = new List<Headers>();
                List<AirtelTigoInfo> airtel_tigo_config = GetAirtelTigoInfoByServiceID(RequestBody.ServiceID, ref lines);



                if (airtel_tigo_config != null)
                {

                    userIdentifierType = "MSISDN";
                    long productId = airtel_tigo_config[0].product_id_sub;
                    int mcc = airtel_tigo_config[0].mcc;
                    int mnc = airtel_tigo_config[0].mnc;
                    entryChannel = "SMS";
                    long largeAccount = airtel_tigo_config[0].largeAccount;
                    subKeyword = "Stop WIN";

                    string json = "{ \"userIdentifier\": \"" + RequestBody.MSISDN + "\", \"userIdentifierType\": \"" + userIdentifierType + "\", \"productId\": \"" + productId
                    + "\",\"mcc\": \"" + mcc + "\", \"mnc\": \"0" + mnc + "\", \"entryChannel\": \"" + entryChannel + "\", \"largeAccount\": \"" + largeAccount + "\", \"subKeyword\": \"" + subKeyword + "\"}";


                    //                {
                    //                    "userIdentifier": "233279798673",
                    //  "userIdentifierType": "MSISDN",
                    //  "productId": "25754",
                    //  "mcc": "620",
                    //  "mnc": "03",
                    //  "entryChannel": "SMS",
                    //  "largeAccount": "798",
                    //  "subKeyword": "WIN"
                    //}
                    if (!String.IsNullOrEmpty(json))
                    {
                        string url = airtel_tigo_config[0].base_url + "/subscription/optout/7200";
                        lines = Add2Log(lines, " Json = " + json, 100, "airteltigo_unsub");
                        headers.Add(new Headers { key = "apikey", value = airtel_tigo_config[0].api_key });
                        headers.Add(new Headers { key = "external-tx-id", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                        headers.Add(new Headers { key = "authentication", value = GetAuth(airtel_tigo_config) });
                        //headers.Add(new Headers { key = "Accept", value = "*/*" });

                        string response_body = Api.CommonFuncations.CallSoap.CallSoapRequest(url, json, headers, 3, true, ref lines);
                        lines = Add2Log(lines, " Response " + response_body, 100, "airteltigo_unsub");
                        if (!String.IsNullOrEmpty(response_body))
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(response_body);

                            try
                            {

                                // string rescode = json_response.service.rescode;
                                string status = json_response.responseData.subscriptionError;
                                if (status == "Optout one success")
                                {
                                    //user was unsubscribed      
                                    result = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                lines = Add2Log(lines, " Exception " + ex.ToString(), 100, "ivr_subscribe");
                            }
                        }

                    }


                }

                return result;
        }
            
    }
}