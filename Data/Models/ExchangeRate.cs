namespace Data.Models;

public partial class ExchangeRate
{
    public int Id { get; set; }

    public int BaseCurrencyId { get; set; }

    public int TargetCurrencyId { get; set; }

    public decimal Rate { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Currency BaseCurrency { get; set; } = null!;

    public virtual Currency TargetCurrency { get; set; } = null!;
}
