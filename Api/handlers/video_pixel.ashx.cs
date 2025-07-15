using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using static Api.Logger.Logger;

namespace Api.handlers
{
    /// <summary>
    /// Summary description for video_pixel
    /// </summary>
    public class video_pixel : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string sub_id = (!String.IsNullOrEmpty(HttpContext.Current.Request.QueryString["sub_id"]) ? HttpContext.Current.Request.QueryString["sub_id"] : "");
            int log_level = Convert.ToInt32(ConfigurationManager.AppSettings["log_level"]);
            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, "*****************************", 100, "video_pixel");
            lines = Add2Log(lines, "IP = " + context.Request.ServerVariables["REMOTE_ADDR"], 100, "video_pixel");
            lines = Add2Log(lines, "UA = " + context.Request.ServerVariables["HTTP_USER_AGENT"], 100, "video_pixel");
            lines = Add2Log(lines, "REFERER = " + context.Request.ServerVariables["HTTP_REFERER"], 100, "video_pixel");
            lines = Add2Log(lines, "sub_id = " + sub_id, 100, "video_pixel");

            foreach (String key in context.Request.QueryString.AllKeys)
            {
                lines = Add2Log(lines, "Key: " + key + " Value: " + context.Request.QueryString[key], 100, "video_pixel");
            }

            lines = Write2Log(lines);

            System.Drawing.Image BCImage = new System.Drawing.Bitmap(@"C:\images\1.png");
            MemoryStream ms = new MemoryStream();
            BCImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            context.Response.ContentType = "image/png";
            context.Response.Clear();
            context.Response.BinaryWrite(ms.ToArray());

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