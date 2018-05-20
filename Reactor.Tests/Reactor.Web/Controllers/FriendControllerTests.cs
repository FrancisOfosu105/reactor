using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Reactor.Core;
using Reactor.Core.Domain.Users;
using Reactor.Services.Friends;
using Reactor.Services.Notifications;
using Reactor.Services.Users;
using Reactor.Web.Controllers;

namespace Reactor.Tests.Reactor.Web.Controllers

{
    [TestFixture]
    public class FriendControllerTests
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IFriendService> _friendService;
        private Mock<IUserService> _userService;
        private Mock<INotificationService> _notificationService;
        private FriendController _friendController;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _friendService = new Mock<IFriendService>();
            _userService = new Mock<IUserService>();
            _notificationService = new Mock<INotificationService>();

            _friendController = new FriendController(_friendService.Object, _userService.Object, _unitOfWork.Object, _notificationService.Object);
        }

        [Test]
        public async Task AddFriendRequest_RequestedByUserIsNull_ReturnNotFound()
        {
            _userService.Setup(us => us.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            var result = await _friendController.AddFriendRequest(It.IsAny<string>()) as NotFoundResult;

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }


        [Test]
        public async Task AddFriendRequest_User1CreatedRequestFirstBeforeUser2_RedirectToHomePage()
        {
            _userService.Setup(us => us.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(() => new User());

            _friendService.Setup(fs => fs.FriendRequestExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _friendController.AddFriendRequest(It.IsAny<string>()) as RedirectToActionResult;

            _friendService.Verify(fs => fs.AddFriendRequestAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        }

        [Test]
        public async Task AddFriendRequest_UserCreatedRequestSuccessfully_RedirectToHomePage()
        {
            _userService.Setup(us => us.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(() => new User());

            _friendService.Setup(fs => fs.FriendRequestExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _friendController.AddFriendRequest(It.IsAny<string>()) as RedirectToActionResult;

            _friendService.Verify(fs => fs.AddFriendRequestAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
            
            
        }
        
    }
}