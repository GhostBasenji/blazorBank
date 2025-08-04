using System.Linq;
using System.Threading.Tasks;
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

    public async Task<AccountInfoDto?> GetAccountInfoByClientIdAsync(int clientId)
    {
        // Логирование для диагностики
        Console.WriteLine($"Searching for account with ClientId: {clientId}");

        // Сначала проверим есть ли аккаунты вообще
        var accountsCount = await _context.Accounts.CountAsync();
        Console.WriteLine($"Total accounts in database: {accountsCount}");

        // Проверим есть ли аккаунты для данного клиента
        var clientAccountsCount = await _context.Accounts
            .Where(a => a.ClientId == clientId)
            .CountAsync();
        Console.WriteLine($"Accounts for ClientId {clientId}: {clientAccountsCount}");

        // Основной запрос
        var result = await _context.Accounts
            .Include(a => a.Client)
            .Include(a => a.Currency)
            .Include(a => a.Status)
            .Where(a => a.ClientId == clientId && a.DeletionDate == null)
            .Select(a => new AccountInfoDto
            {
                AccountNumber = a.AccountNumber,
                FullName = a.Client.FirstName + " " + a.Client.LastName,
                Currency = a.Currency.CurrencyCode,
                Status = a.Status.StatusName,
                Balance = a.Balance ?? 0m,
                CreatedAt = a.CreatedAt ?? DateTime.MinValue
            })
            .FirstOrDefaultAsync();
        return result;
    }
}