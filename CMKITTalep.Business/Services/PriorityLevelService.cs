using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class PriorityLevelService : GenericService<PriorityLevel>, IPriorityLevelService
    {
        private readonly IPriorityLevelRepository _priorityLevelRepository;

        public PriorityLevelService(IPriorityLevelRepository priorityLevelRepository) : base(priorityLevelRepository)
        {
            _priorityLevelRepository = priorityLevelRepository;
        }

        public async Task<PriorityLevel?> GetByNameAsync(string name)
        {
            return await _priorityLevelRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _priorityLevelRepository.ExistsByNameAsync(name);
        }
    }
}
