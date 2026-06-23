
namespace ScooterRental.Presentation.Controllers
{
    [Authorize]
    public class TariffController(IServiceManager _serviceManager) : ApiController
    {
        [HttpGet("active")]
        public async Task<ActionResult<TariffDto>> GetActiveTariff()
        {
            var tariff = await _serviceManager.TariffService.GetActiveTariffAsync();

            return Ok(tariff);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<TariffDto>>> GetAllTariffs([FromQuery] QueryParams queryParams)
        {
            var result = await _serviceManager.TariffService.GetAllTariffsAsync(queryParams);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TariffDto>> CreateTariff([FromBody] TariffForCreationDto tariffDto)
        {
            var createdTariff = await _serviceManager.TariffService.CreateTariffAsync(tariffDto);
            
            return Ok(createdTariff);
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> ActivateTariff(Guid id)
        {
            await _serviceManager.TariffService.ActivateTariffAsync(id);
            
            return Ok(new MessageResponseDto("Tariff activated successfully. The old tariff has been deactivated."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MessageResponseDto>> DeleteTariff(Guid id)
        {
            await _serviceManager.TariffService.DeleteTariffAsync(id);
            
            return Ok(new MessageResponseDto("Tariff deleted successfully."));
        }
    }
}
