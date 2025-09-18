using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IPriorityLevelRepository : IGenericRepository<PriorityLevel>
    {
        Task<PriorityLevel?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
