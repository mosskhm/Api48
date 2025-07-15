using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Cache.Services;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class CreditCardController : Controller
    {
        // GET: CreditCard
        public ActionResult LNB(string id)
        {

            int service_id = 745;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "LNBCC");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "LNBCC");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "LNBCC");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "LNBCC");
            bool is_success = false;
            
            if (!String.IsNullOrEmpty(id))
            {
                ViewBag.Name = "";
                string real_id = Api.CommonFuncations.Base64.EncodeDecodeBase64(id, 2);
                Int64 number;
                bool success = Int64.TryParse(real_id, out number);
                if (success)
                {
                    Int64 check_dya = Api.DataLayer.DBQueries.SelectQueryReturnInt64("SELECT d.dya_id FROM dya_transactions d WHERE d.dya_id = "+number+" AND TIME_TO_SEC(timediff(NOW(),d.date_time)) < 600 AND d.result = '-1' AND d.service_id = " + service_id, ref lines);
                    if (check_dya > 0)
                    {
                        DYATransactions dya_trans = UpdateGetDYAReciveTrans(number, "-1", "Pending", ref lines);
                        if (dya_trans != null)
                        {
                            if (dya_trans.service_id == service_id)
                            {
                                ServiceClass service = new ServiceClass();
                                service = GetServiceByServiceID(dya_trans.service_id, ref lines);
                                ViewBag.Amount = dya_trans.amount;
                                IdoBetUser user = Api.CommonFuncations.iDoBetLNBPariMTNBenin.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                string user_fullname = "", first_name = "", user_id = "";
                                
                                if (!String.IsNullOrEmpty(user.firstName))
                                {
                                    user_fullname = user.lastName + " " + user.firstName;
                                    first_name = user.firstName;
                                    user_id = user.id.ToString();
                                }
                                else
                                {
                                    user = Api.CommonFuncations.iDoBetLNBPariMoovBenin.SearchUserNew(dya_trans.msisdn.ToString(), ref lines);
                                    if (!String.IsNullOrEmpty(user.firstName))
                                    {
                                        user_fullname = user.lastName + " " + user.firstName;
                                        first_name = user.firstName;
                                        user_id = user.id.ToString();
                                    }
                                }
                                ViewBag.Name = user_fullname;
                                KKiaPay k_service = GetKKiaPayServiceInfo(service, ref lines);
                                if (k_service != null)
                                {
                                    ViewBag.KEY = k_service.api_key;
                                    ViewBag.LogoURL = k_service.logo_url;
                                    ViewBag.Position = k_service.position;
                                    ViewBag.Theme = k_service.theme;
                                    ViewBag.Sandbox = (k_service.is_staging == true ? "true" : "false");
                                    ViewBag.CallBackURL = k_service.callback_url + "?trans_id=" + id;
                                    ViewBag.FirstName = first_name;
                                    ViewBag.UserID = user_id;
                                    is_success = true;
                                }
                            }
                        }
                    }
                    
                }
            }
            string view_name = "LNB";
            if (!is_success)
            {
                view_name = "LNBError";
            }
            return View(view_name);
        }
    }
}