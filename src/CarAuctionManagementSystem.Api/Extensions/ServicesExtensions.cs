using System.Diagnostics.CodeAnalysis;
using CarAuctionManagementSystem.Application.Interfaces;
using CarAuctionManagementSystem.Application.Services;

namespace CarAuctionManagementSystem.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IVehiclesService, VehiclesService>();
        serviceCollection.AddScoped<IAuctionsService, AuctionsService>();
    }
}