using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Fulfillment
    {
        public static bool ValidateRequest(FulfillmentRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, 100, "Fullfilment");
                }
                else
                {
                    lines = Add2Log(lines, text, 100, "Fullfilment");
                    lines = Add2Log(lines, "Bad Params", 100, "Fullfilment");
                }
            }
            return result;
        }

        public static bool CallFulfillment(ServiceClass service, string msisdn, string updateType, bool act_deact, ref List<LogLines> lines, out string errormsg)
        {
            bool full_result = false;
            errormsg = "Desole votre requete n'a pas aboutie. Veuillez reessayer plus tard";
            FulfillmentInfo fulfillment_info = GetFulfillmentInfo(service.service_id, ref lines);
            if (fulfillment_info != null)
            {
                string product_id = (act_deact == true ? fulfillment_info.productCode : fulfillment_info.d_productCode);
                string fulfillment_soap = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                fulfillment_soap = fulfillment_soap + "<fulfillmentRequest>";
                fulfillment_soap = fulfillment_soap + "   <msisdn>" + msisdn + "</msisdn>";
                fulfillment_soap = fulfillment_soap + "   <operationType>" + fulfillment_info.operationType + "</operationType>";
                fulfillment_soap = fulfillment_soap + "   <iname>" + fulfillment_info.iname + "</iname>";
                fulfillment_soap = fulfillment_soap + "   <clientTransactionId>55432235678</clientTransactionId>";
                fulfillment_soap = fulfillment_soap + "   <productCode>" + product_id + "</productCode>";
                fulfillment_soap = fulfillment_soap + "   <pin/>";
                fulfillment_soap = fulfillment_soap + "   <splitno>" + fulfillment_info.splitno + "</splitno>";
                fulfillment_soap = fulfillment_soap + "   <extensionInfo>";
                fulfillment_soap = fulfillment_soap + "      <item>";
                fulfillment_soap = fulfillment_soap + "         <key></key>";
                fulfillment_soap = fulfillment_soap + "         <value></value>";
                fulfillment_soap = fulfillment_soap + "      </item>";
                fulfillment_soap = fulfillment_soap + "   </extensionInfo>";
                fulfillment_soap = fulfillment_soap + "</fulfillmentRequest>";

                List<Headers> headers = new List<Headers>();
                string spid = "2290110011521";
                //string spid = service.spid;
                string spid_password = "Huawei123";
                //string spid_password = service.spid_password;
                string nonce = DateTime.Now.ToString("yyyyMMddHHmmss");
                string created = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss") + "Z";
                string string_2_encode = nonce + created + spid_password;
                lines = Add2Log(lines, "string_2_encode = " + string_2_encode, 100, lines[0].ControlerName);
                string sha1_enc = SHA1Enc.Sha1_Encrypt(string_2_encode, ref lines).ToLower();
                lines = Add2Log(lines, "sha1_enc " + sha1_enc, 100, lines[0].ControlerName);
                string base_64 = Base64.EncodeDecodeBase64(sha1_enc, 1);
                lines = Add2Log(lines, "base_64 " + base_64, 100, lines[0].ControlerName);

                lines = Add2Log(lines, "soap = " + fulfillment_soap, 100, lines[0].ControlerName);

                //X-WSSE	UsernameToken Username="2290110011521", PasswordDigest="FRAcU+4emLS/wb6fIeA2k6ESY3A=", Nonce="2010082108334600001", Created="2010-08-21T08:33:46Z"
                headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + spid + "\", PasswordDigest=\"" + base_64 + "\", Nonce=\"" + nonce + "\", Created=\"" + created + "\"" });
                headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + spid + "\", PasswordDigest=\"FRAcU+4emLS/wb6fIeA2k6ESY3A=\", Nonce=\"2010082108334600001\", Created=\"2010-08-21T08:33:46Z\"" });
                headers.Add(new Headers { key = "Authorization", value = "WSSE realm=\"SDP\", profile=\"UsernameToken\"" });
                headers.Add(new Headers { key = "X-RequestHeader", value = "request ServiceId=\"\", TransId=\"\", LinkId=\"\"" });
                headers.Add(new Headers { key = "Msisdn", value = msisdn });

                //foreach (Headers h in headers)
                //{
                //    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                //}

                //string soap_url = Cache.ServerSettings.GetServerSettings("FulfillmentURL_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                //lines = Add2Log(lines, "soap_url = " + soap_url, 100, lines[0].ControlerName);
                //string response = CommonFuncations.CallSoap.CallSoapRequest1(soap_url, fulfillment_soap, headers, ref lines);

                string new_url = Cache.ServerSettings.GetServerSettings("FulfillmentURLCIS_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                new_url = new_url + "&msisdn=" + msisdn + "&input=" + product_id;
                lines = Add2Log(lines, "soap_url = " + new_url, 100, lines[0].ControlerName);
                string response = CommonFuncations.CallSoap.GetURLIgnoreCertificate(new_url, ref lines);
                if (response != "")
                {
                    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                    string full_response = CommonFuncations.ProcessXML.GetXMLNode(response, "responseCode", ref lines);
                    string full_desc = CommonFuncations.ProcessXML.GetXMLNode(response, "responseDescription", ref lines);
                    string price_charge = CommonFuncations.ProcessXML.GetXMLNode(response, "amtCharged", ref lines);
                    if (full_response == "0" && price_charge != "0")
                    {
                        full_result = true;
                    }
                    else
                    {
                        if (!full_desc.Contains("Credit insuffisant") || price_charge != "0")
                        {
                            errormsg = "Yello! Vous n avez pas suffisamment de credit pour activer le service Yellow Game. Veuillez recharger votre compte et reessayer. Merci ";
                        }
                        else
                        {
                            errormsg = full_desc;
                        }
                    }
                }



            }
            return full_result;
        }

        public static bool CallFulfillmentNoCharhing(ServiceClass service, string msisdn, string updateType, bool act_deact, ref List<LogLines> lines, out string errormsg)
        {
            bool full_result = false;
            errormsg = "Desole votre requete n'a pas aboutie. Veuillez reessayer plus tard";
            FulfillmentInfo fulfillment_info = GetFulfillmentInfo(service.service_id, ref lines);
            if (fulfillment_info != null)
            {
                string product_id = (act_deact == true ? fulfillment_info.productCode : fulfillment_info.d_productCode);
                string fulfillment_soap = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                fulfillment_soap = fulfillment_soap + "<fulfillmentRequest>";
                fulfillment_soap = fulfillment_soap + "   <msisdn>" + msisdn + "</msisdn>";
                fulfillment_soap = fulfillment_soap + "   <operationType>" + fulfillment_info.operationType + "</operationType>";
                fulfillment_soap = fulfillment_soap + "   <iname>" + fulfillment_info.iname + "</iname>";
                fulfillment_soap = fulfillment_soap + "   <clientTransactionId>55432235678</clientTransactionId>";
                fulfillment_soap = fulfillment_soap + "   <productCode>" + product_id + "</productCode>";
                fulfillment_soap = fulfillment_soap + "   <pin/>";
                fulfillment_soap = fulfillment_soap + "   <splitno>" + fulfillment_info.splitno + "</splitno>";
                fulfillment_soap = fulfillment_soap + "   <extensionInfo>";
                fulfillment_soap = fulfillment_soap + "      <item>";
                fulfillment_soap = fulfillment_soap + "         <key></key>";
                fulfillment_soap = fulfillment_soap + "         <value></value>";
                fulfillment_soap = fulfillment_soap + "      </item>";
                fulfillment_soap = fulfillment_soap + "   </extensionInfo>";
                fulfillment_soap = fulfillment_soap + "</fulfillmentRequest>";

                List<Headers> headers = new List<Headers>();
                string spid = "2290110011521";
                //string spid = service.spid;
                string spid_password = "Huawei123";
                //string spid_password = service.spid_password;
                string nonce = DateTime.Now.ToString("yyyyMMddHHmmss");
                string created = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss") + "Z";
                string string_2_encode = nonce + created + spid_password;
                lines = Add2Log(lines, "string_2_encode = " + string_2_encode, 100, lines[0].ControlerName);
                string sha1_enc = SHA1Enc.Sha1_Encrypt(string_2_encode, ref lines).ToLower();
                lines = Add2Log(lines, "sha1_enc " + sha1_enc, 100, lines[0].ControlerName);
                string base_64 = Base64.EncodeDecodeBase64(sha1_enc, 1);
                lines = Add2Log(lines, "base_64 " + base_64, 100, lines[0].ControlerName);

                lines = Add2Log(lines, "soap = " + fulfillment_soap, 100, lines[0].ControlerName);

                //X-WSSE	UsernameToken Username="2290110011521", PasswordDigest="FRAcU+4emLS/wb6fIeA2k6ESY3A=", Nonce="2010082108334600001", Created="2010-08-21T08:33:46Z"
                headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + spid + "\", PasswordDigest=\"" + base_64 + "\", Nonce=\"" + nonce + "\", Created=\"" + created + "\"" });
                headers.Add(new Headers { key = "X-WSSE", value = "UsernameToken Username=\"" + spid + "\", PasswordDigest=\"FRAcU+4emLS/wb6fIeA2k6ESY3A=\", Nonce=\"2010082108334600001\", Created=\"2010-08-21T08:33:46Z\"" });
                headers.Add(new Headers { key = "Authorization", value = "WSSE realm=\"SDP\", profile=\"UsernameToken\"" });
                headers.Add(new Headers { key = "X-RequestHeader", value = "request ServiceId=\"\", TransId=\"\", LinkId=\"\"" });
                headers.Add(new Headers { key = "Msisdn", value = msisdn });

                //foreach (Headers h in headers)
                //{
                //    lines = Add2Log(lines, h.key + " = " + h.value, 100, lines[0].ControlerName);
                //}

                //string soap_url = Cache.ServerSettings.GetServerSettings("FulfillmentURL_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                //lines = Add2Log(lines, "soap_url = " + soap_url, 100, lines[0].ControlerName);
                //string response = CommonFuncations.CallSoap.CallSoapRequest1(soap_url, fulfillment_soap, headers, ref lines);

                string new_url = Cache.ServerSettings.GetServerSettings("FulfillmentURLCIS_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                new_url = new_url + "&msisdn=" + msisdn + "&input=" + product_id + "&skipcharging=true";
                lines = Add2Log(lines, "soap_url = " + new_url, 100, lines[0].ControlerName);
                string response = CommonFuncations.CallSoap.GetURLIgnoreCertificate(new_url, ref lines);
                if (response != "")
                {
                    lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
                    string full_response = CommonFuncations.ProcessXML.GetXMLNode(response, "responseCode", ref lines);
                    string full_desc = CommonFuncations.ProcessXML.GetXMLNode(response, "responseDescription", ref lines);
                    string price_charge = CommonFuncations.ProcessXML.GetXMLNode(response, "amtCharged", ref lines);
                    if (full_response == "0" && price_charge != "0")
                    {
                        full_result = true;
                    }
                    else
                    {
                        if (!full_desc.Contains("Credit insuffisant") || price_charge != "0")
                        {
                            errormsg = "Yello! Vous n avez pas suffisamment de credit pour activer le service Yellow Game. Veuillez recharger votre compte et reessayer. Merci ";
                        }
                        else
                        {
                            errormsg = full_desc;
                        }
                    }
                }



            }
            return full_result;
        }

        public static FulfillmentResponse DoFulfullment(FulfillmentRequest RequestBody)
        {
            FulfillmentResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************",100, "Fullfilment");
            if (ValidateRequest(RequestBody, ref lines))
            {
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                string errormsg = "";
                bool resp = CallFulfillment(service, RequestBody.MSISDN.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), RequestBody.Activate, ref lines, out errormsg);
                if (resp)
                {
                    ret = new FulfillmentResponse()
                    {
                        ResultCode = 1000,
                        Description = errormsg
                    };
                }
                else
                {
                    ret = new FulfillmentResponse()
                    {
                        ResultCode = 1050,
                        Description = errormsg
                    };
                }
                
            }
            else
            {
                ret = new FulfillmentResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            lines = Add2Log(lines, text, 100, "Fullfilment");
            lines = Write2Log(lines);

            return ret;
        }
    }
}