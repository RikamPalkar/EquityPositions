using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Interfaces;
using EquityPositions.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EquityPositions.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly EquityDbContext _context;

        public TransactionRepository(EquityDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetByIdAsync(int transactionId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByTradeIdAsync(int tradeId)
        {
            return await _context.Transactions
                .Where(t => t.TradeId == tradeId)
                .OrderBy(t => t.Version)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByTradeIdAndVersionAsync(int tradeId, int version)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.TradeId == tradeId && t.Version == version);
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}

