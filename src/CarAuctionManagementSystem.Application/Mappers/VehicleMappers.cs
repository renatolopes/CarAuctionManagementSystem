namespace CarAuctionManagementSystem.Application.Mappers;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;

public static class VehicleMappers
{
    public static Vehicle MapToVehicle(this AddVehicleRequest vehicle)
    {
        return new Vehicle(
            vehicle.Manufacturer,
            vehicle.Model,
            vehicle.Year,
            vehicle.VehicleType,
            vehicle.LicensePlate,
            vehicle.DoorsNumber,
            vehicle.SeatsNumber,
            vehicle.LoadCapacity);
    }

    public static AvailableVehicle MapToAvailableVehicle(this Vehicle vehicle)
    {
        return new AvailableVehicle(
            vehicle.Manufacturer,
            vehicle.Model,
            vehicle.Year,
            vehicle.Type,
            vehicle.LicensePlate);
    }
}