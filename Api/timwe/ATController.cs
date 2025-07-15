using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Api.Logger.Logger;

namespace Api.timwe
{
    public class ATController : Controller
    {
        // GET: V1
        public ActionResult Subscribe()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_Subscribe");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_Subscribe");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }

        public ActionResult UnSubscribe()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_UnSubscribe");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_UnSubscribe");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }

        public ActionResult Charge()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_Charge");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_Charge");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }

        public ActionResult Mo()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_Mo");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_Mo");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }

        public ActionResult MT()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_MT");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_MT");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }

        public ActionResult Renew()
        {
            Response.Charset = "utf-8";
            var stream = Request.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string xml = System.Text.Encoding.UTF8.GetString(buffer);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "Timwe_Renew");
            lines = Add2Log(lines, "Incomming Json/ XML = " + xml, 100, "Timwe_Renew");
            lines = Add2Log(lines, "IP = " + Request.ServerVariables["REMOTE_ADDR"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "UA = " + Request.ServerVariables["HTTP_USER_AGENT"], 100, "Timwe_sms_dlr");
            lines = Add2Log(lines, "REFERER = " + Request.ServerVariables["HTTP_REFERER"], 100, "Timwe_sms_dlr");
            foreach (String key in Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + Request.QueryString[key], 100, "Timwe_sms_dlr");
            }

            Response.ContentType = "application/json";
            string result = "{\"Response\":\"OK\"}";
            lines = Write2Log(lines);
            return Content(result);
        }
    }
}