using System;
using System.Globalization;

namespace Claims.Business.Util
{
    /// <summary>
    /// Utility class used for parsing strings into DateTime
    /// </summary>
    public class DateUtil
    {
        /// <summary>CultureInfo used for Culture specific parsing of string</summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Initialises DateUtil with default CultureInfo code <see cref="ApplicationConstants.DEFAULT_CULTURE_CODE"/>
        /// </summary>
        public DateUtil() => Culture = CultureInfo.CreateSpecificCulture(ApplicationConstants.DEFAULT_CULTURE_CODE);

        /// <summary>
        /// Initialises DateUtil with a specific CultureInfo
        /// </summary>
        /// <param name="culture">CultureInfo to use for Culture specific parsing of dates</param>
        public DateUtil(CultureInfo culture) => Culture = culture;

        /// <summary>
        /// Parses a string into a DateTime
        /// </summary>
        /// <remarks>If <paramref name="input"/> is null, null is returned. No exception is thrown</remarks>
        /// <param name="input"></param>
        /// <returns>DateTime parsed from <paramref name="input"/></returns>
        /// <exception cref="System.FormatException">Thrown if <paramref name="input"/> cannot be parsed into a DateTime</exception>
        public DateTime? Parse(string input)
        {
            if (input == null)
                return null;

            var dt = DateTime.Now;

            if (DateTime.TryParse(input, Culture, ApplicationConstants.DEFAULT_DATETIME_STYLES, out dt))
                return dt;
            else
                throw new FormatException($"Input is not a valid date: [{input}]");
        }
    }
}
