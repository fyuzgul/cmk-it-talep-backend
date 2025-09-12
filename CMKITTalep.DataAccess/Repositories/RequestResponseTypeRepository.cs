using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestResponseTypeRepository : GenericRepository<RequestResponseType>, IRequestResponseTypeRepository
    {
        public RequestResponseTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RequestResponseType?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(r => r.Name == name);
        }
    }
}
