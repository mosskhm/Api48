using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class DYACheckAccountResponse
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

        [DataMember(Name = "FirstName")]
        [XmlElement(ElementName = "FirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName")]
        [XmlElement(ElementName = "LastName")]
        public string LastName { get; set; }

    }
}