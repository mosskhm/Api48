using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for _9mobileoptin
    /// </summary>
    public class _9mobileoptin : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "9mobileoptin");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "9mobileoptin");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "9mobileoptin");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "9mobileoptin");
            string token = context.Request.QueryString["token"];
            string transaction_id = context.Request.QueryString["tid"];
            string msisdn = "", service = "", created = "", exp = "", amount = "", validity = "";
            if (!String.IsNullOrEmpty(token))
            {
                try
                {
                    var dec_token = new JwtSecurityToken(jwtEncodedString: token);
                    msisdn = dec_token.Claims.First(c => c.Type == "sub").Value;
                    service = dec_token.Claims.First(c => c.Type == "service").Value;
                    created = dec_token.Claims.First(c => c.Type == "created").Value;
                    exp = dec_token.Claims.First(c => c.Type == "exp").Value;
                    amount = dec_token.Claims.First(c => c.Type == "amount").Value;
                    validity = dec_token.Claims.First(c => c.Type == "validity").Value;
                    if (!String.IsNullOrEmpty(msisdn) && !String.IsNullOrEmpty(service) && !String.IsNullOrEmpty(created) && !String.IsNullOrEmpty(exp) && !String.IsNullOrEmpty(amount) && !String.IsNullOrEmpty(validity))
                    {

                    }
                }
                catch(Exception ex)
                {

                }
      
                
            }

//            {
//                "sub": "2348172157369",
//  "service": "LUVDOC20",
//  "created": 1525965664,
//  "account_name": "YELDOT",
//  "iss": "9mobile",
//  "exp": 1526052064,
//  "amount": "10000",
//  "validity": "1"
//}

            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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