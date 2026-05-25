using Microsoft.EntityFrameworkCore;
using PulsePath_Engine_DotNet.Models;

namespace PulsePath_Engine_DotNet.Data;

public class PulsePathContext : DbContext
{
    public PulsePathContext(DbContextOptions<PulsePathContext> options) : base(options) { }
    public PulsePathContext() { }

    public DbSet<DailyLog> DailyLogs { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfileEntity> UserProfiles { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=pulsepath.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfileEntity>()
            .HasKey(p => p.UserId);

        // 🔥 Ajout des barrières physiologiques strictes au niveau SQL pour le TDD (CA-02.1)
        modelBuilder.Entity<UserProfileEntity>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_UserProfile_Age", "[Age] >= 15 AND [Age] <= 120"));
            entity.ToTable(t => t.HasCheckConstraint("CK_UserProfile_Height", "[HeightCm] >= 100 AND [HeightCm] <= 250"));
            entity.ToTable(t => t.HasCheckConstraint("CK_UserProfile_Weight", "[CurrentWeightKg] >= 40 AND [CurrentWeightKg] <= 250"));
        });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfileEntity>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
