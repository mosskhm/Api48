using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            LogExceptionToEventLog(exception);
            Server.ClearError();
            Response.Redirect("~/ErrorPage.html"); // Customize the path to your error page
            
        }

        private void WriteLogToFile(Exception exception)
        {
            try
            {
                string appRoot = HttpRuntime.AppDomainAppPath;
                string logDirectory = Path.Combine(appRoot, "logs");        // create a logs directory

                // Check if the log directory exists, and create it if it doesn't
                if (!Directory.Exists(logDirectory)) Directory.CreateDirectory(logDirectory);

                // Generate a file name with today's date (e.g., exceptions_2024-11-29.log)
                string logFileName = $"exceptions_{DateTime.Now:yyyy-MM-dd}.log";
                string logFilePath = Path.Combine(logDirectory, logFileName);

                // Prepare the log message to write to the file
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Exception: {exception.ToString()}";

                // Write the log message to the log file (appending if the file already exists)
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // throw;
            }
        } // WriteLogToFile

        private void LogExceptionToEventLog(Exception exception)
        {
            try
            { 
                string source = "Application";

                // Write the event to the event log
                EventLog.WriteEntry(source, exception.ToString(), EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                // the only exception we will get is that we can't write to the eventlog -- so write the original message out to the logfile instead
                WriteLogToFile(exception);
            }
        }

    }
}
