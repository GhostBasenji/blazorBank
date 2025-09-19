using Data.DTOs;
using Data.Models;

public interface IAccountService
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);

    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);

    Task TopUpAccountAsync(int accountId, decimal amount, CurrencyType currency);

    Task WithdrawAccountAsync(int accountId, decimal amount);

    Task TransferAsync(int fromAccountId, int toAccountId, decimal amount);
}
