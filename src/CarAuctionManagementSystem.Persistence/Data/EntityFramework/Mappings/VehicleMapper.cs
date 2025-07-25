using CarAuctionManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework.Mappings;

internal static class VehicleMapper
{
    public static void MapVehicles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(
            builder =>
            {
                builder.HasKey(r => r.Id);

                builder.Property(r => r.Id).ValueGeneratedOnAdd();
                builder.Property(r => r.Manufacturer).IsRequired();
                builder.Property(r => r.Model).IsRequired();
                builder.Property(r => r.Year).IsRequired();
                builder.Property(r => r.Type)
                    .IsRequired()
                    .HasConversion<string>();
                builder.Property(r => r.LicensePlate).IsRequired();
                builder.Property(r => r.DoorsNumber);
                builder.Property(r => r.SeatsNumber);
                builder.Property(r => r.LoadCapacity);

                builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                builder.Property(r => r.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                builder.ToTable("vehicle");
            });
        }
}