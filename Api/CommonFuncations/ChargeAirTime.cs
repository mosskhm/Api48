using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ChargeAirTime
    {
        public static string BuildAuthnticateSoap(ChargeAirTimeRequest RequestBody, ServiceClass service, string trans_id, ref List<LogLines> lines)
        {

            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();

            string soap = "";
            soap = soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v2=\"http://www.huawei.com.cn/schema/common/v2_1\" xmlns:v1=\"http://www.csapi.org/schema/authorization/local/v1_0\">";
            soap = soap + "<soapenv:Header>";
            soap = soap + "<v2:RequestSOAPHeader>";
            soap = soap + "<v2:spId>"+service.spid+"</v2:spId>";
            soap = soap + "<v2:spPassword>"+final_password+"</v2:spPassword>";
            soap = soap + "<v2:timeStamp>"+timeStamp+"</v2:timeStamp>";
            if (service.real_service_id != "0")
            {
                soap = soap + "<v1:serviceId>" + service.real_service_id + "</v1:serviceId>";
            }
            soap = soap + "</v2:RequestSOAPHeader>";
            soap = soap + "</soapenv:Header>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<v1:authorization>";
            soap = soap + "<v1:endUserIdentifier>"+RequestBody.MSISDN+"</v1:endUserIdentifier>";
            soap = soap + "<v1:transactionId>"+ trans_id + "</v1:transactionId>";
            //soap = soap + "<v1:scope>17</v1:scope>";
            soap = soap + "<v1:scope>"+(service.is_ondemand == true ? "79" : "17")+"</v1:scope>";
            soap = soap + "<v1:serviceId>"+service.real_service_id+"</v1:serviceId>";
            if (service.is_ondemand == false)
            {
                soap = soap + "<v1:amount>" + RequestBody.Amount + "</v1:amount>";
                string curency = Cache.ServerSettings.GetServerSettings("Curency_" + service.operator_id, ref lines);
                soap = soap + "<v1:currency>" + curency + "</v1:currency>";
            }
            
            
            soap = soap + "<v1:contentId/>";
            soap = soap + "<v1:frequency>0</v1:frequency>";
            soap = soap + "<v1:description>"+service.service_name+"</v1:description>";
            string end_point = Cache.ServerSettings.GetServerSettings("ChargeEndPointURL", ref lines);
            soap = soap + "<v1:notificationURL>"+ end_point + "</v1:notificationURL>";
            soap = soap + "<v1:tokenValidity>1</v1:tokenValidity>";
            soap = soap + "<v1:extensionInfo>";
            soap = soap + "<item>";
            soap = soap + "<key>productName</key>";
            soap = soap + "<value>"+ service.service_name + "</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>productId</key>";
            soap = soap + "<value>"+service.product_id+"</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>totalAmount</key>";
            soap = soap + "<value>"+RequestBody.Amount+"</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>channel</key>";
            soap = soap + "<value>2</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>serviceInterval</key>";
            soap = soap + "<value>1</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>serviceIntervalUnit</key>";
            soap = soap + "<value>2</value>";
            soap = soap + "</item>";
            soap = soap + "<item>";
            soap = soap + "<key>tokenType</key>";
            soap = soap + "<value>0</value>";
            soap = soap + "</item>";
            soap = soap + "</v1:extensionInfo>";
            soap = soap + "</v1:authorization>";
            soap = soap + "</soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";
            return soap;
        }

        public static string BuildChargeAmountSoap(string msisdn, string access_token, ServiceClass service, DYATransactions dya_trans)
        {
            string soap = "";
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();

            soap = soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/payment/amount_charging/v2_1/local\">";
            soap = soap + "   <soapenv:Header>";
            soap = soap + "      <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "         <spId>" + service.spid + "</spId>";
            soap = soap + "         <spPassword>" + final_password + "</spPassword>";
            soap = soap + "         <serviceId>" + service.real_service_id + "</serviceId>";
            soap = soap + "         <timeStamp>" + timeStamp + "</timeStamp>";
            soap = soap + "         <OA>tel:" + msisdn + "</OA>";
            soap = soap + "         <oauth_token>" + access_token + "</oauth_token>";
            soap = soap + "      </RequestSOAPHeader>";
            soap = soap + "   </soapenv:Header>";
            soap = soap + "   <soapenv:Body>";
            soap = soap + "      <loc:chargeAmount>";
            soap = soap + "         <loc:endUserIdentifier>" + msisdn + "</loc:endUserIdentifier>";
            soap = soap + "         <loc:charge>";
            soap = soap + "            <description>" + service.service_name + "</description>";
            soap = soap + "            <currency>NGN</currency>";
            soap = soap + "            <amount>" + dya_trans.amount + ".00</amount>";
            soap = soap + "            <code>" + (service.airtime_code == 0 ? "" : service.airtime_code.ToString()) + "</code>";
            soap = soap + "         </loc:charge>";
            soap = soap + "         <loc:referenceCode>" + dya_trans.dya_trans + "</loc:referenceCode>";
            soap = soap + "      </loc:chargeAmount>";
            soap = soap + "   </soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";
            return soap;
        }

        public static bool ValidateRequest(ChargeAirTimeRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount + ", TransactionID = " + RequestBody.TransactionID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0) && (!String.IsNullOrEmpty(RequestBody.TransactionID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
                    lines = Add2Log(lines, "Bad Params", log_level, lines[0].ControlerName);
                }
            }
            return result;
        }

        public static ChargeAirTimeResponse DoChrageAirTime(ChargeAirTimeRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ChargeAirTime");
            ChargeAirTimeResponse ret = null;
            if (ValidateRequest(RequestBody, ref lines))
            {
                ServiceClass service = GetServiceByServiceIDAndAmount(RequestBody.ServiceID, RequestBody.Amount, ref lines);
                if (service != null)
                {
                    CheckLogin cl = CheckLoginToken(RequestBody.ServiceID, RequestBody.TokenID, ref lines);
                    if (cl == null)
                    {
                        if (service.allow_chargeairtime == false)
                        {
                            ret = new ChargeAirTimeResponse()
                            {
                                ResultCode = 2020,
                                Description = "Charge AirTime is not allowed for this service",
                                TransactionID = "-1"
                            };
                        }
                        else
                        {
                            Int64 ca_id = InsertChargeAmountTrans(RequestBody, ref lines);
                            if (ca_id >0)
                            {
                                if (service.operator_id == 12)
                                {
                                    string response = Benin.ChargeAmount(RequestBody.MSISDN.ToString(), service, ca_id.ToString(), ref lines);
                                    string error = ProcessXML.GetXMLNode(response, "faultcode", ref lines) + " - " + ProcessXML.GetXMLNode(response, "faultstring", ref lines);
                                    string success = ProcessXML.GetXMLNode(response, "ns1:chargeAmountResponse", ref lines); 
                                    if (success == "" && error == " - ")
                                    {
                                        DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = 0, result_desc = 'Success' where dya_id = " + ca_id, ref lines);
                                        ret = new ChargeAirTimeResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "Charge AirTime Request was successful",
                                            TransactionID = ca_id.ToString()
                                        };

                                    }
                                    else
                                    {
                                        
                                        DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = 1, result_desc = '"+ error + "' where dya_id = " + ca_id, ref lines);
                                        ret = new ChargeAirTimeResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "Charge AirTime Request has failed. Error: " + error,
                                            TransactionID = ca_id.ToString()
                                        };
                                    }
                                }
                                else
                                {
                                    string soap = BuildAuthnticateSoap(RequestBody, service, ca_id.ToString(), ref lines);
                                    lines = Add2Log(lines, "Soap = " + soap, 100, "ChargeAirTime");
                                    string sdp_string = "SDPAuthURL_" + service.operator_id + (service.is_staging == true ? "_STG" : "");
                                    string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                                    lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ChargeAirTime");
                                    string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                                    lines = Add2Log(lines, "Auth Response = " + response, 100, "ChargeAirTime");
                                    if (response != "")
                                    {
                                        string auth_response = CommonFuncations.ProcessXML.GetXMLNode(response, "resultCode", ref lines);
                                        string token_id = CommonFuncations.ProcessXML.GetXMLNode(response, "ns1:token", ref lines);
                                        if (auth_response == "0" && !String.IsNullOrEmpty(token_id))
                                        {
                                            lines = Add2Log(lines, "Auth was successful waiting for user consent", 100, "ChargeAirTime");
                                            DataLayer.DBQueries.ExecuteQuery("update dya_transactions set token_id = '" + token_id + "' where dya_id = " + ca_id, ref lines);


                                            ret = new ChargeAirTimeResponse()
                                            {
                                                ResultCode = 1010,
                                                Description = "Charge AirTime Request was sent. Waiting for user consent",
                                                TransactionID = ca_id.ToString()
                                            };

                                        }
                                        else
                                        {
                                            lines = Add2Log(lines, "Auth has failed", 100, "ChargeAirTime");
                                            ret = new ChargeAirTimeResponse()
                                            {
                                                ResultCode = 1050,
                                                Description = "Charge AirTime has failed",
                                                TransactionID = ca_id.ToString()
                                            };
                                        }
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "Auth has failed", 100, "ChargeAirTime");
                                        ret = new ChargeAirTimeResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "Charge AirTime has failed",
                                            TransactionID = ca_id.ToString()
                                        };
                                    }
                                }
                                
                            }
                            else
                            {
                                ret = new ChargeAirTimeResponse()
                                {
                                    ResultCode = 5001,
                                    Description = "Internal Error",
                                    TransactionID = "-1"
                                };
                            }
                            //Authnticate

                        }
                    }
                    else
                    {
                        ret = new ChargeAirTimeResponse()
                        {
                            ResultCode = cl.ResultCode,
                            Description = cl.Description,
                            TransactionID = "-1"
                        };
                    }

                }
                else
                {
                    ret = new ChargeAirTimeResponse()
                    {
                        ResultCode = 2021,
                        Description = "Service or Price not found",
                        TransactionID = "-1"
                    };
                }
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID;
            lines = Add2Log(lines, text, log_level, lines[0].ControlerName);
            lines = Write2Log(lines);

            return ret;
        }
    }
}