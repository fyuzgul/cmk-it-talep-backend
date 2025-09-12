using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    public class RequestController : BaseController<Request>
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService) : base(requestService)
        {
            _requestService = requestService;
        }

        [HttpGet("supportprovider/{supportProviderId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetBySupportProvider(int supportProviderId)
        {
            var requests = await _requestService.GetBySupportProviderIdAsync(supportProviderId);
            return Ok(requests);
        }

        [HttpGet("creator/{requestCreatorId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetByRequestCreator(int requestCreatorId)
        {
            Console.WriteLine($"DEBUG: Searching for RequestCreatorId = {requestCreatorId}");
            var requests = await _requestService.GetByRequestCreatorIdAsync(requestCreatorId);
            Console.WriteLine($"DEBUG: Found {requests.Count()} requests");
            return Ok(requests);
        }

        [HttpGet("status/{requestStatusId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetByRequestStatus(int requestStatusId)
        {
            var requests = await _requestService.GetByRequestStatusIdAsync(requestStatusId);
            return Ok(requests);
        }

        [HttpGet("type/{requestTypeId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetByRequestType(int requestTypeId)
        {
            var requests = await _requestService.GetByRequestTypeIdAsync(requestTypeId);
            return Ok(requests);
        }

        // This endpoint is no longer valid since RequestResponseTypeId moved to RequestResponse
        // Consider removing this endpoint or implementing it differently
        [HttpGet("responsetype/{requestResponseTypeId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetByRequestResponseType(int? requestResponseTypeId)
        {
            // Since RequestResponseTypeId is now in RequestResponse, this endpoint needs to be reimplemented
            // or removed entirely. For now, returning empty collection.
            var requests = await _requestService.GetByRequestResponseTypeIdAsync(requestResponseTypeId);
            return Ok(requests);
        }

        [HttpGet("search/{description}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetByDescription(string description)
        {
            var requests = await _requestService.GetByDescriptionContainingAsync(description);
            return Ok(requests);
        }

        [HttpPost]
        public override async Task<ActionResult<Request>> Create(Request entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set default values for new requests
            entity.SupportProviderId = null; // Support provider will be assigned later
            entity.RequestStatusId = 1; // Default status (e.g., "Open" or "Pending")

            return await base.Create(entity);
        }

        [HttpPut("{id}")]
        public override async Task<IActionResult> Update(int id, Request entity)
        {
            if (id != entity.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!await _requestService.ExistsAsync(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Clear navigation properties to avoid tracking conflicts
            entity.SupportProvider = null;
            entity.RequestCreator = null;
            entity.RequestStatus = null;
            entity.RequestType = null;
            entity.RequestResponses = new List<RequestResponse>();

            await _requestService.UpdateAsync(entity);
            return NoContent();
        }
    }
}
