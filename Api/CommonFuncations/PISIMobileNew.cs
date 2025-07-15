using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class PISIMobileNew
    {

        public static string Login(int service_id, string password, ref List<LogLines> lines)
        {
            string token_id = "";
            LoginRequest RequestBody = new LoginRequest()
            {
                ServiceID = service_id,
                Password = password
            };
            string json = JsonConvert.SerializeObject(RequestBody);
            List<Headers> headers = new List<Headers>();
            string url = Api.Cache.ServerSettings.GetServerSettings("PISIMobileNewLoginURL", ref lines);
            string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            dynamic json_response = JsonConvert.DeserializeObject(body);
            try
            {
                string result_code = json_response.ResultCode;
                string description = json_response.Description;
                string token = json_response.TokenID;
                if (result_code == "1000")
                {
                    token_id = token;
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
            }

            return token_id;
        }

        public static bool SendSMS(SendSMSRequest RequestBody, ServiceClass service, Int64 msg_id, ref List<LogLines> lines)
        {
            bool result = false;

            List<PisiMobileNewServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileNewServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileNewServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {

                    string token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, false, ref lines);
                    string mystr = RequestBody.Text.Replace("\"", "\'");
                    mystr = mystr.Replace(Environment.NewLine, "\\n");
                    mystr = mystr.Replace("\n", "\\n");
                    mystr = mystr.Replace("\'", "");

                    SendSMSRequest RequestBodyPisi = new SendSMSRequest()
                    {
                        MSISDN = RequestBody.MSISDN,
                        Text = mystr,
                        TokenID = token,
                        ServiceID = pisi_service.pisi_service_id,
                        TransactionID = msg_id.ToString()
                    };



                    string json = JsonConvert.SerializeObject(RequestBodyPisi);
                    List<Headers> headers = new List<Headers>();
                    if (!String.IsNullOrEmpty(token))
                    {
                        string url = Api.Cache.ServerSettings.GetServerSettings("PISIMobileNewSendSMSURL", ref lines);
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string result_code = json_response.ResultCode;
                            string description = json_response.Description;
                            if (result_code == "1000" || result_code == "1010")
                            {
                                result = true;
                            }
                            if (result_code == "2000" || result_code == "2001" || result_code == "2002")
                            {
                                token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, true, ref lines);
                                bool send_sms = Api.CommonFuncations.PISIMobileNew.SendSMS(RequestBody, service, msg_id, ref lines);
                            }

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
            List<PisiMobileNewServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileNewServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileNewServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {

                    string token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, false, ref lines);
                    Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                    string activation_channel = (String.IsNullOrEmpty(RequestBody.ActivationID) ? "API" : RequestBody.ActivationID);
                    SubscribeRequest RequestBodyPisi = new SubscribeRequest()
                    {
                        MSISDN = RequestBody.MSISDN,
                        TokenID = token,
                        ServiceID = pisi_service.pisi_service_id,
                        TransactionID = trans_id.ToString(),
                        ActivationID = activation_channel

                    };



                    string json = JsonConvert.SerializeObject(RequestBodyPisi);
                    List<Headers> headers = new List<Headers>();
                    if (!String.IsNullOrEmpty(token))
                    {
                        string url = Api.Cache.ServerSettings.GetServerSettings("PISIMobileNewSubscribeURL", ref lines);
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string result_code = json_response.ResultCode;
                            string description = json_response.Description;
                            if (result_code == "1000" || result_code == "1010")
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = Convert.ToInt32(result_code),
                                    Description = description
                                };
                            }
                            if (result_code == "2000" || result_code == "2001")
                            {
                                token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, true, ref lines);
                                ret = Api.CommonFuncations.PISIMobileNew.Subscribe(RequestBody, service, ref lines);
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
            List<PisiMobileNewServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileNewServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileNewServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {

                    string token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, false, ref lines);
                    Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                    string activation_channel = (String.IsNullOrEmpty(RequestBody.ActivationID) ? "API" : RequestBody.ActivationID);
                    SubscribeRequest RequestBodyPisi = new SubscribeRequest()
                    {
                        MSISDN = RequestBody.MSISDN,
                        TokenID = token,
                        ServiceID = pisi_service.pisi_service_id,
                        TransactionID = trans_id.ToString(),
                        ActivationID = activation_channel

                    };



                    string json = JsonConvert.SerializeObject(RequestBodyPisi);
                    List<Headers> headers = new List<Headers>();
                    if (!String.IsNullOrEmpty(token))
                    {
                        string url = Api.Cache.ServerSettings.GetServerSettings("PISIMobileNewUnSubscribeURL", ref lines);
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string result_code = json_response.ResultCode;
                            string description = json_response.Description;
                            if (result_code == "1000" || result_code == "1010")
                            {
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = Convert.ToInt32(result_code),
                                    Description = description
                                };
                            }
                            if (result_code == "2000" || result_code == "2001")
                            {
                                token = Api.Cache.Services.GetPisiMobileNewServiceTokenID(pisi_service.pisi_service_id, pisi_service.password, true, ref lines);
                                ret = Api.CommonFuncations.PISIMobileNew.Subscribe(RequestBody, service, ref lines);
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