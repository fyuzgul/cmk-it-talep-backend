using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestStatusService : IGenericService<RequestStatus>
    {
        Task<RequestStatus?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
