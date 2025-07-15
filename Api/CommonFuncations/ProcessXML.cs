using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class ProcessXML
    {
        public static string GetXMLNode(string xml, string node_name, ref List<LogLines> lines)
        {
            string result = "";
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var root = xmlDocument.DocumentElement;
                var tag = root.GetElementsByTagName(node_name);
                if (tag.Count > 0)
                {
                    result = tag[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Error on GetXMLNode " + ex.InnerException, 100, lines[0].ControlerName);
            }
            return result;
        }

        public static string GetXMLNode(string xml, string node_name, string attribute, ref List<LogLines> lines)
        {
            string result = "";
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var root = xmlDocument.DocumentElement;
                var tag = root.GetElementsByTagName(node_name);
                if (tag.Count > 0)
                {
                    for(int i=0;i< tag[0].Attributes.Count; i++)
                    {
                        if (tag[0].Attributes[i].Name == attribute)
                        {
                            result = tag[0].Attributes[i].Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "Error on GetXMLNode " + ex.InnerException, 100, lines[0].ControlerName);
            }
            return result;
        }
    }
}