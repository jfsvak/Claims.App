using Claims.Business.Util;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class ClaimServiceAcceptanceCriteriaTest : BaseTest
    {
        public ClaimServiceAcceptanceCriteriaTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("testdata/email_with_opening_tag_missing.txt", "No opening tag found for element [cost_centre]")]
        public void GivenParseEmail_WhenOpeningTagIsMissing_ExceptionIsThrown(string fileName, string expectedMsg)
        {
            string textFromFile = File.ReadAllText(fileName);
            Action actual = () => new ClaimService().ParseClaim(textFromFile);
            var exception = Assert.Throws<FormatException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("testdata/email_with_no_total.txt", "Xml element 'total' is missing.")]
        public void GivenParseEmail_WhenTotalTagIsMissing_ExceptionIsThrown(string fileName, string expectedMsg)
        {
            string textFromFile = File.ReadAllText(fileName);
            Action actual = () => new ClaimService().ParseClaim(textFromFile);
            var exception = Assert.Throws<ApplicationException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("testdata/email_with_no_cost_centre.txt", "UNKNOWN")]
        public void GivenParseEmail_WhenCostCentreTagIsMissing_UnknownCostCentreIsReturned(string fileName, string expectedMsg)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expectedMsg, claim.Expense.CostCentre);
        }
    }
}
