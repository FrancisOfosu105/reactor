using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Posts;
using Reactor.Web.Models.Comments;

namespace Reactor.Web.Components
{
    public class CommentsViewComponent:ViewComponent
    {
        private readonly IPostService _postService;
        public CommentsViewComponent(IPostService postService)
        {
            _postService = postService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(int postId)
        {
            return View(new CommentViewModel
            {
                Comments = await _postService.GetCommentsByIdAsync(postId),
                PostId = postId
            });
        }
    }
}