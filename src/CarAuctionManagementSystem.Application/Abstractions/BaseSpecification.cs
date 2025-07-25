using System.Linq.Expressions;

namespace CarAuctionManagementSystem.Application.Abstractions;

public class BaseSpecification<T> : ISpecification<T>
{
    public int Skip { get; private set; }

    public int Take { get; private set; }

    public bool IsPagingEnabled { get; private set; }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
        OrderByExpressions = new List<(Expression<Func<T, object>> Expression, OrderByType Type)>();
    }

    public Expression<Func<T, bool>> Criteria { get; }

    public List<(Expression<Func<T, object>> Expression, OrderByType Type)> OrderByExpressions { get; }

    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    protected void WithOrderBy(Expression<Func<T, object>> orderByExpression, OrderByType orderByType = OrderByType.Ascending)
    {
        OrderByExpressions.Clear();
        OrderByExpressions.Add((orderByExpression, orderByType));
    }

    protected void WithThenBy(Expression<Func<T, object>> thenByExpression, OrderByType orderByType = OrderByType.Ascending)
    {
        OrderByExpressions.Add((thenByExpression, orderByType));
    }

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}

