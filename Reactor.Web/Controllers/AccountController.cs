using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reactor.Core;
using Reactor.Core.Domain.Users;
using Reactor.Services.Photos;
using Reactor.Services.Users;
using Reactor.Web.ViewModels.Account;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IPhotoService _photoService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
            UserManager<User> userManager,
            ILogger<AccountController> logger,
            SignInManager<User> signInManager, IPhotoService photoService, IUserService userService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
            _photoService = photoService;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(HomeController.Index), "Home");

            //clear existing cookies to ensure a clean login process
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                model.ReturnUrl = returnUrl;
                return View(model);
            }

            var member = await _userManager.FindByNameAsync(model.UserName) ??
                         await _userManager.FindByEmailAsync(model.UserName);

            if (member == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                model.ReturnUrl = returnUrl;
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(member, model.Password,
                isPersistent: model.RemmemberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Member with username: {model.UserName} has logged in.");

                return !string.IsNullOrEmpty(returnUrl)
                    ? RedirectToLocal(returnUrl)
                    : RedirectToAction(nameof(HomeController.Index), "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");

            model.ReturnUrl = returnUrl;

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(HomeController.Index), "Home");

            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                model.ReturnUrl = returnUrl;

                return View();
            }

            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.EmailAddress
            };

            if (model.File != null)
            {
                var photoLocation = await _photoService.UploadAsync(model.File);
                user.ProfilePictureUrl = photoLocation;
            }


            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
             
                var userSetting = new UserSetting
                {
                    NotifyWhenUserAcceptFriendRequest = true,
                    NotifyWhenUserCommentOnPost = true,
                    NotifyWhenUserFollow = true,
                    NotifyWhenUserLikePost = true,
                    NotifyWhenUserRejectFriendRequest = true,
                    NotifyWhenUserSendFriendRequest = true,
                    NotifyWhenUserUnFollow = true,
                    UserId = user.Id
                };

                await _userService.InsertUserSettingAsync(userSetting);
                
                await _unitOfWork.CompleteAsync();
                
                _logger.LogInformation($"User with username: {model.UserName} has been registered.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return !string.IsNullOrEmpty(returnUrl)
                    ? RedirectToLocal(returnUrl)
                    : RedirectToAction(nameof(HomeController.Index), "Home");
            }

            AddErrors(result);

            model.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }
    }
}