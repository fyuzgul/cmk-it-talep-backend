using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestStatusService : GenericService<RequestStatus>, IRequestStatusService
    {
        private readonly IRequestStatusRepository _requestStatusRepository;

        public RequestStatusService(IRequestStatusRepository requestStatusRepository) : base(requestStatusRepository)
        {
            _requestStatusRepository = requestStatusRepository;
        }

        public async Task<RequestStatus?> GetByNameAsync(string name)
        {
            return await _requestStatusRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _requestStatusRepository.ExistsByNameAsync(name);
        }
    }
}
