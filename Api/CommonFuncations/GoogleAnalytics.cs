using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class GoogleAnalytics
    {
        public static void SendData2GoogleAnalytics(string analytics_account, string ds, string msisdn, string ip, string geo_id, string type, string category, string action, string value, string dp, ref List<LogLines> lines)
        {
            //try
            //{
            //    ASCIIEncoding encoding = new ASCIIEncoding();
            //    string postData = "v=1&tid=" + analytics_account + "&ds="+ ds + "&uid=" + msisdn + "&uip=" + ip + "&geoid=" + geo_id + "&t=" + type + "&dp=" + dp;
            //    postData = (type == "event" ? postData + "&ec=" + category + "&ea=" + action + "&ev=" + value : postData);
            //    //+ "&ec=" + category + "&ea=" + action + "&el=" + label + "&ev=" + value;
            //    lines = Add2Log(lines, "Sending Data to Analytics", 100, "SendAnalytics");
            //    lines = Add2Log(lines, postData, 100, "SendAnalytics");
            //    byte[] data = encoding.GetBytes(postData);
            //    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://www.google-analytics.com/collect");
            //    myRequest.Method = "POST";
            //    myRequest.ContentType = "application/x-www-form-urlencoded";
            //    myRequest.ContentLength = data.Length;
            //    Stream newStream = myRequest.GetRequestStream();
            //    newStream.Write(data, 0, data.Length);
            //    newStream.Close();
            //    WebResponse response = myRequest.GetResponse();
            //    Stream receiveStream = response.GetResponseStream();
            //    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            //    string MyResponse = readStream.ReadToEnd().ToString();
            //    lines = Add2Log(lines, " Response " + MyResponse, 100, "SendAnalytics");
            //}
            //catch (WebException ex)
            //{
            //    try
            //    {
            //        var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
            //        lines = Add2Log(lines, "Body = " + resp, 100, lines[0].ControlerName);
            //    }
            //    catch (Exception ex1)
            //    {

            //    }
            //    lines = Add2Log(lines, "InnerException = " + ex.InnerException, 100, lines[0].ControlerName);
            //    lines = Add2Log(lines, "Message = " + ex.Message, 100, lines[0].ControlerName);
            //}
        }
    }
}