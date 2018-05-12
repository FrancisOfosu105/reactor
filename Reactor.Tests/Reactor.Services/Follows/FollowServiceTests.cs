using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Reactor.Core.Domain.Follows;
using Reactor.Core.Repository;
using Reactor.Services.Follows;
using Reactor.Services.Users;

namespace Reactor.Tests.Reactor.Services.Follows
{
    [TestFixture]
    public class FollowServiceTests
    {
        private string _followerId;
        private string _followeeId;
        private Mock<IFollowRepository> _repository;
        private Mock<IUserService> _userService;
        private FollowService _followService;
        private Follow _follow;
        
        private Follow _followee1;
        private Follow _followee2;
        private Follow _follower1;  
        private Follow _follower2;

        [SetUp]
        public void SetUp()
        {
            _followerId = "1";
            _followeeId = "2";

            _repository = new Mock<IFollowRepository>();
            _userService = new Mock<IUserService>();

            _userService.Setup(us => us.GetCurrentUserIdAsync()).ReturnsAsync(_followerId);
            

            _followService = new FollowService(_repository.Object, _userService.Object);
            
            _follow = new Follow
            {
                FollowerId = _followerId,
                FolloweeId = _followeeId
            };
            
            /*List of people following the user*/
            _followee1 = new Follow
            {
                FollowerId = "3",
                FolloweeId = _followerId
            };
            
            _followee2 = new Follow
            {
                FollowerId = "4",
                FolloweeId = _followerId
            };
            
            /*List of people the user will follow*/
            _follower1 = new Follow
            {
                FollowerId = _followerId,
                FolloweeId =  "3"
            };
            
            _follower2 = new Follow
            {
                FollowerId =_followerId ,
                FolloweeId = "4"
            };


            
            
        }

        [Test]
        public void FollowUser_UserIsAlreadyFollowing_ThrowAnInvalidOperationException()
        {
            _repository.Setup(repo => repo.Table).Returns(new List<Follow>
            {
                _follow
            }.AsQueryable());


            Assert.That(async () => await _followService.FollowUserAsync(_followeeId),
                Throws.InvalidOperationException);

            _repository.Verify(repo => repo.AddAsync(It.IsAny<Follow>()), Times.Never);
        }

        [Test]
        public async Task FollowUser_FollowIsNull_AddFollow()
        {
            _repository.Setup(repo => repo.Table).Returns(new List<Follow>
            {
            }.AsQueryable());

            await _followService.FollowUserAsync(It.IsAny<string>());

            _repository.Verify(repo => repo.AddAsync(It.IsAny<Follow>()));
        }
        
        [Test]
        public async Task GetUserFollowers_WhenCalled_ReturnListOfPeopleWhoAreFollowingTheUser()    
        {
            _repository.Setup(repo => repo.Table).Returns(new List<Follow>
            {
                _follow,
                _followee1,
                _followee2
            }.AsQueryable());

            var result = await _followService.GetUserFollowersAsync(_followerId);
            
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EquivalentTo(new List<Follow>(){_followee1,_followee2}));

        }
        
        [Test]
        public async Task GetUserFollowees_WhenCalled_ReturnListOfPeopleTheUserIsFollowing()    
        {
            _repository.Setup(repo => repo.Table).Returns(new List<Follow>
            {
                _follower1,
                _follower2
            }.AsQueryable());

            var result = await _followService.GetUserFolloweesAsync(_followerId);
            
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result, Is.EquivalentTo(new List<Follow>(){_follower1,_follower2}));

        }
    }
}