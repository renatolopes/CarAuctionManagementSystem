namespace CarAuctionManagementSystem.Api.Extensions;

using System.Diagnostics.CodeAnalysis;
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Persistence.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public static class PersistenceExtensions
    {
    public static void AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
        services.AddDbContext<CarAuctionDBContext>(o =>
            o.UseNpgsql(
                configuration.GetConnectionString("CarAuctionService"),
                options => options.EnableRetryOnFailure()));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }

    public static void ApplyMigrations(this IApplicationBuilder app, IConfiguration configuration)
        {
        if (configuration.GetValue<bool>("EnableAutomaticMigrations"))
            {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CarAuctionDBContext>();
            dbContext.Database.Migrate();
            }
        }
    }