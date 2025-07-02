namespace CarAuctionManagementSystem.Domain;

public class Bid
{
    public Bid(float value, string bidder, DateTime bidDateTime)
    {
        Value = value;
        Bidder = bidder;
        BidDateTime = bidDateTime;
    }

    public float Value { get; }

    public string Bidder { get; set; }

    public DateTime BidDateTime { get; set; }
}