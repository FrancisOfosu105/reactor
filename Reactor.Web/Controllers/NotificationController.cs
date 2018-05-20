using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Reactor.Core;
using Reactor.Services.Notifications;
using Reactor.Services.Users;
using Reactor.Web.Hubs;

namespace Reactor.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationController(INotificationService notificationService, IUserService userService,
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> List()
        {
            var notifications =
                await _notificationService.GetNotificationsAsync(await _userService.GetCurrentUserIdAsync());

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _unitOfWork.CompleteAsync();


            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);

            if (notification == null)
                return NotFound();

            _notificationService.RemoveNotification(notification);

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(List));
        }
       
    }
}