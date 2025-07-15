using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for telekom_bigcash
    /// </summary>
    public class telekom_hn : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msisdn = "0";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "telekom_handler");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "telekom_handler");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "telekom_handler");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "telekom_handler");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "telekom_handler");
            }

            string sid = (String.IsNullOrEmpty(context.Request.QueryString["sid"]) ? "0" : context.Request.QueryString["sid"]);


            string redirect_url = "https://sdp-s-vas-payment.telkom.co.za/"+ sid + "?ext_ref=123456776";
            if (!string.IsNullOrEmpty(context.Request.QueryString["msisdn"]) && !string.IsNullOrEmpty(context.Request.QueryString["result"]) && !string.IsNullOrEmpty(context.Request.QueryString["subscription_id"]))
            {
                msisdn = HttpContext.Current.Request.QueryString["msisdn"];
                msisdn = (msisdn.Substring(0, 1) == "0" ? "27" + msisdn.Substring(1) : msisdn);
                msisdn = CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1);
                HttpContext.Current.Response.Cookies["cli"].Value = msisdn;
                //redirect_url = "https://www.bigcash.co.za/telkomsa/?cli=" + msisdn;
                redirect_url = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_url from telkom_service_conf t where t.telkom_service_id = " + sid + " limit 1", ref lines);
                if (!string.IsNullOrEmpty(redirect_url))
                {
                    redirect_url = redirect_url + msisdn;
                }
            }
            else
            {
                if (context.Request.Cookies["cli"] != null)
                {
                    if (!String.IsNullOrEmpty(context.Request.Cookies["cli"].Value))
                    {
                        redirect_url = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_url from telkom_service_conf t where t.telkom_service_id = " + sid + " limit 1", ref lines);
                        if (!string.IsNullOrEmpty(redirect_url))
                        {
                            redirect_url = redirect_url + context.Request.Cookies["cli"].Value;
                        }
                    }
                }
            }

            
            lines = Add2Log(lines, " Redirecting to  = " + redirect_url, 100, "telekom_bigcash");
            lines = Write2Log(lines);
            context.Response.Redirect(redirect_url);
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