using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;

namespace PulsePath.API.Controllers
{
    [Authorize] // Verrouille l'endpoint : nécessite impérativement le token JWT
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertProfile([FromBody] ProfileDto dto)
        {
            // Extraction sécurisée du UserId à partir du jeton de session courant (CA-02.2)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid user session token context." });
            }

            var success = await _profileService.InitializeProfileAsync(userId, dto);
            if (!success)
            {
                return BadRequest(new { message = "Physiological boundaries violation or processing error." });
            }

            return Ok(new { message = "Profile metabolic variables initialized successfully." });
        }
    }
}
