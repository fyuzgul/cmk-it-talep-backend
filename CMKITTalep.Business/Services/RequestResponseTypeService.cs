using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestResponseTypeService : GenericService<RequestResponseType>, IRequestResponseTypeService
    {
        private readonly IRequestResponseTypeRepository _requestResponseTypeRepository;

        public RequestResponseTypeService(IRequestResponseTypeRepository requestResponseTypeRepository) : base(requestResponseTypeRepository)
        {
            _requestResponseTypeRepository = requestResponseTypeRepository;
        }

        public async Task<RequestResponseType?> GetByNameAsync(string name)
        {
            return await _requestResponseTypeRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _requestResponseTypeRepository.ExistsByNameAsync(name);
        }
    }
}
