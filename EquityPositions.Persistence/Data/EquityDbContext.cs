using EquityPositions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EquityPositions.Persistence.Data
{
    public class EquityDbContext : DbContext
    {
        public EquityDbContext(DbContextOptions<EquityDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<TradeState> TradeStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EquityDbContext).Assembly);
        }
    }
}

