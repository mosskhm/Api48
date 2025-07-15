using Api.DataLayer;
using Api.HttpItems;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static Api.Logger.Logger;

namespace Api.Controllers
{
    public class LoginController : ApiController
    {

        
        //public static bool ValidateRequest(LoginRequest RequestBody, ref List<LogLines> lines)
        //{
        //    bool result = false;
        //    int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
        //    if (RequestBody != null)
        //    {
        //        string text = "ServiceID = " + RequestBody.ServiceID + ", Password = " + RequestBody.Password;
        //        if ((RequestBody.ServiceID > 0) && (!String.IsNullOrEmpty(RequestBody.Password)))
        //        {
        //            result = true;
        //            lines = Add2Log(lines, text, log_level, "Login");
        //        }
        //        else
        //        {
        //            lines = Add2Log(lines, text, log_level, "Login");
        //            lines = Add2Log(lines, "Bad Params", log_level, "Login");
        //        }
        //    }
        //    return result;
        //}


        // POST: api/Login
        public LoginResponse Post([FromBody] LoginRequest RequestBody)
        {
            LoginResponse ret = CommonFuncations.Login.DoLogin(RequestBody);
            string content_type = HttpContext.Current.Request.ServerVariables["CONTENT_TYPE"];
            if (content_type == "text/xml")
            {
                HttpContext.Current.Response.ContentType = "text/xml";
            }
            else
            {
                HttpContext.Current.Response.ContentType = "application/json";
            }
            return ret;
            //int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            //LoginResponse ret = null;
            //List<LogLines> lines = new List<LogLines>();
            //lines = Add2Log(lines, "*****************************", Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]), "Login");
            //if (ValidateRequest(RequestBody, ref lines))
            //{
            //    DLValidateLogin result = DBQueries.ValidateLoginRequest(RequestBody, ref lines);
            //    if ((result != null) && (result.RetCode == 1000))
            //    {
            //        ret = new LoginResponse()
            //        {
            //            ResultCode = result.RetCode,
            //            Description = result.Description,
            //            TokenID = result.Token,
            //            TokenExpiration = result.TokenExperation
            //        };
            //    }
            //    else
            //    {
            //        if (result == null)
            //        {
            //            ret = new LoginResponse()
            //            {
            //                ResultCode = 5001,
            //                Description = "Internal Error",
            //                TokenID = "",
            //                TokenExpiration = ""

            //            };
            //        }
            //        else
            //        {
            //            ret = new LoginResponse()
            //            {
            //                ResultCode = result.RetCode,
            //                Description = result.Description,
            //                TokenID = "",
            //                TokenExpiration = ""

            //            };
            //        }
            //    }
            //}
            //else
            //{
            //    ret = new LoginResponse()
            //    {
            //        ResultCode = 2000,
            //        Description = "Bad Parameters",
            //        TokenID = "",
            //        TokenExpiration = ""

            //    };
            //}

            //string text = "RetCode = " + ret.ResultCode + ", Description = " + ret.Description + ", Token = " + ret.TokenID + ", TokenExperation = " + ret.TokenExpiration;
            //lines = Add2Log(lines, text, log_level, "Login");
            //lines = Write2Log(lines);



        }

    }
}
