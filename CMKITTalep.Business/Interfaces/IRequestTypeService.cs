using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestTypeService : IGenericService<RequestType>
    {
        Task<RequestType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameAndSupportTypeAsync(string name, int supportTypeId);
        Task<IEnumerable<RequestType>> GetBySupportTypeIdAsync(int supportTypeId);
    }
}
