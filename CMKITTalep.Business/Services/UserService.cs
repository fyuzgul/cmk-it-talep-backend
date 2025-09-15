using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;
using BCrypt.Net;

namespace CMKITTalep.Business.Services
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) : base(userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _userRepository.ExistsByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _userRepository.GetByDepartmentIdAsync(departmentId);
        }

        public async Task<IEnumerable<User>> GetByUserTypeIdAsync(int userTypeId)
        {
            return await _userRepository.GetByUserTypeIdAsync(userTypeId);
        }

        public async Task<IEnumerable<User>> GetSupportUsersBySupportTypeIdAsync(int supportTypeId)
        {
            return await _userRepository.GetSupportUsersBySupportTypeIdAsync(supportTypeId);
        }

        public override async Task<User> AddAsync(User entity)
        {
            // Hash the password before saving
            entity.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
            return await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(User entity)
        {
            // Get existing user from database
            var existingUser = await _userRepository.GetByIdAsync(entity.Id);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Update properties
            existingUser.FirstName = entity.FirstName;
            existingUser.LastName = entity.LastName;
            existingUser.Email = entity.Email;
            existingUser.DepartmentId = entity.DepartmentId;
            existingUser.TypeId = entity.TypeId;
            existingUser.ModifiedDate = DateTime.Now;

            // If password is being updated, hash it
            if (!string.IsNullOrEmpty(entity.Password))
            {
                // Check if password is already hashed (starts with $2a$ or $2b$)
                if (!entity.Password.StartsWith("$2a$") && !entity.Password.StartsWith("$2b$"))
                {
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
                }
                else
                {
                    existingUser.Password = entity.Password;
                }
            }

            // Update using existing entity (already tracked)
            await _userRepository.UpdateAsync(existingUser);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
