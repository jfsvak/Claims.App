using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Business.Util
{
    /// <summary>
    /// Calculator used for calculating GST and amounts without GST.
    /// GST percentage can be set during initialisation.
    /// </summary>
    public class GSTCalculator
    {
        const int PRECISION = 2;
        public readonly decimal GST;

        /// <summary>
        /// GST is default set to 0.15 == 15%
        /// </summary>
        public GSTCalculator(decimal gst = 0.15m) => this.GST = gst;

        /// <summary>
        /// Calculates the amount without GST from <paramref name="amountWithGST"/>
        /// </summary>
        /// <param name="amountWithGST">Amount containing GST</param>
        /// <returns>Amount without GST</returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="amountWithGST"/> is negative</exception>
        public decimal CalculateAmountWithoutGST(decimal amountWithGST)
        {
            Validate(amountWithGST);

            decimal amountWithGSTPrecision = Decimal.Round(amountWithGST, PRECISION);

            return Decimal.Round(Decimal.Divide(amountWithGSTPrecision, Decimal.Add(1m, GST)), PRECISION);
        }

        /// <summary>
        /// Calculates the GST amount contained in the <paramref name="amountWithGST"/>
        /// </summary>
        /// <param name="amountWithGST">amount with GST</param>
        /// <returns>amount of GST contained in <paramref name="amountWithGST"/></returns>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="amountWithGST"/> is negative</exception>
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
