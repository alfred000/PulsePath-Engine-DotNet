using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;   // Core assembly for security tokens descriptor
using System.IdentityModel.Tokens.Jwt;  // Core assembly for JWT handler
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PulsePath_Engine_DotNet.Data;
using PulsePath_Engine_DotNet.Models;
using PulsePath_Engine_DotNet.DTOs;
 

namespace PulsePath.Engine.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly PulsePathContext _context;
        private readonly IConfiguration _config;

        public AuthController(PulsePathContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Vérification de l'unicité de l'email
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest(new { message = "Email already exists." });
            }

            // RM-AUTH-01: Hachage du mot de passe avec BCrypt
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);

            var newUser = new User 
            { 
                Email = dto.Email, 
                PasswordHash = hashedPassword 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // CA-01.3 : Gestion des erreurs d'authentification
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // CA-01.2 : Génération du jeton JWT
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyStr = _config["Jwt:Key"] ?? "SUPER_SECRET_KEY_MIN_32_CHARS_LONG_PULSEPATH_2026";
            var key = Encoding.UTF8.GetBytes(keyStr);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
