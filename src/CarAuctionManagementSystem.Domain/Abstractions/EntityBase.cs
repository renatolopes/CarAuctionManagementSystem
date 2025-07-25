namespace CarAuctionManagementSystem.Domain.Abstractions;

public class EntityBase
    {
    public long Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
    }