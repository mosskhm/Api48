using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.DEP
{
    /// <summary>
    /// Summary description for ydgames
    /// </summary>
    public class ydgames : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string msisdn = "0";
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "dep_ydgames");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "dep_ydgames");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "dep_ydgames");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "dep_ydgames");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "dep_ydgames");
            }
            string REFERER = (String.IsNullOrEmpty(context.Request.ServerVariables["HTTP_REFERER"]) ? "" : context.Request.ServerVariables["HTTP_REFERER"]);
            
            string result = context.Request.QueryString["result"];
            string subscription_id = context.Request.QueryString["subscription_id"];
            msisdn = context.Request.QueryString["msisdn"];
            string channel = context.Request.QueryString["channel"];
            string ext_ref = context.Request.QueryString["ext_ref"];

            string redirect_url = "http://doi.dep.mtn.co.za/service/7551?ext_ref=123456776";
            if (!string.IsNullOrEmpty(context.Request.QueryString["msisdn"]) && !string.IsNullOrEmpty(context.Request.QueryString["result"]) && !string.IsNullOrEmpty(context.Request.QueryString["subscription_id"]))
            {
                //msisdn = (msisdn.Substring(0, 1) == "0" ? "27" + msisdn.Substring(1) : msisdn);
                Int64 number;
                bool success = Int64.TryParse(ext_ref, out number);
                if (REFERER.Contains("doi.mtndep.co.za"))
                {
                    string real_subid = Api.DataLayer.DBQueries.SelectQueryReturnString("select subscriber_id from dep_subscribers where dep_subscription_id = " + subscription_id, ref lines);
                    if (String.IsNullOrEmpty(real_subid))
                    {
                        string mytime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        real_subid = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64("insert into subscribers (msisdn, service_id, subscription_date, state_id) values(" + msisdn + ",774,'" + mytime + "',1)", ref lines).ToString();
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into dep_subscribers (dep_subscription_id, subscriber_id, channel_name) values(" + subscription_id + "," + real_subid + ",'" + channel + "')", ref lines);
                    }
                    if (success)
                    {
                        Api.DataLayer.DBQueries.ExecuteQuery("update tracking.tracking_requests tr set tr.msisdn = " + msisdn + ", subscriber_id = " + real_subid + " where id = " + number, ref lines);
                    }
                    
                }

                msisdn = CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1);
                HttpContext.Current.Response.Cookies["cli"].Value = msisdn;
                redirect_url = "https://ydgames.netlify.app/redirect.html?cli=" + msisdn + "&sid=774";
            }

            if (context.Request.Cookies["cli"] != null)
            {
                if (!String.IsNullOrEmpty(context.Request.Cookies["cli"].Value))
                {
                    redirect_url = "https://ydgames.netlify.app/redirect.html?cli=" + context.Request.Cookies["cli"].Value + "&sid=774";
                }
            }

            lines = Add2Log(lines, " Redirecting to  = " + redirect_url, 100, "dep_ydgames");
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