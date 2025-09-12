using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IPasswordResetTokenService : IGenericService<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<PasswordResetToken?> GetByEmailAsync(string email);
        Task InvalidateTokensByEmailAsync(string email);
        Task<string> GenerateResetTokenAsync(string email);
    }
}
