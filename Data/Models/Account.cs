namespace Data.Models;

public partial class Account
{
    public int AccountId { get; set; }
    public int ClientId { get; set; }
    public int StatusId { get; set; }

    public string? AccountNumber { get; set; }
    public decimal? Balance { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? DeletionDate { get; set; }

    public virtual Client Client { get; set; } = null!;
    public virtual AccountStatus Status { get; set; } = null!;

    public CurrencyType Currency { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
