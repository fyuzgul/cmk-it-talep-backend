using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestService : IGenericService<Request>
    {
        Task<IEnumerable<Request>> GetBySupportProviderIdAsync(int supportProviderId);
        Task<IEnumerable<Request>> GetByRequestCreatorIdAsync(int requestCreatorId);
        Task<IEnumerable<Request>> GetByRequestStatusIdAsync(int requestStatusId);
        Task<IEnumerable<Request>> GetByRequestTypeIdAsync(int requestTypeId);
        Task<IEnumerable<Request>> GetByRequestResponseTypeIdAsync(int? requestResponseTypeId);
        Task<IEnumerable<Request>> GetByDescriptionContainingAsync(string description);
        
        // Mesajlaşma metodları
        Task<IEnumerable<Request>> GetUserChatRequestsAsync(int userId);
        Task<bool> UserHasAccessToRequestAsync(int userId, int requestId);
        Task<IEnumerable<RequestResponse>> GetRequestMessagesAsync(int requestId);
        Task<RequestResponse> AddRequestMessageAsync(int requestId, int userId, string message, string? filePath = null);
        Task MarkMessageAsReadAsync(int messageId, int userId);
        Task<RequestResponse?> GetRequestMessageByIdAsync(int messageId);
    }
}
