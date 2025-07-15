using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Api.HttpItems
{
    [DataContract(Namespace = "")]
    public class SubscribeRequest
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

        
        [XmlElement(ElementName = "TransactionID")]
        [DataMember(Name = "TransactionID")]
        public string TransactionID { get; set; }

        [XmlElement(ElementName = "ActivationID")]
        [DataMember(Name = "ActivationID")]
        public string ActivationID { get; set; }

        [XmlElement(ElementName = "GetURL")]
        [DataMember(Name = "GetURL")]
        public string GetURL { get; set; }

        [XmlElement(ElementName = "AuthrizationID")]
        [DataMember(Name = "AuthrizationID")]
        public string AuthrizationID { get; set; }

        [XmlElement(ElementName = "EncryptedMSISDN")]
        [DataMember(Name = "EncryptedMSISDN")]
        public string EncryptedMSISDN { get; set; }

        public HttpRequest GetRequest()
        {
            return HttpContext.Current.Request;
        }
 
    }
}