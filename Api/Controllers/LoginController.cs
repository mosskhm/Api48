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


        // OPTIONS: api/Login - Handle CORS preflight request
        [HttpOptions]
        public void Options()
        {
            // CORS origin check for preflight request
            string origin = HttpContext.Current.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin) 
                && (
                    origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydot.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydgames.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydplatform.com", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydaplatform.com", StringComparison.OrdinalIgnoreCase)
                   )
               )
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                HttpContext.Current.Response.Headers.Add("Vary", "Origin");
            }
            
            HttpContext.Current.Response.StatusCode = 200;
        }

        // POST: api/Login
        public LoginResponse Post([FromBody] LoginRequest RequestBody)
        {
            // CORS origin check for actual request
            string origin = HttpContext.Current.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin) 
                && (
                    origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydot.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydgames.co", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydplatform.com", StringComparison.OrdinalIgnoreCase)
                    || origin.EndsWith(".ydaplatform.com", StringComparison.OrdinalIgnoreCase)
                   )
               )
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                HttpContext.Current.Response.Headers.Add("Vary", "Origin");
            }

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
