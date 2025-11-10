namespace Application.DTOs
{
    public class TransactionDto
    {
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Value { get; set; }
    }
}
