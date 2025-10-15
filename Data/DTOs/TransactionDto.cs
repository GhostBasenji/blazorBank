namespace Data.DTOs
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Description { get; set; }
    }
}