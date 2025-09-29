using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestTypeRepository : GenericRepository<RequestType>, IRequestTypeRepository
    {
        public RequestTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RequestType?> GetByNameAsync(string name)
        {
            return await _dbSet.Include(r => r.SupportType)
                               .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsByNameAndSupportTypeAsync(string name, int supportTypeId)
        {
            return await _dbSet.AnyAsync(r => r.Name == name && r.SupportTypeId == supportTypeId);
        }

        public async Task<IEnumerable<RequestType>> GetBySupportTypeIdAsync(int supportTypeId)
        {
            return await _dbSet.Include(r => r.SupportType)
                               .Where(r => r.SupportTypeId == supportTypeId)
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<IEnumerable<RequestType>> GetAllAsync()
        {
            return await _dbSet.Include(r => r.SupportType)
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<RequestType?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(r => r.SupportType)
                               .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
