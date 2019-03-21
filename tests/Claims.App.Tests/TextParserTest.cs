using Claims.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Claims.App.Tests
{
    public class TextParserTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData(@"my email text<total>1345.56<total>")]
        [InlineData(@"my email text<total>1345.56<//total>")]
        [InlineData(@"my email text1345.56</total>")]
        public void TextParser_GivenParseText_WhenNoEndingTagExists_ThenExceptionIsThrown(string text)
        {
            Action actual = () => TextParser.ParseText(text);
            Assert.ThrowsAny<Exception>(actual);
        }

        [Fact]
        public void TextParser_GivenValidXML_ThenXMLIsReturned()
        {
            string textWithXML = @"my email text<total>1345.56</total>";
            XElement xml = TextParser.ParseText(textWithXML);

            Console.WriteLine(xml);

            List<string> total = 
                (from totalElement
                in xml.Descendants("total")
                select (string)totalElement).ToList();
            Assert.NotEmpty(total);
            Assert.Single(total);
            Assert.Equal("1345.56", total[0]);
        }


    }
}
