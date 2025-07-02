namespace CarAuctionManagementSystem.Api.IntegrationTests.Fixtures;

using CarAuctionManagementSystem.Application.DTOs.Bids;

public static class AddBidFixture
{
    public static AddBidRequest GetAddBid(string auctionId)
    {
        return new AddBidRequest(2000, "bidderName");
    }
}