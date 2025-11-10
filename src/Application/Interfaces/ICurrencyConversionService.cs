using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICurrencyConversionService
    {
        Task<TransactionConversionDto> ConvertTransactionAsync(Transaction transaction, string targetCurrency);
    }
}
