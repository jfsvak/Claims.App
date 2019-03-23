namespace Claims.Business.Models
{
    public class Expense
    {
        public string Id { get; set; }
        public decimal? Total { get; set; }
        public decimal? TotalExclGST { get; set; }
        public string PaymentMethod { get; set; }
        public string CostCentre { get; set; }
    }
}