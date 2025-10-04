using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public class CurrencyService : ICurrencyService
{
    private readonly BlazorBankContext _context;
    private readonly ICurrencyRateService _rateService;

    public CurrencyService(BlazorBankContext context, ICurrencyRateService rateService)
    {
        _context = context;
        _rateService = rateService;
    }

    public async Task<List<Currency>> GetAllAsync()
    {
        return await _context.Currencies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<Currency?> GetByIdAsync(int id)
    {
        return await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyId == id && c.IsActive);
    }

    public async Task<Currency?> GetByCodeAsync(string code)
    {
        return await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyCode == code && c.IsActive);
    }

    public async Task AddAsync(Currency currency)
    {
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var currency = await _context.Currencies.FindAsync(id);
        if (currency != null)
        {
            currency.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode)
    {
        return await _rateService.ConvertAsync(amount, fromCurrencyCode, toCurrencyCode);
    }
}