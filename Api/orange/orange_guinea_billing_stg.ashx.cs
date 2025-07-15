using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Cache.USSD;
using static Api.CommonFuncations.iDoBet;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.orange
{
    /// <summary>
    /// Summary description for orange_guinea_billing_stg
    /// </summary>
    public class orange_guinea_billing_stg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Charset = "utf-8";
            var stream = context.Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "orange_guinea_billing_stg");
            lines = Add2Log(lines, "Incomming XML = " + xml, 100, "orange_guinea_billing_stg");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "orange_guinea_billing_stg");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "orange_guinea_billing_stg");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "orange_guinea_billing_stg");
            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "orange_guinea_billing_stg");
            }

            //2020 - 12 - 23 14:29:43.277: Key: referenceNumbere Value: PB_3bbe1f1d - bf60 - 4136 - 8e1c - 4d11eeebd2b2
            //2020 - 12 - 23 14:29:43.277: Key: amount Value: 2000
            //2020 - 12 - 23 14:29:43.277: Key: status Value: sucess


            string referenceNumbere = context.Request.QueryString["referenceNumbere"];
            string amount = context.Request.QueryString["amount"];
            string status = context.Request.QueryString["status"];
            string billing_id = context.Request.QueryString["omtransactionId"];
            //2020-12-24 17:03:45.229: Key: omtransactionId Value: MP201224.1504.A00011


            DYATransactions dya_trans = new DYATransactions();
            string menu_2_display = "", menu = "";
            Int64 number;
            bool result = Int64.TryParse(referenceNumbere, out number);
            if (!String.IsNullOrEmpty(status))
            {
                if (status.Contains("sucess") && !String.IsNullOrEmpty(referenceNumbere))
                {

                    if (result)
                    {
                        OrangeBillingRequest orange_billing = UpdateGetOrangeBillingRequest(number, 1, billing_id, ref lines);
                        if (orange_billing != null)
                        {
                            menu_2_display = menu_2_display + "La transaction a été approuvée." + Environment.NewLine;
                            menu_2_display = menu_2_display + "Un SMS sera envoyé sous peu.";
                        }
                        else
                        {
                            menu_2_display = menu_2_display + "Il y avait une erreur." + Environment.NewLine;
                            menu_2_display = menu_2_display + "Veuillez réessayer";
                        }
                    }
                    else
                    {
                        menu_2_display = menu_2_display + "Il y avait une erreur." + Environment.NewLine;
                        menu_2_display = menu_2_display + "Veuillez réessayer";
                    }
                }
                else
                {
                    if (result)
                    {
                        OrangeBillingRequest orange_billing = UpdateGetOrangeBillingRequest(number, 2, billing_id, ref lines);
                    }
                    menu_2_display = menu_2_display + "Il y avait une erreur." + Environment.NewLine;
                    menu_2_display = menu_2_display + "Veuillez réessayer";
                }

                USSDSession ussd_session = null;
                USSDMenu ussd_menu = new USSDMenu();
                ussd_menu.is_special = false;
                menu = Api.handlers.ussd_orange_guinea_stg.BuildOrangeMenu("", menu_2_display, true, 1, ussd_session, ussd_menu, ref lines);
                lines = Add2Log(lines, "menu = " + menu, 100, "orange_guinea_billing");
            }
            else
            {
                menu_2_display = menu_2_display + "Il y avait une erreur." + Environment.NewLine;
                menu_2_display = menu_2_display + "Veuillez réessayer";
                USSDSession ussd_session = null;
                USSDMenu ussd_menu = new USSDMenu();
                ussd_menu.is_special = false;
                menu = Api.handlers.ussd_orange_guinea_stg.BuildOrangeMenu("", menu_2_display, true, 1, ussd_session, ussd_menu, ref lines);
                lines = Add2Log(lines, "menu = " + menu, 100, "orange_guinea_billing");
            }
            
            lines = Write2Log(lines);
            context.Response.ContentType = "application/xhtml+xml; utf-8";
            context.Response.Write(menu);
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