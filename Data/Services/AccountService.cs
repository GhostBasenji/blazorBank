using System.Threading.Tasks;
using Data.DTOs;
using Data.Repositories;

namespace Data.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;

    public AccountService(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountInfoDto?> GetAccountInfoByClientIdAsync(int clientId)
    {
        return await _repository.GetAccountInfoByClientIdAsync(clientId);
    }
}
