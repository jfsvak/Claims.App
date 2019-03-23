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
using Claims.Business.Service;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class GSTCalculatorTest : BaseTest
    {
        public GSTCalculatorTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        public object GSTCalculator { get; private set; }

        [Theory]
        [InlineData(115, 100)]
        [InlineData(200, 173.91)]
        [InlineData(1024.01, 890.44)]
        [InlineData(65.7651, 57.19)]
        [InlineData(0.076, 0.07)]
        public void GivenCalculateGST_WhenAmountWithGST_ThenAmountWithoutGSTIsReturned(decimal amountWithGST, decimal expected)
        {
            decimal amountWithoutGST = new GSTCalculator(amountWithGST).AmountWithoutGST();
            Assert.Equal(expected, amountWithoutGST);
        }

        [Theory]
        [InlineData(115, 15)]
        [InlineData(200, 26.09)]
        [InlineData(1024.01, 133.57)]
        [InlineData(65.7651, 8.58)]
        [InlineData(0.076, 0.01)]
        public void GivenCalculateGST_WhenAmountWithGST_ThenGSTIsReturned(decimal amountWithGST, decimal expected)
        {
            decimal gstAmount = new GSTCalculator(amountWithGST).GSTAmount();
            output.WriteLine($"GSTAmount [{gstAmount}]");
            Assert.Equal(expected, gstAmount);
        }
    }
}
