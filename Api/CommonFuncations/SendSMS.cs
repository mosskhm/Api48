using Api.DataLayer;
using Api.HttpItems;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class SendSMS
    {
        public static string BuildSendSMSSoap(string msisdn, string sp_id, string password, string service_id, string text, string sender_name, string transaction_id, string endpoint, string auth_id, string link_id)
        {

            switch(sp_id)
            {
                case "2420110012641":
                    sender_name = "YellowBet";
                    break;
            }
            string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
           
            string final_password = md5.Encode_md5(sp_id + password + timeStamp).ToUpper();
           
            string soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:v2=\"http://www.huawei.com.cn/schema/common/v2_1\" xmlns:loc=\"http://www.csapi.org/schema/parlayx/sms/send/v2_2/local\">";
            soap = soap + "<soapenv:Header>";
            soap = soap + "<v2:RequestSOAPHeader>";
            soap = soap + "<v2:spId>" + sp_id + "</v2:spId>";
            if (service_id != "0")
            {
                soap = soap + "<serviceId>" + service_id + "</serviceId>";
            }
            soap = soap + "<v2:spPassword>" + final_password + "</v2:spPassword>";
            soap = soap + "<v2:timeStamp>" + timeStamp + "</v2:timeStamp>";
            if (!String.IsNullOrEmpty(auth_id))
            {
                soap = soap + "<v2:oauth_token>" + auth_id + "</v2:oauth_token>";
            }

            if (!String.IsNullOrEmpty(link_id))
            {
                soap = soap + "<v2:linkid>" + link_id + "</v2:linkid>";
            }
            soap = soap + "<v2:OA>" + msisdn + "</v2:OA>";
            soap = soap + "<v2:FA>" + msisdn + "</v2:FA>";
            soap = soap + "</v2:RequestSOAPHeader>";
            soap = soap + "</soapenv:Header>";
            soap = soap + "<soapenv:Body>";
            soap = soap + "<loc:sendSms>";
            soap = soap + "<loc:addresses>tel:" + msisdn + "</loc:addresses>";
            //soap = soap + "<loc:senderName>8393006881196</loc:senderName>";
            soap = soap + "<loc:senderName>" + sender_name + "</loc:senderName>";
            soap = soap + "<loc:receiptRequest>";
            soap = soap + @"<endpoint>"+ endpoint + "?trans_id="+transaction_id+"</endpoint>";
            soap = soap + "<interfaceName>SmsNotification</interfaceName>";
            soap = soap + "<correlator>12345</correlator>";
            soap = soap + "</loc:receiptRequest>";
            soap = soap + "<loc:message>" + text + "</loc:message>";
            soap = soap + "</loc:sendSms>";
            soap = soap + "</soapenv:Body>";
            soap = soap + "</soapenv:Envelope>";

            return soap;

        }

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

        public static SendSMSResponse DoSMSWU(SendSMSRequest RequestBody, string url)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            SendSMSResponse ret = new SendSMSResponse()
            {
                ResultCode = 1050,
                Description = "Failed"
            };

            string json = JsonConvert.SerializeObject(RequestBody);
            List<Headers> headers = new List<Headers>();
            string body = CallSoap.CallSoapRequest(url, json, headers, 4, ref lines);
            dynamic json_response = JsonConvert.DeserializeObject(body);
            try
            {
                string result_code = json_response.ResultCode;
                string description = json_response.Description;
                if (result_code == "1000" || result_code == "1010")
                {
                    ret = new SendSMSResponse()
                    {
                        ResultCode = Convert.ToInt32(result_code),
                        Description = description
                    };
                }
                if (result_code == "2000" || result_code == "2001" || result_code == "2002")
                {
                    ret = new SendSMSResponse()
                    {
                        ResultCode = Convert.ToInt32(result_code),
                        Description = description
                    };
                }

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, " Exception = " + ex.ToString(), 100, lines[0].ControlerName);
            }
            lines = Write2Log(lines);

            return ret;
            

        }
        public static SendSMSResponse DoSMS(SendSMSRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            SendSMSResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS_" + RequestBody.ServiceID);

            if (ValidateRequest1(RequestBody, ref lines))
            {
                string token_id = "";
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                if (service.add_zero)
                {
                    string msisdn = RequestBody.MSISDN.ToString();
                    lines = Add2Log(lines, "4th digit " + msisdn.Substring(3, 1), 100, "SendSMS");
                    if (msisdn.Substring(3, 1) != "0")
                    {
                        msisdn = msisdn.Substring(0, 3) + "0" + msisdn.Substring(3);
                        RequestBody.MSISDN = Convert.ToInt64(msisdn);
                        lines = Add2Log(lines, "adding 0 to MSISDN " + msisdn, 100, "SendSMS");
                    }
                }

                
                DLValidateSMS result = new DLValidateSMS();
                //result = DBQueries.ValidateSMSRequestLight(RequestBody, ref lines);
                result = Api.Cache.Services.ValidateSMSRequestLight(RequestBody, ref lines);

                bool is_flash = (!String.IsNullOrEmpty(RequestBody.IsFlash) ? (RequestBody.IsFlash == "1" ? true : false) : false);
                if ((result != null) && (result.RetCode == 1000))
                {
                    ServiceURLS s_u = Api.Cache.Services.GetServiceURLS(RequestBody.ServiceID, ref lines);
                    Int64 msg_id = 1;
                    string mysql_q = "insert into send_sms (msisdn, service_id, msg_content, sent_date_time, partner_transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",'" + RequestBody.Text.Replace("'", "''") + "',now(), '" + RequestBody.TransactionID + "')";
                    msg_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(mysql_q, "DBConnectionString_161", ref lines);
                    //if (s_u != null)
                    //{
                    //    if (!String.IsNullOrEmpty(s_u.sms_dlr_url) && s_u.sms_dlr_url != "0")
                    //    {
                    //        if (service.operator_id == 7 || service.operator_id == 15) //consider to remove
                    //        {

                    //        }

                    //    }
                    //}

                    if (msg_id > 0)
                    {
                        int service_id = GetServiceByMSISDNPrefix(RequestBody.MSISDN.ToString(), RequestBody.ServiceID.ToString(), out token_id, ref lines);
                        if (service_id > 0 && RequestBody.ServiceID != service_id)
                        {
                            lines = Add2Log(lines, "Service was changed to " + service_id, 100, "SendSMS");
                            RequestBody.ServiceID = service_id;
                            RequestBody.TokenID = token_id;
                            result = DBQueries.ValidateSMSRequestLight(RequestBody, ref lines);

                        }

                        service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
                        result.operator_id = service.operator_id;
                        switch (result.operator_id)
                        {
                            case 25:
                                bool sendsmscellc = CellC.SendSMSKannel(RequestBody.MSISDN.ToString(), service.sms_mt_code, RequestBody.Text, is_flash, ref lines);
                                if (sendsmscellc)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 24:
                                bool senssmsnamibia = Namibia.SendSMSNamb(RequestBody.MSISDN.ToString(), RequestBody.Text, ref lines);
                                if (senssmsnamibia)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;

                            case 23:
                                bool send_smsmadapi3 = MADAPI.SendSMSV3(RequestBody, service, ref lines);
                                if (send_smsmadapi3)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 28:
                            case 6: //MTN Congo
                                service = GetServiceByServiceID(776, ref lines);
                                bool send_smsmadapi2 = MADAPI.SendSMS(RequestBody, service, msg_id, ref lines);

                                if (send_smsmadapi2)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 3: //Ivory Coast MadAPI
                                bool send_smsmadapi1 = IC.SendSMSKannel(RequestBody.MSISDN.ToString(), service.sms_mt_code.ToString(), RequestBody.Text, is_flash, ref lines);
                                //service = GetServiceByServiceID(889, ref lines);
                                //bool send_smsmadapi1 = MADAPI.SendSMS(RequestBody, service, ref lines);
                                if (send_smsmadapi1)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;

                            case 14:
                                bool send_sms1 = Benin.SendMoovSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, ref lines);
                                if (send_sms1)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 18:
                                bool ogsend_sms1 = false;
                                if (!String.IsNullOrEmpty(RequestBody.EncryptedMSISDN))
                                {
                                    ogsend_sms1 = Orange.SendSMSCMVAS(RequestBody, service, ref lines);
                                }
                                else
                                {
                                    ogsend_sms1 = Orange.SendSMSCM(RequestBody, service, ref lines);
                                }

                                if (ogsend_sms1)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent pending DLR"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 32: // Orange Ivory Coast
                            case 15:
                                bool ogsend_sms = Orange.SendSMSKannel(RequestBody.MSISDN.ToString(), RequestBody.Text, is_flash, msg_id.ToString(), RequestBody.ServiceID.ToString(), ref lines);
                                if (ogsend_sms)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent pending DLR"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 7:
                                bool gnsend_sms = Guinea.SendSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, msg_id.ToString(), RequestBody.ServiceID.ToString(), ref lines);
                                if (gnsend_sms)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }

                                break;
                            case 12: //Benin
                            case 8://9mobile
                                bool send_sms = (result.operator_id == 8 ? NineMobile.SendSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, ref lines) : Benin.SendSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, ref lines));
                                if (send_sms)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 22:
                                bool send_sms_hml = HML.SendSMS(RequestBody.MSISDN.ToString(), RequestBody.Text, service.sms_mt_code, ref lines);
                                if (send_sms_hml)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 26:
                            case 10:
                            case 2: //Nigeria NJ
                                bool send_sms_ps = PISIMobile.SendSMS(RequestBody, service, msg_id, ref lines);
                                if (send_sms_ps)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1010,
                                        Description = "SMS Was sent Pending DLR"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 16:
                                bool send_sms_sts = STS.SendSMS(service, RequestBody, ref lines);
                                if (send_sms_sts)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 4://cameroon
                                bool send_sms_cam = Cameroon.SendSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, ref lines);
                                if (send_sms_cam)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 21:
                                bool eco_send_sms_sl = EcoNet.SendSMSSL(RequestBody.MSISDN.ToString(), RequestBody.Text, ref lines);
                                if (eco_send_sms_sl)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 17:
                                bool eco_send_sms = EcoNet.SendSMS(RequestBody.MSISDN.ToString(), RequestBody.Text, ref lines);
                                if (eco_send_sms)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            case 9://vodacom
                                bool vd_send_sms = Vodacom.SendSMSKannel(RequestBody.MSISDN.ToString(), result.SMSMTCode.ToString(), RequestBody.Text, is_flash, ref lines);
                                //bool vd_send_sms = false;// VodacomIMI.SendSMS(service, RequestBody.MSISDN.ToString(), RequestBody.Text, ref lines);
                                if (vd_send_sms)
                                {
                                    lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1000,
                                        Description = "SMS Was sent"
                                    };
                                }
                                else
                                {
                                    lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                    ret = new SendSMSResponse()
                                    {
                                        ResultCode = 1050,
                                        Description = "SMS Was not sent"
                                    };
                                }
                                break;
                            default: //mtn sdp
                                if (service.service_id == 776)
                                {
                                    bool send_smsmadapi = MADAPI.SendSMS(RequestBody, service, msg_id, ref lines);
                                    if (send_smsmadapi)
                                    {
                                        lines = Add2Log(lines, "SMS was sent", 100, "SendSMS");
                                        ret = new SendSMSResponse()
                                        {
                                            ResultCode = 1000,
                                            Description = "SMS Was sent"
                                        };
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "SMS was not sent", 100, "SendSMS");
                                        ret = new SendSMSResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "SMS Was not sent"
                                        };
                                    }
                                }
                                else
                                {
                                    string endpoint = Cache.ServerSettings.GetServerSettings("SMSEndPointURL", ref lines);
                                    string soap = BuildSendSMSSoap(result.MSISDN.ToString(), result.SPID.ToString(), result.Password, result.RealServiceID.ToString(), RequestBody.Text, result.SMSMTCode.ToString(), RequestBody.TransactionID, endpoint, RequestBody.AuthrizationID, RequestBody.LinkID);
                                    lines = Add2Log(lines, "Soap = " + soap, 100, "SendSMS");
                                    string sdp_string = "SDPSendSMSURL_" + result.operator_id + (result.is_staging == true ? "_STG" : "");
                                    string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                                    lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "SendSMS");
                                    string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                                    lines = Add2Log(lines, "SendSMS Response = " + response, 100, "SendSMS");
                                    if (response != "")
                                    {
                                        string sms_response = CommonFuncations.ProcessXML.GetXMLNode(response, "ns1:result", ref lines);
                                        if (sms_response != "")
                                        {
                                            lines = Add2Log(lines, "SMS was sent pending DLR", 100, "SendSMS");
                                            ret = new SendSMSResponse()
                                            {
                                                ResultCode = 1000,
                                                Description = "SMS Was sent pending DLR"
                                            };

                                        }
                                        else
                                        {
                                            lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
                                            ret = new SendSMSResponse()
                                            {
                                                ResultCode = 1050,
                                                Description = "SMS Was not sent"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
                                        ret = new SendSMSResponse()
                                        {
                                            ResultCode = 1050,
                                            Description = "SMS Was not sent"
                                        };
                                    }
                                }

                                break;
                        }
                    }
                    
                }
                else
                {
                    if (result == null)
                    {
                        ret = new SendSMSResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        ret = new SendSMSResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description
                        };
                    }
                }
                //}
            }
            else
            {
                ret = new SendSMSResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };

            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            lines = Add2Log(lines, text, log_level, "SendSMS");
            lines = Write2Log(lines);


            return ret;
        }

        public static SendSMSResponse DoSMS(SendSMSRequest RequestBody, int overide)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            SendSMSResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");

            ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines);
            if (service.add_zero)
            {
                string msisdn = RequestBody.MSISDN.ToString();
                lines = Add2Log(lines, "4th digit " + msisdn.Substring(3, 1), 100, "SendSMS");
                if (msisdn.Substring(3, 1) != "0")
                {
                    msisdn = msisdn.Substring(0, 3) + "0" + msisdn.Substring(3);
                    RequestBody.MSISDN = Convert.ToInt64(msisdn);
                    lines = Add2Log(lines, "adding 0 to MSISDN " + msisdn, 100, "SendSMS");
                }
            }

            if (ValidateRequest1(RequestBody, ref lines))

            {
                DLValidateSMS result = null;
                if (overide == 0)
                {
                    result = DBQueries.ValidateSMSRequest(RequestBody, ref lines);
                }
                else
                {
                    result = DBQueries.ValidateSMSRequestLight(RequestBody, ref lines);
                }
                    
                if ((result != null) && (result.RetCode == 1000))
                {
                    // send sms
                    string endpoint = Cache.ServerSettings.GetServerSettings("SMSEndPointURL", ref lines);
                    string real_service_id = (overide == 1 ? result.RealServiceID.ToString() : result.RealServiceID.ToString());
                    string soap = BuildSendSMSSoap(result.MSISDN.ToString(), result.SPID.ToString(), result.Password, real_service_id, RequestBody.Text, result.SMSMTCode.ToString(), RequestBody.TransactionID, endpoint, RequestBody.AuthrizationID, RequestBody.LinkID);
                    lines = Add2Log(lines, "Soap = " + soap, 100, "SendSMS");
                    string sdp_string = "SDPSendSMSURL_" + result.operator_id + (result.is_staging == true ? "_STG" : "");
                    string soap_url = Cache.ServerSettings.GetServerSettings(sdp_string, ref lines);
                    lines = Add2Log(lines, "Sending to URL = " + soap_url, 100, "SendSMS");
                    string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, ref lines);
                    lines = Add2Log(lines, "SendSMS Response = " + response, 100, "SendSMS");
                    if (response != "")
                    {
                        string sms_response = CommonFuncations.ProcessXML.GetXMLNode(response, "ns1:result", ref lines);
                        if (sms_response != "")
                        {
                            lines = Add2Log(lines, "SMS was sent pending DLR", 100, "SendSMS");
                            ret = new SendSMSResponse()
                            {
                                ResultCode = 1010,
                                Description = "SMS Was sent pending DLR"
                            };

                        }
                        else
                        {
                            lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
                            ret = new SendSMSResponse()
                            {
                                ResultCode = 1050,
                                Description = "SMS Was not sent"
                            };
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "Error Sending SMS!", 100, "SendSMS");
                        ret = new SendSMSResponse()
                        {
                            ResultCode = 1050,
                            Description = "SMS Was not sent"
                        };
                    }

                }
                else
                {
                    if (result == null)
                    {
                        ret = new SendSMSResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error"
                        };
                    }
                    else
                    {
                        ret = new SendSMSResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description
                        };
                    }
                }
            }
            else
            {
                ret = new SendSMSResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters"
                };

            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description;
            lines = Add2Log(lines, text, log_level, "SendSMS");
            lines = Write2Log(lines);


            return ret;
        }
    }
}