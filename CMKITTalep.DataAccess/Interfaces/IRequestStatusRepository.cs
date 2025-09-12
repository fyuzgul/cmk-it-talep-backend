using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestStatusRepository : IGenericRepository<RequestStatus>
    {
        Task<RequestStatus?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
