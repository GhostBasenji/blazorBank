using Data.Contexts;
using Data.DTOs;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BlazorBankContext _context;

    public AccountRepository(BlazorBankContext context)
    {
        _context = context;
    }

    public async Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Client)
            .Include(a => a.CurrencyNavigation) 
            .Include(a => a.Status)
            .Select(a => new AccountInfoDto
            {
                AccountId = a.AccountId,
                AccountNumber = a.AccountNumber ?? "",
                FullName = $"{a.Client.FirstName} {a.Client.LastName}",
                Currency = a.CurrencyNavigation.CurrencyCode, 
                Status = a.Status.StatusName ?? "",
                Balance = a.Balance ?? 0m,
                CreatedAt = a.CreatedAt,
                ClientId = a.ClientId
            })
            .ToListAsync();
    }

    public async Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm)
    {
        var query = _context.Accounts
            .AsNoTracking()
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Client)
            .Include(a => a.CurrencyNavigation) 
            .Include(a => a.Status)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.AccountNumber!.Contains(searchTerm) ||
                a.CurrencyNavigation.CurrencyCode.Contains(searchTerm) ||
                a.Status.StatusName!.Contains(searchTerm));
        }

        return await query
            .OrderBy(a => a.AccountNumber)
            .Select(a => new AccountInfoDto
            {
                AccountId = a.AccountId,
                AccountNumber = a.AccountNumber ?? "",
                FullName = $"{a.Client.FirstName} {a.Client.LastName}",
                Currency = a.CurrencyNavigation.CurrencyCode, 
                Status = a.Status.StatusName ?? "",
                Balance = a.Balance ?? 0m,
                CreatedAt = a.CreatedAt,
                ClientId = a.ClientId
            })
            .ToListAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        return await _context.Accounts
            .Include(a => a.CurrencyNavigation) 
            .Include(a => a.Client)
            .Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
    }

    public async Task TopUpAccountAsync(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException($"Account with ID {accountId} not found.");

        account.Balance = (account.Balance ?? 0m) + amount;

        var topUpTypeId = await GetTransactionTypeIdAsync("TopUp");

        var transaction = new Transaction
        {
            AccountId = accountId,
            TransactionTypeId = topUpTypeId,
            Amount = amount,
            OriginalAmount = amount,
            OriginalCurrencyId = account.CurrencyId,
            TransactionDate = DateTime.UtcNow,
            Description = "Account top-up"
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task WithdrawAccountAsync(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException($"Account with ID {accountId} not found.");

        var currentBalance = account.Balance ?? 0m;
        if (currentBalance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        account.Balance = currentBalance - amount;

        var withdrawalTypeId = await GetTransactionTypeIdAsync("Withdrawal");

        var transaction = new Transaction
        {
            AccountId = accountId,
            TransactionTypeId = withdrawalTypeId,
            Amount = amount,
            OriginalAmount = amount,
            OriginalCurrencyId = account.CurrencyId,
            TransactionDate = DateTime.UtcNow,
            Description = "Account withdrawal"
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount, string? currencyCode = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var fromAccount = await _context.Accounts
            .Include(a => a.CurrencyNavigation)
            .FirstOrDefaultAsync(a => a.AccountId == fromAccountId);

        var toAccount = await _context.Accounts
            .Include(a => a.CurrencyNavigation)
            .FirstOrDefaultAsync(a => a.AccountId == toAccountId);

        if (fromAccount == null)
            throw new InvalidOperationException($"Source account with ID {fromAccountId} not found.");

        if (toAccount == null)
            throw new InvalidOperationException($"Destination account with ID {toAccountId} not found.");

        // Определяем валюту операции
        Currency operationCurrency;
        if (!string.IsNullOrEmpty(currencyCode))
        {
            // Если указана валюта операции - используем её
            operationCurrency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.CurrencyCode == currencyCode && c.IsActive);

            if (operationCurrency == null)
                throw new InvalidOperationException($"Currency '{currencyCode}' not found.");
        }
        else
        {
            // Если не указана - используем валюту исходного счёта
            operationCurrency = fromAccount.CurrencyNavigation;
        }

        // Конвертируем сумму операции в валюту исходного счёта
        decimal amountInFromCurrency = amount;
        if (operationCurrency.CurrencyCode != fromAccount.CurrencyNavigation.CurrencyCode)
        {
            amountInFromCurrency = await ConvertCurrencyAsync(
                amount,
                operationCurrency.CurrencyId,
                fromAccount.CurrencyId);
        }

        var fromBalance = fromAccount.Balance ?? 0m;
        if (fromBalance < amountInFromCurrency)
            throw new InvalidOperationException("Insufficient funds in source account.");

        // Конвертируем сумму в валюту целевого счёта
        decimal amountInToCurrency = amount;
        if (operationCurrency.CurrencyCode != toAccount.CurrencyNavigation.CurrencyCode)
        {
            amountInToCurrency = await ConvertCurrencyAsync(
                amount,
                operationCurrency.CurrencyId,
                toAccount.CurrencyId);
        }

        var transferOutTypeId = await GetTransactionTypeIdAsync("Transfer Out");
        var transferInTypeId = await GetTransactionTypeIdAsync("Transfer In");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Снимаем деньги с исходного счета
            fromAccount.Balance = fromBalance - amountInFromCurrency;

            // Зачисляем на целевой счет
            toAccount.Balance = (toAccount.Balance ?? 0m) + amountInToCurrency;

            // Транзакция списания
            var withdrawTransaction = new Transaction
            {
                AccountId = fromAccountId,
                TransactionTypeId = transferOutTypeId,
                Amount = amountInFromCurrency,
                OriginalAmount = amount,
                OriginalCurrencyId = operationCurrency.CurrencyId,
                TransactionDate = DateTime.UtcNow,
                Description = $"Transfer to {toAccount.AccountNumber}" +
                             (operationCurrency.CurrencyCode != fromAccount.CurrencyNavigation.CurrencyCode
                                 ? $" ({amount:N2} {operationCurrency.CurrencyCode} = {amountInFromCurrency:N2} {fromAccount.CurrencyNavigation.CurrencyCode})"
                                 : "")
            };

            // Транзакция зачисления
            var depositTransaction = new Transaction
            {
                AccountId = toAccountId,
                TransactionTypeId = transferInTypeId,
                Amount = amountInToCurrency,
                OriginalAmount = amount,
                OriginalCurrencyId = operationCurrency.CurrencyId,
                TransactionDate = DateTime.UtcNow,
                Description = $"Transfer from {fromAccount.AccountNumber}" +
                             (operationCurrency.CurrencyCode != toAccount.CurrencyNavigation.CurrencyCode
                                 ? $" ({amount:N2} {operationCurrency.CurrencyCode} = {amountInToCurrency:N2} {toAccount.CurrencyNavigation.CurrencyCode})"
                                 : "")
            };

            _context.Transactions.Add(withdrawTransaction);
            _context.Transactions.Add(depositTransaction);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Вспомогательный метод для конвертации
    private async Task<decimal> ConvertCurrencyAsync(decimal amount, int fromCurrencyId, int toCurrencyId)
    {
        if (fromCurrencyId == toCurrencyId)
            return amount;

        var exchangeRate = await _context.ExchangeRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.BaseCurrencyId == fromCurrencyId &&
                r.TargetCurrencyId == toCurrencyId);

        if (exchangeRate != null)
            return amount * exchangeRate.Rate;

        var reverseRate = await _context.ExchangeRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.BaseCurrencyId == toCurrencyId &&
                r.TargetCurrencyId == fromCurrencyId);

        if (reverseRate != null)
            return amount / reverseRate.Rate;

        throw new InvalidOperationException($"Exchange rate not found between currencies.");
    }

    private async Task<int> GetTransactionTypeIdAsync(string typeName)
    {
        var transactionType = await _context.TransactionTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TypeName == typeName);

        if (transactionType == null)
        {
            throw new InvalidOperationException(
                $"Transaction type '{typeName}' not found in database. " +
                $"Please run the initialization SQL script.");
        }

        return transactionType.TransactionTypeId;
    }

    public async Task CreateAccountAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AccountNumberExistsAsync(string accountNumber)
    {
        return await _context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<Currency?> GetCurrencyByIdAsync(int currencyId)
    {
        return await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CurrencyId == currencyId);
    }
}