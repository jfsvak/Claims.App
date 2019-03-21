using Claims.Business;
using Claims.Business.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace Claims.App.Tests
{
    public class TextToXmlParserTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Email text. total = 1345.56")]
        [InlineData("<root>Email text <total>1345.56<total>. More text. </root>")]
        [InlineData("<root>Email text <total>1345.56<//total>. More text. </root>")]
        [InlineData("<root>Email text <total>1345.56. More text. </root>")]
        [InlineData("<root>Email text total 1345.56</total>. More text. </root>")]
        public void TextToXmlParser_GivenParseTextToXml_WhenInvalidXmlInText_ThenExceptionIsThrown(string text)
        {
            Action actual = () => new TextToXmlParser().ParseTextToXml(text);
            Assert.ThrowsAny<Exception>(actual);
        }

        [Theory]
        [InlineData("<root></root>")]
        [InlineData("<root>Email text</root>")]
        [InlineData("<root>Email text <total/>. More text. </root>")]
        [InlineData("<root>Email text <total>1345.56</total>. More text.</root>")]
        public void TextToXmlParser_GivenParseTextToXml_WhenValidXmlInText_ThenXmlIsReturned(string text)
        {
            XmlDocument actual = new TextToXmlParser().ParseTextToXml(text);
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt")]
        [InlineData("testdata/email_without_email_tag.txt")]
        public void TextToXmlParser_GivenParseTextToXmlFromFile_WhenValidXmlInText_ThenXmlIsReturned(string fileName)
        {
            string textFromFile = string.Format("<root>{0}</root>", File.ReadAllText(fileName));
            XmlDocument actual = new TextToXmlParser().ParseTextToXml(textFromFile);
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("<root>my email text<total>1345.56</total> after text</root>")]
        public void TextToXmlParser_GivenParseTextToXmlUsingLINQ_WhenValidText_ThenXmlIsReturned(string text)
        {
            
            XElement xml = new TextToXmlParser().ParseTextToXmlUsingLINQ(text);

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
