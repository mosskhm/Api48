using Api.Cache;
using Api.HttpItems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class PISIMobile
    {
        public static string GetBearerToken(ref List<LogLines> lines, out string expiration)
        {
            string bearer = "";
            expiration = "";
            List<Headers> headers = new List<Headers>();
            string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
            string json = "{\"vaspid\": \"" + vas_id + "\"}";
            string url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEURL", ref lines);
            string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            if (!String.IsNullOrEmpty(body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(body);
                try
                {
                    string success = json_response.success;
                    expiration = json_response.expiration;
                    if (success.ToLower() == "true")
                    {
                        String[] spearator2 = { ",\"" };
                        String[] strlist3 = body.Split(spearator2, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in strlist3)
                        {
                            if (s.Contains("pisi-authorization-token"))
                            {
                                bearer = s.Replace("pisi-authorization-token", "").Replace("\"", "").Replace(":", "");
                                break;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }


            return bearer;
        }

        public static bool SendSMS(SendSMSRequest RequestBody, ServiceClass service, long msg_id, ref List<LogLines> lines)
        {
            bool result = false;

            List<PisiMobileNewServiceInfo> pisi_services_new = Api.Cache.Services.GetPisiMobileNewServiceInfo(ref lines);
            if (pisi_services_new != null)
            {
                PisiMobileNewServiceInfo pisi_services_n = pisi_services_new.Find(x => x.service_id == service.service_id);
                if (pisi_services_n != null)
                {
                    result = PISIMobileNew.SendSMS(RequestBody, service, msg_id, ref lines);
                    return result;
                }
            }

            List<PisiMobileServiceInfo> pisi_services = Api.Cache.Services.GetPisiMobileServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {
                    string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
                    string token = Api.Cache.Services.GetPISIBearer(ref lines);
                    string mytransaction_id = Guid.NewGuid().ToString().Replace("-", "");
                    string mystr = RequestBody.Text.Replace("\"", "\'");
                    mystr = mystr.Replace(Environment.NewLine, "\\n");
                    mystr = mystr.Replace("\n", "\\n");
                    string json = "{\"pisisid\": \"" + pisi_service.sms_id + "\",\"msisdn\" : \"" + RequestBody.MSISDN + "\",\"message\": \"" + mystr + "\",\"trxid\": \"" + mytransaction_id + "\"}";
                    List<Headers> headers = new List<Headers>();
                    headers.Add(new Headers { key = "vaspid", value = vas_id });
                    if (!String.IsNullOrEmpty(token))
                    {
                        headers.Add(new Headers { key = "pisi-authorization-token", value = "Bearer " + token });
                        string url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILESENDSMSURL", ref lines);
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string success = json_response.success;
                            if (success.ToLower() == "true")
                            {

                                result = true;
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

        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service, string method, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            try
            {
                List<PisiMobileServiceInfo> pisi_services = Api.DataLayer.DBQueries.GetPisiMobileServiceInfo(ref lines);
                if (pisi_services != null)
                {
                    PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                    if (pisi_service != null)
                    {
                        string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
                        //string token = GetBearerToken(ref lines);
                        string token = Api.Cache.Services.GetPISIBearer(ref lines);
                        if (!String.IsNullOrEmpty(token))
                        {
                            Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                            if (trans_id > 0)
                            {
                                string json = "{\"pisipid\":\"" + pisi_service.subscription_id + "\",\"msisdn\":\"" + RequestBody.MSISDN + "\",\"channel\":\"" + method + "\",\"trxid\":\"" + trans_id + "\"}";
                                List<Headers> headers = new List<Headers>();
                                headers.Add(new Headers { key = "vaspid", value = vas_id });
                                headers.Add(new Headers { key = "pisi-authorization-token", value = "Bearer " + token });
                                try
                                {
                                    // send to PSIS
                                    string url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILESUBURL", ref lines);
                                    string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);

                                    dynamic json_response = JsonConvert.DeserializeObject(body);
                                    if (json_response != null)
                                    {
                                        string success = json_response.success;
                                        if (success.ToLower() == "true")
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
                                                Description = "Subscription Request failed"
                                            };
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " PISI Subscribe FAILED = " + ex.ToString(), 100, lines[0].ControlerName);
            }
            return ret;
        }

        public static SubscribeResponse Unsubscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Unsubscription Request failed"
            };

            List<PisiMobileServiceInfo> pisi_services = Api.DataLayer.DBQueries.GetPisiMobileServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {
                    string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
                    //string token = GetBearerToken(ref lines);
                    string token = Api.Cache.Services.GetPISIBearer(ref lines);
                    if (!String.IsNullOrEmpty(token))
                    {
                        Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                        if (trans_id > 0)
                        {
                            string json = "{\"pisipid\":\"" + pisi_service.subscription_id + "\",\"msisdn\":\"" + RequestBody.MSISDN + "\",\"channel\":\"WEB\",\"trxid\":\"" + trans_id + "\"}";
                            
                            List<Headers> headers = new List<Headers>();
                            headers.Add(new Headers { key = "vaspid", value = vas_id });
                            headers.Add(new Headers { key = "pisi-authorization-token", value = "Bearer " + token });
                            string url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILESUBURL", ref lines);
                            string body = CallSoap.CallSoapRequestWithMethod(url, json, headers, 4,"DELETE", ref lines);
                            dynamic json_response = JsonConvert.DeserializeObject(body);
                            try
                            {
                                string success = json_response.success;
                                if (success.ToLower() == "true")
                                {
                                    ret = new SubscribeResponse()
                                    {
                                        ResultCode = 1010,
                                        Description = "Unsubscription Request was sent"
                                    };
                                }
                                else
                                {
                                    ret = new SubscribeResponse()
                                    {
                                        ResultCode = 1020,
                                        Description = "Unsubscription Request failed"
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
            }
            return ret;
        }

        public static DYAReceiveMoneyResponse ReceiveMoney(DYAReceiveMoneyRequest RequestBody, string dya_id, ServiceClass service, string datetime, ref List<LogLines> lines)
        {
            DYAReceiveMoneyResponse ret = new DYAReceiveMoneyResponse()
            {
                ResultCode = 1010,
                Description = "Pending.",
                TransactionID = dya_id,
                Timestamp = datetime
            };


            List<PisiMobileServiceInfo> pisi_services = Api.DataLayer.DBQueries.GetPisiMobileServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {
                    string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
                    //string token = GetBearerToken(ref lines);
                    string token = Api.Cache.Services.GetPISIBearer(ref lines);
                    if (!String.IsNullOrEmpty(token))
                    {
                        string json = "{\"pisipid\":\"" + pisi_service.subscription_id + "\",\"msisdn\":\"" + RequestBody.MSISDN + "\",\"channel\":\"USSD\",\"trxid\":\"" + dya_id + "\",\"amount\":\"" + RequestBody.Amount + "\"}";
                        List<Headers> headers = new List<Headers>();
                        headers.Add(new Headers { key = "vaspid", value = vas_id });
                        headers.Add(new Headers { key = "pisi-authorization-token", value = "Bearer " + token });
                        string url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILECHARGEURL", ref lines);
                        string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
                        dynamic json_response = JsonConvert.DeserializeObject(body);
                        try
                        {
                            string success = json_response.success;
                            if (success.ToLower() == "true")
                            {
                                ret = new DYAReceiveMoneyResponse()
                                {
                                    ResultCode = 1010,
                                    Description = "Pending"
                                };
                            }
                            else
                            {
                                ret = new DYAReceiveMoneyResponse()
                                {
                                    ResultCode = 1020,
                                    Description = "Failed"
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

        public static List<PriceClass> GetPrices(ref List<LogLines> lines)
        {
            List<PriceClass> result = null;
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(ServerSettings.GetServerSettings("DBConnectionString", ref lines));
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select p.price_id, p.service_id, p.price, p.curency_code from prices p;";
                command.CommandTimeout = 600;
                reader = command.ExecuteReader();

                if (reader.HasRows == true)
                {
                    result = new List<PriceClass>();
                    while (reader.Read())
                    {
                        result.Add(new PriceClass
                        {
                            price_id = reader.GetInt32(0),
                            service_id = reader.GetInt32(1),
                            price = reader.GetDouble(2),
                            curency_code = reader.GetString(3)
                        });
                    }
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                lines = Add2Log(lines, "ErrorCode=" + ex.ErrorCode + ", InnerException=" + ex.InnerException + ", Message=" + ex.Message, 100, lines[0].ControlerName);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }

            }
            return result;
        }

        public static BillResponse Bill(BillRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            BillResponse ret = new BillResponse()
            {
                ResultCode = 1030,
                Description = "Billing Request failed"
            };

            List<PriceClass> price_info = GetPrices(ref lines);
            PriceClass p = price_info.Find(x => x.price_id == RequestBody.PriceID);
            List<PisiMobileServiceInfo> pisi_services = Api.DataLayer.DBQueries.GetPisiMobileServiceInfo(ref lines);
            if (pisi_services != null)
            {
                PisiMobileServiceInfo pisi_service = pisi_services.Find(x => x.service_id == service.service_id);
                if (pisi_service != null)
                {
                    string vas_id = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILEVASID", ref lines);
                    string token = Api.Cache.Services.GetPISIBearer(ref lines);
                    if (!String.IsNullOrEmpty(token))
                    {
                        Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                        if (trans_id > 0)
                        {

                            string json = "{ \"pisipid\": \""  + pisi_service.subscription_id + "\", \"msisdn\": \"" + RequestBody.MSISDN + "\", \"channel\": \"USSD\",\"trxid\": \"" + trans_id + "\", \"amount\": \"" + p.price + "\"}";
                            string bill_url = Api.Cache.ServerSettings.GetServerSettings("PISIMOBILECHARGEURL", ref lines);
                            List<Headers> headers = new List<Headers>();
                            headers.Add(new Headers { key = "vaspid", value = vas_id });
                            headers.Add(new Headers { key = "PISI-AUTHORIZATION-TOKEN", value = "Bearer " + token });
                            string error = "";
                            string body = CallSoap.CallSoapRequest(bill_url, json, headers, 4, out error, ref lines);
                            lines = Add2Log(lines, "error = " + error, 100, "Subscribe");


                            if (!String.IsNullOrEmpty(body) && String.IsNullOrEmpty(error))
                            {
                                dynamic json_response = JsonConvert.DeserializeObject(body);
                                try
                                {

                                    string statusCode = json_response.statusCode;
                                    int ResultCode = 1030;
                                    string Description = "Subscription Request failed";

                                    if (statusCode == "1000")
                                    {
                                        ResultCode = 1010;
                                        Description = "Pending";
                                    }
                                    else
                                    {
                                        ResultCode = 1020;
                                        Description = "Billing failed with the following error " + statusCode;
                                    }

                                    ret = new BillResponse()
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
                            if (!String.IsNullOrEmpty(error))
                            {
                                dynamic json_response = JsonConvert.DeserializeObject(error);
                                try
                                {

                                    string statusCode = json_response.error;
                                    int ResultCode = 1030;
                                    string Description = "Subscription Request failed";

                                    if (statusCode == "0000")
                                    {
                                        ResultCode = 1010;
                                        Description = "Pending";
                                    }
                                    else
                                    {
                                        ResultCode = 1020;
                                        Description = "Billing failed with the following error " + statusCode;
                                    }

                                    ret = new BillResponse()
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
                }
            }
            return ret;
        }
    }
}
