using PulsePath.Core.Models;

namespace PulsePath.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
