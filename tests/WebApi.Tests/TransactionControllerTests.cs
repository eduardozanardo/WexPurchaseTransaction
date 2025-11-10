using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;

namespace WebApi.Tests
{
    public class TransactionControllerTests
    {
        [Fact]
        public async Task CreateTransaction_ShouldReturnCreated_WhenTransactionIsValid()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(s => s.CreateTransactionAsync(It.IsAny<Transaction>()))
                       .Returns(Task.CompletedTask);
            var mockLogger = new Mock<ILogger<TransactionController>>();

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            var dto = new TransactionDto
            {
                Description = "Transaction 1",
                TransactionDate = DateTime.UtcNow,
                Value = 123.45m
            };

            // Act
            var result = await controller.CreateTransaction(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var transaction = Assert.IsType<Transaction>(createdResult.Value);
            Assert.Equal(dto.Description, transaction.Description);
            Assert.Equal(dto.Value, transaction.Value);
            mockService.Verify(s => s.CreateTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task GetTransactionConverted_ShouldReturnOk_WhenConversionSucceeds()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transaction = new Transaction("Purchase", DateTime.UtcNow, 100m);

            var convertedDto = new TransactionConversionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                OriginalValueUSD = transaction.Value,
                ExchangeRateUsed = 1.2m,
                ConvertedValue = 120m,
                Currency = "EUR"
            };

            var mockService = new Mock<ITransactionService>();
            mockService
                .Setup(s => s.GetTransactionConvertedAsync(transactionId, "EUR"))
                .ReturnsAsync(convertedDto);
            var mockLogger = new Mock<ILogger<TransactionController>>();

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetTransactionConverted(transactionId, "EUR");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TransactionConversionDto>(okResult.Value);
            Assert.Equal(120m, dto.ConvertedValue);
            Assert.Equal("EUR", dto.Currency);

            mockService.Verify(s => s.GetTransactionConvertedAsync(transactionId, "EUR"), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactions_ShouldReturnOk_WhenTransactionsExist()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            var transactions = new List<Transaction>
            {
                new Transaction("Compra 1", DateTime.UtcNow, 100m),
                new Transaction("Compra 2", DateTime.UtcNow, 200m)
            };

            mockService.Setup(s => s.GetAllTransactionsAsync(1, 10))
                       .ReturnsAsync(transactions);
            var mockLogger = new Mock<ILogger<TransactionController>>();

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllTransactions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTransactions = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);

            Assert.Equal(2, returnedTransactions.Count());
            mockService.Verify(s => s.GetAllTransactionsAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactions_ShouldReturnNotFound_WhenNoTransactionsExist()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(s => s.GetAllTransactionsAsync(1, 10))
                       .ReturnsAsync(new List<Transaction>());
            var mockLogger = new Mock<ILogger<TransactionController>>();

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllTransactions();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No transactions found.", notFoundResult.Value);
            mockService.Verify(s => s.GetAllTransactionsAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetAllTransactions_ShouldReturnPagedResults_WhenPaginationIsApplied()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            var mockLogger = new Mock<ILogger<TransactionController>>();

            var transactions = new List<Transaction>
            {
                new Transaction("Compra 1", DateTime.UtcNow, 100m),
                new Transaction("Compra 2", DateTime.UtcNow, 200m),
                new Transaction("Compra 3", DateTime.UtcNow, 300m)
            };

            // Simula que a primeira página (pageNumber = 1, pageSize = 2) retorna apenas 2 resultados
            mockService.Setup(s => s.GetAllTransactionsAsync(1, 2))
                       .ReturnsAsync(transactions.Take(2));

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllTransactions(pageNumber: 1, pageSize: 2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTransactions = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);

            Assert.Equal(2, returnedTransactions.Count());
            mockService.Verify(s => s.GetAllTransactionsAsync(1, 2), Times.Once);
        }

        [Fact]
        public async Task DeleteTransaction_ShouldReturnOk_WhenDeletionSucceeds()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var mockService = new Mock<ITransactionService>();
            var mockLogger = new Mock<ILogger<TransactionController>>();

            mockService
                .Setup(s => s.DeleteTransactionAsync(transactionId))
                .Returns(Task.CompletedTask);

            var controller = new TransactionController(mockService.Object, mockLogger.Object);

            // Act
            var result = await controller.DeleteTransaction(transactionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Transaction {transactionId} has been deleted.", okResult.Value);
            mockService.Verify(s => s.DeleteTransactionAsync(transactionId), Times.Once);
        }
    }
}
