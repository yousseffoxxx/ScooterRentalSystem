namespace ScooterRental.Presentation.Controllers
{
    [Authorize]
    public class RideController(IServiceManager _serviceManager) : ApiController
    {
        [HttpPost("start")]
        public async Task<ActionResult<ActiveRideResponseDto>> StartRide([FromBody] StartRideRequestDto requestDto)
        {
            var result = await _serviceManager.RideService.StartRideAsync(requestDto, GetUserIdFromJwtClaims());

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<ActiveRideResponseDto>> GetActiveRide()
        {
            var result = await _serviceManager.RideService.GetCurrentActiveRideAsync(GetUserIdFromJwtClaims());

            return Ok(result);
        }

        [HttpPost("active/end")]
        public async Task<ActionResult<RideDto>> EndRide([FromForm] EndRideRequestDto requestDto)
        {
            var result = await _serviceManager.RideService.EndRideAsync(requestDto, GetUserIdFromJwtClaims());

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("parking-photos/pending")]
        public async Task<ActionResult<PaginatedResult<PendingParkingPhotoDto>>> GetPendingParkingPhotos([FromQuery] QueryParams queryParams)
        {
            var result = await _serviceManager.RideService.GetPendingParkingPhotosAsync(queryParams);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<RideDto>>> GetAllRides([FromQuery] QueryParams queryParams)
        {
            var result = await _serviceManager.RideService.GetAllRidesAsync(queryParams);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("parking-photos/{rideId}/review")]
        public async Task<ActionResult> ReviewParkingPhoto([FromQuery] Guid rideId, [FromBody] ReviewParkingPhotoDto dto)
        {
            await _serviceManager.RideService.ReviewParkingPhotoAsync(rideId,dto);

            return Ok();
        }

        private Guid GetUserIdFromJwtClaims() 
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
    }
}
