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

        public async Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm)
        {
            return await _accountRepository.SearchAccountsAsync(clientId, searchTerm);
        }

        public async Task TopUpAccountAsync(int accountId, decimal amount, CurrencyType currency)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            decimal finalAmount = ConvertCurrency(amount, currency, (CurrencyType)account.Currency);

            await _accountRepository.TopUpAccountAsync(accountId, finalAmount);
        }

        public async Task WithdrawAccountAsync(int accountId, decimal amount, CurrencyType currency)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            var account = await _accountRepository.GetAccountByIdAsync(accountId);

            decimal finalAmount = ConvertCurrency(amount, currency, (CurrencyType)account.Currency);

            await _accountRepository.WithdrawAccountAsync(accountId, finalAmount);
        }

        public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            await _accountRepository.TransferAsync(fromAccountId, toAccountId, amount);
        }

        public decimal ConvertCurrency(decimal amount, CurrencyType from, CurrencyType to)
        {
            if (from == to) return amount;

            return (from, to) switch
            {
                (CurrencyType.USD, CurrencyType.GEL) => amount * 2.7m,
                (CurrencyType.GEL, CurrencyType.USD) => amount / 2.7m,
                _ => amount
            };
        }
    }
}
