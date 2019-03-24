using Claims.Business.Util;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class GSTCalculatorTest : BaseTest
    {
        public GSTCalculatorTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData(115, 100)]
        [InlineData(200, 173.91)]
        [InlineData(1024.01, 890.44)]
        [InlineData(65.7651, 57.19)]
        [InlineData(0.076, 0.07)]
        public void GivenAmountWithoutGST_WhenAmountWithGST_ThenAmountWithoutGSTIsReturned(decimal amountWithGST, decimal expected)
        {
            decimal amountWithoutGST = new GSTCalculator().CalculateAmountWithoutGST(amountWithGST);
            Assert.Equal(expected, amountWithoutGST);
        }

        [Theory]
        [InlineData(115, 15)]
        [InlineData(200, 26.09)]
        [InlineData(1024.01, 133.57)]
        [InlineData(65.7651, 8.58)]
        [InlineData(0.076, 0.01)]
        [InlineData(0, 0)]
        public void GivenGSTAmount_WhenAmountWithGST_ThenGSTIsReturned(decimal amountWithGST, decimal expected)
        {
            decimal gstAmount = new GSTCalculator().CalculateGSTAmount(amountWithGST);
            output.WriteLine($"GSTAmount [{gstAmount}]");
            Assert.Equal(expected, gstAmount);
        }

        [Theory]
        [InlineData(-100, "Amount cannot be negative")]
        public void GivenAmountWithout_WhenNegativeAmount_ThenExceptionIsReturned(decimal amountWithGST, string expected)
        {
            Action actual = () => new GSTCalculator().CalculateGSTAmount(amountWithGST);
            var exception = Assert.Throws<ArgumentException>(actual);
            Assert.Equal(expected, exception.Message);
        }
    }
}
