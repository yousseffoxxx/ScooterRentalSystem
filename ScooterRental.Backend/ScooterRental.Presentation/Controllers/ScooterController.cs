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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<ScooterDto>>> GetAllScooters([FromQuery] ScooterQueryParams queryParams)
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
    }
}
