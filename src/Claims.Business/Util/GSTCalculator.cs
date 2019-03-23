using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Business.Util
{
    public class GSTCalculator
    {
        const int PRECISION = 2;
        public readonly decimal GST;

        public decimal AmountWithGST { get; }

        /// <summary>
        /// GST is default set to 0.15 == 15%
        /// </summary>
        public GSTCalculator(decimal amountWithGST, decimal gst = 0.15m)
        {
            this.AmountWithGST = Decimal.Round(amountWithGST, 2);
            this.GST = gst;
        }

        /// <summary>
        /// Calculates the amount without GST to the nearest cent amount with 2 decimals
        /// </summary>
        /// <returns></returns>
        public decimal AmountWithoutGST()
        {
            return Decimal.Round(Decimal.Divide(AmountWithGST, Decimal.Add(1m, GST)), PRECISION);
        }

        /// <summary>
        /// Calculates the GST amount by first calculating the amount with GST and then subtracting it from 
        /// </summary>
        /// <returns></returns>
        public decimal GSTAmount()
        {
            return Decimal.Subtract(AmountWithGST, AmountWithoutGST());
        }


    }
}
