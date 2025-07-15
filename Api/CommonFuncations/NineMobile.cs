//using JamaaTech.Smpp.Net.Client;
//using JamaaTech.Smpp.Net.Lib;
//using JamaaTech.Smpp.Net.Lib.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class NineMobile
    {
        public static string BillUser(string env, string sync_async, string msisdn, string service, string amount, string transaction_id, ref List<LogLines> lines)
        {
            string response = "";
            string soap = "{ \"id\": \""+transaction_id+"\",\"amount\": \""+ Convert.ToInt32(amount)*100 + "\",\"msisdn\": \"0"+msisdn.Remove(0,3)+"\", \"serviceName\": \""+ service + "\" }";
            lines = Add2Log(lines, "Soap = " + soap, 100, lines[0].ControlerName);
            List<Headers> headers = new List<Headers>();
            
            if (env == "prod")
            {
                headers.Add(new Headers { key = "username", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingUsername_Prod", ref lines) });
                headers.Add(new Headers { key = "Authorization", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingAuth_Prod", ref lines) });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingSubKey_Prod", ref lines) });
                string url = (sync_async == "sync" ? Cache.ServerSettings.GetServerSettings("NineMobileBillingURL_Prod", ref lines) : Cache.ServerSettings.GetServerSettings("NineMobileBillingURL_Prod_Async", ref lines));
                lines = Add2Log(lines, "calling url = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "response = " + response, 100, lines[0].ControlerName);
                response = CommonFuncations.CallSoap.CallSoapRequest(url, soap, headers, 2, true, ref lines);
            }
            else
            {
                headers.Add(new Headers { key = "username", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingUsername_STG", ref lines) });
                headers.Add(new Headers { key = "Authorization", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingAuth_STG", ref lines) });
                headers.Add(new Headers { key = "Ocp-Apim-Subscription-Key", value = Cache.ServerSettings.GetServerSettings("NineMobileBillingSubKey_STG", ref lines) });
                string url = (sync_async == "sync" ? Cache.ServerSettings.GetServerSettings("NineMobileBillingURL_STG", ref lines) : Cache.ServerSettings.GetServerSettings("NineMobileBillingURL_STG_Async", ref lines));
                lines = Add2Log(lines, "calling url = " + url, 100, lines[0].ControlerName);
                lines = Add2Log(lines, "response = " + response, 100, lines[0].ControlerName);
                response = CommonFuncations.CallSoap.CallSoapRequest(url, soap, headers, 2, true, ref lines);
            }
            return response;

        }

        //public static bool SendSMS(string msisdn, string short_code, string text,ref List<LogLines> lines)
        //{
        //    bool sms_sent = true;
        //    SmppClient client = new SmppClient();
        //    SmppConnectionProperties properties = client.Properties;
        //    properties.SystemID = Cache.ServerSettings.GetServerSettings("NineMobileSMSCUserName", ref lines);
        //    properties.Password = Cache.ServerSettings.GetServerSettings("NineMobileSMSCPassword", ref lines);
        //    properties.Port = Convert.ToInt32(Cache.ServerSettings.GetServerSettings("NineMobileSMSCPort", ref lines)); //IP port to use
        //    properties.Host = Cache.ServerSettings.GetServerSettings("NineMobileSMSCHost", ref lines);  //SMSC host name or IP Address
        //    properties.SystemType = "";

        //    client.AutoReconnectDelay = Convert.ToInt32(Cache.ServerSettings.GetServerSettings("NineMobileAutoReconnectDelay", ref lines));

        //    //Send Enquire Link PDU every 15 seconds
        //    client.KeepAliveInterval = Convert.ToInt32(Cache.ServerSettings.GetServerSettings("NineMobileKeepAliveInterval", ref lines));

        //    client.Properties.InterfaceVersion = InterfaceVersion.v34;
        //    client.Properties.DefaultEncoding = DataCoding.SMSCDefault;
        //    client.Properties.SourceAddress = short_code;
        //    client.Properties.AddressNpi = NumberingPlanIndicator.Unknown;
        //    client.Properties.AddressTon = TypeOfNumber.Unknown;
        //    client.Properties.SystemType = "transceiver";
        //    client.Properties.DefaultServiceType = ServiceType.DEFAULT;// "transceiver";      
            

        //    //client.MessageReceived += mmclient_MessageReceived;
        //    //client.MessageDelivered += client_MessageDelivered;
        //    //client.MessageSent += mmclient_MessageSent;


        //    TextMessage msg = new TextMessage();
        //    msisdn = "0" + msisdn.Substring(3);
        //    msg.DestinationAddress = msisdn; //Receipient number
            
        //    msg.SourceAddress = short_code; //Originating number
        //    msg.Text = text;
        //    msg.RegisterDeliveryNotification = true; //I want delivery notification for this message

        //    try
        //    {
        //        client.Start();
        //        if (client.ConnectionState != SmppConnectionState.Connected)
        //            client.ForceConnect(5000);
        //        client.SendMessage(msg, 1000);
        //        client.Shutdown();
        //    }
        //    catch (Exception ex)
        //    {
        //        lines = Add2Log(lines, "Error sending 9Mobile SMS = " + ex.InnerException + ", " + ex.Message, 100, lines[0].ControlerName);
        //        sms_sent = false;
        //    }
            


        //    //Provide a handler for the SmppClient.MessageReceived event
            
            

            

        //    return sms_sent;
        //}


        public static bool SendSMSKannel(string msisdn, string short_code, string text, bool is_flash, ref List<LogLines> lines)
        {

            bool sms_sent = false;
            string url = Cache.ServerSettings.GetServerSettings("NineMobileKannelURL", ref lines) + "from=" + short_code + "&to=" + msisdn + "&text=" + text + "&dlr-mask=24" + (is_flash == true ? "&mclass=0" : "");
            lines = Add2Log(lines, "SMS Url = " + url, Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "SendSMS");
            string send_sms = CallSoap.GetURL(url, ref lines);
            if (send_sms.Contains("Accepted for delivery"))
            {
                sms_sent = true;
            }

            return sms_sent;
        }
    }
}