using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Services.Follows;
using Reactor.Services.Users;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFollowService _followService;
        private readonly IUnitOfWork _unitOfWork;

        public FollowController(IUserService userService,IFollowService followService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _followService = followService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FollowUser([FromForm] string followeeUserName)
        {
            var followee = await _userService.GetUserNameAsync(followeeUserName);

            if (followee == null)
                return NotFound();

            await _followService.FollowUserAsync(followee.Id);

            await _unitOfWork.CompleteAsync();
            
            return Ok();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnFollowUser([FromForm] string followeeUserName)    
        {
            var followee = await _userService.GetUserNameAsync(followeeUserName);

            if (followee == null)
                return NotFound();

            await _followService.UnFollowUserAsync(followee.Id);

            await _unitOfWork.CompleteAsync();
            
            return Ok();
        }
    }
}