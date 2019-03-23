using Claims.Business.Models;
using Claims.Business.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
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

    public class ClaimElementExtractor
    {

        public Claim Extract(string text)
        {
            // if no total element, throw exception
            //var totals = from totalElement in xml.Descendants("total") select (string)totalElement).ToList();

            return new Claim();
        }

        public Claim Extract(XElement xml)
        {
            // if no total element, throw exception
            //var totals = from totalElement in xml.Descendants("total") select (string)totalElement).ToList();

            return new Claim();
        }
    }

    

}
