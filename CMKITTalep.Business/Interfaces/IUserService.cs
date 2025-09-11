using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
