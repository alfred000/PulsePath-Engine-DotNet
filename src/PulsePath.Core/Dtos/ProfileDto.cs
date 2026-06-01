using System.ComponentModel.DataAnnotations;

namespace PulsePath.Core.Dtos
{
    public record ProfileDto(
        [Required, Range(15, 90)] int Age,
        [Required] bool IsMale,
        [Required, Range(100, 250)] double HeightCm,
        [Required, Range(40, 250)] double CurrentWeightKg,
        [Required, Range(1.2, 2.5)] double ActivityFactor
    );
}
