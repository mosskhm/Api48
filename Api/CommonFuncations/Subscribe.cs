using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using Api.Logger;
using static Api.Cache.Services;
using static Api.Logger.Logger;
using static Api.CommonFuncations.Amplitude;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Security.Policy;

namespace Api.CommonFuncations
{
    public class Subscribe
    {
        public static string BuildSubscribeSoap(string msisdn, string sp_id, string password, string token, string channel_id, string product_id, string service_id, string operator_id)
        {

            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            
            string final_password = md5.Encode_md5(sp_id + password + timeStamp).ToUpper();
            string soap = "";
            
                soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
                soap = soap + "<soapenv:Header>";
                soap = soap + "<RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                soap = soap + "<spId>" + sp_id + "</spId>";
                soap = soap + "<spPassword>" + final_password + "</spPassword>";
                soap = soap + "<timeStamp>" + timeStamp + "</timeStamp>";
                if (!String.IsNullOrEmpty(service_id) && String.IsNullOrEmpty(token))
                {
                    soap = soap + "<serviceId>" + service_id + "</serviceId>";
                }

                if (!String.IsNullOrEmpty(token))
                {
                    soap = soap + "<oauth_token>" + token + "</oauth_token>";
                }

                //soap = soap + "<oauth_token>" + token+ "</oauth_token>";
                soap = soap + "<OA>" + msisdn + "</OA>";
                soap = soap + "<FA>" + msisdn + "</FA>";
                soap = soap + "</RequestSOAPHeader>";
                soap = soap + "</soapenv:Header>";
                soap = soap + "<soapenv:Body>";
                soap = soap + "<subscribeProductRequest xmlns=\"http://www.csapi.org/schema/parlayx/subscribe/manage/v1_0/local\">";
                soap = soap + "<subscribeProductReq>";
                soap = soap + "<userID xmlns=\"\">";
                soap = soap + "<ID>" + msisdn + "</ID>";
                soap = soap + "<type>0</type>";
                soap = soap + "</userID>";
                soap = soap + "<subInfo xmlns=\"\">";
                if (product_id != "0")
                {
                    soap = soap + "<productID>" + product_id + "</productID>";
                }
                soap = soap + "<channelID>1</channelID>";
                soap = soap + "<extensionInfo>";
                soap = soap + "<namedParameters>";
                soap = soap + "<key>SubType</key>";
                soap = soap + "<value>0</value>";
                soap = soap + "</namedParameters>";
                soap = soap + "</extensionInfo>";
                soap = soap + "</subInfo>";
                soap = soap + "</subscribeProductReq>";
                soap = soap + "</subscribeProductRequest>";
                soap = soap + "</soapenv:Body>";
                soap = soap + "</soapenv:Envelope>";
                
            return soap;

        }

        public static bool ValidateRequest1(SubscribeRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
	        bool result = false;
	        int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
	        if (RequestBody != null)
	        {
		        string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID +
		                      ", Token = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.TransactionID;
		        lines = Add2Log(lines, text, log_level, "Subscribe");
				logMessages.Add(new
		        {
			        message = text, msisdn = RequestBody.MSISDN, application = app_name, environment = "production",
			        level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id
		        });

                // check all the paramaters are there
                result = (RequestBody.MSISDN > 0) &&
                         (RequestBody.ServiceID > 0) &&
                         (!String.IsNullOrEmpty(RequestBody.TokenID)) &&
                         (!String.IsNullOrEmpty(RequestBody.TransactionID))
                         ;

                // report it something is missing
		        if (!result)
		        {
			        lines = Add2Log(lines, "Bad Params", log_level, "");

			        logMessages.Add(new
			        {
				        message = "Bad Params",
				        msisdn = RequestBody.MSISDN,
				        application = app_name,
				        environment = "production",
				        level = "INFO",
				        timestamp = DateTime.UtcNow,
				        logz_id = logz_id
			        });
				}
	        }

	        return result;
        }

        public static SubscribeResponse DoSubscribe(SubscribeRequest RequestBody)
        {
            SubscribeResponse ret = new SubscribeResponse() { ResultCode = 1050, Description = "Subscription Request has failed"};
            List<LogLines> lines = new List<LogLines>();

            string logName = "Subscribe_" + ( RequestBody == null || RequestBody.ServiceID == 0  ? "undefined" : RequestBody.ServiceID.ToString() );
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), logName);

            try
            {
                var logMessages = new List<object>();
                var logz_id = Guid.NewGuid().ToString().Replace("-", "");
                var app_name = "Subscribe";
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                string sub_response = "";
                if (ValidateRequest1(RequestBody, ref lines, ref logMessages, app_name, logz_id))
                {
                    DLValidateSMS result = null;
                    if (!String.IsNullOrEmpty(RequestBody.EncryptedMSISDN))
                    {
                        result = DBQueries.ValidateSubscribeRequestWithEncMSISDN(RequestBody, 1, ref lines);
                    }
                    else
                    {
                        result = DBQueries.ValidateSubscribeRequest(RequestBody, ref lines);
                    }

                    if (result == null)
                    {
                        ret = new SubscribeResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        string service_id = (result.subscribe_wo_service_id == true ? "" : result.RealServiceID.ToString());
                        ServiceClass myservice = GetServiceByServiceID(RequestBody.ServiceID, ref lines, ref logMessages, app_name, logz_id);
                        if (myservice.is_ondemand && result.RetCode != 1000) result.RetCode = 1000; // make sure OnDemand doesn't fail the next step

                        if (result.RetCode != 1000)         // validate subscribe request failed
                        {
                            ret = new SubscribeResponse()
                            {
                                ResultCode = result.RetCode,
                                Description = result.Description
                            };
                        }
                        else                                // validate subscribe request was successful
                        {
                            string auth_token = (String.IsNullOrEmpty(RequestBody.AuthrizationID) ? "" : RequestBody.AuthrizationID);
                            string activation_id = "1";
                            if (!String.IsNullOrEmpty(RequestBody.ActivationID))
                            {
                                activation_id = RequestBody.ActivationID;
                            }

                            lines = Add2Log(lines, $"Subscribing to serviceid={service_id} (for requested ServiceID={RequestBody.ServiceID}, operatorID={result.operator_id} ({result.OperatorName}), country={result.CountryName}", 100, "");
                            switch (result.operator_id)
                            {
                                case 16:
                                    ServiceClass service_sts = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    if (RequestBody.ActivationID == "2")
                                    {
                                        string sts_url = STS.CreateURL(service_sts, RequestBody, ref lines);
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1302,
                                            Description = "Redirect user to URL",
                                            URL = sts_url
                                        };
                                    }
                                    else
                                    {
                                        bool sub_res = STS.SubscribeUser(service_sts, RequestBody, ref lines);
                                        if (sub_res)
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1000,
                                                Description = "Subscription Request was ok"
                                            };
                                        }
                                        else
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1020,
                                                Description = "Subscription Request has failed"
                                            };
                                        }
                                    }
                                    break;     // South Africa - STS
                                case 1:
                                    bool sub_result = IMI.Subscribe(RequestBody.MSISDN.ToString(), RequestBody.ServiceID.ToString(), ref lines);
                                    if (sub_result)
                                    {
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "Subscription Request was ok"
                                        };
                                    }
                                    else
                                    {
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1020,
                                            Description = "Subscription Request has failed"
                                        };
                                    }

                                    break;      // South Africa - MTN
                                //case 23:
                                //    ServiceClass service_m3 = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                //    ret = MADAPI.SubscribeV3(RequestBody, service_m3, ref lines);
                                //    break;
                                case 23:        // MADAPI
                                case 27:        // Ghana - MTN
                                case 4:         // Cameroon - MTN
                                case 3:
                                    ServiceClass service_m = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    ret = MADAPI.Subscribe(RequestBody, service_m, ref lines);
                                    break;      // Ivory Coast - MTN
                                case 18:        // Cameroon - Orange
                                case 32:        // Ivory Coast - Orange

                                    // insert subscribers
                                    Int64 subscriberId = DBQueries.InsertSub(RequestBody.MSISDN.ToString(), myservice.service_id.ToString(), 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "3", "", ref lines);

                                    if (subscriberId == 0)
                                    {
                                        lines = Add2Log(lines, $"Ivory Coast - Orange - failed to subscribe to serviceID = {myservice.service_name} - subscriberID={subscriberId}", 100, "");
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1030,
                                            Description = "Failed to subscribe to service"
                                        };
                                    }
                                    else if (myservice.subscription_amount == 0)
                                    {
                                        lines = Add2Log(lines, $"Ivory Coast - Orange - failed to subscribe to serviceID = {myservice.service_name} - amount={myservice.subscription_amount}, subscriberID={subscriberId}", 100, "");
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Failed to bill for service - no subscription_amount defined"
                                        }; 
                                    }
                                    else
                                    {
                                        // send a notice users
                                        Orange.SendSMSKannel(RequestBody.MSISDN.ToString()
                                            , $"Bienvenue à Num d'OR ! Vous serez automatiquement débité(e) de {myservice.subscription_amount} F chaque jour pour participer au tirage. * Pour en savoir plus, visitez http://icorgtwn.ydot.co, composez 590#5, ou envoyez HELP par SMS au 7717."
                                            , false
                                            , "subscribe_" + service_id + "_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                                            , service_id
                                            , ref lines
                                        );

                                        // attempt to bill this user directly
                                        var bill = Orange.billIvoryCoastAirTime(RequestBody.MSISDN.ToString(), myservice.subscription_amount, subscriberId, myservice.service_id, ref lines);
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = bill.ResultCode,
                                            Description = bill.Description
                                        };
                                        lines = Add2Log(lines, $"Ivory Coast - Orange - attempting to BILL subscriberID={subscriberId} for serviceID = {myservice.service_name}, amount={myservice.subscription_amount}, result={bill.Description}", 100, "");

                                    }
                                    break;   
                                    
                                case 29:
                                    ServiceClass service_airteltigo = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    ret = airteltigo.Subscribe(RequestBody, service_airteltigo, ref lines);
                                    break;     // Ghana - AirTel 
                                case 12:
                                    ServiceClass service1 = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    if (service1.use_fulfilment == true)
                                    {
                                        string errmsg = "";
                                        bool full_res = Fulfillment.CallFulfillment(service1, RequestBody.MSISDN.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true, ref lines, out errmsg);
                                        if (full_res)
                                        {
                                            int price_id = 0;
                                            switch (service1.service_id)
                                            {
                                                case 669:
                                                    price_id = 1853;
                                                    break;
                                                case 697:
                                                    price_id = 1852;
                                                    break;
                                                case 698:
                                                    price_id = 1855;
                                                    break;
                                            }
                                            Int64 sub_id = Api.DataLayer.DBQueries.InsertSub(RequestBody.MSISDN.ToString(), service1.service_id.ToString(), price_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "3", "", ref lines);
                                            if (sub_id > 0)
                                            {
                                                ServiceBehavior.DecideBehavior(service1, "1", RequestBody.MSISDN.ToString(), sub_id, ref lines);
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1000,
                                                    Description = "Subscription was successful"
                                                };
                                            }
                                            else
                                            {
                                                full_res = Fulfillment.CallFulfillment(service1, RequestBody.MSISDN.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false, ref lines, out errmsg);
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1020,
                                                    Description = "Subscription Request has failed"
                                                };
                                            }
                                        }
                                        else
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1020,
                                                Description = "Subscription Request has failed"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        if (RequestBody.GetURL == "1")
                                        {

                                            string url1 = SDPWap.CreateWapPushURL(service1, RequestBody.MSISDN.ToString(), ref lines);
                                            if (!String.IsNullOrEmpty(url1))
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1302,
                                                    Description = "Redirect user to URL",
                                                    URL = url1
                                                };
                                            }
                                            else
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1020,
                                                    Description = "Subscription Request has failed"
                                                };
                                            }
                                        }
                                        else
                                        {
                                            string soap1 = BuildSubscribeSoap(RequestBody.MSISDN.ToString(), result.SPID.ToString(), result.Password, auth_token, activation_id, result.RealProductID.ToString(), service_id, result.operator_id.ToString());
                                            lines = Add2Log(lines, "Subscribe Soap = " + soap1, 100, "Subscribe");

                                            string sdp_string1 = "SDPSubscribeURL_" + result.operator_id + (result.is_staging == true ? "_STG" : "");

                                            string soap_url1 = Cache.ServerSettings.GetServerSettings(sdp_string1, ref lines);
                                            lines = Add2Log(lines, "soap_url = " + soap_url1, 100, "Subscribe");
                                            string response1 = CommonFuncations.CallSoap.CallSoapRequest(soap_url1, soap1, ref lines);
                                            lines = Add2Log(lines, "Subscribe Response = " + response1, 100, "Subscribe");
                                            if (response1 != "")
                                            {
                                                sub_response = CommonFuncations.ProcessXML.GetXMLNode(response1, "resultDescription", ref lines);
                                                if (sub_response.Contains("Temporary Order saved successfully!") || sub_response == "Success")
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
                                                        Description = "Subscription Request has failed"
                                                    };
                                                }
                                            }
                                            else
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1030,
                                                    Description = "Subscription Request has failed - Network problem"
                                                };
                                            }
                                        }
                                    }



                                    break;     // Benin - MTN
                                case 9: // South Africa - Vodacom
                                        //string emsisdn = CommonFuncations.Vodacom.valitadeMSISDN(true, RequestBody.MSISDN, ref lines);
                                    ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT v.encrypted_msisdn from vod_enrich v WHERE v.msisdn = " + RequestBody.MSISDN + " ORDER BY v.id DESC LIMIT 1", ref lines);
                                    Vodacom.VodSub vod_sub = Vodacom.GetServiceOffers(service, RequestBody.MSISDN.ToString(), result, enc_msisdn, ref lines);
                                    string url = "";//VodacomIMI.IdentifySubscribe(service, RequestBody.MSISDN.ToString(), "", ref lines);
                                    if (vod_sub == null) //error on GetServiceOffers
                                    {
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1020,
                                            Description = "Subscription Request has failed"
                                        };
                                    }
                                    else
                                    {
                                        if (vod_sub.subscription_state == "") //user is not subscribed
                                        {
                                            url = Vodacom.CreateChargeURL(vod_sub, RequestBody.MSISDN.ToString(), RequestBody.TransactionID, ref lines);
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1302,
                                                Description = "Redirect user to URL",
                                                URL = url
                                            };
                                        }
                                        else //need to add user to subscribers
                                        {
                                            if (vod_sub.subscription_state == "1" || vod_sub.subscription_state == "5")
                                            {
                                                DataLayer.DBQueries.ExecuteQuery("insert into subscribers (msisdn, service_id, subscription_date, state_id, subscription_method_id) values(" + RequestBody.MSISDN + ", " + RequestBody.ServiceID + ", now(), 1, 0)", ref lines);
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 3011,
                                                    Description = "User is already subscribed"
                                                };
                                            }
                                            else
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1020,
                                                    Description = "Subscription Request has failed"
                                                };
                                            }

                                        }
                                    }
                                    break;      // South Africa - Vodacom
                                case 8:
                                    service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    string smsrecive_url = Cache.ServerSettings.GetServerSettings("NineMobileSMSReceiveURL", ref lines) + "from=" + RequestBody.MSISDN + "&to=" + service.sms_mt_code + "&text=" + service.service_name;
                                    string get_response = CallSoap.GetURL(smsrecive_url, ref lines);
                                    if (get_response.Contains("OK"))
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
                                            Description = "Subscription Request has failed"
                                        };
                                    }

                                    break;      // Nigeria - 9-mobile
                                case 25:    // South Africa - cellC 
                                    ServiceClass service_cellc = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    ret = CellC.Subscribe(RequestBody, service_cellc, ref lines);
                                    break;     // South Africa - Cell C
                                case 26:        // Nigeria - Airtel
                                case 10:        // Nigeria - Glo
                                case 2:     // Nigeria - MTN
                                            //ServiceClass service_nj = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                            //ret = NJNG.Subscribe(RequestBody, service_nj, ref lines);
                                    ServiceClass service_p = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                    // DEC2024 - change back to the OLD api method
                                    // ret = PISIMobileNew.Subscribe(RequestBody, service_p, ref lines);
                                    ret = PISIMobile.Subscribe(RequestBody, service_p, "USSD", ref lines);

                                    break;      // Nigeria - MTN

                                default: //SDP
                                    if (RequestBody.GetURL == "1")
                                    {
                                        ServiceClass service2 = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                        string url1 = SDPWap.CreateWapPushURLNigeria(service2, RequestBody.MSISDN.ToString(), ref lines);
                                        if (!String.IsNullOrEmpty(url1))
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1302,
                                                Description = "Redirect user to URL",
                                                URL = url1
                                            };
                                        }
                                        else
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1020,
                                                Description = "Subscription Request has failed"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        string soap = BuildSubscribeSoap(RequestBody.MSISDN.ToString(), result.SPID.ToString(), result.Password, auth_token, activation_id, result.RealProductID.ToString(), service_id, result.operator_id.ToString());
                                        lines = Add2Log(lines, "Subscribe Soap = " + soap, 100, "Subscribe");
                                        logMessages.Add(new { message = "Subscribe Soap = " + soap, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });

                                        string sdp_string = "SDPSubscribeURL_" + result.operator_id + (result.is_staging == true ? "_STG" : "");

                                        string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                                        lines = Add2Log(lines, "soap_url = " + soap_url, 100, "Subscribe");
                                        logMessages.Add(new { message = "soap_url = " + soap_url, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });

                                        string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                                        lines = Add2Log(lines, "Subscribe Response = " + response, 100, "Subscribe");
                                        logMessages.Add(new { message = "Subscribe Response = " + response, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });

                                        if (response != "")
                                        {
                                            sub_response = CommonFuncations.ProcessXML.GetXMLNode(response, "resultDescription", ref lines);
                                            if (sub_response.Contains("Temporary Order saved successfully!"))
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1010,
                                                    Description = "Subscription Request was sent"
                                                };
                                            }
                                            if (sub_response == "Success")
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1000,
                                                    Description = "Success"
                                                };
                                            }
                                            if (ret == null && !String.IsNullOrEmpty(sub_response))
                                            {
                                                ret = new SubscribeResponse()
                                                {
                                                    ResultCode = 1020,
                                                    Description = "Subscription Request has failed " + sub_response
                                                };
                                            }
                                        }
                                        else
                                        {
                                            ret = new SubscribeResponse()
                                            {
                                                ResultCode = 1030,
                                                Description = "Subscription Request has failed - Network problem"
                                            };
                                        }
                                    }

                                    break;
                            }
                        }
                    }
                }

                if (ret == null) lines = Add2Log(lines, "!!! SubscribeResponse (ret) is NULL !!", 100, "");
                else
                {

                    string text =   $"RetCode = {ret.ResultCode},"
                                  + $"Description = {ret.Description},"
                                  + $"URL = {ret.URL}"
                                  ;

                    lines = Add2Log(lines, text, log_level, "");

                    TaskManager.ExecuteInAnotherThread(() =>
                    {
                        logMessages.Add(new { message = text, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                        LogzIOHelper.SendLogsAsync(logMessages, "API");
                    }
                    );

                    TaskManager.ExecuteInAnotherThread(() =>
                    {
                        string mysqlQ = "insert into subscription_requests (msisdn, service_id, date_time, result, response, subscriber_id, transaction_id) values (" + RequestBody.MSISDN + ", " + RequestBody.ServiceID + ", now(), " + ret.ResultCode + ", '" + sub_response.Replace("'", "''") + "', 0, '" + RequestBody.TransactionID + "')";
                        Api.DataLayer.DBQueries.ExecuteQuery(mysqlQ, ref lines);
                    }
                    );

                    TaskManager.WaitForAllTasksToFinish();      // pause to wait for everything to close off
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!!! DoSubscribe execption: {ex.ToString()}", 100, "");
            }
            lines = Write2Log(lines);
            return ret;
        }
    }
}