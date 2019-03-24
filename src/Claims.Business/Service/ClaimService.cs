using Claims.Business.Models;
using Claims.Business.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Claims.Business.Service
{
    public class ClaimService
    {
        const string UNKNOWN_COST_CENTRE = "UNKNOWN";

        public Claim ParseClaim(string email)
        {
            XmlExtractor extractor = new XmlExtractor(email);

            decimal total = extractor.GetDecimal("total") ?? throw new ApplicationException("Xml Element 'total' is mandatory.");

            decimal totalWithoutGST = new GSTCalculator().CalculateAmountWithoutGST(total);

            var c = new Claim
            {
                Id = Guid.NewGuid(),
                Expense = new Expense {
                    PaymentMethod = extractor.GetString("payment_method"),
                    Total = total,
                    TotalExclGST = totalWithoutGST,
                    CostCentre = extractor.GetString("cost_centre") ?? UNKNOWN_COST_CENTRE
                },
                Event = new Event
                {
                    Vendor = extractor.GetString("vendor"),
                    Description = extractor.GetString("description"),
                    Date = new DateUtil().Parse(extractor.GetString("date"))
                }
            };

            return c;
        }
    }
}
