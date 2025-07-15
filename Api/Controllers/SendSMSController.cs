using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class SendSMSController : ApiController
    {

        public static bool ValidateRequest1(SendSMSRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "MSISDN = " + RequestBody.MSISDN + ", ServiceID = " + RequestBody.ServiceID + ", Text = " + RequestBody.Text + ", Token = " + RequestBody.TokenID + ", TransactionID = " + RequestBody.TransactionID;
                if ((RequestBody.MSISDN > 0) && (RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.Text)) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (!String.IsNullOrEmpty(RequestBody.TransactionID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "SendSMS");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "SendSMS");
                    lines = Add2Log(lines, "Bad Params", log_level, "SendSMS");
                }
            }
            return result;
        }

        // POST: api/SendSMS
        public SendSMSResponse Post([FromBody] SendSMSRequest RequestBody)
        {
            SendSMSResponse ret = CommonFuncations.SendSMS.DoSMS(RequestBody);
            return ret;
            //int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            //SendSMSResponse ret = null;
            //List<LogLines> lines = new List<LogLines>();
            //lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            
            //if (ValidateRequest1(RequestBody, ref lines))
            
            //{
            //    DLValidateSMS result = DBQueries.ValidateSMSRequest(RequestBody, ref lines);
            //    if ((result != null) && (result.RetCode == 1000))
            //    {
            //        // send sms
            //        string soap = CommonFuncations.SendSMS.BuildSendSMSSoap(result.MSISDN.ToString(), result.SPID.ToString(), result.Password, "", RequestBody.Text, result.SMSMTCode.ToString(), RequestBody.TransactionID);
            //        string response = CommonFuncations.CallSoap.CallSoapRequest("http://196.11.240.224:8310/SendSmsService/services/SendSms", soap, ref lines);
            //        lines = Add2Log(lines, "SendSMS Response = " + response, 100, "SendSMS");
            //        if (response != "")
            //        {
            //            string sms_response = CommonFuncations.ProcessXML.GetXMLNode(response, "ns1:result", ref lines);
            //            if (sms_response != "")
            //            {
            //                lines = Add2Log(lines, "SMS was sent pending DLR", 100, "SendSMS");
            //                ret = new SendSMSResponse()
            //                {
            //                    ResultCode = 1010,
            //                    Description = "SMS Was sent pending DLR"
            //                };

            //            }
            //            else
            //            {
            //                lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
            //                ret = new SendSMSResponse()
            //                {
            //                    ResultCode = 1050,
            //                    Description = "SMS Was not sent"
            //                };
            //            }
            //        }
            //        else
            //        {
            //            lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
            //            ret = new SendSMSResponse()
            //            {
            //                ResultCode = 1050,
            //                Description = "SMS Was not sent"
            //            };
            //        }

            //    }
            //    else
            //    {
            //        if (result == null)
            //        {
            //            ret = new SendSMSResponse()
            //            {
            //                ResultCode = 5001,
            //                Description = "Internal Error"
            //            };
            //        }
            //        else
            //        {
            //            ret = new SendSMSResponse()
            //            {
            //                ResultCode = result.RetCode,
            //                Description = result.Description
            //            };
            //        }
            //    }
            //}
            //else
            //{
            //    ret = new SendSMSResponse()
            //    {
            //        ResultCode = 2000,
            //        Description = "Bad Parameters"
            //    };
                
            //}
            //string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            //lines = Add2Log(lines, text, log_level, "SendSMS");
            //lines = Write2Log(lines);


            //return ret;
        }

        
    }
}
