namespace ScooterRental.Service.Mappings
{
    public static class WalletMappingExtensions
    {
        public static WalletTransactionDto ToDto(this WalletTransaction transaction)
        {
            return new WalletTransactionDto(
                transaction.Id,
                transaction.Amount,
                transaction.Type.ToString(),
                transaction.ReferenceId,
                transaction.Description,
                transaction.Timestamp
            );
        }

        public static IReadOnlyList<WalletTransactionDto> ToDtoList(this IReadOnlyList<WalletTransaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
                return new List<WalletTransactionDto>(0);

            var dtos = new List<WalletTransactionDto>(transactions.Count);
            foreach (var transaction in transactions)
            {
                dtos.Add(transaction.ToDto());
            }
            return dtos;
        }
    }
}