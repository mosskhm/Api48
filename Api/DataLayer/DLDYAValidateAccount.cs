using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataLayer
{
    public class DLDYAValidateAccount
    {
        public string ServiceName { get; set; }
        public string OperatorName { get; set; }
        public string CountryName { get; set; }
        public Int64 SPID { get; set; }
        public bool AllowDYA { get; set; }
        public Int64 RealServiceID { get; set; }
        public string Token { get; set; }
        public string TokenExperation { get; set; }
        public int RetCode { get; set; }
        public string Description { get; set; }
        public int OperatorID { get; set; }
    }
}