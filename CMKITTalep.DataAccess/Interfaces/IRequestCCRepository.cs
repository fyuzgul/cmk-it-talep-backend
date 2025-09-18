using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestCCRepository : IGenericRepository<RequestCC>
    {
        Task<IEnumerable<RequestCC>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<RequestCC>> GetByUserIdAsync(int userId);
        Task<RequestCC?> GetByRequestAndUserIdAsync(int requestId, int userId);
        Task DeleteByRequestIdAsync(int requestId);
    }
}
