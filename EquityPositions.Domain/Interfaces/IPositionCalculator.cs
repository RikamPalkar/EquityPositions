using EquityPositions.Domain.Entities;

namespace EquityPositions.Domain.Interfaces
{
    public interface IPositionCalculator
    {
        Task<Position> ProcessTransactionAsync(Transaction transaction);
    }
}

