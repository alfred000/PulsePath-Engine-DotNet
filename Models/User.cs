using System.ComponentModel.DataAnnotations;

namespace PulsePath_Engine_DotNet.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public UserProfileEntity? Profile { get; set; }
    }
}
