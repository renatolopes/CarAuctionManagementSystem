namespace CarAuctionManagementSystem.Application.Specifications.Vehicles;

using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Abstractions;

public class FindVehicleByLicensePlateSpec : Specification<Vehicle>
{
    public FindVehicleByLicensePlateSpec(string licensePlate)
        : base(
        x => x.LicensePlate == licensePlate)
    {
    }
}