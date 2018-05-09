using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Reactor.Core;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Web.Infrastructure.Mvc;
using Reactor.Web.Models.Comments;
using Reactor.Web.Models.Home;
using Reactor.Web.Models.Templates;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public HomeController(
            IPostService postService,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IPhotoService photoService,
            IServiceProvider serviceProvider
        )
            : base(serviceProvider)
        {
            _postService = postService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            return View(new HomeModel
            {
                UserProfilePicture = await _userService.GetUserProfileAsync(),
                PostLoadMore = await _postService.ShouldPostLoadMoreAsync()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserProfilePicture = await _userService.GetUserProfileAsync();
                model.PostLoadMore = await _postService.ShouldPostLoadMoreAsync();
                return View(nameof(Index));
            }

            var post = new Post
            {
                Content = model.Content,
                CreatedById = await _userService.GetCurrentUserIdAsync(),
                CreatedOn = DateTime.Now
            };

            await _postService.AddPostAsync(post);
            await _unitOfWork.CompleteAsync();

            if (model.Files != null)
            {
                await _photoService.Upload(model.Files, post.Id);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index), "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPosts([FromForm] int pageIndex = 1)
        {
            var result = await _postService.GetPagedPostsAsync(pageIndex);

            var model = new PostTemplateModel
            {
                Posts = result.data,
                LoadMore = result.loadMore
            };
            var postTemplate = await RenderViewToStringAsync("Templates/Post", model);

            return Json(new
            {
                posts = postTemplate,
                loadMore = model.LoadMore
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CreateCommentModel model)
        {
            var post = await _postService.GetPostWithCommentsAsync(model.PostId);

            if (post == null)
                return NotFound();

            var user = await _userService.GetUserAsync(await _userService.GetCurrentUserIdAsync());

            var createdOn = await _postService.AddCommentToPost(post, model.Comment);

            await _unitOfWork.CompleteAsync();

            return Json(new
            {
                comment = model.Comment,
                profilePicture = await _userService.GetUserProfileAsync(),
                postId = model.PostId,
                fullName = user.FullName,
                createdOn = createdOn.ToString("o"),
                totalComments = await _postService.GetTotalCommentsForPostAsnyc(model.PostId)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PreviousComments([FromForm] int postId, [FromForm] int pageIndex = 1)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            var result = await _postService.GetPagedCommentsByPostIdAsync(postId, pageIndex);

            var model = new CommentViewModel
            {
                Comments = result.data,
                LoadMore = result.loadMore,
                PostId = postId
            };


            var commentTemplate = await RenderViewToStringAsync("Templates/Comment", model);

            return Json(new
            {
                comments = commentTemplate,
                loadMore = model.LoadMore
            });
        }
    }
}