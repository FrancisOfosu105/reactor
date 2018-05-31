using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Follows;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Web.ViewModels.Profile;

namespace Reactor.Web.Components
{
    public class ProfileCoverViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IPostService _postService;
        private readonly IFollowService _followService;

        public ProfileCoverViewComponent(IUserService userService, IPhotoService photoService, IPostService postService, IFollowService followService)
        {
            _userService = userService;
            _photoService = photoService;
            _postService = postService;
            _followService = followService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            var user = await _userService.GetUserByUserNameAsync(username);

         
                var model = new ProfileCoverViewModel
                {
                    FullName = user.FullName,
                    ProfilePicture = user.GetProfilePicture(),
                    TotalPhotos = await _photoService.GetUserTotalPhotosAsync(user.Id),
                    TotalPosts = await _postService.GetUserTotalPostsAsync(user.Id),
                    IsFollowingUser = await _followService.IsFollowingUserAsync(user.Id),
                    TotalFollowees = await _followService.GetUserTotalFolloweesAsync(user.Id),
                    TotalFollowers = await _followService.GetUserTotalFollowersAsync(user.Id),
                    Description = user.Description,
                    ProfileCoverPicture = user.GetProfileCoverPicture()
                };

            return View(model);

        }
    }
}