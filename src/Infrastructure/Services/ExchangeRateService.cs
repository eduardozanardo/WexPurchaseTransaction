using Application.Interfaces;

namespace Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ITreasuryExchangeRateClient _treasuryClient;

        public ExchangeRateService(ITreasuryExchangeRateClient treasuryClient)
        {
            _treasuryClient = treasuryClient;
        }

        public async Task<decimal?> GetExchangeRateAsync(string currency, DateTime transactionDate)
        {
            return await _treasuryClient.GetExchangeRateAsync(currency, transactionDate);
        }
    }
}
