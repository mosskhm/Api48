using Api.CommonFuncations;
using Microsoft.Web.Services2.Referral;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.PiSi
{
    /// <summary>
    /// Summary description for newpisi_subcallback
    /// </summary>
    public class newpisi_secured : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            //DeactivationDate
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "NewPisiSecureDCallback");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "NewPisiSecureDCallback");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "NewPisiSecureDCallback");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "NewPisiSecureDCallback");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "NewPisiSecureDCallback");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "NewPisiSecureDCallback");
            }
            string msisdn = context.Request.QueryString["msisdn"];
            string tracking_id = context.Request.QueryString["tracking_id"];


            bool cont = true;
            if (!String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(tracking_id))
            {
                try
                {
                    Int64 tracking_id_i64 = Convert.ToInt64(tracking_id);
                    if (tracking_id_i64 > 100000000)
                    {
                        cont = false;
                    }


                }
                catch(Exception ex)
                {

                }
                if (cont)
                {
                    string tc_msisdn = Api.DataLayer.DBQueries.SelectQueryReturnString("select msisdn from tracking.tracking_requests tc where tc.id = " + tracking_id, ref lines);
                    if (!String.IsNullOrEmpty(tc_msisdn))
                    {
                        if (tc_msisdn == "-1")
                        {
                            Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests tc set tc.msisdn = " + msisdn + " where id = " + tracking_id, ref lines);
                            string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT tc.subscribe_service_id FROM tracking.tracking_requests tr, tracking.tracking_campaign tc WHERE tc.campaign_id = tr.campaign_id AND tr.id = " + tracking_id, ref lines);
                            if (!String.IsNullOrEmpty(service_id))
                            {
                                lines = Add2Log(lines, "Sleeping for 2 seconds", 100, "NewPisiSecureDCallback");
                                Thread.Sleep(2000);
                                string sub_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT IFNULL((select subs.subscriber_id FROM yellowdot.subscribers subs WHERE subs.msisdn = " + msisdn + " AND subs.service_id = "+ service_id +" AND subs.subscription_date BETWEEN DATE_SUB(NOW(), INTERVAL 2 HOUR) AND DATE_ADD(NOW(), INTERVAL 2 HOUR) ORDER BY subs.subscriber_id DESC LIMIT 1),'0')", ref lines);
                                if (!String.IsNullOrEmpty(sub_id))
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests tc set tc.subscriber_id = " + sub_id + " where id = " + tracking_id, ref lines);
                                }
                            }
                        }
                    }
                }
                
            }
            

            lines = Write2Log(lines);
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\":\"OK\"}");
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