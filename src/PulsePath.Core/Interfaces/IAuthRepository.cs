using System.Threading.Tasks;
using PulsePath.Core.Models;

namespace PulsePath.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
    }
}
    