using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestTypeRepository : IGenericRepository<RequestType>
    {
        Task<RequestType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<RequestType>> GetBySupportTypeIdAsync(int supportTypeId);
    }
}
