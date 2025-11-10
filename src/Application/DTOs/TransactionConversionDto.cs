namespace Application.DTOs
{
    public class TransactionConversionDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal OriginalValueUSD { get; set; }
        public decimal ExchangeRateUsed { get; set; }
        public decimal ConvertedValue { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
