using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestResponseRepository : IGenericRepository<RequestResponse>
    {
        Task<IEnumerable<RequestResponse>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<RequestResponse>> GetByMessageContainingAsync(string message);
    }
}
