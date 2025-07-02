namespace CarAuctionManagementSystem.Application.Specifications.Auctions;

using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Infrastructure.Abstractions;

public class FindAuctionByIdSpec : Specification<Auction>
{
    public FindAuctionByIdSpec(string auctionId)
        : base(x => x.Id == auctionId)
    {
    }
}