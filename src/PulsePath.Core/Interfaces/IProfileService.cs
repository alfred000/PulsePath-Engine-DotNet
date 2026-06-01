using System;
using System.Threading.Tasks;
using PulsePath.Core.Dtos;

namespace PulsePath.Core.Interfaces
{
    public interface IProfileService
    {
        Task<bool> InitializeProfileAsync(Guid userId, ProfileDto dto);
    }
}
