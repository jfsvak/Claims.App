using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Claims.Business
{
    public class TextParser
    {
        public static XElement ParseText(string text)
        {
            string textWithRootElement = string.Format("<root>{0}</root>", text);
            XElement xmlElement = XElement.Parse(textWithRootElement);

            return xmlElement;
        }
    }
}
