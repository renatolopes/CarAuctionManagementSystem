using CarAuctionManagementSystem.Domain.Abstractions;

namespace CarAuctionManagementSystem.Domain;

public class Vehicle : EntityBase
    {
    public Vehicle(
        string manufacturer,
        string model,
        int year,
        VehicleType type,
        string licensePlate,
        int? doorsNumber = null,
        int? seatsNumber = null,
        float? loadCapacity = null)
    {
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        Type = type;
        LicensePlate = licensePlate;
        DoorsNumber = doorsNumber;
        SeatsNumber = seatsNumber;
        LoadCapacity = loadCapacity;
    }

    public int? DoorsNumber { get; }

    public int? SeatsNumber { get; }

    public float? LoadCapacity { get; }

    public string Manufacturer { get; }

    public string Model { get; }

    public int Year { get; }

    public VehicleType Type { get; }

    public string LicensePlate { get; }
}
