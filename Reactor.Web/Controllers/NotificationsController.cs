using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Models;
using Reactor.Services.Notifications;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.Models.Templates;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IViewRenderService _renderService;

        public NotificationsController(INotificationService notificationService, IUserService userService,
            IUnitOfWork unitOfWork, IViewRenderService renderService)
        {
            _notificationService = notificationService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _renderService = renderService;
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> List()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            
            await _notificationService.MarkAllAsReadAsync(userId);

            await _unitOfWork.CompleteAsync();

            return View(_notificationService.ShouldNotificationLoadMore(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadNotifications(int pageIndex, int pageSize, NotificationTemplateType type)
        {
            var (notifications, loadMore) = await _notificationService.GetPagedNotificationsAsync(
                await _userService.GetCurrentUserIdAsync(),
                pageIndex, pageSize);

            var model = new NotificationTemplateModel
            {
                LoadMore = loadMore,
                Notifications = notifications
            };

            string template;

            switch (type)
            {
                case NotificationTemplateType.Main:
                    template = await _renderService.RenderViewToStringAsync("Templates/_MainNotification", model);
                    break;
                case NotificationTemplateType.Mini:

                    var liCollection = new List<string>();

                    foreach (var notification in notifications)
                    {
                        liCollection.Add(
                            await _renderService.RenderViewToStringAsync("Templates/_MiniNotification", notification));
                    }

                    template = string.Join("", liCollection);
                    break;
                
                default:
                    throw new InvalidOperationException(nameof(type));
            }

            return Json(new
            {
                notifications = template,
                loadMore,
                type
            });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(await _userService.GetCurrentUserIdAsync());

            await _unitOfWork.CompleteAsync();
            
            return NoContent();
        }
    }
}