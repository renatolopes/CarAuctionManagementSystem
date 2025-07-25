using CarAuctionManagementSystem.Domain.Abstractions;

namespace CarAuctionManagementSystem.Domain;

public class Bid : EntityBase
{
    public Bid(float value, string bidder, DateTime bidDateTime, long auctionId)
    {
        Value = value;
        Bidder = bidder;
        BidDateTime = bidDateTime;
        AuctionId = auctionId;
    }

    public float Value { get; }

    public string Bidder { get; set; }

    public DateTime BidDateTime { get; set; }

    public long AuctionId { get; set; }

    public Auction Auction { get; set; }
}