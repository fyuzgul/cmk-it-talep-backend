using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId);
        Task<IEnumerable<User>> GetSupportUsersBySupportTypeIdAsync(int supportTypeId);
    }
}
