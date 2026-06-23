namespace ScooterRental.Persistence
{
    public static class SpecificationQueryBuilder
    {
        public static IQueryable<TEntity> CreateQuery<TEntity>(IQueryable<TEntity> entryPoint , IBaseSpecifications<TEntity> specifications)
            where TEntity : class
        {
            var query = entryPoint;

            if (specifications.Criteria is not null)
                query = query.Where(specifications.Criteria);
            
            if(specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Count > 0)
                foreach (var expression in specifications.IncludeExpressions)
                {
                    query = query.Include(expression);
                }

            if(specifications.ThenIncludeExpressions is not null && specifications.ThenIncludeExpressions.Count > 0)
                foreach (var expression in specifications.ThenIncludeExpressions)
                {
                    query = query.Include(expression);
                }

            if (specifications.OrderBy is not null)
                query = query.OrderBy(specifications.OrderBy);

            else if (specifications.OrderByDescending is not null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.IsPaginated)
                query = query.Skip(specifications.Skip).Take(specifications.Take);

            return query;
        }
    }
}
