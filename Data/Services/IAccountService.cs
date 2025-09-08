using Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAccountService
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);

    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);
}
