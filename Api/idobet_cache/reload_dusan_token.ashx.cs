using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.idobet_cache
{
    /// <summary>
    /// Summary description for reload_dusan_token
    /// </summary>
    public class reload_dusan_token : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            context.Response.ContentType = "text/plain";
            lines = Add2Log(lines, "*****************************", 100, "reload_dusan_token");
            string service_id = context.Request.QueryString["service_id"];

            ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
            string token = Api.CommonFuncations.DusanLotto.GetToken(service, ref lines);
            if (!String.IsNullOrEmpty(token))
            {
                string mydate = DateTime.Now.AddHours(5).ToString("yyyy-MM-dd HH:mm:ss");
                Api.DataLayer.DBQueries.ExecuteQuery("update lotto_service_configuration set token_id = '" + token + "', exp_date = '" + mydate + "' where service_id = " + service.service_id, ref lines);
                List<DusnServiceInfo> result_list = Api.DataLayer.DBQueries.GetDusanServiceInfo(ref lines);
                HttpContext.Current.Application["GetDusanLottoServiceInfo"] = result_list;
            }



            lines = Write2Log(lines);

            context.Response.Write("Done");
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