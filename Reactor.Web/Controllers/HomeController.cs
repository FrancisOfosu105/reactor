using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Posts;
using Reactor.Services.Photos;
using Reactor.Services.Posts;
using Reactor.Services.Users;
using Reactor.Web.Models.Home;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPhotoService _photoService;

        public HomeController(IPostService postService, IUserService userService, IUnitOfWork unitOfWork,
            IPhotoService photoService)
        {
            _postService = postService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }

        // GET
        public IActionResult Index()
        {
            return View(new HomeModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index));

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
    }
}