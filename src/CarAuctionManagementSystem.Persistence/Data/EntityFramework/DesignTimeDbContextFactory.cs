using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CarAuctionDBContext>
        {
        public CarAuctionDBContext CreateDbContext(string[] args)
            {
            var optionsBuilder = new DbContextOptionsBuilder<CarAuctionDBContext>();
            optionsBuilder.UseNpgsql(args[0]);

            return new CarAuctionDBContext(optionsBuilder.Options);
            }
        }
