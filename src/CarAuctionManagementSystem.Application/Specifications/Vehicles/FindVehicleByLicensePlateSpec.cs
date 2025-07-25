namespace CarAuctionManagementSystem.Application.Specifications.Vehicles;

using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Domain;

public class FindVehicleByLicensePlateSpec(string licensePlate)
    : BaseSpecification<Vehicle>(x => x.LicensePlate == licensePlate);