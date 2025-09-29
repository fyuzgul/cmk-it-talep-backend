using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestTypeService : GenericService<RequestType>, IRequestTypeService
    {
        private readonly IRequestTypeRepository _requestTypeRepository;

        public RequestTypeService(IRequestTypeRepository requestTypeRepository) : base(requestTypeRepository)
        {
            _requestTypeRepository = requestTypeRepository;
        }

        public async Task<RequestType?> GetByNameAsync(string name)
        {
            return await _requestTypeRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _requestTypeRepository.ExistsByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAndSupportTypeAsync(string name, int supportTypeId)
        {
            return await _requestTypeRepository.ExistsByNameAndSupportTypeAsync(name, supportTypeId);
        }

        public async Task<IEnumerable<RequestType>> GetBySupportTypeIdAsync(int supportTypeId)
        {
            return await _requestTypeRepository.GetBySupportTypeIdAsync(supportTypeId);
        }
    }
}
