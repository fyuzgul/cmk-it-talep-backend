using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
