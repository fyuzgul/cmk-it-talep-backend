using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Attributes;

namespace CMKITTalep.API.Controllers
{
    public class DepartmentController : BaseController<Department>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService) : base(departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Department>> GetByName(string name)
        {
            var department = await _departmentService.GetByNameAsync(name);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpPost]
        [RequireAdmin]
        public override async Task<ActionResult<Department>> Create(Department entity)
        {
            // Check if department with same name already exists
            if (await _departmentService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A department with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Update(int id, Department entity)
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
