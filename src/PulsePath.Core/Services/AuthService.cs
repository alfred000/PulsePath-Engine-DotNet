using System;
using System.Threading.Tasks;
using PulsePath.Core.Dtos;
using PulsePath.Core.Interfaces;
using PulsePath.Core.Models;

namespace PulsePath.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly ITokenService _tokenService; // 🔥 Utilisation de l'interface contractuelle

        public AuthService(IAuthRepository repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _repository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return false;

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);

            var newUser = new User
            {
                Email = dto.Email,
                PasswordHash = hashedPassword
            };

            await _repository.AddAsync(newUser);
            return await _repository.SaveChangesAsync();
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            // 🔥 Le Core délègue la création du token sans savoir comment elle est faite
            return _tokenService.GenerateJwtToken(user); 
        }
    }
}
