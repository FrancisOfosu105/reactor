using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Comments;
using Reactor.Core.Domain.Notifications;
using Reactor.Core.Domain.Posts;
using Reactor.Services.Notifications;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.ViewModels.Comments;
using Reactor.Web.ViewModels.Home;
using Reactor.Web.ViewModels.Templates;

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
        private readonly INotificationService _notificationService;

        public HomeController(
            IPostService postService,
            IUserService userService,
            IUnitOfWork unitOfWork,
            IPhotoService photoService,
            IViewRenderService renderService,
            INotificationService notificationService
        )

        {
            _postService = postService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _renderService = renderService;
            _notificationService = notificationService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            return View(new HomeModel
            {
                UserProfilePicture = await _userService.GetUserProfilePictureAsync(),
                PostLoadMore = _postService.ShouldPostLoadMore()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeModel model)
        {
            if (!ModelState.IsValid)
            {
                model.UserProfilePicture = await _userService.GetUserProfilePictureAsync();
                model.PostLoadMore = _postService.ShouldPostLoadMore();
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
                await _photoService.UploadAsync(model.Files, post.Id);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index), "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPosts([FromForm] int pageIndex = 1)
        {
            var (posts, loadmore) = await _postService.GetPagedPostsAsync(pageIndex);

            var model = new PostTemplateModel
            {
                Posts = posts,
                LoadMore = loadmore
            };
            var postTemplate = await _renderService.RenderViewToStringAsync("Templates/_Post", model);

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
            var post = await _postService.GetPostWithUserAsync(postId);

            if (post == null)
                return NotFound();

            var currentUserId = await _userService.GetCurrentUserIdAsync();

            var comment = new Comment
            {
                Content = content,
                CommentById = currentUserId,
                PostId = post.Id,
            };

            await _postService.AddCommentToPostAsync(comment);

            await _unitOfWork.CompleteAsync();
            
            var userSetting = await _userService.GetUserSettingAsync(post.CreatedBy.Id);

            //Notify the user who created the post
            if (!post.IsForCurrentUser(currentUserId))
            {
                if (userSetting.NotifyWhenUserCommentOnPost)
                {
                    var attributes = new List<NotificationAttribute>
                    {
                        new NotificationAttribute
                        {
                            Name = "CommentId",
                            Value = comment.Id.ToString()
                        },
                        new NotificationAttribute
                        {
                            Name = "PostId",
                            Value = post.Id.ToString()
                        }
                    };
                    var notification =
                        new Notification(post.CreatedBy, currentUserId, NotificationType.Comment, attributes);

                    post.CreatedBy.CreateNotification(notification);
                
                    await _unitOfWork.CompleteAsync();
                
                    await _notificationService.PushNotification(post.CreatedBy.Id, notification.Id);
                }
            }

            await _unitOfWork.CompleteAsync();


            var model = new CommentViewModel
            {
                Comments = new List<Comment>
                {
                    comment
                },
            };

            var commentTemplate = await _renderService.RenderViewToStringAsync("Templates/_Comment", model);

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

            var (data, loadMore) = await _postService.GetPagedCommentsByPostIdAsync(postId, pageIndex);

            var model = new CommentViewModel
            {
                Comments = data,
                LoadMore = loadMore,
                PostId = postId
            };


            var commentTemplate = await _renderService.RenderViewToStringAsync("Templates/_Comment", model);

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
            var post = await _postService.GetPostWithUserAsync(postId);

            var currentUserId = await _userService.GetCurrentUserIdAsync();

            if (post == null)
                return NotFound();

            await _postService.LikePostAsync(postId);
            
            var userSetting = await _userService.GetUserSettingAsync(post.CreatedBy.Id);

            
            //Notify the user who created the post
            if (!post.IsForCurrentUser(currentUserId))
            {
                if (userSetting.NotifyWhenUserLikePost)
                {
                    var attributes = new List<NotificationAttribute>
                    {
                        new NotificationAttribute
                        {
                            Name = "PostId",
                            Value = post.Id.ToString()
                        }
                    };

                    var notification = new Notification(post.CreatedBy, currentUserId, NotificationType.Like, attributes);

                    post.CreatedBy.CreateNotification(notification);
                
                    await _unitOfWork.CompleteAsync();
                
                    await _notificationService.PushNotification(post.CreatedBy.Id, notification.Id);
                }
            }

            await _unitOfWork.CompleteAsync();


            return Json(new
            {
                totalLikes = await _postService.GetTotalPostLikesAsync(postId)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLikePost([FromForm] int postId)
        {
            var post = await _postService.GetPostWithUserAsync(postId);

            if (post == null)
                return NotFound();

            await _postService.UnLikePostAsync(postId);

            var currentUserId = await _userService.GetCurrentUserIdAsync();

            var notification =
                await _notificationService.GetNotificationAsync(post.CreatedBy.Id, currentUserId,
                    NotificationType.Like);

            post.CreatedBy.RemoveNotification(notification);

            await _unitOfWork.CompleteAsync();

            return Json(new
            {
                totalLikes = await _postService.GetTotalPostLikesAsync(postId)
            });
        }
    }
}