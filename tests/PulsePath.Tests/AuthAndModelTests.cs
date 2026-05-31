using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PulsePath.Infrastructure.Data;
using PulsePath.Infrastructure.Entities;

namespace PulsePath.Tests;

public class AuthAndModelTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SqliteConnection _connection;

    public AuthAndModelTests()
    {
        // 1. Initialisation de la connexion en mémoire vive
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        // 2. Utilisation explicite du constructeur générique typé pour correspondre à l'API
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        
        // 3. Génération du schéma temporaire
        _context.Database.EnsureCreated();
    }

    [Fact]
public async Task Test_RM_AUTH_01_Password_Must_Be_Hashed_On_Registration()
{
    var rawPassword = "MySecurePassword123";
    string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword, salt);

    var newUser = new UserEntity 
    { 
        Email = "tdd-user@pulsepath.com", 
        PasswordHash = hashedPassword 
    };

    _context.Users.Add(newUser);
    await _context.SaveChangesAsync();

    // 🔥 Correction : On appelle FirstOrDefaultAsync sur la table Users (_context.Users)
    var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "tdd-user@pulsepath.com");
    
    Assert.NotNull(savedUser);
    Assert.NotEqual(rawPassword, savedUser.PasswordHash);
    Assert.True(BCrypt.Net.BCrypt.Verify(rawPassword, savedUser.PasswordHash));
}

    [Fact]
    public async Task Test_CA_07_1_SQLite_Should_Reject_Profile_With_Invalid_Age()
    {
        var user = new UserEntity { Email = "age-check@pulsepath.com", PasswordHash = "hash" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var invalidProfile = new UserProfileEntity
        {
            UserId = user.Id,
            Age = 12, // Invalide : Doit déclencher l'erreur à la sauvegarde
            IsMale = true,
            HeightCm = 175,
            CurrentWeightKg = 75,
            ActivityFactor = 1.5
        };

        _context.UserProfiles.Add(invalidProfile);

        // La validation doit lever une exception DbUpdateException ou une validation interne
        await Assert.ThrowsAsync<DbUpdateException>(async () => await _context.SaveChangesAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
