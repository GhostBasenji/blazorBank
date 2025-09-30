using Data.Models;

namespace Data.Services;

public interface IExchangeRateService
{
    Task<List<ExchangeRate>> GetAllAsync();
    Task<ExchangeRate?> GetByIdAsync(int id);
    Task SetRateAsync(int baseCurrencyId, int targetCurrencyId, decimal rate);
    Task DeleteAsync(int id);
}
