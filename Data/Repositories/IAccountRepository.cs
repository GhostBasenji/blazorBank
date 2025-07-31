using System.Threading.Tasks;
using Data.DTOs;

namespace Data.Repositories;

public interface IAccountRepository
{
    Task<AccountInfoDto?> GetAccountInfoByClientIdAsync(int clientId);
}
