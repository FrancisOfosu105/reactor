using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reactor.Services.Posts;
using Reactor.Web.Models.Profile;

namespace Reactor.Web.Controllers
{
    
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly IPostService _postService;

        public ProfileController(IPostService postService)
        {
            _postService = postService;
        }

        // GET
        [HttpGet("{username}")]
        public async Task<IActionResult> Index(string username)
        {
            
            return View(new ProfileModel
            {
                PostLoadMore = await _postService.ShouldPostLoadMoreAsync()
            });    
        }
    }
}