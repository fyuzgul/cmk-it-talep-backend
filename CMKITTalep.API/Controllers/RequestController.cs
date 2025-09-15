using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Services;

namespace CMKITTalep.API.Controllers
{
    public class RequestController : BaseController<Request>
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IRequestTypeService _requestTypeService;
        private readonly IRequestStatusService _requestStatusService;
        private readonly IEmailService _emailService;

        public RequestController(IRequestService requestService, IUserService userService, IRequestTypeService requestTypeService, IRequestStatusService requestStatusService, IEmailService emailService) : base(requestService)
        {
            _requestService = requestService;
            _userService = userService;
            _requestTypeService = requestTypeService;
            _requestStatusService = requestStatusService;
            _emailService = emailService;
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

            var result = await base.Create(entity);

            // Send notification email to support users
            try
            {
                Console.WriteLine("DEBUG: Starting notification process...");
                Console.WriteLine($"DEBUG: Result type: {result.GetType()}");
                Console.WriteLine($"DEBUG: Result.Value: {result.Value}");
                Console.WriteLine($"DEBUG: Result.Result: {result.Result}");
                
                // Get the created request from the result
                Request? request = null;
                if (result.Result is CreatedAtActionResult createdResult && createdResult.Value is Request createdRequest)
                {
                    request = createdRequest;
                }
                else if (result.Value != null)
                {
                    request = result.Value;
                }
                
                Console.WriteLine($"DEBUG: Extracted request: {request?.Id}");
                
                if (request != null)
                {
                    Console.WriteLine($"DEBUG: Request created with ID: {request.Id}, RequestTypeId: {request.RequestTypeId}");
                    Console.WriteLine($"DEBUG: RequestCreatorId: {request.RequestCreatorId}");
                    
                    // Get the request with navigation properties loaded
                    var fullRequest = await _requestService.GetByIdAsync(request.Id);
                    Console.WriteLine($"DEBUG: Full request loaded. RequestCreator: {fullRequest?.RequestCreator?.FirstName} {fullRequest?.RequestCreator?.LastName}");
                    
                    var requestType = await _requestTypeService.GetByIdAsync(request.RequestTypeId);
                    Console.WriteLine($"DEBUG: RequestType found: {requestType?.Name}");
                    
                    var supportType = requestType?.SupportType;
                    Console.WriteLine($"DEBUG: SupportType found: {supportType?.Name}, ID: {supportType?.Id}");
                    
                    if (supportType != null)
                    {
                        var supportUsers = await _userService.GetSupportUsersBySupportTypeIdAsync(supportType.Id);
                        Console.WriteLine($"DEBUG: Found {supportUsers.Count()} support users");
                        
                        var supportEmails = supportUsers.Select(u => u.Email).ToList();
                        Console.WriteLine($"DEBUG: Support emails: {string.Join(", ", supportEmails)}");
                        
                        if (supportEmails.Any())
                        {
                            var requesterName = "Bilinmeyen Kullanıcı";
                            if (fullRequest?.RequestCreator != null)
                            {
                                requesterName = $"{fullRequest.RequestCreator.FirstName} {fullRequest.RequestCreator.LastName}";
                            }
                            Console.WriteLine($"DEBUG: Sending email to support users. Requester: {requesterName}");
                            
                            await _emailService.SendNewRequestNotificationAsync(
                                supportEmails,
                                requesterName,
                                request.Description,
                                requestType.Name,
                                supportType.Name
                            );
                            
                            Console.WriteLine("DEBUG: Email sent successfully!");
                        }
                        else
                        {
                            Console.WriteLine("DEBUG: No support emails found!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("DEBUG: SupportType is null!");
                    }
                }
                else
                {
                    Console.WriteLine("DEBUG: Result.Value is null!");
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request creation
                Console.WriteLine($"DEBUG: Failed to send notification email: {ex.Message}");
                Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
            }

            return result;
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

            // Get the original request to compare changes
            var originalRequest = await _requestService.GetByIdAsync(id);
            if (originalRequest == null)
            {
                return NotFound();
            }

            // Store original values for comparison
            var originalStatusId = originalRequest.RequestStatusId;
            var originalSupportProviderId = originalRequest.SupportProviderId;
            var originalDescription = originalRequest.Description;

            // Update only the properties that can be changed
            // Don't clear navigation properties as they are required
            var existingRequest = await _requestService.GetByIdAsync(id);
            if (existingRequest != null)
            {
                // Update only the fields that should be updated
                existingRequest.Description = entity.Description;
                existingRequest.ScreenshotFilePath = entity.ScreenshotFilePath;
                existingRequest.RequestStatusId = entity.RequestStatusId;
                existingRequest.RequestTypeId = entity.RequestTypeId;
                existingRequest.SupportProviderId = entity.SupportProviderId;
                existingRequest.ModifiedDate = DateTime.Now;

                await _requestService.UpdateAsync(existingRequest);
            }

            // Send notification email to request creator if there are significant changes
            try
            {
                Console.WriteLine($"DEBUG: Request Update - Checking for changes in RequestId: {id}");
                
                var updatedRequest = await _requestService.GetByIdAsync(id);
                if (updatedRequest != null && updatedRequest.RequestCreator != null)
                {
                    var requesterName = $"{updatedRequest.RequestCreator.FirstName} {updatedRequest.RequestCreator.LastName}";
                    var requesterEmail = updatedRequest.RequestCreator.Email;
                    
                    // Check if status changed
                    if (originalStatusId != entity.RequestStatusId)
                    {
                        var oldStatus = await _requestStatusService.GetByIdAsync(originalStatusId);
                        var newStatus = await _requestStatusService.GetByIdAsync(entity.RequestStatusId);
                        
                        Console.WriteLine($"DEBUG: Request Update - Status changed from {oldStatus?.Name} to {newStatus?.Name}");
                        
                        await _emailService.SendRequestUpdateNotificationAsync(
                            requesterEmail,
                            requesterName,
                            updatedRequest.Description,
                            oldStatus?.Name ?? "Bilinmeyen",
                            newStatus?.Name ?? "Bilinmeyen",
                            "Durum Değişikliği"
                        );
                    }
                    // Check if support provider changed
                    else if (originalSupportProviderId != entity.SupportProviderId)
                    {
                        Console.WriteLine($"DEBUG: Request Update - Support provider changed");
                        
                        await _emailService.SendRequestUpdateNotificationAsync(
                            requesterEmail,
                            requesterName,
                            updatedRequest.Description,
                            "Atanmamış",
                            "Atandı",
                            "Destek Sağlayıcı Ataması"
                        );
                    }
                    // Check if description changed significantly
                    else if (originalDescription != entity.Description)
                    {
                        Console.WriteLine($"DEBUG: Request Update - Description changed");
                        
                        await _emailService.SendRequestUpdateNotificationAsync(
                            requesterEmail,
                            requesterName,
                            updatedRequest.Description,
                            "Güncellenmiş",
                            "Güncellenmiş",
                            "Açıklama Güncellemesi"
                        );
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: Request Update - No significant changes detected");
                    }
                }
                else
                {
                    Console.WriteLine($"DEBUG: Request Update - Request or RequestCreator not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Request Update - Failed to send notification: {ex.Message}");
                Console.WriteLine($"DEBUG: Request Update - Stack trace: {ex.StackTrace}");
            }

            return NoContent();
        }
    }
}
