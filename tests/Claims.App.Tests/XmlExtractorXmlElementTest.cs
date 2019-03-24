using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

using Claims.Business.Util;
using System.Globalization;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class XmlExtractorXmlElementTest : BaseTest
    {
        public XmlExtractorXmlElementTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData(null)]
        public void GivenGetXmlElement_WhenTextIsNull_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement("expense");
            Assert.Throws<ArgumentNullException>(actual);
        }

        [Theory]
        [InlineData("")]
        public void GivenGetXmlElement_WhenTextIsEmpty_ThenNullIsReturned(string text)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement("expense");
            Assert.Null(xml);
        }

        [Theory]
        [InlineData("<vendor>1023.45", "vendor", "No closing tag found for element [vendor]")] // no ending tag
        [InlineData("at Viaduct Steakhouse</vendor>", "vendor", "No opening tag found for element [vendor]")] // no opening tag
        [InlineData("at <vendor>Viaduct Steakhouse<vendor>", "vendor", "No closing tag found for element [vendor]")] // despite multiple opening tags, validation for closing tag comes first, so that is triggered here
        [InlineData("at <vendor>Viaduct Steakhouse<vendor></vendor>", "vendor", "Multiple opening tags found for element [vendor]")] // multiple opening tags, no ending
        public void GivenGetXmlElement_WhenTagsNotBalanced_ThenExceptionIsThrown(string text, string elementName, string expectedMsg)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement(elementName);
            var exception = Assert.Throws<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("<vendor>Viaduct Steakhouse</vendor>", "cost_centre")] // neither opening nor closing tag found
        public void GivenGetXmlElement_WhenTextDoesntContainElementWeAreLookingFor_ThenNullIsReturned(string text, string elementName)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Null(xml);
        }

        [Theory]
        [InlineData("<vendor>before <othertag>Viaduct Steakhouse</vendor>", "vendor")]
        public void GivenGetXmlElement_WhenFoundElementContainsInvalidXml_ThenExceptionIsThrown(string text, string elementName)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Throws<XmlException>(actual);
        }

        [Theory]
        [InlineData("<outertag><vendor>Viaduct Steakhouse</vendor></outertag>", "outertag", "<vendor>Viaduct Steakhouse</vendor>")]
        [InlineData("<outertag>before <vendor>Viaduct Steakhouse</vendor></outertag>", "outertag", "before <vendor>Viaduct Steakhouse</vendor>")]
        [InlineData("<outertag> \n before <vendor>Viaduct Steakhouse</vendor></outertag>", "outertag", " \n before <vendor>Viaduct Steakhouse</vendor>")]
        public void GivenGetXmlElement_WhenTextContainsNestedElements_ThenValueIsReturned(string text, string elementName, string expected)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Equal(expected, xml.InnerXml);
        }

        [Theory]
        [InlineData("at <vendor>Viaduct Steakhouse</vendor> after <vendor>Somewhere else</vendor> end", "vendor", "Multiple opening tags found for element [vendor]")]
        public void GivenGetXmlElement_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text, string elementName, string expectedMsg)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement(elementName);
            var exception = Assert.Throws<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("<a><b>B</b><c>C</c></a>", "a", "/a/c", "C")]
        public void GivenGetXmlElement_WhenTextContainsValidXml_ThenValueIsReturned(string text, string elementName, string nodePath, string expected)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            var node = xml.SelectSingleNode(nodePath);
            output.WriteLine($"Result {node.FirstChild.Value}");
            Assert.Equal(expected, node.FirstChild.Value);
        }


        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "expense", "/expense/cost_centre/text()", "DEV002")]
        public void GivenGetXmlElement_WhenEmailContainsValidXml_ThenValueIsReturned(string fileName, string elementName, string nodePath, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            XmlElement xml = new XmlExtractor(textFromFile).GetXmlElement(elementName);
            var node = xml.SelectSingleNode(nodePath);
            Assert.Equal(expected, node.Value);
        }

    }
}
