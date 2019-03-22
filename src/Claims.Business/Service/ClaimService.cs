using Claims.Business.Models;
using Claims.Business.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Claims.Business.Service
{
    public class ClaimService
    {

        public Claim ParseEmail(string email)
        {
            string textWithRootElement = string.Format("<root>{0}</root>", email);
            XElement xml = new TextToXmlParser().ParseTextToXmlUsingLINQ(textWithRootElement);

            return new ClaimElementExtractor().Extract(xml);
        }
    }

    class ClaimElementExtractor
    {

        public Claim Extract(XElement xml)
        {
            // if no total element, throw exception
            //var totals = from totalElement in xml.Descendants("total") select (string)totalElement).ToList();

            return new Claim();
        }
    }

}
