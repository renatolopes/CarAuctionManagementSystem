namespace CarAuctionManagementSystem.Api.Extensions;

using System.Diagnostics.CodeAnalysis;
using CarAuctionManagementSystem.Infrastructure.Data;
using CarAuctionManagementSystem.Infrastructure.Interfaces;

[ExcludeFromCodeCoverage]
public static class AddRepositoryExtension
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(IServiceRepository<>), typeof(ServiceRepository<>));
    }
}