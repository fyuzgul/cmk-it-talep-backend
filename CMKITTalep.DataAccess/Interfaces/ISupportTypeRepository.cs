using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface ISupportTypeRepository : IGenericRepository<SupportType>
    {
        Task<SupportType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
