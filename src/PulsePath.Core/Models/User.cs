namespace PulsePath.Core.Models
    
{
    public class User
    {
        public Guid Id { get; set; } // Clé primaire auto-générée
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public UserProfile? Profile { get; set; }
    }
}