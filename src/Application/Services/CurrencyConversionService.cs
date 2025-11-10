using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly IExchangeRateService _exchangeRateService;

        public CurrencyConversionService(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<TransactionConversionDto> ConvertTransactionAsync(Transaction transaction, string targetCurrency)
        {
            decimal? rate;

            try
            {
                rate = await _exchangeRateService.GetExchangeRateAsync(targetCurrency, transaction.TransactionDate);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Error retrieving exchange rate from the Treasury.", ex);
            }

            if (!rate.HasValue)
                throw new InvalidOperationException(
                    "No valid exchange rate could be found for the last 6 months.");

            var convertedValue = Math.Round(transaction.Value * rate.Value, 2);

            return new TransactionConversionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                OriginalValueUSD = transaction.Value,
                ExchangeRateUsed = rate.Value,
                ConvertedValue = convertedValue,
                Currency = targetCurrency
            };
        }
    }
}
