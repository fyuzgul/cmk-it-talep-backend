using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestResponseRepository : GenericRepository<RequestResponse>, IRequestResponseRepository
    {
        public RequestResponseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RequestResponse>> GetByRequestIdAsync(int requestId)
        {
            return await _dbSet.Include(r => r.Request)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.RequestId == requestId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<RequestResponse>> GetByMessageContainingAsync(string message)
        {
            return await _dbSet.Include(r => r.Request)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.Message.Contains(message))
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<IEnumerable<RequestResponse>> GetAllAsync()
        {
            return await _dbSet.Include(r => r.Request)
                               .Include(r => r.RequestResponseType)
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<RequestResponse?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(r => r.Request)
                               .Include(r => r.RequestResponseType)
                               .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
