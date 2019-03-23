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
    public class ClaimServiceAcceptanceCriteriaTest : BaseTest
    {
        public ClaimServiceAcceptanceCriteriaTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("testdata/email_with_opening_tag_missing.txt", "Xml element 'cost_centre' is missing opening tag")]
        public void GivenParseEmail_WhenOpeningTagIsMissing_ExceptionIsThrown(string fileName, string expectedMsg)
        {
            string textFromFile = File.ReadAllText(fileName);
            Action actual = () => new ClaimService().ParseClaim(textFromFile);
            var exception = Assert.Throws<ApplicationException>(actual);
            Assert.Equal(expectedMsg, exception.Message);
        }

        [Theory]
        [InlineData("testdata/email_with_no_total.txt", "Xml Element 'total' is mandatory.")]
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
