using Data.DTOs;
using Data.Repositories;

namespace Data.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int clientId, int count = 5)
        {
            return await _transactionRepository.GetRecentTransactionsByClientIdAsync(clientId, count);
        }

        public async Task<List<TransactionDto>> GetAllTransactionsAsync(int clientId)
        {
            return await _transactionRepository.GetAllTransactionsByClientIdAsync(clientId);
        }
    }
}