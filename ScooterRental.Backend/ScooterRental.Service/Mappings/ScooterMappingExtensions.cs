namespace ScooterRental.Service.Mappings
{
    public static class ScooterMappingExtensions
    {
        // 1. Scooter -> ScooterDto
        public static ScooterDto ToDto(this Scooter scooter)
        {
            return new ScooterDto(
                    scooter.Id,
                    scooter.SerialNumber,
                    scooter.CurrentBatteryLevel,
                    scooter.Status.ToString(),
                    scooter.Model?.ModelName ?? "Unknown Model"
                );
        }

        // 2. ScooterForCreationDto -> Scooter
        public static Scooter ToEntity(this ScooterForCreationDto dto)
        {
            return new Scooter
            {
                SerialNumber = dto.SerialNumber,
                CurrentBatteryLevel = 100,
                Status = ScooterStatus.Offline,
                ModelId = dto.ModelId
            };
        }
        
        // 3. ScooterForUpdateDto -> Scooter
        public static void UpdateEntity(this ScooterForUpdateDto dto, Scooter scooter)
        {
            if (!string.IsNullOrWhiteSpace(dto.Status))
                scooter.Status = Enum.Parse<ScooterStatus>(dto.Status, true);
        }

        // 4. List of Scooter -> List of ScooterDto
        public static IReadOnlyList<ScooterDto> ToDtoList(this IReadOnlyList<Scooter> scooters)
        {
            if (scooters == null || scooters.Count == 0)
                return new List<ScooterDto>(0);

            var scooterDtos = new List<ScooterDto>(scooters.Count);

            foreach (var scooter in scooters)
            {
                scooterDtos.Add(new ScooterDto(
                
                    scooter.Id,
                    scooter.SerialNumber,
                    scooter.CurrentBatteryLevel,
                    scooter.Status.ToString(),
                    scooter.Model?.ModelName ?? "Unknown Model"
                ));
            }
            return scooterDtos;
        }

        // 5. Scooter -> ScooterStatusDto
        public static ScooterStatusDto ToStatusDto(this Scooter scooter)
        {
            return new ScooterStatusDto(
                    scooter.SerialNumber,
                    scooter.CurrentBatteryLevel,
                    scooter.Status.ToString()
                );
        }
    }
}
