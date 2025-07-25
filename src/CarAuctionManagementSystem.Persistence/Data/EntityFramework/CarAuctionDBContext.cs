namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework;

using CarAuctionManagementSystem.Persistence.Data.EntityFramework.Mappings;
using Microsoft.EntityFrameworkCore;

public class CarAuctionDBContext : DbContext
{
    public CarAuctionDBContext(DbContextOptions<CarAuctionDBContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.MapVehicles();
        modelBuilder.MapAuctions();
        modelBuilder.MapBids();
    }
}
