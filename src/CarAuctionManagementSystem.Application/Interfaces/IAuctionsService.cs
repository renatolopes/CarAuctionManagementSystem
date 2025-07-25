namespace CarAuctionManagementSystem.Application.Interfaces;

using CarAuctionManagementSystem.Application.DTOs.Auctions;
using CarAuctionManagementSystem.Application.DTOs.Bids;
using FluentResults;

public interface IAuctionsService
{
    public Task<Result<AvailableAuction>> AddAsync(AddAuctionRequest auction, CancellationToken cancellationToken);

    public Task<IEnumerable<AvailableAuction>> GetAllAuctionsAsync(CancellationToken cancellationToken);

    public Task<Result> StartAsync(string auctionId, CancellationToken cancellationToken);

    public Task<Result> CloseAsync(string auctionId, CancellationToken cancellationToken);

    public Task<Result> BidAsync(string auctionId, AddBidRequest bid, CancellationToken cancellationToken);
}