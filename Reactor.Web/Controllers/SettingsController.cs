using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reactor.Core;
using Reactor.Core.Domain.Users;
using Reactor.Services.Photos;
using Reactor.Services.Users;
using Reactor.Web.ViewModels.SettingsViewModel;

namespace Reactor.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingsController(IUserService userService, IMapper mapper, IUnitOfWork unitOfWork,
            IPhotoService photoService)
        {
            _userService = userService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _photoService = photoService;
        }


        [HttpGet]
        public async Task<IActionResult> Basic()
        {
            var user = await _userService.GetUserByIdAsync(await _userService.GetCurrentUserIdAsync());
            var model = _mapper.Map<User, BasicViewModel>(user);
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Basic(BasicViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetUserByIdAsync(await _userService.GetCurrentUserIdAsync());

            _mapper.Map(model, user);

            if (model.ProfilePicture != null)
            {
               user.ProfilePictureUrl= await UploadPhotoAsync(model.ProfilePicture, user.ProfilePictureUrl);
            }

            if (model.ProfileCoverPicture != null)
            {
              user.ProfileCoverPictureUrl=  await UploadPhotoAsync(model.ProfileCoverPicture, user.ProfileCoverPictureUrl);
            }

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Basic));
        }

        private async Task<string> UploadPhotoAsync(IFormFile profileCoverPicture, string url)
        {
            _photoService.RemovePhotoFromDisk(url);

          return await _photoService.UploadAsync(profileCoverPicture);
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var userSetting = await _userService.GetUserSettingUserIdAsync();

            var model = _mapper.Map<UserSetting, NotificationViewModel>(userSetting);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Notifications(NotificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userSetting = await _userService.GetUserSettingUserIdAsync();

            _mapper.Map(model, userSetting);

            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Notifications));
        }
    }
}