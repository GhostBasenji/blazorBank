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
            // Генерируем уникальный номер счета
            string accountNumber = await GenerateAccountNumberAsync(clientId);

            var newAccount = new Account
            {
                ClientId = clientId,
                CurrencyId = currencyId,
                StatusId = 1,
                AccountNumber = accountNumber,
                Balance = 0m,
                CreatedAt = DateTime.UtcNow
            };

            // Сохраняем через репозиторий
            await _accountRepository.CreateAccountAsync(newAccount);

            return accountNumber;
        }

        private async Task<string> GenerateAccountNumberAsync(int clientId)
        {
            // Формат: ACC-{ClientId}-{Currency}-{RandomNumber}
            var random = new Random();
            string randomPart = random.Next(1000, 9999).ToString();

            // Генерируем уникальный номер
            string accountNumber;
            int attempts = 0;

            do
            {
                accountNumber = $"ACC-{clientId}-{randomPart}-{DateTime.UtcNow.Ticks % 10000}";
                attempts++;

                if (attempts > 10)
                {
                    // Если не можем сгенерировать уникальный номер за 10 попыток
                    accountNumber = $"ACC-{clientId}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                }
            }
            while (await _accountRepository.AccountNumberExistsAsync(accountNumber) && attempts <= 10);

            return accountNumber;
        }
    }
}