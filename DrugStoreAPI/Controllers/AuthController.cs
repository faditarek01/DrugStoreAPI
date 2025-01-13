
namespace DrugStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(ModelState);

            var result = await _authRepository.GetTokenAsync(id);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
