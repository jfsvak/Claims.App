using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Claims.Business.Util
{
    public class TextToXmlParser
    {
        public TextToXmlParser()
        {

        }

        public XmlDocument ParseTextToXml(string text)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(text);
            return doc;
        }

        public XElement ParseTextToXmlUsingLINQ(string text)
        {
            //string textWithRootElement = string.Format("<root>{0}</root>", text);
            XElement xmlElement = XElement.Parse(text);

            return xmlElement;
        }
    }
}
