using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.IsDeleted == false)
                               .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && u.IsDeleted == false);
        }

        public async Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.DepartmentId == departmentId && u.IsDeleted == false)
                               .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.TypeId == userTypeId && u.IsDeleted == false)
                               .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetSupportUsersBySupportTypeIdAsync(int supportTypeId)
        {
            // Only return users with UserType that contains "support" (not admin or it)
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.UserType != null && 
                                          u.UserType.Name.ToLower().Contains("support") &&
                                          u.IsDeleted == false)
                               .ToListAsync();
        }

        public new async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.IsDeleted == false)
                               .ToListAsync();
        }

        public new async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.IsDeleted == false)
                               .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Mesajlaşma için ek metodlar
        public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<int> userIds)
        {
            return await _dbSet
                .Where(u => userIds.Contains(u.Id) && u.IsDeleted == false)
                .Include(u => u.Department)
                .Include(u => u.UserType)
                .ToListAsync();
        }

        // Silinen kullanıcılar için metodlar
        public async Task<IEnumerable<User>> GetDeletedUsersAsync()
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.IsDeleted == true)
                               .ToListAsync();
        }

        public async Task RestoreUserAsync(int id)
        {
            var user = await _dbSet.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = false;
                user.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
