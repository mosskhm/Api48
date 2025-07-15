using Api.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static Api.Cache.Services;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Curl
    {
        public static string CallECWCurl(string url, ECWServiceInfo service_info, string data, ref List<LogLines> lines)
        {
            string result = "";
            try
            {
                lines = Add2Log(lines, "url = " + url, 100, "DYACheckAccount");
                lines = Add2Log(lines, "data = " + data, 100, "DYACheckAccount");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = ServerSettings.GetServerSettings("CURLCommand", ref lines);

                string cmd = url + " -H \"Content-Type:text/xml\" --cacert " + service_info.cacert_path + " --cert " + service_info.cert_path + " --key " + service_info.key_path + " --data \"" + data.Replace("\"", "'") + "\" -u " + service_info.username + ":" + service_info.password;
                lines = Add2Log(lines, "cmd = " + cmd, 100, "DYACheckAccount");
                startInfo.Arguments = url + " -H \"Content-Type:text/xml\" --cacert " + service_info.cacert_path + " --cert " + service_info.cert_path + " --key " + service_info.key_path + " --data \"" + data.Replace("\"", "'") + "\" -u " + service_info.username + ":" + service_info.password;


                process.StartInfo = startInfo;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                StreamReader reader = process.StandardOutput;
                result = reader.ReadToEnd();
                lines = Add2Log(lines, "result = " + result, 100, "DYACheckAccount");
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "exception = " + ex.ToString(), 100, "DYACheckAccount");
            }

            return result;
        }
    }
}