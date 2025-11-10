namespace Infrastructure.External.Models
{
    public class TreasuryRateResponse
    {
        public TreasuryRateRecord[]? data { get; set; }
    }

    public class TreasuryRateRecord
    {
        public string? country_currency_desc { get; set; }
        public string? exchange_rate { get; set; }
        public string? record_date { get; set; }
    }
}
