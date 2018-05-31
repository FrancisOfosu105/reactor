using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Notifications;
using Reactor.Services.Friends;
using Reactor.Services.Notifications;
using Reactor.Services.Users;

namespace Reactor.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public FriendController(IFriendService friendService, IUserService userService, IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _friendService = friendService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        // GET
        public async Task<IActionResult> List()
        {
            var myFriends = await _friendService.GetApprovedFriends();
            return View(myFriends);
        }

        public async Task<IActionResult> FriendRequests()
        {
            var friendRequests = await _friendService.GetFriendRequests();
            return View(friendRequests);
        }


        [HttpPost("{requestedToId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriendRequest(string requestedToId)
        {
            if (await _userService.GetUserByIdAsync(requestedToId) == null)
                return NotFound();

            var requestedById = await _userService.GetCurrentUserIdAsync();


            var requestedTo = await _userService.GetUserByIdAsync(requestedToId);

            if (requestedTo == null)
                return NotFound();

            if (await _friendService.FriendRequestExistsAsync(requestedById, requestedToId))
                return RedirectToAction(nameof(HomeController.Index), "Home");


            await _friendService.AddFriendRequestAsync(requestedById,
                (await _userService.GetUserByIdAsync(requestedToId)).Id);

            var userSetting = await _userService.GetUserSettingAsync(requestedTo.Id);

            if (!userSetting.NotifyWhenUserSendFriendRequest)
                return RedirectToAction(nameof(HomeController.Index), "Home");
            
            var attributes = new List<NotificationAttribute>
            {
                new NotificationAttribute
                {
                    Name = "Link",
                    Value = Url.Action(nameof(FriendController.FriendRequests), "Friend")
                }
            };

            var notification =
                new Notification(requestedTo, requestedById, NotificationType.SentFriendRequest, attributes);

            requestedTo.CreateNotification(notification);

            await _unitOfWork.CompleteAsync();

            await _notificationService.PushNotification(requestedTo.Id, notification.Id);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost("{requestedById}/{requestedToId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptFriendRequest(string requestedById, string requestedToId)
        {
            await _friendService.AcceptFriendRequestAsync(requestedById, requestedToId);

            var requestedBy = await _userService.GetUserByIdAsync(requestedById);

            var userSetting = await _userService.GetUserSettingAsync(requestedBy.Id);

            if (!userSetting.NotifyWhenUserAcceptFriendRequest)
                return RedirectToAction(nameof(List));

            var attributes = new List<NotificationAttribute>
            {
                new NotificationAttribute
                {
                    Name = "Link",
                    Value = Url.Action(nameof(FriendController.List), "Friend")
                }
            };

            var notification = new Notification(requestedBy, requestedToId, NotificationType.AcceptedFriendRequest,
                attributes);

            requestedBy.CreateNotification(notification);


            await _unitOfWork.CompleteAsync();

            await _notificationService.PushNotification(requestedBy.Id, notification.Id);

            return RedirectToAction(nameof(List));
        }
    }
}