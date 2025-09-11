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
                               .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.DepartmentId == departmentId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .Where(u => u.TypeId == userTypeId)
                               .ToListAsync();
        }

        public new async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .ToListAsync();
        }

        public new async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(u => u.Department)
                               .Include(u => u.UserType)
                               .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
