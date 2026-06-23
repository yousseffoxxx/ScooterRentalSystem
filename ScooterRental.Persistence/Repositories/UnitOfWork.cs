namespace ScooterRental.Persistence.Repositories
{
    public class UnitOfWork(ApplicationDbContext _dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = [];
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityName = typeof(TEntity).Name;

            if (repositories.TryGetValue(entityName, out object? value))
                return (IGenericRepository<TEntity>)value;

            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);

                repositories[entityName] = repo;

                return repo;
            }
        }

        public async Task<int> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();
    }
}
