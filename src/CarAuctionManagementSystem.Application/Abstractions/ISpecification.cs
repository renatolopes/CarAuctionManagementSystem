
using System.Linq.Expressions;

namespace CarAuctionManagementSystem.Application.Abstractions;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }

    List<(Expression<Func<T, object>> Expression, OrderByType Type)> OrderByExpressions { get; }

    int Skip { get; }

    int Take { get; }

    bool IsPagingEnabled { get; }
}
