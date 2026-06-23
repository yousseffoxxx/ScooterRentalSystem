namespace ScooterRental.Service.Mappings
{
    public static class TariffMappingExtensions
    {
        // 1. TariffForCreationDto -> Tariff
        public static Tariff ToEntity(this TariffForCreationDto dto)
        {
            return new Tariff
            {
                Name = dto.Name,
                UnlockFee = dto.UnlockFee,
                PerMinuteRate = dto.PerMinuteRate,
            };
        }
        
        // 2. Tariff -> TariffDto
        public static TariffDto ToDto(this Tariff tariff)
        {
            return new TariffDto(

                tariff.Id,
                tariff.Name,
                tariff.UnlockFee,
                tariff.PerMinuteRate,
                tariff.IsActive,
                tariff.CreatedAt
            );
        }

        // 3. List of Tariff -> List of TariffDto
        public static IReadOnlyList<TariffDto> ToDtoList(this IReadOnlyList<Tariff> tariffs)
        {
            if (tariffs == null || tariffs.Count == 0)
                return new List<TariffDto>(0);

            var tariffsDtos = new List<TariffDto>(tariffs.Count);
            foreach (var tariff in tariffs)
            {
                tariffsDtos.Add(new TariffDto(

                tariff.Id,
                tariff.Name,
                tariff.UnlockFee,
                tariff.PerMinuteRate,
                tariff.IsActive,
                tariff.CreatedAt
                ));
            }
            return tariffsDtos;
        }
    }
}
