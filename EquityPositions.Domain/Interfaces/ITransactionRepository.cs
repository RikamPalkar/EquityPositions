using EquityPositions.Domain.Entities;

namespace EquityPositions.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<IEnumerable<Transaction>> GetByTradeIdAsync(int tradeId);
        Task<Transaction?> GetByTradeIdAndVersionAsync(int tradeId, int version);
        Task UpdateAsync(Transaction transaction);
    }
}

