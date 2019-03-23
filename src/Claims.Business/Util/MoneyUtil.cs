using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Claims.Business.Util
{
    public class MoneyUtil
    {
        private const string DEFAULT_CULTURE = "en-US";
        public CultureInfo Culture { get; set; }

        public MoneyUtil() => Culture = CultureInfo.CreateSpecificCulture(DEFAULT_CULTURE);
        public MoneyUtil(CultureInfo culture) => Culture = culture;

        public decimal Parse(string input)
        {
            try
            {
                var amount = Decimal.Parse(input, Culture);
                Console.WriteLine($"Converted [{input}] to [{amount}]");
                return amount;
            }
            catch (FormatException e)
            {
                throw new FormatException($"{input} is not a valid decimal: ", e);
            }
        }

    }
}
