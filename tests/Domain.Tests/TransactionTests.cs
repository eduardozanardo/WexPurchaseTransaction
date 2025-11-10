using Domain.Entities;

namespace Domain.Tests
{
    public class TransactionTests
    {
        [Fact]
        public void Should_ThrowException_When_DescriptionTooLong()
        {            
            Assert.Throws<ArgumentException>(() => new Transaction(new string('a', 51), DateTime.UtcNow, 143.45m));
        }

        [Fact]
        public void Should_ThrowException_When_ValueNegative()
        {
            Assert.Throws<ArgumentException>(() => new Transaction("Test purchase", DateTime.UtcNow, -143.45m));
        }

        [Fact]
        public void Should_SetPropertiesCorrectly_When_ValidData()
        {
            var transaction = new Transaction("Test purchase", DateTime.UtcNow, 123.45m);

            Assert.Equal("Test purchase", transaction.Description);
            Assert.Equal(123.45m, transaction.Value);
            Assert.True(transaction.TransactionDate <= DateTime.UtcNow);
        }
    }
}
