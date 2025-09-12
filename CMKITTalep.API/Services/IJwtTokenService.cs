using CMKITTalep.Entities;

namespace CMKITTalep.API.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
