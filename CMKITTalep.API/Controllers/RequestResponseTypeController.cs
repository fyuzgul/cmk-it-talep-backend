using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    public class RequestResponseTypeController : BaseController<RequestResponseType>
    {
        private readonly IRequestResponseTypeService _requestResponseTypeService;

        public RequestResponseTypeController(IRequestResponseTypeService requestResponseTypeService) : base(requestResponseTypeService)
        {
            _requestResponseTypeService = requestResponseTypeService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<RequestResponseType>> GetByName(string name)
        {
            var requestResponseType = await _requestResponseTypeService.GetByNameAsync(name);
            if (requestResponseType == null)
            {
                return NotFound();
            }
            return Ok(requestResponseType);
        }

        [HttpPost]
        public override async Task<ActionResult<RequestResponseType>> Create(RequestResponseType entity)
        {
            // Check if requestResponseType with same name already exists
            if (await _requestResponseTypeService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A request response type with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }
    }
}
