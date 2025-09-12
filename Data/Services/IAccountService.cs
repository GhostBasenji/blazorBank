using Data.DTOs;

public interface IAccountService
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);

    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);

    Task TopUpAccountAsync(int accountId, decimal amount);

    Task WithdrawAccountAsync(int accountId, decimal amount);
}
