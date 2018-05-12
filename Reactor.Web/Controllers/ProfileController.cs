using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Follows;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.Models.Profile;
using Reactor.Web.Models.Templates;

namespace Reactor.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IViewRenderService _renderService;
        private readonly IPhotoService _photoService;
        private readonly IFollowService _followService;
        public ProfileController(IPostService postService, IUserService userService, IViewRenderService renderService,
            IPhotoService photoService, IFollowService followService)

        {
            _postService = postService;
            _userService = userService;
            _renderService = renderService;
            _photoService = photoService;
            _followService = followService;
        }

        // GET
        [HttpGet("{username}")]
        public async Task<IActionResult> Index(string username)
        {
            var user = await _userService.GetUserNameAsync(username);

            if (user == null)
                return NotFound();


            return View(new ProfileModel
            {
                PostLoadMore = await _postService.ShouldPostLoadMoreAsync(user.Id)
            });
        }


        [HttpPost("posts/{username}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUserPosts(string username, [FromForm] int pageIndex = 1)
        {
            var user = await _userService.GetUserNameAsync(username);
            if (user == null)
                return NotFound();

            var result = await _postService.GetUserPostsAsync(pageIndex, user.Id);

            var model = new PostTemplateModel
            {
                Posts = result.data,
                LoadMore = result.loadMore
            };
            var postTemplate = await _renderService.RenderViewToStringAsync("Templates/Post", model);

            return Json(new
            {
                posts = postTemplate,
                loadMore = model.LoadMore
            });
        }

        [HttpGet("photos/{username}")]
        public async Task<IActionResult> GetUserPhotos(string username)
        {
            var user = await _userService.GetUserNameAsync(username);

            if (user == null)
                return NotFound();

            var photos = await _photoService.GetUserPhotosAsync(user.Id);

            return View("Photos", photos);
        }


        [HttpGet("followers/{username}")]
        public async Task<IActionResult> GetUserFollowers(string username)
        {
            var user = await _userService.GetUserNameAsync(username);

            if (user == null)
                return NotFound();

            var followers = await _followService.GetUserFollowersAsync(user.Id);
            
            return View("Followers", followers);
        }

        [HttpGet("following/{username}")]
        public async Task<IActionResult> GetUserFollowees(string username)
        {
            var user = await _userService.GetUserNameAsync(username);

            if (user == null)
                return NotFound();

            var followees = await _followService.GetUserFolloweesAsync(user.Id);

            return View("Followees", followees);
        }
    }
}