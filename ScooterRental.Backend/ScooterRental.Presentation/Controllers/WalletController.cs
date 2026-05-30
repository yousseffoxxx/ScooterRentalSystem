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

        [Authorize(Roles = "Admin")]
        [HttpPost("adjust")]
        public async Task<ActionResult<MessageResponseDto>> Refund([FromBody] AdminWalletAdjustmentDto dto)
        {
            await _serviceManager.PaymobService.AdjustWalletBalanceAsync(dto);

            return Ok(new MessageResponseDto($"Successfully credited {dto.Amount} EGP to the user's wallet."));
        }

        [Authorize]
        [HttpGet("transactions")]
        public async Task<ActionResult<PaginatedResult<WalletTransactionDto>>> GetMyTransactions([FromQuery] QueryParams queryParams)
        {

            var result = await _serviceManager.PaymobService.GetUserTransactionsAsync(GetUserIdFromJwtClaims(), queryParams);

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

        private Guid GetUserIdFromJwtClaims()
            => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
    }
}
