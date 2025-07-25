using CarAuctionManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework.Mappings;

internal static class BidMapper
{
    public static void MapBids(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bid>(
            builder =>
            {
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Id).ValueGeneratedOnAdd();
                builder.Property(r => r.Value).IsRequired();
                builder.Property(r => r.Bidder).IsRequired();

                builder
                   .HasOne(x => x.Auction)
                   .WithMany(x => x.Bids)
                   .HasForeignKey(x => x.AuctionId);

                builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                builder.Property(r => r.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                builder.ToTable("bid");
            });
    }
}