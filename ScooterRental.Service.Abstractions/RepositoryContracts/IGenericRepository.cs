namespace ScooterRental.Service.Abstractions.RepositoryContracts
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<TEntity>> GetAllWithSpecAsync(IBaseSpecifications<TEntity> specifications);
        Task<TEntity?> GetEntityWithSpecAsync(IBaseSpecifications<TEntity> specifications);
        Task<int> CountAsync(IBaseSpecifications<TEntity> specifications);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
