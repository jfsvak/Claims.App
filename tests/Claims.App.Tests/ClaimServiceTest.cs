﻿using Claims.Business.Util;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Claims.App.Tests
{
    [Collection("TestContextCollection")]
    public class ClaimServiceTest : BaseTest
    {
        public ClaimServiceTest(TestContext context, ITestOutputHelper output) : base(context, output) { }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "DEV002")]
        public void GivenParseEmail_WhenEmailContainsValidCostCentre_ThenValueIsReturned(string fileName, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expected, claim.Expense.CostCentre);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", 1024.01)]
        public void GivenParseEmail_WhenEmailContainsValidTotal_ThenValueIsReturned(string fileName, decimal expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expected, claim.Expense.Total);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "personal card")]
        public void GivenParseEmail_WhenEmailContainsValidClaim_ThenValueIsReturned(string fileName, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expected, claim.Expense.PaymentMethod);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "Viaduct Steakhouse")]
        public void GivenParseEmail_WhenEmailContainsValidVendor_ThenValueIsReturned(string fileName, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expected, claim.Event.Vendor);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt", "development team’s project end celebration dinner")]
        public void GivenParseEmail_WhenEmailContainsValidDescription_ThenValueIsReturned(string fileName, string expected)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            Assert.Equal(expected, claim.Event.Description);
        }

        [Theory]
        [InlineData("testdata/email_with_email_tag.txt")]
        public void GivenParseEmail_WhenEmailContainsValidDate_ThenValueIsReturned(string fileName)
        {
            string textFromFile = File.ReadAllText(fileName);
            var claim = new ClaimService().ParseClaim(textFromFile);
            var expectedDT = new DateTime(2017, 4, 25);
            //DateTime.TryParse(expected, out expectedDT);
            Assert.Equal(expectedDT, claim.Event.Date);
        }
    }
}
