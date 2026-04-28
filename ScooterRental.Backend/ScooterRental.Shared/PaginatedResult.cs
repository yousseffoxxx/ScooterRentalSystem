namespace ScooterRental.Shared
{
    public record PaginatedResult<TEntity>(int PageIndex, int PageSize, int TotalCount, IReadOnlyList<TEntity> Data)
    {

    }
}
