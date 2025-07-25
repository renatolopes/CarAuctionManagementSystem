namespace CarAuctionManagementSystem.Application.Interfaces;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;
using FluentResults;

public interface IVehiclesService
{
    public Task<Result<AvailableVehicle>> AddAsync(AddVehicleRequest vehicle, CancellationToken cancellationToken);

    public Task<IEnumerable<AvailableVehicle>> SearchAsync(
        VehicleType? vehicleType,
        string? manufacturer,
        string? model,
        int? year
        , CancellationToken cancellationToken);
}