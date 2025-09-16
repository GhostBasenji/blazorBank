namespace Data.DTOs
{
    public class AccountInfoDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal? Balance { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int ClientId { get; set; }
    }
}
