namespace CarAuctionManagementSystem.Application.Specifications.Auctions;

using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Abstractions;

public class FindAuctionByVehicleLicensePlate : Specification<Auction>
{
    public FindAuctionByVehicleLicensePlate(string licensePlate)
        : base(
        x => x.Vehicle.LicensePlate == licensePlate)
    {
    }
}