using System.Threading.Tasks;
using PulsePath.Core.Dtos;

namespace PulsePath.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
