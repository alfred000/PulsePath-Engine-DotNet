using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Models;         // Modèle attendu par le contrat
using PulsePath.Infrastructure.Data;
using PulsePath.Infrastructure.Entities; // 🔥 Autorisé ici pour manipuler UserEntity

namespace PulsePath.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (entity == null) return null;

            // Mapping de l'entité Infrastructure vers le modèle Core
            return new User
            {
                Id = entity.Id,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash
            };
        }

        public async Task AddAsync(User user)
        {
            // Mapping du modèle Core vers l'entité Infrastructure pour la persistance
            var entity = new UserEntity
            {
                Id = user.Id,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            await _context.Users.AddAsync(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
