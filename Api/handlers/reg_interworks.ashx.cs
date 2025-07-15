using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for reg_interworks
    /// </summary>
    public class reg_interworks : IHttpHandler
    {

        public class BSSAccount
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string RegistrationNumber { get; set; }
            public string WebSite { get; set; }
            public string Description { get; set; }
            public string Email { get; set; }
            public Address Address { get; set; }
            public bool EnableOrdering { get; set; }
            public bool EnableReselling { get; set; }
            public string PaymentMethod { get; set; }
            public bool IsTaxable { get; set; }
            public bool AutoInvoiceNotificationEnabled { get; set; }
            public string Currency { get; set; }
            public string CurrencyCode { get; set; }
        }

        public class Address
        {
            public string Address1 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
        }

        public class BSContact
        {
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string MobilePhone { get; set; }
            public int AccountId { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public bool IsPrimary { get; set; }
            public bool IsBillTo { get; set; }
            public bool IsStorefrontUser { get; set; }

        }

        public static void SendEmail(string email, string customer_name, HttpContext context, ref List<LogLines> lines)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("roy@datagroupit.com", "Roy"));
            msg.To.Add(new MailAddress("oren@yellowdotafrica.com", "Oren"));
            msg.To.Add(new MailAddress("adaramoye@datagroupit.com", "Adaramoye JUMOKE"));
            msg.To.Add(new MailAddress("igor@datagroupit.com", "Igor Santos"));
            //Uganda, Rwanda and Tanzania
            if (context.Request.Form["Country"] == "Kenya" || context.Request.Form["Country"] == "Uganda" || context.Request.Form["Country"] == "Rwanda" || context.Request.Form["Country"] == "Tanzania")
            {
                msg.To.Add(new MailAddress("frank@datagroupit.com", "Frank"));
            }
            
            msg.From = new MailAddress("cloud-noreply@datagroupit.com", "NoReplyCloud");
            string reseller_endcustomer = (context.Request.Form["client_type"] == "reseller" ? "Reseller" : "Customer");
            msg.Subject = "A New "+ reseller_endcustomer + " is interested in cloud products";

            string body = "";
            if (reseller_endcustomer == "Customer")
            {
                body = System.IO.File.ReadAllText(@"E:\apps\BSSInterworks\NewCustomerP1.html");
            }
            else
            {
                body = System.IO.File.ReadAllText(@"E:\apps\BSSInterworks\ResellerP1.html");
            }
             

            foreach (String key in context.Request.Form.AllKeys)
            {
                body = body + key + ": " + context.Request.Form[key] + "<br>";
            }

            body = body + System.IO.File.ReadAllText(@"E:\apps\BSSInterworks\NewCustomerP2.html");

            msg.Body = body;
            msg.IsBodyHtml = true;

            


            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("cloud-noreply@datagroupit.com", "CloudDGIT1");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.office365.com";

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                lines = Add2Log(lines, "E-Mail Sent Succesfully", 100, "reg_interworks");

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "E-Mail Failed" + ex.ToString(), 100, "reg_interworks");

            }
        }

        public static void SendEmail(string email, string customer_name, ref List<LogLines> lines)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(email, customer_name));
            msg.Bcc.Add(new MailAddress("support.cloud@datagroupit.com", "support.cloud@datagroupit.com"));
            msg.From = new MailAddress("cloud-noreply@datagroupit.com", "NoReplyCloud");
            msg.Subject = "Welcome to DataGroup-IT Cloud";
            string body = System.IO.File.ReadAllText(@"E:\apps\BSSInterworks\WelcomeEmailDataGroupIT.html");
            msg.Body = body;
            msg.IsBodyHtml = true;

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
            contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Octet;
            contentType.Name = "ResellerAgreement.pdf";
            msg.Attachments.Add(new Attachment("E:/apps/BSSInterworks/ResellerAgreement.pdf", contentType));


            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;

            client.Credentials = new System.Net.NetworkCredential("cloud-noreply@datagroupit.com", "CloudDGIT1");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.office365.com";
            
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                lines = Add2Log(lines, "E-Mail Sent Succesfully", 100, "reg_interworks");
                
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "E-Mail Failed" + ex.ToString(), 100, "reg_interworks");
                
            }
        }

        public static string GetToken(ref List<LogLines> lines)
        {
            string access_token = "";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("https://bss.eu.interworks.cloud/");
                    var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("9660d9d3-05d9-4ef1-8e34-7164368aecc4:EY7ipzAx4ipdfdefZWhOO7LLY3JqbaTIFX2Iq0InbhA="));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);

                    var form = new Dictionary<string, string>
                    {
                        {"grant_type", "password"},
                        {"username", "web_client"},
                        {"password", "rgX1j5zBpkt5MWXBzXnA"},
                    };

                    HttpResponseMessage response = httpClient.PostAsync("oauth/token", new FormUrlEncodedContent(form)).Result;
                    string response_body = response.Content.ReadAsStringAsync().Result;
                    lines = Add2Log(lines, "GetToken Response = " + response_body, 100, "reg_interworks");
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    access_token = json_response.access_token;
                    lines = Add2Log(lines, "access_token = " + access_token, 100, "reg_interworks");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "GetToken Exception = " + ex.ToString(), 100, "reg_interworks");
                }
            }
            return access_token;
        }

        public static string AddAccount(string access_token, BSSAccount bs_account, ref List<LogLines> lines)
        {
            string account_id = "";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("https://bss.eu.interworks.cloud/");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("X-Api-Version", "latest");

                    // Add the Authorization header with the AccessToken.
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);

                    

                    string postBody = JsonConvert.SerializeObject(bs_account);
                    lines = Add2Log(lines, "Add Account postBody = " + postBody, 100, "reg_interworks");

                    HttpResponseMessage response = httpClient.PostAsync("api/accounts/", new StringContent(postBody, Encoding.UTF8, "application/json")).Result;


                    string response_body = response.Content.ReadAsStringAsync().Result;
                    lines = Add2Log(lines, "Add Account response_body = " + response_body, 100, "reg_interworks");
                    dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    account_id = response_body;
                    lines = Add2Log(lines, "Add Account account_id = " + account_id, 100, "reg_interworks");
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, "Add Account Exception = " + ex.ToString(), 100, "reg_interworks");
                }
            }
            return account_id;


        }

        public static string AddContact(string access_token, BSContact bs_contact, ref List<LogLines> lines)
        {
            string result = "";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri("https://bss.eu.interworks.cloud/");
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("X-Api-Version", "latest");

                    // Add the Authorization header with the AccessToken.
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);


                    string postBody = JsonConvert.SerializeObject(bs_contact);
                    lines = Add2Log(lines, " adding contact postBody = " + postBody, 100, "reg_interworks");
                    HttpResponseMessage response = httpClient.PostAsync("api/contacts/", new StringContent(postBody, Encoding.UTF8, "application/json")).Result;


                    string response_body = response.Content.ReadAsStringAsync().Result;
                    result = response_body;
                    lines = Add2Log(lines, " adding contact response body = " + result, 100, "reg_interworks");
                    //dynamic json_response = JsonConvert.DeserializeObject(response_body);
                    //access_token = json_response.access_token;
                }
                catch (Exception ex)
                {
                    lines = Add2Log(lines, " error adding contact = " + ex.ToString(), 100, "reg_interworks");
                }
            }
            return result;
        }

           
        public void ProcessRequest(HttpContext context)
        {
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "reg_interworks");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "reg_interworks");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "reg_interworks");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "reg_interworks");

            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string post = System.Text.Encoding.UTF8.GetString(buffer);

            lines = Add2Log(lines, "Incomming post = " + post, 100, "reg_interworks");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "QS Key: " + key + " Value: " + context.Request.QueryString[key], 100, "reg_interworks");
            }

            foreach (String key in context.Request.Form.AllKeys)
            {
                lines = Add2Log(lines, "Form Key: " + key + " Value: " + context.Request.Form[key], 100, "reg_interworks");
            }

            string client_type = "", LastName = "", FirstName = "", Phone = "", MobilePhone = "", Email = "", Password = "", CompanyName = "", CompanyPhone = "", Fax = "", RegistrationNumber = "";
            string WebSite = "", Description = "", Address = "", City = "", Country = "", ZIP = "", Domain = "";
            string result = "NOK";
            client_type = context.Request.Form["client_type"];

            //account
            CompanyName = (!String.IsNullOrEmpty(context.Request.Form["Name"]) ? context.Request.Form["Name"] : "");
            CompanyPhone = (!String.IsNullOrEmpty(context.Request.Form["CompanyPhone"]) ? context.Request.Form["CompanyPhone"] : "");
            Fax = (!String.IsNullOrEmpty(context.Request.Form["Fax"]) ? context.Request.Form["Fax"] : "");
            RegistrationNumber = (!String.IsNullOrEmpty(context.Request.Form["RegistrationNumber"]) ? context.Request.Form["RegistrationNumber"] : "");
            WebSite = (!String.IsNullOrEmpty(context.Request.Form["WebSite"]) ? context.Request.Form["WebSite"] : "");
            Description = (!String.IsNullOrEmpty(context.Request.Form["Description"]) ? context.Request.Form["Description"] : "");
            Email = (!String.IsNullOrEmpty(context.Request.Form["Email"]) ? context.Request.Form["Email"] : "");

            Address = (!String.IsNullOrEmpty(context.Request.Form["Address"]) ? context.Request.Form["Address"] : "");
            City = (!String.IsNullOrEmpty(context.Request.Form["City"]) ? context.Request.Form["City"] : "");
            Country = (!String.IsNullOrEmpty(context.Request.Form["Country"]) ? context.Request.Form["Country"] : "");
            ZIP = (!String.IsNullOrEmpty(context.Request.Form["ZIP"]) ? context.Request.Form["ZIP"] : "");
            Domain = (!String.IsNullOrEmpty(context.Request.Form["Domain"]) ? context.Request.Form["Domain"] : "");


            //contact
            LastName = (!String.IsNullOrEmpty(context.Request.Form["LastName"]) ? context.Request.Form["LastName"] : "");
            FirstName = (!String.IsNullOrEmpty(context.Request.Form["FirstName"]) ? context.Request.Form["FirstName"] : "");
            Phone = (!String.IsNullOrEmpty(context.Request.Form["Phone"]) ? context.Request.Form["Phone"] : "");
            MobilePhone = (!String.IsNullOrEmpty(context.Request.Form["MobilePhone"]) ? context.Request.Form["MobilePhone"] : "");
            Password = (!String.IsNullOrEmpty(context.Request.Form["Password"]) ? context.Request.Form["Password"] : "");

            if (client_type == "reseller")
            {
                SendEmail(Email, FirstName + " " + LastName, context, ref lines);
                Address address = new Address()
                {
                    Address1 = Address,
                    City = City,
                    Country = Country,
                    PostCode = ZIP
                };

                BSSAccount bs_account = new BSSAccount()
                {
                    Name = CompanyName,
                    Phone = CompanyPhone,
                    Fax = Fax,
                    RegistrationNumber = RegistrationNumber,
                    WebSite = WebSite,
                    Description = Description,
                    Email = Email,
                    Address = address,
                    EnableOrdering = false,
                    EnableReselling = true,
                    PaymentMethod = "Net 15",
                    IsTaxable = true,
                    AutoInvoiceNotificationEnabled = true,
                    Currency = "NGN",
                    CurrencyCode = "566"
                };



                string token = GetToken(ref lines);
                if (!String.IsNullOrEmpty(token))
                {
                    string account_id = AddAccount(token, bs_account, ref lines);
                    int number;

                    bool success = Int32.TryParse(account_id, out number);
                    if (success)
                    {
                        BSContact bs_contact = new BSContact()
                        {
                            AccountId = number,
                            Email = Email,
                            LastName = LastName,
                            MobilePhone = MobilePhone,
                            Phone = Phone,
                            Password = Password,
                            FirstName = FirstName,
                            IsBillTo = true,
                            IsPrimary = true,
                            IsStorefrontUser = false
                        };

                        string contact_id = AddContact(token, bs_contact, ref lines);
                        success = Int32.TryParse(contact_id, out number);
                        if (success)
                        {
                            SendEmail(Email, FirstName + " " + LastName, ref lines);
                            result = "OK";
                        }
                        else
                        {
                            SendEmail(Email, FirstName + " " + LastName, ref lines);
                            result = "OK";
                        }
                    }
                }
            }
            else
            {
                SendEmail(Email, FirstName + " " + LastName, context, ref lines);
            }
            
 
            lines = Write2Log(lines);

            context.Response.Redirect("http://datagroupit.com/thank-you/");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}