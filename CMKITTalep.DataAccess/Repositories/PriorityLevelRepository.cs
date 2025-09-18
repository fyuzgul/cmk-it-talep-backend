using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class PriorityLevelRepository : GenericRepository<PriorityLevel>, IPriorityLevelRepository
    {
        public PriorityLevelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PriorityLevel?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(p => p.Name == name);
        }
    }
}
