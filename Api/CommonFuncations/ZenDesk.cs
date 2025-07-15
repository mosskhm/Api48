using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.CommonFuncations.iDoBet;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ZenDesk
    {

        public class ZendeskEndUsers
        {
            public Int64 id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string external_id { get; set; }

            public string next_page { get; set; }
        }

        public static List<ZendeskEndUsers> GetAllEndUsers(List<ZendeskEndUsers> zendesk_users, int page, ref List<LogLines> lines)
        {
            
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users.json?role[]=end-user&page="+ page;
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);
            string response_body = CallSoap.GetURL(url, ref lines, username, password);

            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    foreach (var item in json_response.users)
                    {
                        if (item.role == "end-user")
                        {
                            ZendeskEndUsers zendesk_user = new ZendeskEndUsers()
                            {
                                id = item.id,
                                name = item.name,
                                email = item.email,
                                phone = item.phone,
                                external_id = item.external_id
                            };
                            zendesk_user.next_page = json_response.next_page;
                            zendesk_users.Add(zendesk_user);
                        }
                    }
                    
                }
                catch (Exception ex)
                {

                }
            }
            return zendesk_users;
        }

        public static user GetZendeskUserDetails(string zendesk_id, ref List<LogLines> lines)
        {
            user zendesk_user = null; 
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users/" + zendesk_id + ".json";
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);
            string response_body = CallSoap.GetURL(url, ref lines, username, password);
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    zendesk_user = new user()
                    {
                        phone = json_response.user.phone,
                        external_id = json_response.user.external_id,
                        name = json_response.user.name
                    };
                    
                }
                catch (Exception ex)
                {

                }
            }
            return zendesk_user;
        }

        public static string GetMSISDNByZendeskID(string zendesk_id, ref List<LogLines> lines)
        {
            string msisdn = "";
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users/" + zendesk_id + ".json";
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);
            string response_body = CallSoap.GetURL(url, ref lines, username, password);
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    msisdn = json_response.user.phone;
                }
                catch (Exception ex)
                {

                }
            }
            return msisdn;
        }

        public static string GetNameByZendeskID(string zendesk_id, ref List<LogLines> lines)
        {
            string name = "";
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users/" + zendesk_id + ".json";
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);
            string response_body = CallSoap.GetURL(url, ref lines, username, password);
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    name = json_response.user.name;
                }
                catch (Exception ex)
                {

                }
            }
            return name;
        }

        public static string SearchUser(string msisdn, ref List<LogLines> lines)
        {
            string id = "";
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users/search.json?query=" + msisdn;
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);
            string response_body = CallSoap.GetURL(url, ref lines, username, password);

            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    id = json_response.users[0].id;
                }
                catch (Exception ex)
                {

                }
            }
            return id;
        }

        public class user_fields
        {
            public double actual_balance { get; set; }
            public double allowed_withdraw { get; set; }
            public bool approved { get; set; }
            public string country { get; set; }
            public bool locked { get; set; }
            public string mobile_operator { get; set; }
            public string site { get; set; }
        }
        public class user
        {
            public string name { get; set; }
            public string phone { get; set; }
            public bool shared_phone_number { get; set; }
            public string external_id { get; set; }
            public string time_zone { get; set; }
            public user_fields user_fields { get; set; }
            public string locale { get; set; }
            public string email { get; set; }

        }

        public class zendeskuser
        {
            public user user { get; set; }
        }
        public static string CreateUser(string msisdn, IdoBetUser user, string mobile_operator, string site, string country, string lang, ref List<LogLines> lines)
        {
            string id = "";
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users.json";
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);

            user_fields user_fields = new user_fields()
            {
                country = country,
                actual_balance = user.availableCredit,
                allowed_withdraw = user.AvailableForWithdraw,
                approved = true,
                locked = user.isLocked,
                mobile_operator = mobile_operator,
                site = site
            };

            
            user zendesk_user = new user()
            {
                name = user.firstName + " " + user.lastName,
                external_id = user.id.ToString(),
                phone = "+" + msisdn,
                shared_phone_number = true,
                user_fields = user_fields,
                time_zone = "West Central Africa",
                locale = lang
            };
            if (!String.IsNullOrEmpty(user.Email))
            {
                zendesk_user.email = user.Email;
            }
            else
            {
                zendesk_user.email = user.id.ToString() + "@unverified.com";
            }

            zendeskuser final_user = new zendeskuser()
            {
                user = zendesk_user
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            string postBody = JsonConvert.SerializeObject(final_user, settings);

            List<Headers> headers = new List<Headers>();
            string response_body = CallSoap.CallSoapRequest(url, postBody, headers, 2, username, password, ref lines);
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    id = json_response.user.id;
                }
                catch (Exception ex)
                {

                }
            }
            return id;
        }

        public static string UpdateUser(string zendesk_id, IdoBetUser user, string mobile_operator, string site, string country, string lang, ref List<LogLines> lines)
        {
            string id = "";
            string url = Cache.ServerSettings.GetServerSettings("ZenDeskBaseURL", ref lines) + "/api/v2/users/"+zendesk_id+".json";
            string username = Cache.ServerSettings.GetServerSettings("ZenDeskEmail", ref lines);
            string password = Cache.ServerSettings.GetServerSettings("ZenDeskToken", ref lines);

            user_fields user_fields = new user_fields()
            {
                actual_balance = user.availableCredit,
                allowed_withdraw = user.AvailableForWithdraw,
                approved = true,
                locked = user.isLocked,
                country = country,
                mobile_operator = mobile_operator,
                site = site
            };


            user zendesk_user = new user()
            {
                name = user.firstName + " " + user.lastName,
                external_id = user.id.ToString(),
                user_fields = user_fields,
                time_zone = "West Central Africa",
                phone = "+" + user.msisdn,
            };
            if (String.IsNullOrEmpty(user.Email))
            {
                zendesk_user.email = user.id.ToString() + "@unverified.com";
            }
            
            if (!String.IsNullOrEmpty(user.msisdn))
            {
                zendesk_user.phone = user.msisdn;
            }

            zendeskuser final_user = new zendeskuser()
            {
                user = zendesk_user
            };
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            string postBody = JsonConvert.SerializeObject(final_user, settings);

            List<Headers> headers = new List<Headers>();
            string response_body = CallSoap.CallSoapRequest(url, postBody, headers, 2, username, password, "PUT", ref lines);
            if (!String.IsNullOrEmpty(response_body))
            {
                dynamic json_response = JsonConvert.DeserializeObject(response_body);
                try
                {
                    id = json_response.user.id;
                }
                catch (Exception ex)
                {

                }
            }
            return id;
        }
    }
}