namespace CarAuctionManagementSystem.Application.Specifications.Vehicles;

using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Domain;

public class FindVehiclesByMultipleParameters(
        VehicleType? vehicleType,
        string? manufacturer,
        string? model,
        int? year) : BaseSpecification<Vehicle>(
        x =>
            (vehicleType == null || x.Type == vehicleType) &&
            (manufacturer == null || x.Manufacturer == manufacturer) &&
            (model == null || x.Model == model) &&
            (year == null || x.Year == year));