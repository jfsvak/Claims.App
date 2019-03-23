using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Claims.Business.Util
{
    public class DateUtil
    {
        private const string DEFAULT_CULTURE = "en-US";
        public CultureInfo Culture { get; set; }

        public DateUtil() => Culture = CultureInfo.CreateSpecificCulture(DEFAULT_CULTURE);
        public DateUtil(CultureInfo culture) => Culture = culture;

        public DateTime? Parse(string input)
        {
            if (input == null)
                return null;

            var dt = DateTime.Now;

            if (DateTime.TryParse(input, Culture, DateTimeStyles.None, out dt))
                return dt;
            else
                return null;
        }
    }
}
