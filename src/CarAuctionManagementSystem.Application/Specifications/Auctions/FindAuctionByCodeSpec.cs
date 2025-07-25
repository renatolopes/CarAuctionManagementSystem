namespace CarAuctionManagementSystem.Application.Specifications.Auctions;

using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Domain;

public class FindAuctionByCodeSpec(string auctionCode)
    : BaseSpecification<Auction>(x => x.Code == auctionCode);