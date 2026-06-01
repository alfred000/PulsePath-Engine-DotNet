using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Models;          // Modèle du Core
using PulsePath.Infrastructure.Data;
using PulsePath.Infrastructure.Entities; // 🔥 Entités SQLite d'infrastructure autorisées ici

namespace PulsePath.Infrastructure.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
        {
            var entity = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (entity == null) return null;

            // Traduction : Entité Infrastructure ➔ Modèle Core
            return new UserProfile
            {
                UserId = entity.UserId,
                Age = entity.Age,
                IsMale = entity.IsMale,
                HeightCm = entity.HeightCm,
                CurrentWeightKg = entity.CurrentWeightKg,
                ActivityFactor = entity.ActivityFactor
            };
        }

        public async Task AddAsync(UserProfile profile)
        {
            // Traduction : Modèle Core ➔ Entité Infrastructure pour EF Core
            var entity = new UserProfileEntity
            {
                UserId = profile.UserId,
                Age = profile.Age,
                IsMale = profile.IsMale,
                HeightCm = profile.HeightCm,
                CurrentWeightKg = profile.CurrentWeightKg,
                ActivityFactor = profile.ActivityFactor
            };

            await _context.UserProfiles.AddAsync(entity);
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            var entity = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == profile.UserId);
            if (entity != null)
            {
                entity.Age = profile.Age;
                entity.IsMale = profile.IsMale;
                entity.HeightCm = profile.HeightCm;
                entity.CurrentWeightKg = profile.CurrentWeightKg;
                entity.ActivityFactor = profile.ActivityFactor;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
