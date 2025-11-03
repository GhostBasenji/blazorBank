using Data.DTOs;
using Data.Models;

namespace Data.Repositories;

public interface IAccountRepository
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);
    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);

    Task TopUpAccountAsync(int accountId, decimal amount);
    Task WithdrawAccountAsync(int accountId, decimal amount);

    Task<Account?> GetAccountByIdAsync(int accountId);
    Task<bool> AccountNumberExistsAsync(string accountNumber);

    Task CreateAccountAsync(Account account);
    Task<Currency?> GetCurrencyByIdAsync(int currencyId);

    Task TransferAsync(int fromAccountId, int toAccountId, decimal amount, string? currencyCode = null);
}