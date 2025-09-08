using System.Threading.Tasks;
using Data.DTOs;

namespace Data.Repositories;

public interface IAccountRepository
{
    Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId);
    Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm);
}
