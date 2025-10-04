using Data.Models;

namespace Data.Services;

public interface ICurrencyService
{
    Task<List<Currency>> GetAllAsync();
    Task<Currency?> GetByIdAsync(int id);
    Task<Currency?> GetByCodeAsync(string code);
    Task AddAsync(Currency currency);
    Task DeleteAsync(int id);
    Task<decimal> ConvertAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode);
}