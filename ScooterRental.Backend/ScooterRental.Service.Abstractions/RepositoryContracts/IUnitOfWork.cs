namespace ScooterRental.Service.Abstractions.RepositoryContracts
{
    /* IAsyncDisposable ensures that the underlying database connection (ApplicationDbContext) 
     * is securely closed and its memory is released asynchronously at the end of the HTTP request.
     * This prevents database connection pool exhaustion and memory leaks without blocking the main thread.*/
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        public Task<int> SaveChangesAsync();

    }
}
