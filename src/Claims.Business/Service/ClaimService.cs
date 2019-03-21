using Claims.Business.Models;
using Claims.Business.Util;
using System;
using System.Xml;

namespace Claims.Business.Service
{
    public class ClaimService
    {

        public Claim ParseEmail(string email)
        {
            string textWithRootElement = string.Format("<root>{0}</root>", email);
            TextToXmlParser p = new TextToXmlParser();
            XmlDocument doc = p.ParseTextToXml(textWithRootElement);
            
            return new Claim();
        }

    }
}
