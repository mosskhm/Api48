using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class SendUSSD
    {
        public static string BuildSoap(CLSendUSSD db_validate)
        {
            string soap = "";
            soap = soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/ussd/send/v1_0/local\">";
            soap = soap + "<soapenv:Header>";
            soap = soap + "<tns:RequestSOAPHeader xmlns:tns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
            soap = soap + "<tns:spId>2340110001199</tns:spId>";
            soap = soap + "<tns:spPassword>2B919CBBB86A9BD67F1DFAF0B301AB1A</tns:spPassword>";
            soap = soap + "<tns:serviceId>234012000012430</tns:serviceId>";
            soap = soap + "<tns:timeStamp>20180306032625</tns:timeStamp>";
            soap = soap + "<tns:OA>"+ db_validate.msisdn+ "</tns:OA>";
            soap = soap + "<tns:FA>" + db_validate.msisdn + "</tns:FA>";
            soap = soap + "</tns:RequestSOAPHeader>";
            soap = soap + "</soapenv:Header>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<loc:sendUssd>";
            soap = soap + "<loc:msgType>0</loc:msgType>";
            soap = soap + "<loc:senderCB>FFFFFFF</loc:senderCB>";
            soap = soap + "<loc:receiveCB/>";
            soap = soap + "<loc:ussdOpType>2</loc:ussdOpType>";
            soap = soap + "<loc:msIsdn>"+ db_validate.msisdn + "</loc:msIsdn>";
            soap = soap + "<loc:serviceCode>205</loc:serviceCode>";
            soap = soap + "<loc:codeScheme>68</loc:codeScheme>";
            soap = soap + "<loc:ussdString>"+ db_validate.msg + "</loc:ussdString>";
            soap = soap + "<loc:endPoint>http://185.167.99.120:8080/handlers/ussd_mo.ashx?cu_id="+db_validate.cu_id+"</loc:endPoint>";
            soap = soap + "</loc:sendUssd>";
            soap = soap + "</soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";
            return soap;

        }
        public static bool ValidateRequest1(SendUSSDPushRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID + ", Token = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.CUID + ", CampaignID = " + RequestBody.CampaignID;
                if ((RequestBody.MSISDN > 0) && (RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (!String.IsNullOrEmpty(RequestBody.CUID)) && (RequestBody.CampaignID > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "SendUSSD");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "SendUSSD");
                    lines = Add2Log(lines, "Bad Params", log_level, "SendUSSD");
                }
            }
            return result;
        }

        public static SendUSSDPushResponse DoUSSDPush(SendUSSDPushRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendUSSD");
            SendUSSDPushResponse ret = null;
            if (ValidateRequest1(RequestBody, ref lines))
            {
                CLSendUSSD db_validate = ValidateUSSDPush(RequestBody, ref lines);
                if (db_validate != null)
                {
                    if (db_validate.ret_code == 1000)
                    {
                        string ussd_soap = BuildSoap(db_validate);
                        string ussd_url = Cache.ServerSettings.GetServerSettings("SDPUSSDPush_" + db_validate.operator_id , ref lines);
                        lines = Add2Log(lines, "Sending USSD soap " + ussd_soap, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendUSSD");
                        lines = Add2Log(lines, "Sending USSD soap to " + ussd_url, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendUSSD");
                        string ussd_result = CallSoap.CallSoapRequest(ussd_url, ussd_soap, ref lines);
                        lines = Add2Log(lines, "Response " + ussd_result, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendUSSD");

                        string ussd_response = CommonFuncations.ProcessXML.GetXMLNode(ussd_result, "ns1:result", ref lines);
                        if (ussd_response == "0")
                        {
                            ret = new SendUSSDPushResponse()
                            {
                                ResultCode = 1000,
                                Description = "USSD was sent",
                                TransactionID = RequestBody.CUID
                            };
                            Api.DataLayer.DBQueries.ExecuteQuery("update ussd_push.campaign_users set ussd_date_time = now(), ussd_ret_code = 1000 where cu_id = " + RequestBody.CUID, ref lines);
                        }
                        else
                        {
                            string faultstring = CommonFuncations.ProcessXML.GetXMLNode(ussd_result, "faultstring", ref lines);
                            ret = new SendUSSDPushResponse()
                            {
                                ResultCode = 1020,
                                Description = "USSD Request has failed with the following error " + faultstring,
                                TransactionID = RequestBody.CUID
                            };
                            Api.DataLayer.DBQueries.ExecuteQuery("update ussd_push.campaign_users set ussd_date_time = now(), ussd_ret_code = 1020 where cu_id = " + RequestBody.CUID, ref lines);
                        }

                    }
                    else
                    {
                        ret = new SendUSSDPushResponse()
                        {
                            ResultCode = db_validate.ret_code,
                            Description = db_validate.description,
                            TransactionID = "-1"
                        };
                    }
                }
                else
                {
                    ret = new SendUSSDPushResponse()
                    {
                        ResultCode = 5001,
                        Description = "Internal Error",
                        TransactionID = "-1"
                    };
                }
            }
            else
            {
                ret = new SendUSSDPushResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters",
                    TransactionID = "-1"
                };
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID;
            lines = Add2Log(lines, text, log_level, "SendUSSD");
            lines = Write2Log(lines);

            return ret;
        }
    }
}