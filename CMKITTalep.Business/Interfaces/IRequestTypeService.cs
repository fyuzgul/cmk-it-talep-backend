using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestTypeService : IGenericService<RequestType>
    {
        Task<RequestType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<RequestType>> GetBySupportTypeIdAsync(int supportTypeId);
    }
}
