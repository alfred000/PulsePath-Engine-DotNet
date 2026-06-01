using System;
using System.Threading.Tasks;
using PulsePath.Core.Models;

namespace PulsePath.Core.Interfaces
{
    public interface IProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserProfile profile);
        Task UpdateAsync(UserProfile profile);
        Task<bool> SaveChangesAsync();
    }
}
