using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.API.Attributes;
using CMKITTalep.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireAuthenticated]
    public class ChatController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IUserService _userService;
        private readonly IHubContext<MessageHub> _hubContext;

        public ChatController(
            IRequestService requestService,
            IUserService userService,
            IHubContext<MessageHub> hubContext)
        {
            _requestService = requestService;
            _userService = userService;
            _hubContext = hubContext;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Kullanıcının mesajlaşabileceği talepleri getirir
        /// </summary>
        [HttpGet("requests")]
        public async Task<IActionResult> GetChatRequests()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                // Kullanıcının kendi taleplerini ve yetkili olduğu talepleri getir
                var requests = await _requestService.GetUserChatRequestsAsync(userId.Value);
                
                return Ok(new { success = true, data = requests });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Belirli bir talebin mesaj geçmişini getirir
        /// </summary>
        [HttpGet("requests/{requestId}/messages")]
        public async Task<IActionResult> GetRequestMessages(int requestId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                // Talep yetkisi kontrolü
                var hasAccess = await _requestService.UserHasAccessToRequestAsync(userId.Value, requestId);
                if (!hasAccess)
                {
                    return Forbid("Bu talebe erişim yetkiniz yok");
                }

                var messages = await _requestService.GetRequestMessagesAsync(requestId);
                
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Talebe mesaj gönderir
        /// </summary>
        [HttpPost("requests/{requestId}/messages")]
        public async Task<IActionResult> SendRequestMessage(int requestId, [FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                // Talep yetkisi kontrolü
                var hasAccess = await _requestService.UserHasAccessToRequestAsync(userId.Value, requestId);
                if (!hasAccess)
                {
                    return Forbid("Bu talebe erişim yetkiniz yok");
                }

                // Mesajı kaydet
                var message = await _requestService.AddRequestMessageAsync(requestId, userId.Value, request.Message, request.FilePath);

                // SignalR ile gerçek zamanlı bildirim gönder
                var roomName = $"request_{requestId}";
                await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", new
                {
                    Id = message.Id,
                    SenderId = userId.Value,
                    SenderName = message.Sender != null ? $"{message.Sender.FirstName} {message.Sender.LastName}" : "Bilinmeyen",
                    Message = message.Message,
                    FilePath = message.FilePath,
                    Timestamp = message.CreatedDate,
                    RequestId = requestId
                });

                return Ok(new { success = true, data = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Mesajı okundu olarak işaretler
        /// </summary>
        [HttpPost("messages/{messageId}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                await _requestService.MarkMessageAsReadAsync(messageId, userId.Value);
                
                return Ok(new { success = true, message = "Mesaj okundu olarak işaretlendi" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcının çevrimiçi olup olmadığını kontrol eder
        /// </summary>
        [HttpGet("users/{userId}/online")]
        public async Task<IActionResult> IsUserOnline(int userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                // Kullanıcı varlığını kontrol et
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // SignalR hub'ından çevrimiçi durumu kontrol et
                var isOnline = MessageHub.IsUserOnlineStatic(userId);
                var lastSeen = MessageHub.GetUserLastSeenStatic(userId);

                return Ok(new { 
                    success = true, 
                    data = new { 
                        isOnline, 
                        lastSeen 
                    } 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Çevrimiçi kullanıcıları getirir
        /// </summary>
        [HttpGet("users/online")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Kullanıcı kimliği bulunamadı" });
                }

                var onlineUserIds = MessageHub.GetOnlineUserIds();
                var users = await _userService.GetUsersByIdsAsync(onlineUserIds);

                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Sunucu hatası: " + ex.Message });
            }
        }
    }

    public class SendMessageRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? FilePath { get; set; }
    }
}
