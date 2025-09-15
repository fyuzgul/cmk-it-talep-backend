using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class MessageReadStatusService : GenericService<MessageReadStatus>, IMessageReadStatusService
    {
        private readonly IMessageReadStatusRepository _messageReadStatusRepository;

        public MessageReadStatusService(IMessageReadStatusRepository messageReadStatusRepository) : base(messageReadStatusRepository)
        {
            _messageReadStatusRepository = messageReadStatusRepository;
        }

        public async Task<bool> IsMessageReadByUserAsync(int messageId, int userId)
        {
            return await _messageReadStatusRepository.IsMessageReadByUserAsync(messageId, userId);
        }

        public async Task MarkMessageAsReadByUserAsync(int messageId, int userId)
        {
            await _messageReadStatusRepository.MarkMessageAsReadByUserAsync(messageId, userId);
        }

        public async Task<IEnumerable<MessageReadStatus>> GetReadStatusesByMessageIdAsync(int messageId)
        {
            return await _messageReadStatusRepository.GetReadStatusesByMessageIdAsync(messageId);
        }

        public async Task<IEnumerable<MessageReadStatus>> GetReadStatusesByUserIdAsync(int userId)
        {
            return await _messageReadStatusRepository.GetReadStatusesByUserIdAsync(userId);
        }

        public async Task<IEnumerable<MessageReadStatus>> GetUnreadMessagesByUserIdAsync(int userId)
        {
            return await _messageReadStatusRepository.GetUnreadMessagesByUserIdAsync(userId);
        }

        public async Task MarkConversationAsReadByUserAsync(int requestId, int userId)
        {
            await _messageReadStatusRepository.MarkConversationAsReadByUserAsync(requestId, userId);
        }
    }
}
