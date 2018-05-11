using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Posts;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.Models.Comments;
using Reactor.Web.Models.Home;
using Reactor.Web.Models.Templates;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;
        private readonly IViewRenderService _renderService;

        public HomeController(
            IPostService postService,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IPhotoService photoService, IViewRenderService renderService)

        {
            _postService = postService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _renderService = renderService;
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
            var postTemplate = await _renderService.RenderViewToStringAsync("Templates/Post", model);

            return Json(new
            {
                posts = postTemplate,
                loadMore = model.LoadMore
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] int postId, [FromForm] string content)    
        {
            var post = await _postService.GetPostWithCommentsAsync(postId);

            if (post == null)
                return NotFound();

            var user = await _userService.GetUserByIdAsync(await _userService.GetCurrentUserIdAsync());

            var comment = new Comment
            {
                Content = content,
                CreatedOn = DateTime.Now,
                CommentById = user.Id,
                PostId = post.Id,
                CommentBy = user
            };

            await _postService.AddCommentToPostAsync(comment);

            await _unitOfWork.CompleteAsync();

            var model = new CommentViewModel
            {
                Comments = new List<Comment>
                {
                    comment
                },
            };

            var commentTemplate = await _renderService.RenderViewToStringAsync("Templates/Comment", model);

            return Json(new
            {
                postId = comment.PostId,
                totalComments = await _postService.GetTotalCommentsForPostAsnyc(comment.PostId),
                comment = commentTemplate
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


            var commentTemplate = await _renderService.RenderViewToStringAsync("Templates/Comment", model);

            return Json(new
            {
                comments = commentTemplate,
                loadMore = model.LoadMore
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost([FromForm] int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            await _postService.LikePostAsync(postId);

            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                totalLikes = await _postService.GetTotalPostLikesExceptCurrentUserAsync(postId)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLikePost([FromForm] int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            await _postService.UnLikePostAsync(postId);

            await _unitOfWork.CompleteAsync();

            return Ok(new
            {
                totalLikes = await _postService.GetTotalPostLikesExceptCurrentUserAsync(postId)
            });
        }
    }
}