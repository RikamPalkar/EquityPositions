using EquityPositions.Domain.Entities;

namespace EquityPositions.Domain.Interfaces
{
    public interface ITradeStateRepository
    {
        Task<TradeState?> GetByTradeIdAsync(int tradeId);
        Task<TradeState> AddAsync(TradeState tradeState);
        Task UpdateAsync(TradeState tradeState);
    }
}

