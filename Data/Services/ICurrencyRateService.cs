using Data.Models;

public interface ICurrencyRateService
{
    Task<decimal> ConvertAsync(decimal amount, CurrencyType from, CurrencyType to);
}

public class CurrencyRateService : ICurrencyRateService
{
    private readonly Dictionary<(CurrencyType, CurrencyType), decimal> rates = new()
    {
        { (CurrencyType.USD, CurrencyType.GEL), 2.7m },
        { (CurrencyType.GEL, CurrencyType.USD), 1/2.7m }
    };

    public Task<decimal> ConvertAsync(decimal amount, CurrencyType from, CurrencyType to)
    {
        if (from == to) return Task.FromResult(amount);
        if (!rates.TryGetValue((from, to), out var rate))
            throw new InvalidOperationException($"Unsupported currency conversion: {from} → {to}");
        return Task.FromResult(amount * rate);
    }
}