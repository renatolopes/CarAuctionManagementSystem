namespace CarAuctionManagementSystem.Application.DTOs.Vehicles;

using CarAuctionManagementSystem.Domain;

public record AvailableVehicle(
    string Manufacturer,
    string Model, 
    int Year,
    VehicleType Type,
    string LicensePlate);