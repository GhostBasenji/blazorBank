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

    public async Task<List<AccountInfoDto>> GetAccountsByClientIdAsync(int clientId)
    {
        return await _context.Accounts
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Client)
            .Include(a => a.Currency)
            .Include(a => a.Status)
            .Select(a => new AccountInfoDto
            {
                AccountNumber = a.AccountNumber,
                FullName = a.Client.FirstName + " " + a.Client.LastName,
                Currency = a.Currency.CurrencyCode,
                Status = a.Status.StatusName,
                Balance = a.Balance ?? 0,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }
}