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
            TransactionDate = DateTime.UtcNow,
            Description = "Account withdrawal"
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
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

        var fromBalance = fromAccount.Balance ?? 0m;
        if (fromBalance < amount)
            throw new InvalidOperationException("Insufficient funds in source account.");

        if (fromAccount.CurrencyNavigation.CurrencyCode != toAccount.CurrencyNavigation.CurrencyCode)
        {
            throw new InvalidOperationException(
                "Transfer between accounts with different currencies is not supported. " +
                "Please use Top Up or Withdrawal with currency conversion.");
        }

        var transferOutTypeId = await GetTransactionTypeIdAsync("Transfer Out");
        var transferInTypeId = await GetTransactionTypeIdAsync("Transfer In");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        fromAccount.Balance = fromBalance - amount;
        toAccount.Balance = (toAccount.Balance ?? 0m) + amount;

        var withdrawTransaction = new Transaction
        {
            AccountId = fromAccountId,
            TransactionTypeId = transferOutTypeId,
            Amount = amount,
            TransactionDate = DateTime.UtcNow,
            Description = $"Transfer to account {toAccount.AccountNumber}"
        };

        var depositTransaction = new Transaction
        {
            AccountId = toAccountId,
            TransactionTypeId = transferInTypeId,
            Amount = amount,
            TransactionDate = DateTime.UtcNow,
            Description = $"Transfer from account {fromAccount.AccountNumber}"
        };

        _context.Transactions.Add(withdrawTransaction);
        _context.Transactions.Add(depositTransaction);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
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
}