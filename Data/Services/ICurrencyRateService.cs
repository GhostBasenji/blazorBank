namespace Data.Services;

public interface ICurrencyRateService
{
    Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode);
}