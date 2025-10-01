using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Services;
using CMKITTalep.API.Models;

namespace CMKITTalep.API.Controllers
{
    public class RequestController : BaseController<Request>
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IRequestTypeService _requestTypeService;
        private readonly IRequestStatusService _requestStatusService;
        private readonly IEmailService _emailService;
        private readonly IRequestCCService _requestCCService;

        public RequestController(IRequestService requestService, IUserService userService, IRequestTypeService requestTypeService, IRequestStatusService requestStatusService, IEmailService emailService, IRequestCCService requestCCService) : base(requestService)
        {
            _requestService = requestService;
            _userService = userService;
            _requestTypeService = requestTypeService;
            _requestStatusService = requestStatusService;
            _emailService = emailService;
            _requestCCService = requestCCService;
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
            var requests = await _requestService.GetByRequestCreatorIdAsync(requestCreatorId);
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

        [HttpGet("paginated")]
        public async Task<ActionResult<object>> GetPaginated([FromQuery] int? supportProviderId, [FromQuery] int? requestCreatorId, [FromQuery] int? requestStatusId, [FromQuery] int? requestTypeId, [FromQuery] string? description, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (supportProviderId.HasValue)
            {
                var result = await _requestService.GetBySupportProviderIdWithPaginationAsync(supportProviderId.Value, page, pageSize);
                return Ok(new { requests = result.requests, totalCount = result.totalCount, page, pageSize });
            }
            
            // Diğer filtreler için genel pagination (gelecekte implement edilebilir)
            var allRequests = await _requestService.GetAllAsync();
            var totalCount = allRequests.Count();
            var pagedRequests = allRequests.Skip((page - 1) * pageSize).Take(pageSize);
            
            return Ok(new { requests = pagedRequests, totalCount, page, pageSize });
        }

        [HttpPost("create")]
        public async Task<ActionResult<Request>> CreateWithCC([FromBody] CreateRequestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the request entity
            var entity = new Request
            {
                RequestCreatorId = request.RequestCreatorId,
                RequestTypeId = request.RequestTypeId,
                PriorityLevelId = request.PriorityLevelId,
                Description = request.Description,
                ScreenshotFilePath = request.ScreenshotFilePath,
                ScreenshotBase64 = request.ScreenshotBase64,
                ScreenshotFileName = request.ScreenshotFileName,
                ScreenshotMimeType = request.ScreenshotMimeType,
                SupportProviderId = null, // Support provider will be assigned later
                RequestStatusId = 1, // Default status (e.g., "Open" or "Pending")
                CreatedDate = DateTime.Now
            };

            var result = await base.Create(entity);

            // Add CC users if any
            if (request.CCUserIds.Any())
            {
                try
                {
                    Console.WriteLine($"DEBUG: Adding CC users: [{string.Join(", ", request.CCUserIds)}]");
                    
                    // Get the created request from the result
                    Request? createdRequest = null;
                    if (result.Result is CreatedAtActionResult createdResult && createdResult.Value is Request requestValue)
                    {
                        createdRequest = requestValue;
                    }
                    else if (result.Value != null)
                    {
                        createdRequest = result.Value;
                    }
                    
                    Console.WriteLine($"DEBUG: Created request ID: {createdRequest?.Id}");
                    
                    if (createdRequest != null)
                    {
                        await _requestCCService.UpdateCCUsersAsync(createdRequest.Id, request.CCUserIds);
                        Console.WriteLine($"DEBUG: CC users added successfully");
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: Created request is null, cannot add CC users");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DEBUG: Failed to add CC users: {ex.Message}");
                    Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
                }
            }

            // Send notification emails
            try
            {
                Console.WriteLine("DEBUG: Starting notification process for CreateWithCC...");
                
                // Get the created request from the result
                Request? createdRequest = null;
                if (result.Result is CreatedAtActionResult createdResult && createdResult.Value is Request requestValue)
                {
                    createdRequest = requestValue;
                }
                else if (result.Value != null)
                {
                    createdRequest = result.Value;
                }
                
                Console.WriteLine($"DEBUG: Extracted request: {createdRequest?.Id}");
                
                if (createdRequest != null)
                {
                    Console.WriteLine($"DEBUG: Request created with ID: {createdRequest.Id}, RequestTypeId: {createdRequest.RequestTypeId}");
                    Console.WriteLine($"DEBUG: RequestCreatorId: {createdRequest.RequestCreatorId}");
                    
                    // Get the request with navigation properties loaded
                    var fullRequest = await _requestService.GetByIdAsync(createdRequest.Id);
                    Console.WriteLine($"DEBUG: Full request loaded. RequestCreator: {fullRequest?.RequestCreator?.FirstName} {fullRequest?.RequestCreator?.LastName}");
                    
                    var requestType = await _requestTypeService.GetByIdAsync(createdRequest.RequestTypeId);
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
                                createdRequest.Description,
                                requestType.Name,
                                supportType.Name
                            );
                            
                            // Send notification to CC users
                            try
                            {
                                var ccUsers = await _requestCCService.GetByRequestIdAsync(createdRequest.Id);
                                if (ccUsers.Any())
                                {
                                    var ccEmails = ccUsers.Select(cc => cc.User?.Email).Where(email => !string.IsNullOrEmpty(email)).ToList();
                                    if (ccEmails.Any())
                                    {
                                        Console.WriteLine($"DEBUG: Sending CC notification to {ccEmails.Count} users");
                                        await _emailService.SendNewRequestNotificationAsync(
                                            ccEmails,
                                            requesterName,
                                            createdRequest.Description,
                                            requestType.Name,
                                            supportType.Name,
                                            isCCNotification: true
                                        );
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("DEBUG: No CC users found");
                                }
                            }
                            catch (Exception ccEx)
                            {
                                Console.WriteLine($"DEBUG: Failed to send CC notification: {ccEx.Message}");
                            }
                            
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
            // PriorityLevelId comes from frontend selection

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
                            
                            // Send notification to CC users
                            try
                            {
                                var ccUsers = await _requestCCService.GetByRequestIdAsync(request.Id);
                                if (ccUsers.Any())
                                {
                                    var ccEmails = ccUsers.Select(cc => cc.User?.Email).Where(email => !string.IsNullOrEmpty(email)).ToList();
                                    if (ccEmails.Any())
                                    {
                                        await _emailService.SendNewRequestNotificationAsync(
                                            ccEmails,
                                            requesterName,
                                            request.Description,
                                            requestType.Name,
                                            supportType.Name,
                                            isCCNotification: true
                                        );
                                    }
                                }
                            }
                            catch (Exception ccEx)
                            {
                                Console.WriteLine($"DEBUG: Failed to send CC notification: {ccEx.Message}");
                            }
                            
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
