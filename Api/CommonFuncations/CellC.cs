using Api.HttpItems;
using Microsoft.Ajax.Utilities;
using Microsoft.Web.Services2.Addressing;
using Microsoft.Web.Services2.Security.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http.Results;
using System.Xml;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class CellC
    {
        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            //msisdn = msisdn.Substring(3);
            string url = Cache.ServerSettings.GetServerSettings("CellC_SendSMSURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + System.Uri.EscapeDataString(text) + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "");
            lines = Add2Log(lines, "SMS Url = " + url, 100, "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }

        public static SubscribeResponse Subscribe(SubscribeRequest RequestBody, ServiceClass service, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Subscription Request failed"
            };

            try
            {
                lines = Add2Log(lines, $"CellC Subscribe: RequestBody = {RequestBody.ToString()}", 100, "");

                Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscription_requests (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
                if (trans_id == 0) lines = Add2Log(lines, $"failed to insert subscription_request for MSISDN={RequestBody.MSISDN} to service_id={RequestBody.ServiceID}", 100, "");
                else
                {
                    string user_name = Api.Cache.ServerSettings.GetServerSettings("CellC_UN", ref lines);
                    string password = Api.Cache.ServerSettings.GetServerSettings("CellC_PW", ref lines);
                    string url = Api.Cache.ServerSettings.GetServerSettings("CellC_URL", ref lines);

                    UsernameToken usernameTokenSection = new UsernameToken(user_name, password, PasswordOption.SendPlainText); //acquiring security headers
                    string usernameTokenSection1 = usernameTokenSection.GetXml(new XmlDocument()).OuterXml.ToString().Replace("<wsse:Nonce", "<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\"");

                    string bearerType = RequestBody.GetURL == "1" ? "WEB" : "WEBSMS";

                    string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wasp=\"http://wasp.doi.soap.protocol.cellc.co.za\">"
                                  + "<soapenv:Header>"
                                    + "<wsse:Security soapenv:mustUnderstand=\"1\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">"
                                    + usernameTokenSection1
                                    + "</wsse:Security>"
                                  + "</soapenv:Header>"
                                  + "<soapenv:Body>"
                                    + "<wasp:addSubscription>"
                                      + "<msisdn>" + RequestBody.MSISDN == "-1" ? "" : RequestBody.MSISDN + "</msisdn>"
                                      + "<serviceName>" + service.service_name + "</serviceName>"
                                      + "<contentProvider>YD</contentProvider>"
                                      + "<chargeCode>" + service.spid_password + "</chargeCode>"
                                      + "<chargeInterval>DAILY</chargeInterval>"
                                      + "<contentType>OTHER</contentType>"
                                      + "<bearerType>" + bearerType + "</bearerType>"
                                      + "<waspReference>" + trans_id + "</waspReference>"
                                      + "<waspTID>" + trans_id + "</waspTID>"
                                    + "</wasp:addSubscription>"
                                  + "</soapenv:Body>"
                                + "</soapenv:Envelope>"
                                ;

                    List<Headers> headers = new List<Headers>();
                    string body = CallSoap.CallSoapRequestIgnoreCertificate(url, soap, headers, ref lines) ?? "";
                    string result = ProcessXML.GetXMLNode(body, "result", ref lines) ?? "";                 // returns "result" from body
                    string serviceID = ProcessXML.GetXMLNode(body, "serviceID", ref lines) ?? "";           // returns "serviceID" from body !!!!! -- shouldn't this be service.serviceID
                    string base_url = Cache.ServerSettings.GetServerSettings("CellC_WebURL", ref lines) ?? "";

                    lines = Add2Log(lines, $"CellC Subscribe: result={result}, serviceID={serviceID}, base_url={base_url}", 100, "");
                    if (RequestBody.GetURL == "1" && !String.IsNullOrEmpty(serviceID))
                    {
                        ret = new SubscribeResponse()
                        {
                            ResultCode = 1302,
                            Description = "Redirect to URL",
                            URL = base_url + "wasptid=" + trans_id + "&doiserviceid=" + serviceID
                        };
                    }
                    else if (result == "0")
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
            return ret;
        }
        public static SubscribeResponse UnSubscribe(SubscribeRequest RequestBody, ServiceClass service,string real_sub_id, ref List<LogLines> lines)
        {
            SubscribeResponse ret = new SubscribeResponse()
            {
                ResultCode = 1030,
                Description = "Unsubscription Request failed"
            };

          
                string user_name = Api.Cache.ServerSettings.GetServerSettings("CellC_UN", ref lines);
                string password = Api.Cache.ServerSettings.GetServerSettings("CellC_PW", ref lines);
                string url = Api.Cache.ServerSettings.GetServerSettings("CellC_URL", ref lines);

                UsernameToken usernameTokenSection = new UsernameToken(user_name, password, PasswordOption.SendPlainText); //acquiring security headers
                string usernameTokenSection1 = usernameTokenSection.GetXml(new XmlDocument()).OuterXml.ToString().Replace("<wsse:Nonce", "<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\"");

                string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wasp=\"http://wasp.doi.soap.protocol.cellc.co.za\">";
                soap = soap + "<soapenv:Header>";
                soap = soap + "<wsse:Security soapenv:mustUnderstand=\"1\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">";
                soap = soap + usernameTokenSection1;
                soap = soap + "</wsse:Security>";
                soap = soap + "</soapenv:Header>";
                soap = soap + "<soapenv:Body>";
                soap = soap + "      <wasp:cancelSubscription>";
                soap = soap + "       <msisdn>" + RequestBody.MSISDN + "</msisdn>";
                soap = soap + "       <serviceID>" + real_sub_id + "</serviceID>";
                soap = soap + "       <waspTID>" + real_sub_id + "</waspTID>";
                soap = soap + "      </wasp:cancelSubscription>";
                soap = soap + "   </soapenv:Body>";
                soap = soap + "</soapenv:Envelope>";



                List <Headers> headers = new List<Headers>();
                string body = CallSoap.CallSoapRequestIgnoreCertificate(url, soap, headers, ref lines);
                try
                {
                    string status = Api.CommonFuncations.ProcessXML.GetXMLNode(body,"status", ref lines); //takes in xml and tag name and returns tag value
                    if (status.ToLower() == "cancelled")
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

            
            return ret;
        }
        public static BillResponse Bill(BillRequest RequestBody, ServiceClass service,string real_sub_id, DLValidateBill result1,ref List<LogLines> lines)
        {
            BillResponse ret = new BillResponse()
            {
                ResultCode = 1030,
                Description = "Billing Request failed"
            };

            Int64 trans_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into yellowdot.cellc_billing_request (msisdn, service_id, date_time, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now(),'" + RequestBody.TransactionID + "')", ref lines);
            if (trans_id> 0) 
            {
                string user_name = Api.Cache.ServerSettings.GetServerSettings("CellC_UN", ref lines);
                string password = Api.Cache.ServerSettings.GetServerSettings("CellC_PW", ref lines);
                string url = Api.Cache.ServerSettings.GetServerSettings("CellC_URL", ref lines);

                UsernameToken usernameTokenSection = new UsernameToken(user_name, password, PasswordOption.SendPlainText); //acquiring security headers
                string usernameTokenSection1 = usernameTokenSection.GetXml(new XmlDocument()).OuterXml.ToString().Replace("<wsse:Nonce", "<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\"");

                string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:wasp=\"http://wasp.doi.soap.protocol.cellc.co.za\">";
                soap = soap + "<soapenv:Header>";
                soap = soap + "<wsse:Security soapenv:mustUnderstand=\"1\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">";
                soap = soap + usernameTokenSection1;
                soap = soap + "</wsse:Security>";
                soap = soap + "</soapenv:Header>";
                soap = soap + "<soapenv:Body>";
                soap = soap + "      <wasp:chargeSubscriber>";
                soap = soap + "       <msisdn>" + RequestBody.MSISDN + "</msisdn>";
                soap = soap + "       <serviceID>" + real_sub_id + "</serviceID>";
                soap = soap + "       <waspTID>" + real_sub_id + "</waspTID>";
                soap = soap + "      </wasp:chargeSubscriber>";
                soap = soap + "   </soapenv:Body>";
                soap = soap + "</soapenv:Envelope>";


                List <Headers> headers = new List<Headers>();
                string body = CallSoap.CallSoapRequestIgnoreCertificate(url, soap, headers, ref lines);
                try
                {
                    string result = Api.CommonFuncations.ProcessXML.GetXMLNode(body, "result", ref lines); //takes in xml and tag name and returns tag value
                    if (!String.IsNullOrEmpty(result))
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("Update yellowdot.cellc_billing_request set response = '" + result + "' where id = " + trans_id + ";", ref lines);
                    }
                    if (result.ToLower() == "0")
                    {

                        ret = new BillResponse()
                        {
                            ResultCode = 1000,
                            Description = "Billing was successful"
                        };
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into billing (subscriber_id, billing_date_time, price_id) values("+result1.sub_id+",now(),"+RequestBody.PriceID+")", ref lines);
                    }
                    else
                    {
                        ret = new BillResponse()
                        {
                            ResultCode = 1020,
                            Description = "Billing Request failed"
                        };
                    }
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
                }
            }
            

            return ret;
        }

    }
}