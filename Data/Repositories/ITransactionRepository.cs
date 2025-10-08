using Data.DTOs;

namespace Data.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<TransactionDto>> GetRecentTransactionsByClientIdAsync(int clientId, int count = 5);
        Task<List<TransactionDto>> GetAllTransactionsByClientIdAsync(int clientId);
    }
}