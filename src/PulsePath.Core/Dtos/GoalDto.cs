using System.ComponentModel.DataAnnotations;

namespace PulsePath.Core.Dtos
{
    public record GoalRequestDto(
        [Required] string Objective, // "perte" or "gain"
        [Required, Range(40, 250)] double TargetWeightKg,
        [Required, Range(1, 52)] int DurationWeeks
    );

    public record GoalResponseDto(
        string Status, // "Approved" or "Overridden"
        double SafeWeeklyRateKg,
        int CaloricDeficitTarget,
        string WarningMessage
    );
}
