using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Business.Util
{
    public class GSTCalculator
    {
        const int PRECISION = 2;
        public readonly decimal GST;

        /// <summary>
        /// GST is default set to 0.15 == 15%
        /// </summary>
        public GSTCalculator(decimal gst = 0.15m)
        {
            this.GST = gst;
        }

        /// <summary>
        /// Calculates the amount without GST to the nearest cent amount with 2 decimals
        /// </summary>
        /// <returns></returns>
        public decimal CalculateAmountWithoutGST(decimal amountWithGST)
        {
            Validate(amountWithGST);

            decimal amountWithGSTPrecision = Decimal.Round(amountWithGST, PRECISION);

            return Decimal.Round(Decimal.Divide(amountWithGSTPrecision, Decimal.Add(1m, GST)), PRECISION);
        }

        /// <summary>
        /// Calculates the GST amount contained in the amountWithGST
        /// </summary>
        /// <returns></returns>
        public decimal CalculateGSTAmount(decimal amountWithGST)
        {
            Validate(amountWithGST);

            decimal amountWithGSTPrecision = Decimal.Round(amountWithGST, PRECISION);

            return Decimal.Subtract(amountWithGSTPrecision, CalculateAmountWithoutGST(amountWithGST));
        }

        private void Validate(decimal amount)
        {
            if (Decimal.Compare(amount, decimal.Zero) < 0)
                throw new ArgumentException("Amount cannot be negative");
        }
    }
}
