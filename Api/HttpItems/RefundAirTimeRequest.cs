using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class RefundAirTimeRequest
    {
        [XmlElement(ElementName = "ServiceID")]
        [DataMember(Name = "ServiceID")]
        public int ServiceID { get; set; }

        [XmlElement(ElementName = "TokenID")]
        [DataMember(Name = "TokenID")]
        public string TokenID { get; set; }

        [XmlElement(ElementName = "MSISDN")]
        [DataMember(Name = "MSISDN")]
        public Int64 MSISDN { get; set; }

        [XmlElement(ElementName = "Amount")]
        [DataMember(Name = "Amount")]
        public int Amount { get; set; }

        [XmlElement(ElementName = "TransactionID")]
        [DataMember(Name = "TransactionID")]
        public string TransactionID { get; set; }

        [XmlElement(ElementName = "OverrideDelay")]
        [DataMember(Name = "OverrideDelay")]
        public string OverrideDelay { get; set; }
    }
}