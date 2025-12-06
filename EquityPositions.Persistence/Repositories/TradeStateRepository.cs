using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Interfaces;
using EquityPositions.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EquityPositions.Persistence.Repositories
{
    public class TradeStateRepository : ITradeStateRepository
    {
        private readonly EquityDbContext _context;

        public TradeStateRepository(EquityDbContext context)
        {
            _context = context;
        }

        public async Task<TradeState?> GetByTradeIdAsync(int tradeId)
        {
            return await _context.TradeStates
                .FirstOrDefaultAsync(ts => ts.TradeId == tradeId);
        }

        public async Task<TradeState> AddAsync(TradeState tradeState)
        {
            await _context.TradeStates.AddAsync(tradeState);
            await _context.SaveChangesAsync();
            return tradeState;
        }

        public async Task UpdateAsync(TradeState tradeState)
        {
            _context.TradeStates.Update(tradeState);
            await _context.SaveChangesAsync();
        }
    }
}

