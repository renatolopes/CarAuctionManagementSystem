namespace CarAuctionManagementSystem.Infrastructure.Data;

using CarAuctionManagementSystem.Infrastructure.Interfaces;

public class ServiceRepository<T> : IServiceRepository<T>
    where T : class
{
    private readonly List<T> _dataSet = [];

    public T? Find(ISpecification<T> specification)
    {
        var query = _dataSet.AsQueryable().Where(specification.Criteria);

        return query.FirstOrDefault();
    }

    public IEnumerable<T> FindAll(ISpecification<T> specification)
    {
        var query = _dataSet.AsQueryable().Where(specification.Criteria);

        return query.ToList();
    }

    public IEnumerable<T> FindAll()
    {
        var query = _dataSet.AsQueryable();

        return query.ToList();
    }

    public T Add(T entity)
    {
        _dataSet.Add(entity);

        return entity;
    }

    public void DeleteAll()
    {
        _dataSet.Clear();
    }
}