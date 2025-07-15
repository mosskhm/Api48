using Api.Cache;
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
    public class Mobifingg
    {
        public static string CallMobiFingToken(ref List<LogLines> lines)
        {
            string token = "";
            string auth_json = "{\"username\" : \"" + Cache.ServerSettings.GetServerSettings("MobifinggUsername", ref lines) + "\", \"password\": \"" + Cache.ServerSettings.GetServerSettings("MobifinggPassword", ref lines) + "\"}";
            lines = Add2Log(lines, "Sending MobiFing Token Json  = " + auth_json, 100, lines[0].ControlerName);
            lines = Add2Log(lines, "Sending to " + Cache.ServerSettings.GetServerSettings("MobifinggAuthURL", ref lines), 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string auth_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("MobifinggAuthURL", ref lines), auth_json, headers, 2, ref lines);
            if (auth_response != null)
            {
                try
                {
                    dynamic auth_json_response = JsonConvert.DeserializeObject(auth_response);
                    token = auth_json_response.token;
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                    lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                }

            }
            return token;
        }

        public static RefundAirTimeResponse DoMobiFinggRefund(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };
            string token = "";
            try
            {
                if (HttpContext.Current.Application["MobifinggToken"] != null)
                {
                    lines = Add2Log(lines, " MobifinggToken Cache contains Info", 100, "");
                    if (HttpContext.Current.Application["MobifinggToken_expdate"] != null)
                    {
                        DateTime expdate = (DateTime)HttpContext.Current.Application["MobifinggToken_expdate"];
                        lines = Add2Log(lines, " expdate = " + expdate, 100, "");
                        if (DateTime.Now < expdate)
                        {
                            token = (string)HttpContext.Current.Application["MobifinggToken"];
                        }
                        else
                        {
                            token = CallMobiFingToken(ref lines);
                            HttpContext.Current.Application["MobifinggToken"] = token;
                            HttpContext.Current.Application["MobifinggToken_expdate"] = DateTime.Now.AddHours(1);
                        }
                    }
                    else
                    {
                        token = CallMobiFingToken(ref lines);
                        HttpContext.Current.Application["MobifinggToken"] = token;
                        HttpContext.Current.Application["MobifinggToken_expdate"] = DateTime.Now.AddHours(1);
                    }
                }
                else
                {
                    token = CallMobiFingToken(ref lines);
                    HttpContext.Current.Application["MobifinggToken"] = token;
                    HttpContext.Current.Application["MobifinggToken_expdate"] = DateTime.Now.AddHours(1);
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
                token = CallMobiFingToken(ref lines);
            }
            lines = Add2Log(lines, "token = " + token, 100, lines[0].ControlerName);
            if (!String.IsNullOrEmpty(token))
            {
                Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, ref lines);
                string topup_json = "{\"product_id\": \""+Cache.ServerSettings.GetServerSettings("MobifinggToUpProductID_" + service.operator_id, ref lines)+"\", \"denomination\" : "+RequestBody.Amount+",\"send_sms\" : false,\"sms_text\" : \"\"}";
                lines = Add2Log(lines, "topup_json = " + topup_json, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "Sending to URL = " + Cache.ServerSettings.GetServerSettings("MobifinggTopUpURL", ref lines) + RequestBody.MSISDN, 100, lines[0].ControlerName);
                List<Headers> headers = new List<Headers>();
                headers.Add(new Headers { key = "Authorization", value = "Bearer "+token });
                string topup_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("MobifinggTopUpURL", ref lines)+RequestBody.MSISDN, topup_json, headers, 2, ref lines);
                if (topup_response != null)
                {
                    try
                    {
                        dynamic topup_json_response = JsonConvert.DeserializeObject(topup_response);
                        string status = topup_json_response.status;
                        string message = topup_json_response.message;
                        DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '"+ status + "', result_desc = '"+ message.Replace("'","") + "' where dya_id = " + refund_id, ref lines);
                        if (status == "201")
                        {
                            ret = new RefundAirTimeResponse()
                            {
                                ResultCode = 1000,
                                Description = "Refund AirTime was successful",
                                TransactionID = refund_id.ToString()
                            };
                        }
                        else
                        {
                            ret = new RefundAirTimeResponse()
                            {
                                ResultCode = 1050,
                                Description = "Refund AirTime has failed - internal error code: " + status,
                                TransactionID = refund_id.ToString()
                            };
                        }
                    }
                    catch (Exception ex1)
                    {
                        lines = Add2Log(lines, "InnerException = " + ex1.InnerException, 100, lines[0].ControlerName);
                        lines = Add2Log(lines, "Message = " + ex1.Message, 100, lines[0].ControlerName);
                    }

                }

            }


            return ret;

        }


        public static string BuildMobiFingg(RefundAirTimeRequest RequestBody, ServiceClass service, Int64 transaction_id, ref List<LogLines> lines)
        {
            string loginid = Cache.ServerSettings.GetServerSettings("MobifinggLoginID", ref lines);
            string public_key = Cache.ServerSettings.GetServerSettings("MobifinggPublicKey", ref lines);
            string string_2_encode = loginid + "|" + transaction_id + "|5|2|" + RequestBody.MSISDN + "|" + RequestBody.Amount * 100 + "|||" + public_key;
            lines = Add2Log(lines, "string_2_encode = " + string_2_encode, 100, lines[0].ControlerName);
            string sha1_enc = SHA1Enc.Sha1_Encrypt(string_2_encode, ref lines).ToLower();
            lines = Add2Log(lines, "sha1_enc = " + sha1_enc, 100, lines[0].ControlerName);
            string checksum = md5.Encode_md5(sha1_enc);
            lines = Add2Log(lines, "checksum = " + checksum, 100, lines[0].ControlerName);
            
            string soap = "";
            soap = soap + "<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:res=\"http://arizonaadmin.mobifinng.com/WebService/reseller_iTopUp/reseller_iTopUp.wsdl.php\">";
            soap = soap + "<soapenv:Header/>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<res:FlexiRecharge soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">";
            soap = soap + "<FlexiRecharge_Request>";
            soap = soap + "<LoginId>"+ loginid + "</LoginId>";
            soap = soap + "<RequestId>"+ transaction_id + "</RequestId>";
            soap = soap + "<BatchId>5</BatchId>";
            soap = soap + "<SystemServiceID>2</SystemServiceID>";
            soap = soap + "<ReferalNumber>"+RequestBody.MSISDN+"</ReferalNumber>";
            soap = soap + "<Amount>"+ RequestBody.Amount * 100+ "</Amount>";
            soap = soap + "<FromANI></FromANI>";
            soap = soap + "<Email></Email>";
            soap = soap + "<Checksum>"+ checksum + "</Checksum>";
            soap = soap + "</FlexiRecharge_Request>";
            soap = soap + "</res:FlexiRecharge>";
            soap = soap + "</soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";
            return soap;
        }
        public static RefundAirTimeResponse DoMobiFinggRefundNew(RefundAirTimeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            RefundAirTimeResponse ret = new RefundAirTimeResponse()
            {
                ResultCode = 1030,
                Description = "Refund AirTime has failed - Network Problem",
                TransactionID = "-1"
            };
            Int64 refund_id = DataLayer.DBQueries.InsertRefundAmountTrans(RequestBody, ref lines);
            string soap = BuildMobiFingg(RequestBody, service, refund_id, ref lines);
            lines = Add2Log(lines, "topup_xml = " + soap, 100, lines[0].ControlerName);
            lines = Add2Log(lines, "Sending to URL = " + Cache.ServerSettings.GetServerSettings("MobifinggTopUpURLNew", ref lines), 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            string topup_response = CommonFuncations.CallSoap.CallSoapRequest(Cache.ServerSettings.GetServerSettings("MobifinggTopUpURLNew", ref lines), soap, headers, 1, ref lines);
            lines = Add2Log(lines, "Response = " + topup_response, 100, lines[0].ControlerName);
            if (topup_response != null)
            {
                string ResponseDescription = ProcessXML.GetXMLNode(topup_response, "ResponseDescription", ref lines);
                string ResponseCode = ProcessXML.GetXMLNode(topup_response, "ResponseCode", ref lines);
                lines = Add2Log(lines, "ResponseDescription = " + ResponseDescription, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "ResponseCode = " + ResponseCode, 100, lines[0].ControlerName);
                //<ResponseDescription xsi:type="xsd:string">Successfull</ResponseDescription>
                if (ResponseDescription == "Successfull")
                {
                    DataLayer.DBQueries.ExecuteQuery("update dya_transactions set result = '"+ ResponseCode + "', result_desc = '"+ ResponseDescription + "' where dya_id = " + refund_id, ref lines);
                    ret = new RefundAirTimeResponse()
                    {
                        ResultCode = 1000,
                        Description = "Refund AirTime was successful",
                        TransactionID = refund_id.ToString()
                    };
                }

            }




            return ret;

        }
    }
}