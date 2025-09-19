using Data.DTOs;
using Data.Models;
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

        public async Task TopUpAccountAsync(int accountId, decimal amount, CurrencyType currency)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            decimal finalAmount = amount;

            if (account.Currency != currency)
            {
                finalAmount = ConvertCurrency(amount, currency, account.Currency);
            }

            await _accountRepository.TopUpAccountAsync(accountId, finalAmount);
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

        private decimal ConvertCurrency(decimal amount, CurrencyType from, CurrencyType to)
        {
            if (from == CurrencyType.USD && to == CurrencyType.GEL) return amount * 2.7m;
            if (from == CurrencyType.GEL && to == CurrencyType.USD) return amount / 2.7m;
            return amount;
        }
    }
}
