namespace ScooterRental.Presentation.Controllers
{
    public class WalletController(IServiceManager _serviceManager) : ApiController
    {
        [Authorize]
        [HttpPost("top-up")]
        public async Task<ActionResult<TopUpResponseDto>> TopUp(TopUpRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var result = await _serviceManager.PaymobService.InitiateWalletPaymentAsync(dto.Amount, dto.WalletPhoneNumber);

            return Ok(result);
        }
    }
}
