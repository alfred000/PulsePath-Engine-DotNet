using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;

namespace PulsePath.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/goals")]
    public class GoalController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IGoalEngine _goalEngine;

        public GoalController(IProfileRepository profileRepository, IGoalEngine goalEngine)
        {
            _profileRepository = profileRepository;
            _goalEngine = goalEngine;
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateMetabolicGoal([FromBody] GoalRequestDto dto)
        {
            // Extract unique UserId from JWT claim tokens
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { message = "Invalid user session context." });
            }

            // Fetch current metabolic profile attributes from database layer
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return BadRequest(new { message = "Metabolic profile variables must be initialized (US-02) before setting goals." });
            }

            var analysis = _goalEngine.EvaluateGoal(profile, dto);
            return Ok(analysis);
        }
    }
}
    