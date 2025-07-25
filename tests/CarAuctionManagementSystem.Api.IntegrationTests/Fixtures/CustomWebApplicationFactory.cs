using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace CarAuctionManagementSystem.Api.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _database;

    public CustomWebApplicationFactory(DatabaseFixture database)
    {
        _database = database;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:CarAuctionService", _database.ConnectionString);
        builder.ConfigureTestServices(services =>
        {
            //Console.WriteLine(services);
        });
    }
}