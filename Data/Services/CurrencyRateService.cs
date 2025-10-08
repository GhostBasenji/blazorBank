using Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public class CurrencyRateService : ICurrencyRateService
{
    private readonly BlazorBankContext _context;

    public CurrencyRateService(BlazorBankContext context)
    {
        _context = context;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode)
    {
        if (string.IsNullOrEmpty(fromCurrencyCode) || string.IsNullOrEmpty(toCurrencyCode))
            throw new ArgumentException("Currency codes cannot be null or empty.");

        if (fromCurrencyCode == toCurrencyCode)
            return amount;

        // Получаем валюты из БД
        var fromCurrency = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyCode == fromCurrencyCode && c.IsActive);

        var toCurrency = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyCode == toCurrencyCode && c.IsActive);

        if (fromCurrency == null)
            throw new InvalidOperationException($"Currency '{fromCurrencyCode}' not found.");

        if (toCurrency == null)
            throw new InvalidOperationException($"Currency '{toCurrencyCode}' not found.");

        // Ищем прямой курс (например, USD -> GEL)
        var exchangeRate = await _context.ExchangeRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.BaseCurrencyId == fromCurrency.CurrencyId &&
                r.TargetCurrencyId == toCurrency.CurrencyId);

        if (exchangeRate != null)
            return amount * exchangeRate.Rate;

        // Ищем обратный курс (например, GEL -> USD, если есть только USD -> GEL)
        var reverseRate = await _context.ExchangeRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.BaseCurrencyId == toCurrency.CurrencyId &&
                r.TargetCurrencyId == fromCurrency.CurrencyId);

        if (reverseRate != null)
            return amount / reverseRate.Rate;

        throw new InvalidOperationException(
            $"Exchange rate not found for {fromCurrencyCode} to {toCurrencyCode}. " +
            $"Please add it in admin panel.");
    }
}