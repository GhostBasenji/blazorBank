using Data.DTOs;
using Data.Models;
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

        public async Task<string> CreateAccountAsync(int clientId, int currencyId)
        {
            // Генерируем уникальный номер счета с учетом валюты
            string accountNumber = await GenerateAccountNumberAsync(clientId, currencyId); // <- передаем currencyId

            var newAccount = new Account
            {
                ClientId = clientId,
                CurrencyId = currencyId,
                StatusId = 1,
                AccountNumber = accountNumber,
                Balance = 0m,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRepository.CreateAccountAsync(newAccount);

            return accountNumber;
        }

        private async Task<string> GenerateAccountNumberAsync(int clientId, int currencyId)
        {
            // Получаем код валюты
            var currency = await _accountRepository.GetCurrencyByIdAsync(currencyId);
            string currencyCode = currency?.CurrencyCode ?? "XXX";

            var random = new Random();
            string randomPart = random.Next(100000, 999999).ToString();

            // Формат: ACC-{CurrencyCode}-{ClientId}-{RandomNumber}
            string accountNumber;
            int attempts = 0;

            do
            {
                accountNumber = $"ACC-{currencyCode}-{clientId}-{randomPart}";
                attempts++;

                if (attempts > 10)
                {
                    accountNumber = $"ACC-{currencyCode}-{clientId}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
                }

                randomPart = random.Next(100000, 999999).ToString();
            }
            while (await _accountRepository.AccountNumberExistsAsync(accountNumber) && attempts <= 10);

            return accountNumber;
        }
    }
}