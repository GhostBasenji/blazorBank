namespace Data.Models;

public partial class Currency
{
    public int CurrencyId { get; set; }

    public string CurrencyCode { get; set; } = null!;

    public string CurrencyName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<ExchangeRate> ExchangeRateBaseCurrencies { get; set; } = new List<ExchangeRate>();

    public virtual ICollection<ExchangeRate> ExchangeRateTargetCurrencies { get; set; } = new List<ExchangeRate>();
}
