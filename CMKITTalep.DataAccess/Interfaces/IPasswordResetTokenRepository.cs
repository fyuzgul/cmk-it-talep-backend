using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetByTokenAsync(string token);
        Task<PasswordResetToken?> GetByEmailAsync(string email);
        Task InvalidateTokensByEmailAsync(string email);
    }
}
