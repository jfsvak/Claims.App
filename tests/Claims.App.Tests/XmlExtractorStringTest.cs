using Claims.Business.Util;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class XmlExtractorStringTest : BaseTest
    {
        public XmlExtractorStringTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("at <vendor>Viaduct Steakhouse</vendor> after <vendor>Somewhere else</vendor> end", "vendor", "Multiple opening tags found for element [vendor]")]
        public void GivenGetString_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text, string elementName, string expectedMsg)
        {
            Action actual = () => new XmlExtractor(text).GetString(elementName);
            var exception = Assert.ThrowsAny<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("<vendor><vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "Multiple opening tags found for element [vendor]")]
        [InlineData("<vendor>before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "Multiple opening tags found for element [vendor]")]
        [InlineData("<vendor> before<vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "Multiple opening tags found for element [vendor]")]
        [InlineData("<vendor> \n before <vendor>Viaduct Steakhouse</vendor></vendor>", "vendor", "Multiple opening tags found for element [vendor]")]
        public void GivenGetString_WhenMultipleXmlElementsFound_ThenExceptionIsThrown(string text, string elementName, string expectedMsg)
        {
            Action actual = () => new XmlExtractor(text).GetString(elementName);
            var exception = Assert.ThrowsAny<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
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
