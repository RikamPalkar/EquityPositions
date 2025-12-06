using EquityPositions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquityPositions.Persistence.Configurations
{
    public class TradeStateConfiguration : IEntityTypeConfiguration<TradeState>
    {
        public void Configure(EntityTypeBuilder<TradeState> builder)
        {
            builder.ToTable("TradeStates");

            builder.HasKey(ts => ts.Id);

            builder.Property(ts => ts.Id)
                .ValueGeneratedOnAdd();

            builder.Property(ts => ts.TradeId)
                .IsRequired();

            builder.Property(ts => ts.CurrentVersion)
                .IsRequired();

            builder.Property(ts => ts.SecurityCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(ts => ts.Quantity)
                .IsRequired();

            builder.Property(ts => ts.Side)
                .IsRequired();

            builder.Property(ts => ts.IsCancelled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ts => ts.LastUpdatedAt)
                .IsRequired();

            builder.HasIndex(ts => ts.TradeId).IsUnique();
        }
    }
}

