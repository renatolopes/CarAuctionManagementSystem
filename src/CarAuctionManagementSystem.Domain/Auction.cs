namespace CarAuctionManagementSystem.Domain;

using CarAuctionManagementSystem.Domain.Abstractions;

public class Auction : EntityBase
{
    public Auction(float startingBid, long vehicleId)
    {
        StartingBid = startingBid;
        VehicleId = vehicleId;
        Active = false;
        Bids = new List<Bid>();
        StartDate = null;
        CloseDate = null;
    }

    public string Code { get; set; } = Guid.NewGuid().ToString();

    public DateTime? StartDate { get; private set; }

    public DateTime? CloseDate { get; private set; }

    public float StartingBid { get; }

    // foreign key
    public long VehicleId { get; set; }

    public Vehicle Vehicle { get; private set; }

    public ICollection<Bid> Bids { get; }

    public bool Active { get; set; } = false;

    public float GreatestBid { get; set; }

    public void Start()
    {
        Active = true;
        StartDate = DateTime.UtcNow;
    }

    public void Close()
    {
        Active = false;
        CloseDate = DateTime.UtcNow;
    }

    //For unit test
    public void SetVehicle(Vehicle vehicle)
    {
        Vehicle = vehicle;
    }
}