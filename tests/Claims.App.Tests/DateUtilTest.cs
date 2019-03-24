using Claims.Business.Util;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class DateUtilTest : BaseTest
    {
        public DateUtilTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("01.01.2017", 2017, 01, 1)]
        [InlineData("2017 12 01", 2017, 12, 1)]
        [InlineData("Tuesday 25 April 2017", 2017, 4, 25)]
        [InlineData("Thursday 27 April 2017", 2017, 4, 27)]
        [InlineData("18 January 2018", 2018, 1, 18)]
        [InlineData("29. February 2016", 2016, 2, 29)]
        public void GivenParse_WhenValidInput_ValueIsReturned(string input, int year, int month, int day)
        {
            DateTime? value = new DateUtil().Parse(input);
            Assert.Equal(new DateTime(year, month, day), value);
        }

        [Theory]
        [InlineData("18 Januar 2018", 2018, 1, 18)]
        [InlineData("29. Februar 2016", 2016, 2, 29)]
        public void GivenParse_WhenNonDefaultCultureInput_ValueIsReturned(string input, int year, int month, int day)
        {
            DateTime? value = new DateUtil(CultureInfo.CreateSpecificCulture("da-DK")).Parse(input);
            Assert.Equal(new DateTime(year, month, day), value);
        }

        [Theory]
        [InlineData("asdf", "Input is not a valid date: [asdf]")]
        [InlineData("Tuesday 27 April 2017", "Input is not a valid date: [Tuesday 27 April 2017]")]
        [InlineData("29. February 2017", "Input is not a valid date: [29. February 2017]")]
        public void GivenParse_WhenInvalidInput_ThenExceptionIsReturned(string input, string expectedMsg)
        {
            Action actual = () => new DateUtil().Parse(input);
            var exception = Assert.Throws<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }
    }
}
