namespace CarAuctionManagementSystem.Domain;

public class Auction
{
    public Auction(float startingBid, Vehicle vehicle)
    {
        StartingBid = startingBid;
        Vehicle = vehicle;
        Active = false;
        Bids = new List<Bid>();
        StartDate = null;
        CloseDate = null;
    }

    public String Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime? StartDate { get; private set; }

    public DateTime? CloseDate { get; private set; }

    public float StartingBid { get; }

    public Vehicle Vehicle { get; }

    public ICollection<Bid> Bids { get; }

    public bool Active { get; set; }

    public void Start()
    {
        Active = true;
        StartDate ??= DateTime.Now;
    }

    public void Close()
    {
        Active = false;
        CloseDate ??= DateTime.Now;
    }
}