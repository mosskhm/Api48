using Api.CommonFuncations;
using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for momo
    /// </summary>
    public class momo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "MOMO");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "MOMO");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "MOMO");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "MOMO");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "MOMO");

            string dya_transid = "", StatusCode = "", StatusDesc = "", MOMTransactionID = "";
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var root = xmlDocument.DocumentElement;

                dya_transid = ProcessXML.GetXMLNode(xml, "ns3:ProcessingNumber", ref lines);
                lines = Add2Log(lines, " dya_transid = " + dya_transid, 100, "MOMO");

                StatusCode = ProcessXML.GetXMLNode(xml, "ns3:StatusCode", ref lines);
                lines = Add2Log(lines, " StatusCode = " + StatusCode, 100, "MOMO");

                StatusDesc = ProcessXML.GetXMLNode(xml, "ns3:StatusDesc", ref lines);
                lines = Add2Log(lines, " StatusDesc = " + StatusDesc, 100, "MOMO");

                MOMTransactionID = ProcessXML.GetXMLNode(xml, "ns3:MOMTransactionID", ref lines);
                lines = Add2Log(lines, " MOMTransactionID = " + MOMTransactionID, 100, "MOMO");

                if (!String.IsNullOrEmpty(dya_transid) && !String.IsNullOrEmpty(dya_transid) && !String.IsNullOrEmpty(dya_transid))
                {
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                    if (dya_trans != null)
                    {
                        int response_code = (StatusCode == "01" ? 1000 : 1050);
                        //TODO check if decline is 02 
                        ServiceClass service = new ServiceClass();
                        service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                        string token_id = "";
                        if (StatusCode == "100")
                        {
                            lines = Add2Log(lines, " ReChecking Transaction... " + dya_transid, 100, "MOMO");
                            LoginRequest RequestBody = new LoginRequest()
                            {
                                ServiceID = service.service_id,
                                Password = service.service_password
                            };
                            LoginResponse response = Login.DoLogin(RequestBody);
                            if (response != null)
                            {
                                if (response.ResultCode == 1000)
                                {
                                    token_id = response.TokenID;
                                }
                            }
                            if (!String.IsNullOrEmpty(token_id))
                            {
                                DYACheckTransactionRequest momo_request = new DYACheckTransactionRequest()
                                {
                                    MSISDN = dya_trans.msisdn,
                                    ServiceID = service.service_id,
                                    TokenID = token_id,
                                    TransactionID = dya_transid
                                };
                                DYACheckTransactionResponse momo_res = DYACheckTransaction.DODYACheckTransaction(momo_request);
                                if (momo_res.ResultCode == 1000)
                                {
                                    StatusCode = "01";
                                    StatusDesc = "Successfully processed transaction.";
                                    dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                                    response_code = 1000;
                                }
                            }
                        }
                        string mytime = dya_trans.datetime;
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        if (service.hour_diff != "0")
                        {
                            mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                            dya_trans.datetime = mytime;
                        }
                        ServiceBehavior.DecideBehaviorMOMO(service, dya_trans, StatusCode, ref lines);
                        if (!String.IsNullOrEmpty(MOMTransactionID))
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_transid + ",'" + MOMTransactionID + "');", ref lines);
                        }

                        //if (service.timezone_code != "0")
                        //{
                        //    try
                        //    {
                        //        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(service.timezone_code);
                        //        mytime = TimeZoneInfo.ConvertTime(Convert.ToDateTime(mytime), TimeZoneInfo.Local, timezone).ToString("yyyy-MM-dd HH:mm:ss");
                        //    }
                        //    catch(TimeZoneNotFoundException)
                        //    {
                        //        mytime = dya_trans.datetime;
                        //    }
                        //    catch (InvalidTimeZoneException)
                        //    {
                        //        mytime = dya_trans.datetime;
                        //    }
                        //}
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, StatusDesc, response_code, mytime, ref lines);
                        
                    }
                }

                


            }

            lines = Write2Log(lines);
            string response_soap = "";
            response_soap = response_soap + "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            response_soap = response_soap + "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            response_soap = response_soap + "<soapenv:Body>";
            response_soap = response_soap + "<requestPaymentCompletedResponse xmlns=\"http://www.csapi.org/schema/momopayment/local/v1_0\">";
            response_soap = response_soap + "<result>";
            response_soap = response_soap + "<resultCode xmlns=\"\">00000000</resultCode>";
            response_soap = response_soap + "<resultDescription xmlns=\"\">success</resultDescription>";
            response_soap = response_soap + "</result>";
            response_soap = response_soap + "<extensionInfo>";
            response_soap = response_soap + "<item xmlns=\"\">";
            response_soap = response_soap + "<key>result</key>";
            response_soap = response_soap + "<value>success</value>";
            response_soap = response_soap + "</item>";
            response_soap = response_soap + "</extensionInfo>";
            response_soap = response_soap + "</requestPaymentCompletedResponse>";
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