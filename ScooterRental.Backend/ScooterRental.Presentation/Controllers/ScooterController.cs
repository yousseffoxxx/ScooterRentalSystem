namespace ScooterRental.Presentation.Controllers
{
    [Authorize]
    public class ScooterController(IServiceManager _serviceManager) : ApiController
    {
        [HttpGet("{serialNumber}/status")]
        public async Task<ActionResult<ScooterStatusDto>> GetScooterStatus(string serialNumber)
        {
            var scooterStatus = await _serviceManager.ScooterService.GetScooterStatusAsync(serialNumber);

            return Ok(scooterStatus);
        }

        [HttpGet("live-map")]
        public async Task<ActionResult<LiveMapDto>> GetMapData()
        {
            var result = await _serviceManager.ScooterService.GetLiveMapDataAsync();

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<ScooterDto>>> GetAllScooters([FromQuery] QueryParams queryParams)
        {
           var scooters = await _serviceManager.ScooterService.GetAllScootersAsync(queryParams);

            return Ok(scooters);
        }

        [HttpGet("{id:guid}", Name = "GetScooterById")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScooterDto>> GetScooterById(Guid id)
        {
            var scooter = await _serviceManager.ScooterService.GetScooterByIdAsync(id);

            return Ok(scooter);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScooterDto>> CreateScooter([FromBody] ScooterForCreationDto scooterDto)
        {
            var result = await _serviceManager.ScooterService.CreateScooterAsync(scooterDto);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScooterDto>> UpdateScooter(Guid id, [FromBody] ScooterForUpdateDto scooterDto)
        {
            var result = await _serviceManager.ScooterService.UpdateScooterAsync(id, scooterDto);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteScooter(Guid id)
        {
            var result = await _serviceManager.ScooterService.DeleteScooterAsync(id);

            return Ok(result);
        }

        [HttpPost("{id:guid}/unlock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> ForceUnlockScooter(Guid id)
        {
            await _serviceManager.ScooterService.ForceUnlockScooterAsync(id);
            return Ok(new MessageResponseDto("Scooter wheels unlocked successfully."));
        }

        [HttpPost("{id:guid}/lock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> ForceLockScooter(Guid id)
        {
            await _serviceManager.ScooterService.ForceLockScooterAsync(id);
            return Ok(new MessageResponseDto("Scooter wheels locked successfully."));
        }

        [HttpPost("{id:guid}/ping")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> PingScooter(Guid id)
        {
            await _serviceManager.ScooterService.PlayScooterAlarmAsync(id);
            return Ok(new MessageResponseDto("Alarm command sent to scooter."));
        }

        [HttpPost("{id:guid}/maintenance")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> PutScooterInMaintenance(Guid id)
        {
            await _serviceManager.ScooterService.PutScooterInMaintenanceAsync(id);
            
            return Ok(new MessageResponseDto("Scooter moved to Maintenance status."));
        }

        [HttpPost("{id:guid}/retire")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> RetireScooter(Guid id)
        {
            await _serviceManager.ScooterService.RetireScooterAsync(id);
            
            return Ok(new MessageResponseDto("Scooter has been retired (Offline)."));
        }
    }
}
