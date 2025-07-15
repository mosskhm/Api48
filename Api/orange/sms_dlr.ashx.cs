using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for sms_dlr
    /// </summary>
    public class sms_dlr : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "orange_smsdlr");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "orange_smsdlr");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "orange_smsdlr");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "orange_smsdlr");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "orange_smsdlr");
            lines = Add2Log(lines, "HTTP_AUTHORIZATION = " + context.Request.ServerVariables["HTTP_AUTHORIZATION"], 100, "orange_smsdlr");
            string auth = context.Request.ServerVariables["HTTP_AUTHORIZATION"];

            //2021-03-15 18:30:11.914: Incomming XML = {"deliveryInfoNotification":{"callbackData":"74dbcf70-4e23-44a0-a929-52795ff15e02","deliveryInfo":{"address":"tel:+224627143767","deliveryStatus":"DeliveredToTerminal"}}}

            if (xml.Contains("DeliveredToTerminal"))
            {
                dynamic json_response = JsonConvert.DeserializeObject(xml);
                try
                {
                    string callbackData = json_response.deliveryInfoNotification.callbackData;
                    if (!String.IsNullOrEmpty(callbackData))
                    {
                        Int64 id = Api.DataLayer.DBQueries.SelectQueryReturnInt64("select id from orange_dlr_sms o where o.callback_id = '"+ callbackData + "'", ref lines);
                        if (id > 0)
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update marketing_camp set dlr_date = now() where id = " + id, "DBConnectionString_104", ref lines);
                            Api.DataLayer.DBQueries.ExecuteQuery("delete from orange_dlr_sms where id = " + id, ref lines);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            



            lines = Write2Log(lines);

            
            context.Response.ContentType = "text/plain";
            context.Response.Write("ok");
            
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