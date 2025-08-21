using Api.Cache;
using Api.CommonFuncations;
using Api.HttpItems;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.LN
{
    public class TWN_Result
    {
        public string serviceName { get; set; }
        public string winningNumber { get; set; }
        public int numWinners { get; set; }
        public string totalPrize { get; set; }
    }

    public class TWN_drawDetails
    {
        public int serviceId { get; set; }
        public string serviceName { get; set; }
        public double subscriptionAmount { get; set; }
        public double jackpotAmount { get; set; }
        public DateTime subscriptionDate { get; set; }  // used to indicate the whether the user is subscribed
        public DateTime? deactivationDate { get; set; }  // if/when the subscription was deactivated    }
    }

    public class LNservice
    {

        public static Dictionary<string, List<TWN_Result>> fetch_winners(ref List<LogLines> lines, string serviceId, DateTime drawDate, int last_x_draws = 0)
        {
            // object to store results so we can convert to json later
            Dictionary<string, List<TWN_Result>> resultsByDate = new Dictionary<string, List<TWN_Result>>();

            string connectionStr = ServerSettings.GetServerSettings("DBConnectionString", ref lines);

            using (var connection = new MySqlConnection(connectionStr))
            {
                connection.Open();


                string sql = "SELECT w.winning_date, s.service_name, CAST(w.selected_msisdn AS CHAR) AS winning_number,"
                            + " SUM(IF(w.subscriber_id != 0, 1, 0)) AS num_winners,"
                            + " SUM(IF(w.subscriber_id != 0, p.prize, 0)) AS total_prize"
                            + " FROM yellowdot.twn_winners w"
                            + " INNER JOIN yellowdot.twn_prizes p ON w.prize_id = p.prize_id"
                            + " INNER JOIN yellowdot.services s ON p.service_id = s.service_id"
                            + " WHERE DATE(w.winning_date) >= @DrawDate - INTERVAL @last_x_draws DAY"
                            + " AND (@ServiceId = '' OR s.service_id in (@ServiceId))"
                            + " GROUP BY w.winning_date, s.service_name, winning_number"
                            + " ORDER BY w.winning_date DESC, s.service_name"
                            ;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@DrawDate", drawDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@ServiceId", serviceId);
                    command.Parameters.AddWithValue("@last_x_draws", last_x_draws);
                    command.CommandTimeout = 60;

                    lines = Add2Log(lines, $"fetching draw results for: {drawDate} or {last_x_draws} , serviceIDs: {serviceId}", 100, lines[0].ControlerName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string winningDate = Convert.ToDateTime(reader.GetValue(0)).ToString("yyyy-MM-dd");
                            string serviceName = Convert.ToString(reader.GetValue(1));
                            string winningNumber = Convert.ToString(reader.GetValue(2)).PadLeft(13, '0').Substring(7);   // Just the last 6 digits (13-7 = 6)
                            int numWinners = Convert.ToInt32(reader.GetValue(3));
                            double totalPrize = Convert.ToDouble(reader.GetValue(4));

                            TWN_Result result = new TWN_Result()
                            {
                                serviceName = serviceName,
                                winningNumber = "xx" + winningNumber,
                                numWinners = numWinners,
                                totalPrize = totalPrize.ToString("F2")
                            };

                            // add key if this does not exist
                            if (!resultsByDate.ContainsKey(winningDate)) resultsByDate[winningDate] = new List<TWN_Result>();

                            // add result to the results for this DAY
                            resultsByDate[winningDate].Add(result);
                        }
                    }
                }
            }
            return resultsByDate;

        }

        public static List<int> fetch_draw_serviceIds_for_operator(ref List<LogLines> lines, int operatorID)
        {

            List<int> serviceIDs = new List<int>();     // return serviceIds

            // fetch list from database
            string connectionStr = ServerSettings.GetServerSettings("DBConnectionString", ref lines);
            using (var connection = new MySqlConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT distinct s.service_id "
                            + " FROM services s "
                            + " INNER JOIN twn_prizes p on s.service_id = p.service_id"     // include all services for which there is a TWN prize defined
                            + " WHERE p.prize_number = 1 and s.operator_id = @operatorID"   // prize_number=1 is the max price for each service
                            ;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@operatorID", operatorID);
                    command.CommandTimeout = 60;

                    lines = Add2Log(lines, $"fetching serviceIDs for operatorID={operatorID}", 100, "");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            serviceIDs.Add(Convert.ToInt32(reader.GetValue(0)));
                    }
                }
            }

            if (serviceIDs.Count == 0) lines = Add2Log(lines, $"failed to find any TWN prizes specified for services linked to operatorID={operatorID}", 100, "");
            return serviceIDs;
        } // fetch_draw_serviceIds_for_operator

        // looks up the last 7 winning draws numbers and sends an sms to the user
        public static string sms_last_results(ref List<LogLines> lines, int operatorID, int last_x_draws = 7)
        {
            try
            {
                // determine the serviceIDs for this operator
                List<int> ServiceIDs = fetch_draw_serviceIds_for_operator(ref lines, operatorID);

                // fetch the last 7 winners
                Dictionary<string, List<TWN_Result>> results = fetch_winners(ref lines, String.Join(",", ServiceIDs), DateTime.Today, last_x_draws);
    
                // generate output
                List<string> output = new List<string>();
                switch (operatorID)
                {
                    case 32: // FRENCH
                        output.Add("Résultats des tirages précédents");
                        break;
                    default: // ENGLISH
                        output.Add("Previous draw results");
                        break;
                }

                foreach (var r in results)
                {
                    foreach (TWN_Result e in r.Value)
                    {
                        switch (operatorID)
                        {
                            case 32:        // Orange IC
                                output.Add(r.Key + ": 07" + e.winningNumber);
                                break;
                            default:        // the reset
                                output.Add(r.Key + ": " + e.winningNumber);
                                break;
                        }
                        
                    }
                }

                return String.Join("\n", output);

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! failed: {ex.Message}\n\n{ex.StackTrace}", 100, "");
            }

            return "failed to lookup details";
        }
    
        public static List<TWN_drawDetails> fetch_subscribed_drawDetails(ref List<LogLines> lines, int operatorID, string msisdn)
        {

            List<TWN_drawDetails> details = new List<TWN_drawDetails>();     // return value

            // fetch list from database
            string connectionStr = ServerSettings.GetServerSettings("DBConnectionString", ref lines);
            using (var connection = new MySqlConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT distinct s.service_id, s.service_name, s.subscription_amount, p.prize as jackpot"
                           + ",k.subscription_date,k.deactivation_date"
                           + " FROM services s"
                           + " INNER JOIN twn_prizes p on (s.service_id = p.service_id)" // include all services for which there is a TWN prize defined
                           + " INNER JOIN subscribers k on (k.msisdn = @msisdn AND k.service_id = s.service_id)"
                           + " WHERE p.prize_number = 1 and s.operator_id = @operatorID"
                           + " AND k.deactivation_date is null or date(k.deactivation_date) >= CURDATE - INTERVAL 10 DAY"
                           + " ORDER BY s.service_id, k.subscription_date DESC"
                           ;

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@msisdn", msisdn);
                    command.Parameters.AddWithValue("@operatorID", operatorID);
                    command.CommandTimeout = 60;

                    lines = Add2Log(lines, $"fetching Subscriptions for msisdn={msisdn}", 100, "");

                    using (var reader = command.ExecuteReader())
                    {
                            while (reader.Read())
                                details.Add(
                                    new TWN_drawDetails()
                                    {
                                        serviceId = Convert.ToInt32(reader.GetValue(0)),
                                        serviceName = Convert.ToString(reader.GetValue(1)),
                                        subscriptionAmount = Convert.ToDouble(reader.GetValue(2)),
                                        jackpotAmount = Convert.ToDouble(reader.GetValue(3)),
                                        subscriptionDate = Convert.ToDateTime(reader.GetValue(4)),
                                        deactivationDate = reader.IsDBNull(5) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(5))
                                    }
                                );
                    }
                }
            }

            if (details.Count == 0) lines = Add2Log(lines, $"user {msisdn} is not subscribed to any TWN services within operatorID {operatorID}", 100, "");
            return details;
        } // fetch_draw_serviceIds_for_operator



        // details of the draw that they are subscribed to
        public static string sms_subscription_status(ref List<LogLines> lines, int operatorID, string msisdn)
        {
            
            try
            {
                // fetch any subscribed services this user
                List<TWN_drawDetails> subscriptions = fetch_subscribed_drawDetails(ref lines, operatorID, msisdn);

                // generate output
                List<string> output = new List<string>();
                foreach (var s in subscriptions)        // could be more than one
                {
                    if (operatorID == 32)
                    {
                        // FRENCH
                        string msg = !s.deactivationDate.HasValue || s.deactivationDate < s.subscriptionDate
                                   ? $"actif pour le tirage du {s.subscriptionDate.ToShortDateString()}"
                                   : $"non actif pour le tirage du {s.deactivationDate.Value.ToShortDateString()}"
                                   ;

                        output.Add($"{s.serviceName} {s.subscriptionAmount} {msg} - jackpot {s.jackpotAmount}");
                    }
                    else
                    {
                        // ENGLISH
                        string msg = !s.deactivationDate.HasValue || s.deactivationDate < s.subscriptionDate
                                   ? $"subscribed on {s.subscriptionDate.ToShortDateString()}"
                                   : $"unsubscribed on {s.deactivationDate.Value.ToShortDateString()}"
                                   ;

                        output.Add($"{s.serviceName} {s.subscriptionAmount} {msg} - jackpot {s.jackpotAmount}");
                    }

                }

                return String.Join("\n", output); 
              
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! failed: {ex.Message}\n\n{ex.StackTrace}", 100, "");
            }

            return "failed to lookup details";
        }

        // Combined method that fetches subscription status and last draw results in a single SMS
        public static string sms_combined_status_and_results(ref List<LogLines> lines, int operatorID, string msisdn, int last_x_draws = 5)
        {
            try
            {
                lines = Add2Log(lines, $"Generating combined SMS for msisdn={msisdn}, operatorID={operatorID}", 100, "");
                
                // Fetch subscription details
                List<TWN_drawDetails> subscriptions = fetch_subscribed_drawDetails(ref lines, operatorID, msisdn);
                
                // Fetch service IDs for this operator
                List<int> ServiceIDs = fetch_draw_serviceIds_for_operator(ref lines, operatorID);
                
                // Fetch the last X draw results
                Dictionary<string, List<TWN_Result>> results = fetch_winners(ref lines, String.Join(",", ServiceIDs), DateTime.Today, last_x_draws);
                
                List<string> smsLines = new List<string>();
                
                // Add subscription status information
                if (subscriptions.Count > 0)
                {
                    foreach (var s in subscriptions)
                    {
                        bool isActive = !s.deactivationDate.HasValue || s.deactivationDate < s.subscriptionDate;
                        string status = isActive ? "actif" : "non actif";
                        string packAmount = "";
                        if (s.subscriptionAmount == 50) packAmount = "50F";
                        else if (s.subscriptionAmount == 100) packAmount = "100F";
                        else if (s.subscriptionAmount == 250) packAmount = "250F";
                        else packAmount = s.subscriptionAmount.ToString() + "F";
                        string renewable = isActive ? "Renouvelable" : "non renouvelable";
                        smsLines.Add($"Numéro d'Or: Vous êtes {status} sur le pack de {packAmount}. {renewable}.");
                        break; // Only show first subscription
                    }
                }
                else
                {
                    smsLines.Add("Numéro d'Or: Vous n'êtes actuellement abonné à aucun pack.");
                }
                
                // Add draw results section
                smsLines.Add("Les tirages précédents");
                
                if (results.Count > 0)
                {
                    int count = 0;
                    foreach (var r in results.OrderByDescending(x => x.Key))
                    {
                        if (count >= last_x_draws) break;
                        foreach (TWN_Result e in r.Value)
                        {
                            if (count >= last_x_draws) break;
                            string formattedDate = DateTime.Parse(r.Key).ToString("dd/MM");
                            string winningNumber = operatorID == 32 ? "07" + e.winningNumber : e.winningNumber;
                            smsLines.Add($"{formattedDate} : {winningNumber}");
                            count++;
                        }
                    }
                }
                else
                {
                    smsLines.Add("Aucun résultat de tirage disponible.");
                }
                
                string finalMessage = String.Join("\n", smsLines);
                lines = Add2Log(lines, $"Generated SMS message: {finalMessage}", 100, "");
                return finalMessage;
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, $"!! Combined SMS generation failed: {ex.Message}\n\n{ex.StackTrace}", 100, "");
                return "Erreur lors de la génération du message.";
            }
        }

    
           
    }
}