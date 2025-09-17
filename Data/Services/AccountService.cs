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

        public async Task TopUpAccountAsync(int accountId, decimal amount)
        {
            await _accountRepository.TopUpAccountAsync(accountId, amount);
        }

        public async Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm)
        {
            return await _accountRepository.SearchAccountsAsync(clientId, searchTerm);
        }

        public async Task WithdrawAccountAsync(int accountId, decimal amount)
        {
            await _accountRepository.WithdrawAccountAsync(accountId, amount);
        }

        public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            await _accountRepository.TransferAsync(fromAccountId, toAccountId, amount);
        }
    }
}
