using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;

namespace PulsePath.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var success = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return BadRequest(new { message = "Email already exists or invalid data." });
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password." }); // CA-01.3
            }

            return Ok(new { token }); // CA-01.2
        }
    }
}
