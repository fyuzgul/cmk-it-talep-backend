using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    public class SupportTypeController : BaseController<SupportType>
    {
        private readonly ISupportTypeService _supportTypeService;

        public SupportTypeController(ISupportTypeService supportTypeService) : base(supportTypeService)
        {
            _supportTypeService = supportTypeService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<SupportType>> GetByName(string name)
        {
            var supportType = await _supportTypeService.GetByNameAsync(name);
            if (supportType == null)
            {
                return NotFound();
            }
            return Ok(supportType);
        }


        [HttpPost]
        public override async Task<ActionResult<SupportType>> Create(SupportType entity)
        {
            // Check if supportType with same name already exists
            if (await _supportTypeService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A support type with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }
    }
}
