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
        [InlineData("<vendor>1023.45", "vendor")]
        [InlineData("at <vendor>Viaduct Steakhouse<vendor>", "vendor")]
        [InlineData("at Viaduct Steakhouse</vendor>", "vendor")]
        [InlineData("<vendor>Viaduct Steakhouse</vendor>", "cost_centre")] 
        public void GivenGetXmlElement_WhenTextDoesntContainValidElement_ThenNullIsReturned(string text, string elementName)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Null(xml);
        }

        [Theory]
        [InlineData("<vendor>Viaduct Steakhouse</vendor>", "cost_centre")] // Note: we are looking for 'cost_centre', not 'vendor'
        public void GivenGetXmlElement_WhenTextDoesntContainElementWeAreLookingFor_ThenNullIsReturned(string text, string elementName)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Null(xml);
        }

        [Theory]
        [InlineData("<vendor>before <vendor>Viaduct Steakhouse</vendor>", "vendor")]
        public void GivenGetXmlElement_WhenFoundElementContainsInvalidXml_ThenExceptionIsThrown(string text, string elementName)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Throws<XmlException>(actual);
        }

        [Theory]
        [InlineData("<vendor><vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "<vendor>Viaduct Steakhouse</vendor>")]
        [InlineData("<vendor>before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "before <vendor>Viaduct Steakhouse</vendor>")]
        [InlineData("<vendor> \n before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", " \n before <vendor>Viaduct Steakhouse</vendor>")]
        public void GivenGetXmlElement_WhenTextContainsNestedElements_ThenXmlElementIsReturned(string text, string elementName, string expected)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Equal(expected, xml.InnerXml);
        }

        [Theory]
        [InlineData("at <vendor>Viaduct Steakhouse</vendor> after <vendor>Somewhere else</vendor> end", "vendor")]
        public void GivenGetXmlElement_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text, string elementName)
        {
            Action actual = () => new XmlExtractor(text).GetXmlElement(elementName);
            Assert.Throws<XmlException>(actual);
        }

        [Theory]
        [InlineData("<a><b>B</b><c>C</c></a>", "a", "/a/c", "C")]
        public void GivenGetXmlElement_WhenTextContainsValidXml_ThenXmlElementIsReturned(string text, string elementName, string nodePath, string expected)
        {
            XmlElement xml = new XmlExtractor(text).GetXmlElement(elementName);
            var node = xml.SelectSingleNode(nodePath);
            output.WriteLine($"Result {node.FirstChild.Value}");
            Assert.Equal(expected, node.FirstChild.Value);
        }


        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "expense", "/expense/cost_centre/text()", "DEV002")]
        public void GivenGetXmlElement_WhenEmailContainsValidXml_ThenXmlElementIsReturned(string fileName, string elementName, string nodePath, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            XmlElement xml = new XmlExtractor(textFromFile).GetXmlElement(elementName);
            var node = xml.SelectSingleNode(nodePath);
            Assert.Equal(expected, node.Value);
        }

    }
}
