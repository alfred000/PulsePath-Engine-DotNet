using System;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Models;

namespace PulsePath.Core.Services
{
    public class GoalEngine : IGoalEngine
    {
        public GoalResponseDto EvaluateGoal(UserProfile profile, GoalRequestDto request)
        {
            // Calculate raw parameters
            double totalWeightChange = Math.Abs(profile.CurrentWeightKg - request.TargetWeightKg);
            double plannedWeeklyRate = totalWeightChange / request.DurationWeeks;

            // RM-GOAL-01: Establish the 1% safety ceiling based on current biological weight
            double maxSafeWeeklyRate = profile.CurrentWeightKg * 0.01;

            string status = "Approved";
            string warningMessage = "Goal parameters fall within safe metabolic boundaries.";
            double finalWeeklyRate = plannedWeeklyRate;

            // Trigger structural override if planned rate exceeds 1% safety limit
            if (request.Objective.ToLower() == "perte" && plannedWeeklyRate > maxSafeWeeklyRate)
            {
                status = "Overridden";
                finalWeeklyRate = maxSafeWeeklyRate; // Clamping to safety threshold
                warningMessage = $"The planned rate of {plannedWeeklyRate:F2}kg/week exceeds the 1% metabolic safety ceiling ({maxSafeWeeklyRate:F2}kg/week). Goal auto-adjusted to safe limits.";
            }

            // Standard metabolic conversion: 1kg of fat ~ 7700 kcal
            // Weekly deficit = Rate * 7700 -> Daily deficit = (Rate * 7700) / 7
            int calculatedDailyDeficit = (int)Math.Round((finalWeeklyRate * 7700) / 7);

            return new GoalResponseDto(status, finalWeeklyRate, calculatedDailyDeficit, warningMessage);
        }
    }
}
