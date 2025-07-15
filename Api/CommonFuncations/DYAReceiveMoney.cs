using Api.DataLayer;
using Api.HttpItems;
using Api.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class DYAReceiveMoney
    {
        public static string BuildReceiveMoneySoap(ServiceClass service,DLDYAValidateAccount result, DYAReceiveMoneyRequest RequestBody, Int64 dya_trans)
        {
            //string amount = (service.is_staging == true ? RequestBody.Amount.ToString() : (RequestBody.Amount * 100).ToString());
            string amount = (RequestBody.Amount * 100).ToString();
            string real_serviceid = "", final_password = "", timeStamp = "", soap = "";
            switch (service.operator_id)
            {
                case 2:
                    real_serviceid = "234012000010617";
                    final_password = "4ce061d876644e189661a9f1ecc24e8d";
                    timeStamp = "20160408512600";
                    soap = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:b2b=\"http://b2b.mobilemoney.mtn.zm_v1.0\">";
                    soap = soap + "<soapenv:Header>";
                    soap = soap + "<RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "<spId>" + result.SPID + "</spId>";
                    soap = soap + "<spPassword>"+ final_password + "</spPassword>";
                    soap = soap + "<serviceId>"+ real_serviceid + "</serviceId>";
                    soap = soap + "<timeStamp>"+ timeStamp + "</timeStamp>";
                    soap = soap + "</RequestSOAPHeader>";
                    soap = soap + "</soapenv:Header>";
                    soap = soap + "<soapenv:Body>";
                    soap = soap + "<b2b:processRequest>";
                    soap = soap + "<serviceId>yelldot.sp</serviceId>";
                    soap = soap + "<parameter><name>DueAmount</name><value>" + amount + "</value></parameter>";
                    soap = soap + "<parameter><name>MSISDNNum</name><value>" + RequestBody.MSISDN + "</value></parameter>";
                    soap = soap + "<parameter><name>ProcessingNumber</name><value>" + dya_trans + "</value></parameter>";
                    soap = soap + "<parameter><name>serviceId</name><value>yelldot.sp</value></parameter>";
                    soap = soap + "<parameter><name>AcctRef</name><value>1205</value></parameter>";
                    soap = soap + "<parameter><name>AcctBalance</name><value>555</value></parameter>";
                    soap = soap + "<parameter><name>MinDueAmount</name><value>121212</value></parameter>";
                    soap = soap + "<parameter><name>OpCoID</name><value>23401</value></parameter>";
                    soap = soap + "</b2b:processRequest></soapenv:Body>";
                    soap = soap + "</soapenv:Envelope>";
                    break;
                case 7:
                    real_serviceid = "";
                    final_password = "b22d21ac57d2d96b3ba304fdb97dcf49";
                    timeStamp = "20200316160659";
                    amount = RequestBody.Amount.ToString();

                    if (service.is_staging == false)
                    {
                        timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                        final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();
                    }

                    soap = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
                    soap = soap + "   <soap:Header>";
                    soap = soap + "      <ns2:RequestSOAPHeader xmlns:ns3=\"http://b2b.mobilemoney.mtn.zm_v1.0\" xmlns:ns2=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "         <spId>" + result.SPID + "</spId>";
                    soap = soap + "         <spPassword>" + final_password + "</spPassword>";
                    soap = soap + "         <serviceId/>";
                    soap = soap + "         <timeStamp>" + timeStamp + "</timeStamp>";
                    soap = soap + "      </ns2:RequestSOAPHeader>";
                    soap = soap + "   </soap:Header>";
                    soap = soap + "   <soap:Body>";
                    soap = soap + "      <ns3:processRequest xmlns:ns2=\"http://www.huawei.com.cn/schema/common/v2_1\" xmlns:ns3=\"http://b2b.mobilemoney.mtn.zm_v1.0\">";
                    //soap = soap + "         <serviceId>" + RequestBody.MSISDN + "@" + service.momo_service_id + "</serviceId>";
                    soap = soap + "         <serviceId>100</serviceId>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>DueAmount</name>";
                    soap = soap + "            <value>" + amount + "</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>MSISDNNum</name>";
                    soap = soap + "            <value>" + RequestBody.MSISDN + "</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>ProcessingNumber</name>";
                    soap = soap + "            <value>" + dya_trans + "</value>";
                    soap = soap + "         </parameter>";

                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>serviceId</name>";
                    soap = soap + "             <value>"+service.momo_service_id+"</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>AcctRef</name>";
                    soap = soap + "             <value>ECW_test</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>AcctBalance</name>";
                    soap = soap + "             <value>0</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>MinDueAmount</name>";
                    soap = soap + "             <value>0</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>Narration</name>";
                    soap = soap + "             <value>Yellowbet</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>PrefLang</name>";
                    soap = soap + "             <value>fr</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "             <name>OpCoID</name>";
                    soap = soap + "             <value>22401</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>CurrCode</name>";
                    soap = soap + "            <value>GNF</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>SenderID</name>";
                    soap = soap + "            <value>MOM</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "      </ns3:processRequest>";
                    soap = soap + "   </soap:Body>";
                    soap = soap + "</soap:Envelope>";
                    break;
                case 12:
                    real_serviceid = "";
                    final_password = "cb879e82f2796324626ba7efe8f30197";
                    timeStamp = "20181213120000";

                    if (service.is_staging == false)
                    {
                        timeStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                        final_password = md5.Encode_md5(service.spid + service.spid_password + timeStamp).ToUpper();
                    }

                    soap = "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
                    soap = soap + "   <soap:Header>";
                    soap = soap + "      <ns2:RequestSOAPHeader xmlns:ns3=\"http://b2b.mobilemoney.mtn.zm_v1.0\" xmlns:ns2=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "         <spId>"+ result.SPID + "</spId>";
                    soap = soap + "         <spPassword>"+ final_password + "</spPassword>";
                    soap = soap + "         <serviceId/>";
                    soap = soap + "         <timeStamp>"+ timeStamp + "</timeStamp>";
                    soap = soap + "      </ns2:RequestSOAPHeader>";
                    soap = soap + "   </soap:Header>";
                    soap = soap + "   <soap:Body>";
                    soap = soap + "      <ns3:processRequest xmlns:ns2=\"http://www.huawei.com.cn/schema/common/v2_1\" xmlns:ns3=\"http://b2b.mobilemoney.mtn.zm_v1.0\">";
                    soap = soap + "         <serviceId>"+ RequestBody.MSISDN + "@"+service.momo_service_id+"</serviceId>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>DueAmount</name>";
                    //soap = soap + "            <value>"+amount+"</value>";
                    soap = soap + "            <value>-1</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>MSISDNNum</name>";
                    soap = soap + "            <value>"+RequestBody.MSISDN+"</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>ProcessingNumber</name>";
                    soap = soap + "            <value>"+ dya_trans + "</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>CurrCode</name>";
                    soap = soap + "            <value>XOF</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name>SenderID</name>";
                    soap = soap + "            <value>MOM</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "      </ns3:processRequest>";
                    soap = soap + "   </soap:Body>";
                    soap = soap + "</soap:Envelope>";
                    break;
            }
            
            return soap;
        }

        public static bool ValidateRequest(DYAReceiveMoneyRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount + ", TransactionID = " + RequestBody.TransactionID;
                logMessages.Add(new { message = text, msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0) && (!String.IsNullOrEmpty(RequestBody.TransactionID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "DYAReceiveMoney");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "DYAReceiveMoney");
                    lines = Add2Log(lines, "Bad Params", log_level, "DYAReceiveMoney");
                    logMessages.Add(new { message = "Bad Params", msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });

                }
            }
            return result;
        }

        public static DYAReceiveMoneyRequest CheckFlutterWaveService(DYAReceiveMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            
            if (RequestBody.ServiceID == 732)
            {
                lines = Add2Log(lines, "Changing to 759", 100, "DYAReceiveMoney");
                ServiceClass new_service = GetServiceByServiceID(759, ref lines);
                LoginRequest LoginRequestBody1 = new LoginRequest()
                {
                    ServiceID = new_service.service_id,
                    Password = new_service.service_password
                };
                LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
                if (res1 != null)
                {
                    if (res1.ResultCode == 1000)
                    {
                        string token_id = res1.TokenID;
                        RequestBody.TokenID = token_id;
                        RequestBody.ServiceID = 759;
                    }
                }
            }

            if (RequestBody.ServiceID == 720)
            {
                if (RequestBody.MSISDN.ToString().StartsWith("23767") || RequestBody.MSISDN.ToString().StartsWith("237680") || RequestBody.MSISDN.ToString().StartsWith("237681") || RequestBody.MSISDN.ToString().StartsWith("237682") || RequestBody.MSISDN.ToString().StartsWith("237683") || RequestBody.MSISDN.ToString().StartsWith("237650") || RequestBody.MSISDN.ToString().StartsWith("237651") || RequestBody.MSISDN.ToString().StartsWith("237652") || RequestBody.MSISDN.ToString().StartsWith("237653") || RequestBody.MSISDN.ToString().StartsWith("237654"))
                {
                    Random rnd = new Random();
                    int number = rnd.Next(1, 10);
                    lines = Add2Log(lines, "Number = " + number, 100, "DYAReceiveMoney");
                    switch (number)
                    {
                        case 2:
                        case 5:
                        case 9:
                            ServiceClass new_service = GetServiceByServiceID(754, ref lines);
                            if (new_service.block_deposit == false)
                            {
                                lines = Add2Log(lines, "Changing to 754", 100, "DYAReceiveMoney");
                                LoginRequest LoginRequestBody1 = new LoginRequest()
                                {
                                    ServiceID = new_service.service_id,
                                    Password = new_service.service_password
                                };
                                LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
                                if (res1 != null)
                                {
                                    if (res1.ResultCode == 1000)
                                    {
                                        string token_id = res1.TokenID;
                                        RequestBody.TokenID = token_id;
                                        RequestBody.ServiceID = 754;
                                    }
                                }
                            }
                            

                            break;
                        default:
                            break;
                    }
                }
            }

            return RequestBody;
        }

        public static DYAReceiveMoneyResponse DoReceive(DYAReceiveMoneyRequest RequestBody)
        {
            //int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            var logMessages = new List<object>();
            Guid myGuid = Guid.NewGuid();
            string logz_id = myGuid.ToString().Replace("-","");
            string app_name = "DYAReceiveMoney";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DYAReceiveMoney");
            
            DYAReceiveMoneyResponse ret = null;
            if (ValidateRequest(RequestBody, ref lines, ref logMessages, app_name, logz_id))
            {
                RequestBody = CheckFlutterWaveService(RequestBody, ref lines);
                DYACheckAccountRequest req = new DYACheckAccountRequest()
                {
                    MSISDN = RequestBody.MSISDN,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID
                };
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines, ref logMessages, app_name, logz_id);
                string mytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //DLDYAValidateAccount result = DBQueries.ValidateDYARequest(req, ref lines, ref logMessages, app_name, logz_id);
                DLDYAValidateAccount result = Cache.Services.ValidateDYARequestLight(req, ref lines, ref logMessages, app_name, logz_id);



                if (service.add_zero)
                {
                    string msisdn = RequestBody.MSISDN.ToString();
                    lines = Add2Log(lines, "4th digit " + msisdn.Substring(3, 1), 100, "SendSMS");
                    if (msisdn.Substring(3, 1) != "0")
                    {
                        msisdn = msisdn.Substring(0, 3) + "0" + msisdn.Substring(3);
                        RequestBody.MSISDN = Convert.ToInt64(msisdn);
                        lines = Add2Log(lines, "adding 0 to MSISDN " + msisdn, 100, "SendSMS");
                        logMessages.Add(new { message = "adding 0 to MSISDN ", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                    }
                }

                if (service.whitelist_only)
                {
                    List<Int64> whitelist_msisdn = Api.Cache.USSD.GetWhiteListUsersPerService(RequestBody.ServiceID, ref lines);
                    if (whitelist_msisdn != null)
                    {
                        if (!whitelist_msisdn.Contains(RequestBody.MSISDN))
                        {
                            lines = Add2Log(lines, "Service allows whitelist users only. " + RequestBody.MSISDN + " is not in the whitelist numbers", 100, "DYAReceiveMoney");
                            result = null;
                        }
                        else
                        {
                            lines = Add2Log(lines, "User is whitelisted " + RequestBody.MSISDN, 100, "DYAReceiveMoney");
                        }
                    }
                    else
                    {
                        lines = Add2Log(lines, "no whitelist users - Service is allowed to all", 100, "DYAReceiveMoney");
                    }
                }

                
                if(service.block_deposit)
                {
                    if (RequestBody.MSISDN == 22996743244 || RequestBody.MSISDN == 22997246874 || RequestBody.MSISDN == 22996849536 || RequestBody.MSISDN == 22996691258 || RequestBody.MSISDN == 22999935800 || RequestBody.MSISDN == 22998913131)
                    {

                    }
                    else
                    {
                        result.RetCode = 1050;
                        result.Description = "Deposit is blocked";
                        logMessages.Add(new { message = "Deposit is blocked ", application = app_name, environment = "production", level = "WARNING", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                    }
                }



                if ((result != null) && (result.RetCode == 1000))
                {
                    string mysql = "insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, dya_method, transaction_id) value (" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1','',2, '" + RequestBody.TransactionID + "');";

                    Int64 dya_trans = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(mysql, ref lines, ref logMessages, app_name, logz_id);//DBQueries.InsertDYAReceiveTrans(RequestBody, mytime, ref lines);
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (service.hour_diff != "0")
                    {
                        mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                    if (dya_trans > 0)
                    {
                        if (!String.IsNullOrEmpty(RequestBody.Delay))
                        {
                            int num1;
                            bool res = int.TryParse(RequestBody.Delay, out num1);
                            if (res == true)
                            {
                                bool a = true;
                                while (a)
                                {
                                    lines = Add2Log(lines, "Sleeping for " + num1, 100, "ussd_mtnbenin");
                                    logMessages.Add(new { message = "Sleeping for " + num1, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                                    System.Threading.Thread.Sleep(num1);
                                    a = false;
                                }
                            }

                        }

                        switch (service.dya_type)
                        {
                            case 1:
                                string soap = BuildReceiveMoneySoap(service, result, RequestBody, dya_trans);
                                lines = Add2Log(lines, "Receive Money Soap = " + soap, 100, "DYAReceiveMoney");
                                string soap_url = Cache.ServerSettings.GetServerSettings("ReceiveMoneyURL_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                                lines = Add2Log(lines, "SoapURL = " + soap_url, 100, "DYAReceiveMoney");

                                List<Headers> headers = new List<Headers>();
                                if (service.operator_id == 2)
                                {
                                    headers.Add(new Headers { key = "Signature", value = "43AD232FD45FF" });
                                }
                                string StatusCode = "";
                                string StatusDesc = "";

                                


                                string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
                                if (response != "")
                                {
                                    lines = Add2Log(lines, "Response = " + response, 100, "DYAReceiveMoney");
                                    try
                                    {
                                        XmlDocument xmlDocument = new XmlDocument();
                                        xmlDocument.LoadXml(response);
                                        var root = xmlDocument.DocumentElement;
                                        foreach (XmlNode zz in xmlDocument.GetElementsByTagName("return"))
                                        {
                                            var name = zz.ChildNodes[0].InnerText;
                                            var value = zz.ChildNodes[1].InnerText;
                                            if (name == "StatusCode")
                                            {
                                                StatusCode = value;
                                            }
                                            if (name == "StatusDesc")
                                            {
                                                StatusDesc = value;
                                            }
                                        }
                                        if (StatusCode == "1000")
                                        {
                                            ret = new DYAReceiveMoneyResponse()
                                            {
                                                ResultCode = 1010,
                                                Description = StatusDesc,
                                                TransactionID = dya_trans.ToString(),
                                                Timestamp = mytime

                                            };
                                        }
                                        if (StatusCode == "01")
                                        {
                                            ret = new DYAReceiveMoneyResponse()
                                            {
                                                ResultCode = 1000,
                                                Description = StatusDesc,
                                                TransactionID = dya_trans.ToString(),
                                                Timestamp = mytime
                                            };
                                        }
                                        if (StatusCode != "01" && StatusCode != "1000")
                                        {
                                            ret = new DYAReceiveMoneyResponse()
                                            {
                                                ResultCode = 1050,
                                                Description = "Request has failed with the following error: " + StatusCode + " - " + StatusDesc,
                                                TransactionID = dya_trans.ToString(),
                                                Timestamp = mytime
                                            };
                                        }
                                        DBQueries.UpdateDYATrans(dya_trans, StatusCode, StatusDesc, ref lines);
                                    }
                                    catch (Exception ex)
                                    {
                                        ret = new DYAReceiveMoneyResponse()
                                        {
                                            ResultCode = 1030,
                                            Description = "DYA Request has failed - Network problem",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                    }

                                }
                                else
                                {
                                    ret = new DYAReceiveMoneyResponse()
                                    {
                                        ResultCode = 1030,
                                        Description = "DYA Request has failed - Network problem",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime
                                    };
                                }
                                break;
                            case 4:
                                ret = Moov.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 5:
                                ret = ECWBenin.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 6:
                                ret = Orange.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines, ref logMessages, app_name, logz_id);
                                break;
                            case 7:
                                ret = MTNOpeanAPI.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 8: //KKiaPay
                                KKiaPay k_service = GetKKiaPayServiceInfo(service, ref lines);
                                if (k_service != null)
                                {
                                    string enc_id = Api.CommonFuncations.Base64.EncodeDecodeBase64(dya_trans.ToString(), 1);
                                    ret = new DYAReceiveMoneyResponse()
                                    {
                                        ResultCode = 1010,
                                        Description = "Pending",
                                        TransactionID = dya_trans.ToString(),
                                        Timestamp = mytime,
                                        RedirectURL = @"https://cc.lnbpari.com/CreditCard/LNB/" + enc_id
                                    };
                                }
                                break;
                            case 9:
                                ret = Orange.GetWebPayURL(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 10:
                                ret = Flutterwave.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 11:
                                ret = PISIMobile.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 12:
                                ret = SharpVision.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            case 13:
                                ret = AirTelCG.ReceiveMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                break;
                            
                        }
                        
                    }
                    else
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 5002,
                            Description = "Internal Error",
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }

                }
                else
                {
                    if (result == null)
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error",
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                    else
                    {
                        ret = new DYAReceiveMoneyResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description,
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                }
            }
            string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID + ", Timestamp = " + ret.Timestamp;
            logMessages.Add(new { message = text, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
            LogzIOHelper.SendLogsAsync(logMessages,"API");
            lines = Add2Log(lines, text, 100, "DYAReceiveMoney");
            lines = Write2Log(lines);
            return ret;
        }
    }
}