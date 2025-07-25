using Microsoft.EntityFrameworkCore;
using DotNet.Testcontainers.Builders;
using Npgsql;
using Testcontainers.PostgreSql;
using CarAuctionManagementSystem.Persistence.Data.EntityFramework;
using Xunit;

namespace CarAuctionManagementSystem.Api.IntegrationTests.Fixtures;
public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer? container;

    public string ConnectionString
    {
        get
        {
            ArgumentNullException.ThrowIfNull(container);
            return container.GetConnectionString();
        }
    }

    public async Task InitializeAsync()
    {
        var testcontainersBuilder = new PostgreSqlBuilder()
            .WithPortBinding(5433, 5433)
            .WithExposedPort(5433)
            .WithDatabase("car-auction-db-tests")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilCommandIsCompleted("pg_isready -t 5 -d car-auction-db-tests"));

        container = testcontainersBuilder.Build();
        await container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (container is not null)
        {
            await container.DisposeAsync();
        }
    }

    public CarAuctionDBContext CreateDbContext()
    {
        ArgumentNullException.ThrowIfNull(container);

        return new CarAuctionDBContext(
            new DbContextOptionsBuilder<CarAuctionDBContext>()
                   .UseNpgsql(container.GetConnectionString())
                   .Options);
    }
    public async Task<int> DeleteAllRegisters()
    {
        using var dbContext = CreateDbContext();

        await dbContext.Database.ExecuteSqlRawAsync(Utilities.Queries.DeleteAllRegisters);

        return await dbContext.SaveChangesAsync();
    }
}