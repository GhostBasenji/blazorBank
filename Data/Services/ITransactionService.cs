using Data.DTOs;

namespace Data.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetRecentTransactionsAsync(int clientId, int count = 5);
        Task<List<TransactionDto>> GetAllTransactionsAsync(int clientId);
    }
}