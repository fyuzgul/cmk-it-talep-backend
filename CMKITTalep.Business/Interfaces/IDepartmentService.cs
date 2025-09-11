using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IDepartmentService : IGenericService<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
