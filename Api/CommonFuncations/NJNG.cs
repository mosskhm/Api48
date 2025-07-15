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
    public class NJNG
    {
        public static bool SendSMS(SendSMSRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            bool result = false;
            string msg_type = (RequestBody.IsFlash == "1" ? "FLASH" : "SMS");
            List<NJService> nj_services = GetAllNJServiceInfo(ref lines);
            if (nj_services != null)
            {
                var nj_service = nj_services.Find(x => x.service_id == service.service_id);
                if (nj_service != null)
                {
                    string json = "{\"SrcAddr\" : \"" + service.sms_mt_code + "\",\"DestAddr\" : \"" + RequestBody.MSISDN + "\",\"ServiceID\" : \""+ nj_service.nj_service_id+ "\",\"Message\" : \"" + RequestBody.Text + "\",\"msgtype\" : \"" + msg_type + "\"}";
                    string url = Api.Cache.ServerSettings.GetServerSettings("NJBaseURL", ref lines) + "/sendsms";
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "CPID", value = Api.Cache.ServerSettings.GetServerSettings("CPID", ref lines) });
                    headers.Add(new Headers { key = "X-TOKEN", value = Api.Cache.ServerSettings.GetServerSettings("NJAPIKEY", ref lines) });
                    string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            bool status = json_response.status;
                            result = status;
                        }
                        catch (Exception ex)
                        {
                            lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                        }

                    }
                }
            }
            
            
            return result;
        }

        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            List<NJService> nj_services = GetAllNJServiceInfo(ref lines);
            if (nj_services != null)
            {
                var nj_service = nj_services.Find(x => x.service_id == service.service_id);
                if (nj_service != null)
                {
                    string json = "{\"msisdn\" : \"" + RequestBody.MSISDN + "\",\"serviceid\" : \"" + nj_service.nj_service_id + "\",\"channel\" : \"IVR\"}";
                    string url = Api.Cache.ServerSettings.GetServerSettings("NJBaseURL", ref lines) + "/subscribe";
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "CPID", value = Api.Cache.ServerSettings.GetServerSettings("CPID", ref lines) });
                    headers.Add(new Headers { key = "X-TOKEN", value = Api.Cache.ServerSettings.GetServerSettings("NJAPIKEY", ref lines) });
                    string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            bool status = json_response.status;
                            string message = json_response.message;
                            if (status && message.Contains("Temporary Order saved successfully"))
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Subscription Request was sent"
                                };
                            }
                            else
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1020,
                                    Description = "Subscription Request failed - " + message
                                };
                            }
                            
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


        public static SubscribeResponse UnSubscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "UnSubscription Request failed"
            };

            List<NJService> nj_services = GetAllNJServiceInfo(ref lines);
            if (nj_services != null)
            {
                var nj_service = nj_services.Find(x => x.service_id == service.service_id);
                if (nj_service != null)
                {
                    string json = "{\"msisdn\" : \"" + RequestBody.MSISDN + "\",\"serviceid\" : \"" + nj_service.nj_service_id + "\",\"channel\" : \"IVR\"}";
                    string url = Api.Cache.ServerSettings.GetServerSettings("NJBaseURL", ref lines) + "/unsubscribe";
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "CPID", value = Api.Cache.ServerSettings.GetServerSettings("CPID", ref lines) });
                    headers.Add(new Headers { key = "X-TOKEN", value = Api.Cache.ServerSettings.GetServerSettings("NJAPIKEY", ref lines) });
                    string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                    if (!String.IsNullOrEmpty(body))
                    {
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            bool status = json_response.status;
                            string message = json_response.message;
                            if (status)
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "UnSubscription was successful"
                                };
                            }
                            else
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1020,
                                    Description = "Subscription Request failed - " + message
                                };
                            }

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
    }
}