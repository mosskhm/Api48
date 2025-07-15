using Api.Cache;
using Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Configuration;


namespace Api.Logger
{
    public class Logger
    {
        public class LogLines
        {
            public string CurrentTime { set; get; }
            public string Data { set; get; }
            public int LogLevel { get; set; }
            public string ControlerName { get; set; }

        }
        
        public static void Write2File (List<LogLines> lines)
        {
            try
            {
                string file_name = ServerSettings.GetServerSettings("LogsPath", ref lines) + lines[0].ControlerName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(file_name, true))
                {
                    foreach (LogLines line in lines)
                    {
                        file.WriteLine(line.CurrentTime + ": " + line.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    string file_name = ServerSettings.GetServerSettings("LogsPath", ref lines) + "Exception_" + lines[0].ControlerName + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(file_name, true))
                    {
                        file.WriteLine("Exception: " + ex.InnerException);
                        file.WriteLine("Exception: " + ex.Message);
                        foreach (LogLines line in lines)
                        {
                            file.WriteLine(line.CurrentTime + ": " + line.Data);
                        }

                    }
                }
                catch (Exception ex1)
                {
 
                }

            }
        }

        public static List<LogLines> Write2Log(List<LogLines> lines)
        {
            switch (ConfigurationManager.AppSettings["log_type"])
            {
                case "1":
                    Write2File(lines);
                    break;
                case "2":
                    DBQueries.InsertLog(lines);
                    break;
                case "3":
                    Write2File(lines);
                    DBQueries.InsertLog(lines);
                    break;
            }
            lines = new List<LogLines>();
            return lines;

        }

        public static List<LogLines> Add2Log(List<LogLines> lines, string text, int log_level ,string controler_name)
        {
            lines.Add(new LogLines() { CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), Data = text, LogLevel = log_level, ControlerName = controler_name });
            return lines;
        }
    }
}