using EquityPositions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquityPositions.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.TransactionId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.TradeId)
                .IsRequired();

            builder.Property(t => t.Version)
                .IsRequired();

            builder.Property(t => t.SecurityCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.Quantity)
                .IsRequired();

            builder.Property(t => t.Action)
                .IsRequired();

            builder.Property(t => t.Side)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.IsProcessed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(t => t.TradeId);
            builder.HasIndex(t => new { t.TradeId, t.Version }).IsUnique();
        }
    }
}

