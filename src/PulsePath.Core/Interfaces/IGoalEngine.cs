using PulsePath.Core.Dtos;
using PulsePath.Core.Models;

namespace PulsePath.Core.Interfaces
{
    public interface IGoalEngine
    {
        GoalResponseDto EvaluateGoal(UserProfile profile, GoalRequestDto request);
    }
}
