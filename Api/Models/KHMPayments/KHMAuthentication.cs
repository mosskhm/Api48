using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models.KHMPayments
{
    public class KHMAuthentication
    {
        public string emailAddress { get; set; }
        public string password { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class KHMAuthenticationResponse
    {
        public string token { get; set; }
        public string userName { get; set; }
        public string validaty { get; set; }
        public object refreshToken { get; set; }
        public string id { get; set; }
        public string emailId { get; set; }
        public string guidId { get; set; }
        public string expiredTime { get; set; }
        public string tokenMessage { get; set; }
    }
}
