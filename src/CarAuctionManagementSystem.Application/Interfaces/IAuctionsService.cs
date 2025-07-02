namespace CarAuctionManagementSystem.Application.Interfaces;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using FluentResults;

public interface IAuctionsService
{
    public Result<AvailableAuction> Add(AddAuctionRequest auction);

    public IEnumerable<AvailableAuction> GetAllAuctions();

    public Result Start(string auctionId);

    public Result Close(string auctionId);

    public Result Bid(string auctionId, AddBidRequest bid);
}