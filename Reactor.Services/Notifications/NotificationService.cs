using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Notifications;
using Reactor.Core.Domain.Users;
using Reactor.Core.Hubs;
using Reactor.Core.Repository;
using Reactor.Services.ViewRender;

namespace Reactor.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IViewRenderService _renderService;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IRepository<Notification> _notificationRepository;

        public NotificationService(IRepository<Notification> notificationRepository, IViewRenderService renderService,
            IHubContext<NotificationHub> hub)
        {
            _notificationRepository = notificationRepository;
            _renderService = renderService;
            _hub = hub;
        }

        public async Task<Notification> GetNotificationAsync(string recipientId, string senderId, NotificationType type)
        {
            return await _notificationRepository.Table.FirstOrDefaultAsync(n =>
                n.RecipientId == recipientId && n.SenderId == senderId && n.Type == type);
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _notificationRepository.Table.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync(string userId)
        {
            return await _notificationRepository.Table
                .Include(n => n.Attributes)
                .OrderByDescending(n => n.CreatedOn)
                .Where(n => n.RecipientId == userId).ToListAsync();
        }

        public void RemoveNotification(Notification notification)
        {
            _notificationRepository.Remove(notification);
        }

        public async Task PushNotification(string recipientId)
        {

            var model = await GetNotificationsAsync(recipientId);

            var notifications = await _renderService.RenderViewToStringAsync("Templates/_Notification", model);

            await _hub.Clients.User(recipientId).SendAsync("notify", notifications);
        }
    }
}