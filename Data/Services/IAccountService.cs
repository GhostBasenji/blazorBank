using System.Threading.Tasks;
using Data.DTOs;

namespace Data.Services;

public interface IAccountService
{
    Task<AccountInfoDto?> GetAccountInfoByClientIdAsync(int clientId);
}
