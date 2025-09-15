using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Attributes;

namespace CMKITTalep.API.Controllers
{
    public class UserController : BaseController<User>
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetByDepartment(int departmentId)
        {
            var users = await _userService.GetByDepartmentIdAsync(departmentId);
            return Ok(users);
        }

        [HttpGet("usertype/{userTypeId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetByUserType(int userTypeId)
        {
            var users = await _userService.GetByUserTypeIdAsync(userTypeId);
            return Ok(users);
        }

        [HttpPost]
        [RequireAdmin]
        public override async Task<ActionResult<User>> Create(User entity)
        {
            // Check if user with same email already exists
            if (await _userService.ExistsByEmailAsync(entity.Email))
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Update(int id, User entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }

            // Check if user exists
            if (!await _userService.ExistsAsync(id))
            {
                return NotFound();
            }

            // Get existing user to check email uniqueness
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // If email is being changed, check if new email already exists
            if (existingUser.Email != entity.Email && await _userService.ExistsByEmailAsync(entity.Email))
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
