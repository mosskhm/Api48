using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;


namespace Api.Logger
{
    public static class LogzIOHelper
    {
		private static readonly HttpClient httpClient = new HttpClient();

		public static void SendLogsAsync(IEnumerable<object> logMessages, string type)
		{
			try
			{
				string logzioBulkUrl = "https://listener-eu.logz.io:8071/?token=klNfvvrYQGeWYqklfUhwKolEhxAYnNUR&type=" + type;
				foreach (object o in logMessages)
				{
					string jsonLog = JsonConvert.SerializeObject(o);
					var content = new StringContent(jsonLog, Encoding.UTF8, "application/json");
					var response = httpClient.PostAsync(logzioBulkUrl, content);
				}

			}
			catch (Exception ex)
			{
				//Console.WriteLine($"Exception caught while sending logs to Logz.io: {ex.Message}");
			}
		}
	}
}