using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestResponseService : IGenericService<RequestResponse>
    {
        Task<IEnumerable<RequestResponse>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<RequestResponse>> GetByMessageContainingAsync(string message);
    }
}
