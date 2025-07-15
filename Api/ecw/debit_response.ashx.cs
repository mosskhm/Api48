using Api.CommonFuncations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.ecw
{
    /// <summary>
    /// Summary description for debit_response
    /// </summary>
    public class debit_response : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ECW_debit_response");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "ECW_debit_response");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ECW_debit_response");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ECW_debit_response");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ECW_debit_response");

            string dya_transid = "", StatusCode = "", StatusDesc = "", MOMTransactionID = "";

            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var root = xmlDocument.DocumentElement;

                dya_transid = ProcessXML.GetXMLNode(xml, "externaltransactionid", ref lines);
                lines = Add2Log(lines, " dya_transid = " + dya_transid, 100, "ECW_debit_response");

                StatusDesc = ProcessXML.GetXMLNode(xml, "status", ref lines);
                lines = Add2Log(lines, " StatusDesc = " + dya_transid, 100, "ECW_debit_response");

                StatusCode = (StatusDesc == "SUCCESSFUL" ? "01" : "1050");

                MOMTransactionID = ProcessXML.GetXMLNode(xml, "transactionid", ref lines);
                lines = Add2Log(lines, " MOMTransactionID = " + dya_transid, 100, "ECW_debit_response");

                if (!String.IsNullOrEmpty(dya_transid))
                {
                    DYATransactions dya_trans = UpdateGetDYAReciveTrans(Convert.ToInt64(dya_transid), StatusCode, StatusDesc, ref lines);
                    if (dya_trans != null)
                    {
                        int response_code = (StatusCode == "01" ? 1000 : 1050);
                        //TODO check if decline is 02 
                        ServiceClass service = new ServiceClass();
                        service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                        
                        string mytime = dya_trans.datetime;
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "ECW_debit_response");
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

                        
                        lines = Add2Log(lines, " mytime = " + mytime, 100, "ECW_debit_response");
                        CommonFuncations.Notifications.SendDYAReceiveNotification(dya_trans.msisdn.ToString(), dya_trans.service_id.ToString(), dya_transid, dya_trans.partner_transid, StatusDesc, response_code, mytime, ref lines);

                    }
                }
            }
            lines = Write2Log(lines);
            string response_soap = "";
            response_soap = response_soap + "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ns1:debitcompletedresponse xmlns:ns1=\"http://www.ericsson.com/em/emm\"/>";
            

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