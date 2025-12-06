using EquityPositions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquityPositions.Persistence.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("Positions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.SecurityCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.Property(p => p.LastUpdatedAt)
                .IsRequired();

            builder.HasIndex(p => p.SecurityCode).IsUnique();
        }
    }
}

