using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestService : GenericService<Request>, IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IRequestRepository requestRepository) : base(requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<IEnumerable<Request>> GetBySupportProviderIdAsync(int supportProviderId)
        {
            return await _requestRepository.GetBySupportProviderIdAsync(supportProviderId);
        }

        public async Task<IEnumerable<Request>> GetByRequestCreatorIdAsync(int requestCreatorId)
        {
            return await _requestRepository.GetByRequestCreatorIdAsync(requestCreatorId);
        }

        public async Task<IEnumerable<Request>> GetByRequestStatusIdAsync(int requestStatusId)
        {
            return await _requestRepository.GetByRequestStatusIdAsync(requestStatusId);
        }

        public async Task<IEnumerable<Request>> GetByRequestTypeIdAsync(int requestTypeId)
        {
            return await _requestRepository.GetByRequestTypeIdAsync(requestTypeId);
        }

        public async Task<IEnumerable<Request>> GetByRequestResponseTypeIdAsync(int? requestResponseTypeId)
        {
            return await _requestRepository.GetByRequestResponseTypeIdAsync(requestResponseTypeId);
        }

        public async Task<IEnumerable<Request>> GetByDescriptionContainingAsync(string description)
        {
            return await _requestRepository.GetByDescriptionContainingAsync(description);
        }

        // Mesajlaşma metodları
        public async Task<IEnumerable<Request>> GetUserChatRequestsAsync(int userId)
        {
            return await _requestRepository.GetUserChatRequestsAsync(userId);
        }

        public async Task<bool> UserHasAccessToRequestAsync(int userId, int requestId)
        {
            return await _requestRepository.UserHasAccessToRequestAsync(userId, requestId);
        }

        public async Task<IEnumerable<RequestResponse>> GetRequestMessagesAsync(int requestId)
        {
            return await _requestRepository.GetRequestMessagesAsync(requestId);
        }

        public async Task<RequestResponse> AddRequestMessageAsync(int requestId, int userId, string message, string? filePath = null)
        {
            return await _requestRepository.AddRequestMessageAsync(requestId, userId, message, filePath);
        }

        public async Task MarkMessageAsReadAsync(int messageId, int userId)
        {
            await _requestRepository.MarkMessageAsReadAsync(messageId, userId);
        }
    }
}
