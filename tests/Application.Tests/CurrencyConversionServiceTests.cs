using Moq;
using Domain.Entities;
using Application.Services;
using Application.Interfaces;

namespace Application.Tests
{
    public class CurrencyConversionServiceTests
    {
        [Fact]
        public async Task ConvertTransactionAsync_Should_Return_ConvertedValue()
        {
            var transaction = new Transaction("Compra Teste", DateTime.UtcNow, 100m);

            var mockExchange = new Mock<IExchangeRateService>();
            mockExchange.Setup(s => s.GetExchangeRateAsync("EUR", transaction.TransactionDate))
                        .ReturnsAsync(0.9m);

            var service = new CurrencyConversionService(mockExchange.Object);

            var result = await service.ConvertTransactionAsync(transaction, "EUR");

            Assert.Equal(90m, result.ConvertedValue);
            Assert.Equal("EUR", result.Currency);
            Assert.Equal(0.9m, result.ExchangeRateUsed);
        }

        [Fact]
        public async Task ConvertTransactionAsync_Should_Throw_When_RateIsNull()
        {
            var transaction = new Transaction("Compra Teste", DateTime.UtcNow, 100m);

            var mockExchange = new Mock<IExchangeRateService>();
            mockExchange.Setup(s => s.GetExchangeRateAsync("EUR", transaction.TransactionDate))
                        .ReturnsAsync((decimal?)null);

            var service = new CurrencyConversionService(mockExchange.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ConvertTransactionAsync(transaction, "EUR"));
        }
    }
}
