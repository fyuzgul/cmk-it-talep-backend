using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestStatusRepository : GenericRepository<RequestStatus>, IRequestStatusRepository
    {
        public RequestStatusRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RequestStatus?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(r => r.Name == name);
        }
    }
}
