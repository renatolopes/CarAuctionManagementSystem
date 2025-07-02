namespace CarAuctionManagementSystem.Application.DTOs.Vehicles;

using CarAuctionManagementSystem.Domain;

public record AddVehicleRequest(
    string Manufacturer,
    string Model,
    int Year,
    VehicleType VehicleType,
    string LicensePlate,
    int? DoorsNumber = null,
    int? SeatsNumber = null,
    float? LoadCapacity = null
);
