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
using static System.Net.Mime.MediaTypeNames;

namespace Api.CommonFuncations
{
    public class DYATransferMoney
    {
        public static string BuildTransferMoneySoap(ServiceClass service, DLDYAValidateAccount result, DYATransferMoneyRequest RequestBody, Int64 dya_trans)
        {
            string amount = (RequestBody.Amount * 100).ToString(); //(service.is_staging == true ? RequestBody.Amount.ToString() : (RequestBody.Amount * 100).ToString());
            string real_serviceid = "", final_password = "", timeStamp = "", soap = "";
            switch (service.operator_id)
            {
                case 2:
                    real_serviceid = "234012000010617";
                    final_password = "4ce061d876644e189661a9f1ecc24e8d";
                    timeStamp = "20160408512600";
                    soap = "<soapenv:Envelope";
                    soap = soap + "    xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"";
                    soap = soap + "    xmlns:b2b=\"http://b2b.mobilemoney.mtn.zm_v1.0/\">";
                    soap = soap + "    <soapenv:Header>";
                    soap = soap + "        <RequestSOAPHeader";
                    soap = soap + "            xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "            <spId>" + result.SPID + "</spId>";
                    soap = soap + "            <spPassword>" + final_password + "</spPassword>";
                    soap = soap + "            <serviceId>" + real_serviceid + "</serviceId>";
                    soap = soap + "            <timeStamp>" + timeStamp + "</timeStamp>";
                    soap = soap + "        </RequestSOAPHeader>";
                    soap = soap + "    </soapenv:Header>";
                    soap = soap + "    <soapenv:Body>";
                    soap = soap + "        <b2b:processRequest>";
                    soap = soap + "            <serviceId>yelldot.sp</serviceId>";
                    soap = soap + "           <parameter>";
                    soap = soap + "                <name>ProcessingNumber</name>";
                    soap = soap + "                <value>" + dya_trans + "</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "            <parameter>";
                    soap = soap + "                <name>serviceId</name>";
                    soap = soap + "                <value>yelldot.sp</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "            <parameter>";
                    soap = soap + "                <name>OpCoID</name>";
                    soap = soap + "                <value>23401</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "            <parameter>";
                    soap = soap + "                <name>MSISDNNum</name>";
                    soap = soap + "                <value>" + RequestBody.MSISDN + "</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "            <parameter>";
                    soap = soap + "               <name>Amount</name>";
                    soap = soap + "                <value>" + amount + "</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "            <parameter>";
                    soap = soap + "                <name>Narration</name>";
                    soap = soap + "                <value>" + result.ServiceName + "</value>";
                    soap = soap + "            </parameter>";
                    soap = soap + "        </b2b:processRequest>";
                    soap = soap + "    </soapenv:Body>";
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
                    soap = "    <soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:b2b=\"http://b2b.mobilemoney.mtn.zm_v1.0\">";
                    soap = soap + "       <soapenv:Header>";
                    soap = soap + "          <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "             <spId>" + result.SPID + "</spId>";
                    soap = soap + "             <spPassword>" + final_password + "</spPassword>";
                    soap = soap + "             <serviceId/>";
                    soap = soap + "             <timeStamp>" + timeStamp + "</timeStamp>";
                    soap = soap + "          </RequestSOAPHeader>";
                    soap = soap + "       </soapenv:Header>";
                    soap = soap + "       <soapenv:Body>";
                    soap = soap + "          <b2b:processRequest>";
                    soap = soap + "             <serviceId>" + service.momo_service_id + "</serviceId>";

                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>Amount</name>";
                    soap = soap + "                <value>" + amount + "</value>";
                    soap = soap + "             </parameter>";

                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>DueAmount</name>";
                    soap = soap + "                <value>0</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>MSISDNNum</name>";
                    soap = soap + "                <value>" + RequestBody.MSISDN + "</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>ProcessingNumber</name>";
                    soap = soap + "                <value>" + dya_trans + "</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>serviceId</name>";
                    soap = soap + "                <value>" + service.momo_service_id + "</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>AcctRef</name>";
                    soap = soap + "                <value>MTNGN</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>AcctBalance</name>";
                    soap = soap + "                <value>0</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>MinDueAmount</name>";
                    soap = soap + "                <value>0</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>Narration</name>";
                    soap = soap + "                <value>" + service.momo_service_id + "</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>PrefLang</name>";
                    soap = soap + "                <value>FR</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>OpCoID</name>";
                    soap = soap + "                <value>22401</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "             <parameter>";
                    soap = soap + "                <name>CurrCode</name>";
                    soap = soap + "                <value>GNF</value>";
                    soap = soap + "             </parameter>";
                    soap = soap + "          </b2b:processRequest>";
                    soap = soap + "       </soapenv:Body>";
                    soap = soap + "    </soapenv:Envelope>";

                    //soap = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
                    //soap = soap + "   <s:Header>";
                    //soap = soap + "      <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    //soap = soap + "         <spId>" + result.SPID + "</spId>";
                    //soap = soap + "         <spPassword>" + final_password + "</spPassword>";
                    //soap = soap + "        <serviceId></serviceId>";
                    //soap = soap + "         <timeStamp>" + timeStamp + "</timeStamp>";
                    //soap = soap + "      </RequestSOAPHeader>";
                    //soap = soap + "   </s:Header>";
                    //soap = soap + "   <s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd =\"http://www.w3.org/2001/XMLSchema\">";
                    //soap = soap + "      <processRequest xmlns=\"http://b2b.mobilemoney.mtn.zm_v1.0/\">";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">Amount</name>";
                    //soap = soap + "            <value xmlns=\"\">" + amount + "</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">serviceId</name>";
                    //soap = soap + "            <value xmlns=\"\">" + service.momo_service_id + "</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">MSISDNNum</name>";
                    //soap = soap + "            <value xmlns=\"\">" + RequestBody.MSISDN + "</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">ProcessingNumber</name>";
                    //soap = soap + "            <value xmlns=\"\">" + dya_trans + "</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">appVersion</name>";
                    //soap = soap + "            <value xmlns=\"\">1.7</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">Narration</name>";
                    //soap = soap + "            <value xmlns=\"\">" + service.momo_service_id + "</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">PrefLang</name>";
                    //soap = soap + "            <value xmlns=\"\">fr</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "         <parameter>";
                    //soap = soap + "            <name xmlns=\"\">OpCoID</name>";
                    //soap = soap + "            <value xmlns=\"\">22401</value>";
                    //soap = soap + "         </parameter>";
                    //soap = soap + "      </processRequest>";
                    //soap = soap + "   </s:Body>";
                    //soap = soap + "</s:Envelope>";
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
                    soap = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
                    soap = soap + "   <s:Header>";
                    soap = soap + "      <RequestSOAPHeader xmlns=\"http://www.huawei.com.cn/schema/common/v2_1\">";
                    soap = soap + "         <spId>" + result.SPID + "</spId>";
                    soap = soap + "         <spPassword>"+final_password+"</spPassword>";
                    soap = soap + "        <serviceId></serviceId>";
                    soap = soap + "         <timeStamp>"+timeStamp+"</timeStamp>";
                    soap = soap + "      </RequestSOAPHeader>";
                    soap = soap + "   </s:Header>";
                    soap = soap + "   <s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd =\"http://www.w3.org/2001/XMLSchema\">";
                    soap = soap + "      <processRequest xmlns=\"http://b2b.mobilemoney.mtn.zm_v1.0/\">";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">Amount</name>";
                    soap = soap + "            <value xmlns=\"\">"+amount+"</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">serviceId</name>";
                    soap = soap + "            <value xmlns=\"\">"+service.momo_service_id+"</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">MSISDNNum</name>";
                    soap = soap + "            <value xmlns=\"\">"+RequestBody.MSISDN+"</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">ProcessingNumber</name>";
                    soap = soap + "            <value xmlns=\"\">"+ dya_trans + "</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">appVersion</name>";
                    soap = soap + "            <value xmlns=\"\">1.7</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">Narration</name>";
                    soap = soap + "            <value xmlns=\"\">" + service.momo_service_id + "</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">PrefLang</name>";
                    soap = soap + "            <value xmlns=\"\">fr</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "         <parameter>";
                    soap = soap + "            <name xmlns=\"\">OpCoID</name>";
                    soap = soap + "            <value xmlns=\"\">22901</value>";
                    soap = soap + "         </parameter>";
                    soap = soap + "      </processRequest>";
                    soap = soap + "   </s:Body>";
                    soap = soap + "</s:Envelope>";
                    break;
            }


            
            return soap;
        }

        public static bool ValidateRequest(DYATransferMoneyRequest RequestBody, ref List<LogLines> lines, ref List<object> logMessages, string app_name, string logz_id)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", MSISDN = " + RequestBody.MSISDN + ", TokenID = " + RequestBody.TokenID + ", Amount = " + RequestBody.Amount + ", TransactionID = " + (String.IsNullOrEmpty(RequestBody.TransactionID) ? "" : RequestBody.TransactionID) + ", OverrideDelay = " + (String.IsNullOrEmpty(RequestBody.OverrideDelay) ? "" : RequestBody.OverrideDelay);
                logMessages.Add(new { message = text, msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)) && (RequestBody.MSISDN > 0) && (RequestBody.Amount > 0))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "Login");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "Login");
                    lines = Add2Log(lines, "Bad Params", log_level, "Login");
                    logMessages.Add(new { message = "Bad Params", msisdn = RequestBody.MSISDN, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, logz_id = logz_id });
                }
            }
            return result;
        }

        public static DYATransferMoneyRequest CheckFlutterWaveService(DYATransferMoneyRequest RequestBody, ref List<LogLines> lines)
        {
            //if (RequestBody.ServiceID == 720)
            //{
            //    if (RequestBody.MSISDN.ToString().StartsWith("23767") || RequestBody.MSISDN.ToString().StartsWith("237680") || RequestBody.MSISDN.ToString().StartsWith("237681") || RequestBody.MSISDN.ToString().StartsWith("237682") || RequestBody.MSISDN.ToString().StartsWith("237683") || RequestBody.MSISDN.ToString().StartsWith("237650") || RequestBody.MSISDN.ToString().StartsWith("237651") || RequestBody.MSISDN.ToString().StartsWith("237652") || RequestBody.MSISDN.ToString().StartsWith("237653") || RequestBody.MSISDN.ToString().StartsWith("237654"))
            //    {
            //        Random rnd = new Random();
            //        int number = rnd.Next(1, 10);
            //        lines = Add2Log(lines, "Number = " + number, 100, "DYAReceiveMoney");
            //        switch (number)
            //        {
            //            case 1:
            //            case 2:
            //            case 3:
            //            case 4:
            //            case 5:
            //            case 6:
            //            case 7:
            //            case 8:
            //            case 9:
            //            case 10:
            //                lines = Add2Log(lines, "Changing to 754", 100, "DYAReceiveMoney");
            //                ServiceClass new_service = GetServiceByServiceID(754, ref lines);
            //                LoginRequest LoginRequestBody1 = new LoginRequest()
            //                {
            //                    ServiceID = new_service.service_id,
            //                    Password = new_service.service_password
            //                };
            //                LoginResponse res1 = Login.DoLogin(LoginRequestBody1);
            //                if (res1 != null)
            //                {
            //                    if (res1.ResultCode == 1000)
            //                    {
            //                        string token_id = res1.TokenID;
            //                        RequestBody.TokenID = token_id;
            //                        RequestBody.ServiceID = 754;
            //                    }
            //                }

            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}

            return RequestBody;
        }


        public static DYATransferMoneyResponse DoTransfer(DYATransferMoneyRequest RequestBody)
        {
            var logMessages = new List<object>();
            Guid myGuid = Guid.NewGuid();
            string logz_id = myGuid.ToString().Replace("-", "");
            string app_name = "DYATransferMoney";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "DYATransferMoney");
            Int64 dya_trans = -1;
            DYATransferMoneyResponse ret = null;
            string mytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string mydate = DateTime.Now.ToString("yyyy-MM-dd");
            if (ValidateRequest(RequestBody, ref lines, ref logMessages, app_name, logz_id))
            {
                RequestBody = CheckFlutterWaveService(RequestBody, ref lines);
                RequestBody.TransactionID = (String.IsNullOrEmpty(RequestBody.TransactionID) ? "" : RequestBody.TransactionID);
                DYACheckAccountRequest req = new DYACheckAccountRequest()
                {
                    MSISDN = RequestBody.MSISDN,
                    ServiceID = RequestBody.ServiceID,
                    TokenID = RequestBody.TokenID
                };
                //DLDYAValidateAccount result = DBQueries.ValidateDYARequest(req, ref lines);
                DLDYAValidateAccount result = Cache.Services.ValidateDYARequestLight(req, ref lines, ref logMessages, app_name, logz_id);
                 
                ServiceClass service = GetServiceByServiceID(RequestBody.ServiceID, ref lines, ref logMessages, app_name, logz_id);
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

                // JAN2025: reset the status of result after fetching it from the cache
                result.RetCode = 1000;      // assume all is WELL
                result.Description = "";

                string token_id = "";
                int service_id = GetServiceByMSISDNPrefix(RequestBody.MSISDN.ToString(), RequestBody.ServiceID.ToString(), out token_id, ref lines);
                if (RequestBody.ServiceID == 777 || RequestBody.ServiceID == 888)
                {
                    if (service_id == 716 && RequestBody.ServiceID == 888)
                    {
                        lines = Add2Log(lines, "MSISDN prefix do not match the service" + service_id, 100, "SendSMS");
                        result.RetCode = 1040;
                        result.Description = "MSISDN prefix do not match the service ";
                    }
                    if (service_id == 732 && RequestBody.ServiceID == 777)
                    {
                        lines = Add2Log(lines, "MSISDN prefix do not match the service" + service_id, 100, "SendSMS");
                        result.RetCode = 1040;
                        result.Description = "MSISDN prefix do not match the service ";
                    }
                }
                else
                {
                    if (service_id > 0 && RequestBody.ServiceID != service_id)
                    {
                        lines = Add2Log(lines, "MSISDN prefix do not match the service" + service_id, 100, "SendSMS");
                        result.RetCode = 1040;
                        result.Description = "MSISDN prefix do not match the service ";
                    }
                }
                

                if (service.whitelist_only)
                {
                    List<Int64> whitelist_msisdn = Api.Cache.USSD.GetWhiteListUsersPerService(RequestBody.ServiceID, ref lines);
                    if (whitelist_msisdn == null) lines = Add2Log(lines, "no whitelist users - Service is allowed to all", 100, "DYAReceiveMoney");
                    else if (whitelist_msisdn.Contains(RequestBody.MSISDN)) lines = Add2Log(lines, "User is whitelisted " + RequestBody.MSISDN, 100, "DYAReceiveMoney");
                    else
                    {
                        lines = Add2Log(lines, "Service allows whitelist users only. " + RequestBody.MSISDN + " is not in the whitelist numbers", 100, "DYAReceiveMoney");
                        result = null;
                    }
                }


                

                if ((result != null) && (result.RetCode == 1000))
                {
                    //bool delay_withdraw = (service.delay_withdraw == true && service.delay_withdraw_amount == 0 ? true : false);
                    //delay_withdraw = (delay_withdraw == false && service.delay_withdraw == true && RequestBody.Amount >= service.delay_withdraw_amount ? true : delay_withdraw);
                    bool delay_withdraw = (service.delay_withdraw == true ? true : false);
                    RequestBody.OverrideDelay = (String.IsNullOrEmpty(RequestBody.OverrideDelay) ? "" : RequestBody.OverrideDelay);
                    delay_withdraw = (RequestBody.OverrideDelay == "1" ? false : delay_withdraw);

                    lines = Add2Log(lines, " delay_withdraw = " + delay_withdraw, 100, "MOMO");
                    //lines = Add2Log(lines, RequestBody.Amount + " | " + service.delay_withdraw_amount, 100, "MOMO");
                    if (delay_withdraw)
                    {
                        lines = Add2Log(lines, "Delaying Withdraw", 100, "DYATransferMoney");
                        Int64 delay_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into delayed_withdraw_transactions (msisdn, service_id, date_time, amount, transaction_id) values(" + RequestBody.MSISDN+","+RequestBody.ServiceID+", now(), "+RequestBody.Amount+", '"+RequestBody.TransactionID+"')", ref lines, ref logMessages, app_name, logz_id);
                        string mytime1 = mytime;
                        if (service.hour_diff != "0")
                        {
                            mytime1 = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                            
                        }
                        if (delay_id > 0)
                        {
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 1000,
                                Description = "Delayed",
                                TransactionID = delay_id.ToString(),
                                Timestamp = mytime1
                            };
                        }
                        else
                        {
                            ret = new DYATransferMoneyResponse()
                            {
                                ResultCode = 5001,
                                Description = "Internal Error",
                                TransactionID = "0",
                                Timestamp = mytime1
                            };
                        }
                        
                    }
                    else
                    {
                        string check_trans = Api.DataLayer.DBQueries.CheckWithdrawTransaction(RequestBody, ref lines); // Api.DataLayer.DBQueries.SelectQueryReturnStringE("SELECT d.dya_id FROM dya_transactions d WHERE d.service_id = " + RequestBody.ServiceID + " AND d.msisdn = " + RequestBody.MSISDN + " AND d.transaction_id = '" + RequestBody.TransactionID + "' and d.result = '01' LIMIT 1", ref lines);
                        Int64 prev_dyaID = String.IsNullOrEmpty(check_trans) ? 0 : Convert.ToInt64(check_trans);
                        if (prev_dyaID > 0)
                        {
                            lines = Add2Log(lines, $"Found existing transaction dya_id={check_trans} for transactionID={RequestBody.TransactionID}, msisdn={RequestBody.MSISDN}", 100, "DYATransferMoney");
                            result.RetCode = 1041;
                            result.Description = "Transaction Already Exists for this user ";
                        } 
                        
                        if ((result != null) && (result.RetCode == 1000))
                        {
                            //dya_trans = DBQueries.InsertDYATrans(RequestBody, mytime, ref lines);
                            dya_trans = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into dya_transactions (msisdn, service_id, date_time, amount, result, result_desc, transaction_id, dya_method) value (" + RequestBody.MSISDN + ","+ RequestBody.ServiceID + ",now()," + RequestBody.Amount + ",'-1','','" + RequestBody.TransactionID + "', 1)", ref lines, ref logMessages, app_name, logz_id);
                            lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                            if (service.hour_diff != "0")
                            {
                                mytime = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd HH:mm:ss");
                                mydate = Convert.ToDateTime(mytime).AddHours(Convert.ToDouble(service.hour_diff)).ToString("yyyy-MM-dd");
                            }
                            lines = Add2Log(lines, " mytime = " + mytime, 100, "MOMO");
                            if (dya_trans > 0)
                            {

                                if (service.daily_momo_limit > 0)
                                {
                                    Int64 current_momo_daily_amount = DBQueries.SelectQueryReturnInt64("SELECT s.current_momo FROM service_momo_limitation s WHERE s.service_id = " + RequestBody.ServiceID, ref lines, ref logMessages, app_name, logz_id);
                                    if (current_momo_daily_amount >= service.daily_momo_limit)
                                    {
                                        lines = Add2Log(lines, "Limit has reached", 100, "DYATransferMoney");
                                        logMessages.Add(new { message = "Limit has reached ("+ current_momo_daily_amount + ")", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });

                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Transfer has failed with the following error: withdraw limit reached",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                        DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'Withdraw limit reached' where dya_id = " + dya_trans, ref lines, ref logMessages, app_name, logz_id);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "current_momo_daily_amount = " + current_momo_daily_amount + " < " + service.daily_momo_limit, 100, "DYATransferMoney");
                                    }
                                }
                                if (ret == null && service.user_w_limit > 0)
                                {
                                    Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_w FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines, ref logMessages, app_name, logz_id);
                                    if (user_withdraw_limit >= service.user_w_limit)
                                    {
                                        lines = Add2Log(lines, "User Limit has reached", 100, "DYATransferMoney");
                                        logMessages.Add(new { message = "User Limit has reached (" + user_withdraw_limit + ")", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Transfer has failed with the following error: user withdraw limit reached",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                        DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw limit reached' where dya_id = " + dya_trans, ref lines, ref logMessages, app_name, logz_id);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "user_withdraw_limit = " + user_withdraw_limit + " < " + service.user_w_limit, 100, "DYATransferMoney");
                                    }
                                }

                                if (ret == null && service.user_f_limit > 0)
                                {
                                    Int64 user_withdraw_limit = DBQueries.SelectQueryReturnInt64("SELECT d.number_of_f FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines, ref logMessages, app_name, logz_id);
                                    if (user_withdraw_limit >= service.user_f_limit)
                                    {
                                        lines = Add2Log(lines, "User Failed Limit has reached", 100, "DYATransferMoney");
                                        logMessages.Add(new { message = "User Failed Limit has reached (" + user_withdraw_limit + ")", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Transfer has failed with the following error: user withdraw failed limit reached",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                        DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw failed limit reached' where dya_id = " + dya_trans, ref lines, ref logMessages, app_name, logz_id);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "user_failed_withdraw_limit = " + user_withdraw_limit + " < " + service.user_f_limit, 100, "DYATransferMoney");
                                    }
                                }

                                if (ret == null && service.user_limit_amount > 0)
                                {
                                    Int64 user_withdraw_limit_amount = DBQueries.SelectQueryReturnInt64("SELECT d.amount FROM daily_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " and date_time = '" + mydate + "'", ref lines, ref logMessages, app_name, logz_id);
                                    if (user_withdraw_limit_amount >= service.user_limit_amount)
                                    {
                                        lines = Add2Log(lines, "User Limit amount has reached", 100, "DYATransferMoney");
                                        logMessages.Add(new { message = "User Limit amount has reached ("+ user_withdraw_limit_amount + ")", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Transfer has failed with the following error: user withdraw amount limit reached",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                        DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'User Withdraw amount limit reached' where dya_id = " + dya_trans, ref lines, ref logMessages, app_name, logz_id);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "user_withdraw_limit_amount = " + user_withdraw_limit_amount + " < " + service.user_limit_amount, 100, "DYATransferMoney");
                                    }
                                }
                                if (ret == null && service.timeout_between_failed_withdraw > 0)
                                {
                                    Int64 last_failed_withdraw = DBQueries.SelectQueryReturnInt64("SELECT TIMESTAMPDIFF(SECOND,d.date_time, NOW())+1 FROM daily_failed_withdrawls d WHERE d.msisdn = " + RequestBody.MSISDN + " order by d.id desc limit 1", ref lines, ref logMessages, app_name, logz_id);
                                    if (last_failed_withdraw > 0 && last_failed_withdraw < service.timeout_between_failed_withdraw)
                                    {
                                        lines = Add2Log(lines, "Suspending request timeout failed withdraw " + last_failed_withdraw + " < " + service.timeout_between_failed_withdraw, 100, "DYATransferMoney");
                                        logMessages.Add(new { message = "Suspending request timeout failed withdraw (" + last_failed_withdraw + " < "+ service.timeout_between_failed_withdraw + ")", application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                                        ret = new DYATransferMoneyResponse()
                                        {
                                            ResultCode = 1040,
                                            Description = "Transfer has failed with the following error: Timeout failed withdraw",
                                            TransactionID = dya_trans.ToString(),
                                            Timestamp = mytime
                                        };
                                        DBQueries.ExecuteQuery("update dya_transactions set result = '1040', result_desc = 'Timeout failed withdraw' where dya_id = " + dya_trans, ref lines, ref logMessages, app_name, logz_id);
                                    }
                                    else
                                    {
                                        lines = Add2Log(lines, "last_failed_withdraw = " + last_failed_withdraw, 100, "DYATransferMoney");
                                    }
                                }
                                if (ret == null)
                                {
                                    switch (service.dya_type)
                                    {
                                        case 1:
                                            string soap_url = Cache.ServerSettings.GetServerSettings("TransferMoneyURL_" + service.operator_id + (service.is_staging == true ? "_STG" : ""), ref lines);
                                            lines = Add2Log(lines, "SoapURL = " + soap_url, 100, "DYATransferMoney");

                                            string soap = BuildTransferMoneySoap(service, result, RequestBody, dya_trans);
                                            lines = Add2Log(lines, "Transfer Money Soap = " + soap, 100, "DYATransferMoney");

                                            List<Headers> headers = new List<Headers>();
                                            if (service.operator_id == 2)
                                            {
                                                headers.Add(new Headers { key = "Signature", value = "43AD232FD45FF" });
                                            }
                                            string StatusCode = "";
                                            string StatusDesc = "";
                                            string MOMTransactionID = "";
                                            string response = CommonFuncations.CallSoap.CallSoapRequest(soap_url, soap, headers, ref lines);
                                            if (response != "")
                                            {
                                                lines = Add2Log(lines, "Response = " + response, 100, "DYACheckAccount");
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
                                                        if (name == "MOMTransactionID")
                                                        {
                                                            MOMTransactionID = value;
                                                        }
                                                    }
                                                    if (StatusCode == "01")
                                                    {
                                                        ret = new DYATransferMoneyResponse()
                                                        {
                                                            ResultCode = 1000,
                                                            Description = StatusDesc,
                                                            TransactionID = dya_trans.ToString(),
                                                            Timestamp = mytime
                                                        };
                                                    }
                                                    else
                                                    {
                                                        lines = Add2Log(lines, " ReChecking Transaction... " + dya_trans, 100, "MOMO");
                                                        DYACheckTransactionRequest momo_request = new DYACheckTransactionRequest()
                                                        {
                                                            MSISDN = RequestBody.MSISDN,
                                                            ServiceID = RequestBody.ServiceID,
                                                            TokenID = RequestBody.TokenID,
                                                            TransactionID = dya_trans.ToString()
                                                        };
                                                        DYACheckTransactionResponse momo_res = DYACheckTransaction.DODYACheckTransaction(momo_request);
                                                        if (momo_res.ResultCode == 1000)
                                                        {
                                                            StatusCode = "01";
                                                            StatusDesc = "Successfully processed transaction.";
                                                            ret = new DYATransferMoneyResponse()
                                                            {
                                                                ResultCode = 1000,
                                                                Description = StatusDesc,
                                                                TransactionID = dya_trans.ToString(),
                                                                Timestamp = mytime
                                                            };
                                                        }
                                                        else
                                                        {
                                                            //insert into failed transactions
                                                            DBQueries.ExecuteQuery("insert into daily_failed_withdrawls (msisdn, amount, date_time) values(" + RequestBody.MSISDN + "," + RequestBody.Amount + ",now())", ref lines);
                                                            ret = new DYATransferMoneyResponse()
                                                            {
                                                                ResultCode = 1050,
                                                                Description = "Transfer has failed with the following error: " + StatusCode + " - " + StatusDesc,
                                                                TransactionID = dya_trans.ToString(),
                                                                Timestamp = mytime
                                                            };
                                                        }
                                                    }
                                                    DBQueries.UpdateDYATrans(dya_trans, StatusCode, StatusDesc, ref lines);
                                                    if (!String.IsNullOrEmpty(MOMTransactionID))
                                                    {
                                                        Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transaction_external_id (dya_id, external_id) values(" + dya_trans + ",'" + MOMTransactionID + "');", ref lines);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    ret = new DYATransferMoneyResponse()
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

                                                ret = new DYATransferMoneyResponse()
                                                {
                                                    ResultCode = 1030,
                                                    Description = "DYA Request has failed - Network problem",
                                                    TransactionID = dya_trans.ToString(),
                                                    Timestamp = mytime
                                                };
                                            }
                                            break;
                                        case 3:
                                            ret = TrustVas.DoTransfer(RequestBody, dya_trans, mytime, ref lines);
                                            break;
                                        case 4: //Moov
                                            ret = Moov.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                        case 5: //ECWBenin
                                            ret = ECWBenin.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                        case 6: //Orange
                                            ret = Orange.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines, ref logMessages, app_name, logz_id);
                                            break;
                                        case 7:
                                            ret = MTNOpeanAPI.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                        case 10:
                                            ret = Flutterwave.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                        case 12:
                                            ret = SharpVision.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                        case 13:
                                            ret = AirTelCG.TransferMoney(RequestBody, dya_trans.ToString(), service, mytime, ref lines);
                                            break;
                                    }
                                }


                            }
                            else
                            {
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 5002,
                                    Description = "Internal Error",
                                    TransactionID = dya_trans.ToString(),
                                    Timestamp = mytime
                                };
                            }
                        }
                        else
                        {
                            if (result == null)
                            {
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = 5001,
                                    Description = "Internal Error",
                                    TransactionID = "-1",
                                    Timestamp = mytime
                                };
                            }
                            else
                            {
                                ret = new DYATransferMoneyResponse()
                                {
                                    ResultCode = result.RetCode,
                                    Description = result.Description,
                                    TransactionID = "-1",
                                    Timestamp = mytime
                                };
                            }
                        }
                        
                    }
                    
                    
                }
                else
                {
                    if (result == null)
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = 5001,
                            Description = "Internal Error",
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                    else
                    {
                        ret = new DYATransferMoneyResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description,
                            TransactionID = "-1",
                            Timestamp = mytime
                        };
                    }
                }
            }
            try
            {
                string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", TransactionID = " + ret.TransactionID + ", Timestamp = " + ret.Timestamp;
                logMessages.Add(new { message = text, application = app_name, environment = "production", level = "INFO", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
                string ts = Convert.ToDateTime(ret.Timestamp).ToString("yyyy-MM-dd");
                if (ret.ResultCode == 1000 && ret.Description != "Delayed")
                {
                    Api.DataLayer.DBQueries.ExecuteQuery("INSERT INTO daily_withdrawls (`msisdn`, number_of_w, amount, idobet_id, date_time, service_id) VALUES (" + RequestBody.MSISDN + ", 1, " + RequestBody.Amount + ",0,'" + ts + "',"+RequestBody.ServiceID+") ON DUPLICATE KEY UPDATE number_of_w = number_of_w + 1, amount = amount + " + RequestBody.Amount + ", date_time = '" + ts + "', service_id = "+RequestBody.ServiceID+"", ref lines, ref logMessages, app_name, logz_id);
                    Api.DataLayer.DBQueries.ExecuteQuery("insert into dya_transactions_success values ('" + RequestBody.TransactionID + "'," + ret.TransactionID + "," + RequestBody.MSISDN + "," + RequestBody.ServiceID + ")", ref lines, ref logMessages, app_name, logz_id);
                }
                else
                {
                    if (ret.ResultCode == 1030)
                    {
                        //lines = Add2Log(lines, "Delaying Withdraw due to Network problems", 100, "DYATransferMoney");
                        //Int64 delay_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into delayed_withdraw_transactions (msisdn, service_id, date_time, amount, transaction_id) values(" + RequestBody.MSISDN + "," + RequestBody.ServiceID + ", now(), " + RequestBody.Amount + ", '" + RequestBody.TransactionID + "')", ref lines);
                        //ret = new DYATransferMoneyResponse()
                        //{
                        //    ResultCode = 1000,
                        //    Description = "Delayed",
                        //    TransactionID = delay_id.ToString(),
                        //    Timestamp = mytime
                        //};
                        DBQueries.UpdateDYATrans(dya_trans, "1030", "DYA Request has failed - Network problem", ref lines);
                        //DBQueries.ExecuteQuery("insert into daily_failed_withdrawls (msisdn, amount, date_time) values(" + RequestBody.MSISDN + "," + RequestBody.Amount + ",now())", ref lines);
                        Api.DataLayer.DBQueries.ExecuteQuery("INSERT INTO daily_withdrawls (`msisdn`, idobet_id, number_of_f, date_time, service_id) VALUES (" + RequestBody.MSISDN + ", 0, 1,'" + ts + "',"+RequestBody.ServiceID+") ON DUPLICATE KEY UPDATE number_of_f = number_of_f + 1, date_time = '" + ts + "', service_id = "+RequestBody.ServiceID+"", ref lines, ref logMessages, app_name, logz_id);
                    }
                }
                
                lines = Add2Log(lines, text, 100, "DYATransferMoney");
            }
            catch (Exception ex)
            {
                ret = new DYATransferMoneyResponse()
                {
                    ResultCode = 5001,
                    Description = "Internal Error",
                    TransactionID = "-1",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                logMessages.Add(new { message = ex.ToString(), application = app_name, environment = "production", level = "ERROR", timestamp = DateTime.UtcNow, msisdn = RequestBody.MSISDN, logz_id = logz_id });
            }

            LogzIOHelper.SendLogsAsync(logMessages, "API");
            lines = Write2Log(lines);
            return ret;
        }
    }
}