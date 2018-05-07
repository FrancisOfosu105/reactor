using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Services.Friends;
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

        public FriendController(IFriendService friendService, IUserService userService, IUnitOfWork unitOfWork)
        {
            _friendService = friendService;
            _userService = userService;
            _unitOfWork = unitOfWork;
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
            if (await _userService.GetUserAsync(requestedToId) == null)
                return NotFound();

            var requestedById = await _userService.GetCurrentUserIdAsync();


            if (await _friendService.FriendRequestExistsAsync(requestedById, requestedToId))
                return RedirectToAction(nameof(HomeController.Index), "Home");


            await _friendService.AddFriendRequestAsync(requestedById, (await _userService.GetUserAsync(requestedToId)).Id);

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost("{requestedById}/{requestedToId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptFriendRequest(string requestedById, string requestedToId)
        {
            await _friendService.AcceptFriendRequestAsync(requestedById, requestedToId);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(List));
        }
    }
}