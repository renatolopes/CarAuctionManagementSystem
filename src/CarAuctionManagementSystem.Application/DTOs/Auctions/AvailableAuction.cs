namespace CarAuctionManagementSystem.Application.DTOs.Auctions;

using CarAuctionManagementSystem.Application.DTOs.Vehicles;
using CarAuctionManagementSystem.Domain;

public record AvailableAuction(
    string Id,
    AvailableVehicle Vehicle,
    DateTime? StartDate,
    DateTime? CloseDate,
    float StartingBid,
    IEnumerable<Bid> Bids,
    bool Active);