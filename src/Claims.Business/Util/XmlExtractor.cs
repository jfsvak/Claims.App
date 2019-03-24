using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Claims.Business.Util
{
    /// <summary>
    /// Utility class for extracting xml from a string of text.
    /// </summary>
    public class XmlExtractor
    {
        const string REGEX_XML_INCL_TAG_PATTERN = @"(?<{1}><{0}>.*</{0}>)";
        private const string GROUP_NAME_POSTFIX = "Group";

        /// <summary>CultureInfo used for parsing Culture specific data</summary>
        public CultureInfo Culture;

        /// <summary>Gets and sets the MoneyUtils used for Culture sensitive parsing of money</summary>
        //private MoneyUtil MoneyUtils { get;set; }

        /// <summary>Gets and sets the value of Text that is being parsed</summary>
        public string Text { get; set; }

        /// <summary>
        /// Initialises a default XmlExtractor with no text. 
        /// </summary>
        /// <remarks>
        /// Text has to be set before calling get-methods.
        /// </remarks>
        public XmlExtractor() => this.Culture = CultureInfo.CreateSpecificCulture(ApplicationConstants.DEFAULT_CULTURE_CODE);

        /// <summary>
        /// Initialises an XmlExtractor with a Text
        /// </summary>
        /// <param name="text">Text containing xml to be parsed</param>
        public XmlExtractor(string text) : this() => this.Text = text;

        /// <summary>
        /// Initialises an XmlExtractor with a Text and a MoneyUtils for Culture sensitive parsing of money
        /// </summary>
        /// <param name="text">Text containing xml to be parsed</param>
        /// <param name="culture">CultureInfo to use for parsing money</param>
        public XmlExtractor(string text, CultureInfo culture) : this(text) => this.Culture = culture;

        /// <summary>
        /// Initialises an XmlExtractor with a MoneyUtils for Culture specific parsing of money
        /// </summary>
        /// <remarks>
        /// Text has to be set before calling get-methods.
        /// </remarks>
        /// <param name="culture">CultureInfo to use for parsing money</param>
        public XmlExtractor(CultureInfo culture) : this() => Culture = culture;

        /// <summary>
        /// Gets the xml element for the provided tag name
        /// </summary>
        /// <param name="tagName">Tag name for the xml element to get</param>
        /// <returns>Parsed XmlElement containing the xml for the tagName</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if Text or tagName is null</exception>
        /// <exception cref="System.FormatException">Thrown if no closing or opening tags were found or multiple tags were found</exception>
        public XmlElement GetXmlElement(string tagName)
        {
            if (this.Text == null)
                throw new ArgumentNullException("Input text cannot be null");

            if (tagName == null)
                throw new ArgumentNullException("tagName cannot be null");

            Validate(tagName);

            string parameterizedPattern = string.Format(REGEX_XML_INCL_TAG_PATTERN, tagName, tagName + GROUP_NAME_POSTFIX);

            Match m = Regex.Match(this.Text, parameterizedPattern, RegexOptions.IgnoreCase|RegexOptions.Singleline);

            if (m.Success)
            {
                string extractedString = m.Groups[tagName + GROUP_NAME_POSTFIX].Captures[0].Value;

                return ParseToXmlElement(extractedString);
            }

            return null;
        }

        /// <summary>
        /// Gets the text value from the xml element with the provided <paramref name="tagName"/>
        /// </summary>
        /// <remarks>
        /// If no xml element found with the given <paramref name="tagName"/>, null is return
        /// </remarks>
        /// <param name="tagName">Tag name for the xml element to get</param>
        /// <returns>Text value from the xml element with <paramref name="tagName"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if Text or tagName is null</exception>
        /// <exception cref="System.FormatException">Thrown if no closing or opening tags were found or multiple tags were found</exception>
        /// <exception cref="System.ApplicationException">Thrown if found xml element is not a simple text node</exception>
        public string GetString(string tagName)
        {
            var xml = GetXmlElement(tagName);

            if (xml == null)
                return null;

            if (xml.ChildNodes.Count != 1 || xml.FirstChild.NodeType != XmlNodeType.Text)
                throw new ApplicationException($"The found Xml Element is not a simple text element: {xml.InnerXml}");

            return xml.FirstChild.Value;
        }

        /// <summary>
        /// Gets the xml element for the provided <paramref name="tagName"/>
        /// </summary>
        /// <remarks>
        /// If no xml element found with the given <paramref name="tagName"/>, null is return
        /// </remarks>
        /// <param name="tagName">Tag name for the xml element to get</param>
        /// <returns>decimal value from the xml element with <paramref name="tagName"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown if Text or <paramref name="tagName"/> is null</exception>
        /// <exception cref="System.FormatException">Thrown if no closing or opening tags were found or multiple tags were found</exception>
        /// <exception cref="System.ApplicationException">Thrown if found xml element is not a simple number node</exception>
        public decimal? GetDecimal(string tagName)
        {
            var xml = GetXmlElement(tagName);

            if (xml == null)
                return null;

            if (xml.ChildNodes.Count != 1 || xml.FirstChild.NodeType != XmlNodeType.Text)
                throw new ApplicationException($"The found Xml Element is not a decimal element: {xml.InnerXml}");

            string nodeValue = xml.FirstChild.Value;

            try
            {
                return new MoneyUtil(this.Culture).Parse(nodeValue);
            }
            catch (FormatException e)
            {
                throw new FormatException($"{tagName} value [{nodeValue}] is not a valid decimal: ", e);
            }
        }

        /// <summary>
        /// Parses a string containing xml into an XmlElement. By default whitespaces are preserved to allow for 
        /// text content.
        /// </summary>
        private static XmlElement ParseToXmlElement(string xmlString, bool preserveWhitespace = true)
        {
            try
            {
                XmlDocument doc = new XmlDocument
                {
                    PreserveWhitespace = preserveWhitespace
                };
                doc.LoadXml(xmlString);
                return doc.DocumentElement;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Validates that the <paramref name="tagName"/> is a valid xml element in the text 
        /// </summary>
        /// <param name="tagName"></param>
        private void Validate(string tagName)
        {
            ValidateOpeningTag(tagName);
            ValidateClosingTag(tagName);
            ValidateMultipleTags(tagName);
        }

        /// <summary>
        /// Validates that an opening tag exists for <paramref name="tagName"/>
        /// </summary>
        /// <param name="tagName"></param>
        private void ValidateOpeningTag(string tagName)
        {
            // if no opening tag, but closing tag exists
            if (this.Text.IndexOf(CreateOpeningTag(tagName)) < 0 && Text.Contains(CreateClosingTag(tagName)))
                throw new FormatException($"No opening tag found for element [{tagName}]");
        }

        /// <summary>
        /// Validates that a closing tag exists for <paramref name="tagName"/>
        /// </summary>
        /// <param name="tagName"></param>
        private void ValidateClosingTag(string tagName)
        {
            // if no closing tag, but opening tag exists
            if (this.Text.IndexOf(CreateClosingTag(tagName)) < 0 && this.Text.Contains(CreateOpeningTag(tagName)))
                throw new FormatException($"No closing tag found for element [{tagName}]");
        }

        /// <summary>
        /// Validates that the multiple tags with <paramref name="tagName"/> doesn't exist
        /// </summary>
        /// <param name="tagName"></param>
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
    }
}
