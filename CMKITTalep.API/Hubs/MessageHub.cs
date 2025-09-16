using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Collections.Concurrent;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Hubs
{
    public class MessageHub : Hub
    {
        private static readonly ConcurrentDictionary<int, string> _userConnections = new();
        private static readonly ConcurrentDictionary<int, DateTime> _userLastSeen = new();
        private static readonly ConcurrentDictionary<string, HashSet<int>> _typingUsers = new();
        private readonly IUserService _userService;

        public MessageHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = GetUserId();
                Console.WriteLine($"🟢 SignalR Connected - UserId: {userId}, ConnectionId: {Context.ConnectionId}");
                
                if (userId.HasValue)
                {
                    // Kullanıcıyı çevrimiçi olarak işaretle
                    _userConnections[userId.Value] = Context.ConnectionId;
                    _userLastSeen[userId.Value] = DateTime.UtcNow;
                    
                    // Tüm istemcilere kullanıcının çevrimiçi olduğunu bildir
                    await Clients.All.SendAsync("UserOnline", userId.Value);
                    
                    // Çevrimiçi kullanıcı listesini güncelle
                    await NotifyOnlineUsers();
                    
                    Console.WriteLine($"✅ User {userId.Value} connected to SignalR");
                }
                else
                {
                    Console.WriteLine("❌ No valid userId found - authentication failed");
                    Context.Abort();
                    return;
                }
                
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in OnConnectedAsync: {ex.Message}");
                Context.Abort();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = GetUserId();
                Console.WriteLine($"🔴 SignalR Disconnected - UserId: {userId}, ConnectionId: {Context.ConnectionId}");
                
                if (userId.HasValue)
                {
                    // Kullanıcıyı çevrimdışı olarak işaretle
                    _userConnections.TryRemove(userId.Value, out _);
                    _userLastSeen[userId.Value] = DateTime.UtcNow;
                    
                    // Tüm istemcilere kullanıcının çevrimdışı olduğunu bildir
                    await Clients.All.SendAsync("UserOffline", userId.Value);
                    
                    // Çevrimiçi kullanıcı listesini güncelle
                    await NotifyOnlineUsers();
                    
                    Console.WriteLine($"✅ User {userId.Value} disconnected from SignalR");
                }
                
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in OnDisconnectedAsync: {ex.Message}");
            }
        }

        private int? GetUserId()
        {
            try
            {
                Console.WriteLine($"🔍 GetUserId - Context.User: {Context.User != null}");
                Console.WriteLine($"🔍 GetUserId - Context.User.Identity: {Context.User?.Identity != null}");
                Console.WriteLine($"🔍 GetUserId - Context.User.Identity.IsAuthenticated: {Context.User?.Identity?.IsAuthenticated}");
                Console.WriteLine($"🔍 GetUserId - Context.User.Claims count: {Context.User?.Claims?.Count() ?? 0}");
                
                if (Context.User?.Claims != null)
                {
                    foreach (var claim in Context.User.Claims)
                    {
                        Console.WriteLine($"🔍 GetUserId - Claim: {claim.Type} = {claim.Value}");
                    }
                }
                
                // Önce ClaimTypes.NameIdentifier'ı dene
                var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
                Console.WriteLine($"🔍 GetUserId - NameIdentifier claim: {userIdClaim?.Value}");
                
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    Console.WriteLine($"🔍 GetUserId - Parsed userId from NameIdentifier: {userId}");
                    return userId;
                }
                
                // Eğer NameIdentifier yoksa, nameid claim'ini dene
                var nameIdClaim = Context.User?.FindFirst("nameid");
                Console.WriteLine($"🔍 GetUserId - nameid claim: {nameIdClaim?.Value}");
                
                if (nameIdClaim != null && int.TryParse(nameIdClaim.Value, out int nameIdUserId))
                {
                    Console.WriteLine($"🔍 GetUserId - Parsed userId from nameid: {nameIdUserId}");
                    return nameIdUserId;
                }
                
                Console.WriteLine($"🔍 GetUserId - No valid userId found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetUserId: {ex.Message}");
                return null;
            }
        }

        // Mesaj gönderme
        public async Task SendMessage(string roomName, object message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", message);
        }

        // Oda katılma
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", Context.ConnectionId);
        }

        // Odadan ayrılma
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserLeft", Context.ConnectionId);
        }

        // Mesaj okundu olarak işaretleme
        public async Task MarkMessageAsRead(string roomName, int messageId)
        {
            await Clients.Group(roomName).SendAsync("MessageRead", messageId, Context.ConnectionId);
        }

        // Yazıyor durumu
        public async Task StartTyping(string roomName)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                if (!_typingUsers.ContainsKey(roomName))
                {
                    _typingUsers[roomName] = new HashSet<int>();
                }
                _typingUsers[roomName].Add(userId.Value);
                await Clients.Group(roomName).SendAsync("UserTyping", userId.Value);
            }
        }

        public async Task StopTyping(string roomName)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                if (_typingUsers.ContainsKey(roomName))
                {
                    _typingUsers[roomName].Remove(userId.Value);
                }
                await Clients.Group(roomName).SendAsync("UserStoppedTyping", userId.Value);
            }
        }

        // Çevrimiçi kullanıcıları getir
        public async Task GetOnlineUsers()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                var onlineUsers = _userConnections.Keys.ToList();
                await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
            }
        }

        // Çevrimiçi kullanıcı listesini tüm istemcilere bildir
        private async Task NotifyOnlineUsers()
        {
            var onlineUsers = _userConnections.Keys.ToList();
            await Clients.All.SendAsync("OnlineUsers", onlineUsers);
        }

        // Kullanıcının son görülme zamanını güncelle
        public async Task UpdateLastSeen()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _userLastSeen[userId.Value] = DateTime.UtcNow;
            }
        }

        // Belirli bir kullanıcının çevrimiçi olup olmadığını kontrol et
        public bool IsUserOnline(int userId)
        {
            return _userConnections.ContainsKey(userId);
        }

        // Kullanıcının son görülme zamanını getir
        public DateTime? GetUserLastSeen(int userId)
        {
            return _userLastSeen.TryGetValue(userId, out var lastSeen) ? lastSeen : null;
        }

        // Mesaj gönderme (geliştirilmiş)
        public async Task SendMessageToUser(int targetUserId, object message)
        {
            var senderId = GetUserId();
            if (senderId.HasValue && _userConnections.TryGetValue(targetUserId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", new
                {
                    SenderId = senderId.Value,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Grup mesajı gönderme
        public async Task SendMessageToGroup(string groupName, object message)
        {
            var senderId = GetUserId();
            if (senderId.HasValue)
            {
                await Clients.Group(groupName).SendAsync("ReceiveMessage", new
                {
                    SenderId = senderId.Value,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    GroupName = groupName
                });
            }
        }

        // Yazıyor durumunu grup için gönder
        public async Task SendTypingStatus(string groupName, bool isTyping)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                if (isTyping)
                {
                    await StartTyping(groupName);
                }
                else
                {
                    await StopTyping(groupName);
                }
            }
        }

        // Grup içindeki yazıyor durumunu getir
        public async Task GetTypingUsers(string groupName)
        {
            if (_typingUsers.TryGetValue(groupName, out var typingUserIds))
            {
                await Clients.Caller.SendAsync("TypingUsers", typingUserIds.ToList());
            }
        }

        // Çevrimiçi kullanıcı ID'lerini getir (statik metod)
        public static List<int> GetOnlineUserIds()
        {
            return _userConnections.Keys.ToList();
        }

        // Kullanıcının çevrimiçi olup olmadığını kontrol et (statik metod)
        public static bool IsUserOnlineStatic(int userId)
        {
            return _userConnections.ContainsKey(userId);
        }

        // Kullanıcının son görülme zamanını getir (statik metod)
        public static DateTime? GetUserLastSeenStatic(int userId)
        {
            return _userLastSeen.TryGetValue(userId, out var lastSeen) ? lastSeen : null;
        }
    }
}