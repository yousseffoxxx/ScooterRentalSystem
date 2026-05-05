namespace ScooterRental.Service.Specifications
{
    public abstract class BaseSpecifications<TEntity> : IBaseSpecifications<TEntity> where TEntity : class
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];
        public List<string> ThenIncludeExpressions { get; } = [];
        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; private set; }

        protected BaseSpecifications()
        {

        }
        protected BaseSpecifications(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpressions)
            => IncludeExpressions.Add(includeExpressions);
        protected void AddThenInclude(string thenIncludeExpressions)
            => ThenIncludeExpressions.Add(thenIncludeExpressions);
        protected void AddOrderBy(Expression<Func<TEntity, object>> orderBy)
            => OrderBy = orderBy;
        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
            => OrderByDescending = orderByDescending;
        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPaginated = true;
            Skip = (pageIndex - 1) * pageSize;
            Take = pageSize;
        }
    }
}
