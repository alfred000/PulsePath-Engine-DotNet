using System;
using System.Threading.Tasks;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Models; // 🔥 Utilise UserProfile pur

namespace PulsePath.Core.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _repository;

        public ProfileService(IProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> InitializeProfileAsync(Guid userId, ProfileDto dto)
        {
            if (dto.Age < 15 || dto.Age > 90) return false; // Exemple de barrière (CA-02.1)

            var existingProfile = await _repository.GetByUserIdAsync(userId);

            if (existingProfile == null)
            {
                var newProfile = new UserProfile // 🔥 Utilisation du modèle de domaine pur
                {
                    UserId = userId,
                    Age = dto.Age,
                    IsMale = dto.IsMale,
                    HeightCm = dto.HeightCm,
                    CurrentWeightKg = dto.CurrentWeightKg,
                    ActivityFactor = dto.ActivityFactor
                };
                await _repository.AddAsync(newProfile);
            }
            else
            {
                existingProfile.Age = dto.Age;
                existingProfile.IsMale = dto.IsMale;
                existingProfile.HeightCm = dto.HeightCm;
                existingProfile.CurrentWeightKg = dto.CurrentWeightKg;
                existingProfile.ActivityFactor = dto.ActivityFactor;
                await _repository.UpdateAsync(existingProfile);
            }

            return await _repository.SaveChangesAsync();
        }
    }
}

