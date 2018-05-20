using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Notifications;
using Reactor.Services.Users;

namespace Reactor.Web.Components
{
    public class NotificationsViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public NotificationsViewComponent(INotificationService notificationService, IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var notifications = await _notificationService.GetNotificationsAsync(await _userService.GetCurrentUserIdAsync());

            return View(notifications);
        }
    }
}