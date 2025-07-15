using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class CRMController : Controller
    {
        // GET: CRM
        

        public ActionResult Search(string id)
        {
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "********************************", 100, "CRMSearch");
            string msisdn = Request.QueryString["msisdn"];
            msisdn = (String.IsNullOrEmpty(msisdn) ? Request.Form["msisdn"] : msisdn);
                       
            ViewBag.MSISDN = msisdn;
            string redirect_url = "";

            if (!String.IsNullOrEmpty(msisdn))
            {
                if (msisdn.Contains("229"))
                {
                    IdoBetUser user = new IdoBetUser();
                    user = Api.CommonFuncations.iDoBetLNBPariMoovBenin.SearchUserNew(msisdn, ref lines);
                    if (user.isValid)
                    {
                        //user.msisdn = "+"+msisdn;
                        string zendesk_id = CommonFuncations.ZenDesk.SearchUser(msisdn, ref lines);
                        Int64 number;
                        bool success = Int64.TryParse(zendesk_id, out number);
                        if (success)
                        {
                            redirect_url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/agent/users/" + zendesk_id + "/requested_tickets";
                            zendesk_id = CommonFuncations.ZenDesk.UpdateUser(zendesk_id, user, "MTN", "yellowbet.com", "Benin", "fr-FR", ref lines);
                        }
                        else
                        {
                            //create user
                            zendesk_id = CommonFuncations.ZenDesk.CreateUser(msisdn, user, "MTN", "yellowbet.com", "Benin", "fr-FR", ref lines);
                            success = Int64.TryParse(zendesk_id, out number);
                            if (success)
                            {
                                redirect_url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/agent/users/" + zendesk_id + "/requested_tickets";
                            }
                        }
                    }

                }
                
                
            }


            string idobetid = Request.QueryString["idobet_id"];
            idobetid = (String.IsNullOrEmpty(idobetid) ? Request.Form["idobet_id"] : idobetid);
            if (!String.IsNullOrEmpty(idobetid))
            {
                IdoBetUser user = new IdoBetUser();
                user = SearchUserByUserID(idobetid, ref lines);
                
                if (user.isValid)
                {
                    msisdn = user.msisdn.Replace("+","");
                    user.msisdn = user.msisdn.Replace("+", ""); 
                    string zendesk_id = CommonFuncations.ZenDesk.SearchUser(idobetid, ref lines);
                    Int64 number;
                    bool success = Int64.TryParse(zendesk_id, out number);
                    if (success)
                    {
                        redirect_url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/agent/users/" + zendesk_id + "/requested_tickets";
                        zendesk_id = CommonFuncations.ZenDesk.UpdateUser(zendesk_id, user, "MTN", "yellowbet.com", "Benin", "fr-FR", ref lines);
                    }
                    else
                    {
                        //create user
                        zendesk_id = CommonFuncations.ZenDesk.CreateUser(msisdn, user, "MTN", "yellowbet.com", "Benin", "fr-FR", ref lines);
                        success = Int64.TryParse(zendesk_id, out number);
                        if (success)
                        {
                            redirect_url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/agent/users/" + zendesk_id + "/requested_tickets";
                        }
                    }
                }
                else //check greenwin user
                {
                    ViewBag.MSG = "User was not found";
                }
            }


            lines = Write2Log(lines);
            if (!String.IsNullOrEmpty(redirect_url))
            {
                return Redirect(redirect_url);
            }
            return View();
        }
    }
}