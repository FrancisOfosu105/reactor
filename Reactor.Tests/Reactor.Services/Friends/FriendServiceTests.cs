using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Reactor.Core.Domain.Friends;
using Reactor.Core.Domain.Users;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;
using Reactor.Data.Repository;
using Reactor.Services.Friends;
using Reactor.Services.Users;
using Reactor.Tests.Helper;

namespace Reactor.Tests.Reactor.Services.Friends
{
    [TestFixture]
    public class FriendServiceTests
    {
        private Mock<IRepository<Friend>> _friendRepository;
        private Mock<IUserService> _userService;
        private FriendService _friendService;
        private User _currentUser;
        private User _otherUser1;
        private User _otherUser2;
        private User _otherUser3;
        private Friend _approvedfriend;
        private Friend _notapprovedfriend;

        [SetUp]
        public void SetUp()
        {
            _friendRepository = new Mock<IRepository<Friend>>();
            _userService = new Mock<IUserService>();

            _friendService = new FriendService(_friendRepository.Object, _userService.Object);


            _otherUser1 = new User {Id = "2", UserName = "OtherUser1"};
            _otherUser2 = new User {Id = "3", UserName = "OtherUser2"};
            _otherUser3 = new User {Id = "4", UserName = "OtherUser3"};

            _currentUser = new User
            {
                Id = "1",
                UserName = "CurrentUser"
            };

            _approvedfriend = new Friend()
            {
                BecameFriendsOn = DateTime.Now,
                RequestedById = _currentUser.Id,
                RequestedToId = _otherUser1.Id,
                Status = FriendRequestType.Approved
            };

            _notapprovedfriend = new Friend
            {
                RequestedOn = DateTime.Now,
                RequestedById = _otherUser1.Id,
                RequestedToId = _currentUser.Id,
                Status = FriendRequestType.None
            };
        }

        [Test]
        public async Task GetSuggestedFriends_UserHasNoFriends_ReturnAllUsers()
        {
            _userService.Setup(us => us.GetAllUsersExceptCurrentUser()).Returns(new List<User>
            {
                _otherUser1,
                _otherUser2,
            }.AsQueryable());

            _userService.Setup(us => us.GetUserWithFriendsAsync()).ReturnsAsync(new User
            {
                ReceievedFriendRequests = new List<Friend>(),
                SentFriendRequests = new List<Friend>()
            });

            var result = await _friendService.GetSuggestedFriendsAsync();

            Assert.That(result, Is.EquivalentTo(new List<User>
            {
                _otherUser1,
                _otherUser2
            }));

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetSuggestedFriends_UserHasAnAprrovedFriend_ReturnListOfSuggestedUsers()
        {
            _userService.Setup(us => us.GetAllUsersExceptCurrentUser()).Returns(new List<User>
            {
                _otherUser1,
                _otherUser2,
                _otherUser3
            }.AsQueryable());

            _currentUser.SentFriendRequests = new List<Friend>()
            {
                _approvedfriend
            };

            _userService.Setup(us => us.GetUserWithFriendsAsync())
                .ReturnsAsync(
                    _currentUser
                );

            //Act
            var result = await _friendService.GetSuggestedFriendsAsync();

            Assert.That(result, Is.EquivalentTo(new List<User>
            {
                _otherUser2,
                _otherUser3,
            }));

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetSuggestedFriends_UserHasUnAprrovedFriend_ReturnListOfSuggestedUsers()
        {
            _userService.Setup(us => us.GetAllUsersExceptCurrentUser()).Returns(new List<User>
            {
                _otherUser1,
                _otherUser2,
                _otherUser3
            }.AsQueryable());

            _currentUser.SentFriendRequests = new List<Friend>()
            {
                _notapprovedfriend
            };

            _userService.Setup(us => us.GetUserWithFriendsAsync())
                .ReturnsAsync(
                    _currentUser
                );

            //Act
            var result = await _friendService.GetSuggestedFriendsAsync();

            Assert.That(result, Is.EquivalentTo(new List<User>
            {
                _otherUser2,
                _otherUser3,
            }));

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetSuggestedFriends_UserHasBothAprrovedAndNotApprovedFriend_ReturnListOfSuggestedUsers()
        {
            _userService.Setup(us => us.GetAllUsersExceptCurrentUser()).Returns(new List<User>
            {
                _otherUser1,
                _otherUser2,
                _otherUser3
            }.AsQueryable());

            _currentUser.SentFriendRequests = new List<Friend>()
            {
                
                _approvedfriend,
                new Friend
                {
                    RequestedOn = DateTime.Now,
                    RequestedById = _currentUser.Id,
                    RequestedToId = _otherUser2.Id,
                    Status = FriendRequestType.None
                }
            };

            _userService.Setup(us => us.GetUserWithFriendsAsync())
                .ReturnsAsync(
                    _currentUser
                );

            //Act
            var result = await _friendService.GetSuggestedFriendsAsync();

            Assert.That(result, Is.EquivalentTo(new List<User>
            {
                _otherUser3
            }));

            Assert.That(result.Count(), Is.EqualTo(1));
        }


        [Test]
        public async Task AddFriendRequest_WhenCalled_AddFriendToRequestedByUserSentFriendRequestsList()
        {
            _currentUser.SentFriendRequests = new List<Friend>();
            _currentUser.ReceievedFriendRequests = new List<Friend>();

            _userService.Setup(us => us.GetUserWithFriendsAsync()).ReturnsAsync(
                _currentUser
            );

            var service = new Mock<IFriendService>();

            await _friendService.AddFriendRequestAsync(_currentUser.Id, _otherUser1.Id);


            _userService.Verify(us => us.GetUserWithFriendsAsync());

            Assert.That(_currentUser.SentFriendRequests.Count, Is.EqualTo(1));
        }


        [Test]
        public async Task FriendRequestExists_RequestIsNotCreated_ReturnFalse()
        {
            var options =  InMemoryDbHelper.SetUpInMemoryDb("Request_Is_Not_Created");

            using (var dbContext = new ReactorDbContext(options))
            {
                dbContext.Set<Friend>();
            }

            var service = new FriendService(new FriendRepository(new ReactorDbContext(options)), _userService.Object);

            var result = await service.FriendRequestExistsAsync(It.IsAny<string>(), It.IsAny<string>());

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task FriendRequestExists_RequestIsCreatedByRequestedByUser_ReturnTrue()
        {
            var options = InMemoryDbHelper.SetUpInMemoryDb("Request_Is_Created_By_RequestedByUser");

            using (var dbContext = new ReactorDbContext(options))
            {
                await dbContext.Set<Friend>().AddRangeAsync(new List<Friend>
                {
                    _notapprovedfriend
                });

                await dbContext.SaveChangesAsync();
            }

            var service = new FriendService(new FriendRepository(new ReactorDbContext(options)), _userService.Object);

            var result = await service.FriendRequestExistsAsync(_currentUser.Id, _otherUser1.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task FriendRequestExists_RequestIsCreatedByRequestedToUser_ReturnTrue()
        {
            var options =InMemoryDbHelper.SetUpInMemoryDb("Request_Is_Created_By_RequestedToUser");

            using (var dbContext = new ReactorDbContext(options))
            {
                await dbContext.Set<Friend>().AddRangeAsync(new List<Friend>
                {
                    _notapprovedfriend
                });

                await dbContext.SaveChangesAsync();
            }

            var service = new FriendService(new FriendRepository(new ReactorDbContext(options)), _userService.Object);

            var result = await service.FriendRequestExistsAsync(_currentUser.Id, _otherUser1.Id);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetApprovedFriends_WhenCalled_ReturnListOfApprovedFriends()
        {
            _currentUser.SentFriendRequests = new List<Friend>()
            {
                _approvedfriend
            };

            _userService.Setup(us => us.GetUserWithFriendsAsync()).ReturnsAsync(
                _currentUser
            );

            var result = await _friendService.GetApprovedFriends();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetFriendRequests_WhenCalled_ReturnListOfFriendRequests()
        {
            _currentUser.ReceievedFriendRequests = new List<Friend>()
            {
                _notapprovedfriend
            };

            _userService.Setup(us => us.GetUserWithFriendsAsync()).ReturnsAsync(
                _currentUser
            );

            var result = await _friendService.GetFriendRequests();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

      
    }
}