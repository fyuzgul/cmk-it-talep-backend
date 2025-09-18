using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestCCRepository : GenericRepository<RequestCC>, IRequestCCRepository
    {
        public RequestCCRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RequestCC>> GetByRequestIdAsync(int requestId)
        {
            return await _context.RequestCCs
                .Include(rcc => rcc.User)
                .Where(rcc => rcc.RequestId == requestId && !rcc.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<RequestCC>> GetByUserIdAsync(int userId)
        {
            return await _context.RequestCCs
                .Include(rcc => rcc.Request)
                .ThenInclude(r => r.RequestCreator)
                .Include(rcc => rcc.Request)
                .ThenInclude(r => r.RequestType)
                .Where(rcc => rcc.UserId == userId && !rcc.IsDeleted)
                .ToListAsync();
        }

        public async Task<RequestCC?> GetByRequestAndUserIdAsync(int requestId, int userId)
        {
            return await _context.RequestCCs
                .Include(rcc => rcc.User)
                .FirstOrDefaultAsync(rcc => rcc.RequestId == requestId && rcc.UserId == userId && !rcc.IsDeleted);
        }

        public async Task DeleteByRequestIdAsync(int requestId)
        {
            var requestCCs = await _context.RequestCCs
                .Where(rcc => rcc.RequestId == requestId)
                .ToListAsync();

            foreach (var requestCC in requestCCs)
            {
                requestCC.IsDeleted = true;
                requestCC.ModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
