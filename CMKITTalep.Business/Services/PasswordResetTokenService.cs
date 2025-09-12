using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class PasswordResetTokenService : GenericService<PasswordResetToken>, IPasswordResetTokenService
    {
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

        public PasswordResetTokenService(IPasswordResetTokenRepository passwordResetTokenRepository) : base(passwordResetTokenRepository)
        {
            _passwordResetTokenRepository = passwordResetTokenRepository;
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _passwordResetTokenRepository.GetByTokenAsync(token);
        }

        public async Task<PasswordResetToken?> GetByEmailAsync(string email)
        {
            return await _passwordResetTokenRepository.GetByEmailAsync(email);
        }

        public async Task InvalidateTokensByEmailAsync(string email)
        {
            await _passwordResetTokenRepository.InvalidateTokensByEmailAsync(email);
        }

        public async Task<string> GenerateResetTokenAsync(string email)
        {
            // Önce mevcut token'ları geçersiz kıl
            await InvalidateTokensByEmailAsync(email);

            // Yeni token oluştur
            var token = Guid.NewGuid().ToString("N")[..8].ToUpper(); // 8 karakterlik büyük harfli kod
            
            var resetToken = new PasswordResetToken
            {
                Email = email,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15) // 15 dakika geçerli
            };

            await _passwordResetTokenRepository.AddAsync(resetToken);
            return token;
        }
    }
}
