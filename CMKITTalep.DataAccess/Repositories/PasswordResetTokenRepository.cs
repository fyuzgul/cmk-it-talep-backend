using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class PasswordResetTokenRepository : GenericRepository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<PasswordResetToken?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Email == email && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task InvalidateTokensByEmailAsync(string email)
        {
            var tokens = await _dbSet.Where(t => t.Email == email && !t.IsUsed).ToListAsync();
            foreach (var token in tokens)
            {
                token.IsUsed = true;
                token.ModifiedDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
