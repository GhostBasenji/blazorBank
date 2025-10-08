using Data.DTOs;
using Data.Repositories;

namespace Data.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrencyRateService _currencyRateService;

        public AccountService(IAccountRepository accountRepository, ICurrencyRateService currencyRateService)
        {
            _accountRepository = accountRepository;
            _currencyRateService = currencyRateService;
        }

        public async Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId)
        {
            return await _accountRepository.GetAccountsByClientIdAsync(clientId);
        }

        public async Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm)
        {
            return await _accountRepository.SearchAccountsAsync(clientId, searchTerm);
        }

        public async Task TopUpAccountAsync(int accountId, decimal amount, string currencyCode)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {accountId} not found.");

            // Конвертируем из выбранной валюты в валюту счета
            decimal finalAmount = await _currencyRateService.ConvertAsync(
                amount,
                currencyCode,
                account.CurrencyNavigation.CurrencyCode);

            await _accountRepository.TopUpAccountAsync(accountId, finalAmount);
        }

        public async Task WithdrawAccountAsync(int accountId, decimal amount, string currencyCode)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {accountId} not found.");

            // Конвертируем из выбранной валюты в валюту счета
            decimal finalAmount = await _currencyRateService.ConvertAsync(
                amount,
                currencyCode,
                account.CurrencyNavigation.CurrencyCode);

            await _accountRepository.WithdrawAccountAsync(accountId, finalAmount);
        }

        public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            await _accountRepository.TransferAsync(fromAccountId, toAccountId, amount);
        }
    }
}