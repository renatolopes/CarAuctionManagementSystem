namespace CarAuctionManagementSystem.Infrastructure.Abstractions;

using System.Linq.Expressions;
using CarAuctionManagementSystem.Infrastructure.Interfaces;

public class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
}