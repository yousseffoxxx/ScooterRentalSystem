namespace ScooterRental.Service.Mappings
{
    public static class RideMappingExtensions
    {
        // 1. Ride -> ActiveRideResponseDto
        public static ActiveRideResponseDto ToActiveRideDto(this Ride ride)
        {
            decimal currentDurationMinutes = Math.Round((decimal)(DateTimeOffset.UtcNow - ride.StartTime).TotalMinutes, 2);

            decimal currentCost = Math.Round(ride.AppliedUnlockFee + (currentDurationMinutes * ride.AppliedPerMinuteRate), 2);

            return new ActiveRideResponseDto(
                    ride.Id,
                    ride.Scooter.SerialNumber,
                    ride.StartTime,
                    currentDurationMinutes,
                    currentCost
                );
        }

        // 2. Ride -> RideDto
        public static RideDto ToDto(this Ride ride)
        {            
            return new RideDto(
                    ride.Id,
                    ride.Scooter.SerialNumber,
                    ride.User.PhoneNumber ?? "",
                    ride.StartTime,
                    ride.EndTime,
                    ride.DurationMinutes,
                    ride.TotalCost,
                    ride.Status.ToString(),
                    ride.EndPhotoUrl
                );
        }

        // 3. List of Ride -> List of RideDto
        public static IReadOnlyList<RideDto> ToDtoList(this IReadOnlyList<Ride> rides)
        {
            if (rides == null || rides.Count == 0)
                return new List<RideDto>(0);

            var rideDtos = new List<RideDto>(rides.Count);

            foreach (var ride in rides)
            {
                rideDtos.Add(new RideDto(

                    ride.Id,
                    ride.Scooter.SerialNumber,
                    ride.User.PhoneNumber ?? "",
                    ride.StartTime,
                    ride.EndTime,
                    ride.DurationMinutes,
                    ride.TotalCost,
                    ride.Status.ToString(),
                    ride.EndPhotoUrl
                ));
            }
            return rideDtos;
        }

        // 4. List of Ride -> List of PendingParkingPhotoDto
        public static IReadOnlyList<PendingParkingPhotoDto> ToParkingPhotoDtoList(this IReadOnlyList<Ride> rides)
        {
            if (rides == null || rides.Count == 0)
                return new List<PendingParkingPhotoDto>(0);

            var rideDtos = new List<PendingParkingPhotoDto>(rides.Count);

            foreach (var ride in rides)
            {
                rideDtos.Add(new PendingParkingPhotoDto(

                    ride.Id,
                    ride.EndPhotoUrl?? "",
                    ride.Scooter.SerialNumber,
                    ride.User.PhoneNumber?? "",
                    ride.EndTime ?? DateTimeOffset.Now
                ));
            }
            return rideDtos;
        }
    }
}
