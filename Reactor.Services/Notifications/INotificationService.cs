using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Notifications;

namespace Reactor.Services.Notifications
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationAsync(string recipientId, string senderId, NotificationType type);

        Task<Notification> GetNotificationByIdAsync(int id);
        
        bool ShouldNotificationLoadMore(string userId);

        Task MarkAllAsReadAsync(string userId);

        Task<(IEnumerable<Notification> notifications, bool loadMore)> GetPagedNotificationsAsync(string userId, int pageIndex = 1, int pageSize = 10);

        void RemoveNotification(Notification notification);

        Task PushNotification(string recipientId, int notificationId);    

        Task<int> GetTotalUnReadNotificationsAsync(string recipientId);
    }
}