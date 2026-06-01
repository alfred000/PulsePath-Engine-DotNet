using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PulsePath.Core.Dtos;
using PulsePath.Core.Models;
using PulsePath.Core.Services;
using PulsePath.Infrastructure.Data;
using PulsePath.Infrastructure.Entities; // Requis pour insérer UserEntity
using PulsePath.Infrastructure.Repositories;

namespace PulsePath.Tests;

public class ProfileServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SqliteConnection _connection;
    private readonly ProfileRepository _repository;
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using (var command = _connection.CreateCommand())
        {
            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new ProfileRepository(_context);
        _service = new ProfileService(_repository);
    }

    [Fact]
    public async Task Test_CA_02_1_Should_Reject_Profile_Saisie_Hors_Plage_Age()
    {
        var userId = Guid.NewGuid();
        var invalidDto = new ProfileDto(12, true, 175, 75, 1.5);

        var result = await _service.InitializeProfileAsync(userId, invalidDto);

        Assert.False(result);
        var profileInDb = await _repository.GetByUserIdAsync(userId);
        Assert.Null(profileInDb);
    }

    [Fact]
    public async Task Test_US_02_Should_Persist_Valid_Profile_Metabolic_Variables()
    {
        // 1. Arrange : Créer et insérer d'abord l'utilisateur parent pour satisfaire la clé étrangère
        var testUser = new UserEntity 
        { 
            Id = Guid.NewGuid(), 
            Email = "valid-profile@pulsepath.com", 
            PasswordHash = "secure_hash" 
        };
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync(); // Sauvegarde l'utilisateur parent

        var validDto = new ProfileDto(30, true, 180, 80, 1.55);

        // 2. Act : On initialise le profil sur le vrai Id de l'utilisateur créé
        var result = await _service.InitializeProfileAsync(testUser.Id, validDto);

        // 3. Assert
        Assert.True(result);
        var savedProfile = await _repository.GetByUserIdAsync(testUser.Id);
        Assert.NotNull(savedProfile);
        Assert.Equal(30, savedProfile.Age);
        Assert.True(savedProfile.IsMale);
        Assert.Equal(180, savedProfile.HeightCm);
        Assert.Equal(80, savedProfile.CurrentWeightKg);
        Assert.Equal(1.55, savedProfile.ActivityFactor);
    }

    [Fact]
    public async Task Test_US_02_Should_Update_Profile_If_Already_Exists()
    {
        // 1. Arrange : Créer et insérer l'utilisateur parent
        var testUser = new UserEntity 
        { 
            Id = Guid.NewGuid(), 
            Email = "update-profile@pulsepath.com", 
            PasswordHash = "secure_hash" 
        };
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();

        // Création du profil initial
        var initialDto = new ProfileDto(25, false, 165, 60, 1.2);
        await _service.InitializeProfileAsync(testUser.Id, initialDto);

        // Nouvelles données de mise à jour (PUT)
        var updatedDto = new ProfileDto(25, false, 165, 65, 1.375);

        // 2. Act
        var result = await _service.InitializeProfileAsync(testUser.Id, updatedDto);

        // 3. Assert
        Assert.True(result);
        var profileInDb = await _repository.GetByUserIdAsync(testUser.Id);
        Assert.NotNull(profileInDb);
        Assert.Equal(65, profileInDb.CurrentWeightKg);
        Assert.Equal(1.375, profileInDb.ActivityFactor);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
