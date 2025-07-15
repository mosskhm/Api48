using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Logger.Logger;

namespace Api.Cache
{
    public class ServerSettings
    {

        public static string GetServerSettings(string NodeName, ref List<LogLines> lines)
        {
            string result = "";
            try
            {
                if (HttpContext.Current.Application[NodeName] != null)
                {
                    result = (string)HttpContext.Current.Application[NodeName];
                }
                else
                {
                    XmlDocument xmldoc = new XmlDocument();
                    XmlNodeList xmlnode;
                    FileStream fs = new FileStream("C:\\ServerSettings\\APISettings.xml", FileMode.Open, FileAccess.Read);
                    xmldoc.Load(fs);
                    try
                    {
                        xmlnode = xmldoc.GetElementsByTagName(NodeName);
                        if (xmlnode.Count > 0)
                        {
                            result = xmlnode[0].InnerText;
                            HttpContext.Current.Application[NodeName] = result;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        lines = Add2Log(lines, " Error on GetServerSettings " + ex.InnerException, 100, lines[0].ControlerName);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                XmlDocument xmldoc = new XmlDocument();
                XmlNodeList xmlnode;
                FileStream fs = new FileStream("C:\\ServerSettings\\APISettings.xml", FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                try
                {
                    xmlnode = xmldoc.GetElementsByTagName(NodeName);
                    if (xmlnode.Count > 0)
                    {
                        result = xmlnode[0].InnerText;
                    }
                }
                catch (Exception ex1)
                {
                    lines = Add2Log(lines, " Error on GetServerSettings " + ex.InnerException, 100, lines[0].ControlerName);
                }
                fs.Close();
            }
            
            return result;
        }
    }
}