using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataLayer
{
    public class DLValidateSMS
    {
        public Int64 SubscriberID { get; set; }
        public Int64 MSISDN { get; set; }
        public string ServiceName { get; set; }
        public string OperatorName { get; set; }
        public string CountryName { get; set; }
        public int StateID { get; set; }
        public Int64 SPID { get; set; }
        public string Password { get; set; }
        public Int64 RealServiceID { get; set; }
        public Int64 RealProductID { get; set; }
        public Int64 SMSMTCode { get; set; }
        public string Token { get; set; }
        public string TokenExperation { get; set; }
        public int RetCode { get; set; }
        public string Description { get; set; }
        public int operator_id { get; set; }
        public bool is_staging { get; set; }
        public bool subscribe_wo_service_id { get; set; }
        public int service_id { get; set; }

    }
}