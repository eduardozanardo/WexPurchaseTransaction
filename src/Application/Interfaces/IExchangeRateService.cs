namespace Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<decimal?> GetExchangeRateAsync(string currency, DateTime transactionDate);
    }
}
