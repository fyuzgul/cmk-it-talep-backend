using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IUserTypeRepository : IGenericRepository<UserType>
    {
        Task<UserType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
