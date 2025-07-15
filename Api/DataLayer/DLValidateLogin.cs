using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataLayer
{
    public class DLValidateLogin
    {
        public int RetCode { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string TokenExperation { get; set; }

        public int service_id {get;set;}
    }
}