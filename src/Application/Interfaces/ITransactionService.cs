using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(Transaction transaction);
        Task<Transaction> GetTransactionByIdAsync(Guid id);
        Task<TransactionConversionDto> GetTransactionConvertedAsync(Guid id, string targetCurrency);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync(int pageNumber, int pageSize);
        Task DeleteTransactionAsync(Guid id);

    }
}
