using Data.Contexts;
using Data.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BlazorBankContext _context;

        public TransactionRepository(BlazorBankContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsByClientIdAsync(int clientId, int count = 5)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Include(t => t.Account)
                .ThenInclude(a => a.CurrencyNavigation)
                .Include(t => t.TransactionType)
                .Include(t => t.OriginalCurrency)
                .Where(t => t.Account.ClientId == clientId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    AccountNumber = t.Account.AccountNumber ?? "",
                    AccountCurrency = t.Account.CurrencyNavigation.CurrencyCode,
                    TransactionType = t.TransactionType.TypeName,
                    Amount = t.Amount,
                    OriginalAmount = t.OriginalAmount,
                    OriginalCurrency = t.OriginalCurrency.CurrencyCode,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description
                })
                .ToListAsync();
        }

        public async Task<List<TransactionDto>> GetAllTransactionsByClientIdAsync(int clientId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Include(t => t.Account)
                .ThenInclude(a => a.CurrencyNavigation)
                .Include(t => t.TransactionType)
                .Include(t => t.OriginalCurrency)
                .Where(t => t.Account.ClientId == clientId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    AccountNumber = t.Account.AccountNumber ?? "",
                    AccountCurrency = t.Account.CurrencyNavigation.CurrencyCode,
                    TransactionType = t.TransactionType.TypeName,
                    Amount = t.Amount,
                    OriginalAmount = t.OriginalAmount,
                    OriginalCurrency = t.OriginalCurrency.CurrencyCode,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description
                })
                .ToListAsync();
        }
    }
}