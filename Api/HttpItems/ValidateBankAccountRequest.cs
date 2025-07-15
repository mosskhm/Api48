using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class ValidateBankAccountRequest
    {
        [XmlElement(ElementName = "ServiceID")]
        [DataMember(Name = "ServiceID")]
        public int ServiceID { get; set; }

        [XmlElement(ElementName = "TokenID")]
        [DataMember(Name = "TokenID")]
        public string TokenID { get; set; }

        [XmlElement(ElementName = "BankID")]
        [DataMember(Name = "BankID")]
        public string BankID { get; set; }

        [XmlElement(ElementName = "AccountNumber")]
        [DataMember(Name = "AccountNumber")]
        public string AccountNumber { get; set; }
    }
}