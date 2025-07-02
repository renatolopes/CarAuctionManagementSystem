namespace CarAuctionManagementSystem.Application.Specifications.Vehicles;

using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Abstractions;

public class FindVehiclesByMultipleParameters : Specification<Vehicle>
{
    public FindVehiclesByMultipleParameters(
        VehicleType? vehicleType,
        string? manufacturer,
        string? model,
        int? year)
        : base(
        x =>
            (vehicleType == null || x.Type == vehicleType) &&
            (manufacturer == null || x.Manufacturer == manufacturer) &&
            (model == null || x.Model == model) &&
            (year == null || x.Year == year))
    {
    }
}