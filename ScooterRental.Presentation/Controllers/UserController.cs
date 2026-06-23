using System.Text;

namespace ScooterRental.Presentation.Controllers
{
    [Authorize (Roles = "Admin")]
    public class UserController(IServiceManager _serviceManager) : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<UserResponseDto>>> GetAllUsers([FromQuery] QueryParams queryParams)
        {
            var result = await _serviceManager.AuthService.GetAllUsersAsync(queryParams);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserDetails(Guid id)
        {
            var result = await _serviceManager.AuthService.GetUserByIdAsync(id);

            return Ok(result);
        }

        [HttpPost("{id}/suspend")]
        public async Task<ActionResult> SuspendUser(Guid id)
        {
            await _serviceManager.AuthService.SuspendUserAsync(id);

            return Ok(new MessageResponseDto("User account suspended successfully."));
        }

        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateUser(Guid id)
        {
            await _serviceManager.AuthService.ActivateUserAsync(id);

            return Ok(new MessageResponseDto("User account activated successfully."));
        }
    }
}
