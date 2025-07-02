namespace CarAuctionManagementSystem.Application.Interfaces;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;
using FluentResults;

public interface IVehiclesService
{
    public Result<AvailableVehicle> Add(AddVehicleRequest vehicle);

    public IEnumerable<AvailableVehicle> Search(
        VehicleType? vehicleType,
        string? manufacturer,
        string? model,
        int? year);
}