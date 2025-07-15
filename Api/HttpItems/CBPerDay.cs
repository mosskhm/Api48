using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.HttpItems
{
    public class CBPerDay
    {
        public string Day { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int TotalSubs { get; set; }
        public int TotalDeactivation { get; set; }
        public int Churns { get; set; }
        public int BilledUsers { get; set; }
        public double UserSpentLocalCurency { get; set; }
        public int CBGrowth { get; set; }
        public double YDRevenuLocalCurency { get; set; }
        public double YDRevenuLocalUSD { get; set; }
        
    }
}