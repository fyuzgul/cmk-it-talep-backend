using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface ISupportTypeService : IGenericService<SupportType>
    {
        Task<SupportType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
