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
                               .Where(r => r.RequestStatusId == requestStatusId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetByRequestTypeIdAsync(int requestTypeId)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
                               .Where(r => r.RequestTypeId == requestTypeId)
                               .ToListAsync();
        }

        // This method is no longer valid since RequestResponseTypeId moved to RequestResponse
        // Consider removing this method or implementing it differently
        public Task<IEnumerable<Request>> GetByRequestResponseTypeIdAsync(int? requestResponseTypeId)
        {
            // Since RequestResponseTypeId is now in RequestResponse, this method needs to be reimplemented
            // or removed entirely. For now, returning empty collection.
            return Task.FromResult<IEnumerable<Request>>(new List<Request>());
        }

        public async Task<IEnumerable<Request>> GetByDescriptionContainingAsync(string description)
        {
            return await _dbSet.Include(r => r.SupportProvider)
                               .Include(r => r.RequestCreator)
                               .Include(r => r.RequestStatus)
                               .Include(r => r.RequestType)
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
                               .FirstOrDefaultAsync();
        }

        // Mesajlaşma metodları
        public async Task<IEnumerable<Request>> GetUserChatRequestsAsync(int userId)
        {
            return await _dbSet
                .Where(r => !r.IsDeleted && (r.RequestCreatorId == userId || r.SupportProviderId == userId))
                .Include(r => r.SupportProvider)
                .Include(r => r.RequestCreator)
                .Include(r => r.RequestStatus)
                .Include(r => r.RequestType)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> UserHasAccessToRequestAsync(int userId, int requestId)
        {
            return await _dbSet
                .AnyAsync(r => r.Id == requestId && !r.IsDeleted && 
                              (r.RequestCreatorId == userId || r.SupportProviderId == userId));
        }

        public async Task<IEnumerable<RequestResponse>> GetRequestMessagesAsync(int requestId)
        {
            return await _context.RequestResponses
                .Where(rr => rr.RequestId == requestId && !rr.IsDeleted)
                .Include(rr => rr.Sender)
                .OrderBy(rr => rr.CreatedDate)
                .ToListAsync();
        }

        public async Task<RequestResponse> AddRequestMessageAsync(int requestId, int userId, string message, string? filePath = null)
        {
            var requestResponse = new RequestResponse
            {
                RequestId = requestId,
                SenderId = userId,
                Message = message,
                FilePath = filePath,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.RequestResponses.Add(requestResponse);
            await _context.SaveChangesAsync();

            // Include navigation properties
            return await _context.RequestResponses
                .Include(rr => rr.Sender)
                .FirstAsync(rr => rr.Id == requestResponse.Id);
        }

        public async Task MarkMessageAsReadAsync(int messageId, int userId)
        {
            // MessageReadStatus tablosunu kullanarak okundu durumunu işaretle
            var existingReadStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(mrs => mrs.MessageId == messageId && mrs.UserId == userId && !mrs.IsDeleted);
            
            if (existingReadStatus == null)
            {
                var readStatus = new MessageReadStatus
                {
                    MessageId = messageId,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                };
                await _context.MessageReadStatuses.AddAsync(readStatus);
            }
            else
            {
                existingReadStatus.ReadAt = DateTime.UtcNow;
                existingReadStatus.ModifiedDate = DateTime.UtcNow;
            }
            
            // Veritabanına kaydet
            await _context.SaveChangesAsync();
        }

        public async Task<RequestResponse?> GetRequestMessageByIdAsync(int messageId)
        {
            return await _context.RequestResponses
                .Where(rr => rr.Id == messageId && !rr.IsDeleted)
                .Include(rr => rr.Sender)
                .FirstOrDefaultAsync();
        }
    }
}
