using CarAuctionManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework.Mappings;

internal static class AuctionMapper
{
    public static void MapAuctions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>(
            builder =>
            {
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Id).ValueGeneratedOnAdd();
                builder.Property(r => r.Code).IsRequired();
                builder.Property(r => r.StartDate);
                builder.Property(r => r.CloseDate);
                builder.Property(r => r.StartingBid);

                builder.Property(r => r.VehicleId).IsRequired();
                builder
                    .HasOne(r => r.Vehicle)
                    .WithMany()
                    .HasForeignKey(r => r.VehicleId);

                builder.Property(r => r.Active).HasDefaultValue(false);
                builder.Property(r => r.GreatestBid);

                builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                builder.Property(r => r.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                builder.ToTable("auction");
            });
    }
}