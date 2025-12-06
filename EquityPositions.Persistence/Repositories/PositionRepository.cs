using EquityPositions.Domain.Entities;
using EquityPositions.Domain.Interfaces;
using EquityPositions.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EquityPositions.Persistence.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly EquityDbContext _context;

        public PositionRepository(EquityDbContext context)
        {
            _context = context;
        }

        public async Task<Position?> GetBySecurityCodeAsync(string securityCode)
        {
            return await _context.Positions
                .FirstOrDefaultAsync(p => p.SecurityCode == securityCode);
        }

        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            return await _context.Positions
                .OrderBy(p => p.SecurityCode)
                .ToListAsync();
        }

        public async Task<Position> AddOrUpdateAsync(string securityCode, int quantityDelta)
        {
            var position = await _context.Positions
                .FirstOrDefaultAsync(p => p.SecurityCode == securityCode);

            if (position == null)
            {
                position = new Position
                {
                    SecurityCode = securityCode,
                    Quantity = quantityDelta,
                    LastUpdatedAt = DateTime.UtcNow
                };
                await _context.Positions.AddAsync(position);
            }
            else
            {
                position.Quantity += quantityDelta;
                position.LastUpdatedAt = DateTime.UtcNow;
                _context.Positions.Update(position);
            }

            await _context.SaveChangesAsync();
            return position;
        }
    }
}

