using CarAuctionManagementSystem.Application.Abstractions;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework;
public class UnitOfWork : IUnitOfWork
{
    private readonly CarAuctionDBContext _dbContext;

    public UnitOfWork(CarAuctionDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        // After this line runs, all the changes (from the Command Handler and Domain
        // event handlers) performed through the DbContext will be committed
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
