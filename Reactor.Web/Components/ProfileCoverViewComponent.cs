using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Web.Models.Profile;

namespace Reactor.Web.Components
{
    public class ProfileCoverViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IPostService _postService;

        public ProfileCoverViewComponent(IUserService userService, IPhotoService photoService, IPostService postService)
        {
            _userService = userService;
            _photoService = photoService;
            _postService = postService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string username)
        {
            var user = await _userService.GetUserNameAsync(username);

         
                var model = new ProfileCoverModel
                {
                    FullName = user.FullName,
                    ProfilePicture = user.GetPicture(),
                    TotalPhotos = await _photoService.GetUserTotalPhotosAsync(user.Id),
                    TotalPosts = await _postService.GetUserTotalPostsAsync(user.Id)
                };

            return View(model);

        }
    }
}