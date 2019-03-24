using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Claims.Business.Util
{
    public class XmlExtractor
    {
        //const string TOTAL_REGEXP = @"total(\D+)total";
        //const string REGEX_XML_TAG_PATTERN = @"<total>(?<totalgroup>.*)</total>";
        //const string REGEX_XML_TAG_PATTERN = @"<{0}>(?<{1}>.*)</{0}>";
        const string REGEX_XML_INCL_TAG_PATTERN = @"(?<{1}><{0}>.*</{0}>)";
        private const string GROUP_NAME_POSTFIX = "Group";

        public MoneyUtil MoneyUtils {get;set;}
        public string Text { get; set; }

        public XmlExtractor() => MoneyUtils = new MoneyUtil();
        public XmlExtractor(string text) : this() => this.Text = text;
        public XmlExtractor(string text, MoneyUtil moneyUtils) : this(text) => MoneyUtils = moneyUtils;
        public XmlExtractor(MoneyUtil moneyUtils) => MoneyUtils = moneyUtils;

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public XmlElement GetXmlElement(string elementName)
        {
            if (this.Text == null)
                throw new ArgumentNullException("Input text cannot be null");

            if (elementName == null)
                throw new ArgumentNullException("ElementName cannot be null");

            Validate(elementName);

            string parameterizedPattern = string.Format(REGEX_XML_INCL_TAG_PATTERN, elementName, elementName + GROUP_NAME_POSTFIX);

            Console.WriteLine($"Pattern: [{parameterizedPattern}]");

            Match m = Regex.Match(this.Text, parameterizedPattern, RegexOptions.IgnoreCase|RegexOptions.Singleline);

            if (m.Success)
            {
                Console.WriteLine($"Group index for [{elementName + GROUP_NAME_POSTFIX}] : {m.Groups[elementName + GROUP_NAME_POSTFIX].Index}");

                Console.WriteLine($"Group count: {m.Groups.Count}");

                for (int c = 0; c < m.Groups.Count; c++)
                {
                    Group group = m.Groups[c];

                    Console.WriteLine($"Group [{c}]: [{group.Value}]");

                    for (int cc = 0; cc < group.Captures.Count; cc++)
                        Console.WriteLine($"   Capture [{cc}]: [{group.Captures[cc].Value}]");

                    if (group.Index == m.Groups[elementName + GROUP_NAME_POSTFIX].Index && group.Captures.Count > 1)
                    {
                        var appException = new ApplicationException($"Found {group.Captures.Count} <{elementName}> XML tags. Throwing Exception.");
                        Console.WriteLine(appException.Message);
                        throw appException;
                    }
                }

                string extractedString = m.Groups[elementName + GROUP_NAME_POSTFIX].Captures[0].Value;
                Console.WriteLine($"Found: [{extractedString}]");

                return ParseToXmlElement(extractedString);
            }

            //var notFoundException = new KeyNotFoundException($"XML element <{elementName}> not found in text: [{this.Text}]");
            //Console.WriteLine(notFoundException.Message);
            //throw notFoundException;
            return null;
        }

        private void Validate(string tagName)
        {
            ValidateOpeningTag(tagName);
            ValidateClosingTag(tagName);
            ValidateMultipleTags(tagName);
        }

        private void ValidateOpeningTag(string tagName)
        {
            // if no opening tag, but closing tag exists
            if (this.Text.IndexOf(CreateOpeningTag(tagName)) < 0 && Text.Contains(CreateClosingTag(tagName)))
                throw new FormatException($"No opening tag found for element [{tagName}]");
        }

        private void ValidateClosingTag(string tagName)
        {
            // if no closing tag, but opening tag exists
            if (this.Text.IndexOf(CreateClosingTag(tagName)) < 0 && this.Text.Contains(CreateOpeningTag(tagName)))
                throw new FormatException($"No closing tag found for element [{tagName}]");
        }

        private void ValidateMultipleTags(string tagName)
        {
            var openingTag = CreateOpeningTag(tagName);
            var closingTag = CreateClosingTag(tagName);

            if (this.Text.IndexOf(openingTag) != this.Text.LastIndexOf(openingTag))
                throw new FormatException($"Multiple opening tags found for element [{tagName}]");

            if (this.Text.IndexOf(closingTag) != this.Text.LastIndexOf(closingTag))
                throw new FormatException($"Multiple closing tags found for element [{tagName}]");
        }

        private string CreateOpeningTag(string tagName) => $"<{tagName}>";
        private string CreateClosingTag(string tagName) => $"</{tagName}>";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public string GetString(string elementName)
        {
            var xml = GetXmlElement(elementName);

            if (xml == null)
                return null;

            if (xml.ChildNodes.Count != 1 || xml.FirstChild.NodeType != XmlNodeType.Text)
                throw new ApplicationException($"The found Xml Element is not a simple text element: {xml.InnerXml}");

            return xml.FirstChild.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public decimal? GetDecimal(string elementName)
        {
            var xml = GetXmlElement(elementName);

            if (xml == null)
                return null;

            if (xml.ChildNodes.Count != 1 || xml.FirstChild.NodeType != XmlNodeType.Text)
                throw new ApplicationException($"The found Xml Element is not a decimal element: {xml.InnerXml}");

            string nodeValue = xml.FirstChild.Value;

            try
            {
                var amount = MoneyUtils.Parse(nodeValue);
                Console.WriteLine($"Converted [{nodeValue}] to [{amount}]");
                return amount;
            }
            catch (FormatException e)
            {
                throw new FormatException($"{elementName} value [{nodeValue}] is not a valid decimal: ", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extractedString"></param>
        /// <returns></returns>
        private static XmlElement ParseToXmlElement(string extractedString, bool ignoreWhitespace = true)
        {
            try
            {
                XmlDocument doc = new XmlDocument
                {
                    PreserveWhitespace = ignoreWhitespace
                };
                doc.LoadXml(extractedString);
                return doc.DocumentElement;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception while parsing text [{0}]: {1}", extractedString, e.Message);
                throw e;
            }
        }
    }
}
