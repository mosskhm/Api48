using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class GetBalanceResponse
    {
        [DataMember(Name = "RetCode")]
        [XmlElement(ElementName = "RetCode")]
        public int RetCode { get; set; }

        [DataMember(Name = "Description")]
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "AvailableBalance")]
        [XmlElement(ElementName = "AvailableBalance")]
        public string AvailableBalance { get; set; }

    }
}