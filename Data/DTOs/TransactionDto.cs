namespace Data.DTOs
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string AccountCurrency { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal OriginalAmount { get; set; }
        public string OriginalCurrency { get; set; } = null!;
        public DateTime? TransactionDate { get; set; }
        public string? Description { get; set; }

        // Метод для форматирования суммы с правильной валютой
        public string FormattedAmount
        {
            get
            {
                var sign = (TransactionType == "TopUp" || TransactionType == "Transfer In") ? "+" : "-";
                return sign + Data.Helpers.CurrencyHelper.FormatAmount(Amount, AccountCurrency);
            }
        }
    }
}