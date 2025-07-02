namespace CarAuctionManagementSystem.Api.IntegrationTests.Fixtures;

using CarAuctionManagementSystem.Application.DTOs.Auctions;

public static class AddAuctionFixture
{
    public static AddAuctionRequest GetAddAuction(
        float startingBid = 1000,
        string licensePlate = "licensePlate1")
    {
        return new AddAuctionRequest(startingBid, licensePlate);
    }
}