using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Posts;
using Reactor.Web.ViewModels.Comments;

namespace Reactor.Web.Components
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly IPostService _postService;

        public CommentsViewComponent(IPostService postService)
        {
            _postService = postService;
        }
        // GET
        public async Task<IViewComponentResult> InvokeAsync(int postId)
        {
            var result = await _postService.GetPagedCommentsByPostIdAsync(postId);
            
            return View(new CommentViewModel
            {
                Comments = result.data,
                LoadMore = result.loadMore,
                PostId = postId
            });
        }
    }
}