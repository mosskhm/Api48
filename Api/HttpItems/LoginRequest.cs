using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    public class LoginRequest
    {
        [DataMember(Name = "ServiceID")]
        [XmlElement(ElementName = "ServiceID")]
        public int ServiceID { get; set; }

        [DataMember(Name = "Password")]
        [XmlElement(ElementName = "Password")]
        public string Password { get; set; }


    }
}