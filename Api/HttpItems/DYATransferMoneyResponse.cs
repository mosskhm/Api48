using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class DYATransferMoneyResponse
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

        [DataMember(Name = "TransactionID")]
        [XmlElement(ElementName = "TransactionID")]
        public string TransactionID { get; set; }

        [DataMember(Name = "Timestamp")]
        [XmlElement(ElementName = "Timestamp")]
        public string Timestamp { get; set; }
    }
}