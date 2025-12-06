using EquityPositions.Domain.Entities;

namespace EquityPositions.Domain.Interfaces
{
    public interface IPositionRepository
    {
        Task<Position?> GetBySecurityCodeAsync(string securityCode);
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position> AddOrUpdateAsync(string securityCode, int quantityDelta);
    }
}

