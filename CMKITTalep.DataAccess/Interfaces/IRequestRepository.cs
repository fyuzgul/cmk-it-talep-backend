using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestRepository : IGenericRepository<Request>
    {
        Task<IEnumerable<Request>> GetBySupportProviderIdAsync(int supportProviderId);
        Task<IEnumerable<Request>> GetByRequestCreatorIdAsync(int requestCreatorId);
        Task<IEnumerable<Request>> GetByRequestStatusIdAsync(int requestStatusId);
        Task<IEnumerable<Request>> GetByRequestTypeIdAsync(int requestTypeId);
        Task<IEnumerable<Request>> GetByRequestResponseTypeIdAsync(int? requestResponseTypeId);
        Task<IEnumerable<Request>> GetByDescriptionContainingAsync(string description);
    }
}
