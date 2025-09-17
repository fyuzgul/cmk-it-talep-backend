using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using CMKITTalep.API.Services;
using Microsoft.AspNetCore.SignalR;
using CMKITTalep.API.Hubs;

namespace CMKITTalep.API.Controllers
{
    public class RequestResponseController : BaseController<RequestResponse>
    {
        private readonly IRequestResponseService _requestResponseService;
        private readonly IRequestService _requestService;
        private readonly IEmailService _emailService;
        private readonly IMessageReadStatusService _messageReadStatusService;
        private readonly IHubContext<MessageHub> _hubContext;

        public RequestResponseController(IRequestResponseService requestResponseService, IRequestService requestService, IEmailService emailService, IMessageReadStatusService messageReadStatusService, IHubContext<MessageHub> hubContext) : base(requestResponseService)
        {
            _requestResponseService = requestResponseService;
            _requestService = requestService;
            _emailService = emailService;
            _messageReadStatusService = messageReadStatusService;
            _hubContext = hubContext;
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<RequestResponse>>> GetByRequestId(int requestId)
        {
            // Kullanıcı ID'sini token'dan al
            var userIdClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var requestResponses = await _requestResponseService.GetByRequestIdAsync(requestId);
            
            // Her mesaj için okuma durumunu kontrol et ve ekle
            var responseWithReadStatus = new List<object>();
            foreach (var response in requestResponses)
            {
                var isRead = await _messageReadStatusService.IsMessageReadByUserAsync(response.Id, userId);
                var readStatuses = await _messageReadStatusService.GetReadStatusesByMessageIdAsync(response.Id);
                
                responseWithReadStatus.Add(new
                {
                    response.Id,
                    response.Message,
                    response.FilePath,
                    response.FileBase64,
                    response.FileName,
                    response.FileMimeType,
                    response.RequestId,
                    response.SenderId,
                    response.CreatedDate,
                    response.ModifiedDate,
                    response.IsDeleted,
                    response.Request,
                    response.Sender,
                    IsReadByCurrentUser = isRead,
                    ReadByUsers = readStatuses.Select(rs => new
                    {
                        rs.UserId,
                        rs.ReadAt,
                        User = rs.User != null ? new
                        {
                            rs.User.FirstName,
                            rs.User.LastName,
                            rs.User.Email
                        } : null
                    })
                });
            }
            
            return Ok(responseWithReadStatus);
        }

        [HttpGet("search/{message}")]
        public async Task<ActionResult<IEnumerable<RequestResponse>>> GetByMessage(string message)
        {
            var requestResponses = await _requestResponseService.GetByMessageContainingAsync(message);
            return Ok(requestResponses);
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<MessageReadStatus>>> GetUnreadMessages()
        {
            // Kullanıcı ID'sini token'dan al
            var userIdClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var unreadMessages = await _messageReadStatusService.GetUnreadMessagesByUserIdAsync(userId);
            return Ok(unreadMessages);
        }

        [HttpPost("mark-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            // Kullanıcı ID'sini token'dan al
            var userIdClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            await _messageReadStatusService.MarkMessageAsReadByUserAsync(messageId, userId);
            
            // SignalR ile mesajın okunduğunu bildir
            await _hubContext.Clients.All.SendAsync("MessageRead", new
            {
                MessageId = messageId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            });
            
            return Ok(new { message = "Message marked as read" });
        }

        [HttpPost("mark-conversation-read/{requestId}")]
        public async Task<IActionResult> MarkConversationAsRead(int requestId)
        {
            // Kullanıcı ID'sini token'dan al
            var userIdClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            await _messageReadStatusService.MarkConversationAsReadByUserAsync(requestId, userId);
            return Ok(new { message = "Conversation marked as read" });
        }

        [HttpGet("read-status/{messageId}")]
        public async Task<ActionResult<IEnumerable<MessageReadStatus>>> GetMessageReadStatus(int messageId)
        {
            var readStatuses = await _messageReadStatusService.GetReadStatusesByMessageIdAsync(messageId);
            return Ok(readStatuses);
        }

        [HttpPost]
        public override async Task<ActionResult<RequestResponse>> Create(RequestResponse entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // SenderId'yi token'dan al
            var userIdClaim = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            entity.SenderId = userId;

            var result = await base.Create(entity);

            // SignalR ile yeni mesajı bildir
            if (result.Value != null)
            {
                await _hubContext.Clients.Group($"Request_{entity.RequestId}").SendAsync("ReceiveMessage", new
                {
                    result.Value.Id,
                    result.Value.Message,
                    result.Value.FilePath, // Backward compatibility
                    result.Value.FileBase64,
                    result.Value.FileName,
                    result.Value.FileMimeType,
                    result.Value.RequestId,
                    result.Value.SenderId,
                    result.Value.CreatedDate,
                    IsReadByCurrentUser = false,
                    ReadByUsers = new List<object>()
                });
            }

            // Send notification email to request creator
            try
            {
                Console.WriteLine($"DEBUG: RequestResponse - Sending notification for RequestId: {entity.RequestId}");
                var request = await _requestService.GetByIdAsync(entity.RequestId);
                Console.WriteLine($"DEBUG: RequestResponse - Request found: {request?.Id}, RequestCreator: {request?.RequestCreator?.FirstName} {request?.RequestCreator?.LastName}");
                
                if (request != null && request.RequestCreator != null)
                {
                    var responseType = "Genel Cevap";
                    var requesterName = $"{request.RequestCreator.FirstName} {request.RequestCreator.LastName}";
                    Console.WriteLine($"DEBUG: RequestResponse - Sending email to: {request.RequestCreator.Email}, Requester: {requesterName}");
                    
                    await _emailService.SendRequestResponseNotificationAsync(
                        request.RequestCreator.Email,
                        requesterName,
                        request.Description,
                        entity.Message,
                        responseType
                    );
                    
                    Console.WriteLine("DEBUG: RequestResponse - Email sent successfully!");
                }
                else
                {
                    Console.WriteLine("DEBUG: RequestResponse - Request or RequestCreator is null!");
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the response creation
                Console.WriteLine($"DEBUG: RequestResponse - Failed to send notification email: {ex.Message}");
                Console.WriteLine($"DEBUG: RequestResponse - Stack trace: {ex.StackTrace}");
            }

            return result;
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
