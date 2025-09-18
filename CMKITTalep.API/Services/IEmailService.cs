namespace CMKITTalep.API.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string resetToken);
        Task SendRequestResponseNotificationAsync(string requesterEmail, string requesterName, string requestDescription, string responseMessage, string responseType);
        Task SendNewRequestNotificationAsync(List<string> supportEmails, string requesterName, string requestDescription, string requestType, string supportType, bool isCCNotification = false);
        Task SendRequestUpdateNotificationAsync(string requesterEmail, string requesterName, string requestDescription, string oldStatus, string newStatus, string updateType);
    }
}
