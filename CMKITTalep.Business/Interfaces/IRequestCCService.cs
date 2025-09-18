using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestCCService : IGenericService<RequestCC>
    {
        Task<IEnumerable<RequestCC>> GetByRequestIdAsync(int requestId);
        Task<IEnumerable<RequestCC>> GetByUserIdAsync(int userId);
        Task<RequestCC?> GetByRequestAndUserIdAsync(int requestId, int userId);
        Task AddCCUserAsync(int requestId, int userId);
        Task RemoveCCUserAsync(int requestId, int userId);
        Task UpdateCCUsersAsync(int requestId, List<int> userIds);
        Task DeleteByRequestIdAsync(int requestId);
    }
}
