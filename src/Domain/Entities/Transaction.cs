namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _description = string.Empty;
        private decimal _value;

        public Transaction(string description, DateTime transactionDate, decimal value)
        {
            this.Description = description;
            this.TransactionDate = transactionDate;
            this.Value = value;
        }

        public Transaction() { }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
                    throw new ArgumentException("Invalid description.");
                _description = value;
            }
        }

        public DateTime TransactionDate { get; set; }

        public decimal Value
        {
            get => _value;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Value must be positive.");
                _value = Math.Round(value, 2);
            }
        }
    }
}
