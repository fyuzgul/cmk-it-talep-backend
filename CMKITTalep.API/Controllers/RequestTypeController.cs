using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Attributes;

namespace CMKITTalep.API.Controllers
{
    public class RequestTypeController : BaseController<RequestType>
    {
        private readonly IRequestTypeService _requestTypeService;

        public RequestTypeController(IRequestTypeService requestTypeService) : base(requestTypeService)
        {
            _requestTypeService = requestTypeService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<RequestType>> GetByName(string name)
        {
            var requestType = await _requestTypeService.GetByNameAsync(name);
            if (requestType == null)
            {
                return NotFound();
            }
            return Ok(requestType);
        }

        [HttpGet("supporttype/{supportTypeId}")]
        public async Task<ActionResult<IEnumerable<RequestType>>> GetBySupportType(int supportTypeId)
        {
            var requestTypes = await _requestTypeService.GetBySupportTypeIdAsync(supportTypeId);
            return Ok(requestTypes);
        }

        [HttpPost]
        [RequireAdmin]
        public override async Task<ActionResult<RequestType>> Create(RequestType entity)
        {
            // Check if requestType with same name already exists
            if (await _requestTypeService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A request type with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        [RequireAdmin]
        public override async Task<IActionResult> Update(int id, RequestType entity)
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
