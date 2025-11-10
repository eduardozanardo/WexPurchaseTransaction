using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService service, ILogger<TransactionController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new transaction and saves it in the database.
        /// </summary>
        /// <param name="dto">Transaction data containing description, date, and value.</param>
        /// <returns>Returns the created transaction.</returns>
        /// <response code="201">Transaction successfully created.</response>
        /// <response code="400">Invalid transaction data provided.</response>
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("CreateTransaction called with null DTO");

                return BadRequest("The transaction DTO cannot be null.");
            }

            _logger.LogInformation("Creating transaction: {Description}", dto.Description);

            var transaction = new Transaction
            {
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                Value = dto.Value
            };

            try
            {
                await _service.CreateTransactionAsync(transaction);
                _logger.LogInformation("Transaction created successfully with ID {TransactionId}", transaction.Id);
                return CreatedAtAction(nameof(CreateTransaction), transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction {Description}", dto.Description);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Converts a transaction value to the specified target currency.
        /// </summary>
        /// <param name="id">Transaction ID to be converted.</param>
        /// <param name="currency">Target currency code (e.g. USD, EUR, BRL).</param>
        /// <returns>Returns the converted transaction with exchange rate details.</returns>
        /// <response code="200">Conversion successful.</response>
        /// <response code="400">Invalid request or conversion failed.</response>
        /// <response code="404">Transaction not found.</response>
        [HttpGet("{id}/convert")]
        public async Task<IActionResult> GetTransactionConverted(Guid id, [FromQuery] string currency)
        {
            _logger.LogInformation("Starting {Action} for ID {TransactionId} to {Currency}", nameof(GetTransactionConverted), id, currency);

            try
            {
                var result = await _service.GetTransactionConvertedAsync(id, currency);
                _logger.LogInformation("Transaction {TransactionId} converted successfully to {Currency}", id, currency);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Transaction {TransactionId} not found for conversion", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting transaction {TransactionId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all registered transactions.
        /// </summary>
        /// <returns>List of all transactions.</returns>
        /// <response code="200">Returns the list of transactions.</response>
        /// <response code="404">No transactions found.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Retrieving transactions - Page {PageNumber}, Size {PageSize}", pageNumber, pageSize);

            try
            {
                var transactions = await _service.GetAllTransactionsAsync(pageNumber, pageSize);

                if (!transactions.Any())
                {
                    _logger.LogWarning("No transactions found (Page {PageNumber})", pageNumber);
                    return NotFound("No transactions found.");
                }

                _logger.LogInformation("Retrieved {Count} transactions for Page {PageNumber}", transactions.Count(), pageNumber);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a transaction by its ID.
        /// </summary>
        /// <param name="id">Transaction ID (Guid)</param>
        /// <returns>200 OK if deleted successfully, 404 if not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            _logger.LogInformation("Attempting to delete transaction {TransactionId}", id);

            try
            {
                await _service.DeleteTransactionAsync(id);
                _logger.LogInformation("Transaction {TransactionId} deleted successfully", id);
                return Ok($"Transaction {id} has been deleted.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Transaction {TransactionId} not found for deletion", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting transaction {TransactionId}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
