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
            return await _dbSet.AnyAsync(mrs => mrs.MessageId == messageId && mrs.UserId == userId && !mrs.IsDeleted);
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
            // Bu request'e ait kullanıcının göndermediği tüm mesajları al
            var messages = await _context.RequestResponses
                .Where(rr => rr.RequestId == requestId && 
                            !rr.IsDeleted && 
                            rr.SenderId.HasValue && 
                            rr.SenderId != userId)
                .Select(rr => rr.Id)
                .ToListAsync();

            // Her mesaj için okuma durumu oluştur veya güncelle
            foreach (var messageId in messages)
            {
                await MarkMessageAsReadByUserAsync(messageId, userId);
            }
        }
    }
}
