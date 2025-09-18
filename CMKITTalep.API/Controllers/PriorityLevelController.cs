using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Attributes;

namespace CMKITTalep.API.Controllers
{
    public class PriorityLevelController : BaseController<PriorityLevel>
    {
        private readonly IPriorityLevelService _priorityLevelService;

        public PriorityLevelController(IPriorityLevelService priorityLevelService) : base(priorityLevelService)
        {
            _priorityLevelService = priorityLevelService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<PriorityLevel>> GetByName(string name)
        {
            var priorityLevel = await _priorityLevelService.GetByNameAsync(name);
            if (priorityLevel == null)
            {
                return NotFound();
            }
            return Ok(priorityLevel);
        }

        [HttpPost]
        [RequireAdmin]
        public override async Task<ActionResult<PriorityLevel>> Create(PriorityLevel entity)
        {
            // Check if priority level with same name already exists
            if (await _priorityLevelService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A priority level with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Update(int id, PriorityLevel entity)
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
