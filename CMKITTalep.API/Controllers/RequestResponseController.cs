using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    public class RequestResponseController : BaseController<RequestResponse>
    {
        private readonly IRequestResponseService _requestResponseService;

        public RequestResponseController(IRequestResponseService requestResponseService) : base(requestResponseService)
        {
            _requestResponseService = requestResponseService;
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<RequestResponse>>> GetByRequestId(int requestId)
        {
            var requestResponses = await _requestResponseService.GetByRequestIdAsync(requestId);
            return Ok(requestResponses);
        }

        [HttpGet("search/{message}")]
        public async Task<ActionResult<IEnumerable<RequestResponse>>> GetByMessage(string message)
        {
            var requestResponses = await _requestResponseService.GetByMessageContainingAsync(message);
            return Ok(requestResponses);
        }

        [HttpPost]
        public override async Task<ActionResult<RequestResponse>> Create(RequestResponse entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, RequestResponse entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!await _requestResponseService.ExistsAsync(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _requestResponseService.UpdateAsync(entity);
            return NoContent();
        }
    }
}
