namespace ScooterRental.Presentation.Controllers
{
    [Authorize]
    public class ZoneController(IServiceManager _serviceManager) : ApiController
    {
        [HttpPost]
        public async Task<ActionResult<ZoneDto>> CreateZone([FromBody] ZoneForCreationDto dto)
        {
            var createdZone = await _serviceManager.ZoneService.CreateZoneAsync(dto);

            return Ok(createdZone);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ZoneDto>>> GetAllZones([FromQuery] ZoneQueryParams queryParams)
        {
            var result = await _serviceManager.ZoneService.GetAllZonesAsync(queryParams);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ZoneDto>> GetZoneById(Guid id)
        {
            var zone = await _serviceManager.ZoneService.GetZoneByIdAsync(id);

            return Ok(zone);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ZoneDto>> UpdateZone(Guid id, [FromBody] ZoneForUpdateDto dto)
        {
            var updatedZone = await _serviceManager.ZoneService.UpdateZoneAsync(dto, id);

            return Ok(updatedZone);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<bool>> DeleteZone(Guid id)
        {
            var result = await _serviceManager.ZoneService.DeleteZoneAsync(id);

            return Ok(result);
        }

        [HttpGet("location")]
        public async Task<ActionResult<IReadOnlyList<ZoneDto>>> GetZonesByLocation([FromQuery] double longitude, [FromQuery] double latitude)
        {
            var zones = await _serviceManager.ZoneService.GetZoneByLocationAsync(longitude, latitude);

            return Ok(zones);
        }
    }
}
