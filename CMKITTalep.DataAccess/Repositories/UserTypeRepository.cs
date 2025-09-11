using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class UserTypeRepository : GenericRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserType?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(u => u.Name == name);
        }
    }
}
