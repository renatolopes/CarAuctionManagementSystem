using CarAuctionManagementSystem.Application.Specifications.Auctions;
using CarAuctionManagementSystem.Domain;
using CarAuctionManagementSystem.Domain.Abstractions;

namespace CarAuctionManagementSystem.Application.Abstractions;

public interface IRepository<T>
    where T : EntityBase
{
    void Add(T entity);

    void AddRange(IEnumerable<T> entities);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false, params string[] includes);

    Task<IEnumerable<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes);

    Task<T> SingleAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes);

    Task<T?> SingleOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes);

    Task<bool> AnyAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        params string[] includes);

    Task<IEnumerable<T>> FindIgnoringQueryFiltersAsync(
       ISpecification<T> specification,
       CancellationToken cancellationToken,
       bool asNoTracking = false,
       params string[] includes);
}