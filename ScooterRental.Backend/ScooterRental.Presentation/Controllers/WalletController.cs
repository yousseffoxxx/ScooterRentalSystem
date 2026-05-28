namespace ScooterRental.Presentation.Controllers
{
    public class WalletController(IServiceManager _serviceManager) : ApiController
    {
        [Authorize]
        [HttpPost("top-up")]
        public async Task<ActionResult<TopUpResponseDto>> TopUp(TopUpRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var result = await _serviceManager.PaymobService.InitiateWalletPaymentAsync(dto.Amount, dto.WalletPhoneNumber,userId);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("WebHook")]
        public async Task<IActionResult> WebHook([FromQuery] string hmac, PaymobTransactionObjDto request)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var result = await _serviceManager.PaymobService.ProcessPaymobWebhook(hmac,json);

            if (!result)
                return Unauthorized();

            return Ok();
        }
    }
}
