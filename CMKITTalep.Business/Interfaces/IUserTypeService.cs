using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IUserTypeService : IGenericService<UserType>
    {
        Task<UserType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
