using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Notifications;
using Reactor.Services.Follows;
using Reactor.Services.Notifications;
using Reactor.Services.Users;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFollowService _followService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public FollowController(IUserService userService, IFollowService followService, IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _userService = userService;
            _followService = followService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowUser([FromForm] string followeeUserName)
        {
            var followee = await _userService.GetUserByUserNameAsync(followeeUserName);

            if (followee == null)
                return NotFound();

            await _followService.FollowUserAsync(followee.Id);


            var attributes = new List<NotificationAttribute>
            {
                new NotificationAttribute
                {
                    Name = "Link",
                    Value = Url.Action(nameof(ProfileController.GetUserFollowers), "Profile",
                        new {username = followee.UserName})
                }
            };

            var notification = new Notification(followee, await _userService.GetCurrentUserIdAsync(),
                NotificationType.Following, attributes);

            followee.CreateNotification(notification);

            await _unitOfWork.CompleteAsync();

            await _notificationService.PushNotification(followee.Id);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnFollowUser([FromForm] string followeeUserName)
        {
            var followee = await _userService.GetUserByUserNameAsync(followeeUserName);

            if (followee == null)
                return NotFound();

            await _followService.UnFollowUserAsync(followee.Id);

            var attributes = new List<NotificationAttribute>
            {
                new NotificationAttribute
                {
                    Name = "Link",
                    Value = Url.Action(nameof(ProfileController.GetUserFollowers), "Profile",
                        new {username = followee.UserName})
                }
            };

            var notification = new Notification(followee, await _userService.GetCurrentUserIdAsync(),
                NotificationType.UnFollowed, attributes);

            followee.CreateNotification(notification);

            await _unitOfWork.CompleteAsync();

            await _notificationService.PushNotification(followee.Id);

            return Ok();
        }
    }
}