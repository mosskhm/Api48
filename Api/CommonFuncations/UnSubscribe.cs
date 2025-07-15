using Api.DataLayer;
using Api.HttpItems;
using Api.Logger;
using Microsoft.Web.Services2.Referral;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.CommonFuncations.Amplitude;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class UnSubscribe
    {
        public static string BuildUnSubscribeSoap(string msisdn, string sp_id, string password, string token, string channel_id, string product_id)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(sp_id + password + timeStamp).ToUpper();

            string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/subscribe/manage/v1_0/local\">";
            soap = soap + "<soapenv:Header>";
            soap = soap + "<tns:RequestSOAPHeader xmlns:tns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "<tns:spId>" + sp_id + "</tns:spId>";
            soap = soap + "<tns:spPassword>" + final_password + "</tns:spPassword>";
            soap = soap + "<tns:timeStamp>" + timeStamp + "</tns:timeStamp>";
            soap = soap + "</tns:RequestSOAPHeader>";
            soap = soap + "</soapenv:Header>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<loc:unSubscribeProductRequest>";
            soap = soap + "<loc:unSubscribeProductReq>";
            soap = soap + "<userID>";
            soap = soap + "<ID>" + msisdn + "</ID>";
            soap = soap + "<type>0</type>";
            soap = soap + "</userID>";
            soap = soap + "<subInfo>";
            soap = soap + "<productID>" + product_id + "</productID>";
            //soap = soap + "<operCode>zh</operCode>";
            soap = soap + "<isAutoExtend>0</isAutoExtend>";
            soap = soap + "<channelID>2</channelID>";
            soap = soap + "<extensionInfo>";
            soap = soap + "<namedParameters>";
            soap = soap + "<key>keyword</key>";
            soap = soap + "<value>unsub</value>";
            soap = soap + "</namedParameters>";
            soap = soap + "</extensionInfo>";
            soap = soap + "</subInfo>";
            soap = soap + "</loc:unSubscribeProductReq>";
            soap = soap + "</loc:unSubscribeProductRequest>";
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
                string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID + ", Token = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.TransactionID;

				logMessages.Add(new
				{
					message = text,
					msisdn = RequestBody.MSISDN,
					application = app_name,
					environment = "production",
					level = "INFO",
					timestamp = DateTime.UtcNow,
					logz_id = logz_id
				});

				if ((RequestBody.MSISDN > 0) && (RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (!String.IsNullOrEmpty(RequestBody.TransactionID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "UnSubscribe");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "UnSubscribe");
                    lines = Add2Log(lines, "Bad Params", log_level, "UnSubscribe");

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

        public static SubscribeResponse DoUnSubscribe(SubscribeRequest RequestBody)
        {
            SubscribeResponse ret = null;
            List<LogLines> lines = new List<LogLines>();

            // logZ
            var logMessages = new List<object>();
            var logz_id = Guid.NewGuid().ToString().Replace("-", "");
            var app_name = "DoUnSubscribe";

            try
            {
                int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
                lines = Add2Log(lines, "*****************************", log_level, "UnSubscribe");

                if (ValidateRequest1(RequestBody, ref lines, ref logMessages, app_name, logz_id))
                {
                    DLValidateSMS result = String.IsNullOrEmpty(RequestBody.EncryptedMSISDN)
                                         ? DBQueries.ValidateSubscribeRequest(RequestBody, 1, ref lines)
                                         : DBQueries.ValidateSubscribeRequestWithEncMSISDN(RequestBody, 2, ref lines)
                                         ;

                    if (result == null || result.RetCode != 1000)
                    {
                        // something went wrong
                        lines = Add2Log(lines, $"!! validate failed", log_level, "");
                        ret = new SubscribeResponse()
                        {
                            ResultCode  = result == null ? 5001 : result.RetCode,
                            Description = result == null ? "Internal Error" : result.Description,
                        };
                    }
                    else
                    {
                        // all good process unsubscribe 
                    }
                    {
                        string activation_id = string.IsNullOrEmpty(RequestBody.ActivationID)
                                             ? "1"
                                             : RequestBody.ActivationID
                                             ;

                        switch (result.operator_id)
                        {
                            case 1:
                                bool sub_result = IMI.UnSubscribe(RequestBody.MSISDN.ToString(), RequestBody.ServiceID.ToString(), ref lines);
                                if (sub_result)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
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
                                        Description = "UnSubscription Request has failed"
                                    };
                                }
                                break;
                            case 29:
                                bool unsub_result = airteltigo.UnSubscribe(RequestBody, ref lines);
                                if (unsub_result)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
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
                                        Description = "UnSubscription Request has failed"
                                    };
                                }
                                break;
                            case 18:
                                ServiceClass service_or = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                ret = Orange.UnSubscribe(RequestBody, service_or, result, ref lines);
                                string sub_str = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT if(group_concat(s1.subscriber_id) IS NULL,'',group_concat(s1.subscriber_id)) FROM subscribers_ocm o, subscribers s1 WHERE o.encrypted_msisdn = '" + RequestBody.EncryptedMSISDN + "' AND s1.subscriber_id = o.subscriber_id AND s1.state_id = 1 AND s1.service_id = " + RequestBody.ServiceID, ref lines);
                                if (!String.IsNullOrEmpty(sub_str))
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = 2 where subscriber_id in (" + sub_str + ")", ref lines);
                                }

                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "UnSubscription was successful"
                                };
                                break;
                            case 17:
                                DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "UnSubscription was successful"
                                };
                                break;
                            case 3:
                            case 4:
                            case 23:
                                ServiceClass service_m = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                ret = MADAPI.UnSubscribe(RequestBody, service_m, ref lines);
                                if (ret.ResultCode == 1000)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                }
                                break;
                            //case 2:
                            //    ServiceClass service_nj = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                            //    ret = NJNG.UnSubscribe(RequestBody, service_nj, ref lines);
                            //    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                            //    break;
                            case 32:
                                bool success = DBQueries.ExecuteQuery
                                    (
                                        "update yellowdot.subscribers " +
                                        "set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id +
                                        " where subscriber_id = " + result.SubscriberID
                                        , ref lines
                                    );

                                
                                string msg  = success 
                                            ? "Merci davoir participé. Vous avez été désabonné avec succès de ce service.Si vous souhaitez revenir un jour, envoyez NO1; NO2 ou NO3 au 7717"
                                            : "Echec, veuillez reessayer plus tard"
                                            ;

                                bool smsResultCode = Orange.SendSMSKannel(
                                    RequestBody.MSISDN.ToString()
                                    , msg
                                    , false
                                    , "Orange_unsub_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                                    , RequestBody.ServiceID.ToString()
                                    , ref lines
                                );
                                lines = Add2Log(lines, $"Unsubscribe: sending sms: {msg}, result={smsResultCode}", 100, "");

                                ret = new SubscribeResponse
                                {
                                    ResultCode = success ? 1000 : 1050,
                                    Description = success
                                                ? "UnSubscription was successful"
                                                : "Failed to unsubscribe user"
                                };
                                break;
                            case 9: //Vodacom
                                string enc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT v.encrypted_msisdn from vod_enrich v WHERE v.msisdn = " + RequestBody.MSISDN + " ORDER BY v.id DESC LIMIT 1", ref lines);
                                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                Vodacom.VodSub vod_sub = Vodacom.GetServiceOffers(service, RequestBody.MSISDN.ToString(), result, enc_msisdn, ref lines);
                                if (vod_sub != null)
                                {
                                    if (!String.IsNullOrEmpty(vod_sub.subscription_id))
                                    {
                                        bool res = Vodacom.Deactivate(service, vod_sub.subscription_id, RequestBody.MSISDN.ToString(), enc_msisdn, ref lines);
                                        if (res)
                                        {
                                            DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
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
                                                Description = "UnSubscription Request has failed"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now() where subscriber_id = " + result.SubscriberID, ref lines);
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "UnSubscription was successful"
                                        };
                                    }
                                }
                                break;
                            case 8: //9mobile
                                DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "UnSubscription was successful"
                                };
                                break;
                            case 2:
                            case 26:
                            case 10: //PisiMobile
                                ServiceClass service_p = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                                ret = PISIMobileNew.UnSubscribe(RequestBody, service_p, ref lines);
                                //if (ret.ResultCode == 1010 || ret.ResultCode == 1000 || ret.ResultCode == 1050)
                                //{
                                DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                ret = new SubscribeResponse()
                                {
                                    ResultCode = 1000,
                                    Description = "UnSubscription was successful"
                                };
                                //}
                                //else
                                //{
                                //    ret = new SubscribeResponse()
                                //    {
                                //        ResultCode = 1020,
                                //        Description = "UnSubscription Request has failed"
                                //    };
                                //}
                                break;
                            case 25: //cellc SA
                                ServiceClass service_cellc = GetServiceByServiceID(RequestBody.ServiceID, ref lines);

                                string real_sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT subc.real_sub_id FROM yellowdot.subscribers_cellc subc where subc.subscriber_id = " + result.SubscriberID + ";", ref lines);

                                ret = CellC.UnSubscribe(RequestBody, service_cellc, real_sub_id, ref lines);
                                if (ret.ResultCode == 1000)
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                }
                                break;
                            default: //sdp
                                string soap = BuildUnSubscribeSoap(RequestBody.MSISDN.ToString(), result.SPID.ToString(), result.Password, "", "1", result.RealProductID.ToString());
                                lines = Add2Log(lines, "UnSubscribe Soap = " + soap, log_level, "UnSubscribe");

                                string sdp_string = "SDPSubscribeURL_" + result.operator_id + (result.is_staging == true ? "_STG" : "");

                                string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                                lines = Add2Log(lines, "Subscribe Response = " + response, log_level, "UnSubscribe");
                                if (response != "")
                                {
                                    string sub_response = CommonFuncations.ProcessXML.GetXMLNode(response, "resultDescription", ref lines);
                                    if (sub_response.Contains("Temporary Order saved successfully!") || sub_response == "Success")
                                    {
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1010,
                                            Description = "UnSubscription Request was sent"
                                        };
                                        DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                    }
                                    else
                                    {
                                        ret = new SubscribeResponse()
                                        {
                                            ResultCode = 1020,
                                            Description = "UnSubscription Request has failed with the following error" + sub_response
                                        };
                                        DataLayer.DBQueries.ExecuteQuery("update yellowdot.subscribers set state_id = 2, deactivation_date = now(), deactivation_method_id = " + activation_id + " where subscriber_id = " + result.SubscriberID, ref lines);
                                    }
                                }
                                else
                                {
                                    ret = new SubscribeResponse()
                                    {
                                        ResultCode = 1030,
                                        Description = "UnSubscription Request has failed - Network problem"
                                    };
                                }
                                break;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! exception: {ex.Message}", 100, "");
                lines = Add2Log(lines, ex.StackTrace, 100, "");
            }

            // send results to logZ
            if (ret == null) lines = Add2Log(lines, "return is null !!!", 100, "");
            else
            {
                string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", URL = " + ((String.IsNullOrEmpty(ret.URL) ? "" : ret.URL));
                lines = Add2Log(lines, text, 100, "");


                TaskManager.ExecuteInAnotherThread(() =>
                {
                    logMessages.Add
                    (
                        new
                        {
                            message = text,
                            application = app_name,
                            environment = "production",
                            level = "INFO",
                            timestamp = DateTime.UtcNow,
                            msisdn = RequestBody.MSISDN,
                            logz_id = logz_id
                        }
                    );
                    LogzIOHelper.SendLogsAsync(logMessages, "API");
                }
                );
            }

            lines = Write2Log(lines);

            TaskManager.WaitForAllTasksToFinish();      // pause to wait for everything to close off
            return ret;
        }
    }
}