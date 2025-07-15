using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for app_callback
    /// </summary>
    public class app_callback : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "app_callback");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "app_callback");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "app_callback");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "app_callback");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "app_callback");

            string user_id = "0", app_id = "", player_id = "", push_data = "";

            if(!String.IsNullOrEmpty(xml))
            {
                string[] str_arr = xml.Split('&');
                if (str_arr != null)
                {
                    foreach(string s in str_arr)
                    {
                        if (s.Contains("="))
                        {
                            string[] items = s.Split('=');
                            switch (items[0])
                            {
                                case "webUserID":
                                    user_id = items[1];
                                    break;
                                case "AppID":
                                    app_id = items[1];
                                    break;
                                case "pushData":
                                    push_data = items[1];
                                    break;
                                case "playerID":
                                    player_id = items[1];
                                    break;
                            }
                        }
                    }
                    user_id = (String.IsNullOrEmpty(user_id) ? "0" : user_id);

                    if (!String.IsNullOrEmpty(player_id) && user_id != "0")
                    {
                        string service_ids = "0";
                        switch(app_id)
                        {
                            case "Cameroon":
                                service_ids = "720";
                                break;
                            case "LNB":
                                service_ids = "726,730";
                                break;
                            case "Guinea":
                                service_ids = "716,732";
                                break;
                            case "Congo":
                                service_ids = "733";
                                break;
                        }
                        string service_id = Api.DataLayer.DBQueries.SelectQueryReturnString("SELECT i.service_id FROM idobet_users i WHERE i.service_id in ("+ service_ids + ") and i.user_id = " + user_id, "DBConnectionString_32", ref lines);
                        service_id = (String.IsNullOrEmpty(service_id) ? "0" : service_id);
                        Api.DataLayer.DBQueries.ExecuteQuery("insert into application_users (player_id, idobet_userid, last_active, update_time, app_id, service_id) values('" + player_id + "'," + user_id + ",now(), now(), '" + app_id + "',"+service_id+") ON DUPLICATE KEY UPDATE idobet_userid = " + user_id + ", last_active = now(), update_time = now(), app_id = '" + app_id + "', service_id = " + service_id, "DBConnectionString_104", ref lines);
                    }
                    if (!String.IsNullOrEmpty(push_data))
                    {
                        push_data = HttpUtility.UrlDecode(push_data);
                        try
                        {
                            dynamic json_response = JsonConvert.DeserializeObject(push_data);
                            string notificationID = json_response.notificationID;
                            if (!String.IsNullOrEmpty(notificationID))
                            {
                                lines = Add2Log(lines, " notificationID = " + notificationID, 100, "app_callback");
                                if (!String.IsNullOrEmpty(notificationID))
                                {
                                    Api.DataLayer.DBQueries.ExecuteQuery("update application_camp_notifications set clicked_date = now() where notification_id = '"+notificationID+"'", "DBConnectionString_104", ref lines);
                                }
                                
                            }
                        }
                        catch(Exception ex)
                        {
                            lines = Add2Log(lines, " exception = " + ex.ToString(), 100, "app_callback");
                        }
                        

                    }
                }
            }

            lines = Write2Log(lines);


            context.Response.ContentType = "application/json";
            context.Response.Write("OK");
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