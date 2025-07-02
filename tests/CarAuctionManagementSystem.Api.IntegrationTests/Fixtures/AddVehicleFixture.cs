namespace CarAuctionManagementSystem.Api.IntegrationTests.Fixtures;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;

public static class AddVehicleFixture
{
    public static AddVehicleRequest GetAddVehicle(
        string manufacturer = "manufacturer",
        string model = "model",
        int year = 2025,
        VehicleType vehicleType = VehicleType.Sedan,
        string licensePlate = "licensePlate",
        int? doorsNumber = 5,
        int? seatsNumber = null,
        float? loadCapacity = null)
    {
        return new AddVehicleRequest(
            manufacturer,
            model,
            year,
            vehicleType,
            licensePlate,
            doorsNumber,
            seatsNumber,
            loadCapacity);
    }
}