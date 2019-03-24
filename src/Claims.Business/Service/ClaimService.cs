using Claims.Business.Models;
using System;
using System.Globalization;

namespace Claims.Business.Util
{
    /// <summary>
    /// Service containing business logic relating to Claims handling
    /// </summary>
    public class ClaimService
    {
        /// <summary>Default Cost Centre used when cost_centre tag is missing</summary>
        const string COST_CENTRE_UNKNOWN = "UNKNOWN";

        /// <summary>CultureInfo used for parsing Culture sensitive data</summary>
        public CultureInfo Culture;

        /// <summary>
        /// Initialises service with default CultureInfo code <see cref="ApplicationConstants.DEFAULT_CULTURE_CODE"/>
        /// </summary>
        public ClaimService() => this.Culture = CultureInfo.CreateSpecificCulture(ApplicationConstants.DEFAULT_CULTURE_CODE);

        /// <summary>
        /// Initialises service with a CultureInfo used for parsing Culture specific data
        /// </summary>
        /// <param name="culture">CultureInfo to use for parsing Culture specific data</param>
        public ClaimService(CultureInfo culture) : this() => this.Culture = culture;

        /// <summary>
        /// Parses a block of text into a <see cref="Claims.Business.Model.Claim"/> entity.
        /// Total amount without GST is calculated from total.
        /// </summary>
        /// <remarks>All xml elements except total and cost_centre are optional</remarks>
        /// <remarks>If total element is missing an <see cref="System.ApplicationException"/> is thrown</remarks>
        /// <remarks>If cost_centre xml element is missing, <see cref="COST_CENTRE_UNKNOWN"/> is set</remarks>
        /// <param name="text">text containing xml elements and mark ups</param>
        /// <returns>Claim entity extracted from <paramref name="text"/></returns>
        /// <exception cref="System.ApplicationException">Thrown if total tag is missing</exception>
        /// <exception cref="System.FormatException">Thrown if tags or content cannot be parsed</exception>
        public Claim ParseClaim(string text)
        {
            XmlExtractor extractor = new XmlExtractor(text, Culture);

            decimal total = extractor.GetDecimal("total") ?? throw new ApplicationException("Xml element 'total' is missing.");

            decimal totalWithoutGST = new GSTCalculator().CalculateAmountWithoutGST(total);

            var c = new Claim
            {
                Id = Guid.NewGuid(),
                Expense = new Expense {
                    Id = Guid.NewGuid(),
                    PaymentMethod = extractor.GetString("payment_method"),
                    Total = total,
                    TotalExclGST = totalWithoutGST,
                    CostCentre = extractor.GetString("cost_centre") ?? COST_CENTRE_UNKNOWN
                },
                Event = new Event
                {
                    Id = Guid.NewGuid(),
                    Vendor = extractor.GetString("vendor"),
                    Description = extractor.GetString("description"),
                    Date = new DateUtil(Culture).Parse(extractor.GetString("date"))
                }
            };

            return c;
        }
    }
}
