using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Attributes;

namespace CMKITTalep.API.Controllers
{
    public class UserTypeController : BaseController<UserType>
    {
        private readonly IUserTypeService _userTypeService;

        public UserTypeController(IUserTypeService userTypeService) : base(userTypeService)
        {
            _userTypeService = userTypeService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<UserType>> GetByName(string name)
        {
            var userType = await _userTypeService.GetByNameAsync(name);
            if (userType == null)
            {
                return NotFound();
            }
            return Ok(userType);
        }

        [HttpPost]
        [RequireAdmin]
        public override async Task<ActionResult<UserType>> Create(UserType entity)
        {
            // Check if userType with same name already exists
            if (await _userTypeService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A user type with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Update(int id, UserType entity)
        {
            return await base.Update(id, entity);
        }

        [HttpDelete("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
