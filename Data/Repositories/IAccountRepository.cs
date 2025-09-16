using Data.DTOs;

namespace Data.Repositories;

public interface IAccountRepository
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);
    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);

    Task TopUpAccountAsync(int accountId, decimal amount);
    Task WithdrawAccountAsync(int accountId, decimal amount);

    Task TransferAsync(int fromAccountId, int toAccountId, decimal amount);
}
