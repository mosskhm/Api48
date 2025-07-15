using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    [DataContract(Namespace = "")]
    public class FulfillmentRequest
    {
        [XmlElement(ElementName = "TokenID")]
        [DataMember(Name = "TokenID")]
        public string TokenID { get; set; }

        [XmlElement(ElementName = "MSISDN")]
        [DataMember(Name = "MSISDN")]
        public Int64 MSISDN { get; set; }

        [XmlElement(ElementName = "ServiceID")]
        [DataMember(Name = "ServiceID")]
        public int ServiceID { get; set; }

        [XmlElement(ElementName = "Activate")]
        [DataMember(Name = "Activate")]
        public bool Activate { get; set; }


    }
}