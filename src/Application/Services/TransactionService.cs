using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly ICurrencyConversionService _currencyConversionService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository repository,
            ICurrencyConversionService currencyConversionService,
            ILogger<TransactionService> logger)
        {
            _repository = repository;
            _currencyConversionService = currencyConversionService;
            _logger = logger;
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            _logger.LogInformation("Starting transaction creation: {@Transaction}", transaction);

            await _repository.AddAsync(transaction);

            _logger.LogInformation("Transaction saved successfully with ID {TransactionId}", transaction.Id);
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching transaction by ID: {TransactionId}", id);

            var transaction = await _repository.GetByIdAsync(id);

            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found for ID: {TransactionId}", id);

                throw new InvalidOperationException("Transaction not found.");
            }

            _logger.LogInformation("Transaction retrieved: {@Transaction}", transaction);

            return transaction;
        }

        public async Task<TransactionConversionDto> GetTransactionConvertedAsync(Guid id, string targetCurrency)
        {
            _logger.LogInformation("Starting currency conversion for transaction {TransactionId} to {TargetCurrency}", id, targetCurrency);

            var transaction = await GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                _logger.LogInformation("Transaction is null");

                throw new InvalidOperationException("Transaction not found.");
            }

            try
            {
                var result = await _currencyConversionService.ConvertTransactionAsync(transaction, targetCurrency);
                _logger.LogInformation("Transaction {TransactionId} successfully converted to {TargetCurrency}", id, targetCurrency);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting transaction {TransactionId} to {TargetCurrency}", id, targetCurrency);
                throw;
            }
        }
        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Fetching all transactions. PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var allTransactions = await _repository.GetAllAsync();

            var pagedTransactions = allTransactions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return pagedTransactions;
        }
        public async Task DeleteTransactionAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete transaction with ID: {TransactionId}", id);

            try
            {
                await _repository.DeleteAsync(id);
                _logger.LogInformation("Transaction {TransactionId} deleted successfully", id);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to delete transaction {TransactionId}: not found", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting transaction {TransactionId}", id);
                throw;
            }
        }
    }
}
