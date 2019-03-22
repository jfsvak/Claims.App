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

    public class TotalElementExtractor
    {
        //const string TOTAL_REGEXP = @"total(\D+)total";
        const string REGEX_TOTAL_ELEMENT = @"<total>(?<totalgroup>.*)</total>";

        public decimal Extract(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (string.Empty.Equals(text))
                throw new ArgumentException("text");

            //Match m = Regex.Match(text, REGEX_TOTAL_ELEMENT);
            Match m = Regex.Match(text, REGEX_TOTAL_ELEMENT, RegexOptions.IgnoreCase);

            if (m.Success)
            {
                Group group = m.Groups["totalgroup"];

                Console.WriteLine("Group [{0}]: [{1}]", group.Index, group.Value);

                for (int cc = 0; cc < group.Captures.Count; cc++)
                {
                    Capture cap = group.Captures[cc];
                    Console.WriteLine("   Capture [{0}]: [{1}]", cc, cap.Value);
                }

                if (group.Captures.Count > 1)
                    Console.WriteLine("Found {0} <total> XML tags. Returning the first one.", group.Captures.Count);

                try
                {
                    return Convert.ToDecimal(group.Captures[0].Value);
                } catch(FormatException e)
                {
                    throw new FormatException("Total value [{group.Captures[0].Value}] is not a valid amount: ", e);
                }
            }

            string msg = string.Format("XML element <{0}> not found in text: [{1}]", "total", text);
            Console.WriteLine(msg);
            throw new System.Xml.XmlException(msg);
        }
    }

}
