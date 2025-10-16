using Data.DTOs;

namespace Data.Services;

public interface IAccountService
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);
    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);
    Task TopUpAccountAsync(int accountId, decimal amount, string currencyCode);
    Task WithdrawAccountAsync(int accountId, decimal amount, string currencyCode);
    Task TransferAsync(int fromAccountId, int toAccountId, decimal amount);
    Task<string> CreateAccountAsync(int clientId, int currencyId);
}