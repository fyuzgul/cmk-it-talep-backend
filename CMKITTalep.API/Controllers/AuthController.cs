using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Models;
using CMKITTalep.API.Services;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserTypeService _userTypeService;
        private readonly IPasswordResetTokenService _passwordResetTokenService;
        private readonly IEmailService _emailService;

        public AuthController(IUserService userService, IUserTypeService userTypeService, IPasswordResetTokenService passwordResetTokenService, IEmailService emailService)
        {
            _userService = userService;
            _userTypeService = userTypeService;
            _passwordResetTokenService = passwordResetTokenService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!_userService.VerifyPassword(request.Password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Return user without password
            return Ok(new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                departmentId = user.DepartmentId,
                typeId = user.TypeId,
                department = user.Department,
                userType = user.UserType
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request)
        {
            // Check if user with same email already exists
            if (await _userService.ExistsByEmailAsync(request.Email))
            {
                return BadRequest(new { message = "A user with this email already exists." });
            }

            // Get default UserType (User)
            var defaultUserType = (await _userTypeService.GetAllAsync()).FirstOrDefault(ut => ut.Name.ToLower() == "user");
            if (defaultUserType == null)
            {
                return BadRequest(new { message = "Default user type not found. Please contact administrator." });
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password, // Will be hashed in service
                DepartmentId = request.DepartmentId,
                TypeId = defaultUserType.Id
            };

            var createdUser = await _userService.AddAsync(user);

            // Return user without password
            return Ok(new
            {
                id = createdUser.Id,
                firstName = createdUser.FirstName,
                lastName = createdUser.LastName,
                email = createdUser.Email,
                departmentId = createdUser.DepartmentId,
                typeId = createdUser.TypeId,
                department = createdUser.Department,
                userType = createdUser.UserType
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                // Güvenlik için, kullanıcı bulunamasa bile başarılı mesaj döndür
                return Ok(new { message = "If the email exists, a password reset code has been sent." });
            }

            var resetToken = await _passwordResetTokenService.GenerateResetTokenAsync(request.Email);
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetToken);

            return Ok(new { message = "If the email exists, a password reset code has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var resetToken = await _passwordResetTokenService.GetByTokenAsync(request.Token);
            if (resetToken == null || resetToken.Email != request.Email)
            {
                return BadRequest(new { message = "Invalid or expired reset token." });
            }

            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            // Token'ı kullanılmış olarak işaretle
            resetToken.IsUsed = true;
            resetToken.ModifiedDate = DateTime.UtcNow;
            await _passwordResetTokenService.UpdateAsync(resetToken);

            // Şifreyi güncelle
            user.Password = request.NewPassword; // Service'de hash'lenecek
            await _userService.UpdateAsync(user);

            return Ok(new { message = "Password has been reset successfully." });
        }

    }
}
