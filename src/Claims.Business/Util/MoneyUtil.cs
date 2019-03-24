using System;
using System.Globalization;

namespace Claims.Business.Util
{
    /// <summary>
    /// Utility class to handle Culture specific parsing of strings into decimals
    /// </summary>
    public class MoneyUtil
    {
        /// <summary>Gets and sets the CultureInfo used for Culture specific parsing of money</summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Initialises a default MoneyUtil with default CultureInfo <see cref="ApplicationConstants.DEFAULT_CULTURE_CODE"/>
        /// </summary>
        public MoneyUtil() => this.Culture = CultureInfo.CreateSpecificCulture(ApplicationConstants.DEFAULT_CULTURE_CODE);

        /// <summary>
        /// Initialises MoneyUtil with a CultureInfo used for Culture specific parsing of money
        /// </summary>
        /// <param name="culture">CultureInfo to use for parsing of string</param>
        public MoneyUtil(CultureInfo culture) => this.Culture = culture;

        /// <summary>
        /// Parses a string into a decimal using the 
        /// </summary>
        /// <param name="input"></param>
        /// <returns>decimal parsed from <paramref name="input"/></returns>
        /// <exception cref="System.FormatException">Thrown if <paramref name="input"/> is not a valid decimal</exception>
        public decimal Parse(string input)
        {
            try
            {
                return Decimal.Parse(input, Culture);
            }
            catch (FormatException e)
            {
                throw new FormatException($"Input is not a valid decimal: [{input}]", e);
            }
        }
    }
}
