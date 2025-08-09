using Data.DTOs;
using Data.Repositories;

namespace Data.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId)
        {
            return await _accountRepository.GetAccountsByClientIdAsync(clientId);
        }
    }
}
