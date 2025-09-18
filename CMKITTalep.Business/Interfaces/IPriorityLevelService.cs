using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IPriorityLevelService : IGenericService<PriorityLevel>
    {
        Task<PriorityLevel?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
