using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class RequestRepository : GenericRepository<Request>, IRequestRepository
    {
        public RequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Request>> GetBySupportProviderIdAsync(int supportProviderId)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.SupportProviderId == supportProviderId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByRequestCreatorIdAsync(int requestCreatorId)
        {
            Console.WriteLine($"DEBUG Repository: Searching for RequestCreatorId = {requestCreatorId}");
            
            // Önce tüm talepleri sayalım
            var totalRequests = await _dbSet.CountAsync();
            Console.WriteLine($"DEBUG Repository: Total requests in database = {totalRequests}");
            
            // Tüm RequestCreatorId'leri listele
            var allCreatorIds = await _dbSet.Select(r => r.RequestCreatorId).Distinct().ToListAsync();
            Console.WriteLine($"DEBUG Repository: All RequestCreatorIds in database = [{string.Join(", ", allCreatorIds)}]");
            
            // Include'ları kullanarak veri çekelim - SupportProviderId NULL olabilir, bu durumda da kayıt gelsin
            var requests = await _dbSet
                .Where(r => r.RequestCreatorId == requestCreatorId && !r.IsDeleted)
                .Include(r => r.SupportProvider) // NULL olabilir, ama Include ile birlikte gelir
                .Include(r => r.RequestCreator)
                .Include(r => r.RequestStatus)
                .Include(r => r.RequestType)
                .Include(r => r.RequestResponseType)
                .ToListAsync();
                               
            Console.WriteLine($"DEBUG Repository: Found {requests.Count()} requests for RequestCreatorId = {requestCreatorId}");
            return requests;
        }

        public async Task<IEnumerable<Request>> GetByRequestStatusIdAsync(int requestStatusId)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.RequestStatusId == requestStatusId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByRequestTypeIdAsync(int requestTypeId)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.RequestTypeId == requestTypeId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByRequestResponseTypeIdAsync(int? requestResponseTypeId)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.RequestResponseTypeId == requestResponseTypeId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByDescriptionContainingAsync(string description)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .Where(r => r.Description.Contains(description))
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<IEnumerable<Request>> GetAllAsync()
        {
            return await _dbSet.Where(r => !r.IsDeleted)
                               .Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .ToListAsync();
        }

        // Using 'new' keyword to hide the base implementation and include navigation properties
        public new async Task<Request?> GetByIdAsync(int id)
        {
            return await _dbSet.Where(r => r.Id == id && !r.IsDeleted)
                               .Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Include(r => r.RequestResponseType)
                               .FirstOrDefaultAsync();
        }
    }
}
