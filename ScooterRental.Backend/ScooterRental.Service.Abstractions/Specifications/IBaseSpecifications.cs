namespace ScooterRental.Service.Abstractions.Specifications
{
    public interface IBaseSpecifications<TEntity> where TEntity : class
    {
        // where(Filtering)
        Expression<Func<TEntity, bool>>? Criteria { get; }

        // include(Joins)
        List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }

        // then include (nested joins(grandchildren))
        List<string> ThenIncludeExpressions { get; }

        // ordering
        Expression<Func<TEntity, object>>? OrderBy { get; }
        Expression<Func<TEntity, object>>? OrderByDescending { get; }

        // Pagination
        int Skip { get; }
        int Take { get; }
        bool IsPaginated { get; }

    }
}
