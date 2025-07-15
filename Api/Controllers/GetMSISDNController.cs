using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class GetMSISDNController : ApiController
    {


        
        public string Get(string id)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            string url = "";
            if (id == "ngydgames")
            {
                lines = Add2Log(lines, "*****************************", log_level, "Nigeria_Enrichment");
                lines = Add2Log(lines, "IP = " + System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"], 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "UA = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"], 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "REFERER = " + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"], 100, "Nigeria_Enrichment");
                string referer = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

                string msisdn = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_MSISDN"];
                msisdn = (String.IsNullOrEmpty(msisdn) == true ? "-1" : msisdn);
                string encoded_msisdn = (msisdn == "-1" ? "-1" : CommonFuncations.Base64.EncodeDecodeBase64(msisdn, 1));
                url = (!String.IsNullOrEmpty(referer) == true ? "http://www.ydgames.co/nigeria/home": referer);
                url = (referer.Contains("?") == true ? referer + "&cli=" + encoded_msisdn : referer + "?cli=" + encoded_msisdn);
                lines = Add2Log(lines, "msisdn = " + msisdn, 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "encoded_msisdn = " + encoded_msisdn, 100, "Nigeria_Enrichment");
                lines = Add2Log(lines, "Redirecting to URL = " + url, 100, "Nigeria_Enrichment");
                lines = Write2Log(lines);
                Redirect(url);
                System.Web.HttpContext.Current.Response.Redirect(url);
                return url;
            }
            else
            {
                lines = Add2Log(lines, "*****************************", log_level, "GetMSISDN_Get");
                string headers = string.Empty;
                foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
                {
                    headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
                }
                lines = Add2Log(lines, headers, log_level, "GetMSISDN_Get");


                lines = Write2Log(lines);
                return "OK";
            }


        }

        public static bool ValidateRequest(GetMSISDNRequest RequestBody, ref List<LogLines> lines)
        {
            bool result = false;
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            if (RequestBody != null)
            {
                string text = "ServiceID = " + RequestBody.ServiceID + ", Token = " + RequestBody.TokenID;
                if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.TokenID)))
                {
                    result = true;
                    lines = Add2Log(lines, text, log_level, "GetMSISDN");
                }
                else
                {
                    lines = Add2Log(lines, text, log_level, "GetMSISDN");
                    lines = Add2Log(lines, "Bad Params", log_level, "GetMSISDN");
                }
            }
            return result;
        }

        // POST: api/GetMSISDN
        public GetMSISDNResponse Post([FromBody] GetMSISDNRequest RequestBody)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            GetMSISDNResponse ret = null;
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", log_level, "GetMSISDN");

            string headers = string.Empty;
            foreach (var key in System.Web.HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                headers += key + "=" + System.Web.HttpContext.Current.Request.ServerVariables[key] + Environment.NewLine;
            }
            lines = Add2Log(lines, headers, log_level, "GetMSISDN");

            if (ValidateRequest(RequestBody, ref lines))
            {
                DLValidateGetMSISDN result = DBQueries.ValidateGetMSISDN(RequestBody, ref lines);
                if (result == null)
                {
                    ret = new GetMSISDNResponse()
                    {
                        ResultCode = 5001,
                        Description = "Internal Error",
                        MSISDN = 0
                    };
                }
                else
                {
                    if (result.RetCode == 1000)
                    {
                        string msisdn = System.Web.HttpContext.Current.Request.ServerVariables[result.HeaderName];
                        if (!String.IsNullOrEmpty(msisdn))
                        {
                            ret = new GetMSISDNResponse()
                            {
                                ResultCode = result.RetCode,
                                Description = result.Description,
                                MSISDN = Convert.ToInt64(msisdn)
                            };
                        }
                        else
                        {
                            ret = new GetMSISDNResponse()
                            {
                                ResultCode = 1010,
                                Description = "MSISDN header was not found",
                                MSISDN = 0
                            };
                        }
                        
                    }
                    else
                    {
                        ret = new GetMSISDNResponse()
                        {
                            ResultCode = result.RetCode,
                            Description = result.Description,
                            MSISDN = 0
                        };
                    }
                }
            }
            else
            {
                ret = new GetMSISDNResponse()
                {
                    ResultCode = 2000,
                    Description = "Bad Parameters",
                    MSISDN = 0
                };
            }

            if (ret != null)
            {
                lines = Add2Log(lines, " ResultCode = " + ret.ResultCode + " Description = " + ret.Description + " MSISDN = " + ret.MSISDN, 100, "GetMSISDN");
            }
            else
            {
                lines = Add2Log(lines, " Result was null", 100, "GetMSISDN");
            }
            
            lines = Write2Log(lines);
            
            return ret;
        }

        
    }
}
