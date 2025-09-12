using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class RequestResponseService : GenericService<RequestResponse>, IRequestResponseService
    {
        private readonly IRequestResponseRepository _requestResponseRepository;

        public RequestResponseService(IRequestResponseRepository requestResponseRepository) : base(requestResponseRepository)
        {
            _requestResponseRepository = requestResponseRepository;
        }

        public async Task<IEnumerable<RequestResponse>> GetByRequestIdAsync(int requestId)
        {
            return await _requestResponseRepository.GetByRequestIdAsync(requestId);
        }

        public async Task<IEnumerable<RequestResponse>> GetByMessageContainingAsync(string message)
        {
            return await _requestResponseRepository.GetByMessageContainingAsync(message);
        }
    }
}
