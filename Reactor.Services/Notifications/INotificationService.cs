using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Notifications;

namespace Reactor.Services.Notifications
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationAsync(string recipientId, string senderId, NotificationType type);

        Task<Notification> GetNotificationByIdAsync(int id);

        Task<IEnumerable<Notification>> GetNotificationsAsync(string userId);

        void RemoveNotification(Notification notification);

        Task PushNotification(string recipientId);
    }
}