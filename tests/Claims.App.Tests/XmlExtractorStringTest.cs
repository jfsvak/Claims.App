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
    public class XmlExtractorStringTest : BaseTest
    {
        public XmlExtractorStringTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("at <vendor>Viaduct Steakhouse</vendor> after <vendor>Somewhere else</vendor> end", "vendor")]
        public void GivenGetString_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text, string elementName)
        {
            Action actual = () => new XmlExtractor(text).GetString(elementName);
            Assert.ThrowsAny<XmlException>(actual); // we cannot handle multiple elements without a root node
        }

        [Theory]
        [InlineData("<vendor><vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "")]
        [InlineData("<vendor>before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "before ")]
        [InlineData("<vendor> before<vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", " before")]
        [InlineData("<vendor> \n before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", " \n before ")]
        public void GivenGetString_WhenTextContainsNestedElements_ThenOuterTextIsReturned(string text, string elementName, string expected)
        {
            var value = new XmlExtractor(text).GetString(elementName);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("<vendor>Viaduct Steakhouse</vendor>", "vendor", "Viaduct Steakhouse")]
        public void GivenGetString_WhenTextContainsnValidValue_ThenUnexpectedValueIsReturned(string text, string elementName, string expected)
        {
            var value = new XmlExtractor(text).GetString(elementName);
            output.WriteLine("Element value [{0}]", value);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "vendor", "Viaduct Steakhouse")]
        public void GivenGetString_WhenEmailContainsValidDecimal_ThenValueIsReturned(string fileName, string elementName, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var value = new XmlExtractor(textFromFile).GetString(elementName);
            Assert.Equal(expected, value);
        }

    }
}
