using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class ValidateBankAccountResponse
    {
        [DataMember(Name = "ResultCode")]
        [XmlElement(ElementName = "ResultCode")]
        public int ResultCode { get; set; }

        [DataMember(Name = "Description")]
        [XmlElement(ElementName = "Description")]
        public string Description { get; set; }

        [DataMember(Name = "AccountHolderName")]
        [XmlElement(ElementName = "AccountHolderName")]
        public string AccountHolderName { get; set; }



    }
}