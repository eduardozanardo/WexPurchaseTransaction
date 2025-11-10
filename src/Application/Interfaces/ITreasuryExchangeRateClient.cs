namespace Application.Interfaces
{
    public interface ITreasuryExchangeRateClient
    {
        Task<decimal?> GetExchangeRateAsync(string currency, DateTime transactionDate);
    }
}
