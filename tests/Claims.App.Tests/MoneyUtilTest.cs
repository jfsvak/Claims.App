using Claims.Business.Util;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class MoneyUtilTest : BaseTest
    {
        public MoneyUtilTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("100", 100)]
        [InlineData("1021.01", 1021.01)]
        [InlineData("-1021.01", -1021.01)]
        [InlineData("1,0,21.01", 1021.01)]
        [InlineData("0,0121.01", 121.01)]
        public void GivenParse_WhenValidInput_ValueIsReturned(string input, decimal expected)
        {
            decimal value = new MoneyUtil().Parse(input);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("100", 100)]
        [InlineData("1021,01", 1021.01)]
        [InlineData("-1021,01", -1021.01)]
        [InlineData("1.0.21,01", 1021.01)]
        [InlineData("0.0121,01", 121.01)]
        public void GivenParse_WhenNonDefaultCultureInput_ValueIsReturned(string input, decimal expected)
        {
            decimal value = new MoneyUtil(CultureInfo.CreateSpecificCulture("da-DK")).Parse(input);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("asdf", "Input is not a valid decimal: [asdf]")]
        public void GivenParse_WhenInvalidInput_ThenExceptionIsReturned(string input, string expectedMsg)
        {
            Action actual = () => new MoneyUtil().Parse(input);
            var exception = Assert.Throws<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }
    }
}
