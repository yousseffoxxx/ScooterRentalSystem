namespace ScooterRental.Persistence.Repositories
{
    public class GenericRepository<TEntity>(ApplicationDbContext _dbContext) : IGenericRepository<TEntity> where TEntity : class
    {
        public async Task<IReadOnlyList<TEntity>> GetAllAsync() 
            => await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        
        public async Task<TEntity?> GetByIdAsync(Guid id)
            => await _dbContext.Set<TEntity>().FindAsync(id);


        public async Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(IBaseSpecifications<TEntity> specifications)
            => await SpecificationQueryBuilder.CreateQuery(_dbContext.Set<TEntity>(), specifications).AsNoTracking().ToListAsync();


        public async Task<TEntity?> GetEntityWithSpecAsync(IBaseSpecifications<TEntity> specifications)
            => await SpecificationQueryBuilder.CreateQuery(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();

        public async Task<int> CountAsync(IBaseSpecifications<TEntity> specifications)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (specifications.Criteria is not null)
                query = query.Where(specifications.Criteria);

            return await query.CountAsync();
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }
}
