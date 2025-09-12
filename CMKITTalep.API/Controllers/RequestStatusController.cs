using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    public class RequestStatusController : BaseController<RequestStatus>
    {
        private readonly IRequestStatusService _requestStatusService;

        public RequestStatusController(IRequestStatusService requestStatusService) : base(requestStatusService)
        {
            _requestStatusService = requestStatusService;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<RequestStatus>> GetByName(string name)
        {
            var requestStatus = await _requestStatusService.GetByNameAsync(name);
            if (requestStatus == null)
            {
                return NotFound();
            }
            return Ok(requestStatus);
        }

        [HttpPost]
        public override async Task<ActionResult<RequestStatus>> Create(RequestStatus entity)
        {
            // Check if requestStatus with same name already exists
            if (await _requestStatusService.ExistsByNameAsync(entity.Name))
            {
                ModelState.AddModelError("Name", "A request status with this name already exists.");
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }
    }
}
