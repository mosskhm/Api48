using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataLayer
{
    public class DLValidateGetMSISDN
    {
        public int OperatorID { get; set; }
        public string OperatorName { get; set; }
        public string HeaderName { get; set; }
        public string Description { get; set; }
        public int RetCode { get; set; }
    }
}