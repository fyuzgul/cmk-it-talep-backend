using Microsoft.EntityFrameworkCore;
using CMKITTalep.DataAccess.Context;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Repositories
{
    public class MessageReadStatusRepository : GenericRepository<MessageReadStatus>, IMessageReadStatusRepository
    {
        public MessageReadStatusRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsMessageReadByUserAsync(int messageId, int userId)
        {
            var result = await _dbSet.AnyAsync(mrs => mrs.MessageId == messageId && mrs.UserId == userId && !mrs.IsDeleted);
            Console.WriteLine($"DEBUG Repository: IsMessageReadByUserAsync - messageId: {messageId}, userId: {userId}, result: {result}");
            return result;
        }

        public async Task MarkMessageAsReadByUserAsync(int messageId, int userId)
        {
            var existingReadStatus = await _dbSet.FirstOrDefaultAsync(mrs => mrs.MessageId == messageId && mrs.UserId == userId && !mrs.IsDeleted);
            
            if (existingReadStatus == null)
            {
                var readStatus = new MessageReadStatus
                {
                    MessageId = messageId,
                    UserId = userId,
                    ReadAt = DateTime.UtcNow
                };
                await _dbSet.AddAsync(readStatus);
            }
            else
            {
                existingReadStatus.ReadAt = DateTime.UtcNow;
                existingReadStatus.ModifiedDate = DateTime.UtcNow;
            }
            
            // Veritabanına kaydet
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageReadStatus>> GetReadStatusesByMessageIdAsync(int messageId)
        {
            return await _dbSet.Include(mrs => mrs.User)
                              .Where(mrs => mrs.MessageId == messageId && !mrs.IsDeleted)
                              .ToListAsync();
        }

        public async Task<IEnumerable<MessageReadStatus>> GetReadStatusesByUserIdAsync(int userId)
        {
            return await _dbSet.Include(mrs => mrs.Message)
                              .Where(mrs => mrs.UserId == userId && !mrs.IsDeleted)
                              .ToListAsync();
        }

        public async Task<IEnumerable<MessageReadStatus>> GetUnreadMessagesByUserIdAsync(int userId)
        {
            // Bu metod, kullanıcının okumadığı mesajları bulmak için kullanılacak
            // RequestResponse tablosundan kullanıcının göndermediği mesajları al
            // ve bunların MessageReadStatus tablosunda kaydı olmayanları döndür
            var unreadMessages = await _context.RequestResponses
                .Where(rr => !rr.IsDeleted && 
                            rr.SenderId.HasValue && 
                            rr.SenderId != userId &&
                            !_dbSet.Any(mrs => mrs.MessageId == rr.Id && mrs.UserId == userId && !mrs.IsDeleted))
                .Include(rr => rr.Sender)
                .Include(rr => rr.Request)
                .OrderBy(rr => rr.CreatedDate)
                .ToListAsync();

            // MessageReadStatus formatında döndür
            return unreadMessages.Select(rr => new MessageReadStatus
            {
                MessageId = rr.Id,
                UserId = userId,
                Message = rr,
                ReadAt = DateTime.MinValue // Okunmamış mesajlar için
            });
        }

        public async Task MarkConversationAsReadByUserAsync(int requestId, int userId)
        {
            Console.WriteLine($"DEBUG Repository: MarkConversationAsReadByUserAsync called - requestId: {requestId}, userId: {userId}");
            
            // Bu request'e ait sadece kullanıcının göndermediği mesajları al (karşı tarafın mesajları)
            var messages = await _context.RequestResponses
                .Where(rr => rr.RequestId == requestId && 
                            !rr.IsDeleted && 
                            rr.SenderId.HasValue &&
                            rr.SenderId != userId) // Sadece karşı tarafın mesajları
                .Select(rr => rr.Id)
                .ToListAsync();
            
            Console.WriteLine($"DEBUG Repository: Found {messages.Count} messages to mark as read");

            // Mevcut okuma durumlarını al
            var existingReadStatuses = await _dbSet
                .Where(mrs => messages.Contains(mrs.MessageId) && mrs.UserId == userId && !mrs.IsDeleted)
                .ToListAsync();

            var existingMessageIds = existingReadStatuses.Select(mrs => mrs.MessageId).ToHashSet();
            var newReadStatuses = new List<MessageReadStatus>();

            // Yeni okuma durumları oluştur
            foreach (var messageId in messages)
            {
                if (!existingMessageIds.Contains(messageId))
                {
                    newReadStatuses.Add(new MessageReadStatus
                    {
                        MessageId = messageId,
                        UserId = userId,
                        ReadAt = DateTime.UtcNow
                    });
                }
                else
                {
                    // Mevcut kaydı güncelle
                    var existing = existingReadStatuses.First(mrs => mrs.MessageId == messageId);
                    existing.ReadAt = DateTime.UtcNow;
                    existing.ModifiedDate = DateTime.UtcNow;
                }
            }

            // Yeni kayıtları ekle
            if (newReadStatuses.Any())
            {
                await _dbSet.AddRangeAsync(newReadStatuses);
            }

            // Veritabanına kaydet
            await _context.SaveChangesAsync();
            Console.WriteLine($"DEBUG Repository: Successfully saved {newReadStatuses.Count} new read statuses and updated {existingReadStatuses.Count} existing ones");
        }
    }
}
