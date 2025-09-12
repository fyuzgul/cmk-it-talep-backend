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
    }
}
