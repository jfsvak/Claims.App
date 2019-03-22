using Claims.Business.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class TotalElementExtractorTest
    {
        private readonly ITestOutputHelper output;

        public TotalElementExtractorTest(TestContext context, ITestOutputHelper output)
        {
            this.output = output;

            Console.SetOut(new Converter(output));
        }

        [Theory]
        [InlineData(null)]
        public void GivenExtract_WhenTextIsNull_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TotalElementExtractor().Extract(text);
            Assert.ThrowsAny<ArgumentNullException>(actual);
        }

        [Theory]
        [InlineData("")]
        public void GivenExtract_WhenTextIsEmpty_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TotalElementExtractor().Extract(text);
            Assert.ThrowsAny<ArgumentException>(actual);
        }

        [Theory]
        [InlineData("<total>1023.45")]
        [InlineData("before <total>1023.45<total>")]
        [InlineData("before 1023.45</total>")]
        public void GivenExtract_WhenTextContainsInvalidXML_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TotalElementExtractor().Extract(text);
            Assert.ThrowsAny<XmlException>(actual);
        }

        [Theory]
        [InlineData("before <total>ASDF</total> text after")]
        public void GivenExtract_WhenTextContainsInvalidAmount_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TotalElementExtractor().Extract(text);
            Assert.ThrowsAny<FormatException>(actual);
        }

        [Theory]
        [InlineData("<total>1023.45</total>", 1023.45)]
        [InlineData("before <total>1023.45</total>", 1023.45)]
        [InlineData("before <total>1023.45</total> after", 1023.45)]
        public void GivenExtract_WhenTextContainsElement_ThenValueIsReturned(string text, decimal value)
        {
            decimal total = new TotalElementExtractor().Extract(text);
            output.WriteLine("Total value [{0}]", total);
            Assert.Equal(value, total);
        }

        [Theory]
        [InlineData("<total><total>1023.45</total></total>")]
        [InlineData("<total>before <total>1023.45</total>")]
        [InlineData("<total>before <total>1023.45</total></total>")]
        [InlineData("before <total>1023.45</total> after <total>123</total> end")]
        public void GivenExtract_WhenTextContainsElementMultipleTimes_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TotalElementExtractor().Extract(text);
            Assert.ThrowsAny<FormatException>(actual);
        }
    }
}
