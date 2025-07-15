using Api.CommonFuncations;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using static Api.Cache.Services;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;
using static Api.CommonFuncations.Amplitude;
using Microsoft.Ajax.Utilities;
using System.CodeDom.Compiler;
using System.Security.AccessControl;
using System.Drawing.Text;
using System.Security.Cryptography;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for ivr_subscribe
    /// </summary>
    public class ivr_subscribe : IHttpHandler
    {

        public static string GetShortURL(string msisdn, string service_id, string my_long, int campaign_id, ref List<LogLines> lines)
        {
            string result = "";
            string long_url = "";
            switch (service_id)
            {
                case "716":
                    if (!String.IsNullOrEmpty(my_long))
                    {
                        long_url = my_long + (my_long.Contains("?") ? "&" : "?") + "utm_source=YBOBD&utm_medium=OBD&utm_campaign=obdmsg" + campaign_id;
                    }
                    else
                    {
                        long_url = "http://m.yellowbet.com.gn/?utm_source=YBOBD&utm_medium=OBD&utm_campaign=obdmsg" + campaign_id;
                    }

                    result = Api.DataLayer.DBQueries.CreateYBShortURL(msisdn, long_url, campaign_id, ref lines);
                    break;
            }

            return result;
        }


        private class IVR_token
        {
            public string token { get; set; }
            public string service_name { get; set; }
        }

        private IVR_token GetLoginToken(int service_id, ref List<LogLines> lines)
        {
            // lookup service
            ServiceClass service = GetServiceByServiceID(service_id, ref lines);

            IVR_token t = new IVR_token() { service_name = service.service_name, token = null };


            // login into service
            LoginRequest LoginRequestBody = new LoginRequest()
            {
                ServiceID = service.service_id,
                Password = service.service_password
            };

            LoginResponse res = Login.DoLogin(LoginRequestBody);

            if (res == null) lines = Add2Log(lines, $"failed to login into service_id = {service_id}", 100, "");
            else if (res.ResultCode != 1000) lines = Add2Log(lines, $"failed to login into service_id = {service_id} - resultCode = {res.ResultCode}, {res.Description}", 100, "");
            else t.token = res.TokenID;

            return t;
        } // GetLoginToken


        // record session details for amplitude
        private class IVR_details
        {
            public Int64 msisdn { get; set; }
            public int service_id { get; set; }
            public string service_name { get; set; }
            public int retCode { get; set; }
            public string result { get; set; }
            public string campaign_name { get; set; }

        }

        private IVR_details UnSubscribeToService(int service_id, Int64 msisdn, string transaction_id, ref List<LogLines> lines)
        {
            // fetch login token
            IVR_token login = GetLoginToken(service_id, ref lines);

            // return object - assume the worst
            IVR_details ivr_details = new IVR_details() { service_id = service_id, service_name = login.service_name, msisdn = msisdn, retCode = 0, result = "UNSUBSCRIBE: failed" };


            if (login.token == null)
            {
                ivr_details.result = $"!! unsubscribe - failed to get login token for serviceID={service_id}";
                lines = Add2Log(lines, ivr_details.result, 100, "");
            }
            else
            {

                // subscribe user
                SubscribeResponse subscribe_response = CommonFuncations.UnSubscribe.DoUnSubscribe(
                    new SubscribeRequest()
                    {
                        ServiceID = service_id,
                        TokenID = login.token,
                        MSISDN = ivr_details.msisdn,
                        TransactionID = "ivr_unsubscribe",
                        ActivationID = "4"
                    }
                );

                if (subscribe_response == null) lines = Add2Log(lines, "!! null return from DoUnSubscribe", 100, "");
                else
                {
                    lines = Add2Log(lines, "UNSUBSCRIBE: ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");

                    // save result
                    ivr_details.retCode = subscribe_response.ResultCode;
                    ivr_details.result = "UNSUBSCRIBE: " + subscribe_response.Description;
                }
            }

            return ivr_details;
        } // UnSubscribe


        private IVR_details SubscribeToService(int service_id, Int64 msisdn, ref List<LogLines> lines)
        {

            // fetch login token
            IVR_token login = GetLoginToken(service_id, ref lines);

            // return object - assume the worst
            IVR_details ivr_details = new IVR_details() { service_id = service_id, service_name = login.service_name, msisdn = msisdn, retCode = 0, result = "SUBSCRIBE: failed" };

            if (login.token == null)
            {
                ivr_details.result = $"!! subscribe - failed to get login token for serviceID={service_id}";
                lines = Add2Log(lines, ivr_details.result, 100, "");
            }
            else
            {

                // subscribe user
                SubscribeResponse subscribe_response = CommonFuncations.Subscribe.DoSubscribe(
                    new SubscribeRequest()
                    {
                        ServiceID = service_id,
                        TokenID = login.token,
                        MSISDN = msisdn,
                        TransactionID = "ivr_subscribe",
                        ActivationID = "4"
                    }
                );

                if (subscribe_response == null) lines = Add2Log(lines, "!! null return from DoSubscribe", 100, "");
                else
                {
                    lines = Add2Log(lines, "SUBSCRIBE: ResultCode = " + subscribe_response.ResultCode + ", Description = " + subscribe_response.Description, 100, "ivr_subscribe");

                    // save result
                    ivr_details.retCode = subscribe_response.ResultCode;
                    ivr_details.result = "SUBSCRIBE: " + subscribe_response.Description;
                }
            }

            return ivr_details;
        } // SubscribeToService

        private IVR_details Process_Audio_Call(string audio_file, string cid, string dtmf, Int64 msisdn, ref List<LogLines> lines)
        {
            // lookup service attached to this auto file
            int service_id = String.IsNullOrEmpty(audio_file) || String.IsNullOrEmpty(cid) 
                             ? 0
                             : Convert.ToInt32(SelectQueryReturnInt64("select iis.service_id from ivr.in_ivr_services iis where iis.audio_file = '" + audio_file + "' and iis.did = " + cid + " and iis.dtmf = " + dtmf, ref lines))
                             ;

            // if we have a valid serviceID, then return the results of the subscribe attempt
            if (service_id != 0) return SubscribeToService(service_id, msisdn, ref lines);
            else
            {
                lines = Add2Log(lines, $"failed to fetch service ID for audio_file={audio_file} in in_ivr_services table", 100, "");
                return new IVR_details() { msisdn = msisdn, result = $"!! failed to identify service for audio_file={audio_file}" };
            } 
        } // Process_Audio_Call


        private string CRBT_Subscribe(IVRTransactionDetails dtmf_result, string transaction_id, string dtmf, ref List<LogLines> lines)
        {
            // return object
            string crbt_result = "";

            string crbt_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT o.crbt_long_id from ivr.obd_promo_services o WHERE o.service_id = " + dtmf_result.service_id + " AND o.dtmf = " + dtmf, ref lines);
            if (String.IsNullOrEmpty(crbt_id)) lines = Add2Log(lines, $"failed to determine CRBT for service_id={dtmf_result.service_id} and dtmf={dtmf}", 100, "");
            else
            {
                lines = Add2Log(lines, "User " + dtmf_result.msisdn + " from campaign " + dtmf_result.campaign_name + " want to subscribe to CRBTID " + crbt_id, 100, "");
                crbt_result = Api.CommonFuncations.crbt.SubscribeUser(dtmf_result.service_id, crbt_id, dtmf_result.msisdn.ToString(), ref lines);
                lines = Add2Log(lines, "Subscription Result " + crbt_result, 100, "");
                DataLayer.DBQueries.ExecuteQuery("update ivr.obd_promo_users set service_id = " + dtmf_result.service_id + ", subscription_ret_code = " + crbt_result + " where call_id = " + transaction_id, ref lines);
            }

            return crbt_result;
        } // CRBT_Subscribe

        private string IVR_send_sms(IVRTransactionDetails dtmf_result, string transaction_id, string short_url, ref List<LogLines> lines)
        {
            string result = "failed to send SMS";           // assume the worst

            // fetch login token
            IVR_token login = GetLoginToken(dtmf_result.service_id, ref lines);

            if (login.token == null)
            {
                result = $"!! sendsms - failed to get login token for serviceID={dtmf_result.service_id}";
                lines = Add2Log(lines, result, 100, "");
            }
            else
            {
                string sms_text = dtmf_result.sms_text.Replace("[SHORTURL]", short_url);
                SendSMSRequest sms_RequestBody = new SendSMSRequest()
                {
                    ServiceID = dtmf_result.service_id,
                    TokenID = login.token,
                    MSISDN = dtmf_result.msisdn,
                    Text = sms_text,
                    TransactionID = "12345"
                };

                lines = Add2Log(lines, " Sending SMS to MSISDN " + dtmf_result.msisdn + " " + sms_text, 100, lines[0].ControlerName);
                SendSMSResponse sms_Response = CommonFuncations.SendSMS.DoSMS(sms_RequestBody);
                if (sms_Response == null) lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                else if (sms_Response.ResultCode == 1000 || sms_Response.ResultCode == 1010)
                {
                    DataLayer.DBQueries.ExecuteQuery("update ivr.obd_promo_users set service_id = " + dtmf_result.service_id + ", subscription_ret_code = " + sms_Response.ResultCode + " where call_id = " + transaction_id, ref lines);
                    lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                    result = "successfully sent SMS";
                }
                else lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
            }

            return result;
        }

        private IVR_details Process_IVR_Promo(string transaction_id, string unique_id, string dtmf, ref List<LogLines> lines)
        {

            if (String.IsNullOrEmpty(transaction_id)) return new IVR_details() { result = "IVR PROMO: failed - transaction_id is missing" };

            // strip out promo prefix
            transaction_id = transaction_id.Replace("promo_", "");
            int iDTMF = Convert.ToInt32(dtmf);
            
            // return object -- assume the worst
            IVR_details ivr_details = new IVR_details() { service_id = 0, msisdn = 0, retCode = 0, result = $"IVR PROMO: failed to receive call details for transaction={transaction_id}" };


            // upddate the datestamp of the last call
            ExecuteQuery("update ivr.obd_promo_users set ivr_callid = '" + unique_id + "', dtmf = " + dtmf + ", dtmf_datetime = now() where call_id = " + transaction_id, ref lines);

            // fetch transaction details
            List<IVRTransactionDetails> ivr_trans_details = GetIVRTransactionDetails(transaction_id, ref lines);
            if (ivr_trans_details == null) lines = Add2Log(lines, "Call details were not found", 100, "ivr_subscribe");
            else {

                // fidn the transaction that matches this dtmf
                IVRTransactionDetails dtmf_result = ivr_trans_details.Find(x => x.dtmf == iDTMF);
                if (dtmf_result != null) lines = Add2Log(lines, $"!! failed to find {dtmf} in IVR transaction {transaction_id}", 100, ""); //dtmf was not found... check if its 9
                else
                {
                    lines = Add2Log(lines, "User " + dtmf_result.msisdn + " from campaign " + dtmf_result.campaign_name + " clicked " + dtmf, 100, "");

                    // save details
                    ivr_details.msisdn = dtmf_result.msisdn;
                    ivr_details.service_id = dtmf_result.service_id;
                    ivr_details.campaign_name = dtmf_result.campaign_name;

                    if (dtmf_result.service_id == 992)
                    {
                        ivr_details.result = "PROMO: " + CRBT_Subscribe(dtmf_result, transaction_id, dtmf, ref lines);
                        ivr_details.retCode = ivr_details.result.ToLower().Contains("fail") ? 1050 : 1000;

                    }
                    else switch (dtmf_result.campaign_type)
                        {
                            case 1:         // subscribe
                                lines = Add2Log(lines, "User " + dtmf_result.msisdn + " from campaign " + dtmf_result.campaign_name + " want to subscribe to service_id " + dtmf_result.service_id, 100, "ivr_subscribe");
                                ivr_details = SubscribeToService(dtmf_result.service_id, dtmf_result.msisdn, ref lines);
                                DataLayer.DBQueries.ExecuteQuery("update ivr.obd_promo_users set service_id = " + dtmf_result.service_id + ", subscription_ret_code = " + ivr_details.retCode + " where call_id = " + transaction_id, ref lines);

                                break;

                            case 2:
                                if (dtmf == "1")
                                {
                                    string short_url = "";
                                    string long_url = "";
                                    if (dtmf_result.sms_text.Contains("[SHORTURL]"))
                                    {
                                        string new_id = Base64.Reverse(Base64.EncodeDecodeBase64(Base64.Reverse(dtmf_result.msisdn.ToString().Substring(3)), 1).Replace("=", "")) + "O-" + dtmf_result.campaign_id;
                                        switch (dtmf_result.service_id)
                                        {
                                            case 716:
                                                long_url = "http://m.yellowbet.com.gn/?utm_source=YB&utm_medium=OBD&utm_campaign=obdmsg" + dtmf_result.campaign_id;
                                                Api.DataLayer.DBQueries.ExecuteQuery("insert into short_url_access (id, subscriber_id, real_url, creation_date, msisdn) values('" + new_id + "',1,'" + long_url + "',now(), " + dtmf_result.msisdn + ")", "DBConnectionString_104", ref lines);
                                                short_url = "http://yellow.bet/" + new_id;
                                                break;
                                        } // service_id
                                    }
                                    ivr_details.result = IVR_send_sms(dtmf_result, transaction_id, short_url, ref lines);
                                    ivr_details.retCode = ivr_details.result.ToLower().Contains("fail") ? 1050 : 1000;

                                }
                                else if (dtmf == "2") //insert into black list 
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("insert into marketing_blacklist (msisdn, service_id) values(" + dtmf_result.msisdn + "," + dtmf_result.service_id + ")", ref lines);
                                    ivr_details.result = "backlisted MSISDN";
                                    ivr_details.retCode = 1000;
                                }
                                break;


                        } // switch

                }

            }

            return ivr_details;
        } // Process_IVR_Promo

        private IVR_details Process_IVR_Content(string transaction_id, string unique_id, string dtmf, ref List<LogLines> lines)
        {

            // return object -- assume the worst
            IVR_details ivr_details = new IVR_details() { service_id = 0, msisdn = 0, retCode = 0, result = "CONTENT: failed" };

            // strip out content prefix
            transaction_id = transaction_id.Replace("content_", "");

            ExecuteQuery("update ivr.obd_content_users set ivr_callid = '" + unique_id + "', dtmf = " + dtmf + ", dtmf_datetime = now() where call_id = " + transaction_id, ref lines);
            List<IVRTransactionDetails> ivr_trans_details = GetIVRContentTransactionDetails(transaction_id, ref lines);

            if (ivr_trans_details == null)
            {
                lines = Add2Log(lines, $"!! process content - failed to find call details for transaction_id = {transaction_id}", 100, "");
                return ivr_details;
            }


            // save values for return
            ivr_details.service_id = ivr_trans_details[0].service_id;
            ivr_details.msisdn = ivr_trans_details[0].msisdn;
            ivr_details.campaign_name = ivr_trans_details[0].campaign_name;

            if  (dtmf != "9") lines = Add2Log(lines, $"process content - DTMF = {dtmf}", 100, "");
            else
            {
                ivr_details = UnSubscribeToService(ivr_trans_details[0].service_id, ivr_trans_details[0].msisdn, transaction_id, ref lines);
                DataLayer.DBQueries.ExecuteQuery("update ivr.obd_content_users set unsubscription_ret_code = " + ivr_details.retCode + " where call_id = " + transaction_id, ref lines);
            }
            return ivr_details;
        } // Process_IVR_Content

        private IVR_details Process_CallStatus(string transaction_id, string call_status, string unique_id, ref List<LogLines> lines)
        {


            // verify inputs
            if (String.IsNullOrEmpty(transaction_id) || String.IsNullOrEmpty(call_status) || String.IsNullOrEmpty(unique_id))
            {
                lines = Add2Log(lines, $"process callstatus - missing required input: transaction_id={transaction_id}, unique_id={unique_id}, call_status={call_status}", 100, "");
                return new IVR_details() { retCode = 1050, result = $"CALL STATUS: failed to process {call_status} - missing inputs" };
            }


            // return object -- assume the worst
            IVR_details ivr_details = new IVR_details() { service_id = 0, msisdn = 0, retCode = 0, result = "CALL STATUS: " + call_status };

            string call_status_id = null;
            switch (call_status)
            {
                case "NO ANSWER":
                    call_status_id = "1";
                    break;
                case "FAILED":
                    call_status_id = "2";
                    break;
                case "BUSY":
                    call_status_id = "3";
                    break;
                case "ANSWERED":
                    call_status_id = "4";
                    break;
                case "CONGESTION":
                    call_status_id = "5";
                    break;
                default:
                    ivr_details.result = "CALL STATUS: unknown type = " + call_status;
                    break;
            }

            // abort if we did not find what we are looking for
            if (String.IsNullOrEmpty(call_status_id)) return ivr_details;

            if (transaction_id.Contains("promo_"))
            {
                transaction_id = transaction_id.Replace("promo_", "");
                ExecuteQuery("update ivr.obd_promo_users set obd_status_id = " + call_status_id + ", call_status_datetime = now(), ivr_callid = '" + unique_id + "' where call_id = " + transaction_id, ref lines);

                List<IVRTransactionDetails> ivr_trans_details = GetIVRTransactionDetails(transaction_id, ref lines);
                if (ivr_trans_details == null) lines = Add2Log(lines, $" failed to find transaction details for transaction_id={transaction_id} - call status = {call_status}", 100, "");
                else
                {
                    // save values for return
                    ivr_details.service_id = ivr_trans_details[0].service_id;
                    ivr_details.msisdn = ivr_trans_details[0].msisdn;
                    ivr_details.campaign_name = ivr_trans_details[0].campaign_name;

                    //check for campaign type = 3. These campaigns must get SMS and don't require DTMF to trigger SMS
                    if (call_status_id == "4")
                    {
                        IVR_token login = null;
                        int service_id = 0;     // used to check if we need to get Login Token

                        foreach (IVRTransactionDetails transDetail in ivr_trans_details)
                        {
                            // check if we need to get a login login for this service_id
                            if (login == null || transDetail.service_id != service_id)
                            {
                                login = GetLoginToken(transDetail.service_id, ref lines);
                                if (login.token == null) lines = Add2Log(lines, $"!! failed to get login token for service_id = {transDetail.service_id}", 100, "");
                                else
                                {
                                    service_id = transDetail.service_id;       // update our current service_id
                                    ivr_details.service_id = service_id;
                                    ivr_details.service_name = login.service_name;
                                }
                            }

                            if (login.token != null)
                            {
                                //send sms to msisdn with sms text
                                string sms_text = transDetail.sms_text;
                                var smsRequestBody = new SendSMSRequest()
                                {
                                    ServiceID = transDetail.service_id,
                                    TokenID = login.token,
                                    MSISDN = transDetail.msisdn,
                                    Text = transDetail.sms_text,
                                    TransactionID = Guid.NewGuid().ToString()
                                };

                                lines = Add2Log(lines, "smsRequestBody:- " + JsonConvert.SerializeObject(smsRequestBody), 100, lines[0].ControlerName);
                                lines = Add2Log(lines, "Sending SMS to MSISDN " + transDetail.msisdn + " " + sms_text, 100, lines[0].ControlerName);

                                var sms_Response = SendSMS.DoSMS(smsRequestBody);
                                if ((sms_Response == null) || (sms_Response.ResultCode != 1000 && sms_Response.ResultCode != 1010)) lines = Add2Log(lines, " Send SMS Failed", 100, lines[0].ControlerName);
                                else
                                {
                                    DataLayer.DBQueries.ExecuteQuery("update ivr.obd_promo_users set service_id = " + transDetail.service_id + ", subscription_ret_code = " + sms_Response.ResultCode + " where call_id = " + transaction_id, ref lines);
                                    lines = Add2Log(lines, " Send SMS Was OK", 100, lines[0].ControlerName);
                                    ivr_details.retCode = sms_Response.ResultCode;
                                }
                            }
                        } // foreach
                    }
                }
            }
            else
            {
                transaction_id = transaction_id.Replace("content_", "");
                ExecuteQuery("update ivr.obd_content_users set obd_status_id = " + call_status_id + ", call_status_datetime = now(), ivr_callid = '" + unique_id + "' where call_id = " + transaction_id, ref lines);

                List<IVRTransactionDetails> ivr_trans_details = GetIVRContentTransactionDetails(transaction_id, ref lines);
                if (ivr_trans_details == null) lines = Add2Log(lines, $" failed to find transaction details for transaction_id={transaction_id} - call status = {call_status}", 100, "");
                else
                {
                    // save values for return
                    ivr_details.service_id = ivr_trans_details[0].service_id;
                    ivr_details.msisdn = ivr_trans_details[0].msisdn;
                    ivr_details.campaign_name = ivr_trans_details[0].campaign_name;
                   
                
                    // record if the call was answered
                    if (call_status_id == "4")
                    {
                        ExecuteQuery("insert into ivr.cdr (subscriber_id, content_id, date_time) SELECT * FROM (SELECT " + ivr_trans_details[0].subscriber_id + ", " + ivr_trans_details[0].content_id + ", now()) AS tmp where not exists(select c.cdr_id from ivr.cdr c where c.subscriber_id = " + ivr_trans_details[0].subscriber_id + " and c.content_id = " + ivr_trans_details[0].content_id + " and date(date_time) = date(now())) limit 1", ref lines);
                    }

                }
            }

            return ivr_details;
        } // Process_CallStatus


        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "ivr_subscribe");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ivr_subscribe");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ivr_subscribe");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ivr_subscribe");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ivr_subscribe");
            }

            // fetch inputs
            string audio_file = context.Request.QueryString["audio_file"] ?? "";
            string dtmf = context.Request.QueryString["dtmf"] ?? "";
            string cid = context.Request.QueryString["cid"] ?? "";
            string transaction_id = context.Request.QueryString["TransactionID"] ?? "";
            string call_status = context.Request.QueryString["call_status"] ?? "";
            string unique_id = context.Request.QueryString["UniqueID"] ?? "";
            string cli = context.Request.QueryString["cli"].Replace("+","").Replace(" ","").Replace("-","") ?? "";  // CLI = msisdn

            // track for amplitude
            IVR_details ivr_results = new IVR_details() { retCode = 0, result = "missing inputs" };     // assume the worst


            // the choice about what we do is dependant on the paramters provided
            if (String.IsNullOrEmpty(dtmf))                 ivr_results = Process_CallStatus(transaction_id, call_status, unique_id, ref lines);
            else if (String.IsNullOrEmpty(transaction_id))  ivr_results = Process_Audio_Call(audio_file, cid, dtmf, Convert.ToInt64(cli), ref lines);
            else if(transaction_id.Contains("promo_"))      ivr_results = Process_IVR_Promo(transaction_id, unique_id, dtmf, ref lines);
            else                                            ivr_results = Process_IVR_Content(transaction_id, unique_id, dtmf, ref lines);
 

            // Add Amplitude hook
            List<CampaignTracking> campaigns = Cache.Campaigns.GetCampaigns(ref lines);

            Api.CommonFuncations.Amplitude.Call_Amplitude(new AmplitudeRequest
            {
                msisdn = ivr_results.msisdn,
                task = "IVR",
                service_id = ivr_results.service_id,
                retcode = ivr_results.retCode,
                amount = 0,
                result_msg = "result = " + ivr_results.result,
                http = context.Request,
                billing_date = DateTime.Now,
                campaign_name = ivr_results.campaign_name,
                service_name = ivr_results.service_name,
                channel = dtmf,
                tracking_id = transaction_id
            });

            lines = Write2Log(lines);
            context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
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