using Api.Cache;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Prices;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.zw
{
    /// <summary>
    /// Summary description for notification
    /// </summary>
    public class notification : IHttpHandler
    {

        private PriceClass LookupOrInsertPriceEntry(ref List<LogLines> lines, int service_id, double amount_paid, string currency)
        {
            PriceClass price_c = null;

            // lookup the price
            if (amount_paid <= 0.00) lines = Add2Log(lines, $"lookup price - invalid amount_paid = {amount_paid}", 100, "");
            else 
            {
                // check if the price exists
                price_c = GetPricesInfo(service_id, amount_paid, ref lines);

                // if not, insert a new price entry for this service
                if (price_c == null)
                {
                    Int64 price_id = Api.DataLayer.DBQueries.ExecuteQueryReturnInt64(
                                            "insert into prices (service_id, price, curency_code, curency_symbol, real_price) " +
                                            "values (" + service_id + "," + amount_paid + ",'" + currency + "','" + currency + "'," + amount_paid.ToString("F2") + ")"
                                            , ref lines);
                    if (price_id > 0)
                    {
                        // update the cache
                        List<PriceClass> result_list = Api.DataLayer.DBQueries.GetPrices(ref lines);
                        if (result_list != null)
                        {
                            HttpContext.Current.Application["PriceList"] = result_list;
                            HttpContext.Current.Application["PriceList_expdate"] = DateTime.Now.AddHours(10);
                        }

                        // fetch our current entry
                        price_c = GetPricesInfo(service_id, price_id, ref lines);
                    }
                    else throw new Exception($"failed to insert new price entry for serviceID={service_id}, amount={amount_paid}");
                }
            }

            return price_c;
        }

        private int DetermineNumberOfDraws(ref List<LogLines> lines, string service_name)
        {
            // Jan 2025:
            //  if service_name contains Week, then add 7 
            //  if service_name contains Monthly, then add 30

            int num_draws = 1;          // default to ONE draw
            
            service_name = service_name.ToLower();
            if (service_name.Contains("week")) num_draws = 7;
            else if (service_name.Contains("month")) num_draws = 30;

            lines = Add2Log(lines, $"{service_name} = {num_draws} draws", 100, "");
            return num_draws;
        }

        private bool InsertBillingEntry(ref List<LogLines> lines, string amount_billed, Int32 service_id, string currency, string action, string msisdn, ref string description)
        {
            bool flag = false;      // return flag

            try
            {

                // determine which service we are looking at
                ServiceClass service = GetServiceByServiceID(service_id, ref lines) ?? throw new Exception($"unable to lookup serviceID={service_id}");

                int num_draws = DetermineNumberOfDraws(ref lines, service.service_name);
                double amount_paid = Math.Round(Convert.ToDouble(amount_billed) / num_draws, 3);

                PriceClass price_c = LookupOrInsertPriceEntry(ref lines, service_id, amount_paid, currency) ?? throw new Exception($"unable to determine a price entry for serviceID={service_id}, amount_paid={amount_paid}");

                // loop through the number of draws
                for (int i = 0; i < num_draws; i++)
                {
                    // determine the billing date
                    string mydate_time = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd HH:mm:ss");
                    flag = true;        // at this point we are all good

                    Int64 subscriber_id = 0;        // subscribe or unsubscribe the user
                    switch (action)
                    {
                        case "subscribe":
                        case "bill":
                            subscriber_id = Api.DataLayer.DBQueries.InsertSub(msisdn, service_id.ToString(), price_c.price_id, mydate_time, mydate_time, "2", "", ref lines);
                            lines = Add2Log(lines, $"[{action}] {msisdn} to service {service.service_name} sub_id = {subscriber_id}", 100, "");
                            break;

                        case "deactivate":
                            subscriber_id = Api.DataLayer.DBQueries.UnsubscribeSub(msisdn, service_id.ToString(), mydate_time, "2", "", mydate_time, ref lines);
                            lines = Add2Log(lines, $"[unsubscribing] {msisdn} from service {service.service_name} sub_id = {subscriber_id}", 100, "");
                            break;

                        default:
                            throw new Exception($"unknown action={action}");
                    }
                    if (subscriber_id <= 0)
                    {
                        lines = Add2Log(lines, $"failed to process {action} for {msisdn} on {service.service_name} for date = {mydate_time}", 100, "");
                        description = $"Something went wrong processing {action} for {msisdn}";
                        flag = false;
                    }
                    else if (flag)
                    {
                        lines = Add2Log(lines, $"{msisdn} {action} to {service_id} for {mydate_time}: subscriberID = {subscriber_id}", 100, "");
                        description += $" [{action}: {mydate_time}]";
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"failed to insert billing entry for msisdn={msisdn} - reason: {ex.Message}", 100, "");
                description = $"failed to record [{action}] entry for msisdn={msisdn}, amount={amount_billed} {currency}";
                flag = false;
            }

            return flag;
        }

        public void ProcessRequest(HttpContext context)
        {
            List<LogLines> lines = new List<LogLines>();
            string result = "NOK";          // default to failed
            string description = "";        // return message
 
            try
            {
                lines = Add2Log(lines, "*****************************", 100, "ln_zw");
                lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "ln_zw");
                lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "ln_zw");
                lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "ln_zw");

                foreach (String key in context.Request.QueryString.AllKeys)
                {
                    lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "ln_zw");
                }

                // read input stream
                var stream = context.Request.InputStream;
                if (stream.Length > 5000) throw new Exception($"!! input too long !! len = {stream.Length}");       // unrealistic input -- possibly a buffer overflow attack

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                string json = System.Text.Encoding.UTF8.GetString(buffer);
                if (String.IsNullOrEmpty(json)) throw new Exception($"!! no input received !!");

                lines = Add2Log(lines, "Incomming JSON = " + json, 100, "ln_zw");

                dynamic json_response = JsonConvert.DeserializeObject(json);
                string service_id = Convert.ToString(json_response.service_id).Trim();
                string amount_billed = Convert.ToString(json_response.amount_billed).Trim();
                string action = Convert.ToString(json_response.action).Trim();
                string msisdn = Convert.ToString(json_response.msisdn).Trim();
                string currency = Convert.ToString(json_response.currency);

                // default currency to ZWD if not specified
                if (String.IsNullOrEmpty(currency)) currency = "ZWD";
                else currency = currency.Trim().ToUpper();

                if (String.IsNullOrEmpty(service_id) || String.IsNullOrEmpty(action) || String.IsNullOrEmpty(msisdn))
                {
                    lines = Add2Log(lines, $"Missing required inputs: service_id={service_id}, action={action}, msisdn={msisdn}", 100, "");
                    description = "Missing parameters";
                }
                else
                {
                    // add the country code if it is missing
                    if (msisdn.Length < 10 || !msisdn.StartsWith("263")) msisdn = "263" + msisdn;

                    // set result = OK if successful
                    if (InsertBillingEntry(ref lines, amount_billed, Convert.ToInt32(service_id), currency, action, msisdn, ref description)) result = "OK";

                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"failed to process input - reason: {ex.Message}", 100, "");
                description = $"invalid input message received";
            }

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"result\": \""+result+"\", \"description\": \""+description+"\"}");
            lines = Write2Log(lines);
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