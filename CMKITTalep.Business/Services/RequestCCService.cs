using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestCCService : GenericService<RequestCC>, IRequestCCService
    {
        private readonly IRequestCCRepository _requestCCRepository;

        public RequestCCService(IRequestCCRepository requestCCRepository) : base(requestCCRepository)
        {
            _requestCCRepository = requestCCRepository;
        }

        public async Task<IEnumerable<RequestCC>> GetByRequestIdAsync(int requestId)
        {
            return await _requestCCRepository.GetByRequestIdAsync(requestId);
        }

        public async Task<IEnumerable<RequestCC>> GetByUserIdAsync(int userId)
        {
            return await _requestCCRepository.GetByUserIdAsync(userId);
        }

        public async Task<RequestCC?> GetByRequestAndUserIdAsync(int requestId, int userId)
        {
            return await _requestCCRepository.GetByRequestAndUserIdAsync(requestId, userId);
        }

        public async Task AddCCUserAsync(int requestId, int userId)
        {
            // Check if user is already CC'd
            var existingCC = await GetByRequestAndUserIdAsync(requestId, userId);
            if (existingCC != null)
            {
                return; // User is already CC'd
            }

            var requestCC = new RequestCC
            {
                RequestId = requestId,
                UserId = userId,
                AddedDate = DateTime.Now
            };

            await AddAsync(requestCC);
        }

        public async Task RemoveCCUserAsync(int requestId, int userId)
        {
            var requestCC = await GetByRequestAndUserIdAsync(requestId, userId);
            if (requestCC != null)
            {
                await DeleteAsync(requestCC.Id);
            }
        }

        public async Task UpdateCCUsersAsync(int requestId, List<int> userIds)
        {
            Console.WriteLine($"DEBUG: UpdateCCUsersAsync called with RequestId: {requestId}, UserIds: [{string.Join(", ", userIds)}]");
            
            try
            {
                // First, remove all existing CC users for this request
                await _requestCCRepository.DeleteByRequestIdAsync(requestId);
                Console.WriteLine($"DEBUG: Deleted existing CC users for request {requestId}");

                // Then add the new CC users
                foreach (var userId in userIds)
                {
                    Console.WriteLine($"DEBUG: Adding CC user {userId} to request {requestId}");
                    await AddCCUserAsync(requestId, userId);
                    Console.WriteLine($"DEBUG: Successfully added CC user {userId} to request {requestId}");
                }
                
                Console.WriteLine($"DEBUG: UpdateCCUsersAsync completed successfully for request {requestId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: UpdateCCUsersAsync failed: {ex.Message}");
                Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task DeleteByRequestIdAsync(int requestId)
        {
            await _requestCCRepository.DeleteByRequestIdAsync(requestId);
        }
    }
}
