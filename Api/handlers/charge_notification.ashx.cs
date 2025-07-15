using Api.CommonFuncations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for charge_notification
    /// </summary>
    public class charge_notification : IHttpHandler
    {
        public class Items
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public static string BuildQueryAuthSoap(string msisdn, ServiceClass service, ref List<LogLines> lines)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();

            string soap = "";
            soap = soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v2=\"http://www.huawei.com.cn/schema/common/v2_1\" xmlns:v1=\"http://www.csapi.org/schema/authorization/local/v1_0\">";
            soap = soap + "   <soapenv:Header>";
            soap = soap + "      <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "         <spId>" + service.spid + "</spId>";
            soap = soap + "         <spPassword>" + final_password + "</spPassword>";
            soap = soap + "         <timeStamp>" + timeStamp + "</timeStamp>";
            soap = soap + "      </RequestSOAPHeader>";
            soap = soap + "   </soapenv:Header>";
            soap = soap + "   <soapenv:Body>";
            soap = soap + "      <v1:queryAuthorizationList>";
            soap = soap + "         <v1:endUserIdentifier>" + msisdn + "</v1:endUserIdentifier>";
            soap = soap + "         <v1:operation>1</v1:operation>";
            soap = soap + "      </v1:queryAuthorizationList>";
            soap = soap + "   </soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";

            return soap;
        }

        public static string BuildSoapChargeAmount(string msisdn, string access_token, ServiceClass service, DYATransactions dya_trans)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");

            string final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();

            string soap = "";
            soap = soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/payment/amount_charging/v2_1/local\">";
            soap = soap + "   <soapenv:Header>";
            soap = soap + "      <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "         <spId>"+ service.spid + "</spId>";
            soap = soap + "         <spPassword>"+ final_password + "</spPassword>";
            soap = soap + "         <serviceId>"+ service.real_service_id+ "</serviceId>";
            soap = soap + "         <timeStamp>"+timeStamp+"</timeStamp>";
            soap = soap + "         <OA>tel:"+ msisdn + "</OA>";
            soap = soap + "         <oauth_token>"+ access_token + "</oauth_token>";
            soap = soap + "      </RequestSOAPHeader>";
            soap = soap + "   </soapenv:Header>";
            soap = soap + "   <soapenv:Body>";
            soap = soap + "      <loc:chargeAmount>";
            soap = soap + "         <loc:endUserIdentifier>"+msisdn+"</loc:endUserIdentifier>";
            soap = soap + "         <loc:charge>";
            soap = soap + "            <description>"+service.service_name+"</description>";
            soap = soap + "            <currency>NGN</currency>";
            soap = soap + "            <amount>"+dya_trans.amount+".00</amount>";
            soap = soap + "            <code>"+(service.airtime_code == 0 ? "" : service.airtime_code.ToString()) +"</code>";
            soap = soap + "         </loc:charge>";
            soap = soap + "         <loc:referenceCode>"+dya_trans.dya_trans+"</loc:referenceCode>";
            soap = soap + "      </loc:chargeAmount>";
            soap = soap + "   </soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";

            return soap;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "ChargeAirTime");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ChargeAirTime");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ChargeAirTime");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ChargeAirTime");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ChargeAirTime");

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.LoadXml(xml);

                var root = xmlDocument.DocumentElement;

                string MSISDN = ProcessXML.GetXMLNode(xml, "ID", ref lines);
                lines = Add2Log(lines, " MSISDN = " + MSISDN, 100, "ChargeAirTime");

                string SPID = ProcessXML.GetXMLNode(xml, "ns1:spId", ref lines);
                lines = Add2Log(lines, " SPID = " + SPID, 100, "ChargeAirTime");

                string ServiceID = ProcessXML.GetXMLNode(xml, "ns1:serviceId", ref lines);
                lines = Add2Log(lines, " ServiceID = " + ServiceID, 100, "ChargeAirTime");

                string timeStamp = ProcessXML.GetXMLNode(xml, "ns1:timeStamp", ref lines);
                lines = Add2Log(lines, " timeStamp = " + timeStamp, 100, "ChargeAirTime");
                if (!String.IsNullOrEmpty(timeStamp))
                {
                    var date = DateTime.ParseExact(timeStamp, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    timeStamp = date.ToString("yyyy-MM-dd HH:mm:ss");
                }
                string consentResult = ProcessXML.GetXMLNode(xml, "ns2:consentResult", ref lines);
                lines = Add2Log(lines, " consentResult = " + consentResult, 100, "ChargeAirTime");

                string accessToken = ProcessXML.GetXMLNode(xml, "ns2:accessToken", ref lines);
                lines = Add2Log(lines, " accessToken = " + accessToken, 100, "ChargeAirTime");

                List<Items> xml_items = new List<Items>();
                string transactionID = "";
                foreach (XmlNode zz in xmlDocument.GetElementsByTagName("item"))
                {
                    var key = zz.ChildNodes[0].InnerText;
                    var value = zz.ChildNodes[1].InnerText;
                    xml_items.Add(new Items() { Key = key, Value = value });
                    lines = Add2Log(lines, " Key = " + key + ", Value = " + value, 100, "ChargeAirTime");
                    if (key == "transactionID")
                    {
                        transactionID = value;
                    }
                }
                lines = Add2Log(lines, " transactionID = " + transactionID, 100, "ChargeAirTime");

                ServiceClass service = GetServiceInfo(SPID, ServiceID, "", ref lines);

                if (!String.IsNullOrEmpty(transactionID) && consentResult == "0" && (service != null))
                {
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(transactionID), consentResult, "User has consent", ref lines);
                    if (dya_trans != null)
                    {
                        string soap = BuildQueryAuthSoap(MSISDN, service, ref lines);
                        lines = Add2Log(lines, "Soap = " + soap, 100, "ChargeAirTime");
                        string sdp_string = "SDPAuthURL_" + service.operator_id + (service.is_staging == true ? "_STG" : "");
                        string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                        lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ChargeAirTime");
                        string query_response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                        lines = Add2Log(lines, "Query Auth Response = " + query_response, 100, "ChargeAirTime");

                        if (query_response != "")
                        {
                            string access_token = CommonFuncations.ProcessXML.GetXMLNode(query_response, "accessToken", ref lines);
                            lines = Add2Log(lines, "access_token = " + access_token, 100, "ChargeAirTime");
                            if (access_token != "")
                            {
                                soap = BuildSoapChargeAmount(MSISDN, access_token, service, dya_trans);
                                lines = Add2Log(lines, "Soap = " + soap, 100, "ChargeAirTime");
                                sdp_string = "SDPChargeURL_" + service.operator_id + (service.is_staging == true ? "_STG" : "");
                                soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                                lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "ChargeAirTime");
                                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                                lines = Add2Log(lines, "ChargeAirTime Response = " + response, 100, "ChargeAirTime");
                                if (response != "")
                                {
                                    string error_code = CommonFuncations.ProcessXML.GetXMLNode(response, "faultcode", ref lines);
                                    if (error_code == "")
                                    {
                                        PriceClass price_c = GetPricesInfo(service.service_id, Convert.ToDouble(service.airtimr_amount * 100), ref lines);
                                        
                                        
                                        
                                        string MySql = "insert into .subscribers (msisdn, service_id, subscription_date, deactivation_date, state_id, subscription_method_id, deactivation_method_id, subscription_keyword, deactivation_keyword) values(" + MSISDN + ", " + service.service_id + ", '" + timeStamp + "', '" + timeStamp + "', 2, '3', '3', '', '')";
                                        Int64 sub_id = DataLayer.DBQueries.ExecuteQueryReturnInt64(MySql, ref lines);
                                        lines = Add2Log(lines, "sub_id = " + sub_id, 100, "ChargeAirTime");
                                        Int64 sub_id_175 = 0;
                                        if (Cache.ServerSettings.GetServerSettings("DoCallBack", ref lines) == "1")
                                        {
                                            //string MySql = "insert into .subscribers (msisdn, service_id, subscription_date, deactivation_date, state_id, subscription_method_id, deactivation_method_id, subscription_keyword, deactivation_keyword) values(" + MSISDN + ", " + service.service_id + ", '" + timeStamp + "', '" + timeStamp + "', 2, '3', '3', '', '')";
                                            sub_id_175 = DataLayer.DBQueries.ExecuteQueryReturnInt64(MySql, "DBConnectionString_175", ref lines);
                                            lines = Add2Log(lines, "sub_id_175 = " + sub_id_175, 100, "ChargeAirTime");
                                        }
                                        MySql = "update dya_transactions set result = 0, result_desc = 'Success' where dya_id = " + dya_trans.dya_trans;
                                        DataLayer.DBQueries.ExecuteQuery(MySql, ref lines);

                                        
                                        if (sub_id > 0)
                                        {

                                            if (price_c != null)
                                            {
                                                MySql = "insert into .billing (subscriber_id, billing_date_time, price_id) values (" + sub_id + ", '" + timeStamp + "', " + price_c.price_id + ")";
                                                Int64 billing_id = DataLayer.DBQueries.ExecuteQueryReturnInt64(MySql, ref lines);
                                                lines = Add2Log(lines, "billing_id = " + billing_id, 100, "ChargeAirTime");
                                                if (Cache.ServerSettings.GetServerSettings("DoCallBack", ref lines) == "1")
                                                {
                                                    MySql = "insert into .billing (subscriber_id, billing_date_time, price_id) values (" + sub_id_175 + ", '" + timeStamp + "', " + price_c.price_id + ")";
                                                    Int64 billing_id_175 = DataLayer.DBQueries.ExecuteQueryReturnInt64(MySql, "DBConnectionString_175", ref lines);
                                                    lines = Add2Log(lines, "billing_id_175 = " + billing_id_175, 100, "ChargeAirTime");
                                                }

                                            }
                                            else
                                            {
                                                lines = Add2Log(lines, "Error price not found!", 100, "ChargeAirTime");
                                            }
                                            CommonFuncations.Notifications.SendChargeAmountNotification(MSISDN, service.service_id.ToString(), dya_trans.dya_trans.ToString(), dya_trans.partner_transid, "Success", ref lines);
                                        }
                                        
                                        
                                    }
                                    else
                                    {
                                        CommonFuncations.Notifications.SendChargeAmountNotification(MSISDN, service.service_id.ToString(), dya_trans.dya_trans.ToString(), dya_trans.partner_transid, "Failed", ref lines);
                                        string faultstring = CommonFuncations.ProcessXML.GetXMLNode(response, "faultstring", ref lines);
                                        string MySql = "update dya_transactions set result = 0, result_desc = '"+ error_code + " - "+ faultstring + "' where dya_id = " + dya_trans.dya_trans;
                                        DataLayer.DBQueries.ExecuteQuery(MySql, ref lines);
                                    }
                                }
                                else
                                {
                                    CommonFuncations.Notifications.SendChargeAmountNotification(MSISDN, service.service_id.ToString(), dya_trans.dya_trans.ToString(), dya_trans.partner_transid, "Failed", ref lines);
                                    string MySql = "update dya_transactions set result = 0, result_desc = 'Failed - Network Problem' where dya_id = " + dya_trans.dya_trans;
                                    DataLayer.DBQueries.ExecuteQuery(MySql, ref lines);
                                }
                            }
                        }
                        else
                        {
                            CommonFuncations.Notifications.SendChargeAmountNotification(MSISDN, service.service_id.ToString(), dya_trans.dya_trans.ToString(), dya_trans.partner_transid, "Failed", ref lines);
                            string MySql = "update dya_transactions set result = 0, result_desc = 'Failed - Network Problem' where dya_id = " + dya_trans.dya_trans;
                            DataLayer.DBQueries.ExecuteQuery(MySql, ref lines);
                        }
                    }
                }
                else
                {
                    lines = Add2Log(lines, "Something went wrong = " , 100, "ChargeAirTime");
                }
            }


            lines = Write2Log(lines);
            string response_soap = "";
            response_soap = response_soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/subscriberconsnet/data/v1_0/local\">";
            response_soap = response_soap + "<soapenv:Header/>";
            response_soap = response_soap + "<soapenv:Body>";
            response_soap = response_soap + "<loc:notifySubscriberConsentResultResponse>";
            response_soap = response_soap + "<loc:result>0</loc:result>";
            response_soap = response_soap + "<loc:resultDescription>Success</loc:resultDescription>";
            response_soap = response_soap + "</loc:notifySubscriberConsentResultResponse>";
            response_soap = response_soap + "</soapenv:Body>";
            response_soap = response_soap + "</soapenv:Envelope>";

            context.Response.ContentType = "text/xml";
            context.Response.Write(response_soap);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}