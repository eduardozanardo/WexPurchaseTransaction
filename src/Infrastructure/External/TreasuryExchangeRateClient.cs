using Application.Interfaces;
using Infrastructure.External.Models;
using System.Text.Json;

namespace Infrastructure.External
{
    public class TreasuryExchangeRateClient : ITreasuryExchangeRateClient
    {
        private readonly HttpClient _httpClient;

        public TreasuryExchangeRateClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal?> GetExchangeRateAsync(string currency, DateTime transactionDate)
        {
            var sixMonthsAgo = transactionDate.AddMonths(-6);

            var url =
                $"https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v1/accounting/od/rates_of_exchange" +
                $"?fields=country_currency_desc,exchange_rate,record_date" +
                $"&filter=country_currency_desc:eq:{currency},record_date:gte:{sixMonthsAgo:yyyy-MM-dd},record_date:lte:{transactionDate:yyyy-MM-dd}";

            var response = await _httpClient.GetStringAsync(url);

            var data = JsonSerializer.Deserialize<TreasuryRateResponse>(response);

            if (data?.data == null || data.data.Length == 0)
                return null;

            var rates = data.data
                .Select(d =>
                {
                    DateTime.TryParse(d.record_date, out var date);
                    decimal.TryParse(d.exchange_rate, out var rate);
                    return new { date, rate };
                })
                .Where(r => r.date <= transactionDate)
                .OrderByDescending(r => r.date)
                .ToList();

            if (rates.Count == 0)
                return null;

            return rates.First().rate;
        }
    }
}
