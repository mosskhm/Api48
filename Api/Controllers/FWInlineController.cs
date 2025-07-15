using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class FWInlineController : Controller
    {
        // GET: FWInline
        public ActionResult Check(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "FWCheck");
            ViewBag.Forbiden = false;
            if (String.IsNullOrEmpty(id))
            {
                ViewBag.Forbiden = true;
            }
            else
            {
                string result = Api.DataLayer.DBQueries.SelectQueryReturnString("select result from dya_transactions d where d.dya_id = " + id, ref lines);
                string FullName = Api.DataLayer.DBQueries.SelectQueryReturnString("select full_name from dya_banktransfer_transactions d where d.dya_id = " + id, ref lines);
                string Amount = Api.DataLayer.DBQueries.SelectQueryReturnString("select amount from dya_transactions d where d.dya_id = " + id, ref lines);
                string Email = Api.DataLayer.DBQueries.SelectQueryReturnString("select Email from dya_banktransfer_transactions d where d.dya_id = " + id, ref lines);
                string MSISDN = Api.DataLayer.DBQueries.SelectQueryReturnString("select msisdn from dya_transactions d where d.dya_id = " + id, ref lines);
                string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("select service_id from dya_transactions d where d.dya_id = " + id, ref lines);

                string redirect_url = Api.DataLayer.DBQueries.SelectQueryReturnString("select redirect_url from dya_banktransfer_transactions d where d.dya_id = " + id, ref lines);
                string logo_url = Api.DataLayer.DBQueries.SelectQueryReturnString("select logo_url from dya_banktransfer_transactions d where d.dya_id = " + id, ref lines);

                if (result != "-1")
                {
                    lines = Add2Log(lines, "Transaction already updated - redirecting to " + redirect_url, 100, "FWCheck");
                    lines = Write2Log(lines);
                    return Redirect(redirect_url);
                }

                if (!String.IsNullOrEmpty(FullName) && !String.IsNullOrEmpty(Amount) && !String.IsNullOrEmpty(Email) && result == "-1")
                {
                    ViewBag.FullName = FullName;
                    ViewBag.Amount = Amount;
                    ViewBag.Email = Email;
                    ViewBag.RefID = id;
                    ViewBag.MSISDN = MSISDN;
                    ViewBag.RedirectURL = redirect_url;
                    ViewBag.LogoURL = logo_url;

                    ServiceClass service = GetServiceByServiceID(Convert.ToInt32(service_id), ref lines);
                    ViewBag.ServiceName = service.service_name;
                    string fw_key = Api.Cache.ServerSettings.GetServerSettings("FWKey", ref lines);
                    ViewBag.Key = fw_key;


                }
            }


            lines = Write2Log(lines);
            return View();
        }
    }
}