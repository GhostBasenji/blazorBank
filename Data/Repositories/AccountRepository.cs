using Data.Contexts;
using Data.DTOs;
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
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Client)
            .Include(a => a.Currency)
            .Include(a => a.Status)
            .Select(a => new AccountInfoDto
            {
                AccountId = a.AccountId,
                AccountNumber = a.AccountNumber,
                FullName = a.Client.FirstName + " " + a.Client.LastName,
                Currency = a.Currency.CurrencyCode,
                Status = a.Status.StatusName,
                Balance = a.Balance ?? 0m,
                CreatedAt = a.CreatedAt,
                ClientId = a.ClientId
            })
            .ToListAsync();
    }

    public async Task<List<AccountInfoDto>> SearchAccountsAsync(int clientId, string? searchTerm)
    {
        var query = _context.Accounts
            .Include(a => a.Client)
            .Include(a => a.Currency)
            .Include(a => a.Status)
            .Where(a => a.ClientId == clientId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a => a.AccountNumber.Contains(searchTerm));
        }

        return await query
            .OrderBy(a => a.AccountNumber)
            .Select(a => new AccountInfoDto
            {
                AccountId = a.AccountId,
                AccountNumber = a.AccountNumber,
                FullName = a.Client.FirstName + " " + a.Client.LastName,
                Currency = a.Currency.CurrencyCode,
                Status = a.Status.StatusName,
                Balance = a.Balance ?? 0m,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }

    public async Task TopUpAccountAsync(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new KeyNotFoundException("Account not found.");

        account.Balance = (account.Balance ?? 0m) + amount;
        await _context.SaveChangesAsync();
    }

    public async Task WithdrawAccountAsync(int accountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            throw new InvalidOperationException("Account not found.");

        var currentBalance = account.Balance ?? 0m;
        if (currentBalance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        account.Balance = currentBalance - amount;
        await _context.SaveChangesAsync();
    }

    public async Task TransferAsync(int fromAccountId, int toAccountId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        // Загружаем оба счёта
        var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
        var toAccount = await _context.Accounts.FindAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
            throw new KeyNotFoundException("One or both accounts not found.");

        var fromBalance = fromAccount.Balance ?? 0m;
        if (fromBalance < amount)
            throw new InvalidOperationException("Insufficient funds on source account.");

        // Транзакция на уровне БД
        using var transaction = await _context.Database.BeginTransactionAsync();

        fromAccount.Balance = fromBalance - amount;
        toAccount.Balance = (toAccount.Balance ?? 0m) + amount;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
