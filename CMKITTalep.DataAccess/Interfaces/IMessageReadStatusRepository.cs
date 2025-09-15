using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IMessageReadStatusRepository : IGenericRepository<MessageReadStatus>
    {
        Task<bool> IsMessageReadByUserAsync(int messageId, int userId);
        Task MarkMessageAsReadByUserAsync(int messageId, int userId);
        Task<IEnumerable<MessageReadStatus>> GetReadStatusesByMessageIdAsync(int messageId);
        Task<IEnumerable<MessageReadStatus>> GetReadStatusesByUserIdAsync(int userId);
        Task<IEnumerable<MessageReadStatus>> GetUnreadMessagesByUserIdAsync(int userId);
        Task MarkConversationAsReadByUserAsync(int requestId, int userId);
    }
}
