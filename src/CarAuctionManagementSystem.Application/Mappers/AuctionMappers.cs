namespace CarAuctionManagementSystem.Application.Mappers;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;

public static class AuctionMappers
{
    public static AvailableAuction MapToAvailableAuction(this Auction auction)
    {
        var availableVehicle = new AvailableVehicle(
            auction.Vehicle.Manufacturer,
            auction.Vehicle.Model,
            auction.Vehicle.Year,
            auction.Vehicle.Type,
            auction.Vehicle.LicensePlate);

        return new AvailableAuction(
            auction.Id,
            availableVehicle,
            auction.StartDate,
            auction.CloseDate,
            auction.StartingBid,
            auction.Bids,
            auction.Active);
    }
}