using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class UserTypeService : GenericService<UserType>, IUserTypeService
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypeService(IUserTypeRepository userTypeRepository) : base(userTypeRepository)
        {
            _userTypeRepository = userTypeRepository;
        }

        public async Task<UserType?> GetByNameAsync(string name)
        {
            return await _userTypeRepository.GetByNameAsync(name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _userTypeRepository.ExistsByNameAsync(name);
        }
    }
}
