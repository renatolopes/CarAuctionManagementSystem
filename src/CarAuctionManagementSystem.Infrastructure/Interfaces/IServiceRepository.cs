namespace CarAuctionManagementSystem.Infrastructure.Interfaces;

public interface IServiceRepository<T>
{
    T? Find(ISpecification<T> specification);

    IEnumerable<T> FindAll(ISpecification<T> specification);

    IEnumerable<T> FindAll();

    T Add(T entity);

    void DeleteAll();
}