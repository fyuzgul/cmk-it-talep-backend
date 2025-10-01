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

        // ⚠️ DISABLED: GetAll endpoint - tüm mesajları yüklemek performans sorunu yaratır
        // Bunun yerine GetByRequestId endpoint'ini kullanın
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<ActionResult<IEnumerable<RequestResponse>>> GetAll()
        {
            return BadRequest(new { 
                message = "Bu endpoint devre dışı bırakıldı. Lütfen /api/RequestResponse/request/{requestId} endpoint'ini kullanın.",
                reason = "Performance optimization - loading all messages causes 20+ second delays"
            });
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
            
            // Debug: Sender bilgilerini kontrol et
            Console.WriteLine($"DEBUG: Found {requestResponses.Count()} responses for request {requestId}");
            foreach (var response in requestResponses)
            {
                Console.WriteLine($"Response {response.Id}: SenderId={response.SenderId}, Sender={response.Sender?.FirstName} {response.Sender?.LastName}");
            }
            
            // Her mesaj için okuma durumunu kontrol et ve ekle
            var responseWithReadStatus = new List<object>();
            foreach (var response in requestResponses)
            {
                // Sadece kendi mesajları için okundu durumunu kontrol et
                var isOwnMessage = response.SenderId == userId;
                var isRead = false;
                var readStatuses = new List<object>();
                
                if (isOwnMessage)
                {
                    // Kendi mesajı için okundu durumunu kontrol et
                    // Kullanıcının mesajları için: support tarafından okunmuş mu?
                    // Support'un mesajları için: kullanıcı tarafından okunmuş mu?
                    var otherUserId = 0;
                    if (response.Request != null)
                    {
                        if (userId == response.Request.RequestCreatorId)
                        {
                            // Kullanıcının mesajı - support tarafından okunmuş mu?
                            otherUserId = response.Request.SupportProviderId ?? 0;
                        }
                        else if (userId == response.Request.SupportProviderId)
                        {
                            // Support'un mesajı - kullanıcı tarafından okunmuş mu?
                            otherUserId = response.Request.RequestCreatorId;
                        }
                    }
                    
                    if (otherUserId != 0 && otherUserId != userId)
                    {
                        isRead = await _messageReadStatusService.IsMessageReadByUserAsync(response.Id, otherUserId);
                    }
                    Console.WriteLine($"DEBUG Controller: Message {response.Id} - isOwnMessage: {isOwnMessage}, isRead: {isRead}, otherUserId: {otherUserId}");
                    var readStatusesData = await _messageReadStatusService.GetReadStatusesByMessageIdAsync(response.Id);
                    readStatuses = readStatusesData.Select(rs => new
                    {
                        rs.UserId,
                        rs.ReadAt,
                        User = rs.User != null ? new
                        {
                            rs.User.FirstName,
                            rs.User.LastName,
                            rs.User.Email
                        } : null
                    }).Cast<object>().ToList();
                }
                
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
                    IsOwnMessage = isOwnMessage,
                    IsReadByCurrentUser = isRead,
                    ReadByUsers = readStatuses
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

            // Mesajın sahibini kontrol et - sadece kendi mesajları için okundu durumu işaretlenebilir
            var message = await _requestResponseService.GetByIdAsync(messageId);
            if (message == null)
            {
                return NotFound(new { message = "Message not found" });
            }

            if (message.SenderId != userId)
            {
                return BadRequest(new { message = "You can only mark your own messages as read" });
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

            Console.WriteLine($"DEBUG: Marking conversation {requestId} as read for user {userId}");
            await _messageReadStatusService.MarkConversationAsReadByUserAsync(requestId, userId);
            Console.WriteLine($"DEBUG: Conversation {requestId} marked as read successfully for user {userId}");
            
            // SignalR ile konuşmanın okunduğunu bildir
            Console.WriteLine($"DEBUG: Sending ConversationRead SignalR event for RequestId: {requestId}, UserId: {userId}");
            await _hubContext.Clients.Group($"request_{requestId}").SendAsync("ConversationRead", new
            {
                RequestId = requestId,
                UserId = userId,
                ReadAt = DateTime.UtcNow
            });
            Console.WriteLine($"DEBUG: ConversationRead SignalR event sent successfully");
            
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
                // Yeni mesaj için okundu durumunu kontrol et
                var isRead = false;
                var readStatuses = new List<object>();
                
                // Mesajın sahibi için okundu durumunu kontrol et
                if (result.Value.Request != null)
                {
                    var otherUserId = 0;
                    if (userId == result.Value.Request.RequestCreatorId)
                    {
                        // Kullanıcının mesajı - support tarafından okunmuş mu?
                        otherUserId = result.Value.Request.SupportProviderId ?? 0;
                    }
                    else if (userId == result.Value.Request.SupportProviderId)
                    {
                        // Support'un mesajı - kullanıcı tarafından okunmuş mu?
                        otherUserId = result.Value.Request.RequestCreatorId;
                    }
                    
                    if (otherUserId != 0 && otherUserId != userId)
                    {
                        isRead = await _messageReadStatusService.IsMessageReadByUserAsync(result.Value.Id, otherUserId);
                    }
                    
                    var readStatusesData = await _messageReadStatusService.GetReadStatusesByMessageIdAsync(result.Value.Id);
                    readStatuses = readStatusesData.Select(rs => new
                    {
                        rs.UserId,
                        rs.ReadAt,
                        User = rs.User != null ? new
                        {
                            rs.User.FirstName,
                            rs.User.LastName,
                            rs.User.Email
                        } : null
                    }).Cast<object>().ToList();
                }

                await _hubContext.Clients.Group($"request_{entity.RequestId}").SendAsync("ReceiveMessage", new
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
                    IsOwnMessage = true, // Yeni gönderilen mesaj kendi mesajı
                    IsReadByCurrentUser = isRead,
                    ReadByUsers = readStatuses
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
