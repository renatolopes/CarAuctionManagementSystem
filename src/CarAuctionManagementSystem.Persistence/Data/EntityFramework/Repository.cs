
using CarAuctionManagementSystem.Application.Abstractions;
using CarAuctionManagementSystem.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionManagementSystem.Persistence.Data.EntityFramework;

public class Repository<T> : IRepository<T>
    where T : EntityBase
{
    private readonly DbSet<T> _dbSet;

    public Repository(CarAuctionDBContext dbContext)
    {
        _dbSet = dbContext.Set<T>();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false, params string[] includes)
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes)
    {
        var query = BuildBaseQuery(specification, asNoTracking, includes);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T> SingleAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes)
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(specification.Criteria);

        return await query.SingleAsync(cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params string[] includes)
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(specification.Criteria);

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken,
        params string[] includes)
    {
        var query = _dbSet.AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(specification.Criteria);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> FindIgnoringQueryFiltersAsync(
      ISpecification<T> specification,
      CancellationToken cancellationToken,
      bool asNoTracking = false,
      params string[] includes)
    {
        var query = BuildBaseQuery(specification, asNoTracking, includes);

        return await query.IgnoreQueryFilters().ToListAsync(cancellationToken);
    }

    private IQueryable<T> BuildBaseQuery(
        ISpecification<T> specification,
        bool asNoTracking = false,
        params string[] includes)
    {
        var query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet;

        query = includes.Aggregate(query, (current, include) => current.Include(include));

        query = query.Where(specification.Criteria);

        if (specification.OrderByExpressions.Count != 0)
        {
            query = ApplyOrderBy(specification, query);
        }

        return query;
    }

    private static IQueryable<T> ApplyOrderBy(ISpecification<T> specification, IQueryable<T> query)
    {
        IOrderedQueryable<T>? orderedQuery = null;
        for (int i = 0; i < specification.OrderByExpressions.Count; i++)
        {
            var orderBy = specification.OrderByExpressions[i];
            if (i == 0)
            {
                orderedQuery = orderBy.Type == OrderByType.Ascending
                    ? query.OrderBy(orderBy.Expression)
                    : query.OrderByDescending(orderBy.Expression);
            }
            else
            {
                orderedQuery = orderBy.Type == OrderByType.Ascending
                    ? orderedQuery!.ThenBy(orderBy.Expression)
                    : orderedQuery!.ThenByDescending(orderBy.Expression);
            }
        }

        query = orderedQuery ?? query;
        return query;
    }
}
