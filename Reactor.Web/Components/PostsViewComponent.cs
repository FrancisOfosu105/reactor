using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Posts;

namespace Reactor.Web.Components
{
    public class PostsViewComponent: ViewComponent
    {
        private readonly IPostService _postService;

        public PostsViewComponent(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _postService.GetPostsAsync());
        }
    }
}