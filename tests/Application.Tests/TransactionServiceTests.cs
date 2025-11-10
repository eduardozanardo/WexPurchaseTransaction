using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Application.Tests
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task CreateTransactionAsync_Should_Call_AddAsync_Once_When_TransactionIsValid()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);

            var transaction = new Transaction("Test Purchase", DateTime.UtcNow, 150.75m);

            // Act
            await service.CreateTransactionAsync(transaction);

            // Assert
            mockRepo.Verify(
                r => r.AddAsync(It.Is<Transaction>(t =>
                    t.Description == transaction.Description &&
                    t.Value == transaction.Value
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_Should_ThrowException_When_RepositoryThrows()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                    .ThrowsAsync(new Exception("DB error"));

            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);
            var transaction = new Transaction("Test Purchase", DateTime.UtcNow, 200m);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateTransactionAsync(transaction));
        }

        [Fact]
        public async Task GetTransactionByIdAsync_Should_Call_GetByIdAsync_Once()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);

            var transactionId = Guid.NewGuid();

            mockRepo.Setup(r => r.GetByIdAsync(transactionId))
                    .ReturnsAsync(new Transaction("Purchase", DateTime.UtcNow, 100m));

            // Act
            var result = await service.GetTransactionByIdAsync(transactionId);

            // Assert
            Assert.NotNull(result);
            mockRepo.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
        }

        [Fact]
        public async Task GetTransactionConvertedAsync_Should_Call_ConvertTransactionAsync()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);

            var transactionId = Guid.NewGuid();
            var transaction = new Transaction("Purchase", DateTime.UtcNow, 100m);

            mockRepo.Setup(r => r.GetByIdAsync(transactionId))
                    .ReturnsAsync(transaction);

            var convertedDto = new TransactionConversionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                OriginalValueUSD = transaction.Value,
                ExchangeRateUsed = 1.2m,
                ConvertedValue = 120m,
                Currency = "Canada-Dollar"
            };

            mockCurrencyService.Setup(c => c.ConvertTransactionAsync(transaction, "Canada-Dollar"))
                               .ReturnsAsync(convertedDto);

            // Act
            var result = await service.GetTransactionConvertedAsync(transactionId, "Canada-Dollar");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(convertedDto.ConvertedValue, result.ConvertedValue);
            mockRepo.Verify(r => r.GetByIdAsync(transactionId), Times.Once);
            mockCurrencyService.Verify(c => c.ConvertTransactionAsync(transaction, "Canada-Dollar"), Times.Once);
        }

        [Fact]
        public async Task GetTransactionConvertedAsync_Should_Throw_When_TransactionNotFound()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);

            var transactionId = Guid.NewGuid();
            mockRepo.Setup(r => r.GetByIdAsync(transactionId))
                    .ReturnsAsync((Transaction)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetTransactionConvertedAsync(transactionId, "EUR"));
        }

        [Fact]
        public async Task GetTransactionConvertedAsync_Should_Throw_When_ConversionFails()
        {
            // Arrange
            var mockRepo = new Mock<ITransactionRepository>();
            var mockCurrencyService = new Mock<ICurrencyConversionService>();
            var mockLogger = new Mock<ILogger<TransactionService>>();
            var service = new TransactionService(mockRepo.Object, mockCurrencyService.Object, mockLogger.Object);

            var transactionId = Guid.NewGuid();
            var transaction = new Transaction("Purchase", DateTime.UtcNow, 100m);

            mockRepo.Setup(r => r.GetByIdAsync(transactionId))
                    .ReturnsAsync(transaction);

            mockCurrencyService.Setup(c => c.ConvertTransactionAsync(transaction, "EUR"))
                               .ThrowsAsync(new InvalidOperationException("Conversion failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.GetTransactionConvertedAsync(transactionId, "EUR"));
        }
    }
}
