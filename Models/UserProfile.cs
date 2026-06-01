using System.ComponentModel.DataAnnotations;

namespace PulsePath_Engine_DotNet.Models
{
    public class UserProfileEntity
    {
        [Key]
        public Guid UserId { get; set; }
        
        [Required, Range(15, 90)]
        public int Age { get; set; }

        // Ajout de la propriété manquante pour corriger l'erreur CS0117
        [Required]
        public bool IsMale { get; set; } 
        
        [Required, RegularExpression("^[MF]$")]
        public string BiologicalSex { get; set; } = string.Empty; // "M" or "F"
        
        [Required, Range(100, 250)]
        public double HeightCm { get; set; }
        
        [Required, Range(40, 250)]
        public double CurrentWeightKg { get; set; }
        
        [Required, Range(1.2, 2.5)]
        public double ActivityFactor { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
}
