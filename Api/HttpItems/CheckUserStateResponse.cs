using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class CheckUserStateResponse
    {
        [DataMember(Name = "ResultCode")]
        [XmlElement(ElementName = "ResultCode")]
        public int ResultCode { get; set; }
        /// <summary>
        /// Description - Textual description
        /// </summary>
        [DataMember(Name = "Description")]
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "MSISDN")]
        [XmlElement(ElementName = "MSISDN")]
        public Int64 MSISDN { get; set; }

        [DataMember(Name = "ServiceID")]
        [XmlElement(ElementName = "ServiceID")]
        public Int64 ServiceID { get; set; }



        [DataMember(Name = "State")]
        [XmlElement(ElementName = "State")]
        public string State { get; set; }

        [DataMember(Name = "SubscriptionDate")]
        [XmlElement(ElementName = "SubscriptionDate")]
        public string SubscriptionDate { get; set; }

        [DataMember(Name = "DeactivationDate")]
        [XmlElement(ElementName = "DeactivationDate")]
        public string DeactivationDate { get; set; }

        [DataMember(Name = "LastBillingDate")]
        [XmlElement(ElementName = "LastBillingDate")]
        public string LastBillingDate { get; set; }

        [DataMember(Name = "SubscriberID")]
        [XmlElement(ElementName = "SubscriberID")]
        public Int64 SubscriberID { get; set; }
    }
}