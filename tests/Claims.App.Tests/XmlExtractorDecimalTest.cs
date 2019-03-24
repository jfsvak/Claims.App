using Claims.Business.Util;
using System;
using System.Globalization;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class XmlExtractorDecimalTest : BaseTest
    {
        public XmlExtractorDecimalTest(TestContext context, ITestOutputHelper output) : base(context, output) {}

        [Theory]
        [InlineData("before <total>1.24.532</total> text after")]
        [InlineData("before <total>ASDF</total> text after")]
        public void GivenGetDecimal_WhenFoundElementContainsInvalidAmount_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new XmlExtractor(text).GetDecimal("total");
            Assert.ThrowsAny<FormatException>(actual);
        }

        [Theory]
        [InlineData("<total><total>1023.45</total></total>")]
        [InlineData("<total>before <total>1023.45</total></total>")]
        public void GivenGetDecimal_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new XmlExtractor(text).GetDecimal("total");
            Assert.ThrowsAny<FormatException>(actual);
        }

        [Theory]
        [InlineData("<total>1023.45</total>", 1023.45)]
        [InlineData("before <total>1023.45</total>", 1023.45)]
        [InlineData("before <total>1023.45</total> after", 1023.45)]
        [InlineData("before <total>11,987.45</total> after", 11987.45)]
        [InlineData("before <total>1,24.53</total> text after", 124.53)] // <- Maybe this is not a desired effect
        public void GivenGetDecimal_WhenElementContainsValidValue_ThenValueIsReturned(string text, decimal expected)
        {
            var total = new XmlExtractor(text).GetDecimal("total");
            output.WriteLine("Total value [{0}]", total);
            Assert.Equal(expected, total);
        }

        [Theory]
        [InlineData("<total>1023,45</total>", 1023.45)]
        [InlineData("before <total>1023,45</total>", 1023.45)]
        [InlineData("before <total>1023,45</total> after", 1023.45)]
        [InlineData("before <total>1.122,98</total> after", 1122.98)]
        public void GivenGetDecimal_WhenElementContainsValidValueNonENUSCulture_ThenValueIsReturned(string text, decimal expected)
        {
            var total = new XmlExtractor(text, CultureInfo.CreateSpecificCulture("da-DK")).GetDecimal("total");
            output.WriteLine("Total value [{0}]", total);
            Assert.Equal(expected, total);
        }

        [Theory]
        [InlineData("<total>1023.45</total>", 1023.45)]
        public void GivenGetDecimal_WhenTextContainsInValidValueNonENUSCulture_ThenUnexpectedValueIsReturned(string text, decimal expected)
        {
            var total = new XmlExtractor(text, CultureInfo.CreateSpecificCulture("da-DK")).GetDecimal("total");
            output.WriteLine("Total value [{0}]", total);
            Assert.NotEqual(expected, total);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "total", 1024.01)]
        public void GivenGetDecimal_WhenEmailContainsValidDecimal_ThenValueIsReturned(string fileName, string elementName, decimal expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var value = new XmlExtractor(textFromFile, CultureInfo.CreateSpecificCulture("en-US")).GetDecimal(elementName);
            Assert.Equal(expected, value);
        }
    }
}
