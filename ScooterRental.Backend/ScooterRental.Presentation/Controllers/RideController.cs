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
        public async Task<ActionResult<RideDto>> EndRide([FromBody]EndRideRequestDto requestDto)
        {
            var result = await _serviceManager.RideService.EndRideAsync(requestDto, GetUserIdFromJwtClaims());

            return Ok(result);
        }

        private Guid GetUserIdFromJwtClaims() 
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
    }
}
