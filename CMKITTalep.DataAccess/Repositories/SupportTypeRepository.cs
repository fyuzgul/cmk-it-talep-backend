using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class SupportTypeRepository : GenericRepository<SupportType>, ISupportTypeRepository
    {
        public SupportTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<SupportType?> GetByNameAsync(string name)
        {
            return await _dbSet.Include(s => s.RequestTypes)
                               .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(s => s.Name == name);
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<IEnumerable<SupportType>> GetAllAsync()
        {
            return await _dbSet.Include(s => s.RequestTypes)
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<SupportType?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(s => s.RequestTypes)
                               .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
