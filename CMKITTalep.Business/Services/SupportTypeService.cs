using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class SupportTypeService : GenericService<SupportType>, ISupportTypeService
    {
        private readonly ISupportTypeRepository _supportTypeRepository;

        public SupportTypeService(ISupportTypeRepository supportTypeRepository) : base(supportTypeRepository)
        {
            _supportTypeRepository = supportTypeRepository;
        }

        public async Task<SupportType?> GetByNameAsync(string name)
        {
            return await _supportTypeRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _supportTypeRepository.ExistsByNameAsync(name);
        }

    }
}
