using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly BlazorBankContext _context;

    public ExchangeRateService(BlazorBankContext context)
    {
        _context = context;
    }

    public async Task<List<ExchangeRate>> GetAllAsync()
    {
        return await _context.ExchangeRates
            .Include(r => r.BaseCurrency)
            .Include(r => r.TargetCurrency)
            .ToListAsync();
    }

    public async Task<ExchangeRate?> GetByIdAsync(int id)
    {
        return await _context.ExchangeRates
            .AsNoTracking()
            .Include(r => r.BaseCurrency)
            .Include(r => r.TargetCurrency)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(ExchangeRate rate)
    {
        rate.UpdatedAt = DateTime.UtcNow;
        _context.ExchangeRates.Add(rate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ExchangeRate rate)
    {
        var existing = await _context.ExchangeRates.FirstOrDefaultAsync(r => r.Id == rate.Id);
        if (existing != null)
        {
            existing.BaseCurrencyId = rate.BaseCurrencyId;
            existing.TargetCurrencyId = rate.TargetCurrencyId;
            existing.Rate = rate.Rate;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"ExchangeRate with Id {rate.Id} not found.");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var rate = await _context.ExchangeRates.FirstOrDefaultAsync(r => r.Id == id);
        if (rate != null)
        {
            _context.ExchangeRates.Remove(rate);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException($"ExchangeRate with Id {id} not found.");
        }
    }

    public async Task SetRateAsync(int baseCurrencyId, int targetCurrencyId, decimal rate)
    {
        var existing = await _context.ExchangeRates
            .FirstOrDefaultAsync(r =>
                r.BaseCurrencyId == baseCurrencyId &&
                r.TargetCurrencyId == targetCurrencyId);

        if (existing == null)
        {
            var newRate = new ExchangeRate
            {
                BaseCurrencyId = baseCurrencyId,
                TargetCurrencyId = targetCurrencyId,
                Rate = rate,
                UpdatedAt = DateTime.UtcNow
            };
            _context.ExchangeRates.Add(newRate);
        }
        else
        {
            existing.Rate = rate;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.ExchangeRates.Update(existing);
        }

        await _context.SaveChangesAsync();
    }
}
