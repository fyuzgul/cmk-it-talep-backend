using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId);
        Task<IEnumerable<User>> GetSupportUsersBySupportTypeIdAsync(int supportTypeId);
        bool VerifyPassword(string password, string hashedPassword);
        
        // Mesajlaşma için ek metodlar
        Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<int> userIds);
        
        // Silinen kullanıcılar için metodlar
        Task<IEnumerable<User>> GetDeletedUsersAsync();
        Task RestoreUserAsync(int id);
    }
}
